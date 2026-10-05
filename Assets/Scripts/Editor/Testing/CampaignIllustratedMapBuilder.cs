using System.IO;
using UnityEditor;
using UnityEngine;
using Game.Varginha.Experiment;

namespace Game.Editor.Testing
{
    public static class CampaignIllustratedMapBuilder
    {
        public static void ImportUserArtwork()
        {
            const string folder="Assets/Resources/Varginha/IllustratedMaps";
            Directory.CreateDirectory(folder);
            string[] names={"Casa Aconchegante Sob o Luar","Casa Aconchegante sob a Luz da Lua","Rua Urbana Noturna em Pixel Art","Mapa Escolar Pixelado ao Amanhecer","Pátio da Biblioteca em Pixel Art","Clareira Florestal e Santuário em Ruínas","Capela Medieval em Luz de Vitral","Casa-Laboratório Botânico Aconchegante","Oficina Vintage Sob a Chuva"};
            string[] ids={"Child1996","Adult2026","Street","School","Library","Forest","Church","Ouzana","Workshop"};
            for(int i=0;i<ids.Length;i++)File.Copy(Path.Combine("C:/Users/Usuario/Downloads",names[i]+".png"),folder+"/"+ids[i]+".png",true);
            File.Copy("C:/Users/Usuario/.codex/generated_images/01a0fec2-8e2e-7ad2-9b35-43f27cddb7b1/exec-301118b4-e29e-45d7-8b5a-b31aeaa77906.png",folder+"/WorkshopEmpty.png",true);
            File.Copy("C:/Users/Usuario/.codex/generated_images/01a0fec2-8e2e-7ad2-9b35-43f27cddb7b1/exec-b4772857-4044-42dc-a211-9e534d4fa1a1.png",folder+"/AdultEmpty.png",true);
            AssetDatabase.Refresh();
            foreach(string id in ids)ConfigureTexture(folder+"/"+id+".png");
            ConfigureTexture(folder+"/WorkshopEmpty.png");
            ConfigureTexture(folder+"/AdultEmpty.png");CampaignIllustratedMaps.Reload();
        }
        public static void ConfigureTexture(string path)
        {
            var importer=(TextureImporter)AssetImporter.GetAtPath(path);
            importer.textureType=TextureImporterType.Default;
            importer.filterMode=FilterMode.Point;importer.mipmapEnabled=false;importer.isReadable=false;
            importer.wrapMode=TextureWrapMode.Clamp;importer.textureCompression=TextureImporterCompression.Uncompressed;
            importer.maxTextureSize=2048;importer.anisoLevel=0;importer.npotScale=TextureImporterNPOTScale.None;
            importer.alphaSource=TextureImporterAlphaSource.None;importer.SaveAndReimport();
        }
        public static void Capture(int phase,bool colliders=false)
        {
            if(Application.isPlaying)throw new System.InvalidOperationException("Use CaptureRuntime during gameplay.");
            var root=new GameObject("IllustratedPreview");
            try
            {
                var plan=CampaignMapPlan.Create(phase);CampaignMapConstruction.Build(root.transform,plan);
                CaptureCamera(root.transform,plan,phase,colliders);
            }
            finally {Object.DestroyImmediate(root);}
        }
        public static void CaptureRuntime(int phase)=>CaptureCamera(null,CampaignMapPlan.Create(phase),phase,false);
        public static void CaptureFocus(int phase,Vector2 center,float width,string name)
            =>CaptureCamera(null,CampaignMapPlan.Create(phase),phase,false,new Rect(center-Vector2.one*width/2,Vector2.one*width),name);
        private static void CaptureCamera(Transform parent,CampaignMapPlan plan,int phase,bool colliders,Rect? focus=null,string name=null)
        {
            var go=new GameObject("IllustratedCaptureCamera");go.transform.SetParent(parent,false);
            var camera=go.AddComponent<Camera>();camera.enabled=false;camera.orthographic=true;camera.allowHDR=camera.allowMSAA=false;
            // The street capture is one complete neighbourhood rather than the 150m course.
            var bounds=plan.bounds;if(phase==3)bounds=new Rect(-12,-8,24,CampaignIllustratedMaps.Get(3).height*CampaignIllustratedMaps.Get(3).Scale);
            if(focus.HasValue)bounds=focus.Value;
            camera.orthographicSize=bounds.height/2;camera.transform.position=new Vector3(bounds.center.x,bounds.center.y,-10);
            int w=phase==3?1672:CampaignIllustratedMaps.Get(phase).width,h=phase==3?941:CampaignIllustratedMaps.Get(phase).height;
            if(focus.HasValue)w=h=640;
            var rt=RenderTexture.GetTemporary(w,h,16,RenderTextureFormat.ARGB32);var previous=RenderTexture.active;
            try
            {
                camera.targetTexture=rt;camera.Render();RenderTexture.active=rt;
                var image=new Texture2D(w,h,TextureFormat.RGBA32,false);image.ReadPixels(new Rect(0,0,w,h),0,0);image.Apply();
                Directory.CreateDirectory("Preview/IllustratedMaps");File.WriteAllBytes("Preview/IllustratedMaps/"+(name??"Fase"+phase+(Application.isPlaying?"_Runtime":"_Integrada"))+".png",image.EncodeToPNG());Object.DestroyImmediate(image);
            }
            finally{camera.targetTexture=null;RenderTexture.active=previous;RenderTexture.ReleaseTemporary(rt);Object.DestroyImmediate(go);}
        }
    }
}
