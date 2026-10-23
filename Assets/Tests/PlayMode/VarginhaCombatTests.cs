using System.Collections;
using Game.Player;
using Game.Varginha;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;

namespace Game.Tests.PlayMode
{
    public class VarginhaCombatTests
    {
        [UnitySetUp] public IEnumerator IsolateFromMenusAndCampaignInputLocks()
        {
            var isolated=SceneManager.CreateScene("Combat_QA_"+System.Guid.NewGuid().ToString("N"));
            SceneManager.SetActiveScene(isolated);
            for(int i=SceneManager.sceneCount-1;i>=0;i--)
            {
                var scene=SceneManager.GetSceneAt(i);
                // The first batch test may still be owned by Unity's bootstrap scene.
                // Unload game scenes and our own fixtures, never the test runner itself.
                if(scene!=isolated&&(scene.name.StartsWith("Combat_QA_")||scene.name.StartsWith("Ato")||scene.name=="Menu_MisterioDeVarginha"))
                    yield return SceneManager.UnloadSceneAsync(scene);
            }
            Time.timeScale=1;VarginhaGameHUD.Instance?.CloseDialogue();yield return null;
        }
        private GameObject _targetObject;

        [TearDown]
        public void TearDown()
        {
            if (_targetObject != null) Object.DestroyImmediate(_targetObject);
            Time.timeScale = 1f;
        }

        [UnityTest]
        public IEnumerator EdelzioCanMoveDuringAttackAndAttackAgainWithoutCooldown()
        {
            _targetObject = new GameObject("Edelzio_Attack_Test");
            _targetObject.AddComponent<CircleCollider2D>();
            var player = _targetObject.AddComponent<EdelzioTopDownController>();
            var attack = _targetObject.AddComponent<VarginhaPlayerAttack>();
            attack.Configure(5f, true); // Configuração antiga não deve restabelecer a recarga.
            Assert.IsTrue(attack.TryAttack());
            Assert.IsFalse(player.IsInputLocked);
            Assert.AreEqual(0f, attack.CooldownRemaining);
            yield return new WaitForSecondsRealtime(.6f);
            Assert.IsFalse(attack.IsAttacking);
            Assert.IsTrue(attack.TryAttack());
        }

        [Test]
        public void MovementReversesAndStopsInAllEightDirections()
        {
            _targetObject = new GameObject("Edelzio_Movement_Test");
            _targetObject.AddComponent<CircleCollider2D>();
            var player = _targetObject.AddComponent<EdelzioTopDownController>();
            var body = player.GetComponent<Rigidbody2D>();
            var flags = System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic;
            var input = typeof(EdelzioTopDownController).GetField("_moveInput", flags);
            var move = typeof(EdelzioTopDownController).GetMethod("Move", flags);
            foreach (var direction in new[] { Vector2.up, Vector2.down, Vector2.left, Vector2.right,
                new Vector2(1, 1), new Vector2(-1, 1), new Vector2(1, -1), new Vector2(-1, -1) })
            {
                input.SetValue(player, direction.normalized);
                move.Invoke(player, null);
                Assert.Greater(Vector2.Dot(body.linearVelocity, direction.normalized), 4f);
                Assert.AreEqual(4.5f, body.linearVelocity.magnitude, .001f);
            }
            input.SetValue(player, Vector2.zero);
            move.Invoke(player, null);
            Assert.AreEqual(Vector2.zero, body.linearVelocity);
            Assert.AreEqual(0f, player.GetComponent<Collider2D>().sharedMaterial.friction);
        }

        [Test]
        public void PunchAtlasHasFourDirectionsAndThreeSixFrameCombos()
        {
            var atlas = Resources.Load<Texture2D>("Varginha/EdelzioPunchV2");
            Assert.IsNotNull(atlas);
            Assert.AreEqual(1152, atlas.width);
            Assert.AreEqual(256, atlas.height);
        }

        private VarginhaPlayerAttack CreateAttacker()
        {
            _targetObject = new GameObject("Edelzio_Punch_Test");
            _targetObject.AddComponent<CircleCollider2D>();
            return _targetObject.AddComponent<VarginhaPlayerAttack>();
        }

        [UnityTest]
        public IEnumerator AllThreeComboContactsDealDamage()
        {
            var attack = CreateAttacker();
            var enemy = new GameObject("Combo_Target");
            try
            {
                enemy.transform.position = new Vector3(.67f, 0, 0);
                enemy.AddComponent<BoxCollider2D>().isTrigger = true;
                enemy.AddComponent<VarginhaCombatTarget>();
                var health = enemy.GetComponent<HealthSystem>();
                health.SetMaxHealth(250f, false);
                Physics2D.SyncTransforms();
                foreach (int step in new[] { 1, 2, 3 })
                {
                    Assert.IsTrue(attack.TryAttack(Vector2.right));
                    Assert.AreEqual(step, attack.ComboStep);
                    float deadline = Time.realtimeSinceStartup + 2f;
                    while (attack.IsAttacking && Time.realtimeSinceStartup < deadline) yield return null;
                    Assert.IsFalse(attack.IsAttacking);
                }
                Assert.AreEqual(250f - 34f * (1f + 1.10f + 1.45f), health.CurrentHealth, .001f,
                    "Enemy invincibility must not swallow the middle punch.");
            }
            finally { Object.DestroyImmediate(enemy); }
        }

        [UnityTest]
        public IEnumerator EarlyQueuedClickChainsExactlyOnePunch()
        {
            var attack = CreateAttacker();
            Assert.IsTrue(attack.TryAttack(Vector2.right));
            attack.QueueAttack(Vector2.left);
            attack.QueueAttack(Vector2.left);
            float deadline = Time.realtimeSinceStartup + 2f;
            while (attack.ComboStep != 2 && Time.realtimeSinceStartup < deadline)
            {
                Assert.IsTrue(attack.IsAttacking, "A queued combo must not leave an idle frame between punches.");
                Assert.IsTrue(_targetObject.GetComponent<VarginhaPlayerSpriteAnimation>().HasActionPose);
                yield return null;
            }
            Assert.AreEqual(2, attack.ComboStep, "Early input must survive the first punch.");
            Assert.AreEqual(Vector2.left, _targetObject.GetComponent<VarginhaPlayerSpriteAnimation>().ActionFacingDirection);
            while (attack.IsAttacking && Time.realtimeSinceStartup < deadline) yield return null;
            yield return null;
            Assert.IsFalse(attack.IsAttacking, "Multiple clicks buffer one punch, not an unlimited chain.");
            Assert.AreEqual(2, attack.ComboStep);
        }
        [UnityTest] public IEnumerator SpamKeepsEveryAuthoredPoseEvenAtTenFramesPerSecond()
        {
            float previous=Time.captureDeltaTime;
            try
            {
                Time.captureDeltaTime=.1f;
                var attack=CreateAttacker();var renderer=_targetObject.GetComponent<SpriteRenderer>();
                var frames=new System.Collections.Generic.HashSet<Sprite>();
                Assert.That(attack.TryAttack(Vector2.right),Is.True);frames.Add(renderer.sprite);
                for(int click=0;click<40;click++)attack.QueueAttack(Vector2.left);
                float deadline=Time.realtimeSinceStartup+8;
                while(attack.ComboStep==1&&attack.IsAttacking&&Time.realtimeSinceStartup<deadline)
                {frames.Add(renderer.sprite);yield return null;}
                Assert.That(frames.Count,Is.EqualTo(6),"A slow frame cannot erase an authored anticipation or recovery pose.");
                Assert.That(attack.ComboStep,Is.EqualTo(2));
                while(attack.IsAttacking&&Time.realtimeSinceStartup<deadline)yield return null;
                Assert.That(attack.IsAttacking,Is.False);Assert.That(attack.ComboStep,Is.EqualTo(2),"Forty clicks still buffer only one follow-up.");
                Assert.That(_targetObject.GetComponent<VarginhaPlayerSpriteAnimation>().HasActionPose,Is.False);
            }
            finally {Time.captureDeltaTime=previous;}
        }
        [UnityTest,Timeout(15000)] public IEnumerator CampaignRendererRestartsItsVisibleCycleForEachBufferedPunch()
        {
            var attack=CreateAttacker();var team=_targetObject.AddComponent<Game.Varginha.Experiment.CampaignTeamEdelzio>();
            var visual=_targetObject.GetComponent<VarginhaPlayerSpriteAnimation>();var renderer=_targetObject.GetComponent<SpriteRenderer>();
            yield return null;Assert.That(attack.TryAttack(Vector2.right),Is.True);attack.QueueAttack(Vector2.left);
            var seen=new[]{new System.Collections.Generic.HashSet<int>(),new System.Collections.Generic.HashSet<int>()};
            float deadline=Time.realtimeSinceStartup+5;
            while(attack.IsAttacking&&Time.realtimeSinceStartup<deadline)
            {
                int step=attack.ComboStep-1,phase=visual.CombatFrame;
                team.SendMessage("LateUpdate");
                int pose=phase switch{0=>0,1 or 2=>1,3=>2,4=>3,_=>0};
                Assert.That(renderer.sprite.name,Is.EqualTo("Team_V4_Punch_"+(step==0?2:1)+"_"+pose),"The renderer follows this punch's phase, not the first punch's clock.");
                seen[step].Add(pose);yield return null;
            }
            Assert.That(attack.IsAttacking,Is.False);Assert.That(seen[0].Count,Is.EqualTo(4));Assert.That(seen[1].Count,Is.EqualTo(4));
            Assert.That(visual.HasActionPose,Is.False);Assert.That(visual.CombatFrame,Is.EqualTo(-1));
        }

        [UnityTest]
        public IEnumerator LockCancelsQueuedPunchWithoutErasingInteractionPose()
        {
            var attack = CreateAttacker();
            var player = _targetObject.GetComponent<EdelzioTopDownController>();
            var animation = _targetObject.GetComponent<VarginhaPlayerSpriteAnimation>();
            attack.TryAttack(Vector2.right);
            attack.QueueAttack(Vector2.right);
            player.SetInputLocked(true);
            animation.SetActionPose("Edelzio_Sit");
            yield return null;
            Assert.IsFalse(attack.IsAttacking);
            Assert.IsTrue(animation.IsSeated, "Attack cleanup must not erase the new interaction.");
            player.SetInputLocked(false);
            yield return null;
            Assert.IsFalse(attack.IsAttacking);
            Assert.IsTrue(attack.TryAttack());
            Assert.AreEqual(1, attack.ComboStep);
        }

        [UnityTest]
        public IEnumerator PunchDamagesOnlyAtContactAndOnlyOnceForMultipleColliders()
        {
            var attack = CreateAttacker();
            var enemy = new GameObject("Punch_Target");
            try
            {
                enemy.transform.position = new Vector3(.67f, 0, 0);
                enemy.AddComponent<BoxCollider2D>().isTrigger = true;
                enemy.AddComponent<CircleCollider2D>().isTrigger = true;
                var target = enemy.AddComponent<VarginhaCombatTarget>();
                var health = enemy.GetComponent<HealthSystem>();
                Physics2D.SyncTransforms();
                attack.TryAttack(Vector2.right);
                Assert.AreEqual(100f, health.CurrentHealth);
                yield return new WaitForSecondsRealtime(.04f);
                Assert.AreEqual(100f, health.CurrentHealth, "Anticipation cannot deal damage.");
                float deadline = Time.realtimeSinceStartup + 2f;
                while (attack.IsAttacking && Time.realtimeSinceStartup < deadline) yield return null;
                Assert.AreEqual(66f, health.CurrentHealth, .001f);
                Assert.AreEqual(1f, Time.timeScale);
            }
            finally { Object.DestroyImmediate(enemy); }
        }

        [UnityTest]
        public IEnumerator WallBlocksPunchAndDisableRestoresHitstop()
        {
            var attack = CreateAttacker();
            var enemy = new GameObject("Punch_Target");
            var wall = new GameObject("Punch_Wall");
            try
            {
                enemy.transform.position = new Vector3(.67f, 0, 0);
                enemy.AddComponent<BoxCollider2D>().isTrigger = true;
                enemy.AddComponent<VarginhaCombatTarget>();
                var health = enemy.GetComponent<HealthSystem>();
                wall.transform.position = new Vector3(.35f, 0, 0);
                wall.AddComponent<BoxCollider2D>().size = new Vector2(.1f, 2f);
                Physics2D.SyncTransforms();
                attack.TryAttack(Vector2.right);
                float deadline = Time.realtimeSinceStartup + 2f;
                while (attack.IsAttacking && Time.realtimeSinceStartup < deadline) yield return null;
                Assert.AreEqual(100f, health.CurrentHealth, "Punches cannot cross walls.");
                Object.DestroyImmediate(wall);
                Physics2D.SyncTransforms();
                attack.TryAttack(Vector2.right);
                deadline = Time.realtimeSinceStartup + 2f;
                while (health.CurrentHealth == 100f && Time.realtimeSinceStartup < deadline) yield return null;
                Assert.Less(health.CurrentHealth, 100f);
                attack.enabled = false;
                Assert.AreEqual(1f, Time.timeScale);
                Assert.IsFalse(attack.IsAttacking);
            }
            finally
            {
                Object.DestroyImmediate(enemy);
                if (wall != null) Object.DestroyImmediate(wall);
            }
        }

        [UnityTest]
        public IEnumerator MinorManifestationTakesDamageOnlyThroughCombatTarget()
        {
            _targetObject = new GameObject("MinorManifestation_Test");
            _targetObject.AddComponent<BoxCollider2D>();
            var health = _targetObject.AddComponent<HealthSystem>();
            var target = _targetObject.AddComponent<VarginhaCombatTarget>();
            target.SetKind(VarginhaCombatTarget.EnemyKind.MinorManifestation);
            yield return null;
            bool connected = target.ReceiveHit(34f, Vector2.right, .075f);
            Assert.IsTrue(connected);
            Assert.AreEqual(66f, health.CurrentHealth, .001f);
        }

        [UnityTest]
        public IEnumerator AncestralEntityRemainsInvulnerable()
        {
            _targetObject = new GameObject("Entity_Manifestation_Test");
            _targetObject.AddComponent<BoxCollider2D>();
            var health = _targetObject.AddComponent<HealthSystem>();
            var target = _targetObject.AddComponent<VarginhaCombatTarget>();
            target.SetKind(VarginhaCombatTarget.EnemyKind.AncestralEntity);
            yield return null;
            Assert.IsFalse(target.ReceiveHit(999f, Vector2.right, .075f));
            Assert.AreEqual(100f, health.CurrentHealth, .001f);
        }

        [UnityTest]
        public IEnumerator CombatCursorInstantiatesWithAttackAndDetectsTarget()
        {
            _targetObject = new GameObject("Edelzio_Cursor_Test");
            _targetObject.AddComponent<CircleCollider2D>();
            var player = _targetObject.AddComponent<EdelzioTopDownController>();
            var attack = _targetObject.AddComponent<VarginhaPlayerAttack>();
            yield return null;

            var cursor = _targetObject.GetComponent<VarginhaCombatCursor>();
            Assert.IsNotNull(cursor, "VarginhaCombatCursor must be attached automatically to the player attack object.");
            Assert.AreEqual(cursor, VarginhaCombatCursor.Instance);

            var enemyGo = new GameObject("Test_Enemy");
            enemyGo.transform.position = new Vector3(2f, 0f, 0f);
            var col = enemyGo.AddComponent<BoxCollider2D>();
            col.size = new Vector2(1f, 1f);
            var combatTarget = enemyGo.AddComponent<VarginhaCombatTarget>();
            yield return null;

            // Invoca a detecção com coordenada próxima ao alvo
            var detectMethod = typeof(VarginhaCombatCursor).GetMethod("DetectTargetAtAim",
                System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            var detected = detectMethod.Invoke(cursor, new object[] { new Vector2(2f, 0.2f) });
            Assert.AreEqual(combatTarget, detected, "Cursor must acquire target when hovering near enemy combat target.");

            Object.DestroyImmediate(enemyGo);
        }
    }
}
