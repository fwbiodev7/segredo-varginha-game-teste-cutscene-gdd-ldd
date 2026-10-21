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
        private const string Output="output/cinematic-opening-2026-10-21";
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
            var visited=new List<string>();float deadline=Time.realtimeSinceStartup+65;bool paused=false,released=false,withdrawn=false,exit=false,gate=false,signal=false;
            while(VarginhaCampaignStage.Active==stage&&Time.realtimeSinceStartup<deadline)
            {
                string title=Title(stage);
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
            foreach(string name in new[]{"FuscaCabinMorning","FuscaCabinPhotoShell","FuscaDashboardPhoto"})
            {
                var source=Resources.Load<Texture2D>("Varginha/CinemaOpening/"+name);Assert.That(source.isReadable,Is.False);
                var rt=CampaignCinemaTextures.Resolve(name) as RenderTexture;Assert.That(rt,Is.Not.Null,"The pixel shader must run on the actual GPU.");
                var image=new Texture2D(rt.width,rt.height,TextureFormat.RGBA32,false);var previous=RenderTexture.active;
                try{RenderTexture.active=rt;image.ReadPixels(new Rect(0,0,rt.width,rt.height),0,0);image.Apply();}
                finally{RenderTexture.active=previous;}
                var pixels=image.GetPixels32();Assert.That(pixels.All(p=>p.a==0||p.a==255),Is.True,"New cinematic cutouts use hard pixel alpha.");
                Assert.That(pixels.Any(p=>p.a==255),Is.True);if(name!="FuscaCabinMorning")Assert.That(pixels.Any(p=>p.a==0),Is.True);
                Assert.That(pixels.Where(p=>p.a!=0).Distinct().Count(),Is.GreaterThan(8),"Material colour variation must survive pixel resolution.");
                File.WriteAllBytes(Output+"/runtime-layers/"+name+".png",image.EncodeToPNG());Object.Destroy(image);
            }
            yield return null;
        }
    }
}
