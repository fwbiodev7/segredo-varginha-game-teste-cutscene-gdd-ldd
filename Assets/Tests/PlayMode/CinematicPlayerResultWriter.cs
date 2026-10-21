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
        public void RunStarted(ITest testsToRun){}
        public void TestStarted(ITest test){}
        public void TestFinished(ITestResult result){if(!Application.isEditor&&result.Test.IsSuite==false)Debug.Log("NATIVE_TEST "+result.ResultState+" "+result.FullName);}
        public void RunFinished(ITestResult result)
        {
            if(Application.isEditor||result.Test.Parent!=null)return;
            string folder=Path.GetFullPath(CinemaQaEvidence.Folder);Directory.CreateDirectory(folder);
            File.WriteAllText(Path.Combine(folder,"player-native.xml"),result.ToXml(true).OuterXml);
            Debug.Log("NATIVE_RUN_COMPLETE "+result.ResultState+" cases="+result.Test.TestCaseCount);
        }
    }
}
