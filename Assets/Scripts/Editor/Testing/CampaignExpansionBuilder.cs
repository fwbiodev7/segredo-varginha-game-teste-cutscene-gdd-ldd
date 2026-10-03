using System.IO;
using System.Linq;
using Game.Varginha;
using Game.Varginha.Experiment;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Game.Editor.Testing
{
    public static class CampaignExpansionBuilder
    {
        private static bool _architectureOnly;
        public static void ExportArchitecture()
        {
            _architectureOnly=true;
            try { ExportPreviews(); } finally { _architectureOnly=false; }
        }
        [MenuItem("Varginha/Campanha/Reconstruir mapas V2 (preservar casa adulta)")]
        public static void Build()
        {
            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
            foreach (string path in Directory.GetFiles("Assets/Resources/Varginha/TeamArt").Concat(Directory.GetFiles("Assets/Resources/Varginha/WorldArtV2")).Concat(Directory.GetFiles("Assets/Resources/Varginha/WorldArtV3")).Where(p => p.EndsWith(".png") || p.EndsWith(".jpeg")))
            {
                var importer = (TextureImporter)AssetImporter.GetAtPath(path); importer.textureType = TextureImporterType.Default;
                importer.isReadable = true; importer.filterMode = FilterMode.Point; importer.mipmapEnabled = false;
                importer.textureCompression = TextureImporterCompression.Uncompressed; importer.alphaIsTransparency = true;
                importer.npotScale = TextureImporterNPOTScale.None; importer.maxTextureSize = 2048; importer.wrapMode = TextureWrapMode.Clamp;
                importer.SaveAndReimport();
            }
            Debug.Log("CAMPAIGN_PIPELINE=" + VarginhaPixelPresentation.DetectPipeline());
            var scenes = EditorBuildSettings.scenes.ToList();
            for (int phase = 1; phase <= 10; phase++)
            {
                if (phase == 2) continue;
                var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
                var root = new GameObject("Campanha_Fase_" + phase);
                if (phase == 1) root.AddComponent<VarginhaCampaignPhase1>();
                else if(phase==3)root.AddComponent<VarginhaCampaignDrive>();
                else if(phase==4||phase==5)root.AddComponent<VarginhaCampaignStage>().phase=phase;
                else root.AddComponent<CampaignExpansionController>().phase = phase;
                string path = "Assets/Scenes/" + CampaignStorySave.Scene(phase) + ".unity";
                EditorSceneManager.SaveScene(scene,path);
                var entry = scenes.FirstOrDefault(s => s.path == path);
                if (entry == null) scenes.Add(new EditorBuildSettingsScene(path,true)); else entry.enabled = true;
            }
            EditorBuildSettings.scenes = scenes.ToArray(); AssetDatabase.SaveAssets();
            ExportPreviews();
            EditorSceneManager.OpenScene("Assets/Scenes/Menu_MisterioDeVarginha.unity");
            Debug.Log("CAMPAIGN_EXPANSION_READY=6-10");
        }
        [MenuItem("Varginha/Campanha/Exportar plantas e mobília")]
        public static void ExportPreviews()=>ExportPreviewsForPhase(0);
        public static void ExportPreviewsForPhase(int phaseOnly)
        {
            Directory.CreateDirectory("Preview/CampaignMapsV2");
            foreach (int phase in new[] { 1,3,4,5,6,7,8,9,10 })
            {
                if(phaseOnly!=0&&phaseOnly!=phase)continue;
                var plan = CampaignMapPlan.Create(phase);
                var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Single);
                var root = new GameObject("Preview_" + phase); var map = CampaignMapConstruction.Build(root.transform,plan,false);
                var camera = new GameObject("Camera").AddComponent<Camera>(); camera.orthographic = true;
                camera.transform.position = new Vector3(plan.bounds.center.x,plan.bounds.center.y,-10);
                camera.orthographicSize = plan.bounds.height/2+.7f; camera.aspect = plan.bounds.width/plan.bounds.height;
                camera.backgroundColor = new Color(.025f,.035f,.05f); VarginhaPixelPresentation.Configure(camera);
                if(phase==3){camera.transform.position=new Vector3(0,3,-10);camera.orthographicSize=9;camera.aspect=4f/3;}
                Capture(camera,"Preview/CampaignMapsV2/Fase"+phase+"_01_Planta.png",phase == 1 ? 1440 : 1200,800);
                if(_architectureOnly){File.WriteAllText("Preview/CampaignMapsV2/Fase"+phase+"_Planta.svg",Svg(plan,false));continue;}
                CampaignMapConstruction.Furnish(map,plan);
                if(phase==3)
                {
                    var source=Resources.Load<Texture2D>("Varginha/Experiment/FuscaTopView");
                    var car=new GameObject("Fusca_Original_Preview");car.transform.position=new Vector3(-2,3);
                    var sprite=car.AddComponent<SpriteRenderer>();int w=source.width/2,h=source.height/2;
                    sprite.sprite=Sprite.Create(source,new Rect(0,h,w,h),Vector2.one/2,h/3.4f,0,SpriteMeshType.FullRect);sprite.sortingOrder=15000;
                }
                Capture(camera,"Preview/CampaignMapsV2/Fase"+phase+"_02_Mobilia.png",phase == 1 ? 1440 : 1200,800);
                if(phase==4||phase==5)
                {
                    var facade=CampaignMapConstruction.PreserveSchoolFacade(root.transform);
                    Capture(camera,"Preview/CampaignMapsV2/Fase"+phase+"_04_Fachada_Original.png",1200,800);
                    facade.ShowInterior(true);
                    camera.transform.position=new Vector3(.5f,0,-10);camera.orthographicSize=6.5f;
                    Capture(camera,"Preview/CampaignMapsV2/Fase"+phase+"_05_Laboratorio_Remaster.png",1200,800);
                    camera.transform.position=new Vector3(plan.bounds.center.x,plan.bounds.center.y,-10);camera.orthographicSize=plan.bounds.height/2+.7f;
                }
                foreach (var point in plan.points)
                {
                    var label = new GameObject(point.label); label.transform.position = new Vector3(point.position.x,point.position.y,0);
                    var text = label.AddComponent<TextMesh>(); text.text = point.id; text.fontSize = 32; text.characterSize = .11f;
                    text.anchor = TextAnchor.MiddleCenter; text.color = new Color(.8f,1,.9f); text.GetComponent<MeshRenderer>().sortingOrder = 22500;
                }
                Capture(camera,"Preview/CampaignMapsV2/Fase"+phase+"_03_Objetivos.png",phase == 1 ? 1440 : 1200,800);
                File.WriteAllText("Preview/CampaignMapsV2/Fase"+phase+"_Planta.svg",Svg(plan,false));
                File.WriteAllText("Preview/CampaignMapsV2/Fase"+phase+"_Colisoes.svg",Svg(plan,true));
            }
            if(_architectureOnly||phaseOnly!=0)return;
            ExportActor();
            ExportActionActors("EdelzioPunch",3);
            ExportActionActors("EdelzioActions",6);
            ExportActionActors("EdelzioInteractions",6);
        }
        private static void ExportActionActors(string sheet,int columns)
        {
            EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Single);
            for(int row=0;row<4;row++)for(int column=0;column<columns;column++)
            {
                var actor=new GameObject(sheet+row+":"+column);actor.transform.position=new Vector3((column-(columns-1)*.5f)*2.3f,4.5f-row*2.5f);
                actor.AddComponent<SpriteRenderer>().sprite=CampaignTeamEdelzio.Frame(sheet,row,column);
            }
            var camera=new GameObject("Camera").AddComponent<Camera>();camera.orthographic=true;camera.orthographicSize=5.5f;
            camera.transform.position=new Vector3(0,.8f,-10);camera.backgroundColor=new Color(.12f,.18f,.19f);
            VarginhaPixelPresentation.Configure(camera);Capture(camera,"Preview/CampaignMapsV2/"+sheet+".png",columns*230,1000);
        }
        private static string Svg(CampaignMapPlan p,bool furniture)
        {
            var b = new System.Text.StringBuilder();
            string F(float v) => v.ToString("0.###",System.Globalization.CultureInfo.InvariantCulture);
            b.Append("<svg xmlns='http://www.w3.org/2000/svg' viewBox='0 0 "+F(p.bounds.width*40)+" "+F(p.bounds.height*40)+"'><rect width='100%' height='100%' fill='#111b1f'/>");
            void Rect(Rect r,string fill)
                => b.Append("<rect x='"+F((r.x-p.bounds.x)*40)+"' y='"+F((p.bounds.yMax-r.yMax)*40)+"' width='"+F(r.width*40)+"' height='"+F(r.height*40)+"' fill='"+fill+"'/>");
            foreach (var r in p.rooms) Rect(r.rect,"#34453d");
            foreach (var r in p.walls) Rect(r,"#d6c498");
            if (furniture) foreach(var f in p.furniture) { Rect(new Rect(f.position-f.size/2,f.size),"#815f43"); Rect(f.footprint,"#af5f59"); }
            foreach(var point in p.points) b.Append("<circle cx='"+F((point.position.x-p.bounds.x)*40)+"' cy='"+F((p.bounds.yMax-point.position.y)*40)+"' r='5' fill='#91efcf'/><text x='"+F((point.position.x-p.bounds.x)*40+7)+"' y='"+F((p.bounds.yMax-point.position.y)*40)+"' fill='white' font-size='10'>"+point.id+"</text>");
            b.Append("</svg>"); return b.ToString();
        }
        private static void ExportActor()
        {
            EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Single);
            for(int row=0;row<4;row++) for(int col=0;col<4;col++)
            {
                var go=new GameObject("Arte_manual"); go.transform.position=new Vector3(col*2-3,4.5f-row*2.5f);
                go.AddComponent<SpriteRenderer>().sprite=CampaignTeamEdelzio.Frame("EdelzioWalk",row,col);
            }
            var camera = new GameObject("Camera").AddComponent<Camera>(); camera.orthographic=true; camera.orthographicSize=5.5f;
            camera.transform.position=new Vector3(0,.8f,-10); camera.backgroundColor=new Color(.12f,.18f,.19f);
            VarginhaPixelPresentation.Configure(camera); Capture(camera,"Preview/CampaignMapsV2/Edelzio_Equipe.png",800,1000);
        }
        private static void Capture(Camera camera,string path,int width,int height)
        {
            var target=new RenderTexture(width,height,24); camera.targetTexture=target; camera.Render();
            var previous=RenderTexture.active; RenderTexture.active=target;
            var image=new Texture2D(width,height,TextureFormat.RGB24,false); image.ReadPixels(new Rect(0,0,width,height),0,0); image.Apply();
            File.WriteAllBytes(path,image.EncodeToPNG()); RenderTexture.active=previous; camera.targetTexture=null;
            Object.DestroyImmediate(image); Object.DestroyImmediate(target);
        }
    }
}
