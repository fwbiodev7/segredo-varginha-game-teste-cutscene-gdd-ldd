using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using Unity.Profiling;
using UnityEngine.SceneManagement;

namespace Game.Varginha.Experiment
{
    [DefaultExecutionOrder(120)]
    public sealed class CampaignAtmosphereDirector : MonoBehaviour
    {
        public static CampaignAtmosphereDirector Active { get; private set; }
        private static bool _hasPrevious;
        private static float _previousTension;
        private static readonly ProfilerMarker UpdateMarker = new("Campaign.Atmosphere");
        private CampaignContinuationController _continuation;
        private CampaignExpansionController _expansion;
        private VarginhaCampaignStage _stage;
        private VarginhaCampaignPhase1 _opening;
        private VarginhaCampaignDrive _drive;
        private EdelzioTopDownController _actor;
        private CampaignAmbientBridge _bridge;
        private Camera _camera;
        private UniversalAdditionalCameraData _cameraData;
        private bool _previousPost;
        private LayerMask _previousMask;
        private Volume _volume;
        private VolumeProfile _profile;
        private ColorAdjustments _color;
        private WhiteBalance _balance;
        private Vignette _vignette;
        private FilmGrain _grain;
        private LensDistortion _distortion;
        private AudioSource _detail;
        private readonly SpriteRenderer[] _dust = new SpriteRenderer[16];
        private Sprite _dustSprite;
        private int _phase, _area, _band = -1;
        private string _environment;
        private float _clock, _pulseTime = 5, _pulseStrength, _nextDetail = 28, _poll;
        private CampaignAtmosphereProfile _target;
        public float Tension { get; private set; }
        public float Clock => _clock;
        public float PulseWeight { get; private set; }
        public Volume AtmosphereVolume => _volume;
        public int Phase => _phase;
        public int Area => _area;
        public bool OwnsAtmosphereSource(AudioSource source) => source == _detail;
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetState()
        {
            Active = null; _hasPrevious = false; _previousTension = 0;
            SceneManager.sceneLoaded -= ClearBetweenCampaigns; SceneManager.sceneLoaded += ClearBetweenCampaigns;
        }
        private static void ClearBetweenCampaigns(Scene scene, LoadSceneMode mode)
        {
            if (mode == LoadSceneMode.Single && CampaignAtmosphereProfile.ScenePhase(scene.name) == 0)
            { _hasPrevious = false; _previousTension = 0; }
        }
        public void Configure(string environment, EdelzioTopDownController actor)
        {
            Active = this; _environment = environment; _actor = actor;
            _continuation = GetComponent<CampaignContinuationController>(); _expansion = GetComponent<CampaignExpansionController>();
            _stage = GetComponent<VarginhaCampaignStage>(); _opening = GetComponent<VarginhaCampaignPhase1>(); _drive = GetComponent<VarginhaCampaignDrive>();
            _phase = _continuation != null ? _continuation.phase : _expansion != null ? _expansion.phase : _stage != null ? _stage.phase : _opening != null ? 1 : _drive != null ? 3 : CampaignAtmosphereProfile.ScenePhase(gameObject.scene.name);
            _area = _continuation != null ? _continuation.area : 0;
            if (_phase == 0) { enabled = false; return; }
            _target = Resolve(); Tension = _hasPrevious ? _previousTension : _target.tension; _hasPrevious = true;
            _bridge = CampaignAmbientBridge.Instance;
            _camera = Camera.main;
            if (_camera != null && GraphicsSettings.currentRenderPipeline is UniversalRenderPipelineAsset)
            {
                _cameraData = _camera.GetUniversalAdditionalCameraData(); _previousPost = _cameraData.renderPostProcessing; _previousMask = _cameraData.volumeLayerMask;
                var volumeObject = new GameObject("Atmosfera_cinematografica"); volumeObject.transform.SetParent(transform, false);
                _volume = volumeObject.AddComponent<Volume>(); _volume.isGlobal = true; _volume.priority = 10;
                _profile = ScriptableObject.CreateInstance<VolumeProfile>(); _profile.name = "Atmosfera runtime da fase " + CampaignSequence.Chapter(_phase);
                _profile.hideFlags = HideFlags.DontSave; _volume.sharedProfile = _profile;
                _color = _profile.Add<ColorAdjustments>(true); _balance = _profile.Add<WhiteBalance>(true); _vignette = _profile.Add<Vignette>(true);
                _grain = _profile.Add<FilmGrain>(true); _distortion = _profile.Add<LensDistortion>(true);
                _vignette.smoothness.value = .55f; _vignette.color.value = new Color(.025f, .03f, .04f);
                _grain.type.value = FilmGrainLookup.Thin1; _grain.response.value = .8f;
                _cameraData.renderPostProcessing = true; _cameraData.volumeLayerMask |= 1 << volumeObject.layer;
            }
            _detail = gameObject.AddComponent<AudioSource>(); _detail.playOnAwake = false; _detail.spatialBlend = 0; _detail.dopplerLevel = 0;
            _detail.clip = CampaignAtmosphereAudio.Detail();
            _nextDetail += _phase % 7;
            if (_camera != null)
            {
                _dustSprite = Sprite.Create(Texture2D.whiteTexture, new Rect(0, 0, Texture2D.whiteTexture.width, Texture2D.whiteTexture.height), Vector2.one * .5f, Texture2D.whiteTexture.width * 32);
                for (int i = 0; i < _dust.Length; i++)
                {
                    var go = new GameObject("Poeira_ambiental_" + i); go.transform.SetParent(transform, false);
                    _dust[i] = go.AddComponent<SpriteRenderer>(); _dust[i].sprite = _dustSprite; _dust[i].sortingOrder = 30000;
                }
            }
            Advance(0); Apply();
        }
        private CampaignAtmosphereProfile Resolve()
            => CampaignAtmosphereProfile.For(_phase, _area, _continuation != null ? _continuation.Progress : _expansion != null ? _expansion.Progress : _stage != null ? _stage.Progress : _drive != null ? _drive.Progress : null, _opening != null ? _opening.Memory : null);
        public static void Interference(float strength = .35f)
        {
            if (Active != null && Active.isActiveAndEnabled && Time.timeScale > 0)
            { Active._pulseStrength = Mathf.Clamp01(strength); Active._pulseTime = 0; }
        }
        private void Update()
        {
            using var sample = UpdateMarker.Auto();
            bool frozen = Time.timeScale <= 0 || CampaignCinematics.IsTransitioning;
            if (_detail != null) { if (frozen) _detail.Pause(); else _detail.UnPause(); }
            if (!frozen) Advance(Time.deltaTime);
            Apply();
        }
        public void Advance(float delta)
        {
            delta = Mathf.Max(0, delta); _clock += delta; _pulseTime += delta; _poll -= delta;
            if (_poll <= 0)
            {
                var target = Resolve();
                if (target.tension > _target.tension + .015f) Interference(.16f + target.tension * .3f);
                _target = target; _poll = .25f;
                if (_bridge != null && _band != _target.AudioBand)
                { _band = _target.AudioBand; _bridge.Transition(CampaignAtmosphereAudio.Bed(_environment, _band), 3); }
            }
            Tension = Mathf.MoveTowards(Tension, _target.tension, delta * .18f); _previousTension = Tension;
            PulseWeight = _pulseTime < 5 ? Mathf.Sin(Mathf.PI * Mathf.Clamp01(_pulseTime / 5)) * _pulseStrength : 0;
            if (_clock >= _nextDetail)
            {
                _nextDetail = _clock + 28 + _phase % 11;
                if (Tension >= .3f && !(_actor != null && _actor.IsInputLocked))
                {
                    if (_detail != null) _detail.Play();
                    if (Tension >= .7f) Interference(.12f);
                }
            }
        }
        private void Apply()
        {
            bool reduced = VarginhaGameSettings.Current.reducedMotion;
            float breathing = reduced || Tension < .5f ? 0 : Mathf.Sin(_clock * .37f) * .008f * Tension;
            float pulse = reduced ? 0 : PulseWeight;
            if (_volume != null)
            {
                _color.contrast.value = Mathf.Lerp(0, 11, Tension); _color.saturation.value = Mathf.Lerp(0, -13, Tension);
                _balance.temperature.value = Mathf.Lerp(3, _phase == 8 || _phase == 14 ? -5 : -15, Tension);
                _vignette.intensity.value = Mathf.Clamp(.025f + .185f * Tension + breathing + pulse * .018f, .025f, .24f);
                _grain.intensity.value = Tension < .55f ? 0 : pulse * .055f;
                _distortion.intensity.value = Tension < .7f ? 0 : Mathf.Sin(_clock * .8f) * pulse * .014f;
            }
            if (_detail != null) _detail.volume = Mathf.Lerp(.08f, .32f, Tension) * VarginhaGameSettings.Current.music * (_actor != null && _actor.IsInputLocked ? .35f : 1);
            _bridge?.AtmosphereGain(Mathf.Lerp(.85f, 1.5f, Tension));
            if (_camera == null) return;
            float height = _camera.orthographicSize * 2, width = height * _camera.aspect;
            for (int i = 0; i < _dust.Length; i++)
            {
                var dust = _dust[i]; if (dust == null) continue;
                dust.enabled = !reduced;
                float x = Mathf.Repeat(i * .618034f + _clock * .0015f, 1), y = Mathf.Repeat(i * .381966f + _clock * .005f, 1);
                var pos = _camera.transform.position + new Vector3((x - .5f) * width, (y - .5f) * height, -_camera.transform.position.z);
                pos.x = Mathf.Round(pos.x * 32) / 32; pos.y = Mathf.Round(pos.y * 32) / 32; dust.transform.position = pos;
                dust.color = new Color(.74f, .79f, .78f, _target.dust * (.5f + .5f * Mathf.Sin(_clock * .16f + i)));
            }
        }
        private void OnEnable()
        {
            if (_volume != null) _volume.enabled = true;
            if (_cameraData != null) { _cameraData.renderPostProcessing = true; _cameraData.volumeLayerMask |= 1 << _volume.gameObject.layer; }
        }
        private void OnDisable()
        {
            if (_volume != null) _volume.enabled = false;
            if (_cameraData != null) { _cameraData.renderPostProcessing = _previousPost; _cameraData.volumeLayerMask = _previousMask; }
            foreach (var dust in _dust) if (dust != null) dust.enabled = false;
            if (_detail != null) _detail.Stop();
            _bridge?.AtmosphereGain(1);
        }
        private void OnDestroy()
        {
            if (Active == this) Active = null;
            if (_profile != null) { foreach (var component in _profile.components) Destroy(component); Destroy(_profile); }
            if (_dustSprite != null) Destroy(_dustSprite);
        }
    }
}
