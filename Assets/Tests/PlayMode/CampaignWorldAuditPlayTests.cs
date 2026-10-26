using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Game.Varginha;
using Game.Varginha.Experiment;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace Game.Tests.PlayMode
{
    public sealed class CampaignWorldAuditPlayTests
    {
        string saved,memory;
        [SetUp]public void Preserve(){saved=Read(CampaignStorySave.Path);memory=Read(CampaignMemorySave.Path);}
        static string Read(string path)=>File.Exists(path)?File.ReadAllText(path):null;
        static void Write(string path,string value){if(value==null){if(File.Exists(path))File.Delete(path);}else File.WriteAllText(path,value);}
        [UnityTearDown]public IEnumerator Restore()
        {
            Time.timeScale=1;yield return SceneManager.LoadSceneAsync("Menu_MisterioDeVarginha");Write(CampaignStorySave.Path,saved);Write(CampaignMemorySave.Path,memory);
        }
        [UnityTest,Timeout(600000)]public IEnumerator AllGameplayAreasAuditHumanProportionsAndRealFurnitureContacts()
        {
            var entries=CampaignSequence.Entries.Select(p=>(phase:p,area:0)).ToList();entries.Add((10,0));entries.Add((12,1));entries.Add((13,0));entries.Add((21,1));
            var contacts=new List<string>{"phase,area,prop,accessible_sides,contacts"};
            var people=new List<string>{"phase,area,actor,texture,height,width,ppu"};int checks=0,props=0,unreachable=0;
            foreach(var entry in entries)
            {
                var story=new CampaignStory{phase=entry.phase};story.continuation.area=entry.area;story.expansion.workshopParked=true;
                CampaignStorySave.Write(story);CampaignMemorySave.Write(new CampaignMemory{openingSeen=true});
                yield return SceneManager.LoadSceneAsync(CampaignStorySave.Scene(entry.phase));
                if(entry.phase==12&&entry.area==1||entry.phase==21&&entry.area==1)
                    yield return SceneManager.LoadSceneAsync(CampaignContinuationDefinition.SceneName(entry.phase,entry.area));
                yield return new WaitForSeconds(3.5f);
                if(entry.phase==1)VarginhaCampaignPhase1.Active.DeliverControl();
                VarginhaCampaignStage.Active?.CloseDialogue();CampaignExpansionController.Active?.ClosePanel();CampaignContinuationController.Active?.CloseMessage();
                var actor=Object.FindAnyObjectByType<EdelzioTopDownController>();actor.SetInputLocked(true);
                foreach(var threat in Object.FindObjectsByType<CampaignManifestationCombat>())threat.enabled=false;
                var map=GameObject.Find("Mapa_Campanha").transform;var furnishings=map.Find("02_Mobilia_Colisoes");
                foreach(var renderer in Object.FindObjectsByType<SpriteRenderer>().Where(r=>r.sprite!=null&&r.enabled&&r.GetComponentInParent<Canvas>()==null))
                {
                    bool adult=renderer.GetComponent<EdelzioTopDownController>()?.GetComponent<CampaignTeamEdelzio>()!=null||renderer.name=="Ouzana"||renderer.name.StartsWith("Renan_")||renderer.name=="Renan"||renderer.name=="Padre Fábio"&&(entry.phase==14||entry.phase==16);
                    bool pupil=renderer.GetComponent<CampaignSchoolLife>()!=null||renderer.GetComponent<VarginhaStudentAnimation>()!=null;
                    if(!adult&&!pupil)continue;
                    var b=renderer.bounds;float visibleHeight=b.size.y,visibleWidth=b.size.x;
                    if(VarginhaCharacterFrameGeometry.TryBounds(renderer.sprite.texture,renderer.sprite.rect,out var visible,out _))
                    {
                        visibleHeight=visible.height/renderer.sprite.pixelsPerUnit*renderer.transform.lossyScale.y;
                        visibleWidth=visible.width/renderer.sprite.pixelsPerUnit*renderer.transform.lossyScale.x;
                    }
                    people.Add(System.FormattableString.Invariant($"{entry.phase},{entry.area},{renderer.name},{renderer.sprite.texture.name},{visibleHeight:F4},{visibleWidth:F4},{renderer.sprite.pixelsPerUnit:F4}"));
                    Assert.That(visibleHeight,Is.InRange(.35f,2.1f),"Complete human silhouette: "+renderer.name+" phase "+entry.phase);
                    Assert.That(visibleWidth,Is.InRange(.1f,1.8f),"Undistorted human silhouette: "+renderer.name);
                    if(renderer.name=="Padre Fábio")
                    {
                        // The original church sprite keeps transparent padding in its
                        // 64px frame. Compare the visible silhouette, not SpriteRenderer.bounds.
                        Assert.That(visibleHeight,Is.EqualTo(CampaignCharacterProportions.IsChurch(entry.phase)?CampaignTeamEdelzio.ChurchStandingHeight:CampaignTeamEdelzio.StandingHeight).Within(.12f));
                    }
                }
                var probe=new GameObject("QA_contact_probe");var body=probe.AddComponent<Rigidbody2D>();body.gravityScale=0;body.constraints=RigidbodyConstraints2D.FreezeRotation;body.collisionDetectionMode=CollisionDetectionMode2D.Continuous;
                var circle=probe.AddComponent<CircleCollider2D>();circle.radius=.14f;circle.enabled=false;
                foreach(var other in Object.FindObjectsByType<Collider2D>())if(other!=circle&&!other.transform.IsChildOf(map))Physics2D.IgnoreCollision(circle,other);
                foreach(var target in furnishings.GetComponentsInChildren<Collider2D>().Where(c=>c.enabled&&!c.isTrigger))
                {
                    int accessible=0,touched=0;props++;
                    foreach(var direction in new[]{Vector2.left,Vector2.right,Vector2.up,Vector2.down})
                    {
                        var b=target.bounds;float extent=direction.x!=0?b.extents.x:b.extents.y;
                        Vector2 external=(Vector2)b.center+direction*(extent+.3f);Vector2 edge=target.ClosestPoint(external);
                        Vector2 normal=(external-edge).normalized;if(normal.sqrMagnitude<.5f)continue;
                        Vector2 start=edge+normal*.19f;
                        if(Physics2D.OverlapCircleAll(start,.145f).Any(c=>c!=circle&&c.enabled&&!c.isTrigger&&c.transform.IsChildOf(map)))continue;
                        accessible++;circle.enabled=false;body.position=start;body.linearVelocity=Vector2.zero;Physics2D.SyncTransforms();circle.enabled=true;body.WakeUp();
                        for(int frame=0;frame<6;frame++){body.linearVelocity=-normal*1.3f;yield return new WaitForFixedUpdate();}
                        var distance=circle.Distance(target);checks++;
                        Assert.That(distance.distance,Is.GreaterThanOrEqualTo(-.025f),"Physical penetration: "+target.name+" phase "+entry.phase+" side "+direction);
                        if(distance.distance<.025f)touched++;
                        body.linearVelocity=Vector2.zero;circle.enabled=false;
                    }
                    if(accessible==0)unreachable++;
                    else Assert.That(touched,Is.GreaterThan(0),"A free approach must reach the prop's actual collision surface: "+target.name+" phase "+entry.phase);
                    contacts.Add($"{entry.phase},{entry.area},{target.name.Replace(',', ' ')},{accessible},{touched}");
                }
                Object.Destroy(probe);CaptureMap(Camera.main,map,entry.phase,entry.area);yield return null;
            }
            Assert.That(checks,Is.GreaterThan(100));
            File.WriteAllLines(Path.Combine(CinemaQaEvidence.Folder,"furniture-contacts.csv"),contacts);
            File.WriteAllLines(Path.Combine(CinemaQaEvidence.Folder,"character-proportions.csv"),people);
            File.WriteAllText(Path.Combine(CinemaQaEvidence.Folder,"world-audit-counts.json"),$"{{\"areas\":{entries.Count},\"furniture\":{props},\"physicalApproaches\":{checks},\"withoutFreeProbeApproach\":{unreachable}}}");
        }
        static void CaptureMap(Camera camera,Transform map,int phase,int area)
        {
            var data=CampaignIllustratedMaps.Get(area==1?phase==12?112:121:phase);if(data==null)return;
            var follow=camera.GetComponent<Game.Level.CameraFollow2D>();if(follow!=null)follow.enabled=false;
            var b=data.Bounds;camera.transform.position=new Vector3(b.center.x,b.center.y,-10);camera.orthographicSize=Mathf.Max(b.height/2,b.width/(2*16f/9))+.3f;
            var previous=camera.targetTexture;var active=RenderTexture.active;var target=RenderTexture.GetTemporary(1280,720,24);var image=new Texture2D(1280,720,TextureFormat.RGB24,false);
            string folder=Path.Combine(CinemaQaEvidence.Folder,"world-audit");Directory.CreateDirectory(folder);
            try{camera.targetTexture=target;VarginhaPixelPresentation.RenderInto(camera,target);RenderTexture.active=target;image.ReadPixels(new Rect(0,0,1280,720),0,0);image.Apply();File.WriteAllBytes(Path.Combine(folder,phase+"_"+area+".png"),image.EncodeToPNG());}
            finally{camera.targetTexture=previous;RenderTexture.active=active;RenderTexture.ReleaseTemporary(target);Object.Destroy(image);}
        }
    }
}
