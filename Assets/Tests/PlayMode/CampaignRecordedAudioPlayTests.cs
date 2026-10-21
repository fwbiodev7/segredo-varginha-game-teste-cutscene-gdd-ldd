using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Game.Player;
using Game.Varginha;
using Game.Varginha.Experiment;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace Game.Tests.PlayMode
{
    public sealed class CampaignRecordedAudioPlayTests
    {
        string _story,_memory;float _effects;
        static string Output=>CinemaQaEvidence.Folder;
        const BindingFlags Hidden=BindingFlags.NonPublic|BindingFlags.Instance;
        [SetUp] public void Preserve()
        {
            _story=File.Exists(CampaignStorySave.Path)?File.ReadAllText(CampaignStorySave.Path):null;
            _memory=File.Exists(CampaignMemorySave.Path)?File.ReadAllText(CampaignMemorySave.Path):null;
            _effects=VarginhaGameSettings.Current.effects;
        }
        [UnityTearDown] public IEnumerator Restore()
        {
            Time.timeScale=1;VarginhaGameSettings.Current.effects=_effects;
            yield return SceneManager.LoadSceneAsync("Menu_MisterioDeVarginha");yield return null;
            if(_story==null){if(File.Exists(CampaignStorySave.Path))File.Delete(CampaignStorySave.Path);}else File.WriteAllText(CampaignStorySave.Path,_story);
            if(_memory==null){if(File.Exists(CampaignMemorySave.Path))File.Delete(CampaignMemorySave.Path);}else File.WriteAllText(CampaignMemorySave.Path,_memory);
        }
        static IEnumerator Capture(string name)
        {
            if(Application.isEditor)yield break;
            yield return new WaitForEndOfFrame();var image=ScreenCapture.CaptureScreenshotAsTexture();
            Directory.CreateDirectory(Output+"/captures");File.WriteAllBytes(Output+"/captures/"+name+".png",image.EncodeToPNG());Object.Destroy(image);
        }
        [UnityTest] public IEnumerator CyberMeshHasAHardBudgetEvenWithMoreThanSixTargets()
        {
            var go=new GameObject("Cyber_budget_QA");var cyber=go.AddComponent<CampaignCyberPulse>();
            var target=new GameObject("Cyber_target_QA");var renderer=target.AddComponent<SpriteRenderer>();
            renderer.sprite=VarginhaReferenceSprites.PadreFabio();var targets=Enumerable.Repeat(renderer,12).ToArray();
            cyber.Draw(Vector2.zero,Vector2.right*3,.2f,.2f,true,targets);
            Assert.That(cyber.QuadCount,Is.EqualTo(64+24*6));Assert.That(cyber.QuadCount,Is.LessThanOrEqualTo(CampaignCyberPulse.MaxQuads));
            Object.Destroy(go);Object.Destroy(target);yield return null;
        }
        [UnityTest] public IEnumerator ShieldSoundFollowsPauseEffectsSliderAndLifetime()
        {
            var go=new GameObject("Audio_QA");var sound=go.AddComponent<CampaignSoundscape>();
            sound.BeginShield(.6f);yield return new WaitForSeconds(.15f);
            var source=go.GetComponent<AudioSource>();Assert.That(source.clip,Is.SameAs(CampaignRecordedAudio.Clip("FabioShield")));
            Assert.That(source.isPlaying,Is.True);Assert.That(source.loop,Is.True);
            sound.Suspend(true);Time.timeScale=0;yield return new WaitForSecondsRealtime(.7f);
            Assert.That(source.isPlaying,Is.False);Assert.That((float)typeof(CampaignSoundscape).GetField("_shieldRemaining",Hidden).GetValue(sound),Is.GreaterThan(.3f));
            sound.Suspend(false);yield return null;
            Assert.That(source.isPlaying,Is.False,"Clearing local suspension cannot bypass the still-active global pause.");
            Time.timeScale=1;sound.Suspend(false);VarginhaGameSettings.Current.effects=0;yield return null;
            Assert.That(source.volume,Is.Zero);VarginhaGameSettings.Current.effects=_effects;
            yield return new WaitForSeconds(.55f);Assert.That(source.isPlaying,Is.False);Object.Destroy(go);
        }
        [UnityTest,Timeout(90000)] public IEnumerator ThreeSupportCastsHaveSingleAudioCuesAndBoundedVisuals()
        {
            var story=new CampaignStory{phase=20};story.continuation.chambersPrepared=7;CampaignStorySave.Write(story);
            yield return SceneManager.LoadSceneAsync(CampaignContinuationDefinition.SceneName(20));
            yield return new WaitForSeconds(2.3f);var c=CampaignContinuationController.Active;c.CloseMessage();yield return null;
            yield return new WaitUntil(()=>!c.BattleEntrancePlaying);c.CloseMessage();yield return null;
            var support=CampaignFinalAllies.Active;var sound=c.GetComponent<CampaignSoundscape>();
            var boss=Object.FindAnyObjectByType<CampaignManifestationCombat>();boss.enabled=false;boss.Stagger(200);
            var hp=boss.GetComponent<HealthSystem>();hp.SetMaxHealth(10000,false);
            c.Actor.GetComponent<Rigidbody2D>().position=new Vector2(0,.58f);boss.GetComponent<Rigidbody2D>().position=new Vector2(3,.58f);
            yield return new WaitForFixedUpdate();yield return null;
            var expected=new[]{"RenanPulse","OuzanaGlass","FabioShield"};var report=new List<string>();
            for(int i=0;i<3;i++)
            {
                if(support.SelectedSupport!=i)support.SelectSupport(i);int serial=sound.EventSerial,emissions=0,maxEffects=0,maxParticles=0,maxQuads=0;float width=0;
                Assert.That(support.InvokeSupport(i),Is.True);Assert.That(support.InvokeSupport(i),Is.False,"No duplicate cast while busy.");
                float start=Time.time;bool main=false,tail=false;float duration=i==2?7.1f:i==1?4:1.5f;
                while(Time.time-start<duration)
                {
                    if(serial!=sound.EventSerial){if(sound.LastEvent==expected[i])emissions++;serial=sound.EventSerial;}
                    var active=c.GetComponentsInChildren<SpriteRenderer>().Where(r=>r.name.StartsWith("Habilidade_")).ToArray();
                    maxEffects=Mathf.Max(maxEffects,active.Length);maxParticles=Mathf.Max(maxParticles,c.GetComponentsInChildren<ParticleSystem>().Length);
                    var fx=active.FirstOrDefault(r=>r.name=="Habilidade_"+CampaignFinalAllies.Names[i]);
                    if(fx!=null)width=Mathf.Max(width,fx.bounds.size.x);
                    var mesh=c.GetComponentInChildren<CampaignCyberPulse>();if(mesh!=null)maxQuads=Mathf.Max(maxQuads,mesh.QuadCount);
                    float elapsed=Time.time-start;
                    if(!main&&elapsed>(i==0?.75f:i==1?.65f:1f)){main=true;yield return Capture("support-"+i+"-cast");}
                    if(!tail&&elapsed>(i==1?1.45f:i==2?3.8f:1.05f)){tail=true;yield return Capture("support-"+i+"-impact");}
                    yield return null;
                }
                Assert.That(emissions,Is.EqualTo(1),expected[i]);Assert.That(maxEffects,Is.EqualTo(1));Assert.That(maxParticles,Is.Zero);
                Assert.That(maxQuads,Is.LessThanOrEqualTo(CampaignCyberPulse.MaxQuads));
                if(i==0)Assert.That(maxQuads,Is.GreaterThan(64),"Cyber packets and impact fragments are rendered.");
                // Screenshot encoding can stall a frame. Await destruction with a bounded deadline,
                // rather than confusing the nominal visual duration with the coroutine's final frame.
                float cleanupDeadline=Time.realtimeSinceStartup+2;
                while(c.GetComponentsInChildren<SpriteRenderer>().Any(r=>r.name.StartsWith("Habilidade_"))&&Time.realtimeSinceStartup<cleanupDeadline)yield return null;
                Assert.That(c.GetComponentsInChildren<SpriteRenderer>().Any(r=>r.name.StartsWith("Habilidade_")),Is.False,"Effects are destroyed at cast end.");
                report.Add(CampaignFinalAllies.Names[i]+": audioCues="+emissions+"; maxEffectObjects="+maxEffects+"; particleSystems="+maxParticles+"; maximumQuads="+maxQuads+"; maximumSpriteWidth="+width.ToString("F3",System.Globalization.CultureInfo.InvariantCulture));
            }
            // Regression: a modal dialogue must freeze gameplay protection, animation and sound together.
            var ready=(float[])typeof(CampaignFinalAllies).GetField("ready",Hidden).GetValue(support);for(int i=0;i<ready.Length;i++)ready[i]=0;
            Assert.That(support.InvokeSupport(2),Is.True);yield return new WaitForSeconds(.5f);
            float remaining=support.ShieldRemaining;
            c.Say("QA — pausa de interface");yield return new WaitForSeconds(.7f);
            float after=support.ShieldRemaining;
            Assert.That(after,Is.EqualTo(remaining).Within(.06f));Assert.That(support.DamageMultiplier,Is.EqualTo(.45f));
            report.Add("FIX_VERIFIED: shield protection, animation and audio freeze together during modal dialogue.");
            c.CloseMessage();support.CombatActive=false;yield return null;yield return null;
            support.CombatActive=true;for(int i=0;i<ready.Length;i++)ready[i]=0;
            foreach(int i in new[]{2,1,0}){if(support.SelectedSupport!=i)support.SelectSupport(i);Assert.That(support.InvokeSupport(i),Is.True);yield return new WaitForSeconds(1.05f);}
            int concurrent=c.GetComponentsInChildren<SpriteRenderer>().Count(r=>r.name.StartsWith("Habilidade_"));
            Assert.That(concurrent,Is.EqualTo(3));yield return Capture("support-all-concurrent");
            yield return new WaitForSeconds(4.1f);Assert.That(c.GetComponentsInChildren<CampaignCyberPulse>(),Is.Empty);
            Assert.That(c.GetComponentsInChildren<SpriteRenderer>().Any(r=>r.name.StartsWith("Habilidade_")),Is.False);
            report.Add("CONCURRENT: three support effects active, no ParticleSystem; all cleaned after lifetime.");
            Directory.CreateDirectory(Output);File.WriteAllLines(Output+"/support-audit.txt",report);
            foreach(var line in report)Debug.Log("SUPPORT_AUDIT "+line);
        }
        [UnityTest] public IEnumerator ChildExplosionIsEmittedOnceAtTheAuthoredLaunch()
        {
            CampaignMemorySave.Write(new CampaignMemory{openingSeen=true});CampaignStorySave.Write(new CampaignStory{phase=1});
            yield return SceneManager.LoadSceneAsync(VarginhaCampaignPhase1.SceneName);yield return new WaitForSeconds(.3f);
            var phase=VarginhaCampaignPhase1.Active;phase.enabled=false;
            var actor=Object.FindAnyObjectByType<EdelzioTopDownController>();var sound=phase.GetComponent<CampaignSoundscape>();
            Assert.That(sound,Is.Not.Null);var encounter=phase.gameObject.AddComponent<CampaignFlashEncounter>();
            encounter.Begin(actor,actor.GetComponent<SpriteRenderer>(),sound);int serial=sound.EventSerial,count=0;float end=Time.time+1;
            while(Time.time<end){if(serial!=sound.EventSerial){if(sound.LastEvent=="ChildExplosion")count++;serial=sound.EventSerial;}yield return null;}
            Assert.That(count,Is.EqualTo(1));Assert.That(encounter.Elapsed,Is.GreaterThan(.9f));
        }
    }
}
