using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.Tests.PlayMode
{
    // Component fixtures must not inherit a campaign/menu scene from a previous fixture.
    public abstract class IsolatedPlayModeTests
    {
        private static int _number;
        [SetUp] public void CreateComponentSandbox()=>ResetScene();
        public static void ResetScene()
        {
            Time.timeScale=1;
            var previous=new Scene[SceneManager.sceneCount];
            for(int i=0;i<previous.Length;i++)previous[i]=SceneManager.GetSceneAt(i);
            var sandbox=SceneManager.CreateScene("QA_Component_"+(++_number));
            SceneManager.SetActiveScene(sandbox);
            foreach(var scene in previous)
            {
                if(!scene.isLoaded||scene.name.StartsWith("InitTestScene"))continue;
                foreach(var root in scene.GetRootGameObjects())Object.DestroyImmediate(root);
                SceneManager.UnloadSceneAsync(scene);
            }
        }
    }
}
