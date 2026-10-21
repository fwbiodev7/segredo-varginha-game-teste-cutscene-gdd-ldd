using UnityEngine;

namespace Game.Varginha.Experiment
{
    // Campaign-only direction. The historical experiment/lab keeps its own storyboard.
    public static class CampaignOpeningCinema
    {
        public static readonly ExperimentShot[] Shots={
            new(){id="paper_1996",asset="OpeningStoryboard",panel=0,mode="paper_1996",duration=4,title="VARGINHA • 1996",subtitle="Notícias de um avistamento. Os relatos ainda se contradizem.",voice=""},
            new(){id="news_1996",asset="OpeningStoryboard",panel=0,mode="news",duration=5,title="O NOTICIÁRIO",subtitle="A televisão fala da suposta criatura vista em Varginha.",voice=""},
            new(){id="tv_reveal",asset="OpeningStoryboard",panel=0,mode="news",duration=4,title="UMA NOITE COMUM",subtitle="Em casa, Edelzio acompanha a reportagem.",voice=""},
            new(){id="child_1996",asset="OpeningStoryboard",panel=0,mode="news",duration=3,title="EDELZIO • SEIS ANOS",subtitle="A casa está tranquila. Você pode explorar e observar os objetos.",voice=""}
        };
        public static readonly Rect TvScreen=new(974,169,147,130);
        public static float Reveal(float time,bool reduced)=>reduced?1:Mathf.SmoothStep(0,1,Mathf.Clamp01(time/3.5f));
        public static int ChildFrame(float time,bool reduced)=>reduced?0:Mathf.FloorToInt(Mathf.Max(0,time)*8)%8;
        public static void Draw(ExperimentShot shot,float time,bool reduced,Texture news)
        {
            if(shot.id=="paper_1996")
            {
                var paper=CampaignCinemaTextures.Resolve("Newspaper1996");
                float drift=reduced?0:Mathf.SmoothStep(0,1,Mathf.Clamp01(time/4))*12;
                if(paper!=null)GUI.DrawTexture(new Rect(-drift,-drift/2,1280+drift*2,720+drift),paper);
                var old=GUI.color;GUI.color=new Color(.19f,.15f,.1f);
                ExperimentGUI.Label(new Rect(152,98,970,40),"RELATOS EM VARGINHA",true);
                ExperimentGUI.Label(new Rect(153,146,900,30),"1996 • Notícias de um avistamento",small:true);GUI.color=old;
            }
            else if(shot.id=="news_1996")
            {
                if(news!=null)GUI.DrawTexture(new Rect(0,0,1280,720),news);
            }
            else
            {
                var room=CampaignCinemaTextures.Resolve("ChildRoom1996");
                if(room!=null)GUI.DrawTexture(new Rect(0,0,1280,720),room);
                var child=CampaignCinemaTextures.Resolve("ChildWatchingTV");
                int frame=ChildFrame(time,reduced);
                if(child!=null)GUI.DrawTextureWithTexCoords(new Rect(630,260,280,280),child,new Rect(frame%4/4f,1-(frame/4+1)/2f,.25f,.5f));
                float reveal=shot.id=="tv_reveal"?Reveal(time,reduced):1;
                var target=new Rect(Mathf.Lerp(0,TvScreen.x,reveal),Mathf.Lerp(0,TvScreen.y,reveal),
                    Mathf.Lerp(1280,TvScreen.width,reveal),Mathf.Lerp(720,TvScreen.height,reveal));
                if(news!=null)GUI.DrawTexture(target,news,ScaleMode.ScaleAndCrop);
                // Local CRT scan lines move without strobing the whole room.
                if(reveal>=.99f)for(int row=0;row<16;row++)ExperimentGUI.Box(new Rect(TvScreen.x,TvScreen.y+row*8,TvScreen.width,1),new Color(.01f,.03f,.06f,.2f));
            }
            ExperimentGUI.Box(new Rect(0,0,1280,86),Color.black);
            ExperimentGUI.Box(new Rect(0,550,1280,170),Color.black);
        }
    }
}
