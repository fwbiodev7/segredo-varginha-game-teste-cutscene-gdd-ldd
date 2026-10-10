using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using Game.Varginha;
using Game.Varginha.Experiment;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace Game.Tests.PlayMode
{
    [Category("HudPuzzles")] public class CampaignControllerRemapTests : InputTestFixture
    {
        readonly Dictionary<string,string> _preferences=new();
        const string Prefix="Varginha.Controls.Gamepad.";
        public override void Setup(){VarginhaInputActions.Shutdown();base.Setup();}
        public override void TearDown(){VarginhaInputActions.Shutdown();base.TearDown();}
        [SetUp] public void PreserveBindings()
        {
            InputSystem.settings.backgroundBehavior=InputSettings.BackgroundBehavior.IgnoreFocus;
            InputSystem.settings.editorInputBehaviorInPlayMode=InputSettings.EditorInputBehaviorInPlayMode.AllDeviceInputAlwaysGoesToGameView;
            _preferences.Clear();
            foreach(var command in VarginhaGamepadBindings.Commands)
            {
                string key=Prefix+command;_preferences[key]=PlayerPrefs.HasKey(key)?PlayerPrefs.GetString(key):null;
                PlayerPrefs.DeleteKey(key);
            }
            VarginhaInputActions.Rebuild();
        }
        [TearDown] public void RestoreBindings()
        {
            VarginhaGamepadBindings.Reset();
            foreach(var pair in _preferences)
                if(pair.Value==null)PlayerPrefs.DeleteKey(pair.Key);else PlayerPrefs.SetString(pair.Key,pair.Value);
            PlayerPrefs.Save();
        }
        [Test] public void RemappingSwapsConflictsAndSurvivesActionRebuild()
        {
            var pad=InputSystem.AddDevice<Gamepad>();var keyboard=InputSystem.AddDevice<Keyboard>();VarginhaInputActions.Rebuild();
            VarginhaGamepadBindings.Set("Interact","buttonWest");
            Assert.That(VarginhaGamepadBindings.Label("Attack"),Is.EqualTo("A"));
            VarginhaInputActions.Rebuild();Press(pad.buttonWest);
            Assert.That(VarginhaInputActions.Button(VarginhaInputAction.Interact).WasPressedThisFrame(),Is.True);
            Assert.That(VarginhaInputActions.CarPressed,Is.True);Assert.That(VarginhaInputActions.CarLabel,Is.EqualTo("X"));Release(pad.buttonWest);
            InputSystem.Update();Press(pad.buttonSouth);
            Assert.That(VarginhaInputActions.CarPressed,Is.False);Assert.That(VarginhaInputActions.Button(VarginhaInputAction.Attack).WasPressedThisFrame(),Is.True);
            Assert.That(VarginhaInputActions.UI("Submit").WasPressedThisFrame(),Is.True);Release(pad.buttonSouth);
            InputSystem.Update();Press(pad.buttonEast);Assert.That(VarginhaInputActions.CancelPressed,Is.True);Release(pad.buttonEast);
            InputSystem.Update();Press(keyboard.wKey);Assert.That(VarginhaInputActions.CarPressed,Is.True);Release(keyboard.wKey);
            InputSystem.Update();Press(keyboard.eKey);Assert.That(VarginhaInputActions.Button(VarginhaInputAction.Interact).WasPressedThisFrame(),Is.True);
            Assert.That(VarginhaInputActions.CarPressed,Is.False);
        }
        [Test] public void HudLabelsFollowDeviceAndRemapping()
        {
            var pad=InputSystem.AddDevice<Gamepad>();var mouse=InputSystem.AddDevice<Mouse>();VarginhaInputActions.Rebuild();
            VarginhaGamepadBindings.Set("Inventory","rightStickPress");VarginhaGamepadBindings.Set("Journal","buttonNorth");
            Set(pad.leftStick,Vector2.right*.6f);
            Assert.That(CampaignHudIcons.KeyLabel(CampaignHudIcons.Icon.Backpack),Is.EqualTo("R3"));
            Assert.That(CampaignHudIcons.KeyLabel(CampaignHudIcons.Icon.Notebook),Is.EqualTo("Y"));
            Assert.That(CampaignHudIcons.KeyLabel(CampaignHudIcons.Icon.Pause),Is.EqualTo("START"));
            Set(mouse.delta,new Vector2(3,0));
            Assert.That(CampaignHudIcons.KeyLabel(CampaignHudIcons.Icon.Backpack),Is.EqualTo("G"));
            Assert.That(CampaignHudIcons.KeyLabel(CampaignHudIcons.Icon.Notebook),Is.EqualTo("TAB"));
            Assert.That(CampaignHudIcons.KeyLabel(CampaignHudIcons.Icon.Pause),Is.EqualTo("ESC"));
        }
        [UnityTest] public IEnumerator CaptureWaitsForOpeningButtonReleaseAndResumesAfterNewButtonRelease()
        {
            var pad=InputSystem.AddDevice<Gamepad>();VarginhaInputActions.Rebuild();
            Press(pad.buttonSouth);VarginhaGamepadBindings.BeginCapture("Attack");
            yield return null;VarginhaGamepadBindings.Tick();Assert.That(VarginhaGamepadBindings.Capturing,Is.EqualTo("Attack"));
            Release(pad.buttonSouth);yield return null;VarginhaGamepadBindings.Tick();
            Press(pad.rightStickButton);yield return null;VarginhaGamepadBindings.Tick();
            Assert.That(VarginhaGamepadBindings.Label("Attack"),Is.EqualTo("R3"),CaptureState(pad));Assert.That(VarginhaGamepadBindings.Suspended,Is.True);
            Release(pad.rightStickButton);yield return null;VarginhaGamepadBindings.Tick();Assert.That(VarginhaGamepadBindings.Suspended,Is.False);
        }
        [UnityTest] public IEnumerator CancelAndDisconnectRestoreInputsWithoutChangingBindings()
        {
            var pad=InputSystem.AddDevice<Gamepad>();VarginhaInputActions.Rebuild();VarginhaGamepadBindings.BeginCapture("Attack");
            yield return null;Press(pad.startButton);yield return null;VarginhaGamepadBindings.Tick();Release(pad.startButton);yield return null;VarginhaGamepadBindings.Tick();
            Assert.That(VarginhaGamepadBindings.Suspended,Is.False,CaptureState(pad));Assert.That(VarginhaGamepadBindings.Label("Attack"),Is.EqualTo("X"));
            VarginhaGamepadBindings.BeginCapture("Attack");InputSystem.RemoveDevice(pad);VarginhaGamepadBindings.Tick();
            Assert.That(VarginhaGamepadBindings.Suspended,Is.False);Assert.That(VarginhaInputActions.Asset.enabled,Is.True);
        }
        [Test] public void FuscaOriginalPixelsAndAspectArePreservedAndViewsHavePointFiltering()
        {
            string path=Path.Combine(Application.dataPath,"Resources/Varginha/StoryEffects/FuscaOriginal.png");
            using(var hash=SHA256.Create())Assert.That(System.BitConverter.ToString(hash.ComputeHash(File.ReadAllBytes(path))).Replace("-","").ToLowerInvariant(),Is.EqualTo("048dfd8c37a00d2fc84407871ea931f81927efd71bc69d50c733a549292f7344"));
            var side=CampaignOriginalFusca.Side;Assert.That(side.rect.size,Is.EqualTo(new Vector2(99,53)));Assert.That(side.texture.filterMode,Is.EqualTo(FilterMode.Point));
            foreach(CampaignWorkshopVehicle.Facing direction in System.Enum.GetValues(typeof(CampaignWorkshopVehicle.Facing)))
            {var sprite=CampaignOriginalFusca.Top(direction);Assert.That(Mathf.Max(sprite.bounds.size.x,sprite.bounds.size.y),Is.EqualTo(3.9f).Within(.001f));Assert.That(sprite.texture.filterMode,Is.EqualTo(FilterMode.Point));}
            Assert.That(CampaignOriginalFusca.Front.rect.size,Is.EqualTo(new Vector2(50,38)));
        }
        static string CaptureState(Gamepad pad)=>"capturing="+VarginhaGamepadBindings.Capturing+" suspended="+VarginhaGamepadBindings.Suspended+" current="+(Gamepad.current==pad)+" released="+typeof(VarginhaGamepadBindings).GetField("_released",BindingFlags.NonPublic|BindingFlags.Static).GetValue(null)+" held="+string.Join(",",System.Linq.Enumerable.Select(pad.allControls,c=>c.name+":"+c.ReadValueAsObject()));
        [UnityTest,Timeout(30000)] public IEnumerator ControllerDiagramAppearsInSettingsAndCanBeNavigatedWithoutMouse()
        {
            var pad=InputSystem.AddDevice<Gamepad>();VarginhaInputActions.Rebuild();Set(pad.leftStick,Vector2.right*.6f);Set(pad.leftStick,Vector2.zero);
            yield return SceneManager.LoadSceneAsync("Menu_MisterioDeVarginha");yield return null;
            var menu=Object.FindAnyObjectByType<VarginhaMainMenu>();var panel=typeof(VarginhaMainMenu).GetField("panel",BindingFlags.NonPublic|BindingFlags.Instance);
            panel.SetValue(menu,System.Enum.Parse(panel.FieldType,"Settings"));
            var settings=typeof(VarginhaGameSettings);settings.GetField("_tab",BindingFlags.NonPublic|BindingFlags.Static).SetValue(null,0);
            var diagram=settings.Assembly.GetType("Game.Varginha.Experiment.CampaignControllerSettings");diagram.GetField("_view",BindingFlags.NonPublic|BindingFlags.Static).SetValue(null,1);
            #if UNITY_EDITOR
            var view=UnityEditor.EditorWindow.GetWindow(typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.GameView"));view.Show();view.Repaint();
            #endif
            yield return new WaitForSecondsRealtime(.7f);
            Assert.That(Resources.Load<Texture2D>("Varginha/Interface/ControllerDiagram"),Is.Not.Null);Assert.That(VarginhaGamepadUI.HasSelection,Is.True);
            Press(pad.dpad.down);yield return null;Release(pad.dpad.down);yield return null;
            Assert.That(VarginhaGamepadUI.ActiveContext,Does.StartWith("settings:"));
            string path=Path.Combine(Directory.GetCurrentDirectory(),"Docs/QARevisao20261007/Controle_remapeamento.png");
            ScreenCapture.CaptureScreenshot(path);yield return new WaitForSecondsRealtime(.4f);
            Press(pad.buttonEast);yield return null;Release(pad.buttonEast);yield return null;
            Assert.That(VarginhaGamepadUI.ActiveContext,Does.StartWith("menu:"));
            diagram.GetField("_view",BindingFlags.NonPublic|BindingFlags.Static).SetValue(null,-1);
        }
    }
}
