using System.Collections.Generic;
using UnityEngine;

namespace Game.Varginha.Experiment
{
    // Only newly generated cinematic scenery layers pass through this resolver.
    // The approved protagonist/car/evidence atlases are never filtered or recoloured.
    public static class CampaignCinemaTextures
    {
        private static readonly Dictionary<string,RenderTexture> Cache=new();
        private static Material _material;
        public static Texture Resolve(string name)
        {
            if(Cache.TryGetValue(name,out var cached)&&cached!=null)return cached;
            var source=Resources.Load<Texture2D>("Varginha/CinemaOpening/"+name);if(source==null)return null;
            var shader=Resources.Load<Shader>("Varginha/CinemaOpening/CinematicPixelResolve");
            if(shader==null||!shader.isSupported)return source;
            _material??=new Material(shader);
            int height=Mathf.RoundToInt(source.height/4f);
            int width=Mathf.RoundToInt(source.width/4f);
            var rt=new RenderTexture(width,height,0,RenderTextureFormat.ARGB32)
            {name="Cinema_Pixel_"+name,filterMode=FilterMode.Point,wrapMode=TextureWrapMode.Clamp,useMipMap=false};
            rt.Create();var previous=RenderTexture.active;
            try{Graphics.Blit(source,rt,_material);}
            finally{RenderTexture.active=previous;}
            Cache[name]=rt;return rt;
        }
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        public static void Clear()
        {
            foreach(var rt in Cache.Values)if(rt!=null){rt.Release();if(Application.isPlaying)Object.Destroy(rt);else Object.DestroyImmediate(rt);}
            Cache.Clear();if(_material!=null){if(Application.isPlaying)Object.Destroy(_material);else Object.DestroyImmediate(_material);}_material=null;
        }
    }
}
