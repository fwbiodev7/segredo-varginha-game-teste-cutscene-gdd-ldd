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
        public const string Name = "jogo_varginha_alpha_0.1";
        public static string[] Scenes => new[] { "Assets/Scenes/Menu_MisterioDeVarginha.unity" }
            .Concat(CampaignSequence.Entries.Concat(new[] { 10, 13 }).Select(id => "Assets/Scenes/" + CampaignStorySave.Scene(id) + ".unity"))
            .Concat(new[] { "Assets/Scenes/" + CampaignContinuationDefinition.SceneName(12, 1) + ".unity", "Assets/Scenes/" + CampaignContinuationDefinition.SceneName(21, 1) + ".unity" })
            .Distinct().ToArray();

        [MenuItem("Varginha/Build/Alpha Windows x64")]
        public static void Build()
        {
            if (Application.isPlaying) throw new InvalidOperationException("Encerre o Play antes de gerar a build alpha.");
            foreach (string scene in Scenes)
                if (!File.Exists(scene)) throw new FileNotFoundException("Cena necessária à campanha.", scene);
            string folder = Path.GetFullPath(Path.Combine("Builds", Name));
            Directory.CreateDirectory(folder);
            var report = BuildPipeline.BuildPlayer(new BuildPlayerOptions {
                scenes = Scenes, locationPathName = Path.Combine(folder, Name + ".exe"),
                target = BuildTarget.StandaloneWindows64, options = BuildOptions.CompressWithLz4HC | BuildOptions.DetailedBuildReport
            });
            var summary = report.summary;
            Directory.CreateDirectory("Logs");
            File.WriteAllText("Logs/AlphaBuildReport.json", JsonUtility.ToJson(new Result {
                result = summary.result.ToString(), output = summary.outputPath, bytes = summary.totalSize,
                seconds = summary.totalTime.TotalSeconds, errors = summary.totalErrors, warnings = summary.totalWarnings,
                scenes = Scenes, unity = Application.unityVersion
            }, true));
            if (summary.result != BuildResult.Succeeded) throw new InvalidOperationException("Build alpha falhou: " + summary.result);
            Debug.Log("BUILD_ALPHA_PRONTA=" + summary.outputPath);
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
