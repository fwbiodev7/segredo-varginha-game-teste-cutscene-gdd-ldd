using System.IO;
using Game.Varginha;
using Game.Varginha.Experiment;
using UnityEngine;

namespace Game.Editor.Testing
{
    public static class CampaignHouseFeedbackBuilder
    {
        private static bool _burstCaptured;
        public static void CaptureFlashSequence()
        {
            _burstCaptured=false;
            UnityEditor.EditorApplication.update-=CaptureFlashFrame;
            UnityEditor.EditorApplication.update+=CaptureFlashFrame;
        }
        private static void CaptureFlashFrame()
        {
            if(!Application.isPlaying){UnityEditor.EditorApplication.update-=CaptureFlashFrame;return;}
            var flash=Object.FindAnyObjectByType<CampaignFlashEncounter>();
            if(flash==null)return;
            if(!_burstCaptured&&flash.Elapsed>=.5f&&flash.Elapsed<1.1f)
            { CaptureYard("Clarao_Explosao.png");_burstCaptured=true; }
            if(flash.IsLying&&flash.Elapsed>=1.85f)
            { CaptureYard("Clarao_Crianca_Caida.png");UnityEditor.EditorApplication.update-=CaptureFlashFrame; }
        }
        private static void CaptureYard(string name)
        {
            var root=new GameObject("FlashPreviewCamera");
            try
            {
                var camera=root.AddComponent<Camera>();camera.enabled=false;camera.orthographic=true;camera.orthographicSize=4.2f;
                camera.transform.position=new Vector3(14.5f,0,-10);camera.backgroundColor=new Color(.05f,.07f,.1f);camera.allowHDR=camera.allowMSAA=false;
                Capture(camera,name,1280,720);
            }
            finally{Object.DestroyImmediate(root);}
        }
        public static void CaptureAdultHouse()
        {
            var root=new GameObject("FeedbackPreviewCamera");
            try
            {
                var camera=root.AddComponent<Camera>();camera.enabled=false;camera.orthographic=true;camera.orthographicSize=7.8f;
                camera.transform.position=new Vector3(0,0,-10);camera.backgroundColor=new Color(.05f,.07f,.1f);camera.allowHDR=camera.allowMSAA=false;
                Capture(camera,"Fase2_Adulto_Ajustado.png",1600,1200);
            }
            finally{Object.DestroyImmediate(root);}
        }
        public static void ExportEquippedPoses()
        {
            foreach(var sheet in new[]{("EdelzioWalk",4),("EdelzioPunch",3),("EdelzioActions",6),("EdelzioInteractions",6)})
            {
                var root=new GameObject("EquippedPosePreview");
                try
                {
                    for(int row=0;row<4;row++)for(int column=0;column<sheet.Item2;column++)
                    {
                        var go=new GameObject("Pose");go.transform.SetParent(root.transform);go.transform.position=new Vector3(1000+(column-(sheet.Item2-1)*.5f)*2,1003.3f-row*2.1f);
                        go.AddComponent<SpriteRenderer>().sprite=CampaignTeamEdelzio.Frame(sheet.Item1,row,column,true);
                    }
                    var camera=new GameObject("PreviewCamera").AddComponent<Camera>();camera.transform.SetParent(root.transform);camera.enabled=false;
                    camera.orthographic=true;camera.orthographicSize=4.5f;camera.transform.position=new Vector3(1000,1000.8f,-10);
                    camera.backgroundColor=new Color(.12f,.18f,.19f);camera.allowHDR=camera.allowMSAA=false;
                    Capture(camera,sheet.Item1+"_Mochila.png",sheet.Item2*240,960);
                }
                finally{Object.DestroyImmediate(root);}
            }
        }
        public static void ExportReporterMouth()
        {
            var root=new GameObject("ReporterPreview");var composer=new ExperimentFrameComposer();
            try
            {
                var shot=System.Array.Find(ExperimentDefinition.Load().shots,s=>s.id=="news");
                float[] times={0,.46f,.72f};
                for(int i=0;i<times.Length;i++)
                {
                    var image=new Texture2D(384,216,TextureFormat.RGBA32,false){filterMode=FilterMode.Point};
                    image.SetPixels32(composer.Compose(shot,times[i],false).GetPixels32());image.Apply();
                    var go=new GameObject("Boca "+i);go.transform.SetParent(root.transform);go.transform.position=new Vector3(1000+i*2.2f,1000);
                    go.AddComponent<SpriteRenderer>().sprite=Sprite.Create(image,new Rect(174,112,40,50),Vector2.one/2,20,0,SpriteMeshType.FullRect);
                }
                var camera=new GameObject("Camera").AddComponent<Camera>();camera.transform.SetParent(root.transform);camera.enabled=false;
                camera.orthographic=true;camera.orthographicSize=1.55f;camera.transform.position=new Vector3(1002.2f,1000,-10);
                camera.backgroundColor=new Color(.05f,.07f,.1f);camera.allowHDR=camera.allowMSAA=false;
                Capture(camera,"Jornalista_Boca.png",960,400);
                foreach(var renderer in root.GetComponentsInChildren<SpriteRenderer>()){Object.DestroyImmediate(renderer.sprite.texture);Object.DestroyImmediate(renderer.sprite);}
            }
            finally{composer.Dispose();Object.DestroyImmediate(root);}
        }
        private static void Capture(Camera camera,string name,int width,int height)
        {
            Directory.CreateDirectory("Preview/CampaignMapsV2");
            var target=new RenderTexture(width,height,24);var previous=RenderTexture.active;
            camera.targetTexture=target;camera.Render();RenderTexture.active=target;
            var image=new Texture2D(width,height,TextureFormat.RGBA32,false);image.ReadPixels(new Rect(0,0,width,height),0,0);image.Apply();
            File.WriteAllBytes("Preview/CampaignMapsV2/"+name,image.EncodeToPNG());
            RenderTexture.active=previous;camera.targetTexture=null;Object.DestroyImmediate(image);target.Release();Object.DestroyImmediate(target);
        }
    }
}
