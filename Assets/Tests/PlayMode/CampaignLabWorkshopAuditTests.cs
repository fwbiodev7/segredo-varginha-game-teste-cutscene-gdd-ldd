using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
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
    public class CampaignLabWorkshopAuditTests : InputTestFixture
    {
        public override void Setup(){Game.Varginha.VarginhaInputActions.Shutdown();base.Setup();}
        public override void TearDown(){Game.Varginha.VarginhaInputActions.Shutdown();base.TearDown();}
        private byte[] _storySave,_memorySave;
        private Keyboard _keyboard;
        private const BindingFlags Private=BindingFlags.Instance|BindingFlags.NonPublic;
        [SetUp] public void Preserve()
        {
            _storySave=Read(CampaignStorySave.Path);_memorySave=Read(CampaignMemorySave.Path);
            _keyboard=InputSystem.AddDevice<Keyboard>();
        }
        [TearDown] public void Restore()
        {
            Time.timeScale=1;
            RestoreFile(CampaignStorySave.Path,_storySave);RestoreFile(CampaignMemorySave.Path,_memorySave);
        }
        private static byte[] Read(string path)=>File.Exists(path)?File.ReadAllBytes(path):null;
        private static void RestoreFile(string path,byte[] bytes)
        { if(bytes!=null)File.WriteAllBytes(path,bytes);else if(File.Exists(path))File.Delete(path); }
        private static CampaignStory Ready(int phase)
        {
            var story=new CampaignStory { phase=phase };
            var state=story.expansion;
            state.workshopParked=phase==10;
            state.visited=7;state.mapSolved=true;state.forestSigns=3;state.fabioMet=true;
            state.SolveAnchor(1996,1,23);
            return story;
        }
        private static string Modal(CampaignExpansionController stage,string name)
            =>(string)typeof(CampaignExpansionController).GetField(name,Private).GetValue(stage);
        private IEnumerator Load(int phase)
        {
            yield return SceneManager.LoadSceneAsync(CampaignStorySave.Scene(phase));
            yield return new WaitForSeconds(3);
            Assert.That(CampaignExpansionController.Active,Is.Not.Null);
            Assert.That(Object.FindAnyObjectByType<EdelzioTopDownController>().IsInputLocked,Is.False);
        }
        private IEnumerator KeyFor(Key key,float duration)
        {
            float end=Time.realtimeSinceStartup+duration;
            while(Time.realtimeSinceStartup<end)
            { InputSystem.QueueStateEvent(_keyboard,new KeyboardState(key));yield return null; }
            InputSystem.QueueStateEvent(_keyboard,new KeyboardState());yield return null;yield return null;
        }
        private IEnumerator WalkToAndInteract(string id)
        {
            var stage=CampaignExpansionController.Active;
            var actor=Object.FindAnyObjectByType<EdelzioTopDownController>();
            var body=actor.GetComponent<Rigidbody2D>();
            var target=stage.Plan.points.Find(point=>point.id==id).position;
            var route=new List<Vector2>();
            Assert.That(stage.Plan.Route(body.position-Vector2.up*.58f,target,route),Is.True,"Route to "+id);
            route.Add(target);
            foreach(var waypoint in route)
            {
                float deadline=Time.realtimeSinceStartup+2;
                while(Vector2.Distance(body.position-Vector2.up*.58f,waypoint)>.11f)
                {
                    Assert.That(Time.realtimeSinceStartup,Is.LessThan(deadline),"Movement blocked on route to "+id+" at "+waypoint+" from "+(body.position-Vector2.up*.58f)+" panel="+Modal(stage,"_panel")+" message="+Modal(stage,"_message")+" locked="+actor.IsInputLocked);
                    Vector2 delta=waypoint-(body.position-Vector2.up*.58f);
                    // W is now the explicit car command; arrows exercise movement separately.
                    Key key=Mathf.Abs(delta.x)>Mathf.Abs(delta.y)?delta.x>0?Key.RightArrow:Key.LeftArrow:delta.y>0?Key.UpArrow:Key.DownArrow;
                    InputSystem.QueueStateEvent(_keyboard,new KeyboardState(key));yield return null;
                }
            }
            InputSystem.QueueStateEvent(_keyboard,new KeyboardState());yield return null;yield return new WaitForFixedUpdate();
            var nearest=typeof(CampaignExpansionController).GetMethod("Nearest",Private).Invoke(stage,null);
            Assert.That(((CampaignMapPlan.Point)nearest)?.id,Is.EqualTo(id),"Physical approach chooses the visible target");
            yield return KeyFor(stage.phase==10?Key.W:Key.E,.06f);
            bool departing=(bool)typeof(CampaignExpansionController).GetField("_departing",Private).GetValue(stage);
            Assert.That(Modal(stage,"_panel")!=null||Modal(stage,"_message")!=null||departing,Is.True,(stage.phase==10?"W":"E")+" interacts with "+id);
        }

        [UnityTest] public IEnumerator LaboratoryShortComparisonKeepsEvidenceAndSurvivesReload()
        {
            CampaignStorySave.Write(Ready(9));yield return Load(9);
            var stage=CampaignExpansionController.Active;var state=stage.Progress.expansion;
            Assert.That(state.reagentUnlocked,Is.False);
            yield return WalkToAndInteract("ouzana");Assert.That(state.evidencePresented,Is.True);stage.ClosePanel();
            Assert.That(state.labClues,Is.EqualTo(7));Assert.That(state.reagentUnlocked,Is.True);Assert.That(state.reagentCharges,Is.EqualTo(6));
            yield return WalkToAndInteract("tutorial");Assert.That(state.reagentCharges,Is.EqualTo(6));stage.ClosePanel();
            stage.Save();yield return Load(9);stage=CampaignExpansionController.Active;
            Assert.That(stage.Progress.expansion.Complete(9),Is.True);
            Assert.That(GameObject.Find("Ouzana").GetComponent<SpriteRenderer>().sprite.texture.name,Is.EqualTo("OuzanaBiologist"));
            yield return WalkToAndInteract("exit");Assert.That(Modal(stage,"_panel"),Is.EqualTo("complete"));
            yield return SceneManager.LoadSceneAsync("Menu_MisterioDeVarginha");
        }

        [UnityTest] public IEnumerator WorkshopSingleApplicationReloadAndDepartureRemainConsistent()
        {
            var story=Ready(10);var saved=story.expansion;saved.evidencePresented=true;saved.SolveSamples();saved.reagentCharges=0;
            CampaignStorySave.Write(story);yield return Load(10);
            var stage=CampaignExpansionController.Active;var state=stage.Progress.expansion;
            yield return WalkToAndInteract("spray2");Assert.That(state.sprayed,Is.Zero);stage.ClosePanel();
            yield return WalkToAndInteract("refill");Assert.That(state.reagentCharges,Is.EqualTo(6));stage.ClosePanel();
            yield return WalkToAndInteract("spray2");stage.ClosePanel();
            Assert.That(state.sprayed,Is.EqualTo(7));Assert.That(state.reagentCharges,Is.EqualTo(5));Assert.That(state.Complete(10),Is.True);
            stage.Interact("spray1");Assert.That(state.reagentCharges,Is.EqualTo(5));stage.ClosePanel();
            stage.Save();yield return Load(10);stage=CampaignExpansionController.Active;state=stage.Progress.expansion;
            var car=GameObject.Find("Fusca");
            for(int i=0;i<3;i++)Assert.That(car.transform.Find("Marca_"+i),Is.Not.Null);
            Assert.That(state.reagentCharges,Is.EqualTo(5));Assert.That(state.testDriven,Is.False);
            yield return WalkToAndInteract("exit");yield return new WaitForSeconds(3.4f);
            Assert.That(Modal(stage,"_panel"),Is.EqualTo("complete"));Assert.That(state.workshopDeparted,Is.True);
            Assert.That(car.transform.position.x,Is.GreaterThan(stage.Plan.bounds.xMax));
            stage.Save();Assert.That(CampaignStorySave.Load().expansion.workshopDeparted,Is.True);
            yield return SceneManager.LoadSceneAsync("Menu_MisterioDeVarginha");
        }

        [UnityTest] public IEnumerator PreFlashGlowAppearsBeforeEncounterAndDoesNotPersistAfterIt()
        {
            CampaignMemorySave.Write(new CampaignMemory{openingSeen=true,powerFailed=false});
            yield return SceneManager.LoadSceneAsync(VarginhaCampaignPhase1.SceneName);yield return new WaitForSeconds(.3f);
            var stage=VarginhaCampaignPhase1.Active;
            var glow=stage.transform.Find("Brilho_antes_do_clarao");Assert.That(glow.gameObject.activeSelf,Is.False);
            stage.TriggerPowerFailure();yield return null;Assert.That(glow.gameObject.activeSelf,Is.True);
            Assert.That(glow.position.x,Is.EqualTo(17.2f));
            var actor=Object.FindAnyObjectByType<EdelzioTopDownController>();actor.GetComponent<Rigidbody2D>().position=new Vector2(14,0);yield return new WaitForFixedUpdate();yield return null;yield return null;
            Assert.That(stage.IsExploring,Is.False);Assert.That(glow.gameObject.activeSelf,Is.False);
            yield return SceneManager.LoadSceneAsync("Menu_MisterioDeVarginha");
        }

        [UnityTest] public IEnumerator PlayerParksTheFuscaBeforeAnalyzingItAndArrivalSurvivesReload()
        {
            var story=Ready(10);var state=story.expansion;
            state.workshopParked=false;state.evidencePresented=true;state.labClues=7;state.sampleOrder=new[]{0,1,2};state.SolveSamples();CampaignStorySave.Write(story);
            yield return SceneManager.LoadSceneAsync(CampaignStorySave.Scene(10));yield return new WaitForSeconds(3);
            var stage=CampaignExpansionController.Active;var car=GameObject.Find("Fusca");var vehicle=car.GetComponent<CampaignWorkshopVehicle>();
            var actor=Object.FindAnyObjectByType<EdelzioTopDownController>(FindObjectsInactive.Include);
            Assert.That(actor.gameObject.activeSelf,Is.False);Assert.That(stage.Progress.expansion.workshopParked,Is.False);
            stage.Interact("spray0");Assert.That(stage.Progress.expansion.sprayed,Is.Zero,"Park first, analyze second");
            yield return KeyFor(Key.A,.18f);Assert.That(vehicle.Direction,Is.EqualTo(CampaignWorkshopVehicle.Facing.West));
            yield return KeyFor(Key.D,.18f);Assert.That(vehicle.Direction,Is.EqualTo(CampaignWorkshopVehicle.Facing.East));
            yield return KeyFor(Key.S,.1f);Assert.That(vehicle.Direction,Is.EqualTo(CampaignWorkshopVehicle.Facing.South));
            yield return KeyFor(Key.UpArrow,1);
            yield return new WaitForSeconds(.5f);stage.Save();var saved=CampaignStorySave.Load().expansion;
            Assert.That(saved.workshopHasCarPosition,Is.True);Vector2 savedPosition=new Vector2(saved.workshopCarX,saved.workshopCarY);
            yield return SceneManager.LoadSceneAsync(CampaignStorySave.Scene(10));yield return new WaitForSeconds(3);
            stage=CampaignExpansionController.Active;car=GameObject.Find("Fusca");vehicle=car.GetComponent<CampaignWorkshopVehicle>();
            Assert.That(Vector2.Distance(car.transform.position,savedPosition),Is.LessThan(.06f));
            float deadline=Time.realtimeSinceStartup+7;
            while(car.transform.position.y<vehicle.BayPosition.y-.48f)
            {
                Assert.That(Time.realtimeSinceStartup,Is.LessThan(deadline),"Garage entrance must admit the actual car collider");
                InputSystem.QueueStateEvent(_keyboard,new KeyboardState(Key.W));yield return null;
            }
            InputSystem.QueueStateEvent(_keyboard,new KeyboardState());yield return new WaitForSeconds(.65f);
            Assert.That(vehicle.CanPark,Is.True,"Vehicle stopped inside the bay");
            yield return KeyFor(Key.W,.06f);
            actor=Object.FindAnyObjectByType<EdelzioTopDownController>();Assert.That(actor,Is.Not.Null);
            Assert.That(stage.Progress.expansion.workshopParked,Is.True);stage.ClosePanel();
            Assert.That(car.GetComponent<BoxCollider2D>().Distance(actor.GetComponent<CircleCollider2D>()).isOverlapped,Is.False);
            yield return Load(10);Assert.That(CampaignExpansionController.Active.Progress.expansion.workshopParked,Is.True);
            Assert.That(Object.FindAnyObjectByType<EdelzioTopDownController>().GetComponent<SpriteRenderer>().sprite.bounds.size.y,Is.EqualTo(1.5f).Within(.04f));
            yield return SceneManager.LoadSceneAsync("Menu_MisterioDeVarginha");
        }
    }
}
