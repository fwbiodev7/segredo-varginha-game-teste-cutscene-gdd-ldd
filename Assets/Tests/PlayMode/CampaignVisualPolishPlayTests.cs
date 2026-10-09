using System.Collections;
using System.IO;
using System.Linq;
using Game.Varginha;
using Game.Varginha.Experiment;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
#if UNITY_EDITOR
using UnityEditor.SceneManagement;
#endif

namespace Game.Tests.PlayMode
{
    public class CampaignVisualPolishPlayTests
    {
        private string _save, _memory;
        private const string Output="Docs/QAAlpha01";
        [SetUp]public void Setup(){_save=File.Exists(CampaignStorySave.Path)?File.ReadAllText(CampaignStorySave.Path):null;_memory=File.Exists(CampaignMemorySave.Path)?File.ReadAllText(CampaignMemorySave.Path):null;}
        [TearDown]public void TearDown(){Time.timeScale=1;if(_save!=null)File.WriteAllText(CampaignStorySave.Path,_save);else if(File.Exists(CampaignStorySave.Path))File.Delete(CampaignStorySave.Path);if(_memory!=null)File.WriteAllText(CampaignMemorySave.Path,_memory);else if(File.Exists(CampaignMemorySave.Path))File.Delete(CampaignMemorySave.Path);}

        [UnityTest]
        public IEnumerator VitraisColourSpritesOnGpuAndFadeAtTheEdge()
        {
            CampaignStorySave.Write(new CampaignStory{phase=8});
            yield return SceneManager.LoadSceneAsync(CampaignStorySave.Scene(8));yield return new WaitForSeconds(3.3f);
            var actor=Object.FindAnyObjectByType<EdelzioTopDownController>();actor.SetInputLocked(true);
            var data=CampaignIllustratedMaps.Get(8);var glass=GameObject.Find("03_Iluminacao_2D_Vitrais");
            Assert.That(glass.GetComponentsInChildren<Light2D>().Length,Is.EqualTo(10));
            Assert.That(actor.GetComponent<SpriteRenderer>().sharedMaterial.shader.name,Is.EqualTo("Varginha/StainedGlassLighting"));
            var probe=new GameObject("Vitral_QA_Probe");var renderer=probe.AddComponent<SpriteRenderer>();
            var texture=new Texture2D(2,2);texture.SetPixels(Enumerable.Repeat(Color.white,4).ToArray());texture.Apply();
            renderer.sprite=Sprite.Create(texture,new Rect(0,0,2,2),Vector2.one/2,20);renderer.sharedMaterial=CampaignStainedGlassLighting.ActorMaterial;renderer.sortingOrder=25000;
            var colors=new Color[3];
            string[] names={"Azul","Ambar","Violeta"};
            for(int i=0;i<3;i++)
            {
                var source=data.stainedGlass.Single(l=>l.name.StartsWith("Vitral_"+i+"_"));
                float x=0,y=0;for(int p=0;p<source.outline.Length;p+=2){x+=source.outline[p];y+=source.outline[p+1];}
                probe.transform.position=data.Position(x/(source.outline.Length/2),y/(source.outline.Length/2));
                var body=actor.GetComponent<Rigidbody2D>();var feet=actor.GetComponent<CircleCollider2D>();
                body.position=(Vector2)probe.transform.position-feet.offset;yield return new WaitForFixedUpdate();yield return null;
                colors[i]=SampleProbe(renderer);
                probe.SetActive(false);CaptureFocus(actor.GetComponent<SpriteRenderer>(),"Igreja_Vitral_"+names[i]+".png");probe.SetActive(true);
            }
            File.WriteAllText(Output+"/Vitral_GPU.txt",string.Join("\n",colors.Select(c=>c.ToString())));
            Assert.That(colors[0].b-colors[0].r,Is.GreaterThan(.10f),"Blue reaches the sprite GPU material");
            Assert.That(colors[1].r-colors[1].b,Is.GreaterThan(.10f),"Amber reaches the sprite GPU material");
            Assert.That(colors[2].b-colors[2].g,Is.GreaterThan(.10f),"Violet reaches the sprite GPU material");
            foreach(var light in glass.GetComponentsInChildren<Light2D>())
                if(light.lightType==Light2D.LightType.Freeform)light.enabled=light.name.StartsWith("Vitral_0_");
            float[] pixels={172f,140f,130f,110f};var edge=new Color[pixels.Length];
            for(int i=0;i<pixels.Length;i++)
            {
                probe.transform.position=data.Position(pixels[i],453);yield return null;
                edge[i]=SampleProbe(renderer);
            }
            File.WriteAllText(Output+"/Vitral_Edge_GPU.txt",string.Join("\n",edge.Select(c=>c.ToString())));
            Assert.That(edge[0].b-edge[0].r,Is.GreaterThan(edge[3].b-edge[3].r+.12f));
            Assert.That(edge[1].b-edge[1].r,Is.GreaterThan(edge[2].b-edge[2].r),"Falloff changes gradually across the edge");
            Object.Destroy(probe);Object.Destroy(texture);
            yield return SceneManager.LoadSceneAsync("Menu_MisterioDeVarginha");
        }

        [UnityTest]
        public IEnumerator EveryAuthoredCampaignSceneLoadsWithClippedFurnitureAndMatchingColliders()
        {
            int[] phases={1,2,3,4,5,6,7,8,9,10,11,12,112,13,14,15,16,18,19,20,21,121};
            CampaignMemorySave.Write(new CampaignMemory{openingSeen=true});
            foreach(int phase in phases)
            {
                var story=new CampaignStory{phase=phase==112?12:phase==121?21:phase};story.expansion.workshopParked=true;CampaignStorySave.Write(story);
                string scene=phase==112?CampaignContinuationDefinition.SceneName(12,1):phase==121?CampaignContinuationDefinition.SceneName(21,1):CampaignStorySave.Scene(phase);
#if UNITY_EDITOR
                yield return EditorSceneManager.LoadSceneAsyncInPlayMode("Assets/Scenes/"+scene+".unity",new LoadSceneParameters(LoadSceneMode.Single));
#else
                yield return SceneManager.LoadSceneAsync(scene);
#endif
                yield return new WaitForSeconds(3.3f);
                var map=GameObject.Find("Mapa_Campanha");Assert.That(map,Is.Not.Null,scene);
                var data=CampaignIllustratedMaps.Get(phase);
                foreach(var prop in data.props)
                {
                    var transform=map.transform.Find("02_Mobilia_Colisoes/"+prop.name);Assert.That(transform,Is.Not.Null,scene+" "+prop.name);
                    var renderer=transform.GetComponent<SpriteRenderer>();
                    Assert.That(renderer.sprite.texture.filterMode,Is.EqualTo(FilterMode.Point),scene+" "+prop.name+" "+renderer.sprite.texture.name);
                    if(prop.name!="Fusca")Assert.That(renderer.sprite.vertices.Length,Is.EqualTo(prop.pieces?.Length>0?prop.pieces.Sum(p=>p.outline.Length/2):prop.outline.Length/2),scene+" "+prop.name+" foreground mesh follows its authored silhouette");
                    bool solid=prop.@base?.Length==4&&prop.@base[2]>0&&prop.@base[3]>0;
                    var collider=transform.GetComponent<Collider2D>();
                    Assert.That(collider!=null,Is.EqualTo(solid),scene+" "+prop.name+" floor contact");
                    if(solid && prop.collision?.Length>=6)
                    {
                        Assert.That(collider,Is.TypeOf<PolygonCollider2D>(),scene+" "+prop.name+" authored ground polygon");
                        var path=((PolygonCollider2D)collider).GetPath(0);
                        Assert.That(path.Length,Is.EqualTo(prop.collision.Length/2));
                        for(int i=0;i<path.Length;i++)Assert.That(Vector2.Distance(transform.TransformPoint(path[i]),data.Position(prop.collision[i*2],prop.collision[i*2+1])),Is.LessThan(.001f));
                    }
                }
                if(Camera.main!=null)Capture(Camera.main,"Fase_"+phase+".png");
            }
            yield return SceneManager.LoadSceneAsync("Menu_MisterioDeVarginha");
        }
        private static Color SampleProbe(SpriteRenderer probe)
        {
            var go=new GameObject("Vitral QA Camera");var camera=go.AddComponent<Camera>();camera.enabled=false;camera.orthographic=true;camera.orthographicSize=.3f;go.transform.position=probe.transform.position+Vector3.back*10;
            var target=RenderTexture.GetTemporary(64,64,24);var previous=RenderTexture.active;var image=new Texture2D(64,64,TextureFormat.RGB24,false);
            try { RenderPipeline.SubmitRenderRequest(camera,new UniversalRenderPipeline.SingleCameraRequest{destination=target});RenderTexture.active=target;image.ReadPixels(new Rect(0,0,64,64),0,0);image.Apply();return image.GetPixel(32,32); }
            finally {RenderTexture.active=previous;RenderTexture.ReleaseTemporary(target);Object.Destroy(image);Object.Destroy(go);}
        }
        public static void Capture(Camera camera,string name)
        {
            Directory.CreateDirectory(Output);var target=RenderTexture.GetTemporary(1280,960,24);var previous=RenderTexture.active;var image=new Texture2D(1280,960,TextureFormat.RGB24,false);
            try {RenderPipeline.SubmitRenderRequest(camera,new UniversalRenderPipeline.SingleCameraRequest{destination=target});RenderTexture.active=target;image.ReadPixels(new Rect(0,0,1280,960),0,0);image.Apply();File.WriteAllBytes(Output+"/"+name,image.EncodeToPNG());}
            finally {RenderTexture.active=previous;RenderTexture.ReleaseTemporary(target);Object.Destroy(image);}
        }
        private static void CaptureFocus(SpriteRenderer actor,string name)
        {
            var go=new GameObject("Vitral Focus Camera");var camera=go.AddComponent<Camera>();camera.enabled=false;camera.orthographic=true;camera.orthographicSize=2.1f;go.transform.position=actor.bounds.center+Vector3.back*10;
            try{Capture(camera,name);}finally{Object.Destroy(go);}
        }
    }
    public class CampaignVisualPolishMovementTests : InputTestFixture
    {
        private string _save;
        public override void Setup(){VarginhaInputActions.Shutdown();base.Setup();_save=File.Exists(CampaignStorySave.Path)?File.ReadAllText(CampaignStorySave.Path):null;}
        public override void TearDown(){Time.timeScale=1;VarginhaInputActions.Shutdown();if(_save!=null)File.WriteAllText(CampaignStorySave.Path,_save);else if(File.Exists(CampaignStorySave.Path))File.Delete(CampaignStorySave.Path);base.TearDown();}
        [UnityTest]
        public IEnumerator FragmentosAllowsRealMovementBelowColumnAndBlocksTheSolidLadder()
        {
            CampaignStorySave.Write(new CampaignStory{phase=6});
            yield return SceneManager.LoadSceneAsync(CampaignStorySave.Scene(6));yield return new WaitForSeconds(3.3f);
            var actor=Object.FindAnyObjectByType<EdelzioTopDownController>();var body=actor.GetComponent<Rigidbody2D>();var feet=actor.GetComponent<CircleCollider2D>();
            var data=CampaignIllustratedMaps.Get(6);
            body.position=data.Position(295,270)-feet.offset;body.linearVelocity=Vector2.zero;yield return new WaitForFixedUpdate();yield return null;
            var keyboard=InputSystem.AddDevice<Keyboard>();
            Press(keyboard.dKey);yield return new WaitForSeconds(.7f);Release(keyboard.dKey);yield return new WaitForFixedUpdate();
            Assert.That(feet.bounds.center.x,Is.GreaterThan(data.Position(410,270).x),"No invisible body barrier below the column");
            CampaignVisualPolishPlayTests.Capture(Camera.main,"Fragmentos_Circulacao.png");
            body.position=data.Position(415,281)-feet.offset;body.linearVelocity=Vector2.zero;yield return new WaitForFixedUpdate();yield return null;
            Press(keyboard.wKey);yield return new WaitForSeconds(.65f);Release(keyboard.wKey);yield return new WaitForFixedUpdate();
            var ladder=GameObject.Find("Escada da estante").GetComponent<BoxCollider2D>();
            Assert.That(feet.Distance(ladder).distance,Is.GreaterThanOrEqualTo(-.012f));
            CampaignVisualPolishPlayTests.Capture(Camera.main,"Fragmentos_Escada.png");
            body.position=data.Position(506,436)-feet.offset;yield return new WaitForFixedUpdate();yield return null;
            CampaignVisualPolishPlayTests.Capture(Camera.main,"Fragmentos_Mesa_Planta.png");
            var table=GameObject.Find("Mesa central de fragmentos").GetComponent<SortingGroup>();var plant=GameObject.Find("Planta sobre a mesa central").GetComponent<SortingGroup>();
            Assert.That(plant.sortingOrder,Is.EqualTo(table.sortingOrder+1));
            yield return SceneManager.LoadSceneAsync("Menu_MisterioDeVarginha");
        }
    }
}
