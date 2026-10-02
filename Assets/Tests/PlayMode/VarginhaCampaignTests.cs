using System.Collections;
using Game.Varginha;
using Game.Varginha.Experiment;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.InputSystem;

namespace Game.Tests.PlayMode
{
    public class VarginhaCampaignTests : InputTestFixture
    {
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
            Assert.That(Object.FindObjectsByType<EntityManifestationAI>(FindObjectsSortMode.None), Is.Empty);
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
        [TearDown] public void Cleanup() => Time.timeScale = 1;
    }
}
