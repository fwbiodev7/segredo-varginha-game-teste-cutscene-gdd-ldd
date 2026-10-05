using Game.UI;
using UnityEngine;

namespace Game.Varginha.Experiment
{
    // Presented after the last epilogue dialogue, using the campaign's pixel font.
    public static class CampaignCredits
    {
        public static readonly string[] Roles={"devs", "arte e som", "narração", "narrativa", "testes e Q/A"};
        public static readonly string[] Names={
            "fabio, joao pedro matias, asafe e marcos",
            "yasmin, luis martins, giovana e gustavo",
            "luis miguel messias",
            "bianca, joao guilherme e karol",
            "anna sabia, tavares, moscardini e pedro"
        };
        public const string Participation="edelzio, renan, ouzana, professor fabio e ET de Varginha";

        public static bool Draw(float elapsed)
        {
            float scale=Mathf.Max(.01f,Mathf.Min(Screen.width/1280f,Screen.height/720f));
            float width=Screen.width/scale,height=Screen.height/scale;
            ExperimentGUI.Box(new Rect((1280-width)*.5f,(720-height)*.5f,width,height),Color.black);
            // A continuous film roll ends with the teaser resting at the centre of the screen.
            float y=650-Mathf.Min(Mathf.Max(0,elapsed-1)*45,1720);
            GUI.BeginGroup(new Rect(80,25,1120,600));
            PixelMenuTheme.Label(new Rect(0,y,1120,65),"O SEGREDO DE VARGINHA",36,ExperimentGUI.Paper,TextAnchor.MiddleCenter);
            PixelMenuTheme.Label(new Rect(0,y+85,1120,40),"CRÉDITOS",20,ExperimentGUI.Accent,TextAnchor.MiddleCenter);
            for(int i=0;i<Roles.Length;i++)
            {
                float row=y+180+i*150;
                PixelMenuTheme.Label(new Rect(0,row,1120,40),Roles[i]+":",22,ExperimentGUI.Accent,TextAnchor.MiddleCenter);
                PixelMenuTheme.Label(new Rect(0,row+48,1120,65),Names[i],25,ExperimentGUI.Paper,TextAnchor.MiddleCenter);
            }
            PixelMenuTheme.Label(new Rect(0,y+1030,1120,40),"com a participação de:",22,ExperimentGUI.Accent,TextAnchor.MiddleCenter);
            PixelMenuTheme.Label(new Rect(0,y+1080,1120,80),Participation,24,ExperimentGUI.Paper,TextAnchor.MiddleCenter);
            PixelMenuTheme.Label(new Rect(0,y+1300,1120,42),"ALGUNS SEGREDOS AINDA ESPERAM NO ESCURO.",24,ExperimentGUI.Muted,TextAnchor.MiddleCenter);
            PixelMenuTheme.Label(new Rect(0,y+1400,1120,65),"FIM...?",32,ExperimentGUI.Accent,TextAnchor.MiddleCenter);
            GUI.EndGroup();
            bool leave=ExperimentGUI.Button(new Rect(435,655,410,42),"SALVAR E VOLTAR AO MENU");
            return leave&&elapsed>.8f;
        }
    }
}
