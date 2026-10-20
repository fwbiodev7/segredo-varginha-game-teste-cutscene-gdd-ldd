using UnityEngine;

namespace Game.Varginha.Experiment
{
    public static class CampaignHouseArt
    {
        private static readonly Sprite[] Frames=new Sprite[8];
        public static Texture2D Atlas=>Resources.Load<Texture2D>("Varginha/KeyInvestigation/HouseEvidence");
        public static Rect UV(int frame)=>new(frame%4*.25f,frame<4?.5f:0,.25f,.5f);
        public static Sprite Frame(int frame)
        {
            if(Frames[frame]!=null)return Frames[frame];var t=Atlas;if(t==null)return null;
            Frames[frame]=Sprite.Create(t,new Rect(frame%4*128,(frame<4?128:0),128,128),Vector2.one*.5f,128,0,SpriteMeshType.FullRect);
            Frames[frame].name="HouseEvidence_"+frame;return Frames[frame];
        }
        public static void Draw(Rect rect,int frame){var t=Atlas;if(t!=null)GUI.DrawTextureWithTexCoords(rect,t,UV(frame));}
    }
}
