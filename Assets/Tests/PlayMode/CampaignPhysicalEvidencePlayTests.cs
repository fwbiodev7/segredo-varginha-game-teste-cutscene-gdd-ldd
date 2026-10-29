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
    public sealed class CampaignPhysicalEvidencePlayTests:InputTestFixture
    {
        string _save;Keyboard _keyboard;
        public override void Setup(){VarginhaInputActions.Shutdown();base.Setup();_save=File.Exists(CampaignStorySave.Path)?File.ReadAllText(CampaignStorySave.Path):null;_keyboard=InputSystem.AddDevice<Keyboard>();}
        public override void TearDown(){Time.timeScale=1;VarginhaInputActions.Shutdown();base.TearDown();}
        [UnityTearDown]public IEnumerator Restore()
        {
            Time.timeScale=1;yield return SceneManager.LoadSceneAsync("Menu_MisterioDeVarginha");
            if(_save==null){if(File.Exists(CampaignStorySave.Path))File.Delete(CampaignStorySave.Path);}else File.WriteAllText(CampaignStorySave.Path,_save);
        }
        IEnumerator PressInteract()
        {
            InputSystem.QueueStateEvent(_keyboard,new KeyboardState(Key.E));yield return null;yield return null;
            InputSystem.QueueStateEvent(_keyboard,new KeyboardState());yield return null;
        }
        IEnumerator Reach(string id)
        {
            var c=CampaignContinuationController.Active;c.CloseMessage();
            yield return CampaignExpansionPlayTests.WalkLibrary(c.Plan,c.Actor.GetComponent<Rigidbody2D>(),_keyboard,c.Plan.points.Find(p=>p.id==id).position);
        }
        static IEnumerator Capture(string name)
        {
            if(Application.isEditor)yield break;yield return new WaitForEndOfFrame();
            var picture=ScreenCapture.CaptureScreenshotAsTexture();Directory.CreateDirectory(CinemaQaEvidence.Folder);
            File.WriteAllBytes(Path.Combine(CinemaQaEvidence.Folder,name+".png"),picture.EncodeToPNG());Object.Destroy(picture);
        }
        static IEnumerator WaitScene(string name)
        {
            float deadline=Time.realtimeSinceStartup+20;
            while((SceneManager.GetActiveScene().name!=name||CampaignCinematics.IsTransitioning)&&Time.realtimeSinceStartup<deadline)yield return null;
            Assert.That(SceneManager.GetActiveScene().name,Is.EqualTo(name));yield return new WaitForSeconds(3.4f);
        }
        [UnityTest,Timeout(240000)]public IEnumerator CasaraoKeyDocumentsAndBasementCanBeReachedWithRealMovementAndReload()
        {
            CampaignStorySave.Write(new CampaignStory{phase=12});yield return SceneManager.LoadSceneAsync(CampaignStorySave.Scene(12));yield return new WaitForSeconds(3.4f);
            var c=CampaignContinuationController.Active;
            yield return Reach("plan");yield return Capture("casarao-plan-visible");yield return PressInteract();
            Assert.That(c.State.clues[1]&1,Is.EqualTo(1));c.CloseMessage();
            yield return Reach("entrance");yield return PressInteract();yield return WaitScene(CampaignContinuationDefinition.SceneName(12,1));
            c=CampaignContinuationController.Active;
            yield return Reach("plan");yield return PressInteract();Assert.That(c.State.clues[1]&1,Is.EqualTo(1));c.CloseMessage();
            yield return Reach("photo");yield return Capture("casarao-photo-visible");yield return PressInteract();Assert.That(c.State.clues[1]&8,Is.EqualTo(8));c.CloseMessage();
            yield return Reach("key");yield return Capture("casarao-key-visible");yield return PressInteract();Assert.That(c.State.serviceKey,Is.True);yield return null;
            var key=c.transform.Find("Mapa_Campanha/Evidencias_fisicas/Evidencia_key");Assert.That(key.GetComponent<SpriteRenderer>().enabled,Is.False);
            Assert.That(CampaignStorySave.Load().continuation.serviceKey,Is.True);yield return SceneManager.LoadSceneAsync(CampaignContinuationDefinition.SceneName(12,1));yield return new WaitForSeconds(3.4f);
            c=CampaignContinuationController.Active;Assert.That(c.State.serviceKey,Is.True);
            Assert.That(c.transform.Find("Mapa_Campanha/Evidencias_fisicas/Evidencia_key").GetComponent<SpriteRenderer>().enabled,Is.False);
            yield return Reach("reveal");yield return PressInteract();Assert.That(c.State.serviceRevealed,Is.True);c.CloseMessage();
            yield return Reach("service");yield return PressInteract();yield return WaitScene(CampaignStorySave.Scene(13));c=CampaignContinuationController.Active;
            foreach(string id in new[]{"pages","photos","materials"})
            {
                var evidence=c.transform.Find("Mapa_Campanha/Evidencias_fisicas/Evidencia_"+id);
                Assert.That(evidence.GetComponent<SpriteRenderer>().enabled,Is.True);Assert.That(evidence.GetComponent<Collider2D>().isTrigger,Is.True);
                yield return Reach(id);yield return Capture("basement-"+id+"-visible");yield return PressInteract();c.CloseMessage();
            }
            Assert.That(c.State.clues[2]&7,Is.EqualTo(7));Assert.That(CampaignStorySave.Load().continuation.clues[2]&7,Is.EqualTo(7));
            yield return SceneManager.LoadSceneAsync(CampaignStorySave.Scene(13));yield return new WaitForSeconds(3.4f);
            Assert.That(CampaignContinuationController.Active.State.clues[2]&7,Is.EqualTo(7));
        }
    }
}
