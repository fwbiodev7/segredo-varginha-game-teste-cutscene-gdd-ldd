using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Varginha.Experiment
{
    // Received recordings are additive. Unmapped IDs retain the existing generated fallback.
    public static class CampaignRecordedAudio
    {
        [Serializable]public sealed class Entry{public string id,path,source;public float gain=1,seconds;public bool loop;}
        [Serializable]private sealed class Catalog{public Entry[] entries;}
        private static readonly Dictionary<string,Entry> Entries=new();
        private static readonly Dictionary<string,AudioClip> Clips=new();private static int _foot;private static bool _loaded;
        private static void LoadCatalog(string resourcePath)
        {
            var text=Resources.Load<TextAsset>(resourcePath);if(text==null)return;
            var catalog=JsonUtility.FromJson<Catalog>(text.text);if(catalog?.entries==null)return;
            foreach(var entry in catalog.entries)Entries[entry.id]=entry;
        }
        private static void Load()
        {
            if(_loaded)return;_loaded=true;
            // Os pacotes recebidos são aditivos: o pacote novo pode substituir apenas
            // um ID explicitamente remapeado, mantendo os efeitos aprovados anteriores.
            LoadCatalog("Varginha/Audio/Received20261021/Catalog");
            LoadCatalog("Varginha/Audio/Received20261022/Catalog");
        }
        public static Entry Definition(string id){Load();return Entries.TryGetValue(id,out var entry)?entry:null;}
        public static IReadOnlyCollection<Entry> Definitions{get{Load();return Entries.Values;}}
        public static AudioClip Clip(string id)
        {
            if(Clips.TryGetValue(id,out var clip)&&clip!=null)return clip;
            var definition=Definition(id);if(definition==null)return null;
            clip=Resources.Load<AudioClip>(definition.path);if(clip!=null)Clips[id]=clip;return clip;
        }
        public static float Gain(string id)=>Definition(id)?.gain??1;
        public static string NextFoot(string id)
        {
            if(id!="FootWood"&&id!="FootNature")return id;
            int variant=_foot++%3;return variant==0?id:id+(variant+1);
        }
        public static AudioClip Ambient(string environment,int band)
            =>environment=="Forest"?Clip("ForestAmbience"):environment=="School"&&band==0?Clip("SchoolAmbience"):null;
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Clear(){Clips.Clear();Entries.Clear();_loaded=false;_foot=0;}
    }
    // The persistent punch source must respect pause and cannot leak a tail into another scene.
    public sealed class CampaignTransientAudioGuard:MonoBehaviour
    {
        private AudioSource _source;private bool _paused;
        private void Awake(){_source=GetComponent<AudioSource>();UnityEngine.SceneManagement.SceneManager.activeSceneChanged+=SceneChanged;}
        private void SceneChanged(UnityEngine.SceneManagement.Scene a,UnityEngine.SceneManagement.Scene b)=>_source?.Stop();
        private void Update(){bool paused=Time.timeScale<=0;if(paused==_paused)return;_paused=paused;if(paused)_source.Pause();else _source.UnPause();}
        private void OnDestroy()=>UnityEngine.SceneManagement.SceneManager.activeSceneChanged-=SceneChanged;
    }
}
