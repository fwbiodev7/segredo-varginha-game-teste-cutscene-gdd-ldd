using System.Collections;
using Game.Varginha;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Game.Tests.PlayMode
{
    public class VarginhaInteractionTests
    {
        private GameObject _root;
        private const System.Reflection.BindingFlags Private = System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic;
        [TearDown] public void Cleanup()
        {
            Time.timeScale = 1f;
            if (_root != null) Object.DestroyImmediate(_root);
        }

        [Test] public void CoffeeSeatingAndWebcamHaveTheirOwnFrames()
        {
            var atlas = Resources.Load<Texture2D>(VarginhaInteractionSprites.ResourcePath);
            Assert.IsNotNull(atlas);
            Assert.IsTrue(atlas.isReadable);
            Assert.AreEqual(FilterMode.Point, atlas.filterMode);
            for (int row = 0; row < 3; row++)
            for (int frame = 0; frame < 4; frame++)
            {
                var sprite = VarginhaInteractionSprites.Frame(row, frame);
                Assert.AreSame(atlas, sprite.texture);
                Assert.Greater(sprite.rect.width, 20);
                if (frame > 0) Assert.IsFalse(sprite.rect.Overlaps(VarginhaInteractionSprites.Frame(row, frame - 1).rect));
            }
            Time.timeScale = 0f;
            Assert.AreNotSame(VarginhaInteractionSprites.Webcam(0f, false), VarginhaInteractionSprites.Webcam(4.4f, false));
            Assert.AreSame(VarginhaInteractionSprites.Frame(2, 3), VarginhaInteractionSprites.Webcam(1f, true));
        }

        [Test] public void MissingNotebookChairIsCreatedOnlyOnce()
        {
            _root = new GameObject("InteractionTest");
            var notebook = new GameObject("Notebook_TI").transform;
            notebook.SetParent(_root.transform);
            notebook.position = new Vector3(-5f, -3.3f);
            var first = VarginhaHouseComposition.EnsureNotebookChair(notebook);
            Assert.AreSame(first, VarginhaHouseComposition.EnsureNotebookChair(notebook));
            Assert.IsNotNull(first.GetComponent<SpriteRenderer>().sprite);
            Assert.IsNotNull(first.GetComponent<Collider2D>());
            Assert.Less(Vector2.Distance(first.transform.position, notebook.position), 1.5f);
        }

        [Test] public void InteractionRowsDoNotCutThroughVisiblePixels()
        {
            var texture = Resources.Load<Texture2D>(VarginhaInteractionSprites.ResourcePath);
            var pixels = texture.GetPixels32();
            for (int row = 0; row < 3; row++)
            for (int frame = 0; frame < 4; frame++)
            {
                Rect rect = VarginhaInteractionSprites.Frame(row, frame).rect;
                foreach (int y in new[] { (int)rect.yMin - 1, (int)rect.yMax })
                for (int x = (int)rect.xMin; x < rect.xMax; x++)
                    if (y >= 0 && y < texture.height)
                        Assert.Less(pixels[y * texture.width + x].a, 64, $"Quadro {row}/{frame} cortado em {x},{y}");
            }
            Assert.Less(VarginhaInteractionSprites.Frame(1, 2).rect.height,
                VarginhaInteractionSprites.Frame(1, 0).rect.height, "Sentar deve reduzir a altura visível.");
        }

        private EdelzioTopDownController CreatePlayer()
        {
            _root = new GameObject("InteractionSessionTest");
            var actor = new GameObject("Player");
            actor.transform.SetParent(_root.transform);
            actor.AddComponent<SpriteRenderer>();
            actor.AddComponent<Rigidbody2D>().gravityScale = 0;
            actor.AddComponent<CircleCollider2D>().radius = .15f;
            var player = actor.AddComponent<EdelzioTopDownController>();
            actor.AddComponent<VarginhaPlayerSpriteAnimation>();
            actor.AddComponent<VarginhaPlayerActionAnimation>();
            return player;
        }

        [UnityTest] public IEnumerator InterruptedCoffeeCanBeRetriedAndCompletesOnlyOnce()
        {
            var player = CreatePlayer();
            var cup = new GameObject("Coffee");
            cup.transform.SetParent(_root.transform);
            cup.transform.position = Vector3.right * .55f;
            var prop = cup.AddComponent<InteractableProp>();
            prop.Configure(PropType.CoffeeOrFood, "Cafe", "", false);
            int completions = 0;
            prop.OnInteracted += _ => completions++;
            prop.Interact(player);
            yield return new WaitForSeconds(.6f);
            var action = player.GetComponent<VarginhaPlayerActionAnimation>();
            action.enabled = false;
            Assert.IsTrue(prop.CanInteract);
            Assert.IsFalse(player.IsInputLocked);
            Assert.AreEqual(0, completions);
            action.enabled = true;
            prop.Interact(player);
            float deadline = Time.realtimeSinceStartup + 5f;
            while (completions == 0 && Time.realtimeSinceStartup < deadline) yield return null;
            Assert.AreEqual(1, completions);
            Assert.IsFalse(prop.CanInteract);
            prop.Interact(player);
            Assert.AreEqual(1, completions);
            Assert.IsFalse(player.IsInputLocked);
        }

        [UnityTest] public IEnumerator NotebookExitRestoresTimePositionCollisionAndAllowsRetry()
        {
            var player = CreatePlayer();
            var notebook = new GameObject("Notebook_TI");
            notebook.transform.SetParent(_root.transform);
            notebook.transform.position = Vector3.down;
            var prop = notebook.AddComponent<InteractableProp>();
            prop.Configure(PropType.NotebookLaptop, "Notebook", "", false);
            var quiz = _root.AddComponent<VarginhaNotebookQuiz>();
            var chair = VarginhaHouseComposition.EnsureNotebookChair(notebook.transform);
            player.transform.position = Vector3.right * .5f;
            Vector2 start = player.transform.position;
            Physics2D.SyncTransforms();
            prop.Interact(player);
            float deadline = Time.realtimeSinceStartup + 4f;
            while (!quiz.IsOpen && Time.realtimeSinceStartup < deadline) yield return null;
            Assert.IsTrue(quiz.IsOpen);
            Assert.AreEqual(0f, Time.timeScale);
            Assert.IsTrue(player.GetComponent<VarginhaPlayerSpriteAnimation>().IsSeated);
            typeof(VarginhaNotebookQuiz).GetMethod("Close", Private).Invoke(quiz, null);
            yield return new WaitForSeconds(1f);
            Assert.AreEqual(1f, Time.timeScale);
            Assert.IsFalse(player.IsInputLocked);
            Assert.IsTrue(prop.CanInteract);
            Assert.Less(Vector2.Distance(start, player.transform.position), .05f);
            Assert.IsFalse(Physics2D.GetIgnoreCollision(player.GetComponent<Collider2D>(), chair.GetComponent<Collider2D>()));
        }

        [UnityTest] public IEnumerator RejectedNotebookOpenDoesNotLeavePlayerSeated()
        {
            var player = CreatePlayer();
            var quiz = _root.AddComponent<VarginhaNotebookQuiz>();
            player.HasDecodedData = true;
            player.GetComponent<VarginhaPlayerActionAnimation>().PlayNotebookSession(null, () => quiz.Open(player, null));
            yield return new WaitForSeconds(2f);
            Assert.IsFalse(quiz.IsOpen);
            Assert.IsFalse(player.IsInputLocked);
            Assert.IsFalse(player.GetComponent<VarginhaPlayerSpriteAnimation>().IsSeated);
        }

        [UnityTest] public IEnumerator DisablingSeatedPlayerClosesNotebookAndRestoresTime()
        {
            var player = CreatePlayer();
            var quiz = _root.AddComponent<VarginhaNotebookQuiz>();
            var action = player.GetComponent<VarginhaPlayerActionAnimation>();
            action.PlayNotebookSession(null, () => quiz.Open(player, null));
            float deadline = Time.realtimeSinceStartup + 4f;
            while (!quiz.IsOpen && Time.realtimeSinceStartup < deadline) yield return null;
            Assert.IsTrue(quiz.IsOpen);
            action.enabled = false;
            Assert.IsFalse(quiz.IsOpen);
            Assert.AreEqual(1f, Time.timeScale);
            Assert.IsFalse(player.IsInputLocked);
        }

        [UnityTest] public IEnumerator InterruptedCrouchRestoresOriginalScale()
        {
            var player = CreatePlayer();
            var action = player.GetComponent<VarginhaPlayerActionAnimation>();
            Vector3 original = new Vector3(1.2f, 1.4f, 1f);
            player.transform.localScale = original;
            action.StartCoroutine(action.CrouchRoutine(1f));
            yield return new WaitForSeconds(.2f);
            Assert.Less(player.transform.localScale.y, original.y);
            action.enabled = false;
            Assert.AreEqual(original, player.transform.localScale);
            Assert.IsFalse(player.IsInputLocked);
        }

        [Test] public void DisabledBackpackAnimationStillAllowsPickup()
        {
            var player = CreatePlayer();
            var backpack = new GameObject("Backpack");
            backpack.transform.SetParent(_root.transform);
            backpack.AddComponent<SpriteRenderer>();
            backpack.AddComponent<BoxCollider2D>();
            backpack.AddComponent<BackpackPickupAnimation>().enabled = false;
            var prop = backpack.AddComponent<InteractableProp>();
            prop.Configure(PropType.Backpack, "Mochila", "", false);
            prop.Interact(player);
            Assert.IsTrue(player.HasBackpack);
            Assert.IsFalse(backpack.GetComponent<SpriteRenderer>().enabled);
            Assert.IsFalse(backpack.GetComponent<Collider2D>().enabled);
            Assert.IsFalse(prop.CanInteract);
        }

        [UnityTest] public IEnumerator ExternallyStartedCrouchStaysRestoredAfterDisable()
        {
            var player = CreatePlayer();
            var action = player.GetComponent<VarginhaPlayerActionAnimation>();
            Vector3 original = player.transform.localScale;
            player.StartCoroutine(action.CrouchRoutine(.3f));
            yield return new WaitForSeconds(.05f);
            action.enabled = false;
            action.enabled = true;
            yield return new WaitForSeconds(.5f);
            Assert.AreEqual(original, player.transform.localScale);
            Assert.IsFalse(player.IsInputLocked);
        }

        [Test] public void DisabledSeatAnimationDoesNotLockPlayerOrConsumeSeat()
        {
            var player = CreatePlayer();
            player.GetComponent<VarginhaPlayerActionAnimation>().enabled = false;
            var seat = new GameObject("Seat");
            seat.transform.SetParent(_root.transform);
            var prop = seat.AddComponent<InteractableProp>();
            prop.Configure(PropType.ChurchSeat, "Banco", "", false);
            prop.Interact(player);
            Assert.IsFalse(player.IsInputLocked);
            Assert.IsTrue(prop.CanInteract);
        }

        [Test] public void SittingPoseUsesBentKneesInsteadOfStandingFrame()
        {
            var player = CreatePlayer();
            player.GetComponent<VarginhaPlayerSpriteAnimation>().SetActionPose("Edelzio_Sit");
            Assert.AreSame(VarginhaSeatedSprites.Frame(0, 2), player.GetComponent<SpriteRenderer>().sprite);
        }

        [TestCase(0)][TestCase(1)][TestCase(2)][TestCase(3)]
        public void SeatedFramesPreserveFeetAndHaveBentKnees(int direction)
        {
            var standing = VarginhaSeatedSprites.Frame(direction,0);
            var seated = VarginhaSeatedSprites.Frame(direction,2);
            Assert.IsNotNull(standing);
            Assert.IsNotNull(seated);
            Assert.Less(seated.bounds.size.y,standing.bounds.size.y);
            Assert.That(seated.bounds.min.y,Is.EqualTo(standing.bounds.min.y).Within(.001f));
            Assert.AreEqual(FilterMode.Point,seated.texture.filterMode);
        }

        [Test] public void SeatedPoseUsesTheSelectedDirection()
        {
            var player=CreatePlayer();
            var animation=player.GetComponent<VarginhaPlayerSpriteAnimation>();
            animation.SetSeatingFacing(Vector2.left);
            animation.SetSeatingFrame(2);
            Assert.AreEqual(Vector2.left,animation.ActionFacingDirection);
            Assert.AreSame(VarginhaSeatedSprites.Frame(1,2),player.GetComponent<SpriteRenderer>().sprite);
        }

        [UnityTest] public IEnumerator ClassroomETAttacksCaptiveCageAndRescueStopsThePressure()
        {
            var player=CreatePlayer();
            player.transform.position=new Vector3(0,-12);
            VarginhaEnvironmentArt.EnsureSchool(_root.transform);
            var captive=new GameObject("Refem_Yasmin");
            captive.transform.SetParent(_root.transform);
            captive.transform.position=VarginhaClassroomMap.StudentPositions[0];
            captive.AddComponent<SpriteRenderer>();
            var student=captive.AddComponent<VarginhaStudentHostage>();
            student.Configure("Yasmin",Color.white);
            var invader=new GameObject("ETThreatTest");
            invader.transform.SetParent(_root.transform);
            invader.transform.position=VarginhaClassroomMap.EnemyPositions[0];
            invader.AddComponent<SpriteRenderer>();
            invader.AddComponent<VarginhaCombatEnemy>();
            invader.AddComponent<VarginhaClassroomPressure>();
            Physics2D.SyncTransforms();
            float deadline=Time.realtimeSinceStartup+4f;
            while(!student.IsCageUnderAttack && Time.realtimeSinceStartup<deadline) yield return null;
            Assert.IsTrue(student.IsCageUnderAttack,"ET deve disparar contra a jaula enquanto Edelzio está longe.");
            Assert.IsTrue(student.IsCaged);
            var car=new GameObject("Car");
            car.transform.SetParent(_root.transform);
            student.ReleaseTo(car.transform,0);
            Assert.IsFalse(student.IsCageUnderAttack);
        }

        [TestCase("Edelzio", "Edelzio")]
        [TestCase("Padre Fábio", "PadreFabio")]
        [TestCase("Fábio", "Fabio")]
        [TestCase("Ana Tavares", "AnaTavares")]
        [TestCase("Luis Miguel Messias", "LuisMiguelMessias")]
        public void PortraitUsesTheSpeakersCurrentAtlas(string speaker, string asset)
        {
            var portrait = VarginhaDialoguePortraits.ForSpeaker(speaker);
            if (asset == "PadreFabio")
            {
                var worldSprite = VarginhaReferenceSprites.PadreFabio();
                Assert.AreSame(worldSprite.texture, portrait.texture);
                Assert.Less(portrait.rect.height, worldSprite.rect.height);
            }
            else Assert.AreSame(Resources.Load<Texture2D>("Varginha/Allies/" + asset), portrait.texture);
        }

        [UnityTest] public IEnumerator InterruptedCoffeeRestoresCupAndMovement()
        {
            _root = new GameObject("CoffeeActionTest");
            _root.AddComponent<SpriteRenderer>();
            _root.AddComponent<Rigidbody2D>().gravityScale = 0f;
            _root.AddComponent<CircleCollider2D>();
            var player = _root.AddComponent<EdelzioTopDownController>();
            _root.AddComponent<VarginhaPlayerSpriteAnimation>();
            var action = _root.AddComponent<VarginhaPlayerActionAnimation>();
            var cup = new GameObject("Cup");
            cup.transform.SetParent(_root.transform);
            var renderer = cup.AddComponent<SpriteRenderer>();
            renderer.sprite = VarginhaPixelArtSprites.Create("Coffee_Hot", Color.white);
            int completions = 0;
            action.PlayDrinkCoffee(cup.transform, () => completions++);
            yield return new WaitForSeconds(.6f);
            action.enabled = false;
            yield return null;
            Assert.IsTrue(renderer.enabled);
            Assert.IsFalse(player.IsScriptedMotion);
            Assert.IsFalse(player.GetComponent<VarginhaPlayerSpriteAnimation>().IsDrinking);
            Assert.AreEqual(0, completions, "Interromper não deve conceder a cura nem consumir o café.");
        }
    }
}
