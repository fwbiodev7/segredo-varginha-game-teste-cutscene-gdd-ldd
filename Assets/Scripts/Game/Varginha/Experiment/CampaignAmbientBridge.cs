using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.Varginha.Experiment
{
    // Two persistent channels keep the outgoing bed alive through asynchronous scene activation.
    public sealed class CampaignAmbientBridge : MonoBehaviour
    {
        private static CampaignAmbientBridge _instance;
        private readonly AudioSource[] _channels = new AudioSource[2];
        private int _incoming;
        private float _elapsed, _duration = .75f, _duck = 1, _duckTarget = 1;
        private float _start0, _start1;
        private float _base0, _base1;
        private bool _suspended;
        private float _atmosphereGain = 1;
        private AudioClip _requested;
        public static CampaignAmbientBridge Instance
        {
            get
            {
                if (_instance != null) return _instance;
                var go = new GameObject("Continuidade_do_ambiente");
                _instance = go.AddComponent<CampaignAmbientBridge>(); DontDestroyOnLoad(go); return _instance;
            }
        }
        private void Awake()
        {
            SceneManager.sceneLoaded += SceneLoaded;
            EnsureChannels();
        }
        private void EnsureChannels()
        {
            if (_channels[0] != null && _channels[1] != null) return;
            var existing = GetComponents<AudioSource>();
            for (int i = 0; i < 2; i++)
            {
                _channels[i] = i < existing.Length ? existing[i] : gameObject.AddComponent<AudioSource>();
                _channels[i].loop = true; _channels[i].playOnAwake = false; _channels[i].volume = 0;
            }
        }
        private void OnEnable()
        {
            // Reconnect retained native sources after an Editor script reload.
            EnsureChannels(); _instance = this;
            SceneManager.sceneLoaded -= SceneLoaded; SceneManager.sceneLoaded += SceneLoaded;
        }
        private void SceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.name == "Menu_MisterioDeVarginha") { Suspend(false); Duck(0); Transition(null); }
        }
        public void Transition(AudioClip clip, float duration = .75f)
        {
            EnsureChannels();
            if (_requested == clip) return;
            _requested = clip; _start0 = _base0; _start1 = _base1;
            // Reuse the quieter channel when a second request interrupts a blend.
            _incoming = _start0 <= _start1 ? 0 : 1;
            if (_incoming == 0) _start0 = 0; else _start1 = 0;
            _channels[_incoming].Stop(); _channels[_incoming].clip = clip;
            _channels[_incoming].volume = 0;
            if (clip != null) _channels[_incoming].Play();
            if (_suspended) _channels[_incoming].Pause();
            _elapsed = 0; _duration = Mathf.Max(.1f, duration);
        }
        public void Duck(float weight) => _duckTarget = Mathf.Lerp(1, .21f, Mathf.Clamp01(weight));
        public void AtmosphereGain(float gain) => _atmosphereGain = Mathf.Clamp(gain, .5f, 1.5f);
        public void Suspend(bool value)
        {
            _suspended = value;
            foreach (var channel in _channels) { if (value) channel.Pause(); else channel.UnPause(); }
        }
        private void Update()
        {
            if (_suspended) return;
            _elapsed += Time.unscaledDeltaTime;
            _duck = Mathf.MoveTowards(_duck, _duckTarget, Time.unscaledDeltaTime * 2);
            float p = Mathf.SmoothStep(0, 1, Mathf.Clamp01(_elapsed / _duration));
            for (int i = 0; i < 2; i++)
            {
                float start = i == 0 ? _start0 : _start1;
                float end = i == _incoming && _requested != null ? .12f : 0;
                float mixed = Mathf.Lerp(start, end, p);
                if (i == 0) _base0 = mixed; else _base1 = mixed;
                _channels[i].volume = mixed * _duck * _atmosphereGain * VarginhaGameSettings.Current.music;
                if (p >= 1 && i != _incoming && _channels[i].isPlaying) _channels[i].Stop();
            }
        }
        private void OnDestroy() { SceneManager.sceneLoaded -= SceneLoaded; if (_instance == this) _instance = null; }
    }
}
