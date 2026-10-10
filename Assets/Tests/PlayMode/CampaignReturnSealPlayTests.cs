using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Game.Varginha;
using Game.Varginha.Experiment;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering;
using UnityEngine.TestTools;

namespace Game.Tests.PlayMode
{
    public class CampaignReturnSealPlayTests : InputTestFixture
    {
        private string _story,_memory;
        private bool _reduced;
        public override void Setup(){VarginhaInputActions.Shutdown();base.Setup();}
        public override void TearDown(){VarginhaInputActions.Shutdown();base.TearDown();}
        [SetUp] public void Preserve()
        {
            _story=File.Exists(CampaignStorySave.Path)?File.ReadAllText(CampaignStorySave.Path):null;
            _memory=File.Exists(CampaignMemorySave.Path)?File.ReadAllText(CampaignMemorySave.Path):null;
            _reduced=VarginhaGameSettings.Current.reducedMotion;VarginhaGameSettings.Current.reducedMotion=false;
        }
        [TearDown] public void Restore()
        {
            Time.timeScale=1;VarginhaGameSettings.Current.reducedMotion=_reduced;
            if(_story==null){if(File.Exists(CampaignStorySave.Path))File.Delete(CampaignStorySave.Path);}else File.WriteAllText(CampaignStorySave.Path,_story);
            if(_memory==null){if(File.Exists(CampaignMemorySave.Path))File.Delete(CampaignMemorySave.Path);}else File.WriteAllText(CampaignMemorySave.Path,_memory);
        }
        [Test] public void AtlasHasDistinctLoopFramesAndClearRenderedBordersAndCenter()
        {
            var texture=Resources.Load<Texture2D>(CampaignReturnSeal.Resource);
            Assert.That(texture.width,Is.EqualTo(768));Assert.That(texture.height,Is.EqualTo(512));
            Assert.That(texture.filterMode,Is.EqualTo(FilterMode.Point));Assert.That(texture.mipmapCount,Is.EqualTo(1));
            var pixels=texture.GetPixels32();var hashes=new HashSet<ulong>();
            for(int frame=0;frame<24;frame++)
            {
                var sprite=CampaignReturnSeal.Frame(frame);Assert.That(sprite.pixelsPerUnit,Is.EqualTo(32));
                Assert.That(sprite.pivot,Is.EqualTo(new Vector2(64,32)));
                int x0=frame%6*128,y0=(3-frame/6)*128,solid=0,clearCenter=0,totalCenter=0;
                ulong hash=1469598103934665603UL;
                for(int y=0;y<128;y++)for(int x=0;x<128;x++)
                {
                    var p=pixels[(y0+y)*768+x0+x];
                    if(x<2||y<2||x>125||y>125)Assert.That(p.a,Is.LessThan(140),"Visible pixels cannot meet a cell boundary");
                    if(p.a>=140)solid++;
                    if(x>=50&&x<78&&y>=45&&y<85){totalCenter++;if(p.a<140)clearCenter++;}
                    unchecked{hash=(hash^p.r)*1099511628211UL;hash=(hash^p.g)*1099511628211UL;hash=(hash^p.b)*1099511628211UL;hash=(hash^p.a)*1099511628211UL;}
                }
                Assert.That(solid,Is.GreaterThan(1),"Each authored frame has visible content");
                if(frame>=6&&frame<18)
                {Assert.That(clearCenter,Is.GreaterThan(totalCenter*.5f),"The portal must reveal the creature");Assert.That(hashes.Add(hash),Is.True,"Loop frames must actually differ");}
            }
        }
        [Test] public void PresentationOpensLoopsClosesAndUsesStableReducedMotion()
        {
            var go=new GameObject("seal test");var entityGo=new GameObject("entity test");
            try
            {
                var entity=entityGo.AddComponent<SpriteRenderer>();entity.sortingOrder=12000;
                var renderer=go.AddComponent<SpriteRenderer>();var seal=go.AddComponent<CampaignReturnSeal>();seal.Configure(entity);
                seal.Present(0,0,false,false);Assert.That(renderer.enabled,Is.False);
                seal.Present(1,0,false,false);Assert.That(renderer.sprite,Is.SameAs(CampaignReturnSeal.Frame(0)));
                seal.Present(1,.6f,false,false);Assert.That(renderer.sprite,Is.SameAs(CampaignReturnSeal.Frame(3)));
                seal.Present(1,.7f,false,false);var first=renderer.sprite;
                seal.Present(1,.25f,true,false);Assert.That(renderer.sprite,Is.Not.SameAs(first));
                Assert.That(renderer.sortingOrder,Is.EqualTo(entity.sortingOrder-1));Assert.That(go.transform.localScale,Is.EqualTo(Vector3.one));
                seal.Present(2,0,false,true);seal.Present(2,10,false,true);Assert.That(renderer.sprite,Is.SameAs(CampaignReturnSeal.Frame(6)));
                seal.Present(3,0,false,false);Assert.That(renderer.sprite,Is.SameAs(CampaignReturnSeal.Frame(18)));
                seal.Present(3,.7f,false,false);Assert.That(renderer.color.a,Is.EqualTo(.5f).Within(.02f));
                seal.Present(3,.71f,false,false);Assert.That(renderer.enabled,Is.False);
                seal.Present(4,0,false,false);Assert.That(renderer.enabled,Is.False);
            }
            finally{Object.DestroyImmediate(go);Object.DestroyImmediate(entityGo);}
        }
        private static void Advance(CampaignContinuationController controller)
        {typeof(CampaignContinuationController).GetMethod("Finale",BindingFlags.NonPublic|BindingFlags.Instance).Invoke(controller,new object[]{"procedure"});}

        [UnityTest,Timeout(60000)] public IEnumerator ReturnSequenceKeepsTheSealUntilCrossingAndResumesSavedProgress()
        {
            var story=new CampaignStory{phase=21};story.continuation.chambersPrepared=7;
            story.continuation.manifestationDispelled=true;story.continuation.finalRegulators=new[]{1,0,1};
            Assert.That(story.continuation.CalibrateReturn(),Is.True);CampaignStorySave.Write(story);
            SceneManager.LoadScene(CampaignContinuationDefinition.SceneName(21));yield return null;yield return null;
            var controller=CampaignContinuationController.Active;controller.CloseMessage();
            var viewingPosition=controller.Plan.points.Find(p=>p.id=="procedure").position+Vector2.down*2.4f+Vector2.up*.58f;
            controller.Actor.GetComponent<Rigidbody2D>().position=viewingPosition;
            yield return new WaitForSeconds(.7f);
            var seal=Object.FindAnyObjectByType<CampaignReturnSeal>();var renderer=seal.GetComponent<SpriteRenderer>();
            Assert.That(renderer.enabled,Is.False);Advance(controller);yield return new WaitForSeconds(1.3f);
            Assert.That(controller.State.finalStep,Is.EqualTo(1));Assert.That(renderer.enabled,Is.True);
            System.IO.Directory.CreateDirectory("Preview/SeloRetorno20261010");
            ScreenCapture.CaptureScreenshot("Preview/SeloRetorno20261010/SeloNaFase.png");yield return null;
            controller.CloseMessage();controller.TogglePause();var paused=renderer.sprite;
            yield return new WaitForSecondsRealtime(.2f);Assert.That(renderer.sprite,Is.SameAs(paused));controller.TogglePause();
            Advance(controller);yield return new WaitForSeconds(.9f);
            var entity=GameObject.Find("Entidade ferida").GetComponent<SpriteRenderer>();
            Assert.That(entity.enabled,Is.True);Assert.That(renderer.enabled,Is.True);
            Assert.That(renderer.GetComponent<SortingGroup>().sortingOrder,Is.LessThan(entity.GetComponent<SortingGroup>().sortingOrder));
            ScreenCapture.CaptureScreenshot("Preview/SeloRetorno20261010/Travessia.png");yield return null;
            float deadline=Time.realtimeSinceStartup+5;
            while(controller.State.finalStep!=2&&Time.realtimeSinceStartup<deadline)yield return null;
            Assert.That(controller.State.finalStep,Is.EqualTo(2));Assert.That(entity.enabled,Is.False);
            Assert.That(controller.State.finalSealActive,Is.True);Assert.That(renderer.enabled,Is.True);
            SceneManager.LoadScene(CampaignContinuationDefinition.SceneName(21));yield return null;yield return null;
            controller=CampaignContinuationController.Active;controller.CloseMessage();
            renderer=Object.FindAnyObjectByType<CampaignReturnSeal>().GetComponent<SpriteRenderer>();
            Assert.That(controller.State.finalStep,Is.EqualTo(2));Assert.That(renderer.enabled,Is.True);
            Assert.That(GameObject.Find("Entidade ferida").GetComponent<SpriteRenderer>().enabled,Is.False);
            Advance(controller);yield return new WaitForSeconds(.7f);Assert.That(controller.State.finalStep,Is.EqualTo(3));
            deadline=Time.realtimeSinceStartup+5;
            while(controller.State.finalStep!=4&&Time.realtimeSinceStartup<deadline)yield return null;
            Assert.That(controller.State.finalStep,Is.EqualTo(4));Assert.That(renderer.enabled,Is.False);
            Assert.That(controller.State.finalSealActive,Is.False);Assert.That(CampaignStorySave.Load().continuation.finalStep,Is.EqualTo(4));
        }
    }
}
