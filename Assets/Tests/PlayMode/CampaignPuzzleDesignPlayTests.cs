using System.Collections;
using System.Collections.Generic;
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

namespace Game.Tests.PlayMode
{
    public class CampaignPuzzleDesignPlayTests : InputTestFixture
    {
        const BindingFlags Fields=BindingFlags.Public|BindingFlags.NonPublic|BindingFlags.Instance;
        string _save,_memory;
        Gamepad _pad;
        public override void Setup(){VarginhaInputActions.Shutdown();base.Setup();}
        public override void TearDown(){Time.timeScale=1;VarginhaInputActions.Shutdown();VarginhaGamepadUI.Reset();base.TearDown();}
        [SetUp] public void Preserve()
        {
            _save=File.Exists(CampaignStorySave.Path)?File.ReadAllText(CampaignStorySave.Path):null;
            _memory=File.Exists(CampaignMemorySave.Path)?File.ReadAllText(CampaignMemorySave.Path):null;
            _pad=InputSystem.AddDevice<Gamepad>();
            InputSystem.settings.backgroundBehavior=InputSettings.BackgroundBehavior.IgnoreFocus;
            InputSystem.settings.editorInputBehaviorInPlayMode=InputSettings.EditorInputBehaviorInPlayMode.AllDeviceInputAlwaysGoesToGameView;
        }
        [TearDown] public void Restore()
        {
            if(_save==null){if(File.Exists(CampaignStorySave.Path))File.Delete(CampaignStorySave.Path);}else File.WriteAllText(CampaignStorySave.Path,_save);
            if(_memory==null){if(File.Exists(CampaignMemorySave.Path))File.Delete(CampaignMemorySave.Path);}else File.WriteAllText(CampaignMemorySave.Path,_memory);
        }
        static void ShowGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorWindow.GetWindow(typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.GameView")).Show();
#endif
        }
        static Rect Selection()
        {
            var context=typeof(VarginhaGamepadUI).GetField("_active",BindingFlags.Static|BindingFlags.NonPublic).GetValue(null);
            if(context==null)return default;
            var selection=context.GetType().GetField("selection",Fields).GetValue(context);
            return selection==null?default:(Rect)selection.GetType().GetField("local",Fields).GetValue(selection);
        }
        IEnumerator Pad(GamepadButton button)
        {
            for(int i=0;i<2;i++){InputSystem.QueueStateEvent(_pad,new GamepadState().WithButton(button));yield return null;}
            for(int i=0;i<2;i++){InputSystem.QueueStateEvent(_pad,new GamepadState());yield return null;}
        }
        IEnumerator Click(Rect target)
        {
            yield return null;
            for(int attempt=0;attempt<25&&!Selection().Equals(target);attempt++)
            {
                var delta=target.center-Selection().center;
                var key=Mathf.Abs(delta.x)>180?delta.x>0?GamepadButton.DpadRight:GamepadButton.DpadLeft
                    :delta.y>0?GamepadButton.DpadDown:GamepadButton.DpadUp;
                yield return Pad(key);
            }
            Assert.That(Selection(),Is.EqualTo(target),"Controller must reach the actual puzzle control");
            yield return Pad(GamepadButton.South);
        }
        static IEnumerator Capture(string name)
        {
            Directory.CreateDirectory("Docs/QANovaCampanha20261009");
            yield return new WaitForEndOfFrame();
            var image=ScreenCapture.CaptureScreenshotAsTexture();
            File.WriteAllBytes("Docs/QANovaCampanha20261009/"+name+".png",image.EncodeToPNG());Object.Destroy(image);
        }
        [UnityTest,Timeout(180000)]
        public IEnumerator SpatialMapCanBePlacedRotatedAndConfirmedUsingTheController()
        {
            var story=new CampaignStory{phase=6};story.expansion.visited=7;CampaignStorySave.Write(story);
            yield return SceneManager.LoadSceneAsync(CampaignStorySave.Scene(6));yield return new WaitForSeconds(3.2f);
            var c=CampaignExpansionController.Active;ShowGame();VarginhaGamepadUI.Reset();VarginhaInputActions.Rebuild();
            c.Interact("map");yield return new WaitForSecondsRealtime(.3f);
            yield return Click(new Rect(165,292,290,35));yield return Click(new Rect(165,396,290,44));
            yield return Click(new Rect(485,292,290,35));yield return Click(new Rect(485,396,290,44));
            Assert.That(c.Progress.expansion.mapOrder,Is.EqualTo(new[]{0,1,2}));
            Rect[] rotate={new Rect(165,443,290,32),new Rect(485,443,290,32),new Rect(805,391,290,32)};
            int[] directions={1,1,0};
            for(int i=0;i<3;i++)while(c.Progress.expansion.mapDirections[i]!=directions[i])yield return Click(rotate[i]);
            yield return Capture("01_Fragmentos_Trilha");
            yield return Click(new Rect(805,500,290,42));
            Assert.That(c.Progress.expansion.mapSolved,Is.True);
            Assert.That(CampaignStorySave.Load().expansion.mapSolved,Is.True);
            Assert.That(CampaignGuidance.Target(c.Progress,6),Is.EqualTo("exit"));
            yield return SceneManager.LoadSceneAsync("Menu_MisterioDeVarginha");
        }
        [UnityTest,Timeout(240000)]
        public IEnumerator FiveDifferentPuzzleInterfacesAcceptControllerAnswersAndPreservePartialProgress()
        {
            foreach(int phase in new[]{11,13,14,15,18})
            {
                var story=new CampaignStory{phase=phase};var definition=CampaignContinuationDefinition.Get(phase);
                story.continuation.clues[phase-11]=(1<<definition.required)-1;
                if(phase==11||phase==14)story.continuation.clues[phase-11]|=128;
                CampaignStorySave.Write(story);
                yield return SceneManager.LoadSceneAsync(CampaignStorySave.Scene(phase));yield return new WaitForSeconds(3.2f);
                var c=CampaignContinuationController.Active;c.CloseMessage();c.OpenPuzzle();
                ShowGame();VarginhaGamepadUI.Reset();VarginhaInputActions.Rebuild();yield return new WaitForSecondsRealtime(.3f);
                Assert.That(c.Submit(phase==11?0:-1,new[]{0,0,0}),Is.False,"Wrong answers do not advance "+phase);
                c.CloseMessage();
                if(phase==11)
                {
                    yield return Click(new Rect(840,336,275,27));yield return Click(new Rect(840,366,275,27));
                    yield return Capture("02_Arquivos_Comparacao");
                    yield return Click(new Rect(755,530,360,32));
                }
                else if(phase==15)
                {
                    foreach(int x in new[]{390,880,635,145})yield return Click(new Rect(x,352,230,115));
                    yield return Capture("05_Memoria_1996");
                    yield return Click(new Rect(755,530,360,32));
                }
                else
                {
                    var solution=CampaignPuzzleDesign.Solution(phase);
                    for(int row=0;row<solution.Length;row++)
                    {
                        int evidenceRow=(solution[row]+solution.Length-1)%solution.Length;
                        yield return Click(new Rect(145,352+evidenceRow*43,425,39));
                        yield return Click(new Rect(600,352+row*43,515,39));
                        if(phase==13&&row==0)
                        {
                            var saved=CampaignStorySave.Load();Assert.That(saved.puzzles.causes[0],Is.EqualTo(0));
                            Assert.That(saved.puzzles.causes[1],Is.EqualTo(-1));
                            yield return SceneManager.LoadSceneAsync(CampaignStorySave.Scene(phase));yield return new WaitForSeconds(3.2f);
                            c=CampaignContinuationController.Active;c.CloseMessage();c.OpenPuzzle();yield return new WaitForSecondsRealtime(.3f);
                            Assert.That(c.Progress.puzzles.causes[0],Is.EqualTo(0));
                        }
                    }
                    yield return Capture(phase==13?"03_Casarao_Causas":phase==14?"04_Tombo_Datas":"06_Criatura_Provas");
                    yield return Click(new Rect(755,530,360,32));
                }
                Assert.That(c.Progress.continuation.solved[phase-11],Is.True,"New puzzle completed "+phase);
                Assert.That(CampaignStorySave.Load().continuation.solved[phase-11],Is.True,"New answer persists "+phase);
                yield return new WaitForSeconds(4);
            }
            yield return SceneManager.LoadSceneAsync("Menu_MisterioDeVarginha");
        }
        [UnityTest,Timeout(180000)]
        public IEnumerator MansionDoorsConnectTheHallLivingRoomOfficeAndServicePassageWithRealMovement()
        {
            var story=new CampaignStory{phase=12};story.continuation.area=1;story.continuation.clues[1]=1;story.continuation.serviceRevealed=true;
            CampaignStorySave.Write(story);
            yield return SceneManager.LoadSceneAsync(CampaignContinuationDefinition.SceneName(12,1));yield return new WaitForSeconds(3.2f);
            var c=CampaignContinuationController.Active;c.CloseMessage();var data=CampaignIllustratedMaps.Get(112);
            Assert.That(CampaignGuidance.Target(c.Progress,12,1),Is.EqualTo("key"));
            yield return Walk(c,data.Position(325,350));Assert.That(CampaignGuidance.RoomName(c.Plan,c.Feet),Is.EqualTo("Sala de estar"));
            yield return Walk(c,data.Position(525,380));Assert.That(CampaignGuidance.RoomName(c.Plan,c.Feet),Is.EqualTo("Hall central"));
            yield return Capture("07_Casarao_Hall_Portas");
            yield return Walk(c,data.Position(748,329));Assert.That(CampaignGuidance.RoomName(c.Plan,c.Feet),Is.EqualTo("Escritório"));
            yield return Capture("08_Casarao_Escritorio");
            yield return Walk(c,c.Plan.points.Find(p=>p.id=="key").position);yield return Pad(GamepadButton.South);c.CloseMessage();
            Assert.That(c.Progress.continuation.serviceKey,Is.True);
            yield return Walk(c,data.Position(820,200));Assert.That(CampaignGuidance.RoomName(c.Plan,c.Feet),Is.EqualTo("Corredor de serviço"));
            yield return Capture("09_Casarao_Corredor");
            yield return Pad(GamepadButton.South);yield return new WaitForSeconds(3);
            Assert.That(SceneManager.GetActiveScene().name,Is.EqualTo(CampaignStorySave.Scene(13)));
            yield return SceneManager.LoadSceneAsync("Menu_MisterioDeVarginha");
        }
        [UnityTest,Timeout(120000)]
        public IEnumerator BasementStairsKeepTheNextChapterSeparateFromReturningUpstairs()
        {
            var story=new CampaignStory{phase=13};story.continuation.solved[2]=true;
            CampaignStorySave.Write(story);
            yield return SceneManager.LoadSceneAsync(CampaignStorySave.Scene(13));yield return new WaitForSeconds(3.2f);
            var c=CampaignContinuationController.Active;c.CloseMessage();
            var exit=c.Plan.points.Find(p=>p.id=="exit");var ground=c.Plan.points.Find(p=>p.id=="ground");
            Assert.That(Vector2.Distance(exit.position,ground.position),Is.GreaterThan(1.35f));
            Assert.That(CampaignGuidance.Target(c.Progress,13),Is.EqualTo("exit"));
            yield return Walk(c,exit.position);
            var nearest=typeof(CampaignContinuationController).GetMethod("Nearest",Fields);
            Assert.That(((CampaignMapPlan.Point)nearest.Invoke(c,null)).id,Is.EqualTo("exit"));
            yield return Pad(GamepadButton.South);
            Assert.That(typeof(CampaignContinuationController).GetField("_panel",Fields).GetValue(c),Is.EqualTo("complete"));
            yield return Pad(GamepadButton.East);
            Assert.That(typeof(CampaignContinuationController).GetField("_panel",Fields).GetValue(c),Is.Null);
            yield return Walk(c,ground.position);
            Assert.That(((CampaignMapPlan.Point)nearest.Invoke(c,null)).id,Is.EqualTo("ground"));
            yield return Pad(GamepadButton.South);yield return new WaitForSeconds(3);
            Assert.That(SceneManager.GetActiveScene().name,Is.EqualTo(CampaignContinuationDefinition.SceneName(12,1)));
            Assert.That(CampaignStorySave.Load().continuation.solved[2],Is.True);
            yield return SceneManager.LoadSceneAsync("Menu_MisterioDeVarginha");
        }
        [UnityTest,Timeout(120000)]
        public IEnumerator CoveredOfficeSofaBlocksRealMovementFromEveryAccessibleSide()
        {
            var story=new CampaignStory{phase=12};story.continuation.area=1;
            CampaignStorySave.Write(story);
            yield return SceneManager.LoadSceneAsync(CampaignContinuationDefinition.SceneName(12,1));yield return new WaitForSeconds(3.2f);
            var c=CampaignContinuationController.Active;c.CloseMessage();
            var body=c.Actor.GetComponent<Rigidbody2D>();var feet=c.Actor.GetComponent<CircleCollider2D>();
            var sofa=c.transform.Find("Mapa_Campanha/02_Mobilia_Colisoes/Sofá coberto do escritório").GetComponent<PolygonCollider2D>();
            int contacts=0;float capture=Time.captureDeltaTime;Time.captureDeltaTime=Time.fixedDeltaTime;
            try
            {
                foreach(var direction in new[]{Vector2.left,Vector2.right,Vector2.up,Vector2.down})
                {
                    var bounds=sofa.bounds;float extent=direction.x!=0?bounds.extents.x:bounds.extents.y;
                    var start=(Vector2)bounds.center-direction*(extent+feet.radius+.20f);
                    if(!c.Plan.IsClear(start,feet.radius))continue;
                    body.position=start-feet.offset;body.linearVelocity=Vector2.zero;c.Actor.SetInputLocked(false);
                    Physics2D.SyncTransforms();yield return new WaitForFixedUpdate();
                    float until=Time.time+.4f;
                    while(Time.time<until)
                    {
                        InputSystem.QueueStateEvent(_pad,new GamepadState{leftStick=direction});yield return null;
                        Assert.That(feet.Distance(sofa).distance,Is.GreaterThanOrEqualTo(-.025f),"Office sofa blocks "+direction);
                    }
                    InputSystem.QueueStateEvent(_pad,new GamepadState());yield return new WaitForFixedUpdate();
                    Assert.That(Vector2.Dot(body.position+feet.offset-start,direction),Is.GreaterThan(.025f),"Actual controller movement "+direction);
                    contacts++;
                }
                Assert.That(contacts,Is.GreaterThan(0),"At least one accessible physical approach to the office sofa");
            }
            finally{Time.captureDeltaTime=capture;}
            yield return SceneManager.LoadSceneAsync("Menu_MisterioDeVarginha");
        }
        IEnumerator Walk(CampaignContinuationController c,Vector2 target)
        {
            var route=new List<Vector2>();Assert.That(c.Plan.Route(c.Feet,target,route),Is.True,"Authored route to "+target);route.Add(target);
            var body=c.Actor.GetComponent<Rigidbody2D>();var feet=c.Actor.GetComponent<CircleCollider2D>();
            float capture=Time.captureDeltaTime;Time.captureDeltaTime=Time.fixedDeltaTime;
            try
            {
                foreach(var point in route)
                {
                    float deadline=Time.time+3;
                    while(Vector2.Distance(body.position+feet.offset,point)>.035f)
                    {
                        Assert.That(Time.time,Is.LessThan(deadline),"Real movement reaches "+point+" from "+c.Feet);
                        Vector2 delta=point-(body.position+feet.offset);
                        float throttle=.2f+.8f*Mathf.Clamp01(delta.magnitude/.20f);
                        InputSystem.QueueStateEvent(_pad,new GamepadState{leftStick=delta.normalized*throttle});yield return null;
                    }
                }
                InputSystem.QueueStateEvent(_pad,new GamepadState());yield return null;yield return new WaitForFixedUpdate();
            }
            finally {Time.captureDeltaTime=capture;}
        }
    }
}
