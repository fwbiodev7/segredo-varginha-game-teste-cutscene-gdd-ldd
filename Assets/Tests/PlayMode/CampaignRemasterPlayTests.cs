using System.Collections;
using System.IO;
using Game.Level;
using Game.Varginha;
using Game.Varginha.Experiment;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace Game.Tests.PlayMode
{
    public class CampaignRemasterPlayTests
    {
        private string _story, _memory;
        private bool _reduced;
        [SetUp] public void Backup()
        {
            _story = File.Exists(CampaignStorySave.Path) ? File.ReadAllText(CampaignStorySave.Path) : null;
            _memory = File.Exists(CampaignMemorySave.Path) ? File.ReadAllText(CampaignMemorySave.Path) : null;
            _reduced = VarginhaGameSettings.Current.reducedMotion;
        }
        [TearDown] public void Restore()
        {
            Time.timeScale = 1; VarginhaGameSettings.Current.reducedMotion = _reduced;
            if (_story == null) File.Delete(CampaignStorySave.Path); else File.WriteAllText(CampaignStorySave.Path, _story);
            if (_memory == null) File.Delete(CampaignMemorySave.Path); else File.WriteAllText(CampaignMemorySave.Path, _memory);
        }
        private static IEnumerator Capture(string name)
        {
            string directory = Path.Combine(Directory.GetCurrentDirectory(), "Docs", "Remaster20261008", "Prints");
            Directory.CreateDirectory(directory);
            #if UNITY_EDITOR
            var view = UnityEditor.EditorWindow.GetWindow(typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.GameView"));
            view.Show(); view.Repaint();
            #endif
            yield return null; yield return null;
            File.WriteAllText(Path.Combine(directory, "runtime-pipeline.txt"), VarginhaPixelPresentation.DetectPipeline().ToString());
            string path = Path.Combine(directory, name + ".png");
            ScreenCapture.CaptureScreenshot(path);
            yield return new WaitForSecondsRealtime(.4f);
            Assert.That(File.Exists(path), Is.True, "A real rendered screenshot must be saved.");
        }
        private static IEnumerator WaitTransition()
        {
            float deadline = Time.realtimeSinceStartup + 20;
            while (CampaignCinematics.IsTransitioning && Time.realtimeSinceStartup < deadline) yield return null;
            Assert.That(CampaignCinematics.IsTransitioning, Is.False);
            Assert.That(Time.timeScale, Is.EqualTo(1));
        }
        [UnityTest] public IEnumerator ClueFramingFreezesDuringPauseReturnsSafelyAndHonorsReducedMotion()
        {
            VarginhaGameSettings.Current.reducedMotion = false;
            CampaignMemorySave.Write(new CampaignMemory { openingSeen = true });
            yield return SceneManager.LoadSceneAsync(VarginhaCampaignPhase1.SceneName); yield return new WaitForSeconds(3.3f);
            var follow = Camera.main.GetComponent<CameraFollow2D>();
            var director = Camera.main.gameObject.AddComponent<CampaignCameraDirector>();
            float size = Camera.main.orthographicSize;
            director.Focus(follow.Target.position + Vector3.right * 2, 2);
            yield return new WaitForSeconds(.5f);
            yield return Capture("01_Prologo_1996");
            var position = Camera.main.transform.position; float weight = director.Weight;
            Time.timeScale = 0; yield return new WaitForSecondsRealtime(.2f);
            Assert.That(Camera.main.transform.position, Is.EqualTo(position)); Assert.That(director.Weight, Is.EqualTo(weight));
            Time.timeScale = 1; director.Cancel(); yield return null;
            Assert.That(follow.enabled, Is.True); Assert.That(director.IsActive, Is.False);
            Assert.That(Camera.main.orthographicSize, Is.EqualTo(size));
            VarginhaGameSettings.Current.reducedMotion = true; director.Focus(Vector3.zero,2);
            Assert.That(director.IsActive, Is.False);
            yield return SceneManager.LoadSceneAsync("Menu_MisterioDeVarginha");
        }
        [UnityTest] public IEnumerator CrossfadeKeepsAmbientChannelsAliveReleasesFrameAndRestoresControls()
        {
            CampaignStorySave.Write(new CampaignStory { phase = 12 });
            VarginhaGameSettings.Current.reducedMotion = false;
            yield return SceneManager.LoadSceneAsync(CampaignContinuationDefinition.SceneName(12)); yield return new WaitForSeconds(3.2f);
            var bridge = CampaignAmbientBridge.Instance;
            Assert.That(CampaignSoundscape.Clip("Success"), Is.SameAs(CampaignSoundscape.Clip("Success")));
            var outgoing = CampaignSoundscape.Clip("HouseAmbience");
            CampaignCinematics.Load(CampaignContinuationDefinition.SceneName(12,1));
            Assert.That(CampaignCinematics.IsTransitioning, Is.True);
            yield return new WaitForSecondsRealtime(.25f);
            yield return Capture("05_Transicao_Casarao");
            yield return WaitTransition(); yield return new WaitForSeconds(3.2f);
            Assert.That(CampaignAmbientBridge.Instance, Is.SameAs(bridge));
            Assert.That(bridge.GetComponents<AudioSource>().Length, Is.EqualTo(2));
            Assert.That(outgoing, Is.Not.Null);
            var instance = Object.FindAnyObjectByType<CampaignCinematics>();
            var field = typeof(CampaignCinematics).GetField("_transitionFrame", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            Assert.That(field.GetValue(instance), Is.Null);
            Assert.That(CampaignContinuationController.Active.Actor.IsInputLocked, Is.True, "The existing arrival dialogue keeps control until dismissed.");
            CampaignContinuationController.Active.CloseMessage(); yield return null;
            Assert.That(CampaignContinuationController.Active.Actor.IsInputLocked, Is.False);
            yield return Capture("03_Casarao_Interior");
            CampaignCinematics.Load("Menu_MisterioDeVarginha"); yield return WaitTransition();
        }
        [UnityTest] public IEnumerator ContextTransitionsReachSchoolAndFuscaWithScreenshots()
        {
            CampaignStorySave.Write(new CampaignStory { phase = 4 });
            CampaignCinematics.Load(CampaignStorySave.Scene(4)); yield return WaitTransition(); yield return new WaitForSeconds(3.4f);
            yield return Capture("02_Escola_Industrial");
            CampaignStorySave.Write(new CampaignStory { phase = 10 });
            CampaignCinematics.Load(CampaignStorySave.Scene(10)); yield return WaitTransition(); yield return new WaitForSeconds(3.4f);
            yield return Capture("04_Fusca_Oficina");
            Assert.That(Camera.main.GetComponent<CameraFollow2D>().enabled, Is.True);
            CampaignCinematics.Load("Menu_MisterioDeVarginha"); yield return WaitTransition();
        }
        [UnityTest] public IEnumerator AmbientCrossfadeAnticipatesNextBedAndPauseFreezesBlend()
        {
            yield return SceneManager.LoadSceneAsync("Menu_MisterioDeVarginha");
            yield return WaitTransition(); yield return null; yield return null;
            var bridge = CampaignAmbientBridge.Instance; bridge.Suspend(false); bridge.Duck(0);
            bridge.Transition(CampaignSoundscape.Clip("HouseAmbience"), .1f); yield return new WaitForSecondsRealtime(.2f);
            var channels = bridge.GetComponents<AudioSource>();
            bridge.Transition(CampaignSoundscape.Clip("SchoolAmbience"), 1);
            yield return new WaitForSecondsRealtime(.3f);
            Assert.That(channels[0].isPlaying && channels[1].isPlaying, Is.True, "Both beds overlap during the crossfade.");
            float first = channels[0].volume, second = channels[1].volume;
            bridge.Suspend(true); yield return new WaitForSecondsRealtime(.2f);
            Assert.That(channels[0].volume, Is.EqualTo(first)); Assert.That(channels[1].volume, Is.EqualTo(second));
            bridge.Suspend(false); yield return new WaitForSecondsRealtime(.9f);
            Assert.That(Mathf.Min(channels[0].volume,channels[1].volume), Is.Zero);
            bridge.Transition(null, .1f); yield return new WaitForSecondsRealtime(.2f);
        }
    }
}
