using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Varginha.Experiment
{
    // The botanist's home uses an exclusive kit, independent from other maps.
    public static class CampaignOuzanaArt
    {
        [Serializable]public sealed class Entry
        {
            public string name,sheet;
            public int x,y,width,height;
        }
        [Serializable]public sealed class Manifest{public Entry[] furniture;}
        private static Manifest _manifest;
        private static readonly Dictionary<string,Sprite> Cache=new();
        private static Material _material;
        public static Material Material
        {
            get
            {
                if(_material!=null)return _material;
                _material=new Material(Resources.Load<Shader>("Varginha/Flash1996/CrispPixelSprite"));
                _material.name="Ouzana — pixels sem halo";_material.SetFloat("_Cutoff",220f/255);
                return _material;
            }
        }
        public static Sprite Prop(string motif)
        {
            if(!motif.StartsWith("Ouzana_"))return null;
            if(Cache.TryGetValue(motif,out var sprite)&&sprite!=null)return sprite;
            _manifest??=JsonUtility.FromJson<Manifest>(Resources.Load<TextAsset>("Varginha/OuzanaArt/Atlas").text);
            var entry=Array.Find(_manifest.furniture,e=>e.name==motif);
            if(entry==null)return null;
            var texture=Resources.Load<Texture2D>("Varginha/OuzanaArt/"+entry.sheet);
            sprite=Sprite.Create(texture,new Rect(entry.x,entry.y,entry.width,entry.height),Vector2.one/2,64,0,SpriteMeshType.FullRect);
            sprite.name=motif;Cache[motif]=sprite;return sprite;
        }
        public static Sprite Floor(string motif)=>motif=="Ouzana_FloorHome"?MaterialTile(0):motif=="Ouzana_FloorLab"?MaterialTile(1):null;
        public static Sprite Wall(bool home)=>MaterialTile(home?2:3);
        private static Sprite MaterialTile(int index)
        {
            string key="Material"+index;
            if(Cache.TryGetValue(key,out var sprite)&&sprite!=null)return sprite;
            var texture=Resources.Load<Texture2D>("Varginha/OuzanaArt/Architecture");
            if(texture==null)return null;
            int w=texture.width/2,h=texture.height/2;
            sprite=Sprite.Create(texture,new Rect(index%2*w,(1-index/2)*h,w,h),Vector2.one/2,w/1.5f,0,SpriteMeshType.FullRect);
            sprite.name="Ouzana_Material_"+index;Cache[key]=sprite;return sprite;
        }
    }
}
