using System.Collections;
using System.IO;
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
    // Exercise real exit handlers and scene loads, retaining both sides for visual review.
    public sealed class CampaignChapterExitReviewTests : InputTestFixture
    {
        string _story,_memory;
        bool _background;
        public override void Setup(){VarginhaInputActions.Shutdown();base.Setup();}
        public override void TearDown(){Time.timeScale=1;VarginhaInputActions.Shutdown();base.TearDown();}
        [SetUp] public void Preserve()
        {
            _story=Read(CampaignStorySave.Path);_memory=Read(CampaignMemorySave.Path);
            _background=Application.runInBackground;Application.runInBackground=true;
            InputSystem.settings.backgroundBehavior=InputSettings.BackgroundBehavior.IgnoreFocus;
        }
        static string Read(string path)=>File.Exists(path)?File.ReadAllText(path):null;
        static void Restore(string path,string text){if(text==null){if(File.Exists(path))File.Delete(path);}else File.WriteAllText(path,text);}
        [UnityTearDown] public IEnumerator Restore()
        {
            Time.timeScale=1;yield return SceneManager.LoadSceneAsync("Menu_MisterioDeVarginha");
            Restore(CampaignStorySave.Path,_story);Restore(CampaignMemorySave.Path,_memory);
            Application.runInBackground=_background;
        }
        static CampaignStory Ready(int phase)
        {
            var story=new CampaignStory{phase=phase};
            if(phase>=4){story.school.MigrateCompleted();story.SyncSchoolCompletion();}
            // Complete only the source and its prerequisites. The destination
            // must arrive with its own objectives still available to the player.
            var e=story.expansion;
            if(phase>=6){e.visited=7;e.mapSolved=true;}
            if(phase>=7){e.forestSigns=3;e.fabioMet=true;}
            if(phase>=8){e.anchorFound=true;e.anchorClues=7;}
            if(phase>=9)e.evidencePresented=e.reagentUnlocked=true;
            if(phase>=10){e.stabilized=e.workshopParked=e.workshopDeparted=true;e.sprayed=7;}
            var c=story.continuation;
            for(int i=0;i<c.solved.Length;i++)c.solved[i]=i+11<=phase||(phase==18&&i==8);
            c.serviceKey=c.serviceRevealed=phase>=12;
            if(phase>=20){c.manifestationDispelled=c.finalCalibrated=true;c.chambersPrepared=7;c.finalRegulators=new[]{1,0,1};}
            return story;
        }
        static IEnumerator Capture(string name)
        {
            if(Application.isEditor)yield break;
            yield return new WaitForEndOfFrame();var image=ScreenCapture.CaptureScreenshotAsTexture();
            string folder=Path.Combine(CinemaQaEvidence.Folder,"chapter-exits");Directory.CreateDirectory(folder);
            File.WriteAllBytes(Path.Combine(folder,name+".png"),image.EncodeToPNG());Object.Destroy(image);
        }
        static IEnumerator Arrive(int phase,string label)
        {
            string scene=CampaignStorySave.Scene(phase);float deadline=Time.realtimeSinceStartup+20;
            while((CampaignCinematics.IsTransitioning||SceneManager.GetActiveScene().name!=scene)&&Time.realtimeSinceStartup<deadline)yield return null;
            Assert.That(SceneManager.GetActiveScene().name,Is.EqualTo(scene),label);
            Assert.That(CampaignStorySave.Load().phase,Is.EqualTo(phase),"Save advances exactly once: "+label);
            if(phase>=11)Assert.That(CampaignStorySave.Load().continuation.solved[phase-11],Is.False,"The next investigation must not be pre-completed by the exit fixture.");
            Assert.That(CampaignCinematics.IsTransitioning,Is.False,label);
            Assert.That(Time.timeScale,Is.EqualTo(1),"Scene transition releases its pause.");
            yield return new WaitForSecondsRealtime(3.5f);
            CampaignExpansionController.Active?.ClosePanel();CampaignContinuationController.Active?.CloseMessage();
            Assert.That(Camera.main,Is.Not.Null);
            yield return Capture(label+"-arrival");
        }
        IEnumerator Review(int phase)
        {
            var story=Ready(phase);CampaignStorySave.Write(story);
            yield return SceneManager.LoadSceneAsync(CampaignStorySave.Scene(phase));yield return new WaitForSecondsRealtime(3.5f);
            string label=phase.ToString("00")+"-to-"+CampaignSequence.Next(phase).ToString("00");
            if(phase==4)
            {
                var c=VarginhaCampaignStage.Active;Assert.That(c.ContinueChapter(),Is.False);
                c.Interact("notebook");Assert.That(c.PrepareLibraryDeparture(),Is.True);yield return Capture(label+"-complete");
                Assert.That(c.ContinueChapter(),Is.True);Assert.That(c.ContinueChapter(),Is.False);
            }
            else if(phase<11)
            {
                var c=CampaignExpansionController.Active;c.ClosePanel();Assert.That(c.ContinueChapter(),Is.False);
                c.Interact("exit");yield return Capture(label+"-complete");
                Assert.That(c.ContinueChapter(),Is.True);Assert.That(c.ContinueChapter(),Is.False);
            }
            else
            {
                var c=CampaignContinuationController.Active;c.CloseMessage();Assert.That(c.ContinueChapter(),Is.False);
                c.Interact("exit");yield return Capture(label+"-complete");
                Assert.That(c.ContinueChapter(),Is.True);Assert.That(c.ContinueChapter(),Is.False);
            }
            yield return Arrive(CampaignSequence.Next(phase),label);
        }
        [UnityTest,Timeout(60000)] public IEnumerator SchoolToLibrary()=>Review(4);
        [UnityTest,Timeout(60000)] public IEnumerator LibraryToForest()=>Review(6);
        [UnityTest,Timeout(60000)] public IEnumerator ForestToChurch()=>Review(7);
        [UnityTest,Timeout(60000)] public IEnumerator ChurchToOuzana()=>Review(8);
        [UnityTest,Timeout(60000)] public IEnumerator OuzanaToWorkshop()=>Review(9);
        [UnityTest,Timeout(60000)] public IEnumerator WorkshopToArchives()=>Review(10);
        [UnityTest,Timeout(60000)] public IEnumerator ArchivesToHouse()=>Review(11);
        [UnityTest,Timeout(60000)] public IEnumerator BasementToFabio()=>Review(13);
        [UnityTest,Timeout(60000)] public IEnumerator FabioToMemory()=>Review(14);
        [UnityTest,Timeout(60000)] public IEnumerator MemoryToCreature()=>Review(15);
        [UnityTest,Timeout(60000)] public IEnumerator CreatureToManifestation()=>Review(18);
        [UnityTest,Timeout(60000)] public IEnumerator ManifestationToReturn()=>Review(20);
        [UnityTest,Timeout(60000)] public IEnumerator ChildhoodToAdultCannotAdvanceTwice()
        {
            CampaignStorySave.Write(Ready(1));CampaignMemorySave.Write(new CampaignMemory{complete=true,openingSeen=true});
            yield return SceneManager.LoadSceneAsync(CampaignStorySave.Scene(1));yield return new WaitForSecondsRealtime(3.5f);
            var c=VarginhaCampaignPhase1.Active;yield return Capture("01-to-02-complete");
            Assert.That(c.ContinueChapter(),Is.True);Assert.That(c.ContinueChapter(),Is.False);yield return Arrive(2,"01-to-02");
        }
        [UnityTest,Timeout(60000)] public IEnumerator HouseServiceDoorOpensTheBasement()
        {
            var story=Ready(12);story.continuation.area=1;CampaignStorySave.Write(story);
            yield return SceneManager.LoadSceneAsync(CampaignContinuationDefinition.SceneName(12,1));yield return new WaitForSecondsRealtime(3.5f);
            var c=CampaignContinuationController.Active;c.CloseMessage();yield return Capture("12-to-13-service");
            c.Interact("service");yield return Arrive(13,"12-to-13");
        }
        [UnityTest,Timeout(60000)] public IEnumerator EpilogueCreditsReturnToMenuWithTheCompletedSave()
        {
            var story=Ready(21);story.continuation.area=1;story.continuation.finalStep=4;CampaignStorySave.Write(story);
            yield return SceneManager.LoadSceneAsync(CampaignContinuationDefinition.SceneName(21,1));yield return new WaitForSecondsRealtime(3.5f);
            var c=CampaignContinuationController.Active;c.CloseMessage();c.Interact("renan");c.CloseMessage();
            c.Interact("notebook");c.CloseMessage();yield return Capture("21-epilogue");c.Interact("fusca");c.CloseMessage();
            Assert.That(c.CreditsVisible,Is.True);Assert.That(CampaignStorySave.Load().continuation.finished,Is.True);
            yield return new WaitForSecondsRealtime(4);yield return Capture("21-credits");
            var keyboard=InputSystem.AddDevice<Keyboard>();VarginhaInputActions.Rebuild();
            InputSystem.QueueStateEvent(keyboard,new KeyboardState(Key.Enter));yield return null;yield return null;
            InputSystem.QueueStateEvent(keyboard,new KeyboardState());
            float deadline=Time.realtimeSinceStartup+20;
            while((CampaignCinematics.IsTransitioning||SceneManager.GetActiveScene().name!="Menu_MisterioDeVarginha")&&Time.realtimeSinceStartup<deadline)yield return null;
            Assert.That(SceneManager.GetActiveScene().name,Is.EqualTo("Menu_MisterioDeVarginha"));
            Assert.That(CampaignStorySave.Load().continuation.finished,Is.True);yield return Capture("21-menu");
        }
    }
}
