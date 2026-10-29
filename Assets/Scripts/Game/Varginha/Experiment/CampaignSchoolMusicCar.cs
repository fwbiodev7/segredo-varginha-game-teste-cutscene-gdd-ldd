using UnityEngine;

namespace Game.Varginha.Experiment
{
    // The arrival is drawn in map pixels; its car owns the corresponding spatial audio source.
    public sealed class CampaignSchoolMusicCar:MonoBehaviour
    {
        public const string MusicResource="Varginha/Revision20261028/OEdelzio";
        public const float StartTime=.35f,EndTime=5.1f;
        AudioSource _source;
        float _started;
        bool _paused;
        public AudioSource Source=>_source;
        public static float Progress(float time)=>Mathf.Clamp01((time-StartTime)/(EndTime-StartTime));
        public static Vector2 CarPixel(float time)=>new(Mathf.Lerp(-150,1538,Progress(time)),1060);
        public void Begin()
        {
            Stop();_started=Time.time;_paused=false;
            _source=gameObject.GetComponent<AudioSource>()??gameObject.AddComponent<AudioSource>();
            _source.playOnAwake=false;_source.loop=false;_source.clip=Resources.Load<AudioClip>(MusicResource);
            _source.spatialBlend=1;_source.spread=0;_source.dopplerLevel=.12f;
            _source.minDistance=3;_source.maxDistance=24;_source.rolloffMode=AudioRolloffMode.Linear;
            _source.velocityUpdateMode=AudioVelocityUpdateMode.Dynamic;
            _source.volume=0;Place(0);if(_source.clip!=null)_source.Play();
        }
        void Place(float time)
        {
            var listener=Camera.main;if(listener==null)return;
            // Same horizontal map trajectory, relative to the listener of this inset shot.
            var pixel=CarPixel(time);
            transform.position=listener.transform.position+new Vector3((pixel.x-694)*.025f,-3,0);
        }
        void Update()
        {
            if(_source==null)return;
            bool pause=Time.timeScale<=0;
            if(pause!=_paused){_paused=pause;if(pause)_source.Pause();else _source.UnPause();}
            if(pause)return;
            float time=Time.time-_started;
            Place(time);
            float fade=Mathf.Min(Mathf.Clamp01((time-StartTime)/.5f),Mathf.Clamp01((EndTime-time)/.6f));
            _source.volume=.95f*VarginhaGameSettings.Current.music*fade;
            if(time>=EndTime)Stop();
        }
        public void Stop(){if(_source!=null){_source.Stop();_source.clip=null;_source.volume=0;}}
        void OnDisable()=>Stop();
    }
}
