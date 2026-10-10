using Game.UI;
using Game.Varginha.Experiment;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Varginha
{
    [DisallowMultipleComponent]
    public sealed class MenuCinematicAtmosphere : MonoBehaviour
    {
        public const string BackgroundResource="Varginha/MenuBackgroundFuscaAzul";
        private Texture2D _source;
        private RenderTexture _frame;
        private Material _material;
        private AudioSource _bed,_effects;
        private MenuSuspenseClock _clock;
        private float _nextFrame,_nextSound,_navigationAt,_departure,_flightStarted;
        private int _activityFrame=-1;
        private bool _departing;
        private readonly System.Random _random=new System.Random(System.Environment.TickCount);
        public bool AllowSuspense { get; set; }=true;
        public bool PointerActive { get; private set; }=true;
        public Texture Background => _frame!=null?_frame:_source;
        public RenderTexture Frame => _frame;
        public bool TitleGlitch => !VarginhaGameSettings.Current.reducedMotion && _clock.TitleGlitch(Time.unscaledTime);
        public bool OwnsAudioSource(AudioSource source) => source==_bed||source==_effects;
        private void Awake()
        {
            _source=Resources.Load<Texture2D>(BackgroundResource)??Resources.Load<Texture2D>("Varginha/MenuBackgroundV1");
            var shader=Resources.Load<Shader>("Varginha/MenuAtmosphere");
            if(_source!=null && shader!=null && shader.isSupported)
            {
                _material=new Material(shader){hideFlags=HideFlags.DontSave};
                _frame=new RenderTexture(_source.width,_source.height,0,RenderTextureFormat.ARGB32)
                    {filterMode=FilterMode.Point,wrapMode=TextureWrapMode.Clamp,name="Menu — original art in motion"};
                _frame.Create();Graphics.Blit(_source,_frame);
            }
            _clock=new MenuSuspenseClock(_random.Next(),Time.unscaledTime);
            _flightStarted=Time.unscaledTime;
            var clean=Resources.Load<Texture2D>("Varginha/MenuUfoCleanPlate");
            var saucer=Resources.Load<Texture2D>("Varginha/MenuUfoTransparent");
            if(_material!=null&&clean!=null&&saucer!=null)
            {_material.SetTexture("_UfoClean",clean);_material.SetTexture("_UfoSprite",saucer);_material.SetFloat("_UfoLayered",1);}
            _nextSound=Time.unscaledTime+28;
            _bed=gameObject.AddComponent<AudioSource>();_bed.playOnAwake=false;_bed.loop=true;_bed.volume=0;
            _bed.clip=CampaignAtmosphereAudio.Bed("Road",0);_bed.Play();
            _effects=gameObject.AddComponent<AudioSource>();_effects.playOnAwake=false;_effects.volume=0;
        }
        public void NotifyActivity()
        {
            if(_activityFrame==Time.frameCount)return;
            _activityFrame=Time.frameCount;_clock.Activity(Time.unscaledTime);
        }
        public void BeginDeparture(){_departing=true;_departure=0;}
        public void Departure(float progress)=>_departure=Mathf.Clamp01(progress);
        public void Radio(float gain=.24f)=>_effects.PlayOneShot(CampaignSoundscape.Clip("RadioInterference"),gain);
        public void Navigate()
        {
            NotifyActivity();
            if(Time.unscaledTime<_navigationAt)return;
            _navigationAt=Time.unscaledTime+.1f;Radio(.14f);
        }
        private void Update()
        {
            float now=Time.unscaledTime;
            var settings=VarginhaGameSettings.Current;
            if(Mouse.current?.delta.ReadValue().sqrMagnitude>.25f || Mouse.current?.leftButton.wasPressedThisFrame==true)PointerActive=true;
            else if(VarginhaInputActions.UI("Navigate").ReadValue<Vector2>().sqrMagnitude>.16f
                || VarginhaInputActions.UI("Submit").WasPressedThisFrame())PointerActive=false;
            if(!_departing && (Keyboard.current?.anyKey.wasPressedThisFrame==true
                || Mouse.current?.delta.ReadValue().sqrMagnitude>.25f
                || Mouse.current?.leftButton.wasPressedThisFrame==true
                || Mouse.current?.scroll.ReadValue().sqrMagnitude>0
                || VarginhaInputActions.UI("Navigate").ReadValue<Vector2>().sqrMagnitude>.16f
                || VarginhaInputActions.UI("Submit").WasPressedThisFrame()
                || VarginhaInputActions.CancelPressed))NotifyActivity();
            bool eventStarted=_clock.Tick(now,AllowSuspense&&!_departing&&!settings.reducedMotion);
            if(eventStarted)Radio(.25f);
            if(!_departing && now>=_nextSound)
            {
                _nextSound=now+28+(float)_random.NextDouble()*29;
                if(AllowSuspense)_effects.PlayOneShot(_random.Next(2)==0?CampaignAtmosphereAudio.Detail():CampaignSoundscape.Clip("RadioInterference"),.21f);
            }
            _bed.volume=.23f*settings.music*(1-Mathf.SmoothStep(0,1,_departure));
            _effects.volume=.23f*settings.effects;
            if(_material==null || now<_nextFrame)return;
            var flight=MenuUfoFlight.Sample(now-_flightStarted);
            // Fixed 60 Hz budget without losing every other frame to timing jitter.
            _nextFrame=Mathf.Max(_nextFrame+1f/(settings.reducedMotion?12:60),now);
            _material.SetVector("_UfoOffset",new Vector4(flight.Offset.x,flight.Offset.y,0,0));
            _material.SetFloat("_UfoPresence",flight.Presence);
            _material.SetFloat("_UfoTrail",flight.Trail);
            _material.SetFloat("_MenuTime",now);
            _material.SetFloat("_Motion",settings.reducedMotion?0:1);
            _material.SetFloat("_Gaze",_clock.Gaze(now));
            _material.SetFloat("_Interference",settings.reducedMotion?0:(_clock.Glitch(now)?.6f:0)+(_departing?_departure*.6f:0));
            Graphics.Blit(_source,_frame,_material);
        }
        public void DrawInterference()
        {
            if(VarginhaGameSettings.Current.reducedMotion || !_clock.Glitch(Time.unscaledTime))return;
            var before=GUI.color;
            GUI.color=new Color(.47f,.7f,.69f,.095f);
            float y=Mathf.Round((Time.unscaledTime*263)%Screen.height);
            GUI.DrawTexture(new Rect(0,y,Screen.width,2),Texture2D.whiteTexture);
            GUI.DrawTexture(new Rect(Screen.width*.65f,y+6,Screen.width*.26f,1),Texture2D.whiteTexture);
            GUI.color=before;
        }
        private void OnDisable(){_bed?.Stop();_effects?.Stop();}
        private void OnDestroy()
        {
            if(_frame!=null){_frame.Release();Destroy(_frame);}
            if(_material!=null)Destroy(_material);
        }
    }
}
