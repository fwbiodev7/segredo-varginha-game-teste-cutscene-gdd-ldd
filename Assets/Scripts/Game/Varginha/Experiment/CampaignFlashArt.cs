using System;
using UnityEngine;

namespace Game.Varginha.Experiment
{
    public static class CampaignFlashArt
    {
        [Serializable] public sealed class Frame
        {
            public string name;
            public int x,y,width,height;
            public float pivotX,pivotY;
        }
        [Serializable] public sealed class Manifest
        {
            public int sourceWidth,sourceHeight;
            public float alphaCutoff,fallPPU,explosionPPU;
            public Frame[] fall,explosion;
        }
        private static Manifest _manifest;
        private static Sprite[] _fall,_explosion;
        private static Material _material;
        public static Manifest Atlas=>_manifest??=JsonUtility.FromJson<Manifest>(Resources.Load<TextAsset>("Varginha/Flash1996/Atlas").text);
        public static Sprite[] Fall=>_fall??=Load("ChildFall",Atlas.fall,Atlas.fallPPU);
        public static Sprite[] Explosion=>_explosion??=Load("AlienExplosion",Atlas.explosion,Atlas.explosionPPU);
        public static Material Material
        {
            get
            {
                if(_material!=null)return _material;
                _material=new Material(Resources.Load<Shader>("Varginha/Flash1996/CrispPixelSprite"));
                _material.name="Clarão — pixels sem halo";
                _material.SetFloat("_Cutoff",Atlas.alphaCutoff);
                return _material;
            }
        }
        private static Sprite[] Load(string name,Frame[] frames,float ppu)
        {
            var texture=Resources.Load<Texture2D>("Varginha/Flash1996/"+name);
            var result=new Sprite[frames.Length];
            for(int i=0;i<frames.Length;i++)
            {
                var f=frames[i];
                result[i]=Sprite.Create(texture,new Rect(f.x,f.y,f.width,f.height),new Vector2(f.pivotX,f.pivotY),ppu,0,SpriteMeshType.FullRect);
                result[i].name=name+"_"+f.name;
            }
            return result;
        }
    }
}
