using System;
using System.IO;
using System.Linq;
using Game.Varginha.Experiment;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace Game.Editor.Testing
{
    public static class VarginhaAlphaBuild
    {
        public const string Version = "0.3.0";
        public const string Name = "jogo_varginha_alpha_0.3";
        public static string[] Scenes => new[] { "Assets/Scenes/Menu_MisterioDeVarginha.unity" }
            .Concat(CampaignSequence.Entries.Concat(new[] { 10, 13 }).Select(id => "Assets/Scenes/" + CampaignStorySave.Scene(id) + ".unity"))
            .Concat(new[] { "Assets/Scenes/" + CampaignContinuationDefinition.SceneName(12, 1) + ".unity", "Assets/Scenes/" + CampaignContinuationDefinition.SceneName(21, 1) + ".unity" })
            .Distinct().ToArray();

        [MenuItem("Varginha/Build/Alpha Windows x64")]
        public static void Build() => BuildVersion(Version, Name, "AlphaBuildReport.json");

        [MenuItem("Varginha/Build/Beta 0.4 Windows x64")]
        public static void BuildBeta() => BuildVersion("0.4.0-beta", "jogo_varginha_beta_0.4", "BetaBuildReport.json");

        private static void BuildVersion(string version, string name, string reportName)
        {
            if (Application.isPlaying) throw new InvalidOperationException("Encerre o Play antes de gerar a build.");
            PlayerSettings.bundleVersion = version;
            AssetDatabase.SaveAssets();
            foreach (string scene in Scenes)
                if (!File.Exists(scene)) throw new FileNotFoundException("Cena necessária à campanha.", scene);
            string folder = Path.GetFullPath(Path.Combine("Builds", name));
            Directory.CreateDirectory(folder);
            var report = BuildPipeline.BuildPlayer(new BuildPlayerOptions {
                scenes = Scenes, locationPathName = Path.Combine(folder, name + ".exe"),
                target = BuildTarget.StandaloneWindows64, options = BuildOptions.CompressWithLz4HC | BuildOptions.DetailedBuildReport
            });
            var summary = report.summary;
            Directory.CreateDirectory("Logs");
            File.WriteAllText(Path.Combine("Logs", reportName), JsonUtility.ToJson(new Result {
                result = summary.result.ToString(), output = summary.outputPath, bytes = summary.totalSize,
                seconds = summary.totalTime.TotalSeconds, errors = summary.totalErrors, warnings = summary.totalWarnings,
                scenes = Scenes, unity = Application.unityVersion
            }, true));
            if (summary.result != BuildResult.Succeeded) throw new InvalidOperationException("Build falhou: " + summary.result);
            Debug.Log("BUILD_PRONTA=" + summary.outputPath);
        }
        [Serializable] private sealed class Result
        {
            public string result, output, unity;
            public ulong bytes;
            public double seconds;
            public int errors, warnings;
            public string[] scenes;
        }
    }
}
