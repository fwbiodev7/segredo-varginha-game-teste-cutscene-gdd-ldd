using System;
using Game.UI;
using UnityEngine;
namespace Game.Varginha.Experiment
{
    public static class CampaignCredits
    {
        [Serializable] public class Team { public string role;public string[] names; }
        [Serializable] public class Data { public Team[] teams; }
        static Data data;static float clock,last=-1;static bool paused,fast;static Texture2D pixel;static CampaignCreditsMusic music;
        public static Data Content=>data??=JsonUtility.FromJson<Data>(Resources.Load<TextAsset>("Varginha/Credits/Teams").text);
        public static string[] Roles=>Array.ConvertAll(Content.teams,t=>t.role);
        public static string[] Names=>Array.ConvertAll(Content.teams,t=>string.Join(", ",t.names));
        public const string Participation="Edelzio, Ouzana, Professor Fábio, Renan";
        public static bool Draw(float elapsed)
        {
            ExperimentGUI.Init();if(pixel==null){pixel=new Texture2D(1,1);pixel.SetPixel(0,0,Color.white);pixel.Apply();}
            if(last<0||elapsed<last){clock=0;paused=fast=false;last=elapsed;}
            if(!paused)clock+=Mathf.Max(0,elapsed-last)*(fast?3:1);last=elapsed;
            if(music==null){var go=new GameObject("Trilha_original_creditos");music=go.AddComponent<CampaignCreditsMusic>();}
            music.Touch(paused);
            ExperimentGUI.Box(new Rect(-1280,-720,3840,2160),new Color(.008f,.015f,.018f));
            for(int i=0;i<22;i++){float x=(i*139)%1280,y=720-Mathf.Repeat(clock*(4+i%5)+i*67,760);ExperimentGUI.Box(new Rect(x,y,2+i%3,2+i%3),new Color(.2f,.65f,.42f,.18f));}
            if(clock<12)
            {
                string text=clock<4?"OIZLEDE: FRAGMENTO PERDIDO":clock<8?"UMA HISTÓRIA CRIADA POR ALUNOS":"OIZLEDE: FRAGMENTO PERDIDO";
                float a=Mathf.Clamp01(Mathf.Min(clock%4,(4-clock%4))/.8f);GUI.color=new Color(1,1,1,a);
                var logo=Resources.Load<Texture2D>("Varginha/OizledeLogo");
                if(clock<4&&logo!=null)GUI.DrawTexture(new Rect(340,190,600,300),logo,ScaleMode.ScaleToFit,true);
                else PixelMenuTheme.Label(new Rect(110,290,1060,100),text,clock<4?32:23,ExperimentGUI.Paper,TextAnchor.MiddleCenter);GUI.color=Color.white;
            }
            else if(clock<120)
            {
                float y=600-(clock-12)*25;GUI.BeginGroup(new Rect(120,35,1040,585));
                foreach(var team in Content.teams)
                {
                    if(team.role=="EM HOMENAGEM A")y+=100;
                    PixelMenuTheme.Label(new Rect(0,y,1040,42),team.role,22,ExperimentGUI.Accent,TextAnchor.MiddleCenter);y+=58;
                    foreach(string name in team.names){PixelMenuTheme.Label(new Rect(0,y,1040,32),name,18,ExperimentGUI.Paper,TextAnchor.MiddleCenter);y+=37;}
                    y+=85;
                }
                GUI.EndGroup();
                for(int i=0;i<10;i++){float a=(10-i)/10f;ExperimentGUI.Box(new Rect(110,35+i*3,1060,3),new Color(.008f,.015f,.018f,a));ExperimentGUI.Box(new Rect(110,617-i*3,1060,3),new Color(.008f,.015f,.018f,a));}
            }
            else
            {
                float t=clock-120,a=Mathf.Clamp01(t/1.5f);GUI.color=new Color(1,1,1,a);
                PixelMenuTheme.Label(new Rect(120,175,1040,70),"OIZLEDE: FRAGMENTO PERDIDO",30,ExperimentGUI.Paper,TextAnchor.MiddleCenter);
                string message=t<9?"Uma homenagem ao professor Edelzio, que inspirou esta jornada.":t<14?"FEITO COM CRIATIVIDADE, APRENDIZADO E MUITA DEDICAÇÃO.":"OBRIGADO POR JOGAR!";
                PixelMenuTheme.Label(new Rect(120,320,1040,100),message,20,ExperimentGUI.Accent,TextAnchor.MiddleCenter);GUI.color=Color.white;
            }
            if(ExperimentGUI.Button(new Rect(190,655,260,42),paused?"CONTINUAR":"PAUSAR"))paused=!paused;
            if(ExperimentGUI.Button(new Rect(470,655,260,42),fast?"VELOCIDADE NORMAL":"ACELERAR"))fast=!fast;
            return ExperimentGUI.Button(new Rect(750,655,340,42),clock>135?"VOLTAR AO MENU":"PULAR CRÉDITOS");
        }
    }
    public sealed class CampaignCreditsMusic:MonoBehaviour
    {
        AudioSource source;int frame;bool paused;
        void Awake(){source=gameObject.AddComponent<AudioSource>();source.clip=Resources.Load<AudioClip>("Varginha/Credits/MusicTheme");source.loop=true;source.playOnAwake=false;source.volume=0;if(source.clip!=null)source.Play();}
        public void Touch(bool value){frame=Time.frameCount;if(paused!=value){if(value)source.Pause();else source.UnPause();paused=value;}}
        void Update(){if(Time.frameCount-frame>4){source.volume=Mathf.MoveTowards(source.volume,0,Time.unscaledDeltaTime*.2f);if(source.volume==0)Destroy(gameObject);return;}source.volume=Mathf.MoveTowards(source.volume,.22f*VarginhaGameSettings.Current.music*(CampaignNarration.Speaking?.35f:1),Time.unscaledDeltaTime*.1f);}
    }
}
