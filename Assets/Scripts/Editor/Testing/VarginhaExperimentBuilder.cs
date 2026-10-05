using System.IO;
using System.Linq;
using Game.Varginha;
using Game.Varginha.Experiment;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.Editor.Testing
{
    public static class VarginhaExperimentBuilder
    {
        public const string ScenePath = "Assets/Scenes/Laboratorio_GDD_Cutscene.unity";
        [MenuItem("Varginha/Experimentos/Preparar laboratório GDD")]
        public static void Build()
        {
            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
            foreach (string file in Directory.GetFiles("Assets/Resources/Varginha/Experiment", "*.png"))
            {
                var importer = AssetImporter.GetAtPath(file) as TextureImporter;
                if (importer == null) continue;
                importer.textureType = TextureImporterType.Default;
                importer.filterMode = FilterMode.Point;
                importer.mipmapEnabled = false;
                importer.textureCompression = TextureImporterCompression.Uncompressed;
                importer.isReadable = true;
                importer.alphaIsTransparency = true;
                importer.npotScale = TextureImporterNPOTScale.None;
                importer.maxTextureSize = file.Contains("FuscaTopView") ? 256 : file.Contains("FuscaBreakdown") ? 1024 : file.Contains("GameCast") ? 128 : file.Contains("ChildEdelzio") ? 256 : file.Contains("ReferenceCast") ? 512 : 2048;
                importer.SaveAndReimport();
            }
            VarginhaPixelPresentation.Pipeline pipeline = VarginhaPixelPresentation.DetectPipeline();
            Debug.Log("EXPERIMENT_PIPELINE=" + pipeline);
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            new GameObject("Laboratorio_GDD_Cutscene").AddComponent<VarginhaExperimentLab>();
            EditorSceneManager.SaveScene(scene, ScenePath);
            var scenes = EditorBuildSettings.scenes.ToList();
            if (!scenes.Any(item => item.path == ScenePath))
                scenes.Add(new EditorBuildSettingsScene(ScenePath, true));
            else foreach (var item in scenes) if (item.path == ScenePath) item.enabled = true;
            EditorBuildSettings.scenes = scenes.ToArray();
            AssetDatabase.SaveAssets();
            ExportFrames();
            BuildCampaign();
            Debug.Log("EXPERIMENT_LAB_READY=" + ScenePath);
        }
        public static void BuildCampaign()
        {
            var house = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            new GameObject("Campanha_Ato1_Fase1").AddComponent<VarginhaCampaignPhase1>();
            string path = "Assets/Scenes/" + VarginhaCampaignPhase1.SceneName + ".unity";
            EditorSceneManager.SaveScene(house, path);
            var scenes = EditorBuildSettings.scenes.ToList();
            if (!scenes.Any(s => s.path == path)) scenes.Add(new EditorBuildSettingsScene(path, true));
            EditorBuildSettings.scenes = scenes.ToArray();
            BuildFollowingChapters();
            EditorSceneManager.OpenScene("Assets/Scenes/Menu_MisterioDeVarginha.unity");
            AssetDatabase.SaveAssets();
            Debug.Log("EXPERIMENT_CAMPAIGN_READY=" + path);
        }
        public static void BuildFollowingChapters()
        {
            for (int phase = 2; phase <= 5; phase++)
            {
                var scene = phase == 2 ? EditorSceneManager.OpenScene("Assets/Scenes/FaseTopView_Varginha.unity")
                    : EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
                var root = new GameObject("Campanha_Ato2_Fase" + phase);
                if (phase == 3) root.AddComponent<VarginhaCampaignDrive>();
                else root.AddComponent<VarginhaCampaignStage>().phase = phase;
                string path = "Assets/Scenes/" + CampaignStorySave.Scene(phase) + ".unity";
                EditorSceneManager.SaveScene(scene, path);
                var scenes = EditorBuildSettings.scenes.ToList();
                if (!scenes.Any(s => s.path == path)) scenes.Add(new EditorBuildSettingsScene(path, true));
                EditorBuildSettings.scenes = scenes.ToArray();
            }
            // Remove the synthetic speech from shipped Resources through the asset database.
            foreach (string name in new[] { "NewsAnchor", "Witness", "Memory" })
                AssetDatabase.DeleteAsset("Assets/Resources/Varginha/Experiment/Audio/" + name + ".wav");
            AssetDatabase.SaveAssets();
        }
        public static void ImportCampaignArt()
        {
            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
            foreach (string name in new[] { "FuscaBreakdown", "FuscaTopView" })
            {
                var importer = AssetImporter.GetAtPath("Assets/Resources/Varginha/Experiment/" + name + ".png") as TextureImporter;
                if (importer == null) throw new System.InvalidOperationException("Asset ausente: " + name);
                importer.textureType = TextureImporterType.Default; importer.isReadable = true;
                importer.filterMode = FilterMode.Point; importer.mipmapEnabled = false; importer.alphaIsTransparency = true;
                importer.textureCompression = TextureImporterCompression.Uncompressed; importer.wrapMode = TextureWrapMode.Clamp;
                importer.npotScale = TextureImporterNPOTScale.None; importer.maxTextureSize = name == "FuscaTopView" ? 256 : 1024;
                importer.SaveAndReimport();
            }
        }
        public static void ExportFrames()
        {
            string directory = "Preview/Frames";
            Directory.CreateDirectory(directory);
            var data = ExperimentDefinition.Load();
            var composer = new ExperimentFrameComposer();
            foreach (var shot in data.shots)
            {
                composer.Compose(shot, shot.duration / 2, false);
                File.WriteAllBytes(Path.Combine(directory, shot.id + ".png"), composer.Frame.EncodeToPNG());
            }
            // Editor-owned preview texture must be cleaned without deferred runtime destruction.
            Object.DestroyImmediate(composer.Frame);
        }
        [MenuItem("Varginha/Experimentos/Abrir laboratório")]
        public static void Open()
        {
            EditorSceneManager.OpenScene(ScenePath);
            SessionState.SetBool("Varginha.PlayCurrentSceneOnce", true);
            EditorSceneManager.playModeStartScene = null;
        }
        public static void BuildPlayer()
        {
            Build();
            PlayerSettings.productName = "O Segredo de Varginha - Laboratorio";
            PlayerSettings.defaultScreenWidth = 1280;
            PlayerSettings.defaultScreenHeight = 720;
            PlayerSettings.fullScreenMode = FullScreenMode.FullScreenWindow;
            Directory.CreateDirectory("Builds/Laboratorio");
            var report = BuildPipeline.BuildPlayer(new BuildPlayerOptions {
                scenes = EditorBuildSettings.scenes.Where(s => s.enabled && File.Exists(s.path)).Select(s => s.path).ToArray(),
                locationPathName = "Builds/Laboratorio/O_Segredo_de_Varginha_Laboratorio.exe",
                target = BuildTarget.StandaloneWindows64,
                options = BuildOptions.None
            });
            if (report.summary.result != UnityEditor.Build.Reporting.BuildResult.Succeeded)
                throw new System.InvalidOperationException("Experimental player build failed: " + report.summary.result);
            Debug.Log("EXPERIMENT_PLAYER_READY=" + report.summary.outputPath);
        }
    }
}
