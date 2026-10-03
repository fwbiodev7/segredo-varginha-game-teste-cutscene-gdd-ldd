using System.Collections;
using System.IO;
using Game.Varginha;
using Game.Varginha.Experiment;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace Game.Tests.PlayMode
{
    public class CampaignChaptersTests : InputTestFixture
    {
        private string _previousStory, _previousMemory;
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
            CampaignMemorySave.Write(new CampaignMemory { openingSeen = true, x = 8, y = 3 });
            yield return SceneManager.LoadSceneAsync(VarginhaCampaignPhase1.SceneName); yield return null; yield return null;
            var actor = Object.FindAnyObjectByType<EdelzioTopDownController>();
            var keyboard = InputSystem.AddDevice<Keyboard>();
            Press(keyboard.dKey); yield return new WaitForSeconds(.65f); Release(keyboard.dKey);
            Assert.That(actor.transform.position.x, Is.LessThan(9), "The child cannot cross the east wall.");
            var body = actor.GetComponent<Rigidbody2D>(); body.position = new Vector2(8, 0); body.linearVelocity = Vector2.zero;
            Press(keyboard.dKey); yield return new WaitForSeconds(.9f); Release(keyboard.dKey);
            Assert.That(actor.transform.position.x, Is.GreaterThan(10), "The existing east doorway must remain passable.");
            Assert.That(actor.GetComponent<CircleCollider2D>().radius, Is.LessThan(.3f), "The collision shape follows the child's feet.");
            yield return SceneManager.LoadSceneAsync("Menu_MisterioDeVarginha");
        }
        [UnityTest] public IEnumerator HouseDriveSchoolAndCodeFormAPlayableSavedCampaign()
        {
            // Include the childhood wall/door check in the full campaign run as well.
            yield return ChildhoodFeetRespectWallsAndPassThroughTheExistingDoor();
            CampaignStorySave.Write(new CampaignStory { phase = 2 });
            yield return SceneManager.LoadSceneAsync(CampaignStorySave.Scene(2)); yield return new WaitForSeconds(3.2f);
            var stage = VarginhaCampaignStage.Active;
            stage.Interact("box"); stage.CloseDialogue();
            Assert.That(stage.Progress.boxFound, Is.False, "Routine cannot be bypassed.");
            foreach (string point in new[] { "wash", "food", "work", "bag" })
            {
                var actor = Object.FindAnyObjectByType<EdelzioTopDownController>();
                var target = GameObject.Find(point == "bag" ? "Backpack_Prop" : point == "food" ? "Coffee_Cup" : "Notebook_TI");
                if (target != null) actor.GetComponent<Rigidbody2D>().position = point == "work" ? (Vector2)GameObject.Find("Chair_Office").transform.position + Vector2.left * .8f + Vector2.up * .35f : (Vector2)target.transform.position + Vector2.down;
                yield return new WaitForFixedUpdate(); yield return null;
                stage.Interact(point); yield return new WaitUntil(() => !stage.IsActing); stage.CloseDialogue();
            }
            var backpackActor = Object.FindAnyObjectByType<EdelzioTopDownController>();
            Assert.That(stage.Progress.routine, Is.EqualTo(15), "All four original routine animations finish successfully.");
            Assert.That(stage.OpenBackpack(), Is.True, "Backpack open: has=" + backpackActor.HasBackpack + ", locked=" + backpackActor.IsInputLocked + ", dialogue=" + VarginhaGameHUD.Instance.IsDialogueOpen + ", scale=" + Time.timeScale);
            yield return null;
            Assert.That(VarginhaGameHUD.Instance.IsInventoryOpen, Is.True);
            VarginhaGameHUD.Instance.CloseBackpack(); yield return null;
            Assert.That(VarginhaGameHUD.Instance.IsInventoryOpen, Is.False);
            yield return new WaitForSeconds(.1f);
            Assert.That(Object.FindAnyObjectByType<EdelzioTopDownController>().GetComponent<SpriteRenderer>().sprite.name, Does.StartWith("Team_"), "The supplied character remains visible after closing the original backpack.");
            stage.Interact("box"); stage.FinishCutscene();
            Assert.That(stage.SubmitPages(), Is.False);
            stage.Progress.pages = new[] { 0, 1, 2 }; Assert.That(stage.SubmitPages(), Is.True); stage.FinishCutscene();
            Assert.That(stage.Progress.MapFragments, Is.EqualTo(1));
            Assert.That(Object.FindAnyObjectByType<EdelzioTopDownController>().HasFuscaKey, Is.True);
            stage.Interact("car"); yield return null; yield return null;
            Assert.That(SceneManager.GetActiveScene().name, Is.EqualTo(VarginhaCampaignDrive.SceneName));
            var drive = VarginhaCampaignDrive.Active;
            // Begin near the scripted pane; real keyboard movement must trigger it.
            drive.Progress.driveDistance = 54;
            var car = GameObject.Find("Fusca_TopView_Campanha").GetComponent<Rigidbody2D>(); car.position = new Vector2(-2, 54);
            yield return new WaitForSeconds(3.2f);
            var keyboard = InputSystem.AddDevice<Keyboard>(); Press(keyboard.wKey); yield return new WaitForSeconds(.5f); Release(keyboard.wKey);
            Assert.That(drive.HasBrokenDown, Is.True);
            drive.TogglePause(); Assert.That(Time.timeScale, Is.Zero); drive.TogglePause();
            drive.Inspect(0); drive.Inspect(1); drive.Inspect(2); drive.CloseInspection();
            yield return new WaitForSeconds(4.3f);
            Press(keyboard.wKey); yield return new WaitForSeconds(7.6f); Release(keyboard.wKey);
            Assert.That(drive.Progress.arrival, Is.True, "The car must reach the Industrial after inspection and restart.");
            CampaignStorySave.GoTo(4); yield return new WaitForSeconds(3.2f); stage = VarginhaCampaignStage.Active;
            Assert.That(Object.FindObjectsByType<VarginhaCombatEnemy>(FindObjectsSortMode.None), Is.Empty);
            Assert.That(GameObject.Find("Renan_Industrial_Campanha"), Is.Not.Null);
            Assert.That(Object.FindAnyObjectByType<VarginhaIndustrialSchoolFacade>(), Is.Not.Null, "The real school frontage must be present in the campaign map.");
            Assert.That(Object.FindObjectsByType<CampaignSchoolLife>().Length, Is.EqualTo(9), "All original students have classroom activities.");
            stage.Interact("student:0"); stage.CloseDialogue(); stage.Interact("research"); stage.CloseDialogue();
            Assert.That(stage.Progress.studentsTalked, Is.EqualTo(1)); Assert.That(stage.Progress.ouzanaNote, Is.True);
            stage.Interact("renan"); stage.CloseDialogue(); stage.Interact("lesson"); stage.CloseDialogue(); stage.Interact("archive"); stage.CloseDialogue();
            Assert.That(stage.SubmitBuilding(0), Is.False); Assert.That(stage.SubmitBuilding(1), Is.True); stage.FinishCutscene();
            CampaignStorySave.GoTo(5); yield return new WaitForSeconds(3.2f); stage = VarginhaCampaignStage.Active;
            stage.Interact("notebook"); stage.CloseDialogue(); stage.Interact("mural"); stage.CloseDialogue();
            Assert.That(stage.SubmitCode(), Is.False); stage.Progress.symbols = new[] { 0, 1, 0, 1 }; stage.Progress.circle = 2; stage.Progress.triangle = 3;
            Assert.That(stage.SubmitCode(), Is.True); stage.FinishCutscene(); stage.Interact("renan"); stage.FinishCutscene();
            Assert.That(stage.Progress.renanConfirmed, Is.True); stage.Save();
            yield return SceneManager.LoadSceneAsync("Menu_MisterioDeVarginha");
            VarginhaCampaignPhase1.StartCampaign(true); yield return new WaitForSeconds(3.2f);
            Assert.That(VarginhaCampaignStage.Active.phase, Is.EqualTo(5));
            Assert.That(VarginhaCampaignStage.Active.Progress.MapFragments, Is.EqualTo(2));
            Assert.That(VarginhaCampaignStage.Active.Progress.renanConfirmed, Is.True);
            yield return SceneManager.LoadSceneAsync("Menu_MisterioDeVarginha");
        }
    }
}
