using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Game.Varginha;
using Game.Varginha.Experiment;
using UnityEditor.SceneManagement;
using UnityEngine;
using Object=UnityEngine.Object;

namespace Game.Editor.Testing
{
    // Production sprites exported at one canvas scale for manual placement.
    // This never saves or changes an authored scene.
    public static class CampaignManualLayoutExporter
    {
        private const int Ppu=48;
        private const string Root="Entregas/Mapas_Para_Organizar";
        [Serializable]private sealed class Item
        {
            public string name,file;
            public float worldX,worldY,worldWidth,worldHeight;
            public int left,top,width,height;
        }
        [Serializable]private sealed class Layout
        {
            public int phase,pixelsPerWorldUnit=Ppu,width,height;
            public string title,coordinateOrigin="top-left";
            public float worldLeft,worldBottom;
            public List<Item> objects=new();
        }
        private static Layout _layout;
        private static Rect _canvas;
        private static string _directory;
        private static List<SpriteRenderer> _objects;
        private static readonly Dictionary<string,string> Files=new();
        public static int BeginPhase(int phase)
        {
            if(Application.isPlaying)throw new InvalidOperationException("Export procedural maps outside Play Mode.");
            EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Single);
            var plan=CampaignMapPlan.Create(phase);
            SetCanvas(phase,plan.title,plan.bounds);
            var owner=new GameObject("ManualLayoutPreview");
            var map=CampaignMapConstruction.Build(owner.transform,plan,false);
            CaptureCanvas(_directory+"/01_Planta.png");
            CampaignMapConstruction.Furnish(map,plan);
            _objects=map.Find("02_Mobilia_Colisoes").Cast<Transform>().Select(t=>t.GetComponent<SpriteRenderer>()).Where(r=>r!=null).ToList();
            if(phase==3)
            {
                var texture=Resources.Load<Texture2D>("Varginha/Experiment/FuscaTopView");
                int w=texture.width/2,h=texture.height/2;
                var car=new GameObject("Fusca original").AddComponent<SpriteRenderer>();
                car.sprite=Sprite.Create(texture,new Rect(0,h,w,h),Vector2.one/2,h/3.4f,0,SpriteMeshType.FullRect);
                car.transform.position=new Vector3(-2,3);car.sortingOrder=15000;_objects.Add(car);
            }
            CaptureCanvas(_directory+"/02_Referencia_Mobiliada.png");
            if(phase==4||phase==5)
            {
                var facade=CampaignMapConstruction.PreserveSchoolFacade(owner.transform).GetComponent<SpriteRenderer>();
                _objects.Add(facade);
                CaptureCanvas(_directory+"/03_Fachada_Original.png");
            }
            return _objects.Count;
        }
        public static int BeginAdultHouse()
        {
            if(!Application.isPlaying)throw new InvalidOperationException("Export adult house from the running authored scene.");
            SetCanvas(2,"CASA DE EDELZIO ADULTO",new Rect(-9,-8,36,16));
            var renderers=Object.FindObjectsByType<SpriteRenderer>(FindObjectsInactive.Exclude).Where(r=>r.enabled).ToArray();
            var actors=renderers.Where(r=>r.GetComponentInParent<EdelzioTopDownController>()!=null).ToArray();
            foreach(var renderer in actors)renderer.enabled=false;
            _objects=renderers.Where(r=>!IsArchitecture(r)&&!IsEffect(r)&&!actors.Contains(r)).ToList();
            var hidden=renderers.Where(r=>!IsArchitecture(r)&&!actors.Contains(r)).ToArray();
            try
            {
                foreach(var renderer in hidden)renderer.enabled=false;
                CaptureCanvas(_directory+"/01_Planta.png");
                foreach(var renderer in hidden)renderer.enabled=true;
                CaptureCanvas(_directory+"/02_Referencia_Mobiliada.png");
            }
            finally
            {
                foreach(var renderer in hidden)renderer.enabled=true;
                foreach(var renderer in actors)renderer.enabled=true;
            }
            return _objects.Count;
        }
        private static bool IsArchitecture(SpriteRenderer renderer)
        {
            for(var t=renderer.transform;t!=null;t=t.parent)
                if(t.name.StartsWith("Floor_")||t.name.StartsWith("Wall_")||t.name.StartsWith("Driveway_")||t.name.StartsWith("Street_Road_")||t.name.StartsWith("Fence_")||t.name=="Porch_Wood"||t.name=="Juncoes_Paredes"||t.name=="Portais_Abertos")return true;
            return false;
        }
        private static bool IsEffect(SpriteRenderer renderer)
        {
            for(var t=renderer.transform;t!=null;t=t.parent)
                if(t.name==VarginhaSoftLighting.LayerName||t.name.Contains("Poeira")||t.name.Contains("Sombra")||t.name.Contains("Shadow")||t.name.Contains("Iluminacao")||t.name.Contains("Lighting")||t.name.Contains("Particles")||t.name.Contains("PixelSoftLighting"))return true;
            return false;
        }
        private static void SetCanvas(int phase,string title,Rect bounds)
        {
            int width=Mathf.CeilToInt(bounds.width*Ppu),height=Mathf.CeilToInt(bounds.height*Ppu);
            _canvas=new Rect(bounds.min,new Vector2(width/(float)Ppu,height/(float)Ppu));
            _layout=new Layout{phase=phase,title=title,width=width,height=height,worldLeft=_canvas.xMin,worldBottom=_canvas.yMin};
            _directory=Root+"/Fase_"+phase.ToString("00");
            Directory.CreateDirectory(_directory+"/Moveis_PNG");Files.Clear();
        }
        public static string ExportFurnitureBatch(int first,int count)
        {
            int end=Mathf.Min(first+count,_objects.Count);
            for(int index=first;index<end;index++)
            {
                var renderer=_objects[index];if(renderer==null||renderer.sprite==null)continue;
                var bounds=new Rect((Vector2)renderer.bounds.min,renderer.bounds.size);
                bounds=new Rect(bounds.min-Vector2.one/Ppu,bounds.size+Vector2.one*2/Ppu);
                int width=Mathf.CeilToInt(bounds.width*Ppu),height=Mathf.CeilToInt(bounds.height*Ppu);
                bounds.size=new Vector2(width/(float)Ppu,height/(float)Ppu);
                string key=renderer.sprite.texture.GetEntityId()+":"+renderer.sprite.rect+":"+renderer.drawMode+":"+renderer.bounds.size+":"+renderer.sharedMaterial.name+":"+renderer.color;
                if(!Files.TryGetValue(key,out var file))
                {
                    file="Moveis_PNG/"+(Files.Count+1).ToString("000")+"_"+Regex.Replace(renderer.name,@"[^A-Za-z0-9_-]+","_")+".png";
                    RenderObject(renderer,bounds,_directory+"/"+file,width,height);Files[key]=file;
                }
                _layout.objects.Add(new Item{name=renderer.name,file=file,worldX=renderer.transform.position.x,worldY=renderer.transform.position.y,
                    worldWidth=renderer.bounds.size.x,worldHeight=renderer.bounds.size.y,left=Mathf.RoundToInt((bounds.xMin-_canvas.xMin)*Ppu),
                    top=Mathf.RoundToInt((_canvas.yMax-bounds.yMax)*Ppu),width=width,height=height});
            }
            File.WriteAllText(_directory+"/Posicoes_Atuais.json",JsonUtility.ToJson(_layout,true));
            return end+"/"+_objects.Count+"; PNGs únicos: "+Files.Count;
        }
        private static void RenderObject(SpriteRenderer source,Rect bounds,string path,int width,int height)
        {
            var proxy=new GameObject("ExportSprite");proxy.layer=31;
            try
            {
                proxy.transform.position=source.transform.position;proxy.transform.rotation=source.transform.rotation;proxy.transform.localScale=source.transform.lossyScale;
                var renderer=proxy.AddComponent<SpriteRenderer>();renderer.sprite=source.sprite;renderer.sharedMaterial=source.sharedMaterial;
                renderer.color=source.color;renderer.drawMode=source.drawMode;renderer.size=source.size;renderer.flipX=source.flipX;renderer.flipY=source.flipY;
                Capture(bounds,path,width,height,Color.clear,1<<31);
            }
            finally{Object.DestroyImmediate(proxy);}
        }
        private static void CaptureCanvas(string path)=>Capture(_canvas,path,_layout.width,_layout.height,new Color(.025f,.035f,.05f,1),~(1<<31));
        private static void Capture(Rect bounds,string path,int width,int height,Color background,int mask)
        {
            var go=new GameObject("ManualExportCamera");var previous=RenderTexture.active;RenderTexture target=null;Texture2D image=null;
            try
            {
                var camera=go.AddComponent<Camera>();camera.enabled=false;camera.orthographic=true;camera.orthographicSize=bounds.height/2;camera.aspect=width/(float)height;
                camera.transform.position=new Vector3(bounds.center.x,bounds.center.y,-10);camera.backgroundColor=background;camera.clearFlags=CameraClearFlags.SolidColor;
                camera.cullingMask=mask;camera.allowHDR=camera.allowMSAA=false;
                target=new RenderTexture(width,height,24,RenderTextureFormat.ARGB32);camera.targetTexture=target;camera.Render();RenderTexture.active=target;
                image=new Texture2D(width,height,TextureFormat.RGBA32,false){filterMode=FilterMode.Point};image.ReadPixels(new Rect(0,0,width,height),0,0);image.Apply();
                File.WriteAllBytes(path,image.EncodeToPNG());
            }
            finally
            {
                RenderTexture.active=previous;if(image!=null)Object.DestroyImmediate(image);
                if(target!=null){target.Release();Object.DestroyImmediate(target);}Object.DestroyImmediate(go);
            }
        }
    }
}
