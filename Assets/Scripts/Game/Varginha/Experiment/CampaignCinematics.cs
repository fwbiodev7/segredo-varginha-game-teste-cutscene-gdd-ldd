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
        {
            if (!Application.isPlaying) { SceneManager.LoadScene(scene); return; }
            if (Instance._loading) return;
            Instance.StartCoroutine(Instance.ChangeScene(scene));
        }
        private IEnumerator ChangeScene(string scene)
        {
            _loading = true; Time.timeScale = 0;
            float duration = VarginhaGameSettings.Current.reducedMotion ? .15f : .38f;
            for (float t=0;t<duration;t+=Time.unscaledDeltaTime) { _fade=Mathf.SmoothStep(0,1,t/duration); yield return null; }
            _fade=1; ReleasePause();
            var operation=SceneManager.LoadSceneAsync(scene);
            while (!operation.isDone) yield return null;
            Time.timeScale=1;
            // Allow scene actors, cameras and shared textures to settle behind the opaque frame.
            yield return null; yield return null;
            for (float t=0;t<duration;t+=Time.unscaledDeltaTime) { _fade=1-Mathf.SmoothStep(0,1,t/duration); yield return null; }
            _fade=0; _loading=false;
        }
        public static void Pause(bool paused)
        {
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
            if (_fade<=0) return;
            var matrix=GUI.matrix; var color=GUI.color; int depth=GUI.depth;
            GUI.matrix=Matrix4x4.identity; GUI.depth=-20000; GUI.color=new Color(0,0,0,_fade);
            GUI.DrawTexture(new Rect(0,0,Screen.width,Screen.height),Texture2D.whiteTexture);
            GUI.color=color; GUI.matrix=matrix; GUI.depth=depth;
        }
        private void ReleasePause() { if (_pauseFrame==null) return; _pauseFrame.Release(); Destroy(_pauseFrame); _pauseFrame=null; }
        private void OnDestroy() { SceneManager.sceneLoaded-=SceneLoaded; ReleasePause(); if (_blur!=null) Destroy(_blur); if (_instance==this) { _instance=null; Time.timeScale=1; } }
    }
}
