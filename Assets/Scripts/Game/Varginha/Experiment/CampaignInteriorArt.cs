using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Varginha.Experiment
{
    public static class CampaignInteriorArt
    {
        [Serializable]public sealed class Entry{public string name,sheet;public int x,y,width,height;}
        [Serializable]public sealed class Manifest{public Entry[] furniture;}
        private static readonly Dictionary<string,Manifest> Manifests=new();
        private static readonly Dictionary<string,Sprite> Sprites=new();
        private static Material _material;
        public static bool Contains(string motif)=>motif.StartsWith("Child90_")||motif.StartsWith("Church_");
        public static Material Material
        {
            get
            {
                if(_material!=null)return _material;
                _material=new Material(Resources.Load<Shader>("Varginha/Flash1996/CrispPixelSprite"));
                _material.name="Interiores — pixels sem halo";_material.SetFloat("_Cutoff",220f/255);
                return _material;
            }
        }
        public static Sprite Prop(string motif)
        {
            if(!Contains(motif))return null;
            if(Sprites.TryGetValue(motif,out var sprite)&&sprite!=null)return sprite;
            string kit=motif.StartsWith("Child90_")?"Childhood1996":"ChurchArt";
            if(!Manifests.TryGetValue(kit,out var manifest))
            {
                var source=Resources.Load<TextAsset>("Varginha/"+kit+"/Atlas");if(source==null)return null;
                manifest=JsonUtility.FromJson<Manifest>(source.text);Manifests[kit]=manifest;
            }
            var entry=Array.Find(manifest.furniture,e=>e.name==motif);if(entry==null)return null;
            var texture=Resources.Load<Texture2D>("Varginha/"+kit+"/"+entry.sheet);
            sprite=Sprite.Create(texture,new Rect(entry.x,entry.y,entry.width,entry.height),Vector2.one/2,64,0,SpriteMeshType.FullRect);
            sprite.name=motif;Sprites[motif]=sprite;return sprite;
        }
    }
}
