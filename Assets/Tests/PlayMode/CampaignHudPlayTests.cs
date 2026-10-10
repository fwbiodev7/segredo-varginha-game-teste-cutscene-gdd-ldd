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
    [Category("HudPuzzles")] public class CampaignHudPlayTests : InputTestFixture
    {
        string _save,_memory;
        public override void Setup(){VarginhaInputActions.Shutdown();base.Setup();}
        public override void TearDown(){Time.timeScale=1;VarginhaInputActions.Shutdown();VarginhaGamepadUI.Reset();CampaignHud.Reset();base.TearDown();}
        [SetUp] public void Preserve()
        {
            _save=File.Exists(CampaignStorySave.Path)?File.ReadAllText(CampaignStorySave.Path):null;
            _memory=File.Exists(CampaignMemorySave.Path)?File.ReadAllText(CampaignMemorySave.Path):null;
            InputSystem.settings.backgroundBehavior=InputSettings.BackgroundBehavior.IgnoreFocus;
            InputSystem.settings.editorInputBehaviorInPlayMode=InputSettings.EditorInputBehaviorInPlayMode.AllDeviceInputAlwaysGoesToGameView;
        }
        [TearDown] public void Restore()
        {
            RestoreFile(CampaignStorySave.Path,_save);RestoreFile(CampaignMemorySave.Path,_memory);
        }
        static void RestoreFile(string path,string data){if(data==null){if(File.Exists(path))File.Delete(path);}else File.WriteAllText(path,data);}
        [UnityTest,Timeout(120000)] public IEnumerator RealChapterControllersFreezeHudInMenusAndResumeWithoutReappearing()
        {
            var keyboard=InputSystem.AddDevice<Keyboard>();var pad=InputSystem.AddDevice<Gamepad>();
            foreach(int phase in new[]{2,6,12,18})
            {
                CampaignHud.Reset();CampaignStorySave.Write(new CampaignStory{phase=phase});
                yield return SceneManager.LoadSceneAsync(CampaignStorySave.Scene(phase));yield return new WaitForSeconds(3.3f);
#if UNITY_EDITOR
                UnityEditor.EditorWindow.GetWindow(typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.GameView")).Focus();
#endif
                VarginhaInputActions.Rebuild();
                if(phase==2)VarginhaCampaignStage.Active.CloseDialogue();
                else if(phase==6)CampaignExpansionController.Active.ClosePanel();
                else CampaignContinuationController.Active.CloseMessage();
                yield return null;yield return null;
                var hud=CampaignHud.For(phase);float initial=hud.Elapsed;
                yield return new WaitForSecondsRealtime(.15f);
                Assert.That(hud.Elapsed,Is.GreaterThan(initial),"HUD advances in chapter "+phase);
                if(phase==2)yield return Capture("HUD_Inicio");
                hud.Tick(14,true,hud.Goal,hud.Room);
                if(phase==2)yield return Capture("HUD_Apos_Fade");
                Press(keyboard.escapeKey);yield return null;Release(keyboard.escapeKey);yield return null;
                Assert.That(Time.timeScale,Is.Zero,"Keyboard pause "+phase);
                float paused=hud.Elapsed;yield return new WaitForSecondsRealtime(.1f);
                Assert.That(hud.Elapsed,Is.EqualTo(paused));
                Press(pad.startButton);yield return null;Release(pad.startButton);yield return null;
                Assert.That(Time.timeScale,Is.EqualTo(1));Assert.That(hud.IdentificationAlpha,Is.Zero);
                Press(keyboard.tabKey);yield return null;Release(keyboard.tabKey);yield return null;
                paused=hud.Elapsed;yield return new WaitForSecondsRealtime(.1f);
                Assert.That(hud.Elapsed,Is.EqualTo(paused),"Journal freezes HUD "+phase);
                Press(pad.buttonEast);yield return null;Release(pad.buttonEast);yield return null;
                Assert.That(hud.IdentificationAlpha,Is.Zero);
            }
            yield return SceneManager.LoadSceneAsync("Menu_MisterioDeVarginha");
        }
        static IEnumerator Capture(string name)
        {
            Directory.CreateDirectory("Docs/QAHudPuzzles20261009");yield return new WaitForEndOfFrame();
            var screenshot=ScreenCapture.CaptureScreenshotAsTexture();
            File.WriteAllBytes("Docs/QAHudPuzzles20261009/"+name+".png",screenshot.EncodeToPNG());Object.Destroy(screenshot);
        }
        [Test] public void KeyboardCanNavigateAndSubmitTheSamePuzzleControlsAsGamepad()
        {
            var keyboard=InputSystem.AddDevice<Keyboard>();var pad=InputSystem.AddDevice<Gamepad>();VarginhaInputActions.Rebuild();_ = VarginhaInputActions.Asset;InputSystem.Update();
            Press(keyboard.rightArrowKey);Assert.That(VarginhaInputActions.UI("Navigate").ReadValue<Vector2>().x,Is.EqualTo(1));
            Assert.That(VarginhaInputActions.NavigationLabel,Is.EqualTo("SETAS"));Release(keyboard.rightArrowKey);
            Press(keyboard.enterKey);Assert.That(VarginhaInputActions.UI("Submit").WasPressedThisFrame(),Is.True);Release(keyboard.enterKey);
            InputSystem.Update();Press(pad.buttonSouth);Assert.That(VarginhaInputActions.SubmitLabel,Is.EqualTo("A"));
            Assert.That(VarginhaInputActions.UI("Submit").WasPressedThisFrame(),Is.True);Release(pad.buttonSouth);
            InputSystem.Update();Press(keyboard.eKey);Assert.That(VarginhaInputActions.UsingGamepad,Is.False);Release(keyboard.eKey);
        }
    }
}
