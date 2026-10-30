using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
namespace Game.Varginha.Experiment
{
    public sealed class CampaignNarration:MonoBehaviour
    {
        public static CampaignNarration Instance{get;private set;}
        public static bool Speaking=>Instance!=null&&Instance.source!=null&&Instance.source.isPlaying;
        AudioSource source; bool paused;int current;float delay;string scene;bool boxSeen;
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]static void Install()
        {if(Instance!=null)return;var go=new GameObject("Narração_da_campanha");DontDestroyOnLoad(go);go.AddComponent<CampaignNarration>();}
        void Awake(){Instance=this;source=gameObject.AddComponent<AudioSource>();source.playOnAwake=false;source.spatialBlend=0;SceneManager.sceneLoaded+=Loaded;Loaded(SceneManager.GetActiveScene(),LoadSceneMode.Single);}
        public static AudioClip Clip(int id)=>Resources.Load<AudioClip>("Varginha/Narration/Narration"+id.ToString("D2"));
        public static void Play(int id){if(Instance==null)Install();Instance.current=id;Instance.source.Stop();Instance.source.clip=Clip(id);if(Instance.source.clip!=null)Instance.source.Play();}
        public static void Stop(){if(Instance!=null){Instance.source.Stop();Instance.current=0;}}
        void Loaded(Scene value,LoadSceneMode mode){StopAllCoroutines();Stop();scene=value.name;delay=.7f;boxSeen=false;}
        void Update()
        {
            source.volume=.85f*VarginhaGameSettings.Current.voice;
            bool modal=Time.timeScale<=0||VarginhaGameHUD.Instance?.IsInventoryOpen==true;
            if(modal&&!paused){source.Pause();paused=true;}else if(!modal&&paused){source.UnPause();paused=false;}
            if(delay>0){delay-=Time.unscaledDeltaTime;if(delay<=0){int[] phases={2,4,6,11,13,15,18,21};int[] ids={5,2,3,11,8,9,14,10};for(int i=0;i<phases.Length;i++)if(scene==CampaignStorySave.Scene(phases[i])){Play(ids[i]);break;}}}
            var stage=VarginhaCampaignStage.Active;
            if(stage!=null&&!boxSeen&&stage.Progress.boxFound&&stage.Progress.phase==2){boxSeen=true;Play(1);}
        }
        void OnDestroy(){SceneManager.sceneLoaded-=Loaded;if(Instance==this)Instance=null;}
    }
}
