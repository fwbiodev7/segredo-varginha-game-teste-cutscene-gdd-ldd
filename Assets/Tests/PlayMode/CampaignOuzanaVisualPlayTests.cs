using System.Collections;
using System.IO;
using Game.Varginha;
using Game.Varginha.Experiment;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace Game.Tests.PlayMode
{
    public sealed class CampaignOuzanaVisualPlayTests
    {
        string saved;
        [SetUp] public void Preserve()=>saved=File.Exists(CampaignStorySave.Path)?File.ReadAllText(CampaignStorySave.Path):null;
        [UnityTearDown] public IEnumerator Restore()
        {
            Time.timeScale=1;yield return SceneManager.LoadSceneAsync("Menu_MisterioDeVarginha");
            if(saved==null){if(File.Exists(CampaignStorySave.Path))File.Delete(CampaignStorySave.Path);}else File.WriteAllText(CampaignStorySave.Path,saved);
        }
        [UnityTest,Timeout(90000)] public IEnumerator NewScientistRendersAllEightWalkAndAttackDirectionsAtHerExistingLaboratoryContact()
        {
            CampaignStorySave.Write(new CampaignStory{phase=18});
            yield return SceneManager.LoadSceneAsync(CampaignStorySave.Scene(18));yield return new WaitForSeconds(3.5f);
            var controller=CampaignContinuationController.Active;controller.CloseMessage();
            var actor=controller.transform.Find("Ouzana");var renderer=actor.GetComponent<SpriteRenderer>();
            Assert.That(renderer.sprite,Is.SameAs(CampaignOuzanaVisual.Idle(Vector2.right)));
            var collider=actor.GetComponent<CircleCollider2D>();var contact=collider.bounds;
            var camera=Camera.main;var follow=camera.GetComponent<Game.Level.CameraFollow2D>();if(follow!=null)follow.enabled=false;
            camera.orthographicSize=1.8f;camera.transform.position=new Vector3(actor.position.x,actor.position.y+.3f,-10);
            for(int direction=0;direction<8;direction++)
            {
                float angle=direction*45*Mathf.Deg2Rad;Vector2 face=new(-Mathf.Sin(angle),-Mathf.Cos(angle));
                for(int frame=0;frame<6;frame++)
                {
                    renderer.sprite=CampaignOuzanaVisual.Walk(face,frame*.11f+.001f);yield return null;
                    Capture(camera,"walk_"+direction+"_"+frame);
                    renderer.sprite=CampaignOuzanaVisual.Attack(face,frame);yield return null;
                    Capture(camera,"attack_"+direction+"_"+frame);
                    Assert.That(collider.bounds,Is.EqualTo(contact),"Visual poses cannot resize or move the existing collision contact.");
                }
            }
            renderer.sprite=CampaignOuzanaVisual.Idle(Vector2.right);
        }
        static void Capture(Camera camera,string name)
        {
            string folder=Path.Combine(CinemaQaEvidence.Folder,"ouzana-runtime");Directory.CreateDirectory(folder);
            var previous=camera.targetTexture;var active=RenderTexture.active;var target=RenderTexture.GetTemporary(640,360,24);
            var image=new Texture2D(640,360,TextureFormat.RGB24,false);
            try{camera.targetTexture=target;VarginhaPixelPresentation.RenderInto(camera,target);RenderTexture.active=target;image.ReadPixels(new Rect(0,0,640,360),0,0);image.Apply();File.WriteAllBytes(Path.Combine(folder,name+".png"),image.EncodeToPNG());}
            finally{camera.targetTexture=previous;RenderTexture.active=active;RenderTexture.ReleaseTemporary(target);Object.Destroy(image);}
        }
    }
}
