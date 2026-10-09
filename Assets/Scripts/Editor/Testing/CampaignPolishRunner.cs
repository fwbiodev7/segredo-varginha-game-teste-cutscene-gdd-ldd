using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.TestTools.TestRunner.Api;
using UnityEngine;
using UnityEngine.Rendering;

namespace Game.Editor.Testing
{
    // Explicit file requests allow QA on an already open editor without closing the user's project.
    [InitializeOnLoad]
    public static class CampaignPolishRunner
    {
        private const string RequestPath = "Library/CampaignPolishRequest.json";
        public const string Output = "Docs/QAAlpha01";
        [Serializable] private class Request { public string action, mode; public string[] names; }
        static CampaignPolishRunner() => EditorApplication.update += Poll;
        private static void Poll()
        {
            if (EditorApplication.isCompiling || EditorApplication.isUpdating || !File.Exists(RequestPath)) return;
            string signature=File.GetLastWriteTimeUtc(RequestPath).Ticks.ToString();
            if(SessionState.GetString("CampaignPolishRefresh", "")!=signature)
            {
                SessionState.SetString("CampaignPolishRefresh",signature);
                SessionState.SetFloat("CampaignPolishReady",(float)EditorApplication.timeSinceStartup+3);
                AssetDatabase.Refresh();
                return;
            }
            if(EditorApplication.timeSinceStartup<SessionState.GetFloat("CampaignPolishReady",0))return;
            var request = JsonUtility.FromJson<Request>(File.ReadAllText(RequestPath));
            File.Delete(RequestPath);
            Directory.CreateDirectory(Output);
            try
            {
                if (request.action == "inspect")
                    File.WriteAllText(Output + "/EditorState.txt", "Play=" + Application.isPlaying + "\nPipeline=" + GraphicsSettings.currentRenderPipeline + "\nQuality=" + QualitySettings.renderPipeline + "\nScenes=" + string.Join(",", EditorSceneManager.GetSceneManagerSetup().Select(s => s.path)));
                else if (request.action == "test")
                {
                    var api = ScriptableObject.CreateInstance<TestRunnerApi>();
                    api.RegisterCallbacks(new Results(request.mode, api));
                    api.Execute(new ExecutionSettings(new Filter { testMode = request.mode == "PlayMode" ? TestMode.PlayMode : TestMode.EditMode, testNames = request.names }));
                }
                else if (request.action == "build") VarginhaAlphaBuild.Build();
                else if (request.action == "pipeline") CampaignPixelPipelineBuilder.Configure();
                else if (request.action == "open") EditorSceneManager.OpenScene("Assets/Scenes/Ato3_Fase6_Fragmentos.unity");
                else throw new ArgumentException("Unknown QA action: " + request.action);
            }
            catch (Exception error)
            {
                File.WriteAllText(Output + "/RunnerError.txt", error.ToString());
                Debug.LogException(error);
            }
        }
        private sealed class Results : ICallbacks
        {
            private readonly string _mode;
            private readonly TestRunnerApi _api;
            public Results(string mode, TestRunnerApi api) { _mode = mode; _api = api; }
            public void RunStarted(ITestAdaptor test) => File.WriteAllText(Output + "/" + _mode + "Progress.txt", "Started " + test.TestCaseCount + "\n");
            public void TestStarted(ITestAdaptor test) { if (!test.IsSuite) File.AppendAllText(Output + "/" + _mode + "Progress.txt", "RUN " + test.FullName + "\n"); }
            public void TestFinished(ITestResultAdaptor result) { if (!result.Test.IsSuite) File.AppendAllText(Output + "/" + _mode + "Progress.txt", result.ResultState + " " + result.Test.FullName + " " + result.Message + "\n"); }
            public void RunFinished(ITestResultAdaptor result)
            {
                bool empty = result.Test.TestCaseCount == 0;
                TestRunnerApi.SaveResultToFile(result, Output + "/" + _mode + (empty ? "EmptySelection.xml" : "Results.xml"));
                if (empty) File.WriteAllText(Output + "/RunnerError.txt", "No tests executed for " + _mode + ". The previous non-empty result was preserved.");
                // Callbacks are global in the Test Runner. Remove this request's listener
                // so a later EditMode run cannot overwrite the PlayMode evidence.
                _api.UnregisterCallbacks(this);
                UnityEngine.Object.DestroyImmediate(_api);
            }
        }
    }
}
