using System.Collections;
using System.IO;
using System.Reflection;
using Game.Varginha;
using Game.Varginha.Experiment;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using System.Collections.Generic;
using System.Linq;

namespace Game.Tests.PlayMode
{
    public class CampaignFeedbackPlayTests : InputTestFixture
    {
        const string SettingsKey="Varginha.Experiment.Settings.v1";
        const BindingFlags Hidden=BindingFlags.NonPublic|BindingFlags.Instance;
        string _settings,_save,_memory;int _tab;
        public override void Setup(){VarginhaInputActions.Shutdown();base.Setup();}
        public override void TearDown(){VarginhaInputActions.Shutdown();VarginhaGamepadUI.Reset();base.TearDown();}
        [SetUp] public void Preserve()
        {
            _settings=PlayerPrefs.HasKey(SettingsKey)?PlayerPrefs.GetString(SettingsKey):null;
            _save=File.Exists(CampaignStorySave.Path)?File.ReadAllText(CampaignStorySave.Path):null;
            _memory=File.Exists(CampaignMemorySave.Path)?File.ReadAllText(CampaignMemorySave.Path):null;
            _tab=(int)typeof(VarginhaGameSettings).GetField("_tab",BindingFlags.NonPublic|BindingFlags.Static).GetValue(null);
            InputSystem.settings.backgroundBehavior=InputSettings.BackgroundBehavior.IgnoreFocus;
            InputSystem.settings.editorInputBehaviorInPlayMode=InputSettings.EditorInputBehaviorInPlayMode.AllDeviceInputAlwaysGoesToGameView;
        }
        [TearDown] public void Restore()
        {
            Time.timeScale=1;
            if(_settings==null)PlayerPrefs.DeleteKey(SettingsKey);else PlayerPrefs.SetString(SettingsKey,_settings);
            typeof(VarginhaGameSettings).GetField("_current",BindingFlags.NonPublic|BindingFlags.Static).SetValue(null,null);
            typeof(VarginhaGameSettings).GetField("_tab",BindingFlags.NonPublic|BindingFlags.Static).SetValue(null,_tab);
            VarginhaGameSettings.Apply(false);PlayerPrefs.Save();
            if(_save==null){if(File.Exists(CampaignStorySave.Path))File.Delete(CampaignStorySave.Path);}else File.WriteAllText(CampaignStorySave.Path,_save);
            if(_memory==null){if(File.Exists(CampaignMemorySave.Path))File.Delete(CampaignMemorySave.Path);}else File.WriteAllText(CampaignMemorySave.Path,_memory);
        }
        [UnityTest] public IEnumerator AudioRowsRespondToDpadAnalogKeyboardAndSaveAllFourVolumes()
        {
            yield return SceneManager.LoadSceneAsync("Menu_MisterioDeVarginha");
            var menu=Object.FindAnyObjectByType<VarginhaMainMenu>();var panel=typeof(VarginhaMainMenu).GetField("panel",Hidden);
            panel.SetValue(menu,System.Enum.Parse(panel.FieldType,"Settings"));
            typeof(VarginhaGameSettings).GetField("_tab",BindingFlags.NonPublic|BindingFlags.Static).SetValue(null,0);
            VarginhaGamepadUI.Reset();var pad=InputSystem.AddDevice<Gamepad>();var keyboard=InputSystem.AddDevice<Keyboard>();
            VarginhaInputActions.Rebuild();
#if UNITY_EDITOR
            UnityEditor.EditorWindow.GetWindow(typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.GameView")).Show();
#endif
            yield return new WaitForSecondsRealtime(.5f);
            yield return Pad(pad,GamepadButton.DpadRight);yield return Pad(pad,GamepadButton.South);
            Assert.That((int)typeof(VarginhaGameSettings).GetField("_tab",BindingFlags.NonPublic|BindingFlags.Static).GetValue(null),Is.EqualTo(1));
            var settings=VarginhaGameSettings.Current;settings.master=settings.music=settings.effects=settings.voice=.5f;VarginhaGameSettings.Save();
            yield return Pad(pad,GamepadButton.DpadDown);
            for(int i=0;i<4;i++)
            {
                float before=Volume(i);
                if(i==1)yield return Analog(pad,Vector2.right*.8f);
                else if(i==2)yield return KeyPress(keyboard,Key.RightArrow);
                else yield return Pad(pad,GamepadButton.DpadRight);
                Assert.That(Volume(i),Is.EqualTo(before+.05f).Within(.001f),"Selected audio row "+i);
                if(i<3)yield return Pad(pad,GamepadButton.DpadDown);
            }
            var saved=JsonUtility.FromJson<GameSettingsData>(PlayerPrefs.GetString(SettingsKey));
            Assert.That(saved.master,Is.EqualTo(.55f).Within(.001f));Assert.That(saved.music,Is.EqualTo(.55f).Within(.001f));
            Assert.That(saved.effects,Is.EqualTo(.55f).Within(.001f));Assert.That(saved.voice,Is.EqualTo(.55f).Within(.001f));
            Assert.That(AudioListener.volume,Is.EqualTo(.55f).Within(.001f));
            typeof(VarginhaGameSettings).GetField("_current",BindingFlags.NonPublic|BindingFlags.Static).SetValue(null,null);
            Assert.That(VarginhaGameSettings.Current.voice,Is.EqualTo(.55f).Within(.001f),"Reload persisted settings");
            yield return Pad(pad,GamepadButton.East);yield return null;
            Assert.That(panel.GetValue(menu).ToString(),Is.Not.EqualTo("Settings"),"B returns to the menu");
        }
        static float Volume(int i)=>i==0?VarginhaGameSettings.Current.master:i==1?VarginhaGameSettings.Current.music:i==2?VarginhaGameSettings.Current.effects:VarginhaGameSettings.Current.voice;
#if UNITY_EDITOR
        [UnityTest,Category("FeedbackMouse")] public IEnumerator MouseDragUpdatesEveryAudioRowAndSavesTheSelectedValue()
        {
            yield return SceneManager.LoadSceneAsync("Menu_MisterioDeVarginha");
            var menu=Object.FindAnyObjectByType<VarginhaMainMenu>();
            var panel=typeof(VarginhaMainMenu).GetField("panel",Hidden);
            panel.SetValue(menu,System.Enum.Parse(panel.FieldType,"Settings"));
            typeof(VarginhaGameSettings).GetField("_tab",BindingFlags.NonPublic|BindingFlags.Static).SetValue(null,1);
            VarginhaGamepadUI.Reset();
            var settings=VarginhaGameSettings.Current;
            settings.master=settings.music=settings.effects=settings.voice=.5f;
            VarginhaGameSettings.Save();
            var view=UnityEditor.EditorWindow.GetWindow(typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.GameView"));
            var probe=ScriptableObject.CreateInstance<CampaignFeedbackMouseProbe>();
            probe.position=new Rect(20,20,Screen.width,Screen.height);probe.ShowUtility();
            menu.enabled=false;
            try
            {
            yield return new WaitForSecondsRealtime(.5f);
            for(int row=0;row<4;row++)
            {
                float[] before={Volume(0),Volume(1),Volume(2),Volume(3)};
                yield return SendMouse(probe,EventType.MouseDown,new Vector2(827.5f,244+row*65));
                yield return SendMouse(probe,EventType.MouseDrag,new Vector2(945,244+row*65));
                yield return SendMouse(probe,EventType.MouseUp,new Vector2(945,244+row*65));
                Assert.That(Volume(row),Is.GreaterThan(.65f),"Mouse drag changes audio row "+row+" events="+probe.events);
                for(int other=0;other<4;other++)if(other!=row)
                    Assert.That(Volume(other),Is.EqualTo(before[other]).Within(.001f),"Mouse changes only the selected row");
                var saved=JsonUtility.FromJson<GameSettingsData>(PlayerPrefs.GetString(SettingsKey));
                float persisted=row==0?saved.master:row==1?saved.music:row==2?saved.effects:saved.voice;
                Assert.That(persisted,Is.EqualTo(Volume(row)).Within(.001f));
            }
            Assert.That(AudioListener.volume,Is.EqualTo(settings.master).Within(.001f));
            }
            finally {menu.enabled=true;probe.Close();view.Focus();}
        }
        static IEnumerator SendMouse(CampaignFeedbackMouseProbe probe,EventType type,Vector2 canvas)
        {
            float scale=Mathf.Min(Screen.width/1280f,Screen.height/720f);
            var pixel=canvas*scale+new Vector2((Screen.width-1280*scale)/2,(Screen.height-720*scale)/2);
            probe.SendEvent(new Event {type=type,button=0,mousePosition=pixel});
            yield return null;
        }
#endif
        static IEnumerator Pad(Gamepad pad,GamepadButton button)
        {
            for(int i=0;i<2;i++){InputSystem.QueueStateEvent(pad,new GamepadState().WithButton(button));yield return null;}
            for(int i=0;i<2;i++){InputSystem.QueueStateEvent(pad,new GamepadState());yield return null;}
        }
        static IEnumerator Analog(Gamepad pad,Vector2 value)
        {
            for(int i=0;i<2;i++){InputSystem.QueueStateEvent(pad,new GamepadState {leftStick=value});yield return null;}
            for(int i=0;i<2;i++){InputSystem.QueueStateEvent(pad,new GamepadState());yield return null;}
        }
        static IEnumerator KeyPress(Keyboard keyboard,Key key)
        {
            for(int i=0;i<2;i++){InputSystem.QueueStateEvent(keyboard,new KeyboardState(key));yield return null;}
            for(int i=0;i<2;i++){InputSystem.QueueStateEvent(keyboard,new KeyboardState());yield return null;}
        }
        [UnityTest] public IEnumerator RadioCueSharesThePhraseClockPausesAndContinuesToSchool()
        {
            foreach(bool reduced in new[]{false,true})
            {
                VarginhaGameSettings.Current.reducedMotion=reduced;
                CampaignStorySave.Write(new CampaignStory{phase=2});
                yield return SceneManager.LoadSceneAsync(CampaignStorySave.Scene(2));yield return new WaitForSeconds(3.2f);
                var stage=VarginhaCampaignStage.Active;
                var cue=typeof(VarginhaCampaignStage).GetField("_radioCueTime",Hidden);
                var dialogue=typeof(VarginhaCampaignStage).GetField("_dialogue",Hidden);
                stage.StartCoroutine((IEnumerator)typeof(VarginhaCampaignStage).GetMethod("SchoolTransition",Hidden).Invoke(stage,null));
                Assert.That((string)dialogue.GetValue(stage),Does.Not.Contain("Não deixa ela sair"));
                yield return new WaitForSeconds(.96f);
                Assert.That((string)dialogue.GetValue(stage),Does.Contain("Não deixa ela sair"));
                Assert.That((float)cue.GetValue(stage),Is.GreaterThanOrEqualTo(0));
                Directory.CreateDirectory("Docs/QAFeedback20261009");
                // Capture the rendered cue before pausing; deferred screenshots used
                // to capture the pause menu and hide the effect in both variants.
                yield return new WaitForEndOfFrame();
                var image=ScreenCapture.CaptureScreenshotAsTexture();
                File.WriteAllBytes("Docs/QAFeedback20261009/Radio_"+(reduced?"ReducedMotion":"Normal")+".png",image.EncodeToPNG());
                Object.Destroy(image);
                stage.CloseDialogue();Assert.That(dialogue.GetValue(stage),Is.Not.Null,"Continue cannot erase the timed phrase");
                stage.TogglePause();float paused=Time.time;yield return new WaitForSecondsRealtime(.25f);
                Assert.That(Time.time,Is.EqualTo(paused).Within(.001f));Assert.That((float)cue.GetValue(stage),Is.GreaterThanOrEqualTo(0));
                stage.TogglePause();yield return new WaitForSeconds(.7f);
                Assert.That((float)cue.GetValue(stage),Is.LessThan(0),"Finite cue cleans up");
                float deadline=Time.realtimeSinceStartup+8;
                while(SceneManager.GetActiveScene().name!=CampaignStorySave.Scene(4)&&Time.realtimeSinceStartup<deadline)yield return null;
                Assert.That(SceneManager.GetActiveScene().name,Is.EqualTo(CampaignStorySave.Scene(4)));
                Assert.That(CampaignStorySave.Load().arrival,Is.True);
            }
            yield return SceneManager.LoadSceneAsync("Menu_MisterioDeVarginha");
        }
        [UnityTest,Timeout(240000)] public IEnumerator AuthoredGroundShapesBlockMovementFromEveryAccessibleSide()
        {
            var results=new List<string>();int contacts=0;
            foreach(int phase in new[]{1,2,4,6,7,8,9,12,14,15,18})
            {
                CampaignMemorySave.Write(new CampaignMemory{openingSeen=true});
                CampaignStorySave.Write(new CampaignStory{phase=phase});
                yield return SceneManager.LoadSceneAsync(CampaignStorySave.Scene(phase));yield return new WaitForSeconds(3.4f);
                Object.FindAnyObjectByType<CampaignContinuationController>()?.CloseMessage();
                VarginhaGameHUD.Instance?.CloseDialogue();
                var actor=Object.FindAnyObjectByType<EdelzioTopDownController>();var body=actor.GetComponent<Rigidbody2D>();var feet=actor.GetComponent<CircleCollider2D>();
                var data=CampaignIllustratedMaps.Get(phase);var plan=CampaignMapPlan.Create(phase);
                var map=GameObject.Find("Mapa_Campanha").transform;var keyboard=InputSystem.AddDevice<Keyboard>();VarginhaInputActions.Rebuild();
                foreach(var prop in data.props.Where(p=>p.collision?.Length>=6))
                {
                    var solid=map.Find("02_Mobilia_Colisoes/"+prop.name).GetComponent<Collider2D>();
                    var bounds=solid.bounds;
                    foreach(var direction in new[]{Vector2.left,Vector2.right,Vector2.up,Vector2.down})
                    {
                        var centre=(Vector2)bounds.center;
                        float extent=direction.x!=0?bounds.extents.x:bounds.extents.y;
                        var start=centre-direction*(extent+feet.radius+.20f);
                        if(!plan.IsClear(start,feet.radius)){results.Add(phase+" "+prop.name+" "+direction+" unavailable side: neighbouring obstacle");continue;}
                        body.position=start-feet.offset;body.linearVelocity=Vector2.zero;actor.SetInputLocked(false);
                        Physics2D.SyncTransforms();yield return new WaitForFixedUpdate();
                        Key key=direction.x<0?Key.A:direction.x>0?Key.D:direction.y>0?Key.W:Key.S;
                        float until=Time.time+.35f;
                        while(Time.time<until)
                        {
                            InputSystem.QueueStateEvent(keyboard,new KeyboardState(key));yield return null;
                            Assert.That(feet.Distance(solid).distance,Is.GreaterThanOrEqualTo(-.025f),phase+" "+prop.name+" stays outside the solid throughout movement "+direction);
                        }
                        InputSystem.QueueStateEvent(keyboard,new KeyboardState());yield return new WaitForFixedUpdate();yield return null;
                        var finish=body.position+feet.offset;
                        Assert.That(Vector2.Dot(finish-start,direction),Is.GreaterThan(.025f),phase+" "+prop.name+" actual input movement "+direction);
                        Assert.That(feet.Distance(solid).distance,Is.GreaterThanOrEqualTo(-.025f),phase+" "+prop.name+" cannot cross the solid base "+direction);
                        contacts++;results.Add(phase+" "+prop.name+" "+direction+" PASS");
                    }
                }
                InputSystem.RemoveDevice(keyboard);
            }
            Directory.CreateDirectory("Docs/QAFeedback20261009");File.WriteAllLines("Docs/QAFeedback20261009/PhysicalContacts.txt",results);
            Assert.That(contacts,Is.GreaterThan(60),"Actual contact checks across authored maps");
            yield return SceneManager.LoadSceneAsync("Menu_MisterioDeVarginha");
        }
    }
#if UNITY_EDITOR
    public class CampaignFeedbackMouseProbe : UnityEditor.EditorWindow
    {
        public string events="";
        void OnGUI()
        {
            // Native editor IMGUI events exercise the same settings renderer without
            // depending on the hardware input runtime replaced by InputTestFixture.
            if(Event.current.type==EventType.MouseDown||Event.current.type==EventType.MouseDrag||Event.current.type==EventType.MouseUp)
                events+=Event.current.type+"@"+Event.current.mousePosition+";";
            VarginhaGameSettings.Draw();
        }
    }
#endif
}
