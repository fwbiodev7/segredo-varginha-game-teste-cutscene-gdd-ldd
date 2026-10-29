using System.Collections;
using System.IO;
using Game.Varginha;
using Game.Varginha.Experiment;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace Game.Tests.PlayMode
{
    public sealed class CampaignCloseRoutinePlayTests:InputTestFixture
    {
        string _save;Keyboard _keyboard;
        public override void Setup(){VarginhaInputActions.Shutdown();base.Setup();_save=File.Exists(CampaignStorySave.Path)?File.ReadAllText(CampaignStorySave.Path):null;_keyboard=InputSystem.AddDevice<Keyboard>();}
        public override void TearDown(){Time.timeScale=1;VarginhaInputActions.Shutdown();base.TearDown();}
        [UnityTearDown]public IEnumerator Restore()
        {
            Time.timeScale=1;yield return SceneManager.LoadSceneAsync("Menu_MisterioDeVarginha");
            if(_save==null){if(File.Exists(CampaignStorySave.Path))File.Delete(CampaignStorySave.Path);}else File.WriteAllText(CampaignStorySave.Path,_save);
        }
        IEnumerator Hold(Key key,float seconds)
        {
            float until=Time.time+seconds;
            while(Time.time<until){InputSystem.QueueStateEvent(_keyboard,new KeyboardState(key));yield return null;}
            InputSystem.QueueStateEvent(_keyboard,new KeyboardState());yield return null;
        }
        static IEnumerator Load()
        {
            CampaignStorySave.Write(new CampaignStory{phase=2});yield return SceneManager.LoadSceneAsync(CampaignStorySave.Scene(2));yield return new WaitForSeconds(3.4f);
            VarginhaCampaignStage.Active.CloseDialogue();
        }
        static IEnumerator Capture(string name)
        {
            if(Application.isEditor)yield break;yield return new WaitForEndOfFrame();var texture=ScreenCapture.CaptureScreenshotAsTexture();
            var folder=Path.Combine(CinemaQaEvidence.Folder,"close-routines");Directory.CreateDirectory(folder);
            File.WriteAllBytes(Path.Combine(folder,name+".png"),texture.EncodeToPNG());Object.Destroy(texture);
        }
        [UnityTest,Timeout(180000)]public IEnumerator RealWalkWashCoffeeAndNotebookUseCloseAnchorsThenRestoreControlAndReload()
        {
            yield return Load();var stage=VarginhaCampaignStage.Active;var actor=Object.FindAnyObjectByType<EdelzioTopDownController>();var body=actor.GetComponent<Rigidbody2D>();
            var feet=actor.GetComponent<CircleCollider2D>();var plan=CampaignMapPlan.Create(2);int expected=0;
            foreach(string id in new[]{"wash","food","work"})
            {
                Assert.That(CampaignAdultHouse.CanUseRoutine(actor,id),Is.False,"The previous room cannot activate "+id);
                stage.Interact(id);yield return null;Assert.That(stage.IsActing,Is.False);Assert.That(stage.Progress.routine,Is.EqualTo(expected));
                yield return CampaignExpansionPlayTests.WalkLibrary(plan,body,_keyboard,plan.points.Find(p=>p.id==id).position);
                Assert.That(CampaignAdultHouse.CanUseRoutine(actor,id),Is.True);
                yield return Hold(Key.E,.08f);Assert.That(stage.IsActing,Is.True,id+" starts through the actual interaction binding.");
                yield return new WaitForSeconds(.55f);yield return Capture(id+"-near");
                var action=actor.GetComponent<VarginhaPlayerSpriteAnimation>();
                if(id=="wash")
                {
                    Assert.That(action.CurrentActionPose,Is.EqualTo("Edelzio_WashFace"));
                    Assert.That(Vector2.Distance(body.position,CampaignAdultHouse.WashApproach),Is.LessThanOrEqualTo(CampaignAdultHouse.RoutineArrivalTolerance));
                    Assert.That(feet.Distance(stage.transform.Find("Mapa_Campanha/02_Mobilia_Colisoes/Kitchen_Cabinet").GetComponent<Collider2D>()).distance,Is.GreaterThan(-.01f));
                }
                float deadline=Time.realtimeSinceStartup+10;while(stage.IsActing&&Time.realtimeSinceStartup<deadline)yield return null;
                Assert.That(stage.IsActing,Is.False);yield return null;Assert.That(actor.IsInputLocked,Is.False,id+" restores input.");
                expected|=id=="wash"?1:id=="food"?2:4;Assert.That(stage.Progress.routine,Is.EqualTo(expected));
                Vector2 start=body.position;yield return Hold(Key.D,.16f);Assert.That(body.position.x-start.x,Is.GreaterThan(.15f),id+" allows immediate movement.");
            }
            Assert.That(stage.Progress.house.step,Is.Zero,"The routine does not skip the key investigation.");
            var items=stage.GetComponent<CampaignHouseRoutineItems>();Assert.That(items.Coffee.gameObject.activeSelf,Is.False);Assert.That(items.Notebook.gameObject.activeSelf,Is.False);
            Assert.That(CampaignStorySave.Load().routine,Is.EqualTo(7));
            yield return SceneManager.LoadSceneAsync(CampaignStorySave.Scene(2));yield return new WaitForSeconds(3.4f);
            stage=VarginhaCampaignStage.Active;items=stage.GetComponent<CampaignHouseRoutineItems>();Assert.That(stage.Progress.routine,Is.EqualTo(7));
            Assert.That(items.Coffee.gameObject.activeSelf,Is.False);Assert.That(items.Notebook.gameObject.activeSelf,Is.False);
            Assert.That(Object.FindAnyObjectByType<EdelzioTopDownController>().IsInputLocked,Is.False);
        }
        [UnityTest,Timeout(45000)]public IEnumerator RoutineActivationRejectsDistanceAndObstaclesWithoutConsumingItems()
        {
            yield return Load();var stage=VarginhaCampaignStage.Active;var actor=Object.FindAnyObjectByType<EdelzioTopDownController>();var body=actor.GetComponent<Rigidbody2D>();var feet=actor.GetComponent<CircleCollider2D>();
            foreach(string id in new[]{"wash","food","work"})
            {
                var target=CampaignAdultHouse.RoutineApproach(id);body.position=target+Vector2.up;yield return new WaitForFixedUpdate();
                Assert.That(CampaignAdultHouse.CanUseRoutine(actor,id),Is.False);stage.Interact(id);yield return null;Assert.That(stage.Progress.routine,Is.Zero);
                body.position=target+Vector2.up*.5f;yield return new WaitForFixedUpdate();
                var obstacle=new GameObject("QA_routine_occluder");var box=obstacle.AddComponent<BoxCollider2D>();box.size=new Vector2(.8f,.04f);
                obstacle.transform.position=(Vector2)feet.bounds.center+Vector2.down*.25f;Physics2D.SyncTransforms();
                Assert.That(CampaignAdultHouse.CanUseRoutine(actor,id),Is.False,id+" cannot be used through an obstacle.");
                stage.Interact(id);yield return null;Assert.That(stage.IsActing,Is.False);Assert.That(stage.Progress.routine,Is.Zero);
                Object.Destroy(obstacle);yield return null;Physics2D.SyncTransforms();
            }
            var items=stage.GetComponent<CampaignHouseRoutineItems>();Assert.That(items.Coffee.gameObject.activeSelf,Is.True);Assert.That(items.Notebook.gameObject.activeSelf,Is.True);
        }
        [UnityTest,Timeout(30000)]public IEnumerator CoffeeBlockedByTheFeetRadiusCancelsPromptlyWithoutConsumption()
        {
            yield return Load();var stage=VarginhaCampaignStage.Active;var actor=Object.FindAnyObjectByType<EdelzioTopDownController>();
            var body=actor.GetComponent<Rigidbody2D>();var feet=actor.GetComponent<CircleCollider2D>();var target=CampaignAdultHouse.RoutineApproach("food");
            body.position=target+Vector2.left*.5f;yield return new WaitForFixedUpdate();yield return null;
            var obstacle=new GameObject("QA_tangent_coffee_obstacle");var box=obstacle.AddComponent<BoxCollider2D>();box.size=new Vector2(.02f,.08f);
            obstacle.transform.position=(Vector2)feet.bounds.center+new Vector2(.25f,.23f);Physics2D.SyncTransforms();
            try
            {
                Assert.That(CampaignAdultHouse.CanUseRoutine(actor,"food"),Is.True,"The central line is clear; the physical feet radius still meets an obstacle.");
                stage.Interact("food");Assert.That(stage.IsActing,Is.True);
                float started=Time.realtimeSinceStartup,deadline=started+2;
                while(stage.IsActing&&Time.realtimeSinceStartup<deadline)yield return null;
                Assert.That(stage.IsActing,Is.False);Assert.That(Time.realtimeSinceStartup-started,Is.LessThan(1.5f));yield return null;
                Assert.That(actor.IsInputLocked,Is.False);Assert.That(stage.Progress.routine,Is.Zero);
                Assert.That(stage.GetComponent<CampaignHouseRoutineItems>().Coffee.gameObject.activeSelf,Is.True);
                Assert.That(feet.Distance(box).distance,Is.GreaterThan(-.01f));
            }
            finally{Object.Destroy(obstacle);}
        }
    }
}
