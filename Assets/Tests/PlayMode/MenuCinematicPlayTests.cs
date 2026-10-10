using System.Collections;
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
    public class MenuCinematicPlayTests : InputTestFixture
    {
        private const string MenuScene="Menu_MisterioDeVarginha";
        private const BindingFlags Private=BindingFlags.NonPublic|BindingFlags.Instance;
        private string _story,_memory;
        private float _music,_effects;
        private bool _reduced;
        public override void Setup(){VarginhaInputActions.Shutdown();base.Setup();}
        public override void TearDown(){VarginhaInputActions.Shutdown();base.TearDown();}
        [SetUp] public void Preserve()
        {
            InputSystem.settings.backgroundBehavior=InputSettings.BackgroundBehavior.IgnoreFocus;
            InputSystem.settings.editorInputBehaviorInPlayMode=InputSettings.EditorInputBehaviorInPlayMode.AllDeviceInputAlwaysGoesToGameView;
            _story=File.Exists(CampaignStorySave.Path)?File.ReadAllText(CampaignStorySave.Path):null;
            _memory=File.Exists(CampaignMemorySave.Path)?File.ReadAllText(CampaignMemorySave.Path):null;
            _music=VarginhaGameSettings.Current.music;_effects=VarginhaGameSettings.Current.effects;
            _reduced=VarginhaGameSettings.Current.reducedMotion;
        }
        [UnityTearDown] public IEnumerator Restore()
        {
            float until=Time.realtimeSinceStartup+15;
            while(CampaignCinematics.IsTransitioning&&Time.realtimeSinceStartup<until)yield return null;
            Time.timeScale=1;
            SceneManager.LoadScene(MenuScene);yield return null;yield return null;
            RestoreFile(CampaignStorySave.Path,_story);RestoreFile(CampaignMemorySave.Path,_memory);
            VarginhaGameSettings.Current.music=_music;VarginhaGameSettings.Current.effects=_effects;
            VarginhaGameSettings.Current.reducedMotion=_reduced;
        }
        private static void RestoreFile(string path,string content)
        {if(content!=null)File.WriteAllText(path,content);else if(File.Exists(path))File.Delete(path);}
        private static IEnumerator LoadMenu()
        {
            SceneManager.LoadScene(MenuScene);yield return null;yield return new WaitForSecondsRealtime(.35f);
            Assert.That(VarginhaMainMenu.Active,Is.Not.Null);
        }
        private static string Panel => typeof(VarginhaMainMenu).GetField("panel",Private).GetValue(VarginhaMainMenu.Active).ToString();
        private static IEnumerator KeyPress(Keyboard keyboard,Key key)
        {
            InputSystem.QueueStateEvent(keyboard,new KeyboardState(key));yield return null;yield return null;
            InputSystem.QueueStateEvent(keyboard,new KeyboardState());yield return null;yield return null;
        }
        private static IEnumerator PadPress(Gamepad pad,GamepadButton button)
        {
            InputSystem.QueueStateEvent(pad,new GamepadState().WithButton(button));yield return null;yield return null;
            InputSystem.QueueStateEvent(pad,new GamepadState());yield return null;yield return null;
        }
        [UnityTest,Timeout(60000)]
        public IEnumerator KeyboardAndControllerUseTheSameAnimatedButtonsAndPanels()
        {
            CampaignStorySave.Write(new CampaignStory{phase=18});
            var keyboard=InputSystem.AddDevice<Keyboard>();var pad=InputSystem.AddDevice<Gamepad>();
            yield return LoadMenu();
            yield return KeyPress(keyboard,Key.DownArrow);yield return new WaitForSecondsRealtime(.25f);
            var motions=(MenuButtonMotion[])typeof(VarginhaMainMenu).GetField("_buttonMotion",Private).GetValue(VarginhaMainMenu.Active);
            Assert.That(motions[1].Amount,Is.GreaterThan(.9f),"Keyboard focus gets the cyan animation");
            yield return KeyPress(keyboard,Key.Enter);Assert.That(Panel,Is.EqualTo("Play"));
            yield return KeyPress(keyboard,Key.Escape);Assert.That(Panel,Is.EqualTo("None"));
            yield return KeyPress(keyboard,Key.DownArrow);yield return KeyPress(keyboard,Key.Enter);
            Assert.That(Panel,Is.EqualTo("Settings"));
            yield return KeyPress(keyboard,Key.Escape);Assert.That(Panel,Is.EqualTo("None"));
            yield return PadPress(pad,GamepadButton.DpadUp);yield return new WaitForSecondsRealtime(.25f);
            Assert.That(motions[1].Amount,Is.GreaterThan(.9f));
            yield return PadPress(pad,GamepadButton.South);Assert.That(Panel,Is.EqualTo("Play"));
            yield return PadPress(pad,GamepadButton.East);Assert.That(Panel,Is.EqualTo("None"));
            Assert.That(CampaignStorySave.Load().phase,Is.EqualTo(18),"Navigation does not touch progress");
        }
        [UnityTest,Timeout(60000)]
        public IEnumerator MenuReusesItsRenderTargetAndRespectsReducedMotionAndAudioSettings()
        {
            yield return LoadMenu();var atmosphere=VarginhaMainMenu.Active.Atmosphere;var frame=atmosphere.Frame;
            Assert.That(frame,Is.Not.Null);Assert.That(frame.filterMode,Is.EqualTo(FilterMode.Point));
            Assert.That(frame.width,Is.EqualTo(Resources.Load<Texture2D>(MenuCinematicAtmosphere.BackgroundResource).width));
            var material=(Material)typeof(MenuCinematicAtmosphere).GetField("_material",Private).GetValue(atmosphere);
            Assert.That(material.HasProperty("_UfoSprite"),Is.True);
            Assert.That(material.GetTexture("_UfoSprite"),Is.SameAs(Resources.Load<Texture2D>("Varginha/MenuUfoTransparent")),"Live menu must bind the alpha sprite");
            var first=Read(frame);yield return new WaitForSecondsRealtime(.4f);var second=Read(frame);
            Assert.That(Differences(first,second),Is.GreaterThan(25),"The existing art actually moves");
            Assert.That(atmosphere.Frame,Is.SameAs(frame),"No render target allocations per frame");
            VarginhaGameSettings.Current.reducedMotion=true;
            VarginhaGameSettings.Current.music=0;VarginhaGameSettings.Current.effects=0;
            yield return new WaitForSecondsRealtime(.4f);first=Read(frame);
            yield return new WaitForSecondsRealtime(.4f);second=Read(frame);
            Assert.That(Differences(first,second),Is.Zero,"Reduced motion is stable, without distortions");
            foreach(var source in atmosphere.GetComponents<AudioSource>())
                if(atmosphere.OwnsAudioSource(source))Assert.That(source.volume,Is.Zero);
        }
        [UnityTest,Timeout(60000)]
        public IEnumerator ContinueShowsSignalLossAndPreservesTheSavedChapter()
        {
            CampaignStorySave.Write(new CampaignStory{phase=18});yield return LoadMenu();
            var keyboard=InputSystem.AddDevice<Keyboard>();
            yield return KeyPress(keyboard,Key.DownArrow);yield return KeyPress(keyboard,Key.UpArrow);
            yield return KeyPress(keyboard,Key.Enter);
            Assert.That(CampaignCinematics.IsTransitioning,Is.True);
            bool sawMessage=false;float deadline=Time.realtimeSinceStartup+18;
            while(CampaignCinematics.IsTransitioning&&Time.realtimeSinceStartup<deadline)
            {sawMessage|=CampaignCinematics.MenuSignalMessageVisible;yield return null;}
            Assert.That(sawMessage,Is.True,"Radio -> sweep -> black -> signal loss -> load");
            Assert.That(CampaignCinematics.IsTransitioning,Is.False);
            Assert.That(CampaignContinuationController.Active.phase,Is.EqualTo(18));
            Assert.That(CampaignStorySave.Load().phase,Is.EqualTo(18));
            Assert.That(VarginhaMainMenu.IsOpen,Is.False);Assert.That(Time.timeScale,Is.EqualTo(1));
            Assert.That(Object.FindAnyObjectByType<MenuCinematicAtmosphere>(),Is.Null,"No menu effect survives into gameplay");
            CampaignCinematics.Load(CampaignContinuationDefinition.SceneName(13));
            bool unexpectedSignal=false;deadline=Time.realtimeSinceStartup+12;
            while(CampaignCinematics.IsTransitioning&&Time.realtimeSinceStartup<deadline)
            {unexpectedSignal|=CampaignCinematics.MenuSignalMessageVisible;yield return null;}
            Assert.That(unexpectedSignal,Is.False,"Normal chapter transitions retain their original behavior");
            Assert.That(CampaignContinuationController.Active.phase,Is.EqualTo(13));
            Assert.That(Time.timeScale,Is.EqualTo(1));
        }
        [UnityTest,Timeout(60000)]
        public IEnumerator NewStoryRetainsConfirmationAndStartsTheOriginalFirstChapter()
        {
            CampaignStorySave.Write(new CampaignStory{phase=18});yield return LoadMenu();
            var keyboard=InputSystem.AddDevice<Keyboard>();
            yield return KeyPress(keyboard,Key.DownArrow);yield return KeyPress(keyboard,Key.Enter);
            Assert.That(Panel,Is.EqualTo("Play"));Assert.That(CampaignStorySave.Load().phase,Is.EqualTo(18));
            // The confirmation panel selects INICIAR CAMPANHA first, as before.
            yield return KeyPress(keyboard,Key.Enter);
            float deadline=Time.realtimeSinceStartup+18;
            while(CampaignCinematics.IsTransitioning&&Time.realtimeSinceStartup<deadline)yield return null;
            Assert.That(SceneManager.GetActiveScene().name,Is.EqualTo(VarginhaCampaignPhase1.SceneName));
            Assert.That(CampaignStorySave.Load().phase,Is.EqualTo(1));
            Assert.That(Time.timeScale,Is.EqualTo(1));
        }
        [Test]
        public void DisabledShaderPreservesEveryOriginalPixelAndGazeOnlyUsesExistingArt()
        {
            var texture=Resources.Load<Texture2D>(MenuCinematicAtmosphere.BackgroundResource);
            var shader=Resources.Load<Shader>("Varginha/MenuAtmosphere");Assert.That(shader.isSupported,Is.True);
            var material=new Material(shader);
            var reference=RenderTexture.GetTemporary(texture.width,texture.height,0,RenderTextureFormat.ARGB32);
            var result=RenderTexture.GetTemporary(texture.width,texture.height,0,RenderTextureFormat.ARGB32);
            try
            {
                Graphics.Blit(texture,reference);material.SetFloat("_Motion",0);Graphics.Blit(texture,result,material);
                Assert.That(Differences(Read(reference),Read(result)),Is.Zero,"No redraw or blur at rest");
                material.SetFloat("_Motion",1);material.SetFloat("_MenuTime",3);material.SetFloat("_Gaze",0);
                Graphics.Blit(texture,result,material);var resting=Read(result);
                material.SetFloat("_Gaze",1);Graphics.Blit(texture,result,material);var looking=Read(result);
                int changedHead=0,changedElsewhere=0;
                for(int i=0;i<resting.Length;i++)if(!resting[i].Equals(looking[i]))
                {
                    float x=(i%texture.width+.5f)/texture.width,y=(i/texture.width+.5f)/texture.height;
                    if(x>=.864f&&x<=.891f&&y>=.375f&&y<=.419f)changedHead++;else changedElsewhere++;
                }
                Assert.That(changedHead,Is.GreaterThan(5));Assert.That(changedElsewhere,Is.Zero,"The rare look leaves the composition intact");
            }
            finally{Object.DestroyImmediate(material);RenderTexture.ReleaseTemporary(reference);RenderTexture.ReleaseTemporary(result);}
        }
        private static Color32[] Read(RenderTexture frame)
        {
            var previous=RenderTexture.active;var copy=new Texture2D(frame.width,frame.height,TextureFormat.RGBA32,false);
            try{RenderTexture.active=frame;copy.ReadPixels(new Rect(0,0,frame.width,frame.height),0,0);copy.Apply();return copy.GetPixels32();}
            finally{RenderTexture.active=previous;Object.DestroyImmediate(copy);}
        }
        private static int Differences(Color32[] a,Color32[] b)
        {int count=0;for(int i=0;i<a.Length;i++)if(!a[i].Equals(b[i]))count++;return count;}
        [UnityTest,Timeout(60000)]
        public IEnumerator ExistingHudAndPauseRegressionStillPassesWithTheAnimatedMenu()
        {
            yield return LoadMenu();
            // Reuse the original regression body; a real menu must hide the gameplay HUD.
            var active=VarginhaMainMenu.Active;
            Object.Destroy(active.gameObject);yield return null;
            var regression=new VarginhaMenuAndDeathTests();
            try{yield return regression.PersistentHudCannotDrawOrPauseOverMenu();}
            finally{regression.Cleanup();}
        }

        [UnityTest,Timeout(60000)]
        public IEnumerator BackpackHoverPreservesSpritesClicksAndDisabledButtonsWhilePaused()
        {
            yield return LoadMenu();
            var root=new GameObject("BackpackHoverTest");
            try
            {
                root.AddComponent<CircleCollider2D>();root.AddComponent<SpriteRenderer>();
                var player=root.AddComponent<EdelzioTopDownController>();player.HasBackpack=true;
                var pack=root.AddComponent<VarginhaBackpackInventory>();
                Assert.That(pack.Open(player),Is.True);
                yield return null;
                var buttons=Object.FindObjectsByType<UnityEngine.UI.Button>(FindObjectsSortMode.None);
                Assert.That(buttons.Length,Is.GreaterThanOrEqualTo(9));
                foreach(var candidate in buttons)Assert.That(candidate.GetComponent<Game.UI.PixelButtonHoverUGUI>(),Is.Not.Null,candidate.name);
                var button=System.Array.Find(buttons,b=>b.IsInteractable()&&b.name.StartsWith("Tab_"));
                Assert.That(button,Is.Not.Null);
                var motion=button.GetComponent<Game.UI.PixelButtonHoverUGUI>();
                var graphic=(UnityEngine.UI.Image)button.targetGraphic;
                var rect=((RectTransform)button.transform).rect;var sprite=graphic.sprite;
                int clicks=0;button.onClick.AddListener(()=>clicks++);
                var pointer=new UnityEngine.EventSystems.PointerEventData(UnityEngine.EventSystems.EventSystem.current){button=UnityEngine.EventSystems.PointerEventData.InputButton.Left};
                UnityEngine.EventSystems.ExecuteEvents.Execute(button.gameObject,pointer,UnityEngine.EventSystems.ExecuteEvents.pointerEnterHandler);
                yield return new WaitForSecondsRealtime(.25f);
                Assert.That(motion.Amount,Is.GreaterThan(.98f));
                Assert.That(Time.timeScale,Is.Zero,"Hover must animate while paused");
                Assert.That(graphic.sprite,Is.SameAs(sprite));
                Assert.That(((RectTransform)button.transform).rect,Is.EqualTo(rect));
                Assert.That(button.GetComponentInChildren<Game.UI.PixelHoverGraphic>().raycastTarget,Is.False);
                UnityEngine.EventSystems.ExecuteEvents.Execute(button.gameObject,pointer,UnityEngine.EventSystems.ExecuteEvents.pointerClickHandler);
                Assert.That(clicks,Is.EqualTo(1));
                button.interactable=false;yield return null;yield return null;
                Assert.That(motion.Amount,Is.Zero);
                UnityEngine.EventSystems.ExecuteEvents.Execute(button.gameObject,pointer,UnityEngine.EventSystems.ExecuteEvents.pointerClickHandler);
                Assert.That(clicks,Is.EqualTo(1),"Disabled button must not invoke its action");
                button.interactable=true;
                UnityEngine.EventSystems.ExecuteEvents.Execute(button.gameObject,pointer,UnityEngine.EventSystems.ExecuteEvents.pointerExitHandler);
                var pad=InputSystem.AddDevice<Gamepad>();yield return PadPress(pad,GamepadButton.DpadRight);
                UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(button.gameObject);
                yield return new WaitForSecondsRealtime(.25f);
                Assert.That(motion.Amount,Is.GreaterThan(.98f),"Controller selection shares the hover effect");
                pack.Close();Assert.That(Time.timeScale,Is.EqualTo(1));
            }
            finally{Object.DestroyImmediate(root);Time.timeScale=1;}
        }

        [UnityTest,Timeout(60000)]
        public IEnumerator SharedHoverRetainsTheCarInteractionBinding()
        {
            yield return LoadMenu();
            var keyboard=InputSystem.AddDevice<Keyboard>();
            var root=new GameObject("CarHoverProbe");
            var probe=root.AddComponent<PixelHoverCarProbe>();
            try
            {
                yield return null;yield return null;
                yield return KeyPress(keyboard,Key.Enter);
                Assert.That(probe.Clicks,Is.Zero,"Car actions must not become ordinary Submit buttons");
                yield return KeyPress(keyboard,Key.W);
                Assert.That(probe.Clicks,Is.EqualTo(1));
            }
            finally{Object.DestroyImmediate(root);}
        }

        [Test] public void BlueFuscaReplacementPreservesEveryPixelOutsideTheCarRectangle()
        {
            var original=new Texture2D(2,2);var replacement=new Texture2D(2,2);
            try
            {
                original.LoadImage(File.ReadAllBytes("Assets/Resources/Varginha/MenuBackgroundV1.png"));
                replacement.LoadImage(File.ReadAllBytes("Assets/Resources/Varginha/MenuBackgroundFuscaAzul.png"));
                Assert.That(replacement.width,Is.EqualTo(original.width));Assert.That(replacement.height,Is.EqualTo(original.height));
                var a=original.GetPixels32();var b=replacement.GetPixels32();int inside=0,outside=0;
                var region=new RectInt(1336,69,336,217);
                for(int i=0;i<a.Length;i++)if(!a[i].Equals(b[i])){if(region.Contains(new Vector2Int(i%original.width,i/original.width)))inside++;else outside++;}
                Assert.That(inside,Is.GreaterThan(1000));Assert.That(outside,Is.Zero);
            }
            finally{Object.DestroyImmediate(original);Object.DestroyImmediate(replacement);}
        }

        [Test] public void LayeredUfoActuallyLeavesTheSkyAndFogMovesVisibly()
        {
            var source=Resources.Load<Texture2D>(MenuCinematicAtmosphere.BackgroundResource);
            var clean=Resources.Load<Texture2D>("Varginha/MenuUfoCleanPlate");Assert.That(clean,Is.Not.Null);
            var material=new Material(Resources.Load<Shader>("Varginha/MenuAtmosphere"));
            var target=RenderTexture.GetTemporary(source.width,source.height,0,RenderTextureFormat.ARGB32);
            try
            {
                material.SetTexture("_UfoClean",clean);material.SetTexture("_UfoSprite",Resources.Load<Texture2D>("Varginha/MenuUfoTransparent"));
                material.SetFloat("_UfoLayered",1);material.SetFloat("_Motion",1);
                material.SetFloat("_MenuTime",1.7f);material.SetFloat("_UfoPresence",1);
                Graphics.Blit(source,target,material);var present=Read(target);
                material.SetFloat("_UfoPresence",0);Graphics.Blit(source,target,material);var absent=Read(target);
                Graphics.Blit(clean,target);var cleanPixels=Read(target);
                int damagedCloudPixels=0;
                // This stationary sky area contains neither fog nor the moving cloud layer.
                // Departure must not turn the green cloud clusters into black speckles.
                for(int y=(int)(source.height*.64f);y<(int)(source.height*.92f);y++)
                    for(int x=(int)(source.width*.80f);x<(int)(source.width*.87f);x++)
                    {
                        int i=y*source.width+x;
                        if(cleanPixels[i].g>10 && absent[i].g<cleanPixels[i].g*.75f)damagedCloudPixels++;
                    }
                Assert.That(damagedCloudPixels,Is.Zero,"The departed beam must leave intact cloud colors");
                float ufoDifference=RegionDifference(present,absent,source.width,source.height,new Rect(.77f,.815f,.06f,.065f));
                Assert.That(ufoDifference,Is.GreaterThan(.02f),"The saucer must disappear, leaving actual sky behind it");
                material.SetFloat("_MenuTime",4.2f);Graphics.Blit(source,target,material);var later=Read(target);
                float mistDifference=RegionDifference(absent,later,source.width,source.height,new Rect(.4f,.26f,.27f,.12f));
                Assert.That(mistDifference,Is.GreaterThan(.008f),"Fog movement should be visible over a few seconds");
            }
            finally{Object.DestroyImmediate(material);RenderTexture.ReleaseTemporary(target);}
        }
        private static float RegionDifference(Color32[] a,Color32[] b,int width,int height,Rect region)
        {
            float total=0;int count=0;
            for(int y=(int)(region.yMin*height);y<(int)(region.yMax*height);y++)
                for(int x=(int)(region.xMin*width);x<(int)(region.xMax*width);x++)
                {int i=y*width+x;total+=(Mathf.Abs(a[i].r-b[i].r)+Mathf.Abs(a[i].g-b[i].g)+Mathf.Abs(a[i].b-b[i].b))/(3f*255);count++;}
            return total/count;
        }

        [Test] public void TransparentUfoHasRealAlphaAndClearBorders()
        {
            var texture=new Texture2D(2,2);
            try
            {
                texture.LoadImage(File.ReadAllBytes("Assets/Resources/Varginha/MenuUfoTransparent.png"));
                var pixels=texture.GetPixels32();int clear=0,visible=0;
                for(int i=0;i<pixels.Length;i++)
                {
                    if(pixels[i].a==0)clear++;else visible++;
                    int x=i%texture.width,y=i/texture.width;
                    if(x==0||x==texture.width-1||y==0||y==texture.height-1)Assert.That(pixels[i].a,Is.Zero,"No opaque rectangular edge");
                }
                Assert.That(clear,Is.GreaterThan(pixels.Length/4));Assert.That(visible,Is.GreaterThan(1000));
            }
            finally{Object.DestroyImmediate(texture);}
        }
    }

    public sealed class PixelHoverCarProbe : MonoBehaviour
    {
        public int Clicks;
        private void OnGUI()
        {
            VarginhaGamepadUI.Begin("hover-car-regression",true,9000);
            try{if(VarginhaGamepadUI.CarButton(new Rect(20,20,150,30),"Teste"))Clicks++;}
            finally{VarginhaGamepadUI.End();}
        }
    }
}
