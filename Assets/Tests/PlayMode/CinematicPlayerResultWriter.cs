using System.IO;
using NUnit.Framework.Interfaces;
using UnityEngine;
using UnityEngine.TestRunner;

[assembly:TestRunCallback(typeof(Game.Tests.PlayMode.CinematicPlayerResultWriter))]
namespace Game.Tests.PlayMode
{
    // Player-native NUnit evidence survives an unavailable EditorConnection callback.
    // This does not reinterpret failures or count an unfinished run as passed.
    public sealed class CinematicPlayerResultWriter:ITestRunCallback
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void KeepQaRunning(){if(!Application.isEditor)Application.runInBackground=true;}
        public void RunStarted(ITest testsToRun)=>KeepQaRunning();
        public void TestStarted(ITest test)=>KeepQaRunning();
        public void TestFinished(ITestResult result)
        {
            if(Application.isEditor||result.Test.IsSuite)return;
            string folder=Path.Combine(Path.GetFullPath(CinemaQaEvidence.Folder),"per-case");Directory.CreateDirectory(folder);
            string name=result.FullName;
            foreach(char invalid in Path.GetInvalidFileNameChars())name=name.Replace(invalid,'_');
            if(name.Length>180)name=name.Substring(0,180)+"_"+result.Test.Id;
            File.WriteAllText(Path.Combine(folder,name+".xml"),result.ToXml(true).OuterXml);
            Debug.Log("NATIVE_TEST "+result.ResultState+" "+result.FullName+(result.ResultState.Status==TestStatus.Failed?"\n"+result.Message+"\n"+result.StackTrace:""));
        }
        public void RunFinished(ITestResult result)
        {
            if(Application.isEditor||result.Test.Parent!=null)return;
            string folder=Path.GetFullPath(CinemaQaEvidence.Folder);Directory.CreateDirectory(folder);
            File.WriteAllText(Path.Combine(folder,"player-native.xml"),result.ToXml(true).OuterXml);
            Debug.Log("NATIVE_RUN_COMPLETE "+result.ResultState+" cases="+result.Test.TestCaseCount);
        }
    }
}
