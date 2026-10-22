using System.Collections;
using System.IO;
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
    public sealed class CampaignMessagePlayTests : InputTestFixture
    {
        private string _save,_memory;
        private float _voice,_effects;
        private int _size;
        private bool _subtitles;
        const BindingFlags Hidden=BindingFlags.Instance|BindingFlags.NonPublic;
        public override void Setup(){VarginhaInputActions.Shutdown();base.Setup();}
        public override void TearDown(){Time.timeScale=1;VarginhaInputActions.Shutdown();base.TearDown();}
        [SetUp] public void Preserve()
        {
            _save=File.Exists(CampaignStorySave.Path)?File.ReadAllText(CampaignStorySave.Path):null;
            _memory=File.Exists(CampaignMemorySave.Path)?File.ReadAllText(CampaignMemorySave.Path):null;
            var s=VarginhaGameSettings.Current;_voice=s.voice;_effects=s.effects;_size=s.subtitleSize;_subtitles=s.subtitles;
            InputSystem.settings.backgroundBehavior=InputSettings.BackgroundBehavior.IgnoreFocus;
            InputSystem.settings.editorInputBehaviorInPlayMode=InputSettings.EditorInputBehaviorInPlayMode.AllDeviceInputAlwaysGoesToGameView;
        }
        [UnityTearDown] public IEnumerator Restore()
        {
            Time.timeScale=1;var s=VarginhaGameSettings.Current;s.voice=_voice;s.effects=_effects;s.subtitleSize=_size;s.subtitles=_subtitles;
            yield return SceneManager.LoadSceneAsync("Menu_MisterioDeVarginha");yield return null;
            RestoreFile(CampaignStorySave.Path,_save);RestoreFile(CampaignMemorySave.Path,_memory);
        }
        static void RestoreFile(string path,string data){if(data==null){if(File.Exists(path))File.Delete(path);}else File.WriteAllText(path,data);}
        static IEnumerator Capture(string name)
        {
            if(Application.isEditor)yield break;
            yield return new WaitForEndOfFrame();var texture=ScreenCapture.CaptureScreenshotAsTexture();
            string folder=Path.Combine(CinemaQaEvidence.Folder,"captures");Directory.CreateDirectory(folder);
            File.WriteAllBytes(Path.Combine(folder,name+".png"),texture.EncodeToPNG());Object.Destroy(texture);
        }
        static IEnumerator WaitAction(VarginhaCampaignStage stage)
        {
            float deadline=Time.realtimeSinceStartup+12;
            while(stage.IsActing&&Time.realtimeSinceStartup<deadline)yield return null;
            Assert.That(stage.IsActing,Is.False,"Routine action must finish within its normal deadline.");yield return null;
        }
        [UnityTest,Timeout(90000)] public IEnumerator RoutineConsumesRealItemsAndKeepsThemGoneAfterReload()
        {
            CampaignStorySave.Write(new CampaignStory{phase=2});yield return SceneManager.LoadSceneAsync(CampaignStorySave.Scene(2));yield return new WaitForSeconds(3.3f);
            var stage=VarginhaCampaignStage.Active;var actor=Object.FindAnyObjectByType<EdelzioTopDownController>();var body=actor.GetComponent<Rigidbody2D>();
            var data=CampaignIllustratedMaps.Get(2);
            var map=stage.transform.Find("Mapa_Campanha");
            Assert.That(map.Find("01_Planta_Paredes_Divisoes/Arte_Integrada_0").GetComponent<SpriteRenderer>().sprite.texture.name,Is.EqualTo("AdultInteractiveBase"));
            var authoredHouse=CampaignMapPlan.Create(2);
            foreach(string name in new[]{"Fridge_Kitchen","Stove_Kitchen","Kitchen_Cabinet","Bookshelf_Office","Dresser_Bedroom","Desk_Office","CoffeeTable_Kitchen"})
            {
                var furniture=map.Find("02_Mobilia_Colisoes/"+name);
                Assert.That(furniture.GetComponent<SpriteRenderer>().enabled,Is.False,"Old crop must be hidden: "+name);
                var collider=furniture.GetComponent<Collider2D>();var authored=authoredHouse.furniture.Find(p=>p.name==name);
                Assert.That(collider.enabled,Is.True,"Authored collision must remain: "+name);
                Assert.That(Vector2.Distance(collider.bounds.center,authored.footprint.center),Is.LessThan(.001f),"Collision position: "+name);
                Assert.That(Vector2.Distance(collider.bounds.size,authored.footprint.size),Is.LessThan(.001f),"Collision size: "+name);
            }
            yield return Capture("house-clean-items-before");
            foreach(string id in new[]{"wash","food","work"})
            {
                body.position=data.Objective(id)+Vector2.up*.58f;yield return new WaitForFixedUpdate();
                stage.Interact(id);yield return new WaitForSeconds(.75f);
                Assert.That(actor.transform.localScale,Is.EqualTo(Vector3.one),"Action cannot scale physics or the whole protagonist: "+id);
                yield return Capture("routine-"+id+"-action");yield return WaitAction(stage);
                Assert.That((stage.Progress.routine&(id=="wash"?1:id=="food"?2:4)),Is.Not.Zero,id);
            }
            var items=stage.GetComponent<CampaignHouseRoutineItems>();
            Assert.That(items.Coffee.gameObject.activeSelf,Is.False);Assert.That(items.Notebook.gameObject.activeSelf,Is.False);
            yield return Capture("routine-items-consumed");
            body.position=data.Position(749,600)+Vector2.up*.58f;yield return new WaitForFixedUpdate();stage.Interact("counter");yield return WaitAction(stage);
            Assert.That(stage.Progress.house.KeyOnCounter,Is.True);
            var key=stage.transform.Find("Chave_Fusca_Balcao");Assert.That(Vector3.Distance(key.position,data.Position(733,531)),Is.LessThan(.001f));
            Assert.That(key.localScale.x,Is.EqualTo(.9f).Within(.001));yield return Capture("key-coffee-table-larger");
            stage.Save();yield return SceneManager.LoadSceneAsync(CampaignStorySave.Scene(2));yield return new WaitForSeconds(3.3f);
            stage=VarginhaCampaignStage.Active;items=stage.GetComponent<CampaignHouseRoutineItems>();
            Assert.That(items.Coffee.gameObject.activeSelf,Is.False);Assert.That(items.Notebook.gameObject.activeSelf,Is.False);
            Assert.That(stage.Progress.house.KeyOnCounter,Is.True);Assert.That(stage.transform.Find("Chave_Fusca_Balcao").GetComponent<SpriteRenderer>().enabled,Is.True);
            var follow=Camera.main.GetComponent<Game.Level.CameraFollow2D>();follow.enabled=false;Camera.main.orthographicSize=9;Camera.main.transform.position=new Vector3(17,0,-10);
            yield return Capture("yard-olive-palette");
            Camera.main.orthographicSize=8;Camera.main.transform.position=new Vector3(0,0,-10);yield return Capture("house-rear-furniture");
        }
        [UnityTest,Timeout(60000)] public IEnumerator InterruptedCoffeeAndWashDoNotConsumeTheirObjectives()
        {
            CampaignStorySave.Write(new CampaignStory{phase=2});yield return SceneManager.LoadSceneAsync(CampaignStorySave.Scene(2));yield return new WaitForSeconds(3.3f);
            var stage=VarginhaCampaignStage.Active;var actor=Object.FindAnyObjectByType<EdelzioTopDownController>();var action=actor.GetComponent<VarginhaPlayerActionAnimation>();
            actor.GetComponent<Rigidbody2D>().position=CampaignIllustratedMaps.Get(2).Objective("food")+Vector2.up*.58f;yield return new WaitForFixedUpdate();
            stage.Interact("food");yield return new WaitForSeconds(.4f);action.enabled=false;yield return WaitAction(stage);
            Assert.That(stage.Progress.routine&2,Is.Zero);Assert.That(stage.GetComponent<CampaignHouseRoutineItems>().Coffee.gameObject.activeSelf,Is.True);
            action.enabled=true;actor.GetComponent<Rigidbody2D>().position=CampaignAdultHouse.WashApproach;yield return new WaitForFixedUpdate();
            stage.Interact("wash");yield return new WaitForSeconds(.4f);action.enabled=false;yield return WaitAction(stage);
            Assert.That(stage.Progress.routine&1,Is.Zero);action.enabled=true;
            stage.Interact("wash");yield return WaitAction(stage);Assert.That(stage.Progress.routine&1,Is.EqualTo(1));
        }
        [UnityTest,Timeout(60000)] public IEnumerator BagPickupKeepsTheOriginalSmallPanelAndNeedsNoContinue()
        {
            var story=new CampaignStory{phase=2,routine=7};story.house.step=1;CampaignStorySave.Write(story);
            yield return SceneManager.LoadSceneAsync(CampaignStorySave.Scene(2));yield return new WaitForSeconds(3.3f);
            var stage=VarginhaCampaignStage.Active;stage.Interact("bag");
            float deadline=Time.realtimeSinceStartup+15;
            while((stage.IsActing||Object.FindAnyObjectByType<CampaignPickupNotice>()?.CurrentText==null)&&Time.realtimeSinceStartup<deadline)yield return null;
            var notice=Object.FindAnyObjectByType<CampaignPickupNotice>();Assert.That(notice,Is.Not.Null);
            Assert.That(notice.CurrentText,Does.Contain("Mochila"));Assert.That(notice.Bounds.width,Is.LessThan(500));Assert.That(notice.Bounds.height,Is.LessThan(180));
            Assert.That(stage.Progress.house.Missing,Is.True);Assert.That(stage.Progress.routine&8,Is.EqualTo(8));
            yield return null;Assert.That(VarginhaCampaignStage.IsModalOpen,Is.False);Assert.That(Object.FindAnyObjectByType<EdelzioTopDownController>().IsInputLocked,Is.False);
            yield return Capture("pickup-small-original-panel");
            yield return new WaitForSeconds(7.3f);Assert.That(notice.CurrentText,Is.Null,"No input required to dismiss a pickup.");
        }
        [UnityTest,Timeout(60000)] public IEnumerator RenanSubtitleRevealsByKeyboardAndGamepadWithoutClosingEarly()
        {
            var keyboard=InputSystem.AddDevice<Keyboard>();var pad=InputSystem.AddDevice<Gamepad>();VarginhaInputActions.Rebuild();
            CampaignStorySave.Write(new CampaignStory{phase=4});yield return SceneManager.LoadSceneAsync(CampaignStorySave.Scene(4));yield return new WaitForSeconds(3.3f);
            var stage=VarginhaCampaignStage.Active;stage.Interact("renan");yield return null;
            var text=(CampaignMessagePresentation)typeof(VarginhaCampaignStage).GetField("_textMessage",Hidden).GetValue(stage);
            Assert.That(text.Kind,Is.EqualTo(CampaignMessageKind.Speech));Assert.That(text.PageCount,Is.GreaterThan(1));Assert.That(text.Revealed,Is.False);
            Press(keyboard.eKey);yield return null;Release(keyboard.eKey);yield return null;
            Assert.That(text.PageIndex,Is.Zero);Assert.That(text.Revealed,Is.True);
            yield return Capture("renan-subtitle-revealed");
            Press(pad.buttonSouth);yield return null;Release(pad.buttonSouth);yield return null;
            Assert.That(text.PageIndex,Is.EqualTo(1));Assert.That(VarginhaCampaignStage.IsModalOpen,Is.True);
            stage.TogglePause();string paused=text.VisibleText;yield return new WaitForSecondsRealtime(.4f);Assert.That(text.VisibleText,Is.EqualTo(paused));stage.TogglePause();
            foreach(int size in new[]{0,1,2})
            {
                VarginhaGameSettings.Current.subtitleSize=size;stage.Interact("renan");yield return new WaitForSeconds(.4f);
                foreach(var resolution in new[]{new Vector2Int(960,540),new Vector2Int(1280,720),new Vector2Int(1920,1080)})
                {
                    Screen.SetResolution(resolution.x,resolution.y,false);yield return new WaitForSeconds(.2f);
                    yield return Capture("subtitle-size-"+size+"-"+resolution.x);
                }
            }
            stage.CloseDialogue();yield return null;Assert.That(VarginhaCampaignStage.IsModalOpen,Is.False);
        }
        [UnityTest] public IEnumerator RecordedVoiceUsesItsOwnVolumeAndPauseClock()
        {
            var owner=new GameObject("Voice_QA").AddComponent<CampaignDialogueVoice>();
            var clip=AudioClip.Create("QA_silence_not_dubbing",44100*2,1,44100,false);
            var text=new CampaignMessagePresentation();VarginhaGameSettings.Current.voice=.27f;VarginhaGameSettings.Current.effects=0;
            text.Show(owner,"Renan","Compare as duas cópias.",CampaignMessageKind.Speech,clip);yield return new WaitForSeconds(.1f);
            text.Tick(.1f,false);Assert.That(owner.Source.volume,Is.EqualTo(.27f).Within(.001));Assert.That(owner.Source.isPlaying,Is.True);
            string visible=text.VisibleText;Time.timeScale=0;text.Tick(8,false);yield return new WaitForSecondsRealtime(.2f);
            Assert.That(owner.Source.isPlaying,Is.False);Assert.That(text.VisibleText,Is.EqualTo(visible));Assert.That(text.FinishedByVoice,Is.False);
            Time.timeScale=1;text.Tick(.2f,false);yield return null;Assert.That(owner.Source.isPlaying,Is.True);
            text.Tick(3,false);Assert.That(text.FinishedByVoice,Is.True);text.Close();Assert.That(owner.Source.isPlaying,Is.False);
            Object.Destroy(owner.gameObject);Object.Destroy(clip);
        }
        [UnityTest,Timeout(60000)] public IEnumerator ServiceKeyReceiptDoesNotOpenADialogueAndStaysInJournal()
        {
            CampaignStorySave.Write(new CampaignStory{phase=12});yield return SceneManager.LoadSceneAsync(CampaignStorySave.Scene(12));yield return new WaitForSeconds(2.3f);
            var stage=CampaignContinuationController.Active;stage.CloseMessage();stage.Interact("key");yield return null;
            Assert.That(stage.State.serviceKey,Is.True);Assert.That(stage.State.clues[1]&4,Is.EqualTo(4));Assert.That(stage.Modal,Is.False);
            Assert.That(Object.FindAnyObjectByType<CampaignPickupNotice>().CurrentText,Does.Contain("Chave"));yield return Capture("service-key-receipt");
        }
    }
}
