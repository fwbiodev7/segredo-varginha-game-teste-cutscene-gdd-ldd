using System;
using UnityEngine;

namespace Game.Varginha.Experiment
{
    // A notebook map: fixed landmarks, visible terrain, three route decisions.
    // Old permutation/compass fields remain serialized only for compatibility.
    public static class CampaignTrailPuzzle
    {
        public static int CorrectBranch(int step)=>step==1?0:1;
        public static int Step(CampaignExpansionState s){int i=Array.IndexOf(s.trailChoices,-1);return i<0?3:i;}
        public static bool Commit(CampaignExpansionState s,int choice)
        {
            int step=Step(s);if(s.mapSolved||step>=3||choice<0||choice>1)return false;
            s.trailChoices[step]=choice;s.trailFeedback=0;return true;
        }
        public static void Undo(CampaignExpansionState s)
        {if(s.mapSolved)return;int step=Step(s);if(step>0)s.trailChoices[step-1]=-1;s.trailFeedback=0;}
        public static void Reset(CampaignExpansionState s)
        {if(s.mapSolved)return;Array.Fill(s.trailChoices,-1);s.trailFeedback=0;}
        public static bool Validate(CampaignExpansionState s)
        {
            if(s.visited!=7){s.trailFeedback=4;return false;}
            for(int i=0;i<3;i++)
            {
                if(s.trailChoices[i]<0){s.trailFeedback=5;return false;}
                if(s.trailChoices[i]!=CorrectBranch(i))
                {s.trailFeedback=i+1;for(int j=i;j<3;j++)s.trailChoices[j]=-1;return false;}
            }
            s.trailFeedback=0;return true;
        }
        public static string Feedback(CampaignExpansionState s)=>s.trailFeedback switch
        {
            1=>"Um tronco bloqueia o primeiro trecho. Os trechos anteriores foram mantidos.",
            2=>"Esse trecho entra no rio fora da ponte. Volte à margem e procure a travessia.",
            3=>"O muro bloqueia esse acesso. A fotografia mostra a entrada junto ao portão.",
            4=>"Faltam registros: examine o livro, o globo e o painel. Seu traçado foi mantido.",
            5=>"O caminho está incompleto. Escolha e trace os trechos que faltam.",
            _=>Step(s)<3?"Selecione uma trilha para visualizar; depois use TRAÇAR TRECHO.":"Compare o caminho destacado com as três pistas e confira a rota."
        };
        public static string Hint(CampaignExpansionState s,int level)
        {
            if(s.visited!=7)return level==0?"As pistas ficam nos três objetos da biblioteca.":level==1?"Examine livro, globo e painel; os registros aparecem ao lado do mapa.":"Volte à exploração e examine os três objetos antes de confirmar a rota.";
            int step=Mathf.Min(Step(s),2);
            if(level==0)return "Observe o terreno de cada bifurcação. Uma linha desenhada nem sempre é uma passagem livre.";
            if(level==1)return step==0?"Compare o tronco caído com a passagem livre perto da árvore.":step==1?"Observe onde cada trilha encontra a água. Procure a ponte.":"Procure o portão; a linha que atravessa o muro não é um acesso.";
            return step==0?"No primeiro trecho, use a trilha B para contornar o tronco.":step==1?"No rio, a trilha A passa pela ponte.":"No último trecho, a trilha B chega ao portão da capela.";
        }
        public static Rect ChoiceRect(int step,int branch)=>new(505+step*205,branch==0?282:431,128,33);
        public static readonly Rect TraceRect=new(465,518,205,32),UndoRect=new(680,518,180,32),CheckRect=new(870,518,235,32);
        private static readonly Color Path=new(.67f,.49f,.28f),Chosen=new(.43f,.93f,.81f),Preview=new(1f,.76f,.3f);
        private static Vector2 Root(int i)=>new(465+i*205,373);
        private static Vector2 Bend(int i,int branch)=>new(567+i*205,branch==0?325:415);
        private static void Line(Vector2 a,Vector2 b,Color colour,int size,bool terrain)
        {
            int n=Mathf.CeilToInt(Vector2.Distance(a,b)/2);
            for(int i=0;i<=n;i++)
            {
                var p=Vector2.Lerp(a,b,n>0?(float)i/n:0);
                // Dirt ends at the banks; a dotted planning trace is never an extra bridge.
                if(terrain&&CampaignTrailMapArt.InRiver(p.x)||!terrain&&i%5>=3)continue;
                ExperimentGUI.Box(new Rect(Mathf.Round(p.x-size/2f),Mathf.Round(p.y-size/2f),size,size),colour);
            }
        }
        private static void Road(int step,int branch,Color colour,int size,bool terrain)
        {var bend=Bend(step,branch);Line(Root(step),bend,colour,size,terrain);Line(bend,Root(step+1),colour,size,terrain);}
        public static void Draw(CampaignExpansionState s,ref int preview,Action confirm,Action save)
        {
            ExperimentGUI.Label(new Rect(165,165,950,42),"FRAGMENTOS • UM CAMINHO ATÉ A CAPELA",true);
            ExperimentGUI.Label(new Rect(165,213,950,44),"Parta da árvore. Veja a prévia e trace cada trecho até o portão.",small:true);
            string[] names={"LIVRO • PASSAGEM","GLOBO • TRAVESSIA","PAINEL • DESTINO"};
            string[] clues={"Parta da árvore.\nContorne o tronco.","Cruze o rio\npela ponte.","Entre pelo portão.\nO muro bloqueia."};
            for(int i=0;i<3;i++)
            {
                ExperimentGUI.Panel(new Rect(165,265+i*70,265,64));
                ExperimentGUI.Label(new Rect(176,271+i*70,243,19),names[i],small:true);
                ExperimentGUI.Label(new Rect(176,291+i*70,243,32),(s.visited&(1<<i))!=0?clues[i]:"Examine esse objeto para registrar a pista.",small:true);
            }
            CampaignTrailMapArt.DrawTerrain();
            int current=Step(s);bool enabled=GUI.enabled;
            for(int i=0;i<3;i++)for(int branch=0;branch<2;branch++)
            {
                Road(i,branch,new Color(.24f,.22f,.14f),11,true);
                Road(i,branch,Path,7,true);
                if(s.trailChoices[i]==branch||i==current&&preview==branch)
                    Road(i,branch,s.trailChoices[i]==branch?Chosen:Preview,3,false);
            }
            CampaignTrailMapArt.DrawLandmarks();
            for(int i=0;i<3;i++)for(int branch=0;branch<2;branch++)
            {
                GUI.enabled=enabled&&i==current;
                if(ExperimentGUI.Choice(ChoiceRect(i,branch),"TRILHA "+(branch==0?"A":"B"),i==current&&preview==branch))preview=branch;
            }
            GUI.enabled=enabled;
            ExperimentGUI.Label(new Rect(448,264,160,18),"ÁRVORE • INÍCIO",small:true);
            ExperimentGUI.Label(new Rect(995,264,112,18),"CAPELA",small:true);
            ExperimentGUI.Label(new Rect(165,481,940,28),Feedback(s),small:true);
            GUI.enabled=enabled&&current<3&&preview>=0;
            if(ExperimentGUI.Button(TraceRect,"TRAÇAR TRECHO")){Commit(s,preview);preview=-1;save();}
            GUI.enabled=enabled&&current>0;
            if(ExperimentGUI.Button(UndoRect,"DESFAZER")){Undo(s);preview=-1;save();}
            GUI.enabled=enabled;
            if(ExperimentGUI.Button(CheckRect,"CONFERIR ROTA")){preview=-1;confirm();}
            if(ExperimentGUI.Button(new Rect(165,518,135,32),"RECOMEÇAR")){Reset(s);preview=-1;save();}
            if(ExperimentGUI.Button(new Rect(310,518,120,32),"DICA "+(Mathf.Min(s.trailHints,2)+1))){s.trailHints=Mathf.Min(3,s.trailHints+1);save();}
            if(s.trailHints>0)ExperimentGUI.Label(new Rect(480,561,625,48),Hint(s,s.trailHints-1),small:true);
        }
    }
}
