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
        public const string Version = "0.4.5-beta";
        public const string Folder = "Builds/Varginha_0.4.5-beta_2026-10-22";

        [MenuItem("Varginha/Build/Daily 22-10 Windows x64")]
        public static void Build()
        {
            if (Application.isPlaying) throw new InvalidOperationException("Encerre o Play antes da build.");
            if (Directory.Exists(Folder)) throw new IOException("A entrega já existe: " + Folder);
            foreach (string scene in VarginhaAlphaBuild.Scenes)
                if (!File.Exists(scene)) throw new FileNotFoundException("Cena necessária à campanha.", scene);
            string previousVersion = PlayerSettings.bundleVersion;
            try
            {
                PlayerSettings.bundleVersion = Version;
                Directory.CreateDirectory(Folder);
                var report = BuildPipeline.BuildPlayer(new BuildPlayerOptions {
                    scenes = VarginhaAlphaBuild.Scenes,
                    locationPathName = Path.GetFullPath(Path.Combine(Folder, "SegredoDeVarginha.exe")),
                    target = BuildTarget.StandaloneWindows64,
                    options = BuildOptions.CompressWithLz4HC | BuildOptions.DetailedBuildReport
                });
                var s = report.summary;
                var result = new Result { result = s.result.ToString(), output = s.outputPath,
                    bytes = s.totalSize, seconds = s.totalTime.TotalSeconds, errors = s.totalErrors,
                    warnings = s.totalWarnings, scenes = VarginhaAlphaBuild.Scenes,
                    unity = Application.unityVersion, version = Version };
                string evidence=Environment.GetEnvironmentVariable("VARGINHA_CINEMA_QA")??"output/house-interactions-2026-10-22";
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
