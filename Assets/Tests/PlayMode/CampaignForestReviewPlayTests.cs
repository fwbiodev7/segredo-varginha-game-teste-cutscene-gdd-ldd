using System.Collections;
using System.IO;
using System.Reflection;
using Game.Varginha;
using Game.Varginha.Experiment;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace Game.Tests.PlayMode
{
    public sealed class CampaignForestReviewPlayTests
    {
        string saved;
        const BindingFlags Fields=BindingFlags.NonPublic|BindingFlags.Instance;
        [SetUp]public void Preserve()=>saved=File.Exists(CampaignStorySave.Path)?File.ReadAllText(CampaignStorySave.Path):null;
        [UnityTearDown]public IEnumerator Restore()
        {
            Time.timeScale=1;yield return SceneManager.LoadSceneAsync("Menu_MisterioDeVarginha");
            if(saved==null){if(File.Exists(CampaignStorySave.Path))File.Delete(CampaignStorySave.Path);}else File.WriteAllText(CampaignStorySave.Path,saved);
        }
        static IEnumerator Load(int phase)
        {
            var story=new CampaignStory{phase=phase};story.expansion.visited=7;story.expansion.trailChoices=new[]{1,0,1};story.expansion.mapSolved=true;CampaignStorySave.Write(story);
            yield return SceneManager.LoadSceneAsync(CampaignStorySave.Scene(phase));yield return new WaitForSeconds(3.4f);
            CampaignExpansionController.Active.ClosePanel();
        }
        [UnityTest,Timeout(90000)] public IEnumerator PriestIsCompleteOnFreeForestPavingAndKeepsHisOriginalChurchScale()
        {
            yield return Load(7);var stage=CampaignExpansionController.Active;var data=CampaignIllustratedMaps.Get(7);
            var priest=GameObject.Find("Padre Fábio").GetComponent<SpriteRenderer>();var feet=priest.GetComponent<CircleCollider2D>();
            Assert.That(priest.sprite.bounds.size.y,Is.EqualTo(CampaignTeamEdelzio.StandingHeight).Within(.08f));
            Assert.That(stage.Plan.IsClear(feet.bounds.center,.24f),Is.True);
            Assert.That(Vector2.Distance(feet.bounds.center,data.Position(1405,335)),Is.LessThan(.04f));
            var bell=stage.transform.Find("Mapa_Campanha/02_Mobilia_Colisoes/Sino do santuário").GetComponent<SpriteRenderer>();
            Assert.That(priest.bounds.Intersects(bell.bounds),Is.False,"The full enlarged priest must clear the foreground bell.");
            Capture(Camera.main,priest.bounds.center,2.3f,"forest-priest");
            stage.Interact("sign0");stage.ClosePanel();stage.Interact("sign1");stage.ClosePanel();stage.Interact("sign2");stage.ClosePanel();
            stage.Interact("fabio");Assert.That(stage.Progress.expansion.fabioMet,Is.True,"Moving the character must keep the conversation and exit progression.");stage.ClosePanel();
            yield return Load(8);priest=GameObject.Find("Padre Fábio").GetComponent<SpriteRenderer>();
            Assert.That(priest.sprite,Is.SameAs(VarginhaReferenceSprites.PadreFabio()),"The church preserves the original frame/PPU.");
            Capture(Camera.main,priest.bounds.center,1.8f,"church-priest-original");
        }
        [UnityTest,Timeout(90000)] public IEnumerator CaptureShowsGameOverAndRetryReloadsTheSavedRiverCheckpoint()
        {
            yield return Load(7);var stage=CampaignExpansionController.Active;
            var actor=Object.FindAnyObjectByType<EdelzioTopDownController>();var body=actor.GetComponent<Rigidbody2D>();
            stage.Interact("sign0");stage.ClosePanel();stage.Interact("sign1");stage.ClosePanel();
            Vector2 checkpoint=new(stage.Progress.expansion.forestCheckpointX,stage.Progress.expansion.forestCheckpointY);
            Assert.That(stage.Progress.expansion.forestHasCheckpoint,Is.True);
            Assert.That(stage.Progress.expansion.forestCheckpointSigns,Is.EqualTo(2));
            yield return null;
            var entity=GameObject.Find("Manifestação não combatível").GetComponent<SpriteRenderer>();
            Assert.That(entity.color.a,Is.GreaterThan(.9f));
            Assert.That(entity.sprite.name,Does.Contain("ForestPresenceHybridV1"));
            Assert.That(entity.bounds.size.y,Is.GreaterThan(.8f).And.LessThan(CampaignTeamEdelzio.StandingHeight),"The fused ET should remain readable and clearly smaller than Edelzio.");
            stage.Interact("sign2");stage.ClosePanel();
            typeof(CampaignExpansionController).GetField("_chaseGrace",Fields).SetValue(stage,0f);
            entity.transform.position=(Vector2)body.position-Vector2.up*.58f;
            yield return null;yield return null;
            Assert.That(stage.ForestCaught,Is.True);Assert.That(VarginhaGameHUD.Instance.IsGameOver,Is.True);
            Assert.That(Time.timeScale,Is.Zero);Assert.That(actor.IsInputLocked,Is.True);
            var persisted=CampaignStorySave.Load();Assert.That(persisted.phase,Is.EqualTo(7));Assert.That(persisted.expansion.forestSigns,Is.EqualTo(2));
            Assert.That(new Vector2(persisted.x,persisted.y),Is.EqualTo(checkpoint+Vector2.up*.58f));
            if(!Application.isEditor){yield return new WaitForEndOfFrame();var shot=ScreenCapture.CaptureScreenshotAsTexture();File.WriteAllBytes(Path.Combine(CinemaQaEvidence.Folder,"forest-game-over.png"),shot.EncodeToPNG());Object.Destroy(shot);}
            VarginhaGameOverFlow.RetryCurrentPhase();yield return null;yield return new WaitForSeconds(3.4f);
            stage=CampaignExpansionController.Active;stage.ClosePanel();actor=Object.FindAnyObjectByType<EdelzioTopDownController>();
            Assert.That(stage.phase,Is.EqualTo(7));Assert.That(stage.ForestCaught,Is.False);Assert.That(VarginhaGameHUD.Instance.IsGameOver,Is.False);
            Assert.That(Vector2.Distance((Vector2)actor.transform.position-Vector2.up*.58f,checkpoint),Is.LessThan(.08f));
            Assert.That(stage.Progress.expansion.forestSigns,Is.EqualTo(2));Assert.That(actor.CurrentSanity,Is.EqualTo(actor.MaxSanity));
        }
        [UnityTest,Timeout(90000)] public IEnumerator PursuerIsVisibleUsesWalkableRoutesAndFreezesWithThePause()
        {
            yield return Load(7);var stage=CampaignExpansionController.Active;var actor=Object.FindAnyObjectByType<EdelzioTopDownController>();
            stage.Interact("sign0");stage.ClosePanel();stage.Interact("sign1");stage.ClosePanel();
            var body=actor.GetComponent<Rigidbody2D>();body.position=stage.Plan.points.Find(p=>p.id=="sign1").position+Vector2.up*.58f;
            yield return null;yield return new WaitForSeconds(.1f);var entity=GameObject.Find("Manifestação não combatível");
            Assert.That(stage.Plan.IsClear(entity.transform.position,.2f),Is.True,"The chase starts on the route, not inside canopy/tree collision.");
            Vector2 start=entity.transform.position;float distance=Vector2.Distance(start,(Vector2)body.position-Vector2.up*.58f);
            Assert.That(distance,Is.GreaterThan(3.5f));
            Capture(Camera.main,Vector2.Lerp(start,body.position,.5f),4.3f,"forest-pursuit-visible");
            Time.timeScale=0;yield return new WaitForSecondsRealtime(.2f);Assert.That((Vector2)entity.transform.position,Is.EqualTo(start));Time.timeScale=1;
            yield return new WaitForSeconds(1.8f);Assert.That(Vector2.Distance(entity.transform.position,(Vector2)body.position-Vector2.up*.58f),Is.LessThan(distance));
        }
        static void Capture(Camera camera,Vector2 centre,float size,string name)
        {
            var follow=camera.GetComponent<Game.Level.CameraFollow2D>();bool enabled=follow!=null&&follow.enabled;if(follow!=null)follow.enabled=false;
            var position=camera.transform.position;float oldSize=camera.orthographicSize;var previous=camera.targetTexture;var active=RenderTexture.active;
            var target=RenderTexture.GetTemporary(960,540,24);var image=new Texture2D(960,540,TextureFormat.RGB24,false);
            try{camera.transform.position=new Vector3(centre.x,centre.y,-10);camera.orthographicSize=size;camera.targetTexture=target;VarginhaPixelPresentation.RenderInto(camera,target);RenderTexture.active=target;image.ReadPixels(new Rect(0,0,960,540),0,0);image.Apply();File.WriteAllBytes(Path.Combine(CinemaQaEvidence.Folder,name+".png"),image.EncodeToPNG());}
            finally{camera.targetTexture=previous;RenderTexture.active=active;RenderTexture.ReleaseTemporary(target);Object.Destroy(image);camera.transform.position=position;camera.orthographicSize=oldSize;if(follow!=null)follow.enabled=enabled;}
        }
    }
}
