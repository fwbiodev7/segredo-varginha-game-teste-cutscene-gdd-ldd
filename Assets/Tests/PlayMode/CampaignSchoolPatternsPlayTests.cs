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

namespace Game.Tests.PlayMode
{
    public sealed class CampaignSchoolPatternsPlayTests : InputTestFixture
    {
        string _save,_memory;Gamepad _pad;Keyboard _keyboard;bool _keys,_background;
        const BindingFlags Fields=BindingFlags.Public|BindingFlags.NonPublic|BindingFlags.Instance;
        public override void Setup(){VarginhaInputActions.Shutdown();base.Setup();}
        public override void TearDown(){Time.timeScale=1;VarginhaInputActions.Shutdown();VarginhaGamepadUI.Reset();base.TearDown();}
        [SetUp]public void Preserve()
        {
            _save=File.Exists(CampaignStorySave.Path)?File.ReadAllText(CampaignStorySave.Path):null;
            _memory=File.Exists(CampaignMemorySave.Path)?File.ReadAllText(CampaignMemorySave.Path):null;
            _background=Application.runInBackground;Application.runInBackground=true;
            _pad=InputSystem.AddDevice<Gamepad>();_keyboard=InputSystem.AddDevice<Keyboard>();
            InputSystem.settings.backgroundBehavior=InputSettings.BackgroundBehavior.IgnoreFocus;
        }
        [UnityTearDown]public IEnumerator Restore()
        {
            yield return SceneManager.LoadSceneAsync("Menu_MisterioDeVarginha");
            RestoreFile(CampaignStorySave.Path,_save);RestoreFile(CampaignMemorySave.Path,_memory);Application.runInBackground=_background;
        }
        static void RestoreFile(string path,string text){if(text==null){if(File.Exists(path))File.Delete(path);}else File.WriteAllText(path,text);}
        static Rect Selection()
        {
            var context=typeof(VarginhaGamepadUI).GetField("_active",BindingFlags.Static|BindingFlags.NonPublic).GetValue(null);
            if(context==null)return default;
            var selection=context.GetType().GetField("selection",Fields).GetValue(context);
            return selection==null?default:(Rect)selection.GetType().GetField("local",Fields).GetValue(selection);
        }
        IEnumerator PressUI(GamepadButton button)
        {
            for(int i=0;i<2;i++)
            {
                if(_keys){Key key=button==GamepadButton.South?Key.Enter:button==GamepadButton.DpadUp?Key.UpArrow:button==GamepadButton.DpadDown?Key.DownArrow:button==GamepadButton.DpadLeft?Key.LeftArrow:Key.RightArrow;InputSystem.QueueStateEvent(_keyboard,new KeyboardState(key));}
                else InputSystem.QueueStateEvent(_pad,new GamepadState().WithButton(button));yield return null;
            }
            for(int i=0;i<2;i++){InputSystem.QueueStateEvent(_keyboard,new KeyboardState());InputSystem.QueueStateEvent(_pad,new GamepadState());yield return null;}
        }
        IEnumerator Click(Rect target)
        {
            yield return null;yield return null;
            for(int attempt=0;attempt<40&&!Selection().Equals(target);attempt++)
            {
                var delta=target.center-Selection().center;
                yield return PressUI(Mathf.Abs(delta.x)>180?delta.x>0?GamepadButton.DpadRight:GamepadButton.DpadLeft:delta.y>0?GamepadButton.DpadDown:GamepadButton.DpadUp);
            }
            Assert.That(Selection(),Is.EqualTo(target),"The real UI must be reachable with "+(_keys?"keyboard":"controller"));
            yield return PressUI(GamepadButton.South);
        }
        IEnumerator Capture(string name)
        {
            if(Application.isEditor)yield break;yield return new WaitForEndOfFrame();
            var image=ScreenCapture.CaptureScreenshotAsTexture();string folder=Path.Combine(CinemaQaEvidence.Folder,"school-patterns");Directory.CreateDirectory(folder);
            File.WriteAllBytes(Path.Combine(folder,(_keys?"keyboard-":"controller-")+name+".png"),image.EncodeToPNG());Object.Destroy(image);
        }
        static IEnumerator LoadSchool(bool owned)
        {
            var story=new CampaignStory{phase=4,boxFound=owned};if(owned)story.school.RecoverDocuments();CampaignStorySave.Write(story);
            yield return SceneManager.LoadSceneAsync(CampaignStorySave.Scene(4));yield return new WaitForSecondsRealtime(3.5f);
        }
        IEnumerator CompleteWithUI(bool keys)
        {
            _keys=keys;yield return LoadSchool(true);var stage=VarginhaCampaignStage.Active;
            if(!Application.isEditor)
            {
                Directory.CreateDirectory(CinemaQaEvidence.Folder);
                File.Copy(CampaignStorySave.Path,Path.Combine(CinemaQaEvidence.Folder,"school-mouse-start.json"),true);
            }
            stage.Interact("notebook");Assert.That(stage.SchoolInvestigationOpen,Is.True);Assert.That(stage.Progress.renanMet,Is.False);
            yield return Capture("01-chronology");Assert.That(stage.SubmitSchoolStep(),Is.False);
            yield return Click(new Rect(473,416,329,36));Assert.That(stage.SchoolDocumentOpen,Is.True);yield return Capture("document");
            // Read all owned materials through the same next/previous controls as the player.
            for(int i=0;i<5;i++)yield return Click(new Rect(905,570,220,45));
            yield return Click(new Rect(155,570,300,45));
            yield return Click(new Rect(93,465,329,36));yield return Click(new Rect(473,465,329,36));
            yield return Click(new Rect(473,465,329,36));yield return Click(new Rect(853,465,329,36));
            yield return Click(new Rect(910,610,290,42));Assert.That(stage.Progress.school.stage,Is.EqualTo(1));
            Assert.That(stage.OpenSchoolDocument(0),Is.False,"Comparison is a memory question, without a document reader.");
            yield return Click(new Rect(155,285,970,62));Assert.That(stage.SubmitSchoolStep(),Is.False);
            yield return Click(new Rect(155,437,970,62));Assert.That(stage.Progress.school.comparisonChoice,Is.EqualTo(2));
            yield return Capture("02-patterns");yield return Click(new Rect(910,610,290,42));Assert.That(stage.Progress.school.stage,Is.EqualTo(2));
            yield return Capture("03-shuffled-empty");
            var school=stage.Progress.school;
            yield return Click(new Rect(480,237,95,34));Assert.That(school.selectedDocument,Is.EqualTo(school.noteOrder[0]+3),"Read must follow the shuffled note.");
            yield return Click(new Rect(155,570,300,45));
            for(int note=0;note<3;note++)
            {
                int left=System.Array.IndexOf(school.noteOrder,note),right=System.Array.IndexOf(school.passageOrder,note);
                Assert.That(left,Is.Not.EqualTo(right),"No correct pair may share a row.");
                yield return Click(new Rect(75,237+left*97,390,80));yield return Click(new Rect(755,237+right*97,445,80));
            }
            int undoRow=System.Array.IndexOf(school.noteOrder,1),targetRow=System.Array.IndexOf(school.passageOrder,1);
            yield return Click(new Rect(480,279+undoRow*97,95,34));
            // Undo uses its own row button; correct it without touching rows 0 and 2.
            Assert.That(stage.Progress.school.links[0],Is.EqualTo(0));
            Assert.That(stage.Progress.school.links[1],Is.EqualTo(-1));
            yield return Click(new Rect(75,237+undoRow*97,390,80));yield return Click(new Rect(755,237+targetRow*97,445,80));
            yield return Capture("03-crossing");
            yield return Click(new Rect(910,610,290,42));Assert.That(stage.Progress.school.stage,Is.EqualTo(3));
            Assert.That(stage.OpenSchoolDocument(2),Is.False,"Conclude must not reopen the recortes or reveal their origin.");
            yield return Click(new Rect(95,435,500,60));Assert.That(stage.Progress.school.noticeRead,Is.True);yield return Capture("notice");
            yield return Click(new Rect(155,570,300,45));yield return Click(new Rect(655,340,545,70));
            yield return Capture("04-origin");yield return Click(new Rect(910,610,290,42));Assert.That(stage.Progress.school.completed,Is.True);
            Assert.That(CampaignStorySave.Load().school.completed,Is.True);Assert.That(stage.Progress.renanMet,Is.False,"Renan does not provide supernatural confirmation.");
            yield return Capture("05-conclusion");yield return Click(new Rect(650,457,500,55));
            Assert.That(stage.ContinueChapter(),Is.False,"The destination button already started the transition; no redundant confirmation.");
            float deadline=Time.realtimeSinceStartup+20;while((CampaignCinematics.IsTransitioning||SceneManager.GetActiveScene().name!=CampaignStorySave.Scene(6))&&Time.realtimeSinceStartup<deadline)yield return null;
            Assert.That(SceneManager.GetActiveScene().name,Is.EqualTo(CampaignStorySave.Scene(6)));Assert.That(CampaignStorySave.Load().school.completed,Is.True);
            Assert.That(CampaignStorySave.Load().expansion.visited,Is.Zero,"The library must retain its own uncompleted puzzle.");
            yield return new WaitForSecondsRealtime(3.5f);yield return Capture("06-library");
        }
        [UnityTest,Timeout(120000)]public IEnumerator FourStepsPlayableWithKeyboard()=>CompleteWithUI(true);
        [UnityTest,Timeout(120000)]public IEnumerator FourStepsPlayableWithController()=>CompleteWithUI(false);
        [UnityTest,Timeout(60000)]public IEnumerator PartialWrongLinksAndHintsSurviveReopenAndReload()
        {
            yield return LoadSchool(true);var stage=VarginhaCampaignStage.Active;stage.Interact("notebook");
            for(int i=0;i<6;i++){stage.OpenSchoolDocument(i);stage.CloseSchoolDocument();}
            stage.SwapSchoolDocuments(0,1);stage.SwapSchoolDocuments(1,2);Assert.That(stage.SubmitSchoolStep(),Is.True);
            stage.ChooseSchoolComparison(2);Assert.That(stage.SubmitSchoolStep(),Is.True);
            stage.LinkSchoolNote(0,0);stage.LinkSchoolNote(1,2);
            int[] leftBefore=(int[])stage.Progress.school.noteOrder.Clone(),rightBefore=(int[])stage.Progress.school.passageOrder.Clone();
            yield return Click(new Rect(290,610,195,42));yield return Click(new Rect(650,475,475,45));
            Assert.That(stage.Progress.school.hintLevel,Is.EqualTo(1));Assert.That(stage.Progress.school.stage,Is.EqualTo(2));
            yield return Click(new Rect(155,475,360,45));stage.Save();stage.CloseSchoolInvestigation();stage.Interact("notebook");
            Assert.That(stage.Progress.school.links,Is.EqualTo(new[]{0,2,-1}));
            Assert.That(stage.Progress.school.noteOrder,Is.EqualTo(leftBefore));Assert.That(stage.Progress.school.passageOrder,Is.EqualTo(rightBefore));
            yield return SceneManager.LoadSceneAsync(CampaignStorySave.Scene(4));yield return new WaitForSecondsRealtime(3.5f);stage=VarginhaCampaignStage.Active;stage.Interact("notebook");
            Assert.That(stage.Progress.school.stage,Is.EqualTo(2));Assert.That(stage.Progress.school.links,Is.EqualTo(new[]{0,2,-1}));Assert.That(stage.Progress.school.hintLevel,Is.EqualTo(1));
            Assert.That(stage.Progress.school.noteOrder,Is.EqualTo(leftBefore));Assert.That(stage.Progress.school.passageOrder,Is.EqualTo(rightBefore));
            stage.LinkSchoolNote(1,-1);Assert.That(stage.Progress.school.links,Is.EqualTo(new[]{0,-1,-1}));stage.LinkSchoolNote(1,1);stage.LinkSchoolNote(2,2);Assert.That(stage.SubmitSchoolStep(),Is.True);
        }
        [UnityTest,Timeout(40000)]public IEnumerator NoSchoolInteractionCollectsTheDocumentsAgainOrBypassesTheGate()
        {
            yield return LoadSchool(false);var stage=VarginhaCampaignStage.Active;
            stage.Interact("renan");stage.CloseDialogue();stage.Interact("student:0");stage.CloseDialogue();stage.Interact("notebook");stage.CloseDialogue();
            Assert.That(stage.SchoolInvestigationOpen,Is.False);Assert.That(stage.Progress.school.documentsRecovered,Is.False);
            Assert.That(stage.SubmitPhotoClue(1)||stage.SubmitCode()||stage.PrepareLibraryDeparture()||stage.ContinueChapter(),Is.False);
        }
        [UnityTest,Timeout(60000)]public IEnumerator ControllerCanReadTheWholeJournalAfterHotbarItemsAreRemoved()
        {
            yield return LoadSchool(true);var stage=VarginhaCampaignStage.Active;
            var actor=Object.FindAnyObjectByType<EdelzioTopDownController>();actor.HasResearchNotebook=actor.HasDecodedData=false;
            InputSystem.QueueStateEvent(_keyboard,new KeyboardState(Key.Tab));yield return null;yield return null;
            InputSystem.QueueStateEvent(_keyboard,new KeyboardState());yield return null;yield return null;
            var field=typeof(VarginhaCampaignStage).GetField("_journalScroll",Fields);
            for(int i=0;i<CampaignJournal.Entries(stage.Progress).Count-1;i++)yield return Click(new Rect(635,505,180,32));
            Assert.That(((Vector2)field.GetValue(stage)).y,Is.GreaterThan(1000),"The final childhood note must be reachable with controller input.");
            Assert.That(stage.Progress.school.documentsRecovered,Is.True);
            yield return Capture("journal-final-note");
            float end=((Vector2)field.GetValue(stage)).y;
            yield return Click(new Rect(140,505,180,32));Assert.That(((Vector2)field.GetValue(stage)).y,Is.EqualTo(end-190).Within(.1f));
        }
    }
}
