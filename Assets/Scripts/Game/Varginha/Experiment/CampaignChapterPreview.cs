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
            7=>"ALÉM DA PONTE",8=>"A LUZ GUARDA MEMÓRIAS",9=>"O QUE A LUZ REVELA",10=>"DE VOLTA À OFICINA",
            11=>"AS CÓPIAS NÃO CONCORDAM",12=>"A CASA POR TRÁS DOS REGISTROS",13=>"1898",14=>"UMA PÁGINA AUSENTE",
            15=>"ANTES DO CLARÃO",16=>"SOB A CIDADE",18=>"DO OUTRO LADO DO MECANISMO",
            19=>"A LEMBRANÇA CONTINUA",20=>"MANTENHA A ESTABILIDADE",21=>"CUMPRA O ACORDO",_=>"VARGINHA"
        };
        public static void Background(int phase,float elapsed=0)
        {
            if(phase>=11)
            {
                var data=CampaignIllustratedMaps.Get(phase);var texture=data==null?null:Resources.Load<Texture2D>("Varginha/IllustratedMaps/"+data.image);
                if(texture!=null){var saved=GUI.matrix;GUI.matrix=Matrix4x4.identity;GUI.DrawTexture(new Rect(0,0,Screen.width,Screen.height),texture,ScaleMode.ScaleAndCrop);ExperimentGUI.Box(new Rect(0,0,Screen.width,Screen.height),new Color(0,0,0,.3f));GUI.matrix=saved;}return;
            }
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
            ExperimentGUI.Init();
            if(phase==2)
            {
                float opacity=1-Mathf.SmoothStep(0,1,Mathf.InverseLerp(total-.6f,total,elapsed));
                var color=GUI.color;GUI.color=new Color(1,1,1,opacity);
                var before=GUI.matrix;GUI.matrix=Matrix4x4.identity;
                ExperimentGUI.Box(new Rect(0,0,Screen.width,Screen.height),new Color(.025f,.035f,.045f,opacity));GUI.matrix=before;
                var canvas=ExperimentGUI.BeginCanvas();
                ExperimentGUI.Panel(new Rect(220,242,840,218));
                ExperimentGUI.Label(new Rect(260,268,760,30),"TRINTA ANOS DEPOIS",small:true);
                ExperimentGUI.Label(new Rect(260,318,760,55),"A LEMBRANÇA VOLTA",true);
                ExperimentGUI.Label(new Rect(260,401,760,30),"VARGINHA, 2026 • FASE 2",small:true);
                GUI.matrix=canvas;GUI.color=color;return;
            }
            Background(phase,elapsed);
            var matrix=ExperimentGUI.BeginCanvas();
            ExperimentGUI.Panel(new Rect(155,510,970,130));
            ExperimentGUI.Label(new Rect(185,529,910,55),Caption(phase),true);
            ExperimentGUI.Label(new Rect(185,598,910,28),CampaignSequence.Heading(phase)+" • VARGINHA, "+(phase==15||phase==19?"1996":"2026"),small:true);
            GUI.matrix=Matrix4x4.identity;
            float alpha=Mathf.Max(1-Mathf.Clamp01(elapsed/.3f),Mathf.Clamp01((elapsed-total+.35f)/.35f));
            ExperimentGUI.Box(new Rect(0,0,Screen.width,Screen.height),new Color(0,0,0,alpha));GUI.matrix=matrix;
        }
    }
}
