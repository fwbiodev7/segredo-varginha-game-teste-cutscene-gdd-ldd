using System.Collections;
using System.IO;
using Game.Varginha;
using Game.Varginha.Experiment;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace Game.Tests.PlayMode
{
    public class CampaignChaptersTests : InputTestFixture
    {
        public override void Setup(){VarginhaInputActions.Shutdown();base.Setup();}
        public override void TearDown(){VarginhaInputActions.Shutdown();base.TearDown();}
        private string _previousStory, _previousMemory;
        private static IEnumerator HoldKey(Keyboard keyboard,Key key,float seconds)
        {
            float end=Time.time+seconds;
            while(Time.time<end){InputSystem.QueueStateEvent(keyboard,new KeyboardState(key));yield return null;}
            InputSystem.QueueStateEvent(keyboard,new KeyboardState());yield return null;
        }
        private static Bounds VisibleBounds(SpriteRenderer renderer)
        {
            // Check authored opaque pixels, excluding the transparent animation-cell padding.
            var sprite=renderer.sprite;if(!sprite.texture.isReadable)return renderer.bounds;var pixels=sprite.texture.GetPixels32();
            int left=(int)sprite.rect.xMax,right=(int)sprite.rect.xMin,bottom=(int)sprite.rect.yMax,top=(int)sprite.rect.yMin;
            for(int y=(int)sprite.rect.yMin;y<(int)sprite.rect.yMax;y++)for(int x=(int)sprite.rect.xMin;x<(int)sprite.rect.xMax;x++)
                if(pixels[y*sprite.texture.width+x].a>128){left=Mathf.Min(left,x);right=Mathf.Max(right,x+1);bottom=Mathf.Min(bottom,y);top=Mathf.Max(top,y+1);}
            Vector2 a=(new Vector2(left,bottom)-sprite.rect.position-sprite.pivot)/sprite.pixelsPerUnit;
            Vector2 b=(new Vector2(right,top)-sprite.rect.position-sprite.pivot)/sprite.pixelsPerUnit;
            if(renderer.flipX){a.x=-a.x;b.x=-b.x;}
            var p=renderer.transform.TransformPoint(a);var q=renderer.transform.TransformPoint(b);
            var bounds=new Bounds();bounds.SetMinMax(Vector3.Min(p,q),Vector3.Max(p,q));return bounds;
        }
        [SetUp] public void PreserveSaves()
        {
            _previousStory = File.Exists(CampaignStorySave.Path) ? File.ReadAllText(CampaignStorySave.Path) : null;
            _previousMemory = File.Exists(CampaignMemorySave.Path) ? File.ReadAllText(CampaignMemorySave.Path) : null;
        }
        [TearDown] public void RestoreSaves()
        {
            Time.timeScale = 1;
            if (_previousStory != null) File.WriteAllText(CampaignStorySave.Path, _previousStory); else if (File.Exists(CampaignStorySave.Path)) File.Delete(CampaignStorySave.Path);
            if (_previousMemory != null) File.WriteAllText(CampaignMemorySave.Path, _previousMemory); else if (File.Exists(CampaignMemorySave.Path)) File.Delete(CampaignMemorySave.Path);
        }
        [UnityTest] public IEnumerator ChildhoodFeetRespectWallsAndPassThroughTheExistingDoor()
        {
            float eastWall=CampaignIllustratedMaps.Get(1).Position(880,0).x;
            CampaignMemorySave.Write(new CampaignMemory { openingSeen = true, x = eastWall-.7f, y = 3 });
            yield return SceneManager.LoadSceneAsync(VarginhaCampaignPhase1.SceneName); yield return null; yield return null;
            var actor = Object.FindAnyObjectByType<EdelzioTopDownController>();
            var keyboard = InputSystem.AddDevice<Keyboard>();
            Press(keyboard.dKey); yield return new WaitForSeconds(.65f); Release(keyboard.dKey);
            Assert.That(actor.transform.position.x, Is.LessThan(eastWall), "The child cannot cross the east wall.");
            var body = actor.GetComponent<Rigidbody2D>(); body.position = new Vector2(eastWall-.7f, 0); body.linearVelocity = Vector2.zero;
            Press(keyboard.dKey); yield return new WaitForSeconds(.9f); Release(keyboard.dKey);
            Assert.That(actor.transform.position.x, Is.GreaterThan(eastWall+1), "The east portal must remain passable.");
            Assert.That(actor.GetComponent<CircleCollider2D>().radius, Is.LessThan(.3f), "The collision shape follows the child's feet.");
            yield return SceneManager.LoadSceneAsync("Menu_MisterioDeVarginha");
        }
        [UnityTest] public IEnumerator MovingCharactersHaveShadowsAndPauseReleasesItsSmallFrozenFrame()
        {
            yield return SceneManager.LoadSceneAsync(CampaignStorySave.Scene(4));
            yield return new WaitForSeconds(3.3f);
            var actor=Object.FindAnyObjectByType<EdelzioTopDownController>();
            Assert.That(actor.GetComponent<CampaignCharacterShadow>(),Is.Not.Null);
            foreach(var pupil in Object.FindObjectsByType<VarginhaStudentAnimation>())
                Assert.That(pupil.GetComponent<CampaignCharacterShadow>(),Is.Not.Null,pupil.name);
            var camera=Camera.main;var target=camera.targetTexture;
            VarginhaCampaignStage.Active.TogglePause();
            Assert.That(Time.timeScale,Is.Zero);
            Assert.That(camera.targetTexture,Is.SameAs(target),"Pause capture restores the live camera target.");
            var cinematic=Object.FindAnyObjectByType<CampaignCinematics>();
            var field=typeof(CampaignCinematics).GetField("_pauseFrame",System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Instance);
            var frame=(RenderTexture)field.GetValue(cinematic);
            Assert.That(frame,Is.Not.Null);Assert.That(frame.height,Is.EqualTo(90));
            Assert.That((float)frame.width/frame.height,Is.EqualTo((float)Screen.width/Screen.height).Within(.02f),"Pause must cover wide displays without clear side strips.");
            VarginhaCampaignStage.Active.TogglePause();
            Assert.That(Time.timeScale,Is.EqualTo(1));Assert.That(field.GetValue(cinematic),Is.Null);
            CampaignStorySave.GoTo(4);Assert.That(CampaignCinematics.IsTransitioning,Is.True);
            float until=Time.realtimeSinceStartup+5;
            while(CampaignCinematics.IsTransitioning&&Time.realtimeSinceStartup<until)yield return null;
            Assert.That(SceneManager.GetActiveScene().name,Is.EqualTo(CampaignStorySave.Scene(4)));
            Assert.That(CampaignCinematics.IsTransitioning,Is.False);Assert.That(Time.timeScale,Is.EqualTo(1));
            yield return SceneManager.LoadSceneAsync("Menu_MisterioDeVarginha");
        }
        [UnityTest]public IEnumerator AdultHouseFeetBlockFurnitureAndWallsWithCorrectDepthAndLocalLight()
        {
            CampaignStorySave.Write(new CampaignStory {phase=2});
            yield return SceneManager.LoadSceneAsync(CampaignStorySave.Scene(2));yield return new WaitForSeconds(3.3f);
            var actor=Object.FindAnyObjectByType<EdelzioTopDownController>();var body=actor.GetComponent<Rigidbody2D>();
            var feet=actor.GetComponent<CircleCollider2D>();var renderer=actor.GetComponent<SpriteRenderer>();
            var keyboard=InputSystem.AddDevice<Keyboard>();
            var furniture=GameObject.Find("Mapa_Campanha/02_Mobilia_Colisoes/Fridge_Kitchen");
            var fridge=furniture.GetComponent<BoxCollider2D>();var fridgeGroup=furniture.GetComponent<UnityEngine.Rendering.SortingGroup>();
            Assert.That(actor.GetComponent<VarginhaWorldDepth>(),Is.Not.Null,"The original adult actor must participate in depth sorting.");
            foreach(var direction in new[]{Vector2.left,Vector2.up,Vector2.down})
            {
                body.position=(Vector2)fridge.bounds.center+direction*(direction.x!=0?fridge.bounds.extents.x+.6f:fridge.bounds.extents.y+.6f)-feet.offset;
                body.linearVelocity=Vector2.zero;yield return new WaitForFixedUpdate();yield return null;
                var key=direction.x<0?Key.D:direction.y>0?Key.S:Key.W;
                yield return HoldKey(keyboard,key,.6f);yield return new WaitForFixedUpdate();yield return null;
                Assert.That(feet.Distance(fridge).distance,Is.GreaterThanOrEqualTo(-.01f),"Refrigerator blocks feet from "+direction+" within the physics solver contact tolerance.");
                int actorOrder=actor.GetComponent<UnityEngine.Rendering.SortingGroup>().sortingOrder;
                Assert.That(direction.y<0?actorOrder>fridgeGroup.sortingOrder:direction.y>0?actorOrder<fridgeGroup.sortingOrder:true,Is.True,"Actor covers furniture in front and is occluded behind.");
            }
            foreach(string name in new[]{"Office_Chair","Kitchen_Chair_Left","Kitchen_Chair_Right","Kitchen_Chair_Center"})
            {
                var chair=GameObject.Find("Mapa_Campanha/02_Mobilia_Colisoes/"+name).GetComponent<BoxCollider2D>();
                body.position=(Vector2)chair.bounds.center+Vector2.down*(chair.bounds.extents.y+.6f)-feet.offset;body.linearVelocity=Vector2.zero;
                yield return new WaitForFixedUpdate();yield return HoldKey(keyboard,Key.W,.6f);yield return new WaitForFixedUpdate();
                Assert.That(feet.Distance(chair).distance,Is.GreaterThanOrEqualTo(-.01f),name+" has its own solid floor contact.");
            }
            var wall=GameObject.Find("Mapa_Campanha/01_Planta_Paredes_Divisoes/Parede_6").GetComponent<BoxCollider2D>();
            body.position=new Vector2(wall.bounds.min.x-.6f,wall.bounds.center.y)-feet.offset;body.linearVelocity=Vector2.zero;
            yield return new WaitForFixedUpdate();yield return HoldKey(keyboard,Key.D,.6f);yield return new WaitForFixedUpdate();
            Assert.That(feet.Distance(wall).distance,Is.GreaterThanOrEqualTo(-.01f),"Bedroom wall blocks real movement within the physics solver contact tolerance.");
            var block=new MaterialPropertyBlock();body.position=CampaignIllustratedMaps.Get(2).Position(280,290)-feet.offset;
            yield return new WaitForSeconds(.25f);renderer.GetPropertyBlock(block);var shade=block.GetColor("_SceneTint");
            body.position=CampaignIllustratedMaps.Get(2).Position(115,335)-feet.offset;
            yield return new WaitForSeconds(.25f);renderer.GetPropertyBlock(block);var warm=block.GetColor("_SceneTint");
            Assert.That(renderer.sharedMaterial.shader.name,Is.EqualTo("Varginha/IllustratedActorLighting"));
            Assert.That(warm.r-shade.r,Is.GreaterThan(.12f),"The nearby bedside lamp visibly changes the player's illumination.");
            yield return SceneManager.LoadSceneAsync("Menu_MisterioDeVarginha");
        }
        [UnityTest]public IEnumerator PartitionEndsProtectTheWholeBodyAndChildFridgeWallHasNoInvisibleOpening()
        {
            CampaignMemorySave.Write(new CampaignMemory{openingSeen=true});
            yield return SceneManager.LoadSceneAsync(VarginhaCampaignPhase1.SceneName);yield return new WaitForSeconds(.3f);
            var actor=Object.FindAnyObjectByType<EdelzioTopDownController>();var feet=actor.GetComponent<CircleCollider2D>();var keyboard=InputSystem.AddDevice<Keyboard>();
            var data=CampaignIllustratedMaps.Get(1);actor.GetComponent<Rigidbody2D>().position=data.Position(437,730)-feet.offset;
            Press(keyboard.aKey);yield return new WaitForSeconds(.8f);Release(keyboard.aKey);yield return null;
            Assert.That(VisibleBounds(actor.GetComponent<SpriteRenderer>()).min.x,Is.GreaterThanOrEqualTo(data.Position(394,0).x-.04f),"The child's visible body cannot enter the lower wall beside the refrigerator.");
            CampaignStorySave.Write(new CampaignStory{phase=2});yield return SceneManager.LoadSceneAsync(CampaignStorySave.Scene(2));yield return new WaitForSeconds(3.3f);
            actor=Object.FindAnyObjectByType<EdelzioTopDownController>();feet=actor.GetComponent<CircleCollider2D>();
            var wall=GameObject.Find("Mapa_Campanha/01_Planta_Paredes_Divisoes/Parede_7").GetComponent<BoxCollider2D>();
            actor.GetComponent<Rigidbody2D>().position=new Vector2(wall.bounds.center.x,wall.bounds.min.y-2)-feet.offset;
            Press(keyboard.wKey);yield return new WaitForSeconds(.9f);Release(keyboard.wKey);yield return null;
            Assert.That(VisibleBounds(actor.GetComponent<SpriteRenderer>()).max.y,Is.LessThanOrEqualTo(wall.bounds.min.y+.12f),"Walking north cannot put the visible torso inside the partition end.");
            yield return SceneManager.LoadSceneAsync("Menu_MisterioDeVarginha");
        }
        [UnityTest] [Category("HudPuzzles")] public IEnumerator OptionalHintsPreserveProgressAndReleaseControlsAcrossChapters()
        {
            var keyboard=InputSystem.AddDevice<Keyboard>();
            foreach(int phase in new[]{2,10,18})
            {
                var story=new CampaignStory{phase=phase};story.expansion.workshopParked=true;
                CampaignStorySave.Write(story);yield return SceneManager.LoadSceneAsync(CampaignStorySave.Scene(phase));yield return new WaitForSeconds(3.3f);
                var actor=Object.FindAnyObjectByType<EdelzioTopDownController>();
                if(phase==2)Assert.That(VarginhaCampaignStage.Active.OpenHints(),Is.True);
                if(phase==10){CampaignExpansionController.Active.ClosePanel();Assert.That(CampaignExpansionController.Active.OpenHints(),Is.True);}
                if(phase==18){CampaignContinuationController.Active.CloseMessage();Assert.That(CampaignContinuationController.Active.OpenHints(),Is.True);}
                yield return null;Assert.That(actor.IsInputLocked,Is.True);
                Assert.That(CampaignStorySave.Load().continuation.solved[6],Is.False,"Reading hints cannot complete the chambers.");
                InputSystem.QueueStateEvent(keyboard,new KeyboardState(Key.Escape));yield return null;
                InputSystem.QueueStateEvent(keyboard,new KeyboardState());yield return null;
                Assert.That(actor.IsInputLocked,Is.False,"Closing optional hints returns control in phase "+phase);
            }
            yield return SceneManager.LoadSceneAsync("Menu_MisterioDeVarginha");
        }
        private static IEnumerator PrepareAdultHouse(VarginhaCampaignStage stage)
        {
            var actor=Object.FindAnyObjectByType<EdelzioTopDownController>();
            var plan=CampaignMapPlan.Create(2);
            void At(string id)
            {
                actor.transform.position=plan.points.Find(p=>p.id==id).position+Vector2.up*.58f;
                actor.GetComponent<Rigidbody2D>().linearVelocity=Vector2.zero;Physics2D.SyncTransforms();
            }
            stage.Interact("box");stage.CloseDialogue();
            Assert.That(stage.Progress.boxFound,Is.False,"The childhood box cannot skip the adult opening.");
            foreach(var id in new[]{"wash","food","work"})
            {
                At(id);stage.Interact(id);
                float end=Time.realtimeSinceStartup+9;
                while(stage.IsActing&&Time.realtimeSinceStartup<end)yield return null;
                stage.CloseDialogue();yield return null;
            }
            Assert.That(stage.Progress.routine&7,Is.EqualTo(7),"All three authored preparation animations complete.");
            var layout=CampaignIllustratedMaps.Get(2);
            actor.transform.position=layout.Position(749,600)+Vector2.up*.58f;Physics2D.SyncTransforms();
            stage.Interact("counter");
            yield return new WaitForSeconds(.8f);
            stage.TogglePause();float px=actor.transform.position.x;
            yield return new WaitForSecondsRealtime(.15f);
            Assert.That(actor.transform.position.x,Is.EqualTo(px));Assert.That(stage.IsActing,Is.True);
            stage.TogglePause();
            float deadline=Time.realtimeSinceStartup+6;
            while(stage.IsActing&&Time.realtimeSinceStartup<deadline)yield return null;
            Assert.That(stage.Progress.house.KeyOnCounter,Is.True);
            Assert.That(GameObject.Find("Chave_Fusca_Balcao").GetComponent<SpriteRenderer>().enabled,Is.True);
            var counterGroup=GameObject.Find("Mapa_Campanha/02_Mobilia_Colisoes/CoffeeTable_Kitchen").GetComponent<UnityEngine.Rendering.SortingGroup>();
            Assert.That(GameObject.Find("Chave_Fusca_Balcao").GetComponent<UnityEngine.Rendering.SortingGroup>().sortingOrder,Is.GreaterThan(counterGroup.sortingOrder),"The key must render on top of its coffee table, not behind the furniture.");
            stage.Save();
            yield return SceneManager.LoadSceneAsync(CampaignStorySave.Scene(2));yield return new WaitForSeconds(3.3f);
            stage=VarginhaCampaignStage.Active;actor=Object.FindAnyObjectByType<EdelzioTopDownController>();
            Assert.That(stage.Progress.house.KeyOnCounter,Is.True,"Reload restores the key on the counter.");
            stage.Interact("bag");Assert.That(stage.IsActing,Is.False,"Bag pickup starts only after the door reminder.");
            Assert.That(stage.Progress.house.Missing,Is.False);
            stage.Interact("exitclue");stage.CloseDialogue();
            Assert.That(stage.Progress.house.bagReminder,Is.True);
            Assert.That(stage.transform.Find("Porta_Saida_Condicional").GetComponent<BoxCollider2D>().enabled,Is.True);
            At("bag");stage.Interact("bag");deadline=Time.realtimeSinceStartup+9;
            while(stage.IsActing&&Time.realtimeSinceStartup<deadline)yield return null;
            stage.CloseDialogue();
            Assert.That(stage.Progress.house.Missing,Is.True);
            Assert.That(GameObject.Find("Chave_Fusca_Balcao").GetComponent<SpriteRenderer>().enabled,Is.False);
            Assert.That(CampaignStorySave.Load().house.Missing,Is.True);
            stage.Interact("box");stage.CloseDialogue();Assert.That(stage.Progress.boxFound,Is.False);
            foreach(string clue in new[]{"counter","exitclue","paperclue"}){stage.Interact(clue);stage.CloseDialogue();}
            Assert.That(stage.Progress.house.clues,Is.EqualTo(7));
            Assert.That(stage.SubmitKeyConclusion(0),Is.False);
            Assert.That(stage.SubmitKeyConclusion(1),Is.True);stage.CloseDialogue();
            At("box");stage.Interact("box");deadline=Time.realtimeSinceStartup+5;
            while(stage.IsActing&&Time.realtimeSinceStartup<deadline)yield return null;
            Assert.That(stage.Progress.boxFound,Is.True);
        }
        [UnityTest] [Category("HudPuzzles")] public IEnumerator HouseDriveSchoolAndCodeFormAPlayableSavedCampaign()
        {
            // Include the childhood wall/door check in the full campaign run as well.
            yield return ChildhoodFeetRespectWallsAndPassThroughTheExistingDoor();
            CampaignStorySave.Write(new CampaignStory { phase = 2 });
            yield return SceneManager.LoadSceneAsync(CampaignStorySave.Scene(2)); yield return new WaitForSeconds(3.2f);
            var stage = VarginhaCampaignStage.Active;
            yield return PrepareAdultHouse(stage);
            stage=VarginhaCampaignStage.Active;
            Assert.That(stage.Progress.boxFound, Is.True, "The key investigation opens the box after the evidence has been interpreted.");
            stage.FinishCutscene();
            Assert.That(stage.SubmitPages(), Is.False);
            stage.Progress.pages = new[] { 0, 1, 2 }; Assert.That(stage.SubmitPages(), Is.True); stage.FinishCutscene();
            var backpackActor = Object.FindAnyObjectByType<EdelzioTopDownController>();
            Assert.That(stage.Progress.routine, Is.EqualTo(15), "Preparing for school teaches the existing actions.");
            Assert.That(stage.OpenBackpack(), Is.True, "Backpack open: has=" + backpackActor.HasBackpack + ", locked=" + backpackActor.IsInputLocked + ", dialogue=" + VarginhaGameHUD.Instance.IsDialogueOpen + ", scale=" + Time.timeScale);
            yield return null;
            Assert.That(VarginhaGameHUD.Instance.IsInventoryOpen, Is.True);
            VarginhaGameHUD.Instance.CloseBackpack(); yield return null;
            Assert.That(VarginhaGameHUD.Instance.IsInventoryOpen, Is.False);
            yield return new WaitForSeconds(.1f);
            Assert.That(Object.FindAnyObjectByType<EdelzioTopDownController>().GetComponent<SpriteRenderer>().sprite.name, Does.StartWith("Team_"), "The supplied character remains visible after closing the original backpack.");
            Assert.That(stage.Progress.MapFragments, Is.EqualTo(1));
            Assert.That(Object.FindAnyObjectByType<EdelzioTopDownController>().HasFuscaKey, Is.True);
            stage.Interact("car");
            float deadline=Time.realtimeSinceStartup+40;
            while((VarginhaCampaignStage.Active==null||VarginhaCampaignStage.Active.phase!=4||CampaignCinematics.IsTransitioning)&&Time.realtimeSinceStartup<deadline)yield return null;
            yield return new WaitForSeconds(3.3f);
            stage = VarginhaCampaignStage.Active;
            Assert.That(stage.phase,Is.EqualTo(4));Assert.That(stage.Progress.arrival,Is.True);
            Assert.That(Object.FindObjectsByType<VarginhaCombatEnemy>(), Is.Empty);
            Assert.That(GameObject.Find("Renan_Industrial_Campanha"), Is.Not.Null);
            Assert.That(GameObject.Find("Mapa_Campanha/01_Planta_Paredes_Divisoes/Arte_Integrada_0").GetComponent<SpriteRenderer>().sprite.texture.name, Is.EqualTo("School"), "The supplied real school frontage must be present in the campaign map.");
            Assert.That(Object.FindObjectsByType<CampaignSchoolLife>().Length, Is.EqualTo(9), "All original students have classroom activities.");
            stage.Interact("student:0"); stage.CloseDialogue();
            Assert.That(stage.Progress.studentsTalked, Is.EqualTo(1));
            stage.Interact("renan"); stage.CloseDialogue(); stage.Interact("notebook");
            Assert.That(stage.Progress.school.documentsRecovered,Is.True,"The documents must travel from the house box, without being collected again at school.");
            for(int id=0;id<6;id++){Assert.That(stage.OpenSchoolDocument(id),Is.True);stage.CloseSchoolDocument();}
            stage.SwapSchoolDocuments(0,1);stage.SwapSchoolDocuments(1,2);Assert.That(stage.SubmitSchoolStep(),Is.True);
            foreach(int id in new[]{0,1,3,4,6,7})stage.HighlightSchoolPassage(id);Assert.That(stage.SubmitSchoolStep(),Is.True);
            for(int id=0;id<3;id++)stage.LinkSchoolNote(id,id);Assert.That(stage.SubmitSchoolStep(),Is.True);
            stage.OpenSchoolDocument(6);stage.CloseSchoolDocument();stage.ChooseSchoolDestination(1);Assert.That(stage.SubmitSchoolStep(),Is.True);
            Assert.That(stage.Progress.renanConfirmed, Is.True); stage.Save();
            yield return SceneManager.LoadSceneAsync("Menu_MisterioDeVarginha");
            VarginhaCampaignPhase1.StartCampaign(true); yield return new WaitForSeconds(3.2f);
            Assert.That(VarginhaCampaignStage.Active.phase, Is.EqualTo(4));
            Assert.That(VarginhaCampaignStage.Active.Progress.MapFragments, Is.EqualTo(2));
            Assert.That(VarginhaCampaignStage.Active.Progress.renanConfirmed, Is.True);
            yield return SceneManager.LoadSceneAsync("Menu_MisterioDeVarginha");
        }
    }
}
