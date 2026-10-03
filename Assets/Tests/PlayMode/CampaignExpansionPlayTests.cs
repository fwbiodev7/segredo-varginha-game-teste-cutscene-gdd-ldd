using System.Collections;
using System.IO;
using Game.Varginha;
using Game.Varginha.Experiment;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace Game.Tests.PlayMode
{
    public class CampaignExpansionPlayTests : InputTestFixture
    {
        private string _save;
        [SetUp] public void Preserve() => _save=File.Exists(CampaignStorySave.Path)?File.ReadAllText(CampaignStorySave.Path):null;
        [TearDown] public void Restore()
        {
            Time.timeScale=1;
            if(_save!=null) File.WriteAllText(CampaignStorySave.Path,_save); else if(File.Exists(CampaignStorySave.Path))File.Delete(CampaignStorySave.Path);
        }
        [UnityTest] public IEnumerator AllFiveScenesLoadAndRetainEvidenceAndFurnitureCollisions()
        {
            var story=new CampaignStory{phase=6}; CampaignStorySave.Write(story);
            for(int phase=6;phase<=10;phase++)
            {
                yield return SceneManager.LoadSceneAsync(CampaignStorySave.Scene(phase)); yield return null; yield return new WaitForSeconds(.15f);
                var stage=CampaignExpansionController.Active; Assert.That(stage,Is.Not.Null);
                Assert.That(stage.phase,Is.EqualTo(phase));
                var map=GameObject.Find("Mapa_Campanha"); Assert.That(map.GetComponentsInChildren<Collider2D>().Length,Is.EqualTo(stage.Plan.walls.Count+stage.Plan.furniture.FindAll(p=>p.footprint.width>0&&p.footprint.height>0).Count));
                var actor=Object.FindAnyObjectByType<EdelzioTopDownController>();
                Assert.That(actor.GetComponent<CampaignTeamEdelzio>(),Is.Not.Null);
                Assert.That(actor.GetComponent<SpriteRenderer>().sprite.name,Does.StartWith("Team_"));
                if(phase==6)
                {
                    foreach(string point in new[]{"archive","school","square","truth0","truth1"}){stage.Interact(point);stage.ClosePanel();}
                    stage.Progress.expansion.mapOrder=new[]{0,1,2}; stage.Interact("map"); Assert.That(stage.SubmitPuzzle(),Is.True); stage.ClosePanel();
                }
                if(phase==7)
                {
                    foreach(string point in new[]{"sign0","sign1","sign2"}){stage.Interact(point);stage.ClosePanel();}
                    stage.ReturnToCheckpoint(); stage.ClosePanel(); Assert.That(stage.Progress.expansion.forestSigns,Is.EqualTo(3));
                    stage.Interact("fabio");stage.ClosePanel();
                }
                if(phase==8)
                {
                    foreach(string point in new[]{"index","symbol","record","truth2"}){stage.Interact(point);stage.ClosePanel();}
                    Assert.That(stage.Progress.expansion.SolveAnchor(1996,1,23),Is.True);stage.Save();
                }
                if(phase==9)
                {
                    foreach(string point in new[]{"ouzana","control","residue","protocol"}){stage.Interact(point);stage.ClosePanel();}
                    stage.Progress.expansion.sampleOrder=new[]{0,1,2};stage.Interact("samples");Assert.That(stage.SubmitPuzzle(),Is.True);stage.ClosePanel();
                }
                if(phase==10)
                {
                    foreach(string point in new[]{"spray0","spray1","spray2"}){stage.Interact(point);stage.ClosePanel();}
                    stage.Progress.expansion.sealOrder=new[]{0,1,2};stage.Interact("seal");Assert.That(stage.SubmitPuzzle(),Is.True);stage.ClosePanel();
                    Assert.That(stage.Progress.expansion.truthClues,Is.EqualTo(7));
                    stage.Save();
                }
                Assert.That(CampaignStorySave.Load().phase,Is.EqualTo(phase));
            }
            yield return SceneManager.LoadSceneAsync("Menu_MisterioDeVarginha");
        }
        [UnityTest] public IEnumerator PhysicalMovementUsesDoorsAndCannotCrossTheArchiveWall()
        {
            CampaignStorySave.Write(new CampaignStory{phase=6});yield return SceneManager.LoadSceneAsync(CampaignStorySave.Scene(6));yield return new WaitForSeconds(3);
            var actor=Object.FindAnyObjectByType<EdelzioTopDownController>();var body=actor.GetComponent<Rigidbody2D>();var keyboard=InputSystem.AddDevice<Keyboard>();
            body.position=new Vector2(-9,0);Press(keyboard.wKey);yield return new WaitForSeconds(.6f);Release(keyboard.wKey);
            Assert.That(body.position.y-.58f,Is.LessThan(1),"Solid wall blocks feet.");
            body.position=new Vector2(-7.5f,0);body.linearVelocity=Vector2.zero;Press(keyboard.wKey);yield return new WaitForSeconds(.9f);Release(keyboard.wKey);
            Assert.That(body.position.y-.58f,Is.GreaterThan(1.4f),"Door allows actual rigidbody movement.");
            yield return SceneManager.LoadSceneAsync("Menu_MisterioDeVarginha");
        }
        [UnityTest] public IEnumerator FuscaRequiresStabilizationAndCompletesOnlyAfterDrivingTheTrack()
        {
            var story=new CampaignStory{phase=10};var s=story.expansion;
            s.visited=7;s.mapSolved=true;s.forestSigns=3;s.fabioMet=true;s.anchorClues=7;s.anchorFound=true;s.evidencePresented=true;s.labClues=7;s.reagentUnlocked=true;s.sprayed=7;s.stabilized=true;
            CampaignStorySave.Write(story);yield return SceneManager.LoadSceneAsync(CampaignStorySave.Scene(10));yield return new WaitForSeconds(3);
            var stage=CampaignExpansionController.Active;stage.Interact("drive");Assert.That(stage.Progress.expansion.testDriven,Is.False);
            var keyboard=InputSystem.AddDevice<Keyboard>();Press(keyboard.dKey);yield return new WaitForSeconds(4.2f);Release(keyboard.dKey);
            Assert.That(stage.Progress.expansion.Complete(10),Is.True);
            Assert.That(CampaignStorySave.Load().expansion.testDriven,Is.True);
            yield return SceneManager.LoadSceneAsync("Menu_MisterioDeVarginha");
        }
        [UnityTest] public IEnumerator SchoolKeepsGateScaleAndStudentsClearOfRenanAndFurniture()
        {
            CampaignStorySave.Write(new CampaignStory{phase=4});
            yield return SceneManager.LoadSceneAsync(CampaignStorySave.Scene(4));yield return new WaitForSeconds(3.2f);
            var facade=Object.FindAnyObjectByType<VarginhaIndustrialSchoolFacade>();
            Assert.That(facade,Is.Not.Null);var renderer=facade.GetComponent<SpriteRenderer>();Assert.That(renderer.enabled,Is.True);
            Assert.That(renderer.sprite.texture,Is.EqualTo(Resources.Load<Texture2D>(VarginhaIndustrialSchoolFacade.ResourcePath)));
            var archive=GameObject.Find("Arquivo escolar").GetComponent<SpriteRenderer>();
            Assert.That(archive.enabled,Is.False,"Interior furniture must not appear over the exterior sign.");
            var renan=GameObject.Find("Renan_Industrial_Campanha").GetComponent<Collider2D>();
            var plan=CampaignMapPlan.Create(4);
            foreach(var student in Object.FindObjectsByType<CampaignSchoolLife>(FindObjectsSortMode.None))
            {
                var feet=student.GetComponent<CircleCollider2D>();var ground=(Vector2)student.transform.position+feet.offset;
                Assert.That(plan.IsClear(ground,feet.radius),Is.True,student.StudentName+" feet");
                Assert.That(feet.Distance(renan).isOverlapped,Is.False,student.StudentName+" overlaps Renan");
            }
            var actor=Object.FindAnyObjectByType<EdelzioTopDownController>();actor.GetComponent<Rigidbody2D>().position=new Vector2(.75f,-4.3f);
            yield return null;yield return null;Assert.That(renderer.enabled,Is.False);
            Assert.That(archive.enabled,Is.True);
            actor.GetComponent<Rigidbody2D>().position=new Vector2(0,-10);yield return null;yield return null;Assert.That(renderer.enabled,Is.True);
            yield return SceneManager.LoadSceneAsync("Menu_MisterioDeVarginha");
        }
        [UnityTest]public IEnumerator CharacterActionsAndPunchUseNewTransparentAnimationSheets()
        {
            CampaignStorySave.Write(new CampaignStory{phase=6});
            yield return SceneManager.LoadSceneAsync(CampaignStorySave.Scene(6));yield return new WaitForSeconds(3);
            var actor=Object.FindAnyObjectByType<EdelzioTopDownController>();var actions=actor.GetComponent<VarginhaPlayerSpriteAnimation>();var renderer=actor.GetComponent<SpriteRenderer>();
            actions.SetSeatingFacing(Vector2.left);actions.SetSeatingFrame(1);yield return null;yield return null;
            Assert.That(renderer.sprite.texture.name,Is.EqualTo("EdelzioActionsV2"));
            actions.SetCoffeeFrame(2);yield return null;yield return null;Assert.That(renderer.sprite.name,Does.Contain("EdelzioActions"));
            foreach(string pose in new[]{"Edelzio_Crouch","Edelzio_Reach"})
            {actions.SetActionPose(pose);yield return null;yield return null;Assert.That(renderer.sprite.texture.name,Is.EqualTo("EdelzioInteractionsV2"));}
            actions.ClearActionPose();yield return null;
            var attack=actor.GetComponent<VarginhaPlayerAttack>();Assert.That(attack,Is.Not.Null);
            Assert.That(attack.TryAttack(Vector2.right),Is.True);yield return new WaitForSeconds(.1f);
            Assert.That(renderer.sprite.texture.name,Is.EqualTo("EdelzioPunchV2"));yield return new WaitForSeconds(.4f);
            Assert.That(renderer.sprite.texture.name,Is.Not.EqualTo("EdelzioPunchV2"));
            yield return SceneManager.LoadSceneAsync("Menu_MisterioDeVarginha");
        }
    }
}
