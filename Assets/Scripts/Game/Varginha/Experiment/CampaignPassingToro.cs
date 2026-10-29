using UnityEngine;

namespace Game.Varginha.Experiment
{
    // Only the musical traffic cameo uses this sprite. The player's Fusca stays original.
    public static class CampaignPassingToro
    {
        public const string Resource="Varginha/Revision20261029/FiatToroSide";
        public const float MapPixelWidth=218;
        static readonly float[] WheelCentresX={22.5f,99.5f};
        static Sprite _side;
        public static int WheelFrameAt(float time,bool reduced)=>reduced?0:Mathf.FloorToInt(Mathf.Max(0,time-CampaignSchoolMusicCar.StartTime)*18)%8;
        public static float BodyOffsetAt(float time,bool reduced)=>reduced?0:Mathf.Round(Mathf.Sin(Mathf.Max(0,time-CampaignSchoolMusicCar.StartTime)*16)*.75f);
        public static void Draw(Rect bounds,float time,bool reduced)
        {
            var shadow=new Color(.025f,.045f,.055f,.24f);
            ExperimentGUI.Box(new Rect(bounds.x+6,bounds.yMax-2,bounds.width-12,2),shadow);
            ExperimentGUI.Box(new Rect(bounds.x+15,bounds.yMax,bounds.width-30,1),shadow);
            bounds.y+=BodyOffsetAt(time,reduced);
            var sprite=Side;
            if(sprite==null)return;
            var uv=new Rect(sprite.rect.x/sprite.texture.width,sprite.rect.y/sprite.texture.height,sprite.rect.width/sprite.texture.width,sprite.rect.height/sprite.texture.height);
            GUI.DrawTextureWithTexCoords(bounds,sprite.texture,uv);
            float pixel=bounds.width/123;
            foreach(float sourceX in WheelCentresX)
            {
                var centre=new Vector2(bounds.x+sourceX*pixel,bounds.y+44*pixel);
                // Only the inner rim is redrawn. Authored tyres, arches and body remain intact.
                for(int y=-3;y<=3;y++)for(int x=-3;x<=3;x++)
                    if(x*x+y*y<=10)Plot(centre,x,y,pixel,new Color(.19f,.25f,.27f));
                float angle=WheelFrameAt(time,reduced)*Mathf.PI/4;
                for(int spoke=0;spoke<5;spoke++)
                {
                    float a=angle+spoke*Mathf.PI*2/5;
                    for(int radius=1;radius<=3;radius++)Plot(centre,Mathf.RoundToInt(Mathf.Cos(a)*radius),Mathf.RoundToInt(Mathf.Sin(a)*radius),pixel,new Color(.67f,.75f,.77f));
                }
                Plot(centre,0,0,pixel,new Color(.83f,.89f,.89f));
            }
        }
        static void Plot(Vector2 centre,int x,int y,float pixel,Color colour)
            =>ExperimentGUI.Box(new Rect(centre.x+(x-.5f)*pixel,centre.y+(y-.5f)*pixel,pixel,pixel),colour);
        public static Sprite Side
        {
            get
            {
                if(_side!=null)return _side;
                var texture=Resources.Load<Texture2D>(Resource);
                if(texture==null)return null;
                _side=Sprite.Create(texture,new Rect(2,11,123,52),new Vector2(.5f,0),123/4.9f,0,SpriteMeshType.FullRect);
                _side.name="Fiat_Toro_Passagem_Musical";return _side;
            }
        }
    }
}
