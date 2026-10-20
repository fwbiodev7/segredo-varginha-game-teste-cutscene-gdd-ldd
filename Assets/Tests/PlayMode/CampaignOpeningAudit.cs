using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.Tests.PlayMode
{
    // Alternate execution of the same walkthrough when the Editor TestRunner's
    // PlayModeRunTask fails during its own domain reload. No production dependency.
    public sealed class CampaignOpeningAudit:MonoBehaviour
    {
        private readonly List<string> _runtimeErrors=new();
        private void Log(string message,string stack,LogType type){if(type==LogType.Exception||type==LogType.Error)_runtimeErrors.Add(message+"\n"+stack);}
        public static void Begin(){new GameObject("Campaign_Opening_Audit").AddComponent<CampaignOpeningAudit>();}
        private IEnumerator Start()
        {
            DontDestroyOnLoad(gameObject);
            Application.logMessageReceived+=Log;
            var context=NUnit.Framework.Internal.TestExecutionContext.CurrentContext;
            var previousExecution=NUnit.Framework.TestContext.CurrentTestExecutionContext;
            NUnit.Framework.TestContext.CurrentTestExecutionContext=context;
            var previousTest=context.CurrentTest;var previousResult=context.CurrentResult;
            context.CurrentTest=new NUnit.Framework.Internal.TestMethod(new NUnit.Framework.Internal.MethodWrapper(typeof(CampaignChaptersTests),nameof(CampaignChaptersTests.HouseDriveSchoolAndCodeFormAPlayableSavedCampaign)));
            context.CurrentResult=context.CurrentTest.MakeTestResult();
            var fixture=new CampaignChaptersTests();Exception failure=null;
            try{fixture.Setup();fixture.PreserveSaves();}catch(Exception e){failure=e;}
            var stack=new Stack<IEnumerator>();
            if(failure==null)stack.Push(fixture.HouseDriveSchoolAndCodeFormAPlayableSavedCampaign());
            while(stack.Count>0&&failure==null)
            {
                object current=null;bool moved=false;
                try{moved=stack.Peek().MoveNext();if(moved)current=stack.Peek().Current;}catch(Exception e){failure=e;}
                if(failure!=null)break;
                if(!moved){stack.Pop();continue;}
                if(current is IEnumerator nested && !(current is CustomYieldInstruction))stack.Push(nested);
                else yield return current;
            }
            Time.timeScale=1;
            yield return SceneManager.LoadSceneAsync("Menu_MisterioDeVarginha");yield return null;
            try{fixture.RestoreSaves();fixture.TearDown();}catch(Exception e){failure??=e;}
            Application.logMessageReceived-=Log;
            if(_runtimeErrors.Count>0)failure??=new Exception(string.Join("\n",_runtimeErrors));
            context.CurrentTest=previousTest;context.CurrentResult=previousResult;
            NUnit.Framework.TestContext.CurrentTestExecutionContext=previousExecution;
            Directory.CreateDirectory("output/key-investigation-2026-10-20");
            File.WriteAllText("output/key-investigation-2026-10-20/playable-audit.json",JsonUtility.ToJson(new Result{passed=failure==null,error=failure?.ToString()},true));
            Debug.Log("Adult opening audit: "+(failure==null?"PASSED":failure.Message));Destroy(gameObject);
        }
        [Serializable]private sealed class Result{public bool passed;public string error;}
    }
}
