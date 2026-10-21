using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.Tests.PlayMode
{
    // Real rendered frames for a lightweight silent preview, not a recreated mockup.
    public sealed class CinemaFrameRecorder:MonoBehaviour
    {
        private IEnumerator Start()
        {
            if(Application.isEditor)yield break;
            const string folder="output/cinematic-opening-2026-10-21/film";Directory.CreateDirectory(folder);
            int n=0;
            while(SceneManager.GetActiveScene().name==Game.Varginha.Experiment.CampaignStorySave.Scene(2))
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
