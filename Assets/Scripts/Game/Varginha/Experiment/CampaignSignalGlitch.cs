using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Varginha.Experiment
{
    // Horizontal digital tearing, shared by the CRT compositor and the layered car shots.
    // Local deterministic hashes never change gameplay's UnityEngine.Random state.
    public static class CampaignSignalGlitch
    {
        public readonly struct Band
        {
            public readonly RectInt rect;
            public readonly Color32 color;
            public readonly int displacement;
            public Band(RectInt rect,Color32 color,int displacement=0)
            {this.rect=rect;this.color=color;this.displacement=displacement;}
        }
        public static readonly Color32[] Palette={new(38,242,241,255),new(235,39,195,255),new(248,247,73,255),
            new(35,91,245,255),new(246,62,43,255),new(39,239,99,255),new(239,245,233,255)};
        private static readonly List<Band> Scratch=new(48);
        private static Color32[] _source;
        private static uint Hash(uint v){v^=v>>16;v*=0x7feb352d;v^=v>>15;v*=0x846ca68b;return v^(v>>16);}
        public static float BurstStrength(float time)
        {
            float phase=Mathf.Max(0,time)%2.4f;
            uint cycle=(uint)Mathf.FloorToInt(Mathf.Max(0,time)/2.4f);uint h=Hash(cycle+71);
            float duration=.73f+(h%16)*.01f,echo=.98f+((h>>8)%12)*.01f;
            return phase<duration?1-phase*.45f:phase>=echo&&phase<echo+.09f?.45f:0;
        }

        public static void Sample(float time,int width,int height,bool reduced,List<Band> bands)
        {
            bands.Clear();if(width<8||height<8)return;
            if(reduced)
            {
                // A fixed damaged-signal signature replaces moving/flickering fragments.
                for(int i=0;i<3;i++){var c=Palette[i];c.a=48;bands.Add(new Band(new RectInt(width/5,height*(i+1)/4,width/2,2),c));}
                return;
            }
            time=Mathf.Max(0,time);
            if(BurstStrength(time)<=0)return; // stronger primary burst and one short echo, then a readable hold
            uint cycle=(uint)Mathf.FloorToInt(time/2.4f);
            int groups=3+(int)(Hash(cycle+91)%3);
            for(int group=0;group<groups;group++)
            {
                // Independent clocks and shuffled 2-D cells prevent rows of
                // fragments changing in unison or gathering across the face.
                uint tick=(uint)Mathf.FloorToInt(time*(7+group*2));
                uint seed=Hash(cycle*479u+(uint)group*97u+tick*19u);
                int cell=(int)((cycle*3+(uint)(group*3)+tick/3)%8);
                int centreX=width*(cell%4)/4+width*(20+(int)(seed%60))/400;
                int centreY=height*(cell/4)/2+height*(18+(int)((seed>>8)%64))/200;
                int count=8+(int)((seed>>16)%4);
                for(int i=0;i<count;i++)
                {
                    uint h=Hash(seed+(uint)i*731u);
                    int length=Mathf.Clamp(width*(5+(int)(h%23))/100,7,width-2);
                    if(i==0&&group==0)length=Mathf.Max(length,width*38/100);
                    int thickness=1+(int)((h>>20)%3);
                    int x=Mathf.Clamp(centreX-length/2+(int)((h>>8)%(uint)Mathf.Max(1,width/5))-width/10,0,width-length);
                    int y=Mathf.Clamp(centreY+(int)((h>>14)%(uint)Mathf.Max(1,height/5))-height/10,0,height-thickness);
                    var colour=Palette[(int)((h>>24)%(uint)Palette.Length)];
                    if(i%3!=0)colour.a=(byte)(135+(h%100));
                    bands.Add(new Band(new RectInt(x,y,length,thickness),colour,i%4==0?(int)(h%49)-24:0));
                }
            }
        }
        public static void DrawEcho(Texture scene,Rect area,float time,bool reduced)
        {
            if(reduced)return;
            float strength=BurstStrength(time);if(strength<=0)return;
            var saved=GUI.color;
            try
            {
                if(scene!=null)
                {
                    float offset=Mathf.Round(7+strength*9);
                    GUI.color=new Color(.22f,1,.55f,.20f*strength);
                    GUI.DrawTexture(new Rect(area.x+offset,area.y,area.width,area.height),scene);
                    GUI.color=new Color(.7f,.17f,1,.12f*strength);
                    GUI.DrawTexture(new Rect(area.x-offset,area.y+3,area.width,area.height),scene);
                }
            }
            finally{GUI.color=saved;}
            // An uncanny green field and drifting pixel filaments suggest an
            // external presence without inventing symbols, a face or a reveal.
            uint cycle=(uint)Mathf.FloorToInt(Mathf.Max(0,time)/2.4f);
            for(int i=0;i<43;i++)
            {
                uint h=Hash((uint)i*731u+cycle*97u);
                float x=area.x+area.width*(h%1000)/1000f+Mathf.Sin(time*(.8f+(h%7)*.1f)+i)*13;
                float y=86+((h>>10)%440+time*(7+h%11))%450;
                var colour=i%4==0?new Color(.65f,.3f,.95f,.16f*strength):new Color(.26f,.95f,.52f,.26f*strength);
                ExperimentGUI.Box(new Rect(Mathf.Round(x),Mathf.Round(y),2+h%4,3+(h>>20)%9),colour);
            }
            float pulse=(.65f+.35f*Mathf.Sin(time*3))*strength;
            for(int cloud=0;cloud<3;cloud++)
            {
                uint h=Hash(cycle*97u+(uint)cloud*731u);
                float x=area.x+area.width*(.12f+(h%70)*.01f),y=130+(h>>12)%260;
                for(int i=0;i<5;i++)ExperimentGUI.Box(new Rect(x+i*8,y+i*6,170-i*16,104-i*12),new Color(.18f,.92f,.42f,.028f*pulse));
            }
        }
        public static void DrawCabin(Rect viewport,Texture scene,float time,bool reduced)
        {
            const int w=384,h=216;Sample(time,w,h,reduced,Scratch);
            GUI.BeginGroup(viewport);
            try
            {
                foreach(var b in Scratch)
                {
                    var r=new Rect(Mathf.Round(b.rect.x*viewport.width/w),Mathf.Round(b.rect.y*viewport.height/h),
                        Mathf.Max(1,Mathf.Round(b.rect.width*viewport.width/w)),Mathf.Max(1,Mathf.Round(b.rect.height*viewport.height/h)));
                    if(scene!=null&&b.displacement!=0&&!reduced)
                    {
                        var uv=new Rect((viewport.x+r.x)/1280,1-(viewport.y+r.y+r.height)/720,r.width/1280,r.height/720);
                        r.x+=Mathf.Round(b.displacement*viewport.width/w);GUI.DrawTextureWithTexCoords(r,scene,uv);
                    }
                    else ExperimentGUI.Box(r,b.color);
                }
            }
            finally{GUI.EndGroup();}
        }
        public static void Draw(Rect area,float time,bool reduced)
        {
            const int w=384,h=108;Sample(time,w,h,reduced,Scratch);
            foreach(var b in Scratch)
                ExperimentGUI.Box(new Rect(Mathf.Round(area.x+b.rect.x*area.width/w),Mathf.Round(area.y+b.rect.y*area.height/h),
                    Mathf.Max(1,Mathf.Round(b.rect.width*area.width/w)),Mathf.Max(1,Mathf.Round(b.rect.height*area.height/h))),b.color);
        }
        public static void Apply(Color32[] pixels,int width,int height,float time,bool reduced)
        {
            Sample(time,width,height,reduced,Scratch);if(Scratch.Count==0)return;
            if(_source==null||_source.Length!=pixels.Length)_source=new Color32[pixels.Length];
            Array.Copy(pixels,_source,pixels.Length);
            foreach(var b in Scratch)for(int y=b.rect.y;y<b.rect.yMax;y++)for(int x=b.rect.x;x<b.rect.xMax;x++)
            {
                int index=y*width+x;
                if(b.displacement!=0)pixels[index]=_source[y*width+Mathf.Clamp(x+b.displacement,0,width-1)];
                else if(b.color.a==255)pixels[index]=b.color;
                else pixels[index]=(Color32)Color.Lerp(pixels[index],b.color,b.color.a/255f);
            }
        }
    }
}
