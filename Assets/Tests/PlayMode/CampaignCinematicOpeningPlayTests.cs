using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Game.Varginha;
using Game.Varginha.Experiment;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace Game.Tests.PlayMode
{
    public sealed class CampaignCinematicOpeningPlayTests:InputTestFixture
    {
        private string _story,_memory;private bool _reduced;
        private static string Output=>CinemaQaEvidence.Folder;
        private const BindingFlags Hidden=BindingFlags.Instance|BindingFlags.NonPublic;
        private static string Title(VarginhaCampaignStage s)=>(string)typeof(VarginhaCampaignStage).GetField("_cinemaTitle",Hidden).GetValue(s);
        private static float Clock(VarginhaCampaignStage s)=>Time.time-(float)typeof(VarginhaCampaignStage).GetField("_cinemaClock",Hidden).GetValue(s);
        public override void Setup(){VarginhaInputActions.Shutdown();base.Setup();}
        public override void TearDown(){VarginhaInputActions.Shutdown();base.TearDown();}
        [SetUp] public void Preserve()
        {
            _story=File.Exists(CampaignStorySave.Path)?File.ReadAllText(CampaignStorySave.Path):null;
            _memory=File.Exists(CampaignMemorySave.Path)?File.ReadAllText(CampaignMemorySave.Path):null;
            _reduced=VarginhaGameSettings.Current.reducedMotion;
        }
        [UnityTearDown] public IEnumerator Restore()
        {
            Time.timeScale=1;yield return SceneManager.LoadSceneAsync("Menu_MisterioDeVarginha");yield return null;
            if(_story==null){if(File.Exists(CampaignStorySave.Path))File.Delete(CampaignStorySave.Path);}else File.WriteAllText(CampaignStorySave.Path,_story);
            if(_memory==null){if(File.Exists(CampaignMemorySave.Path))File.Delete(CampaignMemorySave.Path);}else File.WriteAllText(CampaignMemorySave.Path,_memory);
            VarginhaGameSettings.Current.reducedMotion=_reduced;CampaignCinemaTextures.Clear();
        }
        private static IEnumerator House(int watched,bool reduced)
        {
            VarginhaGameSettings.Current.reducedMotion=reduced;
            CampaignStorySave.Write(new CampaignStory{phase=2,routine=15,boxFound=true,pagesSolved=true,cinematicsSeen=watched});
            yield return SceneManager.LoadSceneAsync(CampaignStorySave.Scene(2));yield return new WaitForSeconds(3.3f);
            VarginhaCampaignStage.Active.CloseDialogue();
        }
        private static IEnumerator Capture(string name)
        {
            // Batch Editor does not dispatch WaitForEndOfFrame; visual evidence comes
            // from the actual test Player. Editor still validates state and timing.
            if(Application.isEditor)yield break;
            yield return new WaitForEndOfFrame();var image=ScreenCapture.CaptureScreenshotAsTexture();
            Directory.CreateDirectory(Output+"/captures");File.WriteAllBytes(Output+"/captures/"+name+".png",image.EncodeToPNG());Object.Destroy(image);
        }
        [UnityTest,Timeout(90000)] public IEnumerator AllFuscaBeatsRenderInOrderPauseAndRestoreSchoolControl()
        {
            yield return House(0,false);var stage=VarginhaCampaignStage.Active;stage.Interact("car");
            var recorder=new GameObject("QA_Cinema_Recorder").AddComponent<CinemaFrameRecorder>();
            Assert.That(stage.TrySkipMissionCinema(),Is.False,"A first viewing cannot be skipped.");
            var visited=new List<string>();var sounds=new HashSet<string>();int soundSerial=-1;float deadline=Time.realtimeSinceStartup+65;bool paused=false,released=false,withdrawn=false,exit=false,gate=false,signal=false;
            var driverCaptured=new bool[4];
            while(VarginhaCampaignStage.Active==stage&&Time.realtimeSinceStartup<deadline)
            {
                var sound=stage.GetComponent<CampaignSoundscape>();if(sound.EventSerial!=soundSerial){sounds.Add(sound.LastEvent??"");soundSerial=sound.EventSerial;}
                string title=Title(stage);
                if(title=="VARGINHA • 2026")
                {
                    int frame=CampaignFuscaCinema.DriverFrame(Clock(stage),true,false);
                    if(!driverCaptured[frame]){driverCaptured[frame]=true;yield return Capture("driving-pose-"+frame);}
                }
                if(title=="O RÁDIO"&&Clock(stage)>.59f&&!signal){signal=true;yield return Capture("signal-variation");}
                if(title=="INDUSTRIAL"&&Clock(stage)>.7f&&!exit){exit=true;yield return Capture("school-exit-car");}
                if(title=="INDUSTRIAL"&&Clock(stage)>5.55f&&!gate){gate=true;yield return Capture("school-arrival-gate");}
                if(title=="SILÊNCIO"&&Clock(stage)>1.05f&&!released){released=true;yield return Capture("ignition-release");}
                if(title=="SILÊNCIO"&&Clock(stage)>1.8f&&!withdrawn){withdrawn=true;yield return Capture("ignition-key-remains");}
                if(CampaignFuscaCinema.Shots.Any(s=>s.title==title)&&!visited.Contains(title)&&Clock(stage)>.25f)
                {
                    visited.Add(title);yield return Capture("fusca-"+visited.Count.ToString("00"));
                    if(title=="UMA VOZ CONHECIDA")
                    {
                        float before=Clock(stage);stage.TogglePause();yield return new WaitForSecondsRealtime(.18f);
                        Assert.That(Clock(stage),Is.EqualTo(before).Within(.001f));Assert.That(stage.TrySkipMissionCinema(),Is.False);
                        stage.TogglePause();paused=true;
                    }
                }
                Assert.That(Object.FindAnyObjectByType<EdelzioTopDownController>().IsInputLocked,Is.True);
                yield return null;
            }
            CollectionAssert.AreEqual(CampaignFuscaCinema.Shots.Select(s=>s.title).ToArray(),visited);
            foreach(string cue in new[]{"DoorOpen","Starter","CarFail","RadioInterference"})Assert.That(sounds.Contains(cue),Is.True,cue);
            if(recorder!=null)Object.Destroy(recorder.gameObject); // scene changes may have already removed the QA recorder
            Assert.That(paused,Is.True);Assert.That(CampaignStorySave.Load().cinematicsSeen&8,Is.EqualTo(8));
            Assert.That(released&&withdrawn,Is.True,"All ignition poses must be reached before spontaneous restart.");
            Assert.That(exit&&gate,Is.True,"Disembarkation and the walk to the gameplay spawn must finish.");
            yield return new WaitForSeconds(3.4f);
            Assert.That(VarginhaCampaignStage.Active.phase,Is.EqualTo(4));Assert.That(VarginhaCampaignStage.Active.Progress.arrival,Is.True);
            var actor=Object.FindAnyObjectByType<EdelzioTopDownController>();
            float releaseDeadline=Time.realtimeSinceStartup+8;
            while(actor.IsInputLocked&&Time.realtimeSinceStartup<releaseDeadline)yield return null;
            Assert.That(actor.IsInputLocked,Is.False,"The school title/fade must finish before control is returned.");
            Assert.That(actor.GetComponent<SpriteRenderer>().enabled,Is.True);Assert.That(actor.GetComponent<Rigidbody2D>().simulated,Is.True);
            yield return Capture("school-control-restored");
        }
        [UnityTest,Timeout(30000)] public IEnumerator SeenMovieCanSkipInReducedMotionWithoutRepeatingOrLosingItems()
        {
            yield return House(8,true);var stage=VarginhaCampaignStage.Active;stage.Interact("car");
            yield return new WaitForSeconds(.75f);
            Assert.That(stage.CanSkipMissionCinema,Is.True);stage.TogglePause();Assert.That(stage.TrySkipMissionCinema(),Is.False);
            stage.TogglePause();Assert.That(stage.TrySkipMissionCinema(),Is.True);
            float deadline=Time.realtimeSinceStartup+8;
            while((VarginhaCampaignStage.Active==stage||CampaignCinematics.IsTransitioning)&&Time.realtimeSinceStartup<deadline)yield return null;
            yield return new WaitForSeconds(3.4f);
            var story=CampaignStorySave.Load();Assert.That(story.phase,Is.EqualTo(4));Assert.That(story.house.Complete&&story.pagesSolved&&story.boxFound&&story.arrival,Is.True);
            Assert.That(story.cinematicsSeen&8,Is.EqualTo(8));Assert.That(Object.FindAnyObjectByType<EdelzioTopDownController>().HasFuscaKey,Is.True);
        }
        [UnityTest] public IEnumerator PixelResolvePreservesOpaqueInteriorAndWindowHolesOnGpu()
        {
            Directory.CreateDirectory(Output+"/runtime-layers");
            foreach(string path in new[]{"Varginha/CinemaOpening/MorningTown","Varginha/CinemaOpening/ChildRoom1996","Varginha/CinemaOpening/Newspaper1996","Varginha/CinemaOpening/ChildWatchingTV","Varginha/CinemaOpening/FuscaCabinCartoon","Varginha/CinemaOpening/FuscaDashboardCartoon",CampaignTrailMapArt.TerrainPath,CampaignTrailMapArt.LandmarksPath})
            {
                string name=path.Substring(path.LastIndexOf('/')+1);
                var source=Resources.Load<Texture2D>(path);Assert.That(source,Is.Not.Null);Assert.That(source.isReadable,Is.False);
                var rt=CampaignCinemaTextures.ResolveResource(path) as RenderTexture;Assert.That(rt,Is.Not.Null,"The pixel shader must run on the actual GPU.");
                var image=new Texture2D(rt.width,rt.height,TextureFormat.RGBA32,false);var previous=RenderTexture.active;
                try{RenderTexture.active=rt;image.ReadPixels(new Rect(0,0,rt.width,rt.height),0,0);image.Apply();}
                finally{RenderTexture.active=previous;}
                var pixels=image.GetPixels32();Assert.That(pixels.All(p=>p.a==0||p.a==255),Is.True,"New cinematic cutouts use hard pixel alpha.");
                Assert.That(pixels.Any(p=>p.a==255),Is.True);if(name=="FuscaCabinCartoon"||name=="FuscaDashboardCartoon"||name=="ChildWatchingTV"||name=="Landmarks")Assert.That(pixels.Any(p=>p.a==0),Is.True);
                Assert.That(pixels.Where(p=>p.a!=0).Distinct().Count(),Is.GreaterThan(8),"Material colour variation must survive pixel resolution.");
                File.WriteAllBytes(Output+"/runtime-layers/"+name+".png",image.EncodeToPNG());Object.Destroy(image);
            }
            yield return null;
        }
        [UnityTest,Timeout(90000)] public IEnumerator NewspaperAndTvRevealDeliverControlBeforeThePowerFailure()
        {
            CampaignMemorySave.Write(new CampaignMemory());CampaignStorySave.Write(new CampaignStory{phase=1});
            yield return SceneManager.LoadSceneAsync(VarginhaCampaignPhase1.SceneName);yield return null;yield return null;
            var phase=VarginhaCampaignPhase1.Active;var seen=new HashSet<string>();var child=new bool[8];bool paused=false,room=false;
            Assert.That(phase.GetComponents<AudioSource>().Any(s=>s.clip==CampaignRecordedAudio.Clip("TelevisionAmbience")),Is.True);
            var recorder=new GameObject("QA_Prologue_Recorder").AddComponent<CinemaFrameRecorder>();recorder.folderName="opening-film";
            float deadline=Time.realtimeSinceStartup+45;
            while(!phase.IsExploring&&Time.realtimeSinceStartup<deadline)
            {
                Assert.That(phase.Memory.powerFailed,Is.False,"The anomaly must wait for gameplay.");Assert.That(phase.TrySkipOpening(),Is.False);
                var timeline=(ExperimentTimeline)typeof(VarginhaCampaignPhase1).GetField("_timeline",Hidden).GetValue(phase);
                if(!timeline.Finished)
                {
                    var shot=timeline.Shots[timeline.ShotIndex];
                    if(timeline.ShotTime>.3f&&seen.Add(shot.id))yield return Capture("opening-"+shot.id);
                    if(shot.id=="tv_reveal"&&timeline.ShotTime>3.55f&&!room){room=true;yield return Capture("opening-room-revealed");}
                    if(shot.id=="child_1996")
                    {int frame=CampaignOpeningCinema.ChildFrame(timeline.ShotTime,false);if(!child[frame]){child[frame]=true;yield return Capture("child-pose-"+frame);}}
                    if(shot.id=="news_1996"&&!paused)
                    {float before=timeline.Time;phase.TogglePause();yield return new WaitForSecondsRealtime(.2f);Assert.That(timeline.Time,Is.EqualTo(before));phase.TogglePause();paused=true;}
                }
                yield return null;
            }
            Assert.That(phase.IsExploring,Is.True);Assert.That(seen.Count,Is.EqualTo(4));Assert.That(child.All(x=>x),Is.True);
            var actor=Object.FindAnyObjectByType<EdelzioTopDownController>();Assert.That(actor.IsInputLocked,Is.False);
            Assert.That(phase.Memory.openingSeen,Is.True);Assert.That(phase.Memory.powerFailed,Is.False);
            yield return Capture("opening-control");Object.Destroy(recorder.gameObject);phase.TriggerPowerFailure();yield return null;yield return Capture("opening-power-peak");
            Assert.That(phase.Memory.powerFailed,Is.True);
            yield return SceneManager.LoadSceneAsync(VarginhaCampaignPhase1.SceneName);yield return null;yield return null;
            Assert.That(VarginhaCampaignPhase1.Active.IsExploring,Is.True,"Continue must not replay the montage.");
            VarginhaCampaignPhase1.Active.PlayOpening();Assert.That(VarginhaCampaignPhase1.Active.TrySkipOpening(),Is.True);
        }
        [UnityTest]public IEnumerator AdultHouseHasDaylightGlassWithoutChangingFurnitureOrProgress()
        {
            CampaignStorySave.Write(new CampaignStory{phase=2});yield return SceneManager.LoadSceneAsync(CampaignStorySave.Scene(2));yield return new WaitForSeconds(3.4f);
            var root=GameObject.Find("Mapa_Campanha").transform.Find("Manha_07h");Assert.That(root,Is.Not.Null);
            Assert.That(root.childCount,Is.EqualTo(CampaignAdultMorning.GlassPixels.Length));Assert.That(root.GetComponentsInChildren<Collider2D>(),Is.Empty);
            foreach(var sprite in root.GetComponentsInChildren<SpriteRenderer>())
            {Assert.That(sprite.sprite.texture.name,Is.EqualTo("MorningTown"));Assert.That(sprite.sortingOrder,Is.GreaterThan(22000),"Daylight glass must stay above lower wall sorting groups.");}
            Assert.That(GameObject.Find("Mapa_Campanha").transform.Find("01_Planta_Paredes_Divisoes/Arte_Integrada_0").GetComponent<SpriteRenderer>().sharedMaterial.shader.name,Is.EqualTo("Varginha/IllustratedMorning"));
            Assert.That(VarginhaCampaignStage.Active.Progress.routine,Is.Zero);Assert.That(VarginhaCampaignStage.Active.Progress.house.step,Is.Zero);
            yield return Capture("adult-morning-07h");
            var data=CampaignIllustratedMaps.Get(2);var camera=Camera.main;var previous=camera.transform.position;float size=camera.orthographicSize;
            var follow=camera.GetComponent<Game.Level.CameraFollow2D>();bool followEnabled=follow!=null&&follow.enabled;if(follow!=null)follow.enabled=false;
            camera.transform.position=new Vector3(data.Bounds.center.x,data.Bounds.center.y,-10);camera.orthographicSize=data.Bounds.height*.6f;yield return null;yield return Capture("adult-morning-overview");
            camera.transform.position=previous;camera.orthographicSize=size;if(follow!=null)follow.enabled=followEnabled;
        }
    }
}
