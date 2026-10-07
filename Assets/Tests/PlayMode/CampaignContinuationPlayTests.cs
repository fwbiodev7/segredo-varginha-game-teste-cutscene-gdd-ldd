using System.Collections;
using System.IO;
using System.Linq;
using Game.Player;
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
    public class CampaignContinuationPlayTests : InputTestFixture
    {
        public override void Setup(){VarginhaInputActions.Shutdown();base.Setup();}
        public override void TearDown(){VarginhaInputActions.Shutdown();base.TearDown();}
        string saved;bool background;
        [SetUp] public void PreserveSave(){saved=File.Exists(CampaignStorySave.Path)?File.ReadAllText(CampaignStorySave.Path):null;background=Application.runInBackground;Application.runInBackground=true;}
        [TearDown] public void RestoreSave(){Time.timeScale=1;Application.runInBackground=background;if(saved!=null)File.WriteAllText(CampaignStorySave.Path,saved);else if(File.Exists(CampaignStorySave.Path))File.Delete(CampaignStorySave.Path);}
        [UnityTearDown] public IEnumerator LeaveScene(){Time.timeScale=1;yield return SceneManager.LoadSceneAsync("Menu_MisterioDeVarginha");}
        IEnumerator Load(int phase,int area=0)
        {
            yield return SceneManager.LoadSceneAsync(CampaignContinuationDefinition.SceneName(phase,area));
            Debug.Log("QA continuation scene loaded: "+phase+" / "+area);
            yield return new WaitForSeconds(2.3f);CampaignContinuationController.Active.CloseMessage();yield return null;
            yield return new WaitUntil(()=>!CampaignContinuationController.Active.BattleEntrancePlaying);
            // A resumed battle shows its controls after the chapter introduction.
            CampaignContinuationController.Active.CloseMessage();yield return null;
            Debug.Log("QA continuation ready: "+phase+" / "+area);
        }
        [UnityTest,Timeout(90000)] public IEnumerator ManifestationCinematicsFreezeOnPauseRestoreCameraAndRequireCalibration()
        {
            bool originalMotion=VarginhaGameSettings.Current.reducedMotion;
            try
            {
                foreach(bool reduced in new[]{false,true})
                {
                    VarginhaGameSettings.Current.reducedMotion=reduced;
                    var story=new CampaignStory{phase=20};CampaignStorySave.Write(story);yield return Load(20);
                    var c=CampaignContinuationController.Active;var camera=Camera.main;
                    float size=camera.orthographicSize;var follow=camera.GetComponent<Game.Level.CameraFollow2D>();
                    c.State.chambersPrepared=7;c.StartBattle();yield return null;
                    var boss=Object.FindObjectsByType<CampaignManifestationCombat>().First(b=>!b.IsMinor);
                    Assert.That(c.BattleEntrancePlaying,Is.True);Assert.That(c.Actor.IsInputLocked,Is.True);
                    Assert.That(boss.GetComponent<VarginhaCombatTarget>().ReceiveHit(1000,Vector2.right,0),Is.False);
                    yield return new WaitUntil(()=>boss.CinematicProgress>.7f);
                    var body=boss.transform.Find("Cinematica_da_manifestacao/Corpo_em_pixelart").GetComponent<SpriteRenderer>();
                    Assert.That(body.sprite.texture.name,Is.EqualTo("BossManifestationCinematicV1"));
                    Assert.That(body.sprite.texture.filterMode,Is.EqualTo(FilterMode.Point));
                    Assert.That(boss.transform.Find("Cinematica_da_manifestacao").childCount,Is.EqualTo(reduced?2:26));
                    Assert.That(camera.orthographicSize,reduced?Is.EqualTo(size):Is.LessThanOrEqualTo(size));
                    if(!reduced)CaptureBossFrame("Entrada.png",camera);
                    c.TogglePause();float progress=boss.CinematicProgress;var pose=body.sprite;var position=camera.transform.position;
                    yield return new WaitForSecondsRealtime(.3f);
                    Assert.That(boss.CinematicProgress,Is.EqualTo(progress));Assert.That(body.sprite,Is.SameAs(pose));
                    Assert.That(Vector3.Distance(position,camera.transform.position),Is.LessThan(.001f));
                    c.TogglePause();yield return new WaitUntil(()=>!c.BattleEntrancePlaying);c.CloseMessage();yield return null;
                    Assert.That(follow.enabled,Is.True);Assert.That(camera.orthographicSize,Is.EqualTo(size).Within(.01f));
                    Assert.That(boss.GetComponent<Rigidbody2D>().simulated,Is.True);Assert.That(CampaignFinalAllies.Active.CombatActive,Is.True);
                    boss.enabled=false;boss.Stagger(10);boss.GetComponent<HealthSystem>().SetInvincibilityDuration(0);
                    Assert.That(boss.GetComponent<VarginhaCombatTarget>().ReceiveHit(9999,Vector2.right,0),Is.True);
                    yield return null;yield return null;
                    Assert.That(c.BattleExitPlaying,Is.True);Assert.That(c.Actor.IsInputLocked,Is.True);
                    Assert.That(c.State.manifestationDispelled,Is.True);Assert.That(CampaignStorySave.Load().continuation.manifestationDispelled,Is.True);
                    Assert.That(CampaignFinalAllies.Active.CombatActive,Is.False);
                    c.State.finalRegulators=new[]{1,0,1};Assert.That(c.SubmitCalibration(),Is.False,"The final panel must wait for the cinematic.");
                    yield return new WaitUntil(()=>boss.CinematicProgress>.48f);
                    if(!reduced)CaptureBossFrame("Saida.png",camera);
                    c.TogglePause();progress=boss.CinematicProgress;yield return new WaitForSecondsRealtime(.2f);
                    Assert.That(boss.CinematicProgress,Is.EqualTo(progress));c.TogglePause();
                    yield return new WaitUntil(()=>!c.BattleExitPlaying);c.CloseMessage();yield return null;
                    Assert.That(follow.enabled,Is.True);Assert.That(camera.orthographicSize,Is.EqualTo(size).Within(.01f));
                    Assert.That(boss.transform.Find("Cinematica_da_manifestacao"),Is.Null);Assert.That(boss.ActiveMinions,Is.Zero);
                    Assert.That(boss.GetComponents<Collider2D>().All(x=>!x.enabled),Is.True);
                    Assert.That(c.State.solved[9],Is.False);Assert.That(c.State.finalCalibrated,Is.False);
                    Assert.That(c.SubmitCalibration(),Is.True);
                    yield return Load(20);Assert.That(Object.FindObjectsByType<CampaignManifestationCombat>().Length,Is.Zero,"Reload must not repeat the defeated manifestation.");
                }
            }
            finally{VarginhaGameSettings.Current.reducedMotion=originalMotion;Time.timeScale=1;}
        }
        static void CaptureBossFrame(string name,Camera camera)
        {
            string folder=Path.Combine(Directory.GetCurrentDirectory(),"Docs","QABossCinematic20261005");Directory.CreateDirectory(folder);
            var previous=camera.targetTexture;var active=RenderTexture.active;
            var target=RenderTexture.GetTemporary(960,720,24);var image=new Texture2D(960,720,TextureFormat.RGB24,false);
            try
            {
                camera.targetTexture=target;camera.Render();RenderTexture.active=target;image.ReadPixels(new Rect(0,0,960,720),0,0);image.Apply();
                File.WriteAllBytes(Path.Combine(folder,name),image.EncodeToPNG());
            }
            finally{camera.targetTexture=previous;RenderTexture.active=active;RenderTexture.ReleaseTemporary(target);Object.Destroy(image);}
        }
        [UnityTest,Timeout(90000)] public IEnumerator BossAndChildrenHurtboxesFollowCrouchesWithoutIncludingClawTrails()
        {
            CampaignStorySave.Write(new CampaignStory{phase=20});yield return Load(20);
            var c=CampaignContinuationController.Active;
            var boss=CampaignManifestationCombat.Spawn(c.transform,c.Actor,c.Plan,new Vector2(0,2));
            var child=CampaignManifestationCombat.Spawn(c.transform,c.Actor,c.Plan,new Vector2(3,2),true);
            boss.enabled=child.enabled=false;
            var pose=typeof(CampaignManifestationCombat).GetMethod("Pose",System.Reflection.BindingFlags.Instance|System.Reflection.BindingFlags.NonPublic);
            foreach(var enemy in new[]{boss,child})
            {
                var hurt=enemy.GetComponent<CapsuleCollider2D>();var feet=enemy.GetComponent<CircleCollider2D>();
                Assert.That(hurt.isTrigger,Is.True);Assert.That(feet.isTrigger,Is.False);
                Assert.That(enemy.GetComponent<Rigidbody2D>().interpolation,Is.EqualTo(RigidbodyInterpolation2D.Interpolate));
                for(int direction=0;direction<4;direction++)
                {
                    pose.Invoke(enemy,new object[]{direction,0,false});Vector2 standing=hurt.size;
                    pose.Invoke(enemy,new object[]{direction,2,false});Assert.That(hurt.size.y,Is.LessThan(standing.y),"Crouch lowers the body.");
                    for(int frame=0;frame<8;frame++)
                    {
                        pose.Invoke(enemy,new object[]{direction,frame,false});
                        Assert.That(hurt.size.x,Is.EqualTo(standing.x).Within(.001f));
                        Assert.That(hurt.size.y,Is.LessThanOrEqualTo(standing.y+.001f),"Raised claws never enlarge the body.");
                        Assert.That(hurt.offset.y-hurt.size.y*.5f,Is.EqualTo(-.52f).Within(.001f),"The lower body stays anchored above the feet.");
                    }
                    for(int frame=0;frame<6;frame++)
                    {
                        pose.Invoke(enemy,new object[]{direction,frame,true});
                        Assert.That(hurt.size.x,Is.EqualTo(standing.x).Within(.001f));
                        Assert.That(hurt.size.y,Is.LessThanOrEqualTo(standing.y+.001f));
                    }
                    pose.Invoke(enemy,new object[]{direction,4,false});Physics2D.SyncTransforms();
                    Vector2 outside=(Vector2)enemy.transform.position+Vector2.up*hurt.offset.y+Vector2.right*(hurt.size.x*.5f+.2f);
                    Assert.That(hurt.OverlapPoint(outside),Is.False,"Energy around the claws is outside the hurtbox.");
                }
                pose.Invoke(enemy,new object[]{0,4,true});
            }
            var camera=Camera.main;var follow=camera.GetComponent<Game.Level.CameraFollow2D>();follow.enabled=false;
            camera.orthographicSize=4.4f;camera.transform.position=new Vector3(1.2f,3,camera.transform.position.z);
            yield return null;CaptureBossFrame("Combate.png",camera);
        }
        [UnityTest,Timeout(90000)] public IEnumerator InterpolatedMovementAndShieldRepulsionUseThePhysicsPosition()
        {
            CampaignStorySave.Write(new CampaignStory{phase=20});yield return Load(20);
            var c=CampaignContinuationController.Active;c.enabled=false;c.Actor.enabled=false;c.Actor.SetInputLocked(false);
            var playerBody=c.Actor.GetComponent<Rigidbody2D>();playerBody.position=new Vector2(20,.58f);
            var root=new GameObject("QA_Movimento_manifestacao");
            var plan=new CampaignMapPlan{bounds=new Rect(-50,-50,100,100)};
            foreach(bool minor in new[]{false,true})
            {
                var enemy=CampaignManifestationCombat.Spawn(root.transform,c.Actor,plan,Vector2.zero,minor);
                var body=enemy.GetComponent<Rigidbody2D>();yield return null;yield return new WaitForFixedUpdate();
                Vector2 start=body.position;int ticks=20;
                for(int i=0;i<ticks;i++)yield return new WaitForFixedUpdate();
                float expected=(minor?2.3f:1.2f)*ticks*Time.fixedDeltaTime;
                Assert.That(body.position.x-start.x,Is.EqualTo(expected).Within(.04f),"Interpolation must not shorten or jitter physics steps.");
                enemy.Repel(body.position+Vector2.right, .5f);float before=body.position.x;
                yield return new WaitForFixedUpdate();yield return new WaitForFixedUpdate();
                Assert.That(body.position.x,Is.LessThan(before-.35f),"The shield moves the creature away even while it pursues Edelzio.");
                c.Actor.SetInputLocked(true);yield return null;Vector2 stopped=body.position;
                yield return new WaitForSeconds(.12f);Assert.That(Vector2.Distance(stopped,body.position),Is.LessThan(.001f));
                c.Actor.SetInputLocked(false);Object.Destroy(enemy.gameObject);yield return null;
            }
            Object.Destroy(root);
        }
        [UnityTest,Timeout(90000)] public IEnumerator NaturalBossAndChildAttacksWarnThenDealOneHitAndRecover()
        {
            CampaignStorySave.Write(new CampaignStory{phase=20});yield return Load(20);
            var c=CampaignContinuationController.Active;c.enabled=false;c.Actor.enabled=false;c.Actor.SetInputLocked(false);
            var playerBody=c.Actor.GetComponent<Rigidbody2D>();var health=c.Actor.GetComponent<HealthSystem>();
            // With the controller disabled, no FixedUpdate cancels velocity imparted by
            // the previous enemy. Keep this deliberately stationary target stationary.
            playerBody.constraints=RigidbodyConstraints2D.FreezeAll;playerBody.linearVelocity=Vector2.zero;
            health.SetMaxHealth(2000,false);health.SetInvincibilityDuration(0);
            var root=new GameObject("QA_Ataques_manifestacao");var plan=new CampaignMapPlan{bounds=new Rect(-50,-50,100,100)};
            foreach(bool minor in new[]{false,true})
            {
                playerBody.position=new Vector2(1.3f,.58f);yield return new WaitForFixedUpdate();
                var enemy=CampaignManifestationCombat.Spawn(root.transform,c.Actor,plan,Vector2.zero,minor);
                var warning=root.transform.Find("Aviso_de_ataque").GetComponent<SpriteRenderer>();
                var poses=new System.Collections.Generic.HashSet<string>();float deadline=Time.time+6;
                while(!warning.enabled&&Time.time<deadline){poses.Add(enemy.GetComponent<SpriteRenderer>().sprite.name);yield return null;}
                Assert.That(warning.enabled,Is.True,"The floor warning precedes the attack.");
                float before=health.CurrentHealth;yield return new WaitForSeconds(.55f);
                Assert.That(health.CurrentHealth,Is.EqualTo(before),"Preparation never deals contact damage.");
                Assert.That(enemy.CanReceiveHit,Is.EqualTo(minor),"The boss is protected while preparing; children remain vulnerable.");
                deadline=Time.time+2;
                while(health.CurrentHealth>=before&&Time.time<deadline){poses.Add(enemy.GetComponent<SpriteRenderer>().sprite.name);yield return null;}
                Assert.That(health.CurrentHealth,Is.EqualTo(before-(minor?10:24)),"The actual lunge connects exactly once.");
                yield return new WaitForSeconds(.55f);
                Assert.That(health.CurrentHealth,Is.EqualTo(before-(minor?10:24)),"Remaining close during recovery adds no hits.");
                Assert.That(enemy.CanReceiveHit,Is.True,"Recovery opens the boss to the player's counterattack.");
                Assert.That(warning.enabled,Is.False);
                Assert.That(poses.Any(p=>p.StartsWith(minor?"EchoFlow_":"BossFlow_")),Is.True,"The live attack uses intermediate poses.");
                var body=enemy.GetComponent<Rigidbody2D>();Vector2 recovered=body.position;
                yield return new WaitForSeconds(.2f);Assert.That(Vector2.Distance(recovered,body.position),Is.LessThan(.001f),"Recovery does not retain the dash.");
                Object.Destroy(enemy.gameObject);yield return null;yield return null;
            }
            Object.Destroy(root);
        }
        [UnityTest,Timeout(180000)] public IEnumerator InvestigationAreasRemainReachableAndPuzzlesPreserveTheirSolutions()
        {
            var route=new System.Collections.Generic.List<Vector2>();
            foreach(int phase in new[]{11,12,13,14,15,18})
            {
                CampaignStorySave.Write(new CampaignStory{phase=phase});
                int areas=phase==12?2:1;
                for(int area=0;area<areas;area++)
                {
                    var progress=CampaignStorySave.Load();progress.continuation.area=area;CampaignStorySave.Write(progress);
                    yield return Load(phase,area);var c=CampaignContinuationController.Active;
                    Assert.That(c.Plan.IsClear(c.Plan.spawn),Is.True,"Spawn "+phase+"/"+area);
                    foreach(var point in c.Plan.points)
                    {
                        if(point.id.StartsWith("student"))continue; // Seated optional conversations use their chair contact.
                        Assert.That(c.Plan.Route(c.Plan.spawn,point.position,route),Is.True,"Reachable "+phase+"/"+area+" "+point.id);
                    }
                    var map=c.transform.Find("Mapa_Campanha");
                    Assert.That(map.GetComponentsInChildren<Collider2D>().Length,Is.GreaterThan(0));
                    foreach(var renderer in map.GetComponentsInChildren<SpriteRenderer>())
                        if(renderer.sprite.name.StartsWith("Illustrated_"))Assert.That(renderer.sprite.texture.filterMode,Is.EqualTo(FilterMode.Point),renderer.name);
                    foreach(var name in new[]{"Padre Fábio","Ouzana","Renan"})
                    {
                        var npc=c.transform.Find(name);if(npc==null||phase==11||phase==14)continue;
                        Assert.That(c.Plan.IsClear((Vector2)npc.position-Vector2.up*.58f,.3f),Is.True,"NPC floor "+phase+" "+name);
                    }
                    if(phase==11){c.Interact("renan");c.CloseMessage();}
                    if(phase==14){c.Interact("fabio");c.CloseMessage();}
                    var definition=CampaignContinuationDefinition.Get(phase,area);
                    foreach(var id in definition.ids){c.Interact(id);c.CloseMessage();}
                    if(phase==12&&area==0)
                    {
                        c.Interact("entrance");yield return new WaitUntil(()=>CampaignContinuationController.Active!=null&&CampaignContinuationController.Active.area==1);
                        yield return new WaitForSeconds(2.3f);CampaignContinuationController.Active.CloseMessage();
                    }
                }
                var current=CampaignContinuationController.Active;
                var finalDefinition=CampaignContinuationDefinition.Get(phase,current.area);
                Assert.That(current.Submit(0,System.Linq.Enumerable.Range(0,finalDefinition.cards.Length).ToArray()),Is.True,"Puzzle "+phase);
                yield return new WaitForSeconds(3.1f);current.CloseMessage();
                Assert.That(current.State.solved[phase-11],Is.True);
                current.Interact(phase==12?"service":"exit");
                Debug.Log("QA investigation passed: "+phase);
            }
        }
        [UnityTest,Timeout(90000)] public IEnumerator ArenaNavigationSpawnPoolAndDefeatAllowRetry()
        {
            CampaignStorySave.Write(new CampaignStory{phase=20});yield return Load(20);
            var c=CampaignContinuationController.Active;var keyboard=InputSystem.AddDevice<Keyboard>();
            Vector2 start=c.Actor.transform.position;
            InputSystem.QueueStateEvent(keyboard,new KeyboardState(Key.W));yield return new WaitForSeconds(1.2f);
            InputSystem.QueueStateEvent(keyboard,new KeyboardState());yield return null;
            Assert.That(c.Actor.transform.position.y,Is.GreaterThan(start.y+2),"The southern stairs reach the arena through physical movement.");
            c.Actor.GetComponent<Rigidbody2D>().position=new Vector2(-11.7f,.58f);yield return new WaitForFixedUpdate();
            InputSystem.QueueStateEvent(keyboard,new KeyboardState(Key.A));yield return new WaitForSeconds(.8f);
            InputSystem.QueueStateEvent(keyboard,new KeyboardState());yield return null;
            Assert.That(c.Feet.x,Is.GreaterThan(-12.6f),"The arena edge must remain solid.");
            c.Actor.GetComponent<Rigidbody2D>().position=new Vector2(0,.58f);
            foreach(var id in new[]{"tree","river","chapel"}){c.Interact(id);c.CloseMessage();}yield return null;
            Assert.That(c.BattleEntrancePlaying,Is.True);Assert.That(c.Actor.IsInputLocked,Is.True);
            yield return new WaitForSeconds(1);
            var entranceBoss=Object.FindAnyObjectByType<CampaignManifestationCombat>();
            var framed=Camera.main.WorldToViewportPoint(entranceBoss.GetComponent<SpriteRenderer>().bounds.center);
            Assert.That(framed.x,Is.InRange(.15f,.85f));Assert.That(framed.y,Is.InRange(.15f,.85f),"The camera shows the boss even when Edelzio starts at the south stairs.");
            yield return new WaitUntil(()=>!c.BattleEntrancePlaying);c.CloseMessage();yield return null;
            var boss=Object.FindAnyObjectByType<CampaignManifestationCombat>();boss.enabled=false;
            Assert.That(boss.IsMinor,Is.False);Assert.That(boss.CanReceiveHit,Is.False);
            Assert.That(boss.GetComponent<VarginhaCombatTarget>().ReceiveHit(10,Vector2.right,0),Is.False);
            boss.Stagger(8);var bossHealth=boss.GetComponent<HealthSystem>();float bossBefore=bossHealth.CurrentHealth;
            c.Actor.GetComponent<Rigidbody2D>().position=(Vector2)boss.transform.position+Vector2.up*2.7f;yield return new WaitForFixedUpdate();
            Assert.That(c.Actor.GetComponent<VarginhaPlayerAttack>().TryAttack(Vector2.down),Is.True);yield return new WaitForSeconds(.5f);
            Assert.That(bossHealth.CurrentHealth,Is.LessThan(bossBefore),"A punch at the visible upper body reaches the boss hurtbox.");
            c.Actor.GetComponent<Rigidbody2D>().position=new Vector2(0,.58f);yield return new WaitForFixedUpdate();
            for(int i=0;i<5;i++)Assert.That(boss.SpawnMinion(),Is.True);
            Assert.That(boss.SpawnMinion(),Is.False);Assert.That(boss.ActiveMinions,Is.EqualTo(5));
            var children=Object.FindObjectsByType<CampaignManifestationCombat>();
            CampaignManifestationCombat minor=null;foreach(var e in children){if(e.IsMinor){e.enabled=false;minor=e;}}
            Assert.That(boss.GetComponent<SpriteRenderer>().sprite.bounds.size.y,Is.GreaterThan(minor.GetComponent<SpriteRenderer>().sprite.bounds.size.y*2));
            minor.GetComponent<Rigidbody2D>().position=new Vector2(3,.58f);c.Actor.GetComponent<Rigidbody2D>().position=new Vector2(2.1f,1.23f);yield return new WaitForFixedUpdate();
            float minorBefore=minor.GetComponent<HealthSystem>().CurrentHealth;
            Assert.That(c.Actor.GetComponent<VarginhaPlayerAttack>().TryAttack(Vector2.right),Is.True);yield return new WaitForSeconds(.5f);
            Assert.That(minor.GetComponent<HealthSystem>().CurrentHealth,Is.LessThan(minorBefore),"Punches at a child's upper body also connect.");
            minor.GetComponent<VarginhaCombatTarget>().ReceiveHit(100,Vector2.right,0);minor.enabled=true;yield return null;
            Assert.That(boss.SpawnMinion(),Is.True,"A defeated echo is safely reset and reused.");
            Assert.That(boss.ActiveMinions,Is.EqualTo(5));
            c.TogglePause();float hp=boss.HealthPercent;yield return new WaitForSecondsRealtime(.25f);
            Assert.That(boss.HealthPercent,Is.EqualTo(hp));Assert.That(CampaignFinalAllies.Active.InvokeSupport(0),Is.False);c.TogglePause();
            c.Actor.GetComponent<HealthSystem>().SetInvincibilityDuration(0);c.Actor.GetComponent<HealthSystem>().TakeDamage(1000);yield return null;
            Assert.That(VarginhaGameHUD.Instance.IsGameOver,Is.True);Assert.That(Time.timeScale,Is.Zero);
            VarginhaGameOverFlow.RetryCurrentPhase();yield return new WaitForSeconds(2.4f);
            Assert.That(CampaignContinuationController.Active.Actor.GetComponent<HealthSystem>().IsDead,Is.False);
            Assert.That(VarginhaGameHUD.Instance.IsGameOver,Is.False);
        }
        [UnityTest,Timeout(90000)] public IEnumerator AllStudentsHaveTwoTimedAttacksAndSupportSkillsWork()
        {
            var story=new CampaignStory{phase=20};story.continuation.chambersPrepared=7;CampaignStorySave.Write(story);yield return Load(20);
            var c=CampaignContinuationController.Active;var support=CampaignFinalAllies.Active;
            Debug.Log("QA skills: controller ready");
            var boss=Object.FindAnyObjectByType<CampaignManifestationCombat>();boss.enabled=false;
            var health=boss.GetComponent<HealthSystem>();health.SetMaxHealth(10000,false);boss.Stagger(200);
            Debug.Log("QA skills: target prepared");
            c.Actor.GetComponent<Rigidbody2D>().position=new Vector2(0,.58f);boss.GetComponent<Rigidbody2D>().position=new Vector2(3,.58f);yield return new WaitForFixedUpdate();
            Assert.That(support.Squad.Allies.Count,Is.EqualTo(9));
            var priest=GameObject.Find("Padre Fábio_Apoio").GetComponent<SpriteRenderer>().sprite;
            Assert.That(priest.bounds.size.y,Is.GreaterThan(VarginhaReferenceSprites.PadreFabio().bounds.size.y*1.6f),"The arena priest has the same scale as the adult allies.");
            Assert.That(VarginhaReferenceSprites.PadreFabio().pixelsPerUnit,Is.EqualTo(44),"The church's shared priest sprite keeps its original size.");
            foreach(var ally in support.Squad.Allies)
            {
                ally.ActivateManual(c.Actor.transform,0,.1f);
                Debug.Log("QA skills: activated "+ally.StudentName);
                for(int variant=0;variant<2;variant++)
                {
                    float before=health.CurrentHealth;Assert.That(ally.TryManualAttack(boss.GetComponent<VarginhaCombatTarget>()),Is.True,ally.StudentName);
                    Debug.Log("QA skills: attack "+ally.StudentName+" / "+variant);
                    Assert.That(ally.AttackVariant,Is.EqualTo(variant),ally.StudentName);Assert.That(health.CurrentHealth,Is.EqualTo(before),"Preparation must not apply damage.");
                    yield return new WaitForSeconds(.15f);
                    Assert.That(health.CurrentHealth,Is.EqualTo(before));
                    Assert.That(ally.GetComponent<VarginhaAllyAttackPresentation>().IsPresenting,Is.True);
                    var presentation=GameObject.Find("Animacao_"+ally.StudentName);
                    var actorOrder=boss.GetComponent<UnityEngine.Rendering.SortingGroup>().sortingOrder;
                    foreach(var fx in presentation.GetComponentsInChildren<SpriteRenderer>())
                        if(fx.name!="Sombra_do_aluno"&&fx.name!="Area_do_golpe")Assert.That(fx.sortingOrder,Is.GreaterThan(actorOrder),"Attack effects must render above the boss.");
                    Assert.That(System.Array.FindAll(presentation.GetComponentsInChildren<SpriteRenderer>(),x=>x.name=="Particula").Length,Is.InRange(20,32),"The authored effects have a bounded, expanded particle budget.");
                    Assert.That(presentation.transform.Find("Area_do_golpe").GetComponent<SpriteRenderer>().bounds.extents.x,Is.GreaterThan(2.5f),"The footprint contains the effects and particles.");
                    yield return new WaitForSeconds(1.8f);
                    Assert.That(presentation.GetComponents<AudioSource>().Length,Is.EqualTo(1),"The pooled animation reuses its impact audio source.");
                    Assert.That(health.CurrentHealth,Is.LessThan(before),ally.StudentName+" attack "+variant);
                    Assert.That(CampaignHeroArt.Pose(ally.StudentName,variant,false,0),Is.Not.Null);
                    Assert.That(CampaignHeroArt.Effect(ally.StudentName,variant,1),Is.Not.Null);
                }
            }
            float supportHP=health.CurrentHealth;Assert.That(support.InvokeSupport(0),Is.True);Assert.That(support.InvokeSupport(0),Is.False);
            yield return new WaitForSeconds(1.4f);Assert.That(health.CurrentHealth,Is.LessThan(supportHP));Assert.That(boss.CanReceiveHit,Is.True);
            supportHP=health.CurrentHealth;Assert.That(support.InvokeSupport(1),Is.True);yield return new WaitForSeconds(3.7f);
            Assert.That(health.CurrentHealth,Is.LessThan(supportHP-24),"The reagent cloud continues damaging after impact.");
            Assert.That(support.InvokeSupport(2),Is.True);yield return new WaitForSeconds(.45f);
            Assert.That(Object.FindObjectsByType<CampaignCharacterShadow>().Length,Is.LessThan(32),"Support shadow visuals must not generate further character shadows.");
            Assert.That(support.DamageMultiplier,Is.EqualTo(.45f));
            c.Actor.GetComponent<Rigidbody2D>().position=new Vector2(-4,.58f);yield return new WaitForFixedUpdate();
            Assert.That(support.DamageMultiplier,Is.EqualTo(1),"The priest's protection has a bounded area.");
            boss.Stagger(100);health.SetInvincibilityDuration(0);boss.GetComponent<VarginhaCombatTarget>().ReceiveHit(20000,Vector2.right,0);yield return null;
            Assert.That(c.State.manifestationDispelled,Is.True);Assert.That(c.State.solved[9],Is.False);Assert.That(support.CombatActive,Is.False);
            Assert.That(CampaignStorySave.Load().continuation.manifestationDispelled,Is.True);
            yield return Load(20);c=CampaignContinuationController.Active;
            Assert.That(Object.FindAnyObjectByType<CampaignManifestationCombat>(),Is.Null,"Saved victory must never replay the boss.");
            Assert.That(c.SubmitReturnCalibration(),Is.False);c.CloseMessage();
            c.State.finalRegulators=new[]{1,0,1};Assert.That(c.SubmitReturnCalibration(),Is.True);c.CloseMessage();
            yield return Load(20);c=CampaignContinuationController.Active;Assert.That(c.State.CanReturn,Is.True);
            CampaignStorySave.GoTo(21);yield return new WaitForSeconds(3.2f);c=CampaignContinuationController.Active;c.CloseMessage();
            c.Interact("procedure");c.CloseMessage();c.Interact("procedure");yield return new WaitForSeconds(2.9f);
            Assert.That(c.State.finalStep,Is.EqualTo(2));c.CloseMessage();c.Interact("procedure");
            c.Interact("exit");Assert.That(c.State.area,Is.Zero,"Release cannot be skipped.");
            yield return new WaitForSeconds(3.1f);c.CloseMessage();c.Interact("exit");yield return new WaitForSeconds(3.2f);
            c=CampaignContinuationController.Active;c.CloseMessage();c.Interact("renan");c.CloseMessage();c.Interact("notebook");c.CloseMessage();c.Interact("fusca");c.CloseMessage();
            Assert.That(c.State.finished&&c.CreditsVisible,Is.True);
        }
        [UnityTest,Timeout(90000)] public IEnumerator ReturnWaitsForCrossingAndSurvivesReloadBeforeSchoolEpilogue()
        {
            var story=new CampaignStory{phase=21};story.continuation.manifestationDispelled=true;story.continuation.chambersPrepared=7;story.continuation.finalCalibrated=true;story.continuation.finalRegulators=new[]{1,0,1};CampaignStorySave.Write(story);yield return Load(21);
            Assert.That(GameObject.Find("Padre Fábio").GetComponent<SpriteRenderer>().sprite.bounds.size.y,Is.GreaterThan(VarginhaReferenceSprites.PadreFabio().bounds.size.y*1.6f),"The epilogue keeps the arena priest's adult scale.");
            var c=CampaignContinuationController.Active;c.Interact("procedure");Assert.That(c.State.finalStep,Is.EqualTo(1));c.CloseMessage();
            yield return Load(21);c=CampaignContinuationController.Active;
            Assert.That(c.State.finalStep,Is.EqualTo(1),"The opened passage survives reload before crossing.");
            c.Interact("procedure");Assert.That(c.State.finalStep,Is.EqualTo(1),"The seal stays active throughout crossing.");
            yield return new WaitForSeconds(2.9f);Assert.That(c.State.finalStep,Is.EqualTo(2));
            yield return Load(21);c=CampaignContinuationController.Active;
            Assert.That(GameObject.Find("Entidade ferida").GetComponent<SpriteRenderer>().enabled,Is.False,"A completed crossing must not respawn the entity.");
            c.Interact("procedure");yield return new WaitForSeconds(3.1f);Assert.That(c.State.finalStep,Is.EqualTo(4));
            c.CloseMessage();c.Interact("exit");yield return new WaitForSeconds(3.3f);c=CampaignContinuationController.Active;
            Assert.That(c.area,Is.EqualTo(1));c.CloseMessage();c.Interact("fusca");Assert.That(c.State.finished,Is.False);
            c.CloseMessage();c.Interact("renan");c.CloseMessage();c.Interact("notebook");c.CloseMessage();c.Interact("fusca");
            Assert.That(c.State.finished,Is.True);Assert.That(CampaignStorySave.Load().continuation.finished,Is.True);
            Assert.That(c.CreditsVisible,Is.False,"The final dialogue precedes the credits.");c.CloseMessage();
            Assert.That(c.CreditsVisible,Is.True);Assert.That(c.Modal,Is.True);
            Assert.That(CampaignCredits.Names[0],Is.EqualTo("fabio, joao pedro matias, asafe e marcos"));
            Assert.That(CampaignCredits.Participation,Is.EqualTo("edelzio, renan, ouzana, professor fabio e ET de Varginha"));
        }
        [UnityTest,Timeout(90000)] public IEnumerator ReturnCannotOpenBeforeVictoryAndCalibrationOrSkipRelease()
        {
            CampaignStorySave.Write(new CampaignStory{phase=21});yield return Load(21);
            var c=CampaignContinuationController.Active;c.Interact("procedure");
            Assert.That(c.State.finalStep,Is.Zero,"Return must require victory and calibration.");
            c.CloseMessage();c.State.manifestationDispelled=true;c.Interact("procedure");
            Assert.That(c.State.finalStep,Is.Zero,"Victory alone must not bypass calibration.");
            c.CloseMessage();c.State.finalStep=3;c.Interact("exit");
            yield return null;Assert.That(c.State.area,Is.Zero,"Release must complete before the school epilogue.");
        }
        IEnumerator Click(Mouse mouse,string name)
        {
            Canvas.ForceUpdateCanvases();var rect=GameObject.Find(name).GetComponent<RectTransform>();
            Set(mouse.position,RectTransformUtility.WorldToScreenPoint(null,rect.TransformPoint(rect.rect.center)));
            yield return null;Press(mouse.leftButton);yield return null;Release(mouse.leftButton);yield return null;
        }
        IEnumerator Command(Keyboard keyboard,Mouse mouse,VarginhaInputAction action)
        {
            var key=VarginhaInputBindings.GetKeyboard(action);var button=VarginhaInputBindings.GetMouseButton(action);
            if(key!=Key.None)InputSystem.QueueStateEvent(keyboard,new KeyboardState(key));else Press(button==0?mouse.leftButton:mouse.rightButton);
            yield return null;
            if(key!=Key.None)InputSystem.QueueStateEvent(keyboard,new KeyboardState());else Release(button==0?mouse.leftButton:mouse.rightButton);
            yield return null;
        }
        [UnityTest,Timeout(90000)] public IEnumerator BackpackSelectsStudentsAndSupportWhilePausedWithMouseAndKeyboard()
        {
            var story=new CampaignStory{phase=20};story.continuation.chambersPrepared=7;CampaignStorySave.Write(story);yield return Load(20);
            var c=CampaignContinuationController.Active;var allies=CampaignFinalAllies.Active;
            var keyboard=InputSystem.AddDevice<Keyboard>();var mouse=InputSystem.AddDevice<Mouse>();
            Assert.That(VarginhaGameHUD.Instance.OpenBackpack(),Is.True);yield return null;
            Assert.That(Time.timeScale,Is.Zero);
            var bag=VarginhaGameHUD.Instance.GetComponent<VarginhaBackpackInventory>();
            yield return Click(mouse,"Tab_ALUNOS");Assert.That(bag.ShowingStudents,Is.True);
            yield return Click(mouse,"Slot_8");yield return Click(mouse,"Botao_EQUIPAR ALUNO");
            Assert.That(allies.Squad.SelectedStudentIndex,Is.EqualTo(8));
            Press(keyboard.leftArrowKey);yield return null;Release(keyboard.leftArrowKey);
            Press(keyboard.enterKey);yield return null;Release(keyboard.enterKey);
            Assert.That(allies.Squad.SelectedStudentIndex,Is.EqualTo(7));
            Press(keyboard.leftArrowKey);yield return null;Release(keyboard.leftArrowKey);Press(keyboard.enterKey);yield return null;Release(keyboard.enterKey);
            Assert.That(allies.Squad.SelectedStudentIndices.Count,Is.EqualTo(3));
            Press(keyboard.leftArrowKey);yield return null;Release(keyboard.leftArrowKey);Press(keyboard.enterKey);yield return null;Release(keyboard.enterKey);
            Assert.That(allies.Squad.SelectedStudentIndices.Count,Is.EqualTo(3));Assert.That(allies.Squad.SelectedStudentIndices[2],Is.EqualTo(5));
            yield return Click(mouse,"Tab_APOIO");Assert.That(bag.ShowingSupport,Is.True);
            yield return Click(mouse,"Slot_1");yield return Click(mouse,"Botao_EQUIPAR ALUNO");
            Assert.That(allies.SelectedSupport,Is.EqualTo(1));Assert.That(allies.Squad.SelectedStudentIndices.Count,Is.EqualTo(3));
            foreach(var ally in allies.Squad.Allies)Assert.That(ally.GetComponent<SpriteRenderer>().enabled,Is.EqualTo(ally.IsEquippedPresentation),"Only equipped students are visible in the arena.");
            foreach(int i in new[]{0,1,2})Assert.That(GameObject.Find(CampaignFinalAllies.Names[i]+"_Apoio").GetComponent<SpriteRenderer>().enabled,Is.EqualTo(i==1),"Only the selected teacher is visible.");
            Assert.That(CampaignStorySave.Load().continuation.battleStudents,Is.EqualTo(new[]{8,7,5}));
            Assert.That(CampaignStorySave.Load().continuation.battleSupport,Is.EqualTo(1));
            Assert.That(Time.timeScale,Is.Zero);VarginhaGameHUD.Instance.CloseBackpack();yield return null;
            Assert.That(Time.timeScale,Is.EqualTo(1));Assert.That(c.Modal,Is.False);
            var boss=Object.FindAnyObjectByType<CampaignManifestationCombat>();boss.enabled=false;boss.Stagger(30);
            boss.GetComponent<HealthSystem>().SetMaxHealth(10000,false);
            c.Actor.GetComponent<Rigidbody2D>().position=new Vector2(0,.58f);
            boss.GetComponent<Rigidbody2D>().position=new Vector2(3,.58f);yield return new WaitForFixedUpdate();
            yield return Command(keyboard,mouse,VarginhaInputAction.AllyCommand);
            Assert.That(allies.Squad.Allies[8].IsAttacking,Is.True);
            Assert.That(allies.Squad.Allies[7].ManualCooldownRemaining,Is.Zero);
            Assert.That(allies.Squad.Allies[5].ManualCooldownRemaining,Is.Zero);
            Assert.That(allies.Cooldown(1),Is.Zero,"A student command never launches the teacher.");
            yield return new WaitForSeconds(.6f);
            yield return Command(keyboard,mouse,VarginhaInputAction.AllyCommand);
            Assert.That(allies.Squad.Allies[7].ManualCooldownRemaining,Is.Zero,"Another press cannot overlap an unfinished attack.");
            Assert.That(allies.InvokeSupport(1),Is.False,"Teacher casting also waits for the current student.");
            yield return new WaitUntil(()=>!allies.Squad.CommandInProgress);
            yield return Command(keyboard,mouse,VarginhaInputAction.AllyCommand);
            Assert.That(allies.Squad.Allies[7].IsAttacking,Is.True,"The next command selects the next equipped student.");
            Assert.That(allies.Squad.Allies[5].ManualCooldownRemaining,Is.Zero);
            yield return new WaitUntil(()=>!allies.Squad.CommandInProgress);
            InputSystem.QueueStateEvent(keyboard,new KeyboardState(Key.Digit3));yield return null;
            InputSystem.QueueStateEvent(keyboard,new KeyboardState());yield return null;
            Assert.That(allies.Squad.Allies[5].IsAttacking,Is.True,"The third slot can be commanded directly.");
            yield return new WaitUntil(()=>!allies.Squad.CommandInProgress);
            yield return Command(keyboard,mouse,VarginhaInputAction.SupportCommand);
            Assert.That(allies.Cooldown(1),Is.GreaterThan(0),"The separate teacher command preserves the student selection.");
            Assert.That(allies.Squad.CommandInProgress,Is.False);
            yield return new WaitForSeconds(1.1f);
            Assert.That(GameObject.Find("Habilidade_Ouzana"),Is.Not.Null,"The reagent effect persists after casting.");
            Assert.That(allies.Squad.SelectedStudentIndices,Is.EqualTo(new[]{8,7,5}));
        }
    }
}
