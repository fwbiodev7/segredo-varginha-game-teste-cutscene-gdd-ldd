using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.Varginha.Experiment
{
    // Transitions run on unscaled time. Pause captures once, at 90px high, matching the display ratio.
    public sealed class CampaignCinematics : MonoBehaviour
    {
        private static CampaignCinematics _instance;
        private float _fade;
        private RenderTexture _pauseFrame;
        private Material _blur;
        private bool _loading;
        private RenderTexture _transitionFrame;
        private Rect _transitionRect;
        private CampaignTransitionProfile _profile;
        private float _transitionProgress, _bars;
        private void Awake() => SceneManager.sceneLoaded += SceneLoaded;
        private void SceneLoaded(Scene scene, LoadSceneMode mode) => ReleasePause();
        public static bool IsTransitioning => _instance != null && _instance._loading;
        private static CampaignCinematics Instance
        {
            get
            {
                if (_instance == null)
                {
                    var go = new GameObject("Transições_da_história");
                    _instance = go.AddComponent<CampaignCinematics>(); DontDestroyOnLoad(go);
                }
                return _instance;
            }
        }
        public static void Load(string scene)
            => Load(scene, CampaignTransitionProfile.ForScene(scene));
        public static void Load(string scene, CampaignTransitionProfile profile)
        {
            if (!Application.isPlaying) { SceneManager.LoadScene(scene); return; }
            if (Instance._loading) return;
            if (!Application.CanStreamedLevelBeLoaded(scene)) { Debug.LogError("Cena da campanha ausente: " + scene); return; }
            Instance.StartCoroutine(Instance.ChangeScene(scene, profile));
        }
        private IEnumerator ChangeScene(string scene, CampaignTransitionProfile profile)
        {
            _loading = true; _profile = profile;
            bool reduced = VarginhaGameSettings.Current.reducedMotion;
            if (reduced) { _profile.style = CampaignTransitionStyle.Fade; _profile.letterbox = false; }
            float duration = reduced ? .15f : Mathf.Clamp(profile.duration, .1f, 1.5f);
            try
            {
                var ambient = CampaignAmbientBridge.Instance;
                ambient.Suspend(false); ambient.Duck(0);
                ambient.Transition(string.IsNullOrEmpty(profile.ambience) ? null : CampaignSoundscape.Clip(profile.ambience + "Ambience"));
                if (_profile.style == CampaignTransitionStyle.CameraPan && Time.timeScale > 0)
                {
                    var camera = Camera.main;
                    if (camera != null) CampaignCameraDirector.Reveal(camera.transform.position + Vector3.right * .7f, .9f);
                    for (float t = 0; t < .25f; t += Time.unscaledDeltaTime) { _bars = Mathf.SmoothStep(0, 1, t / .25f); yield return null; }
                }
                Time.timeScale = 0; ReleasePause();
                bool crossfade = _profile.style == CampaignTransitionStyle.Crossfade || _profile.style == CampaignTransitionStyle.CameraPan;
                if (crossfade) CaptureTransition();
                if (_transitionFrame == null)
                    for (float t=0;t<duration;t+=Time.unscaledDeltaTime) { _fade=Mathf.SmoothStep(0,1,t/duration); yield return null; }
                _fade = _transitionFrame == null ? 1 : 0; _transitionProgress = 0;
                var operation=SceneManager.LoadSceneAsync(scene);
                while (!operation.isDone) yield return null;
                Time.timeScale=1;
                // New controllers settle behind the captured image or opaque fade.
                yield return null; yield return null;
                for (float t=0;t<duration;t+=Time.unscaledDeltaTime)
                {
                    _transitionProgress = Mathf.SmoothStep(0,1,t/duration);
                    _fade = _transitionFrame == null ? 1 - _transitionProgress : 0;
                    _bars = _profile.letterbox ? 1 - _transitionProgress : 0;
                    yield return null;
                }
            }
            finally
            {
                _fade = _bars = 0; _loading = false; Time.timeScale = 1; ReleaseTransition();
            }
        }
        private void CaptureTransition()
        {
            var camera = Camera.main; if (camera == null) return;
            // Capture only the world: existing menus, objectives and inventory keep one live copy.
            var viewport = camera.pixelRect;
            _transitionRect = new Rect(viewport.x, Screen.height - viewport.yMax, viewport.width, viewport.height);
            int height = Mathf.Min(540, Mathf.Max(1, camera.pixelHeight));
            int width = Mathf.Max(1, Mathf.RoundToInt(height * camera.aspect));
            _transitionFrame = new RenderTexture(width, height, 16, RenderTextureFormat.ARGB32) { filterMode = FilterMode.Point };
            _transitionFrame.Create();
            var target = camera.targetTexture; var active = RenderTexture.active; var rect = camera.rect;
            try { camera.targetTexture = _transitionFrame; camera.rect = new Rect(0, 0, 1, 1); camera.Render(); }
            finally { camera.targetTexture = target; camera.rect = rect; RenderTexture.active = active; }
        }
        public static void Pause(bool paused)
        {
            if(paused)VarginhaRumble.Stop();
            if (!paused) { if (_instance != null) _instance.ReleasePause(); return; }
            Instance.CapturePause();
        }
        private void CapturePause()
        {
            ReleasePause(); var camera=Camera.main; if (camera==null) return;
            _blur ??=new Material(Resources.Load<Shader>("Varginha/IllustratedMaps/PausePixelBlur")) { hideFlags=HideFlags.DontSave };
            int width=Mathf.Clamp(Mathf.RoundToInt(90f*Screen.width/Mathf.Max(1,Screen.height)),90,384);
            _pauseFrame=new RenderTexture(width,90,16,RenderTextureFormat.ARGB32) { filterMode=FilterMode.Point, name="Pausa congelada em pixels" }; _pauseFrame.Create();
            var temporary=RenderTexture.GetTemporary(width,90,16,RenderTextureFormat.ARGB32);
            var target=camera.targetTexture; var active=RenderTexture.active;
            try { camera.targetTexture=temporary; camera.Render(); Graphics.Blit(temporary,_pauseFrame,_blur); }
            finally { camera.targetTexture=target; RenderTexture.active=active; RenderTexture.ReleaseTemporary(temporary); }
        }
        public static void DrawPauseBackdrop()
        {
            var matrix=GUI.matrix;
            GUI.matrix=Matrix4x4.identity;
            var screen=new Rect(0,0,Screen.width,Screen.height);
            if (_instance?._pauseFrame != null) GUI.DrawTexture(screen,_instance._pauseFrame,ScaleMode.StretchToFill);
            ExperimentGUI.Box(screen,new Color(.005f,.015f,.025f,.48f));
            GUI.matrix=matrix;
        }
        public static Color ChapterBlack(float elapsed, float total=3)
            => new(0,0,0,1-Mathf.SmoothStep(0,1,Mathf.InverseLerp(total-.6f,total,elapsed)));
        private void OnGUI()
        {
            if (!_loading && _fade<=0) return;
            var matrix=GUI.matrix; var color=GUI.color; int depth=GUI.depth;
            GUI.matrix=Matrix4x4.identity; GUI.depth=-20000;
            if (_transitionFrame != null)
            {
                GUI.color = new Color(1,1,1,1 - _transitionProgress);
                GUI.DrawTexture(_transitionRect, _transitionFrame, ScaleMode.StretchToFill);
            }
            if (_fade > 0)
            {
                GUI.color=new Color(0,0,0,_fade);
                GUI.DrawTexture(new Rect(0,0,Screen.width,Screen.height),Texture2D.whiteTexture);
            }
            if (_profile.style == CampaignTransitionStyle.Paranormal && _fade > .3f && _fade < .75f)
            {
                GUI.color = new Color(.35f,.7f,.72f,.055f);
                float y = Mathf.Round(Screen.height * .43f);
                GUI.DrawTexture(new Rect(0,y,Screen.width,2),Texture2D.whiteTexture);
            }
            if (_bars > 0)
            {
                GUI.color = new Color(0,0,0,_bars); float height = Screen.height * .028f;
                GUI.DrawTexture(new Rect(0,0,Screen.width,height),Texture2D.whiteTexture);
                GUI.DrawTexture(new Rect(0,Screen.height-height,Screen.width,height),Texture2D.whiteTexture);
            }
            GUI.color=color; GUI.matrix=matrix; GUI.depth=depth;
        }
        private void ReleasePause() { if (_pauseFrame==null) return; _pauseFrame.Release(); Destroy(_pauseFrame); _pauseFrame=null; }
        private void ReleaseTransition() { if (_transitionFrame == null) return; _transitionFrame.Release(); Destroy(_transitionFrame); _transitionFrame = null; }
        private void OnDestroy() { SceneManager.sceneLoaded-=SceneLoaded; ReleasePause(); ReleaseTransition(); if (_blur!=null) Destroy(_blur); if (_instance==this) { _instance=null; Time.timeScale=1; } }
    }
}
