using System.Collections;
using Game.Varginha;
using Game.Varginha.Experiment;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.InputSystem;
using System.IO;

namespace Game.Tests.PlayMode
{
    public class VarginhaCampaignTests : InputTestFixture
    {
        public override void Setup(){Game.Varginha.VarginhaInputActions.Shutdown();base.Setup();}
        public override void TearDown(){Game.Varginha.VarginhaInputActions.Shutdown();base.TearDown();}
        private string _memoryBefore,_storyBefore;
        [SetUp]public void PreserveSaves()
        {
            _memoryBefore=File.Exists(CampaignMemorySave.Path)?File.ReadAllText(CampaignMemorySave.Path):null;
            _storyBefore=File.Exists(CampaignStorySave.Path)?File.ReadAllText(CampaignStorySave.Path):null;
        }
        [UnityTest]public IEnumerator AlienBurstKnocksTheChildBackAndLeavesHimLyingWithNoMemory()
        {
            CampaignMemorySave.Write(new CampaignMemory());
            yield return SceneManager.LoadSceneAsync(VarginhaCampaignPhase1.SceneName);
            yield return null;yield return null;
            var phase=VarginhaCampaignPhase1.Active;phase.FinishOpening();phase.DeliverControl();phase.TriggerPowerFailure();
            var actor=Object.FindAnyObjectByType<EdelzioTopDownController>();
            actor.GetComponent<Rigidbody2D>().position=new Vector2(14,0);
            yield return new WaitForFixedUpdate();yield return null;yield return null;
            var encounter=Object.FindAnyObjectByType<CampaignFlashEncounter>();
            Assert.That(encounter,Is.Not.Null);
            Assert.That(actor.IsInputLocked&&actor.IsScriptedMotion,Is.True);
            yield return new WaitForSeconds(.48f);
            Assert.That(encounter.Burst.enabled,Is.True);
            Assert.That(encounter.Burst.sprite.texture.name,Is.EqualTo("AlienExplosion"));
            Assert.That(encounter.Burst.bounds.size.x,Is.GreaterThan(7));
            phase.TogglePause();var paused=actor.transform.position;
            yield return new WaitForSecondsRealtime(.12f);
            Assert.That(actor.transform.position,Is.EqualTo(paused));phase.TogglePause();
            yield return new WaitForSeconds(1.15f);
            Assert.That(encounter.HasHitHead&&encounter.IsLying,Is.True);
            Assert.That(actor.transform.position.x,Is.LessThan(12.1f));
            Assert.That(CampaignMapPlan.Create(1).IsClear(actor.transform.position,.22f),Is.True);
            var sprite=actor.GetComponent<SpriteRenderer>().sprite;
            Assert.That(sprite.name,Does.EndWith("unconscious"));
            Assert.That(sprite.bounds.size.x,Is.GreaterThan(sprite.bounds.size.y*1.5f));
            var resting=actor.transform.position;
            yield return new WaitForSeconds(.25f);
            Assert.That(Vector2.Distance(actor.transform.position,resting),Is.LessThan(.01f));
            yield return new WaitForSeconds(4.2f);
            var memory=CampaignMemorySave.Load();
            Assert.That(memory.complete&&memory.headHit&&memory.memoryLost,Is.True);
            Assert.That(actor.IsInputLocked,Is.True);
            yield return SceneManager.LoadSceneAsync("Menu_MisterioDeVarginha");
        }
        [UnityTest]
        public IEnumerator OpeningHandsControlToChildWithoutCombatAndPauseRestoresInput()
        {
            CampaignMemorySave.Write(new CampaignMemory());
            yield return SceneManager.LoadSceneAsync(VarginhaCampaignPhase1.SceneName);
            yield return null; yield return null;
            var phase = VarginhaCampaignPhase1.Active;
            Assert.That(phase, Is.Not.Null);
            Assert.That(VarginhaCampaignPhase1.IsModalOpen, Is.True);
            phase.FinishOpening(); phase.DeliverControl();
            var actor = Object.FindAnyObjectByType<EdelzioTopDownController>();
            Assert.That(actor.GetComponent<SpriteRenderer>().sprite.texture.name, Is.EqualTo("ChildEdelzio"));
            Assert.That(actor.HasBackpack, Is.False);
            Assert.That(actor.IsInputLocked, Is.False);
            var keyboard = InputSystem.AddDevice<Keyboard>();
            float startX = actor.transform.position.x;
            Press(keyboard.dKey);
            yield return new WaitForSeconds(.25f);
            Release(keyboard.dKey);
            Assert.That(actor.transform.position.x, Is.GreaterThan(startX + .15f), "D must move the child after the opening.");
            Assert.That(actor.TryDodge(Vector2.right), Is.False);
            Assert.That(Object.FindObjectsByType<EntityManifestationAI>(), Is.Empty);
            phase.TogglePause();
            Assert.That(Time.timeScale, Is.Zero);
            Assert.That(actor.IsInputLocked, Is.True);
            phase.TogglePause();
            Assert.That(Time.timeScale, Is.EqualTo(1));
            Assert.That(actor.IsInputLocked, Is.False);
            phase.Examine(0);
            var save = CampaignMemorySave.Load();
            Assert.That(save.openingSeen && save.powerFailed, Is.True);
            Assert.That(save.evidence & 1, Is.EqualTo(1));
            yield return SceneManager.LoadSceneAsync("Menu_MisterioDeVarginha");
            Assert.That(VarginhaCampaignPhase1.Active, Is.Null);
        }
        [TearDown]public void Cleanup()
        {
            Time.timeScale=1;
            if(_memoryBefore==null){if(File.Exists(CampaignMemorySave.Path))File.Delete(CampaignMemorySave.Path);}
            else File.WriteAllText(CampaignMemorySave.Path,_memoryBefore);
            if(_storyBefore==null){if(File.Exists(CampaignStorySave.Path))File.Delete(CampaignStorySave.Path);}
            else File.WriteAllText(CampaignStorySave.Path,_storyBefore);
        }
    }
}
