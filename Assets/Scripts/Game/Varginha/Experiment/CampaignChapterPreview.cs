using UnityEngine;

namespace Game.Varginha.Experiment
{
    public static class CampaignChapterPreview
    {
        private static Texture2D _atlas;
        public static string Caption(int phase)=>phase switch
        {
            2=>"CADE A CHAVE?",3=>"A CIDADE AINDA ESTÁ ACORDADA",4=>"ENTRE AULAS E PISTAS",
            5=>"UMA FOTO. DUAS ANOTAÇÕES.",6=>"CADA FRAGMENTO CONTA UMA HISTÓRIA",
            7=>"ALÉM DA PONTE",8=>"A LUZ GUARDA MEMÓRIAS",9=>"O QUE A LUZ REVELA",10=>"DE VOLTA À OFICINA",_=>"VARGINHA"
        };
        public static void Background(int phase,float elapsed=0)
        {
            _atlas??=Resources.Load<Texture2D>("Varginha/Interface/ChapterPreviews");
            if(_atlas==null)return;
            int cell=Mathf.Clamp(phase-2,0,8),col=cell%3,row=cell/3;
            float panelAspect=_atlas.width/(float)_atlas.height,screenAspect=Screen.width/(float)Screen.height;
            float w=1f/3,h=1f/3;if(screenAspect>panelAspect)h*=panelAspect/screenAspect;else w*=screenAspect/panelAspect;
            float zoom=VarginhaGameSettings.Current.reducedMotion?1:1-Mathf.Clamp(elapsed,0,4)*.004f;w*=zoom;h*=zoom;
            var matrix=GUI.matrix;GUI.matrix=Matrix4x4.identity;
            GUI.DrawTextureWithTexCoords(new Rect(0,0,Screen.width,Screen.height),_atlas,new Rect((col+.5f)/3-w/2,(2-row+.5f)/3-h/2,w,h));
            ExperimentGUI.Box(new Rect(0,0,Screen.width,Screen.height),new Color(0,0,0,.22f));
            GUI.matrix=matrix;
        }
        public static void Draw(int phase,float elapsed,float total=3)
        {
            ExperimentGUI.Init();Background(phase,elapsed);
            var matrix=ExperimentGUI.BeginCanvas();
            ExperimentGUI.Panel(new Rect(155,510,970,130));
            ExperimentGUI.Label(new Rect(185,529,910,55),Caption(phase),true);
            ExperimentGUI.Label(new Rect(185,598,910,28),"FASE "+phase+" • VARGINHA, 2026",small:true);
            GUI.matrix=Matrix4x4.identity;
            float alpha=Mathf.Max(1-Mathf.Clamp01(elapsed/.3f),Mathf.Clamp01((elapsed-total+.35f)/.35f));
            ExperimentGUI.Box(new Rect(0,0,Screen.width,Screen.height),new Color(0,0,0,alpha));GUI.matrix=matrix;
        }
    }
}
