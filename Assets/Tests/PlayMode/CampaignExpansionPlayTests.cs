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
                // Arrival driving is exercised by the workshop vehicle test; this
                // regression checks the already parked investigation in each scene.
                if(phase==10){var saved=CampaignStorySave.Load();saved.expansion.workshopParked=true;CampaignStorySave.Write(saved);}
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
        [UnityTest] public IEnumerator CorrectChurchAnswerAndOuzanaWorkWithoutLegacyInspectionFlags()
        {
            var story=new CampaignStory {phase=8};
            story.expansion.anchorYear=1996;story.expansion.anchorSymbol=1;story.expansion.anchorRecord=23;
            CampaignStorySave.Write(story);
            yield return SceneManager.LoadSceneAsync(CampaignStorySave.Scene(8));
            yield return null;yield return new WaitForSeconds(.2f);
            var church=CampaignExpansionController.Active;
            Assert.That(church.Progress.expansion.anchorClues,Is.Zero);
            church.Interact("anchor");Assert.That(church.SubmitPuzzle(),Is.True);
            Assert.That(CampaignStorySave.Load().expansion.Complete(8),Is.True,"A correct answer must remain unlocked after save repair.");
            var actor=Object.FindAnyObjectByType<EdelzioTopDownController>();
            Assert.That(actor.GetComponent<SpriteRenderer>().sprite.bounds.size.y,Is.EqualTo(CampaignTeamEdelzio.ChurchStandingHeight).Within(.04f));
            Assert.That(actor.transform.localScale,Is.EqualTo(Vector3.one));
            Assert.That(actor.GetComponent<CircleCollider2D>().offset.y,Is.EqualTo(-.58f));
            yield return SceneManager.LoadSceneAsync(CampaignStorySave.Scene(9));
            yield return null;yield return new WaitForSeconds(.4f);
            var ouzana=GameObject.Find("Ouzana");
            Assert.That(ouzana.GetComponent<CampaignOuzanaBiologist>(),Is.Not.Null);
            var render=ouzana.GetComponent<SpriteRenderer>();
            Assert.That(render.sprite.texture.name,Is.EqualTo("OuzanaBiologist"));
            Assert.That(render.sprite.bounds.size.y,Is.EqualTo(CampaignTeamEdelzio.StandingHeight).Within(.04f));
            Assert.That(render.sprite.texture.isReadable,Is.False);
            Assert.That(ouzana.GetComponent<CircleCollider2D>(),Is.Not.Null);
            actor=Object.FindAnyObjectByType<EdelzioTopDownController>();
            Assert.That(actor.GetComponent<SpriteRenderer>().sprite.bounds.size.y,Is.EqualTo(CampaignTeamEdelzio.StandingHeight).Within(.04f),"Leaving the church restores Edelzio's normal size.");
            CampaignExpansionController.Active.Interact("ouzana");
            Assert.That(CampaignExpansionController.Active.Progress.expansion.evidencePresented,Is.True);
        }
        [UnityTest]public IEnumerator AdultHouseHygieneRespondsToKeyboardAndUnlocksTheBox()
        {
            CampaignStorySave.Write(new CampaignStory{phase=2,routine=14});
            yield return SceneManager.LoadSceneAsync(CampaignStorySave.Scene(2));yield return new WaitForSeconds(3.4f);
            var stage=VarginhaCampaignStage.Active;var actor=Object.FindAnyObjectByType<EdelzioTopDownController>();
            var map=GameObject.Find("Mapa_Campanha").transform;var data=CampaignIllustratedMaps.Get(2);
            var fridge=map.Find("02_Mobilia_Colisoes/Fridge_Kitchen").GetComponent<SpriteRenderer>();
            Assert.That(actor.GetComponent<SpriteRenderer>().bounds.size.y,Is.LessThan(fridge.bounds.size.y));
            Assert.That(actor.transform.localScale,Is.EqualTo(Vector3.one));
            Assert.That(map.Find("01_Planta_Paredes_Divisoes/Arte_Integrada_0").GetComponent<SpriteRenderer>().sprite.texture.name,Is.EqualTo("AdultEmpty"));
            Assert.That(map.Find("02_Mobilia_Colisoes/Sofa_LivingRoom").GetComponent<SpriteRenderer>().sprite.texture.name,Is.EqualTo("Adult2026"));
            var oldDoor=GameObject.Find("Door_Office_Accessible");if(oldDoor!=null){Assert.That(oldDoor.GetComponent<SpriteRenderer>().enabled,Is.False);foreach(var c in oldDoor.GetComponents<Collider2D>())Assert.That(c.enabled,Is.False);}
            Assert.That(actor.GetComponent<SpriteRenderer>().sharedMaterial.shader.name,Is.EqualTo("Varginha/IllustratedActorLighting"));
            var keyboard=InputSystem.AddDevice<Keyboard>();
            actor.GetComponent<Rigidbody2D>().position=data.Position(220,350)+Vector2.up*.58f;yield return new WaitForFixedUpdate();
            yield return HoldKey(keyboard,Key.S,.7f);
            Assert.That(actor.GetComponent<CircleCollider2D>().bounds.center.y,Is.LessThan(data.Position(220,430).y),"Walk through the bedroom portal in the supplied image.");
            actor.GetComponent<Rigidbody2D>().position=data.Position(416,550)+Vector2.up*.58f;yield return new WaitForFixedUpdate();
            Assert.That(actor.IsInputLocked,Is.False,"Office approach must remain interactive; HUD dialogue="+VarginhaGameHUD.Instance?.IsDialogueOpen);
            yield return HoldKey(keyboard,Key.D,.6f);
            Assert.That(actor.transform.position.x,Is.GreaterThan(data.Position(489,550).x),"Walk through the office portal.");
            actor.GetComponent<Rigidbody2D>().position=CampaignAdultHouse.WashApproach;
            yield return new WaitForFixedUpdate();yield return null;
            Assert.That(actor.GetComponent<CircleCollider2D>().Distance(map.Find("02_Mobilia_Colisoes/Kitchen_Cabinet").GetComponent<BoxCollider2D>()).isOverlapped,Is.False);
            yield return HoldKey(keyboard,Key.E,.1f);
            yield return new WaitForSeconds(1.2f);
            Assert.That(stage.Progress.routine,Is.EqualTo(15),"A reachable hygiene action completes the saved routine=14 state.");
            stage.CloseDialogue();stage.Interact("box");Assert.That(stage.Progress.boxFound,Is.True);
            yield return SceneManager.LoadSceneAsync("Menu_MisterioDeVarginha");
        }
        private static IEnumerator HoldKey(Keyboard keyboard,Key key,float duration)
        {
            // Queue a held hardware state each frame rather than mixing manual input updates
            // with the Editor's dynamic/fixed update buffers after a rigidbody teleport.
            float end=Time.time+duration;
            while(Time.time<end){InputSystem.QueueStateEvent(keyboard,new KeyboardState(key));yield return null;}
            InputSystem.QueueStateEvent(keyboard,new KeyboardState());yield return null;
        }
        [UnityTest] public IEnumerator IndustrialLibraryHasPhysicalPassagesAndReachableEvidence()
        {
            CampaignStorySave.Write(new CampaignStory{phase=6});yield return SceneManager.LoadSceneAsync(CampaignStorySave.Scene(6));yield return new WaitForSeconds(3);
            var actor=Object.FindAnyObjectByType<EdelzioTopDownController>();var body=actor.GetComponent<Rigidbody2D>();var keyboard=InputSystem.AddDevice<Keyboard>();
            var data=CampaignIllustratedMaps.Get(6);var stage=CampaignExpansionController.Active;
            Assert.That(data.image,Is.EqualTo("IndustrialLibrary"));
            Assert.That(CampaignCharacterShadows.OpenAir(data,stage.Plan.spawn),Is.False,"The school library is indoors.");
            // The entrance and rug are floor, while the central chair stops real movement.
            yield return WalkLibrary(stage.Plan,body,keyboard,stage.Plan.points.Find(p=>p.id=="map").position);
            var chair=GameObject.Find("Mapa_Campanha").transform.Find("02_Mobilia_Colisoes/Cadeira central sul meio").GetComponent<BoxCollider2D>();
            body.position=data.Position(480,507)+Vector2.up*.58f;body.linearVelocity=Vector2.zero;yield return new WaitForFixedUpdate();
            yield return HoldKey(keyboard,Key.W,.6f);
            Assert.That(actor.GetComponent<CircleCollider2D>().Distance(chair).distance,Is.GreaterThanOrEqualTo(-.01f),"The chair cannot be walked through (physics contact tolerance).");
            Assert.That(actor.GetComponent<CircleCollider2D>().bounds.center.y,Is.LessThanOrEqualTo(chair.bounds.min.y-.22f));
            // Probe the T-shaped exterior and the bookshelf using the same physical controller.
            body.position=data.Position(365,645)+Vector2.up*.58f;body.linearVelocity=Vector2.zero;yield return new WaitForFixedUpdate();
            yield return HoldKey(keyboard,Key.A,.6f);
            Assert.That(body.position.x-.23f,Is.GreaterThan(data.Position(347,645).x),"Vestibule wall closes the black external corner.");
            body.position=data.Position(115,502)+Vector2.up*.58f;body.linearVelocity=Vector2.zero;yield return new WaitForFixedUpdate();
            yield return HoldKey(keyboard,Key.W,.6f);
            Assert.That(body.position.y-.58f,Is.LessThan(data.Position(115,465).y),"The west bookshelf blocks the feet.");
            // Return to a real floor location, then walk the three clue routes without teleporting.
            body.position=stage.Plan.spawn+Vector2.up*.58f;body.linearVelocity=Vector2.zero;yield return new WaitForFixedUpdate();
            foreach(string id in new[]{"archive","school","square"})
            {
                yield return WalkLibrary(stage.Plan,body,keyboard,stage.Plan.points.Find(p=>p.id==id).position);
                yield return HoldKey(keyboard,Key.E,.1f);stage.ClosePanel();yield return null;
            }
            Assert.That(stage.Progress.expansion.visited,Is.EqualTo(7),"Each visible object responds to the interaction key.");
            yield return WalkLibrary(stage.Plan,body,keyboard,stage.Plan.points.Find(p=>p.id=="map").position);
            yield return HoldKey(keyboard,Key.E,.1f);
            stage.Progress.expansion.mapOrder=new[]{2,1,0};Assert.That(stage.SubmitPuzzle(),Is.False);
            stage.ClosePanel();stage.Interact("map");stage.Progress.expansion.mapOrder=new[]{0,1,2};Assert.That(stage.SubmitPuzzle(),Is.True);stage.ClosePanel();
            Assert.That(CampaignStorySave.Load().expansion.mapSolved,Is.True);
            yield return WalkLibrary(stage.Plan,body,keyboard,stage.Plan.points.Find(p=>p.id=="exit").position);
            yield return HoldKey(keyboard,Key.E,.1f);
            Assert.That(actor.IsInputLocked,Is.True,"The reached exit opens the completion panel.");
            yield return SceneManager.LoadSceneAsync("Menu_MisterioDeVarginha");
        }
        private static IEnumerator WalkLibrary(CampaignMapPlan plan,Rigidbody2D body,Keyboard keyboard,Vector2 target)
        {
            var path=new System.Collections.Generic.List<Vector2>();
            Assert.That(plan.Route(body.position-Vector2.up*.58f,target,path),Is.True,"Physical route to "+target);
            var corners=new System.Collections.Generic.List<Vector2>();
            for(int i=0;i<path.Count;i++)
                if(i==path.Count-1||i==0||(path[i]-path[i-1])!=(path[i+1]-path[i]))corners.Add(path[i]);
            corners.Add(target);
            foreach(var corner in corners)
            {
                float deadline=Time.time+8;
                while(Vector2.Distance(body.position-Vector2.up*.58f,corner)>.10f&&Time.time<deadline)
                {
                    var delta=corner-(body.position-Vector2.up*.58f);var keys=new System.Collections.Generic.List<Key>();
                    if(Mathf.Abs(delta.x)>.065f)keys.Add(delta.x>0?Key.D:Key.A);
                    if(Mathf.Abs(delta.y)>.065f)keys.Add(delta.y>0?Key.W:Key.S);
                    InputSystem.QueueStateEvent(keyboard,new KeyboardState(keys.ToArray()));yield return null;
                }
                InputSystem.QueueStateEvent(keyboard,new KeyboardState());yield return null;
                Assert.That(Vector2.Distance(body.position-Vector2.up*.58f,corner),Is.LessThan(.28f),"Blocked physical passage at "+corner);
            }
            yield return new WaitForFixedUpdate();yield return new WaitForFixedUpdate();
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
            var map=GameObject.Find("Mapa_Campanha").transform;var data=CampaignIllustratedMaps.Get(4);
            var renderer=map.Find("01_Planta_Paredes_Divisoes/Parede_7/Face_da_parede").GetComponent<SpriteRenderer>();Assert.That(renderer.enabled,Is.True);
            Assert.That(renderer.sprite.texture,Is.EqualTo(Resources.Load<Texture2D>("Varginha/IllustratedMaps/School")));
            Assert.That(renderer.bounds.size.x/renderer.bounds.size.y,Is.EqualTo(422f/187).Within(.001f),"Frontage crop retains its proportions.");
            var renan=GameObject.Find("Renan_Industrial_Campanha").GetComponent<Collider2D>();
            var plan=CampaignMapPlan.Create(4);
            foreach(var student in Object.FindObjectsByType<CampaignSchoolLife>())
            {
                var feet=student.GetComponent<CircleCollider2D>();var ground=(Vector2)student.transform.position+feet.offset;
                Assert.That(plan.IsClear(ground,feet.radius),Is.True,student.StudentName+" feet");
                Assert.That(feet.Distance(renan).isOverlapped,Is.False,student.StudentName+" overlaps Renan");
            }
            var actor=Object.FindAnyObjectByType<EdelzioTopDownController>();
            actor.GetComponent<Rigidbody2D>().position=data.Position(700,740)+Vector2.up*.58f;
            yield return new WaitForFixedUpdate();var gateKeyboard=InputSystem.AddDevice<Keyboard>();
            yield return HoldKey(gateKeyboard,Key.W,1.2f);
            Assert.That(actor.GetComponent<CircleCollider2D>().bounds.center.y,Is.GreaterThan(data.Position(700,640).y),"Player crosses the real gate instead of its painted walls.");
            Assert.That(renderer.enabled,Is.True,"Facade remains a depth-sorted wall face.");
            yield return new WaitForSeconds(1);
            foreach(var student in Object.FindObjectsByType<CampaignSchoolLife>())
            {
                Assert.That(student.Walks,Is.False,"Students stay seated while Renan teaches.");
                Assert.That((Vector2)student.transform.position,Is.EqualTo(student.Home));
                Assert.That(student.GetComponent<SpriteRenderer>().sprite.name,Does.StartWith("StudentSeated_"));
                var back=GameObject.Find("Encosto_"+student.name).GetComponent<SpriteRenderer>();
                Assert.That(back.enabled,Is.True);
                Assert.That(back.GetComponent<UnityEngine.Rendering.SortingGroup>().sortingOrder,Is.GreaterThan(student.GetComponent<UnityEngine.Rendering.SortingGroup>().sortingOrder));
            }
            Assert.That(renan.transform.position.y,Is.GreaterThan(data.Position(945,120).y));
            Assert.That(renan.GetComponent<SpriteRenderer>().sprite.name,Does.Contain("RenanTeaching"));
            Assert.That(GameObject.Find("Notebook_do_Renan"),Is.Not.Null);
            Assert.That(GameObject.Find("Mochila_cinza_do_Renan"),Is.Not.Null);
            Assert.That(GameObject.Find("Notebook_Campanha_Industrial"),Is.Null,"Use the real teacher laptop, without another floating prop.");
            var header=map.Find("01_Planta_Paredes_Divisoes/Portal_aberto_0").GetComponent<SpriteRenderer>();
            Assert.That(header.GetComponent<UnityEngine.Rendering.SortingGroup>().sortingOrder,
                Is.GreaterThan(actor.GetComponent<UnityEngine.Rendering.SortingGroup>().sortingOrder),"Entrance crossbar covers the player after walking under it.");
            var wall=map.Find("01_Planta_Paredes_Divisoes/Parede_11").GetComponent<BoxCollider2D>();
            actor.GetComponent<Rigidbody2D>().position=data.Position(1110,710)+Vector2.up*.58f;
            yield return new WaitForFixedUpdate();yield return HoldKey(gateKeyboard,Key.W,1);
            Assert.That(actor.GetComponent<CircleCollider2D>().Distance(wall).distance,Is.GreaterThanOrEqualTo(-.01f),"Closed facade corner blocks actual movement.");
            yield return SceneManager.LoadSceneAsync("Menu_MisterioDeVarginha");
        }
        [UnityTest]public IEnumerator CharacterActionsAndPunchUseNewTransparentAnimationSheets()
        {
            CampaignStorySave.Write(new CampaignStory{phase=6});
            yield return SceneManager.LoadSceneAsync(CampaignStorySave.Scene(6));yield return new WaitForSeconds(3);
            var actor=Object.FindAnyObjectByType<EdelzioTopDownController>();var actions=actor.GetComponent<VarginhaPlayerSpriteAnimation>();var renderer=actor.GetComponent<SpriteRenderer>();
            actions.SetSeatingFacing(Vector2.left);actions.SetSeatingFrame(1);yield return null;yield return null;
            Assert.That(renderer.sprite.name,Does.Contain("SeatedGray"));
            actions.SetCoffeeFrame(2);yield return null;yield return null;Assert.That(renderer.sprite.name,Does.Contain("EdelzioActions"));
            foreach(string pose in new[]{"Edelzio_Crouch","Edelzio_Reach"})
            {actions.SetActionPose(pose);yield return null;yield return null;Assert.That(renderer.sprite.name,Does.Contain("EdelzioInteractions"));Assert.That(renderer.sprite.name,Does.Contain("ComMochila"));}
            actions.ClearActionPose();yield return null;
            var attack=actor.GetComponent<VarginhaPlayerAttack>();Assert.That(attack,Is.Not.Null);
            Assert.That(attack.TryAttack(Vector2.right),Is.True);yield return new WaitForSeconds(.1f);
            Assert.That(renderer.sprite.name,Does.Contain("Team_V4_Punch"));yield return new WaitForSeconds(.4f);
            Assert.That(renderer.sprite.name,Does.Not.Contain("Team_V4_Punch"));
            yield return SceneManager.LoadSceneAsync("Menu_MisterioDeVarginha");
        }
        [UnityTest]public IEnumerator DailyAnimationsKeepEquipmentAndGroundCollisionWhileBackpackStillOpens()
        {
            CampaignStorySave.Write(new CampaignStory{phase=6});
            yield return SceneManager.LoadSceneAsync(CampaignStorySave.Scene(6));yield return new WaitForSeconds(3);
            var actor=Object.FindAnyObjectByType<EdelzioTopDownController>();var renderer=actor.GetComponent<SpriteRenderer>();
            Assert.That(renderer.sprite.name,Does.Contain("LifeGray"));Assert.That(renderer.sprite.texture.isReadable,Is.False);
            var poses=actor.GetComponent<VarginhaPlayerSpriteAnimation>();
            Vector2[] directions={Vector2.down,new(-1,-1),Vector2.left,new(-1,1),Vector2.up,new(1,1),Vector2.right,new(1,-1)};
            for(int i=0;i<directions.Length;i++)
            {
                poses.SetSeatingFacing(directions[i]);poses.SetSeatingFrame(1);yield return null;yield return null;
                Assert.That(renderer.sprite.name,Does.Contain("SeatedGray_"+i+"_"));
            }
            poses.ClearActionPose();
            var motion=actor.GetComponent<VarginhaPlayerActionAnimation>();motion.StartCoroutine(motion.WashFaceRoutine());
            yield return new WaitForSeconds(.35f);
            Assert.That(renderer.sprite.name,Does.Contain("LifeGray_3_9"));Assert.That(actor.IsInputLocked,Is.True);
            yield return new WaitForSeconds(.8f);Assert.That(motion.IsActing,Is.False);
            Assert.That(poses.HasActionPose,Is.False);Assert.That(actor.IsInputLocked,Is.False,"Daily action restores movement before jumping.");
            var body=actor.GetComponent<Rigidbody2D>();var before=body.position;var keyboard=InputSystem.AddDevice<Keyboard>();
            InputSystem.QueueStateEvent(keyboard,new KeyboardState(VarginhaInputBindings.GetKeyboard(VarginhaInputAction.Jump)));yield return null;
            Assert.That(keyboard[VarginhaInputBindings.GetKeyboard(VarginhaInputAction.Jump)].isPressed,Is.True,"Virtual keyboard received the jump key.");
            Assert.That(VarginhaInputBindings.IsPressed(VarginhaInputAction.Jump),Is.True,"Jump binding uses the current keyboard: "+Keyboard.current?.name);
            yield return null;
            var team=actor.GetComponent<CampaignTeamEdelzio>();
            Assert.That(team.IsJumping,Is.True,"The remapped keyboard binding starts the hop.");
            InputSystem.QueueStateEvent(keyboard,new KeyboardState());yield return null;
            Assert.That(Vector2.Distance(body.position,before),Is.LessThan(.015f),"The visual hop does not move the physical floor contact.");
            yield return new WaitForSeconds(.55f);Assert.That(actor.GetComponent<CampaignTeamEdelzio>().IsJumping,Is.False);
            Assert.That(CampaignExpansionController.Active.OpenBackpack(),Is.True);yield return null;
            Assert.That(VarginhaGameHUD.Instance.IsInventoryOpen,Is.True);VarginhaGameHUD.Instance.CloseBackpack();yield return null;
            yield return SceneManager.LoadSceneAsync("Menu_MisterioDeVarginha");
        }
    }
}
