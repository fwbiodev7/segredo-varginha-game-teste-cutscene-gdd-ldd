using System.Collections;
using System.IO;
using System.Linq;
using System.Reflection;
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
    public class CampaignRequestedChangesTests : InputTestFixture
    {
        string _save;
        public override void Setup(){VarginhaInputActions.Shutdown();base.Setup();}
        public override void TearDown(){VarginhaInputActions.Shutdown();base.TearDown();}
        [UnityTearDown] public IEnumerator LeaveScene(){Time.timeScale=1;yield return SceneManager.LoadSceneAsync("Menu_MisterioDeVarginha");}
        [SetUp] public void Preserve()
        {
            _save=File.Exists(CampaignStorySave.Path)?File.ReadAllText(CampaignStorySave.Path):null;
            VarginhaInputActions.Rebuild();VarginhaGamepadUI.Reset();
        }
        [TearDown] public void Restore()
        {
            VarginhaRumble.Stop();Time.timeScale=1;
            if(_save!=null)File.WriteAllText(CampaignStorySave.Path,_save);else if(File.Exists(CampaignStorySave.Path))File.Delete(CampaignStorySave.Path);
        }
        static IEnumerator Load(int phase,int area=0)
        {
            yield return SceneManager.LoadSceneAsync(phase>=11?CampaignContinuationDefinition.SceneName(phase,area):CampaignStorySave.Scene(phase));
            yield return new WaitForSeconds(3.2f);
            CampaignContinuationController.Active?.CloseMessage();yield return null;
        }
        [Test] public void DeadzoneCarIsolationAndLastDevicePrompts()
        {
            var pad=InputSystem.AddDevice<Gamepad>();var keyboard=InputSystem.AddDevice<Keyboard>();var mouse=InputSystem.AddDevice<Mouse>();
            VarginhaInputActions.Rebuild();Set(pad.leftStick,new Vector2(.08f,.08f));Assert.That(VarginhaInputActions.Move,Is.EqualTo(Vector2.zero));
            Set(pad.leftStick,new Vector2(.65f,0));Assert.That(VarginhaInputActions.Move.x,Is.InRange(.4f,.9f));Assert.That(VarginhaInputActions.InteractLabel,Is.EqualTo("A"));
            Press(keyboard.eKey);Assert.That(VarginhaInputActions.CarPressed,Is.False);Assert.That(VarginhaInputActions.InteractLabel,Is.EqualTo("E"));Release(keyboard.eKey);
            Press(keyboard.enterKey);Assert.That(VarginhaInputActions.CarPressed,Is.False);Release(keyboard.enterKey);
            Press(mouse.leftButton);Assert.That(VarginhaInputActions.CarPressed,Is.False);Release(mouse.leftButton);
            Press(keyboard.wKey);Assert.That(VarginhaInputActions.CarPressed,Is.True);Release(keyboard.wKey);
            Press(pad.buttonSouth);Assert.That(VarginhaInputActions.CarPressed,Is.True);Assert.That(VarginhaInputActions.CarLabel,Is.EqualTo("A"));
            Set(mouse.delta,new Vector2(5,0));Assert.That(VarginhaInputActions.CarLabel,Is.EqualTo("W"));
        }
        [UnityTest,Timeout(40000)] public IEnumerator RealMenuSettingsAndCancelWorkWithoutMouse()
        {
            var pad=InputSystem.AddDevice<Gamepad>();VarginhaInputActions.Rebuild();
            CampaignStorySave.Write(new CampaignStory{phase=2});
            yield return SceneManager.LoadSceneAsync("Menu_MisterioDeVarginha");yield return null;yield return null;
            #if UNITY_EDITOR
            var view=UnityEditor.EditorWindow.GetWindow(typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.GameView"));view.Show();view.Repaint();
            #endif
            yield return new WaitForSecondsRealtime(.5f);
            Press(pad.dpad.down);yield return null;Release(pad.dpad.down);yield return null;
            Assert.That(VarginhaGamepadUI.HasSelection,Is.True);
            Press(pad.dpad.down);yield return null;Release(pad.dpad.down);yield return null;
            Press(pad.buttonSouth);yield return null;Release(pad.buttonSouth);yield return null;yield return null;
            Assert.That(VarginhaGamepadUI.ActiveContext,Does.StartWith("settings:"));
            Press(pad.buttonEast);yield return null;Release(pad.buttonEast);yield return null;
            Assert.That(VarginhaGamepadUI.ActiveContext,Does.StartWith("menu:"));
        }
        [UnityTest,Timeout(60000)] public IEnumerator OuzanaDoorAndCasaraoArrivalsHaveReachableDestinations()
        {
            var story=new CampaignStory{phase=9};story.expansion.SolveAnchor(1996,1,23);CampaignStorySave.Write(story);yield return Load(9);
            var ouzana=CampaignExpansionController.Active;Assert.That(GameObject.Find("Porta_de_saida_Ouzana"),Is.Not.Null);
            var route=new System.Collections.Generic.List<Vector2>();Assert.That(ouzana.Plan.Route(ouzana.Plan.spawn,ouzana.Plan.points.Find(p=>p.id=="exit").position,route),Is.True);
            ouzana.Interact("ouzana");CampaignStorySave.GoTo(10);
            yield return new WaitUntil(()=>CampaignExpansionController.Active?.phase==10);Assert.That(CampaignExpansionController.Active.Progress.expansion.reagentUnlocked,Is.True);
            CampaignStorySave.Write(new CampaignStory{phase=12});yield return Load(12);
            var c=CampaignContinuationController.Active;c.Interact("entrance");
            yield return new WaitUntil(()=>CampaignContinuationController.Active?.area==1&&!CampaignCinematics.IsTransitioning);yield return new WaitForSeconds(2.5f);
            c=CampaignContinuationController.Active;c.CloseMessage();Assert.That(c.Plan.IsClear(c.Feet),Is.True);Assert.That(Vector2.Distance(c.Feet,c.Plan.spawn),Is.LessThan(.2f));
            foreach(var id in new[]{"key","photo","garden","service"})Assert.That(c.Plan.Route(c.Plan.spawn,c.Plan.points.Find(p=>p.id==id).position,route),Is.True,id);
            c.Interact("garden");yield return new WaitUntil(()=>CampaignContinuationController.Active?.area==0&&!CampaignCinematics.IsTransitioning);yield return new WaitForSeconds(2.5f);
            c=CampaignContinuationController.Active;Assert.That(c.Plan.IsClear(c.Feet),Is.True);Assert.That(Vector2.Distance(c.Feet,c.Plan.points.Find(p=>p.id=="entrance").position),Is.InRange(.5f,1.2f));
            c.Interact("entrance");yield return new WaitUntil(()=>CampaignContinuationController.Active?.area==1&&!CampaignCinematics.IsTransitioning);yield return new WaitForSeconds(2.5f);
            c=CampaignContinuationController.Active;c.CloseMessage();c.State.serviceRevealed=true;c.State.serviceKey=true;c.Interact("service");
            yield return new WaitUntil(()=>CampaignContinuationController.Active?.phase==13&&!CampaignCinematics.IsTransitioning);yield return new WaitForSeconds(3.2f);
            c=CampaignContinuationController.Active;c.CloseMessage();Assert.That(c.Plan.IsClear(c.Feet),Is.True);c.Interact("ground");
            yield return new WaitUntil(()=>CampaignContinuationController.Active?.phase==12&&CampaignContinuationController.Active?.area==1&&!CampaignCinematics.IsTransitioning);yield return new WaitForSeconds(3.2f);
            c=CampaignContinuationController.Active;Assert.That(c.Plan.IsClear(c.Feet),Is.True);Assert.That(Vector2.Distance(c.Feet,c.Plan.points.Find(p=>p.id=="service").position),Is.InRange(.3f,.8f));
        }
        [UnityTest,Timeout(40000)] public IEnumerator SchoolSeatingAndFaceWashKeepSourceArtAndStopCleanly()
        {
            CampaignStorySave.Write(new CampaignStory{phase=11});yield return Load(11);
            var students=Object.FindObjectsByType<CampaignSeatingLayers>(FindObjectsInactive.Exclude).Where(s=>s.GetComponent<SpriteRenderer>().sprite.name.StartsWith("StudentSeated_")).ToArray();
            Assert.That(students.Length,Is.EqualTo(9),"Scene="+SceneManager.GetActiveScene().name+" poses="+string.Join(",",Object.FindObjectsByType<CampaignSeatingLayers>(FindObjectsInactive.Include).Select(s=>s.name+":"+s.GetComponent<SpriteRenderer>().sprite.name)));
            foreach(var s in students)
            {var actor=s.GetComponent<SpriteRenderer>();var chair=s.SeatCollider.GetComponent<SpriteRenderer>();Assert.That(actor.bounds.min.y,Is.EqualTo(chair.bounds.center.y+chair.bounds.size.y*.05f).Within(.03f));Assert.That(actor.transform.localScale.x,Is.EqualTo(actor.transform.localScale.y));}
            var camera=Camera.main;camera.GetComponent<Game.Level.CameraFollow2D>().enabled=false;
            var centre=students.Select(s=>s.transform.position).Aggregate(Vector3.zero,(a,b)=>a+b)/students.Length;
            camera.transform.position=new Vector3(centre.x,centre.y,-10);camera.orthographicSize=6;
            Capture(camera,"Alunos_sentados.png");
            CampaignStorySave.Write(new CampaignStory{phase=2});yield return Load(2);
            var player=Object.FindAnyObjectByType<EdelzioTopDownController>();var action=player.GetComponent<VarginhaPlayerActionAnimation>();
            player.GetComponent<Rigidbody2D>().position=CampaignAdultHouse.WashApproach+Vector2.up*.3f;yield return new WaitForFixedUpdate();
            action.StartCoroutine(action.WashFaceRoutine());yield return new WaitForSeconds(.9f);
            Assert.That(action.IsActing,Is.True);Assert.That(Vector2.Distance(player.transform.position,CampaignAdultHouse.WashApproach),Is.LessThan(.15f));
            Assert.That(player.GetComponent<VarginhaPlayerSpriteAnimation>().ActionFacingDirection,Is.EqualTo(Vector2.down));Capture(Camera.main,"Edelzio_pia.png");
            yield return new WaitForSeconds(1.2f);Assert.That(action.IsActing,Is.False);Assert.That(player.IsScriptedMotion,Is.False);
            var car=GameObject.Find("Fusca").GetComponent<SpriteRenderer>();Assert.That(car.sprite,Is.SameAs(CampaignOriginalFusca.Side));
            var lamps=car.GetComponentsInChildren<CampaignDynamicLight>();Assert.That(lamps.Length,Is.EqualTo(1),"Only the visible side-view headlamp emits a beam");
            var sprite=car.sprite;
            var lampPixel=new Vector2(98-sprite.rect.xMin-sprite.pivot.x,100-51.5f-sprite.rect.yMin-sprite.pivot.y)/sprite.pixelsPerUnit;
            Assert.That(Vector2.Distance(lamps[0].Origin,car.transform.TransformPoint(lampPixel)),Is.LessThan(.015f),"Beam starts on the original headlamp pixels");
            Assert.That(Vector2.Dot(lamps[0].Direction,Vector2.right),Is.GreaterThan(.99f));
            Assert.That(lamps[0].reach,Is.LessThan(car.bounds.size.x*.5f));
            camera=Camera.main;camera.GetComponent<Game.Level.CameraFollow2D>().enabled=false;
            camera.transform.position=new Vector3(car.transform.position.x,car.transform.position.y,-10);camera.orthographicSize=3.5f;
            Capture(camera,"Fusca_original_no_mapa.png");
        }
        [UnityTest] public IEnumerator RumbleExpiresAndUnsupportedDevicesNeverThrow()
        {
            var pad=InputSystem.AddDevice<Gamepad>();VarginhaInputActions.Rebuild();Set(pad.leftStick,Vector2.right*.6f);yield return null;Set(pad.leftStick,Vector2.zero);yield return null;
            Assert.That(VarginhaInputActions.UsingGamepad,Is.True);
            bool enabled=VarginhaGameSettings.Current.vibration;float gain=VarginhaGameSettings.Current.vibrationIntensity;
            try
            {
                VarginhaGameSettings.Current.vibration=true;VarginhaGameSettings.Current.vibrationIntensity=.5f;
                Assert.DoesNotThrow(()=>VarginhaRumble.Play(.7f,.5f,.05f));yield return new WaitForSecondsRealtime(.1f);VarginhaRumble.Tick();
                var field=typeof(VarginhaRumble).GetField("_pad",BindingFlags.Static|BindingFlags.NonPublic);Assert.That(field.GetValue(null),Is.Null);
                VarginhaRumble.Play(.7f,.5f,1);VarginhaGameSettings.Current.vibration=false;VarginhaRumble.Tick();Assert.That(field.GetValue(null),Is.Null);
            }
            finally{VarginhaGameSettings.Current.vibration=enabled;VarginhaGameSettings.Current.vibrationIntensity=gain;VarginhaRumble.Stop();}
        }
        static void Capture(Camera camera,string name)
        {
            string folder=Path.Combine(Directory.GetCurrentDirectory(),"Docs","QARevisao20261007");Directory.CreateDirectory(folder);
            var original=camera.targetTexture;var active=RenderTexture.active;var rt=RenderTexture.GetTemporary(1280,960,24);var tex=new Texture2D(1280,960,TextureFormat.RGB24,false);
            try{camera.targetTexture=rt;camera.Render();RenderTexture.active=rt;tex.ReadPixels(new Rect(0,0,1280,960),0,0);tex.Apply();File.WriteAllBytes(Path.Combine(folder,name),tex.EncodeToPNG());}
            finally{camera.targetTexture=original;RenderTexture.active=active;RenderTexture.ReleaseTemporary(rt);Object.Destroy(tex);}
        }
    }
}
