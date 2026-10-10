using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Game.Varginha;
using Game.Varginha.Experiment;
using NUnit.Framework;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using Object = UnityEngine.Object;

namespace Game.Tests.PlayMode
{
    [Category("Atmosphere")]
    public class CampaignAtmospherePlayTests
    {
        const string Folder = "Docs/QAAtmosfera20261009";
        string _save, _memory;
        bool _reduced;
        float _music;
        [SetUp] public void Preserve()
        {
            _save = File.Exists(CampaignStorySave.Path) ? File.ReadAllText(CampaignStorySave.Path) : null;
            _memory = File.Exists(CampaignMemorySave.Path) ? File.ReadAllText(CampaignMemorySave.Path) : null;
            _reduced = VarginhaGameSettings.Current.reducedMotion; _music = VarginhaGameSettings.Current.music;
            VarginhaGameSettings.Current.reducedMotion = false; Time.timeScale = 1; Directory.CreateDirectory(Folder);
        }
        [TearDown] public void Restore()
        {
            Time.timeScale = 1; VarginhaGameSettings.Current.reducedMotion = _reduced; VarginhaGameSettings.Current.music = _music;
            RestoreFile(CampaignStorySave.Path, _save); RestoreFile(CampaignMemorySave.Path, _memory);
        }
        static void RestoreFile(string path, string contents)
        { if (contents == null) { if (File.Exists(path)) File.Delete(path); } else File.WriteAllText(path, contents); }
        static IEnumerator Load(int phase, int area = 0)
        {
            var story = new CampaignStory { phase = phase }; story.continuation.area = area; CampaignStorySave.Write(story);
            if (phase == 1) CampaignMemorySave.Write(new CampaignMemory { openingSeen = true });
            yield return SceneManager.LoadSceneAsync(phase >= 11 ? CampaignContinuationDefinition.SceneName(phase, area) : CampaignStorySave.Scene(phase));
#if UNITY_EDITOR
            UnityEditor.EditorWindow.GetWindow(typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.GameView")).Focus();
#endif
            yield return new WaitForSeconds(3.3f);
            if (phase == 1) VarginhaCampaignPhase1.Active.DeliverControl();
            else if (phase == 2 || phase == 4) VarginhaCampaignStage.Active.CloseDialogue();
            else if (phase <= 10) CampaignExpansionController.Active.ClosePanel();
            else CampaignContinuationController.Active.CloseMessage();
            yield return null; yield return null;
        }
        [UnityTest, Timeout(180000)] public IEnumerator EveryChapterAndSubareaHasReadablePostProcessingAndBoundedResources()
        {
            var entries = new List<(int phase, int area)>();
            foreach (int phase in CampaignSequence.Entries) entries.Add((phase, 0));
            entries.Add((10, 0)); entries.Add((12, 1)); entries.Add((13, 0)); entries.Add((21, 1));
            var rows = new List<string> { "chapter,phase,area,tension,baseline_luma,graded_luma,volume_count,dust_count,audio_sources" };
            foreach (var entry in entries)
            {
                yield return Load(entry.phase, entry.area);
                var director = CampaignAtmosphereDirector.Active;
                Assert.That(director, Is.Not.Null, "Director for phase " + entry.phase);
                Assert.That(director.Phase, Is.EqualTo(entry.phase)); Assert.That(director.Area, Is.EqualTo(entry.area));
                Assert.That(Object.FindObjectsByType<CampaignAtmosphereDirector>().Length, Is.EqualTo(1));
                director.Advance(8); yield return null;
                var camera = Camera.main; Assert.That(camera.GetUniversalAdditionalCameraData().renderPostProcessing, Is.True);
                Assert.That(director.AtmosphereVolume.sharedProfile.components.Count, Is.EqualTo(5));
                Assert.That(camera.GetUniversalAdditionalCameraData().volumeLayerMask.value & 1, Is.EqualTo(1));
                string name = CampaignSequence.Chapter(entry.phase).ToString("00") + "_" + entry.phase + "_" + entry.area;
                float graded = Capture(camera, name + "_Atmosfera.png");
                director.enabled = false; yield return null;
                float baseline = Capture(camera, name + "_Original.png");
                Assert.That(graded, Is.GreaterThan(baseline * .62f), "Exposure floor phase " + entry.phase);
                director.enabled = true; yield return null;
                int dust = 0; foreach (var sr in director.GetComponentsInChildren<SpriteRenderer>()) if (sr.name.StartsWith("Poeira_ambiental_")) dust++;
                Assert.That(dust, Is.EqualTo(16));
                rows.Add(FormattableString.Invariant($"{CampaignSequence.Chapter(entry.phase)},{entry.phase},{entry.area},{director.Tension:F3},{baseline:F4},{graded:F4},1,{dust},{Object.FindObjectsByType<AudioSource>().Length}"));
            }
            File.WriteAllLines(Folder + "/Chapters.csv", rows);
            yield return SceneManager.LoadSceneAsync("Menu_MisterioDeVarginha");
            Assert.That(CampaignAtmosphereDirector.Active, Is.Null);
        }
        [UnityTest, Timeout(120000)] public IEnumerator PauseReducedMotionNarrativeTriggersAndTransitionsRemainConsistent()
        {
            yield return Load(12, 1);
            var director = CampaignAtmosphereDirector.Active;
            director.Advance(8); yield return null;
            float clock = director.Clock, tension = director.Tension;
            Time.timeScale = 0; yield return new WaitForSecondsRealtime(.2f);
            Assert.That(director.Clock, Is.EqualTo(clock)); Assert.That(director.Tension, Is.EqualTo(tension));
            Time.timeScale = 1; CampaignAtmosphereDirector.Interference(.8f); yield return new WaitForSeconds(.3f);
            Assert.That(director.PulseWeight, Is.GreaterThan(0));
            VarginhaGameSettings.Current.reducedMotion = true; yield return null;
            Assert.That(director.AtmosphereVolume.sharedProfile.TryGet<FilmGrain>(out var grain), Is.True);
            Assert.That(grain.intensity.value, Is.Zero);
            Assert.That(director.AtmosphereVolume.sharedProfile.TryGet<LensDistortion>(out var distortion), Is.True);
            Assert.That(distortion.intensity.value, Is.Zero);
            foreach (var sr in director.GetComponentsInChildren<SpriteRenderer>()) if (sr.name.StartsWith("Poeira_ambiental_")) Assert.That(sr.enabled, Is.False);
            VarginhaGameSettings.Current.music = 0; yield return new WaitForSeconds(.4f);
            foreach (var source in director.GetComponents<AudioSource>()) if (director.OwnsAtmosphereSource(source)) Assert.That(source.volume, Is.Zero);
            foreach (var source in CampaignAmbientBridge.Instance.GetComponents<AudioSource>()) Assert.That(source.volume, Is.Zero);
            VarginhaGameSettings.Current.reducedMotion = false;
            CampaignCinematics.Load(CampaignContinuationDefinition.SceneName(13));
            float started = Time.realtimeSinceStartup;
            while (CampaignCinematics.IsTransitioning && Time.realtimeSinceStartup - started < 25) yield return null;
            Assert.That(CampaignCinematics.IsTransitioning, Is.False); Assert.That(Time.timeScale, Is.EqualTo(1));
            Assert.That(CampaignAtmosphereDirector.Active.Phase, Is.EqualTo(13));
            yield return Load(20); director = CampaignAtmosphereDirector.Active; director.Advance(8);
            CampaignAtmosphereDirector.Interference(.8f); director.Advance(1); yield return null;
            Assert.That(director.AtmosphereVolume.sharedProfile.TryGet<FilmGrain>(out var finalGrain), Is.True);
            Assert.That(finalGrain.intensity.value, Is.GreaterThan(0));
            Assert.That(director.AtmosphereVolume.sharedProfile.TryGet<LensDistortion>(out var finalDistortion), Is.True);
            Assert.That(Mathf.Abs(finalDistortion.intensity.value), Is.GreaterThan(.0001f));
            Capture(Camera.main, "13_Interferencia.png");
            VarginhaGameSettings.Current.reducedMotion = true; yield return null;
            Assert.That(finalGrain.intensity.value, Is.Zero); Assert.That(finalDistortion.intensity.value, Is.Zero);
            VarginhaGameSettings.Current.reducedMotion = false;
            float before = director.Tension; CampaignContinuationController.Active.State.manifestationDispelled = true;
            director.Advance(8); Assert.That(director.Tension, Is.LessThan(before));
            yield return Load(21, 1); director = CampaignAtmosphereDirector.Active; director.Advance(8);
            Assert.That(director.Tension, Is.LessThan(.1f));
            yield return SceneManager.LoadSceneAsync("Menu_MisterioDeVarginha");
        }
        [UnityTest, Timeout(150000)] public IEnumerator RepresentativeChaptersRecordBeforeAfterPerformance()
        {
            var rows = new List<string> { "phase,enabled,frames,main_thread_ms,gc_bytes_frame,draw_calls,render_submission_ms,atmosphere_update_ms" };
            foreach (int phase in new[] { 2, 7, 12, 20 })
            {
                yield return Load(phase);
                var director = CampaignAtmosphereDirector.Active; director.Advance(8);
                foreach (bool enabled in new[] { false, true })
                {
                    director.enabled = enabled;
                    for (int i = 0; i < 20; i++) yield return null;
                    using var cpu = ProfilerRecorder.StartNew(ProfilerCategory.Internal, "Main Thread", 1);
                    using var gc = ProfilerRecorder.StartNew(ProfilerCategory.Memory, "GC Allocated In Frame", 1);
                    using var draw = ProfilerRecorder.StartNew(ProfilerCategory.Render, "Draw Calls Count", 1);
                    using var atmosphere = ProfilerRecorder.StartNew(ProfilerCategory.Scripts, "Campaign.Atmosphere", 1);
                    double cpuSum = 0, gcSum = 0, drawSum = 0, renderSum = 0, atmosphereSum = 0;
                    var camera = Camera.main; var target = RenderTexture.GetTemporary(960, 540, 24); target.filterMode = FilterMode.Point;
                    try
                    {
                        for (int i = 0; i < 90; i++)
                        {
                            yield return null;
                            long started = System.Diagnostics.Stopwatch.GetTimestamp();
                            // Explicit requests render even when the Editor's Game view
                            // is hidden behind the desktop app. No readback/allocation here.
                            VarginhaPixelPresentation.RenderInto(camera, target);
                            renderSum += (System.Diagnostics.Stopwatch.GetTimestamp() - started) * 1000d / System.Diagnostics.Stopwatch.Frequency;
                            cpuSum += cpu.LastValue; gcSum += gc.LastValue; atmosphereSum += atmosphere.LastValue;
#if UNITY_EDITOR
                            drawSum += UnityEditor.UnityStats.drawCalls;
#else
                            drawSum += draw.LastValue;
#endif
                        }
                    }
                    finally { RenderTexture.ReleaseTemporary(target); }
                    rows.Add(FormattableString.Invariant($"{phase},{enabled},90,{cpuSum / 90 / 1000000:F3},{gcSum / 90:F1},{drawSum / 90:F1},{renderSum / 90:F3},{atmosphereSum / 90 / 1000000:F4}"));
                    Assert.That(cpu.Valid, Is.True, "Main Thread counter available");
                }
            }
            File.WriteAllLines(Folder + "/Performance.csv", rows);
            yield return SceneManager.LoadSceneAsync("Menu_MisterioDeVarginha");
        }
        [UnityTest, Timeout(40000)] public IEnumerator ReturningToMenuDoesNotCarryFinalHorrorIntoANewCampaign()
        {
            yield return Load(20); CampaignAtmosphereDirector.Active.Advance(8);
            Assert.That(CampaignAtmosphereDirector.Active.Tension, Is.GreaterThan(.9f));
            yield return SceneManager.LoadSceneAsync("Menu_MisterioDeVarginha");
            yield return Load(2);
            Assert.That(CampaignAtmosphereDirector.Active.Tension, Is.LessThan(.1f));
            yield return SceneManager.LoadSceneAsync("Menu_MisterioDeVarginha");
        }
        static float Capture(Camera camera, string name)
        {
            var previous = camera.targetTexture; var active = RenderTexture.active;
            var target = RenderTexture.GetTemporary(960, 540, 24); target.filterMode = FilterMode.Point;
            var image = new Texture2D(960, 540, TextureFormat.RGB24, false);
            try
            {
                camera.targetTexture = target; VarginhaPixelPresentation.RenderInto(camera, target); RenderTexture.active = target;
                image.ReadPixels(new Rect(0, 0, 960, 540), 0, 0); image.Apply();
                File.WriteAllBytes(Folder + "/" + name, image.EncodeToPNG());
                double sum = 0; var pixels = image.GetPixels32();
                // Centre remains readable; cinematic edge falloff is allowed.
                for (int y = 108; y < 432; y++) for (int x = 192; x < 768; x++)
                { var p = pixels[y * 960 + x]; sum += (.2126 * p.r + .7152 * p.g + .0722 * p.b) / 255; }
                return (float)(sum / (324 * 576));
            }
            finally { camera.targetTexture = previous; RenderTexture.active = active; RenderTexture.ReleaseTemporary(target); Object.Destroy(image); }
        }
    }
}
