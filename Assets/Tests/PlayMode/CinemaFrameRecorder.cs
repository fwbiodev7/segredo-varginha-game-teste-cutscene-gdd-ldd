using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.Tests.PlayMode
{
    public static class CinemaQaEvidence
    {
        public static string Folder=>System.Environment.GetEnvironmentVariable("VARGINHA_CINEMA_QA")??"output/cinematic-opening-2026-10-21";
    }
    // Real rendered frames for a lightweight silent preview, not a recreated mockup.
    public sealed class CinemaFrameRecorder:MonoBehaviour
    {
        public string folderName="film";
        private IEnumerator Start()
        {
            if(Application.isEditor)yield break;
            string folder=CinemaQaEvidence.Folder+"/"+folderName;Directory.CreateDirectory(folder);
            string scene=SceneManager.GetActiveScene().name;
            int n=0;
            while(SceneManager.GetActiveScene().name==scene)
            {
                yield return new WaitForEndOfFrame();
                var screen=ScreenCapture.CaptureScreenshotAsTexture();
                var rt=RenderTexture.GetTemporary(640,360,0,RenderTextureFormat.ARGB32);var previous=RenderTexture.active;
                var frame=new Texture2D(640,360,TextureFormat.RGB24,false);
                try{Graphics.Blit(screen,rt);RenderTexture.active=rt;frame.ReadPixels(new Rect(0,0,640,360),0,0);frame.Apply();File.WriteAllBytes(folder+"/"+(n++).ToString("D4")+".png",frame.EncodeToPNG());}
                finally{RenderTexture.active=previous;RenderTexture.ReleaseTemporary(rt);Destroy(frame);Destroy(screen);}
                yield return new WaitForSeconds(.25f);
            }
        }
    }
}
