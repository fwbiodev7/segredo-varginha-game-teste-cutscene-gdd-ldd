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
    public sealed class CampaignVehicleContactTests:InputTestFixture
    {
        private string _save;
        public override void Setup(){VarginhaInputActions.Shutdown();base.Setup();_save=File.Exists(CampaignStorySave.Path)?File.ReadAllText(CampaignStorySave.Path):null;}
        public override void TearDown()
        {
            Time.timeScale=1;VarginhaInputActions.Shutdown();base.TearDown();
            if(_save==null){if(File.Exists(CampaignStorySave.Path))File.Delete(CampaignStorySave.Path);}else File.WriteAllText(CampaignStorySave.Path,_save);
        }
        private static IEnumerator Hold(Keyboard keyboard,Key key,float seconds)
        {
            float until=Time.time+seconds;
            while(Time.time<until)
            {
                InputSystem.QueueStateEvent(keyboard,new KeyboardState(key));yield return null;
            }
            InputSystem.QueueStateEvent(keyboard,new KeyboardState());yield return null;
        }
        [UnityTest,Timeout(90000)] public IEnumerator WorkshopDepartureStopsAtObstaclesThenReversesThroughTheGateAndTurnsOnTheStreet()
        {
            var story=new CampaignStory{phase=10};var s=story.expansion;
            s.workshopParked=true;s.visited=7;s.mapSolved=true;s.forestSigns=3;s.fabioMet=true;
            s.anchorClues=7;s.anchorFound=true;s.evidencePresented=true;s.labClues=7;s.reagentUnlocked=true;s.sprayed=7;s.stabilized=true;
            CampaignStorySave.Write(story);yield return SceneManager.LoadSceneAsync(CampaignStorySave.Scene(10));yield return new WaitForSeconds(3.4f);
            var stage=CampaignExpansionController.Active;var vehicle=Object.FindAnyObjectByType<CampaignWorkshopVehicle>();
            var body=vehicle.GetComponent<Rigidbody2D>();var box=vehicle.GetComponent<BoxCollider2D>();
            var obstacle=new GameObject("QA_Obstacle_in_departure");var wall=obstacle.AddComponent<BoxCollider2D>();wall.size=new Vector2(3,.25f);obstacle.transform.position=new Vector2(body.position.x,-2.2f);Physics2D.SyncTransforms();
            stage.Interact("drive");float deadline=Time.realtimeSinceStartup+12;
            while(!vehicle.DepartureBlocked&&Time.realtimeSinceStartup<deadline)
            {Assert.That(body.simulated,Is.True);Assert.That(box.Distance(wall).distance,Is.GreaterThan(-.015f));yield return null;}
            Assert.That(vehicle.DepartureBlocked,Is.True);Assert.That(stage.Progress.expansion.workshopDeparted,Is.False);
            yield return null;stage.ClosePanel();Assert.That(Object.FindAnyObjectByType<EdelzioTopDownController>().IsInputLocked,Is.False);
            Object.Destroy(obstacle);yield return null;stage.Interact("drive");
            bool reversed=false,turned=false;deadline=Time.realtimeSinceStartup+22;
            var layout=CampaignIllustratedMaps.Get(10);float gateSouth=layout.Position(0,669).y;
            while(!vehicle.DepartureComplete&&Time.realtimeSinceStartup<deadline)
            {
                Assert.That(body.simulated,Is.True,"Cinematics keep the real body and collision sweeps.");
                reversed|=vehicle.Speed<-.1f;
                if(Mathf.Abs(vehicle.Heading)>5){turned=true;Assert.That(box.bounds.max.y,Is.LessThan(gateSouth),"Turn only after the entire car clears the garage.");}
                foreach(var solid in Object.FindObjectsByType<Collider2D>())
                    if(!solid.isTrigger&&solid.attachedRigidbody!=body)Assert.That(box.Distance(solid).distance,Is.GreaterThan(-.02f),solid.name);
                yield return new WaitForFixedUpdate();
            }
            Assert.That(reversed&&turned&&vehicle.DepartureComplete,Is.True);yield return null;
            Assert.That(CampaignStorySave.Load().expansion.workshopDeparted,Is.True);
            Assert.That(box.bounds.max.x,Is.LessThan(layout.Bounds.xMax),"The next chapter fades from the street before the boundary wall.");
            yield return SceneManager.LoadSceneAsync("Menu_MisterioDeVarginha");
        }
        [UnityTest,Timeout(60000)] public IEnumerator LegacyRoadAcceleratesCoastsReversesAndEscapesItsLowerLimit()
        {
            var keyboard=InputSystem.AddDevice<Keyboard>();CampaignStorySave.Write(new CampaignStory{phase=3});
            // The legacy road is deliberately absent from the 19-scene release.
            // Exercise its real controller/map in an isolated test scene instead.
            yield return SceneManager.LoadSceneAsync("Menu_MisterioDeVarginha");var menu=SceneManager.GetActiveScene();
            var scene=SceneManager.CreateScene("LegacyRoad_Physics_QA");SceneManager.SetActiveScene(scene);
            yield return SceneManager.UnloadSceneAsync(menu);
            new GameObject("LegacyRoad_QA").AddComponent<VarginhaCampaignDrive>();yield return new WaitForSeconds(3.4f);
            var road=VarginhaCampaignDrive.Active;var body=GameObject.Find("Fusca_TopView_Campanha").GetComponent<Rigidbody2D>();
            yield return Hold(keyboard,Key.DownArrow,.6f);Assert.That(body.position.y,Is.EqualTo(0).Within(.02f));
            yield return Hold(keyboard,Key.UpArrow,.5f);
            if(body.position.y<=.2f)
            {
                var fields=System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Instance;
                foreach(string field in new[]{"_state","_titleTime","_paused","_inspect","_input","_speed","_velocity"})
                    Debug.Log("ROAD_DIAGNOSTIC "+field+"="+typeof(VarginhaCampaignDrive).GetField(field,fields).GetValue(road));
                var box=body.GetComponent<BoxCollider2D>();var hits=Physics2D.BoxCastAll(body.position,box.size,body.rotation,Vector2.up,.15f);
                foreach(var hit in hits)if(hit.collider!=box)Debug.Log("ROAD_CONTACT "+hit.collider.name+" distance="+hit.distance+" normal="+hit.normal+" separation="+box.Distance(hit.collider).distance);
            }
            Assert.That(body.position.y,Is.GreaterThan(.2f));
            Assert.That(road.Speed,Is.InRange(1.5f,4f),"The initial half second must accelerate instead of jumping to full speed.");
            float speed=road.Speed,y=body.position.y;yield return new WaitForSeconds(.1f);
            Assert.That(road.Speed,Is.GreaterThan(0));Assert.That(road.Speed,Is.LessThan(speed));Assert.That(body.position.y,Is.GreaterThan(y));
            // Observe reverse before this short approach reaches its lower wall.
            // Holding it for 1.2s had already hit the boundary and correctly stopped.
            yield return Hold(keyboard,Key.DownArrow,.45f);Assert.That(road.Speed,Is.LessThan(0));
            yield return Hold(keyboard,Key.RightArrow,.1f);Assert.That(Mathf.Abs(road.Steering),Is.LessThan(.7f));
            yield return SceneManager.LoadSceneAsync("Menu_MisterioDeVarginha");
        }
        [UnityTest,Timeout(60000)] public IEnumerator OptionalTrackUsesFixedStepAccelerationCoastAndBrakeBeforeChangingDirection()
        {
            var story=new CampaignStory{phase=10};story.expansion.workshopParked=true;CampaignStorySave.Write(story);
            yield return SceneManager.LoadSceneAsync(CampaignStorySave.Scene(10));yield return new WaitForSeconds(3.4f);
            var vehicle=Object.FindAnyObjectByType<CampaignWorkshopVehicle>();var body=vehicle.GetComponent<Rigidbody2D>();
            vehicle.BeginTrack();vehicle.SetPosition(new Vector2(-4,-4.5f));vehicle.SetTrackControls(1);
            yield return new WaitForSeconds(.4f);Assert.That(vehicle.Speed,Is.InRange(.8f,2f));
            float x=body.position.x,speed=vehicle.Speed;vehicle.SetTrackControls(0);yield return new WaitForFixedUpdate();yield return new WaitForFixedUpdate();
            Assert.That(vehicle.Speed,Is.GreaterThan(0));Assert.That(vehicle.Speed,Is.LessThan(speed));Assert.That(body.position.x,Is.GreaterThan(x));
            vehicle.SetTrackControls(-1);yield return new WaitForFixedUpdate();
            Assert.That(vehicle.Direction,Is.EqualTo(CampaignWorkshopVehicle.Facing.East),"A reverse command brakes before the body changes direction.");
            yield return new WaitForSeconds(.8f);Assert.That(vehicle.Speed,Is.LessThan(0));Assert.That(vehicle.Direction,Is.EqualTo(CampaignWorkshopVehicle.Facing.West));
            vehicle.FinishTrack();Assert.That(vehicle.Speed,Is.Zero);yield return SceneManager.LoadSceneAsync("Menu_MisterioDeVarginha");
        }
        [UnityTest,Timeout(60000)] public IEnumerator InitialContactWithTheBottomWallDoesNotTrapForwardMotion()
        {
            var keyboard=InputSystem.AddDevice<Keyboard>();
            var story=new CampaignStory{phase=10};story.expansion.anchorFound=true;story.expansion.anchorClues=7;
            story.expansion.evidencePresented=true;story.expansion.reagentUnlocked=true;CampaignStorySave.Write(story);
            yield return SceneManager.LoadSceneAsync(CampaignStorySave.Scene(10));yield return new WaitForSeconds(3.2f);
            var vehicle=Object.FindAnyObjectByType<CampaignWorkshopVehicle>();var body=vehicle.GetComponent<Rigidbody2D>();
            // A small initial penetration models a contact restored at the street limit.
            vehicle.SetPosition(new Vector2(.17f,-6.02f));Physics2D.SyncTransforms();float initial=body.position.y;
            float until=Time.realtimeSinceStartup+1.4f;
            while(Time.realtimeSinceStartup<until){InputSystem.QueueStateEvent(keyboard,new KeyboardState(Key.UpArrow));yield return null;}
            InputSystem.QueueStateEvent(keyboard,new KeyboardState());
            Assert.That(body.position.y,Is.GreaterThan(initial+.6f),"An initial contact must allow movement away from the wall.");
            yield return SceneManager.LoadSceneAsync("Menu_MisterioDeVarginha");
        }
    }
}
