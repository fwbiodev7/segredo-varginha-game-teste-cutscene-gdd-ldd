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
        [UnityTest,Timeout(180000)] public IEnumerator InvestigationAreasRemainReachableAndPuzzlesPreserveTheirSolutions()
        {
            var route=new System.Collections.Generic.List<Vector2>();
            for(int phase=11;phase<=19;phase++)
            {
                CampaignStorySave.Write(new CampaignStory{phase=phase});
                int areas=phase==12?2:phase==17?3:1;
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
                    if(phase==17){c.Interact("reading"+area);c.CloseMessage();c.Interact("diagram");c.CloseMessage();c.Interact("hide");c.CloseMessage();}
                    else foreach(var id in definition.ids){c.Interact(id);c.CloseMessage();}
                    if(phase==12&&area==0)
                    {
                        c.Interact("entrance");yield return new WaitUntil(()=>CampaignContinuationController.Active!=null&&CampaignContinuationController.Active.area==1);
                        yield return new WaitForSeconds(2.3f);CampaignContinuationController.Active.CloseMessage();
                    }
                    if(phase==17&&area<2)
                    {
                        c.Interact("next");int target=area+1;
                        yield return new WaitUntil(()=>CampaignContinuationController.Active!=null&&CampaignContinuationController.Active.area==target);
                    }
                }
                var current=CampaignContinuationController.Active;
                if(phase==17){current.State.chamberValues=new[]{0,2,1};Assert.That(current.State.ChambersStable,Is.True);}
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
            Assert.That(c.State.manifestationDispelled,Is.True);Assert.That(c.State.solved[9],Is.True);Assert.That(support.CombatActive,Is.False);
        }
        [UnityTest,Timeout(90000)] public IEnumerator ReturnWaitsForCrossingAndSurvivesReloadBeforeSchoolEpilogue()
        {
            var story=new CampaignStory{phase=21};story.continuation.manifestationDispelled=true;story.continuation.chambersPrepared=7;CampaignStorySave.Write(story);yield return Load(21);
            Assert.That(GameObject.Find("Padre Fábio").GetComponent<SpriteRenderer>().sprite.bounds.size.y,Is.GreaterThan(VarginhaReferenceSprites.PadreFabio().bounds.size.y*1.6f),"The epilogue keeps the arena priest's adult scale.");
            var c=CampaignContinuationController.Active;c.Interact("procedure");Assert.That(c.State.finalStep,Is.EqualTo(1));c.CloseMessage();
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
