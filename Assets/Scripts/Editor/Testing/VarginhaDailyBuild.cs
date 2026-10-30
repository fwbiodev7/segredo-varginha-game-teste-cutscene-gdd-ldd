using System;
using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace Game.Editor.Testing
{
    // A separate candidate preserves previous deliveries and restores the project's version.
    public static class VarginhaDailyBuild
    {
        public const string Version = "0.6.0-apresentacao";
        public const string Folder = "Builds/Oizlede_0.6.0_Apresentacao_2026-10-30";
        public const string Executable = "Oizlede - Segredo de Varginha.exe";

        [MenuItem("Varginha/Build/Apresentação 30-10 Windows x64")]
        public static void Build()
        {
            if (Application.isPlaying) throw new InvalidOperationException("Encerre o Play antes da build.");
            string root=Environment.GetEnvironmentVariable("VARGINHA_FINAL_BUILD_ROOT")??Directory.GetCurrentDirectory();
            string folder=Path.GetFullPath(Path.Combine(root,Folder));
            if (Directory.Exists(folder)) Directory.Delete(folder, true);
            foreach (string scene in VarginhaAlphaBuild.Scenes)
                if (!File.Exists(scene)) throw new FileNotFoundException("Cena necessária à campanha.", scene);
            string previousVersion = PlayerSettings.bundleVersion;
            try
            {
                PlayerSettings.bundleVersion = Version;
                Directory.CreateDirectory(folder);
                var report = BuildPipeline.BuildPlayer(new BuildPlayerOptions {
                    scenes = VarginhaAlphaBuild.Scenes,
                    locationPathName = Path.Combine(folder, Executable),
                    target = BuildTarget.StandaloneWindows64,
                    options = BuildOptions.CompressWithLz4HC | BuildOptions.DetailedBuildReport
                });
                var s = report.summary;
                var result = new Result { result = s.result.ToString(), output = s.outputPath,
                    bytes = s.totalSize, seconds = s.totalTime.TotalSeconds, errors = s.totalErrors,
                    warnings = s.totalWarnings, scenes = VarginhaAlphaBuild.Scenes,
                    unity = Application.unityVersion, version = Version };
                string evidence=Environment.GetEnvironmentVariable("VARGINHA_CINEMA_QA")??"output/completion-2026-10-27";
                Directory.CreateDirectory(evidence);
                File.WriteAllText(Path.Combine(evidence,"windows-build.json"), JsonUtility.ToJson(result, true));
                if (s.result != BuildResult.Succeeded) throw new InvalidOperationException("Build falhou: " + s.result);
                Debug.Log("BUILD_PRONTA=" + s.outputPath);
            }
            finally { PlayerSettings.bundleVersion = previousVersion; }
        }

        [Serializable] private sealed class Result
        {
            public string result, output, unity, version;
            public ulong bytes;
            public double seconds;
            public int errors, warnings;
            public string[] scenes;
        }
    }
}
