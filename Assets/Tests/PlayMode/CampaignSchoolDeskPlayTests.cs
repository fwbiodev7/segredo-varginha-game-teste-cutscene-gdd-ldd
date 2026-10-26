using System.Collections;
using System.IO;
using System.Linq;
using Game.Varginha;
using Game.Varginha.Experiment;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace Game.Tests.PlayMode
{
    public sealed class CampaignSchoolDeskPlayTests
    {
        private string _story,_memory;
        [SetUp]public void Preserve()
        {
            _story=Read(CampaignStorySave.Path);_memory=Read(CampaignMemorySave.Path);
        }
        static string Read(string path)=>File.Exists(path)?File.ReadAllText(path):null;
        static void Restore(string path,string data)
        {if(data==null){if(File.Exists(path))File.Delete(path);}else File.WriteAllText(path,data);}
        [UnityTearDown]public IEnumerator Restore()
        {
            Time.timeScale=1;yield return SceneManager.LoadSceneAsync("Menu_MisterioDeVarginha");yield return null;
            Restore(CampaignStorySave.Path,_story);Restore(CampaignMemorySave.Path,_memory);
        }
        static IEnumerator Load(int phase)
        {
            CampaignStorySave.Write(new CampaignStory{phase=phase});
            yield return SceneManager.LoadSceneAsync(CampaignStorySave.Scene(phase));yield return new WaitForSeconds(3.5f);
        }
        static void Focus(Vector2 point,float size)
        {
            var camera=Camera.main;var follow=camera.GetComponent<Game.Level.CameraFollow2D>();if(follow!=null)follow.enabled=false;
            camera.transform.position=new Vector3(point.x,point.y,-10);camera.orthographicSize=size;
        }
        static IEnumerator Capture(string name)
        {
            if(Application.isEditor)yield break;
            yield return new WaitForEndOfFrame();var image=ScreenCapture.CaptureScreenshotAsTexture();
            string folder=Path.Combine(CinemaQaEvidence.Folder,"captures");Directory.CreateDirectory(folder);
            File.WriteAllBytes(Path.Combine(folder,name+".png"),image.EncodeToPNG());Object.Destroy(image);
        }
        static void CaptureWorld(string name)
        {
            var camera=Camera.main;var old=camera.targetTexture;var active=RenderTexture.active;
            var target=RenderTexture.GetTemporary(960,540,24);target.filterMode=FilterMode.Point;
            var image=new Texture2D(960,540,TextureFormat.RGB24,false);
            try
            {
                camera.targetTexture=target;VarginhaPixelPresentation.RenderInto(camera,target);RenderTexture.active=target;
                image.ReadPixels(new Rect(0,0,960,540),0,0);image.Apply();
                string folder=Path.Combine(CinemaQaEvidence.Folder,"world-review");Directory.CreateDirectory(folder);
                File.WriteAllBytes(Path.Combine(folder,name+".png"),image.EncodeToPNG());
            }
            finally{camera.targetTexture=old;RenderTexture.active=active;RenderTexture.ReleaseTemporary(target);Object.Destroy(image);}
        }
        [UnityTest,Timeout(40000)] public IEnumerator GlobeKeepsItsNativeScaleCabinetContactAndInvestigation()
        {
            yield return Load(6);var data=CampaignIllustratedMaps.Get(6);
            var globe=GameObject.Find("Globo_Esferico_Arquivo").GetComponent<SpriteRenderer>();var cabinet=globe.transform.parent;
            Assert.That(globe.sprite.texture.width,Is.EqualTo(44));Assert.That(globe.sprite.texture.height,Is.EqualTo(64));
            Assert.That(globe.sprite.texture.filterMode,Is.EqualTo(FilterMode.Point));Assert.That(globe.sprite.pixelsPerUnit,Is.EqualTo(50));
            Assert.That(globe.transform.lossyScale,Is.EqualTo(Vector3.one));Assert.That(globe.bounds.size.x,Is.EqualTo(.88f).Within(.001f));
            Assert.That(globe.GetComponent<Collider2D>(),Is.Null);Assert.That(cabinet.GetComponent<Collider2D>(),Is.Not.Null);
            Assert.That(globe.GetComponent<SortingGroup>().sortingOrder,Is.GreaterThan(cabinet.GetComponent<SortingGroup>().sortingOrder));
            Focus(data.Position(896,169),1.8f);yield return null;CaptureWorld("globe-library");
            CampaignExpansionController.Active.Interact("school");Assert.That(CampaignExpansionController.Active.Progress.expansion.visited&2,Is.EqualTo(2));
        }
        [UnityTest,Timeout(40000)] public IEnumerator OfficeSeatAlignsWithTheCushionAndReturnsToItsClearApproach()
        {
            yield return Load(2);var stage=VarginhaCampaignStage.Active;var data=CampaignIllustratedMaps.Get(2);
            var floor=GameObject.Find("Mapa_Campanha/01_Planta_Paredes_Divisoes/Arte_Integrada_0").GetComponent<SpriteRenderer>();
            Assert.That(floor.sharedMaterial.GetFloat("_OfficeFloor"),Is.EqualTo(1),"The morning palette must preserve the floor repair.");
            var actor=Object.FindAnyObjectByType<EdelzioTopDownController>();var body=actor.GetComponent<Rigidbody2D>();
            var action=actor.GetComponent<VarginhaPlayerActionAnimation>();var visual=actor.GetComponent<VarginhaPlayerSpriteAnimation>();
            var chair=GameObject.Find("Mapa_Campanha/02_Mobilia_Colisoes/Office_Chair").GetComponent<SpriteRenderer>();
            Vector2 approach=data.Objective("work")+Vector2.up*.58f;body.position=approach;yield return new WaitForFixedUpdate();
            Focus(data.Position(86,487),1.8f);CaptureWorld("office-approach");stage.Interact("work");
            float deadline=Time.realtimeSinceStartup+5;
            while(visual.CurrentActionPose!="Edelzio_UseNotebook"&&Time.realtimeSinceStartup<deadline)yield return null;
            Assert.That(visual.CurrentActionPose,Is.EqualTo("Edelzio_UseNotebook"));yield return null;
            var renderer=actor.GetComponent<SpriteRenderer>();
            Assert.That(actor.transform.position.x,Is.EqualTo(chair.bounds.center.x).Within(.035f),"Torso is centered on the actual chair, not the old hidden anchor.");
            Assert.That(renderer.sprite.name,Does.EndWith("_OfficeOccludedLegs"));
            Assert.That(renderer.bounds.min.y,Is.GreaterThan(chair.bounds.center.y+.15f),"The rear seat hides the lower legs instead of drawing shoes on the cushion.");
            Assert.That(renderer.bounds.min.y,Is.LessThan(chair.bounds.max.y),"The torso overlaps the backrest instead of floating above it.");
            CaptureWorld("office-notebook-seated");
            while(stage.IsActing&&Time.realtimeSinceStartup<deadline)yield return null;
            Assert.That(stage.IsActing,Is.False);Assert.That(stage.Progress.routine&4,Is.EqualTo(4));
            Assert.That(Vector2.Distance(body.position,approach),Is.LessThan(.1f));CaptureWorld("office-return");
        }
        [UnityTest,Timeout(40000)]public IEnumerator ClassroomDesktopStaysSupportedAndNotebookInteractionWorks()
        {
            yield return Load(4);var stage=VarginhaCampaignStage.Active;
            var desk=GameObject.Find("Mapa_Campanha/02_Mobilia_Colisoes/Mesa do professor");
            var collider=desk.GetComponent<Collider2D>();var collisionBefore=collider.bounds;
            var items=stage.transform.Find("Objetos_da_mesa_do_Renan");Assert.That(items.childCount,Is.EqualTo(3));
            var props=items.GetComponentsInChildren<SpriteRenderer>();
            Assert.That(props.Select(p=>p.sprite.texture).Distinct().Count(),Is.EqualTo(1));
            Assert.That(props[0].sprite.texture.width,Is.EqualTo(2400),"No importer downscale may damage the selected crops.");
            foreach(var prop in props)
            {
                Assert.That(prop.sprite.texture.name,Is.EqualTo("RenanDeskProps"));
                Assert.That(prop.bounds.min.y,Is.EqualTo(CampaignIllustratedMaps.Get(4).Position(0,77).y).Within(.002f),prop.name+" must touch the tabletop.");
                Assert.That(prop.GetComponent<Collider2D>(),Is.Null);
                Assert.That(prop.GetComponent<SortingGroup>().sortingOrder,Is.GreaterThan(desk.GetComponent<SortingGroup>().sortingOrder));
            }
            Assert.That(props[0].bounds.max.x,Is.LessThan(props[1].bounds.min.x));
            Assert.That(props[1].bounds.max.x,Is.LessThan(props[2].bounds.min.x));
            CampaignRenanTeacher.DeskItems(stage.transform,CampaignIllustratedMaps.Get(4));Assert.That(items.childCount,Is.EqualTo(3),"No duplicate desk props.");
            Assert.That(collider.bounds,Is.EqualTo(collisionBefore));
            Focus(CampaignIllustratedMaps.Get(4).Position(714,80),2.1f);yield return null;yield return Capture("renan-desktop");
            stage.Progress.renanMet=true;stage.Interact("notebook");
            Assert.That(stage.Progress.photoOpened,Is.True,"The teacher notebook still opens the investigation photograph.");
            yield return Capture("renan-notebook-interaction");
        }
        [UnityTest,Timeout(40000)]public IEnumerator AdultWindowsDoNotCutThePlayerAndStillCoverTheOldNight()
        {
            yield return Load(2);var data=CampaignIllustratedMaps.Get(2);
            var actor=Object.FindAnyObjectByType<EdelzioTopDownController>();var body=actor.GetComponent<Rigidbody2D>();
            var player=actor.GetComponent<SpriteRenderer>();var root=GameObject.Find("Mapa_Campanha").transform;
            foreach(int x in new[]{332,662})
            {
                body.position=data.Position(x,108)+Vector2.up*.58f;yield return new WaitForFixedUpdate();yield return null;
                Focus(data.Position(x,87),1.9f);yield return null;
                var group=player.GetComponent<SortingGroup>();
                foreach(var pane in root.Find("Manha_07h").GetComponentsInChildren<SpriteRenderer>().Where(p=>p.bounds.Intersects(player.bounds)))
                {
                    Assert.That(group.sortingOrder,Is.GreaterThan(pane.GetComponent<SortingGroup>().sortingOrder),"Foreground player must remain whole: "+pane.name);
                    var depth=pane.GetComponent<VarginhaWorldDepth>();Assert.That(depth,Is.Not.Null);
                }
                yield return Capture("window-player-"+x);
            }
            Assert.That(VarginhaCampaignStage.Active.Progress.routine,Is.Zero);
        }
    }
}
