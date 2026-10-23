using System;
using UnityEngine;

namespace Game.Varginha.Experiment
{
    // A document assembled from movable strips; evidence is attached only after inspection.
    public static class CampaignBookPuzzle
    {
        public static readonly Rect CheckRect=new(820,526,285,36),HintRect=new(165,526,280,36);
        public static Rect PieceRect(int slot)=>new(490,264+slot*78,390,70);
        public static Rect LinkRect(int slot)=>new(885,264+slot*78,220,70);
        public static Rect EvidenceRect(int clue)=>new(165,268+clue*73,280,60);
        public static bool Assembled(CampaignExpansionState s)=>CampaignStory.Sequence(s.bookOrder,new[]{0,1,2});
        public static bool Swap(CampaignExpansionState s,int a,int b)
        {
            if(s.anchorFound||a<0||a>2||b<0||b>2)return false;
            (s.bookOrder[a],s.bookOrder[b])=(s.bookOrder[b],s.bookOrder[a]);
            // References belong to the physical strip, not the screen position.
            (s.bookLinks[a],s.bookLinks[b])=(s.bookLinks[b],s.bookLinks[a]);s.bookFeedback=0;return true;
        }
        public static bool Link(CampaignExpansionState s,int slot,int clue)
        {
            if(s.anchorFound||slot<0||slot>2||clue<0||clue>2||(s.anchorClues&(1<<clue))==0)return false;
            s.bookLinks[slot]=clue;s.bookFeedback=0;return true;
        }
        public static bool Ready(CampaignExpansionState s)
            =>s.anchorClues==7&&Assembled(s)&&CampaignStory.Sequence(s.bookLinks,new[]{0,1,2});
        public static bool Submit(CampaignExpansionState s)
        {
            if(s.anchorClues!=7){s.bookFeedback=1;return false;}
            if(!Assembled(s)){s.bookFeedback=2;return false;}
            if(!CampaignStory.Sequence(s.bookLinks,new[]{0,1,2})){s.bookFeedback=3;return false;}
            s.bookFeedback=0;return s.SolveAnchor(1996,1,23);
        }
        public static string Feedback(CampaignExpansionState s)=>s.bookFeedback switch
        {
            1=>"Ainda faltam evidências no arquivo. A montagem foi mantida.",
            2=>"A leitura está interrompida. Reorganize os fragmentos; nada foi apagado.",
            3=>"Uma referência não corresponde ao campo. Confira e troque só a associação necessária.",
            _=>s.anchorFound?"Registro reconstruído e conferido. Compartimento aberto.":"Troque dois fragmentos. Selecione uma evidência e depois o campo que ela explica."
        };
        public static string Hint(CampaignExpansionState s,int level)
            =>s.anchorClues!=7?"Examine o índice, a inscrição e a ficha no cenário. Só os registros encontrados podem ser associados."
            :level==0?"Procure a continuidade da leitura e as marcas da margem do documento."
            :level==1?"O título inicia a página; o carimbo encerra. Cada campo pede uma fonte diferente."
            :"Monte cabeçalho → identificação → carimbo. Associe índice ao ano, inscrição ao símbolo e ficha ao registro.";
        private static string Reference(int clue)=>clue==0?"ÍNDICE • 1996":clue==1?"INSCRIÇÃO • ÂNCORA":"FICHA • 23";
        private static string Fragment(int id)=>id==0?"DIOCESE • LIVRO DO TOMBO\nANO DO REGISTRO: ______"
            :id==1?"IDENTIFICAÇÃO: EDELZIO\nSÍMBOLO DO COMPARTIMENTO: ______":"CARIMBO DO ARQUIVO\nNÚMERO DO REGISTRO: ______";
        public static void Draw(CampaignExpansionState s,ref int selectedPiece,ref int selectedEvidence,Action confirm,Action save)
        {
            ExperimentGUI.Label(new Rect(165,166,950,40),"LIVRO DO TOMBO • RECONSTRUIR REGISTRO",true);
            ExperimentGUI.Label(new Rect(165,211,950,40),"Reorganize as partes do documento e relacione os campos às pistas encontradas.",small:true);
            bool enabled=GUI.enabled;GUI.enabled=enabled&&!s.anchorFound;
            for(int clue=0;clue<3;clue++)
            {
                bool acquired=(s.anchorClues&(1<<clue))!=0;GUI.enabled=enabled&&!s.anchorFound&&acquired;
                if(ExperimentGUI.Choice(EvidenceRect(clue),acquired?Reference(clue):"EVIDÊNCIA NÃO ENCONTRADA",selectedEvidence==clue))selectedEvidence=clue;
            }
            GUI.enabled=enabled&&!s.anchorFound;
            for(int slot=0;slot<3;slot++)
            {
                int piece=s.bookOrder[slot];var area=PieceRect(slot);
                ExperimentGUI.Box(new Rect(area.x-8,area.y,area.width+233,area.height),new Color(.77f,.70f,.53f));
                // The original three pieces carry different segments of one printed margin.
                for(int n=0;n<3;n++)ExperimentGUI.Box(new Rect(area.x-7,area.y+8+n*19,4+(piece==n?4:0),10),new Color(.27f,.24f,.18f));
                if(ExperimentGUI.Choice(area,Fragment(piece),selectedPiece==slot))
                {
                    if(selectedPiece<0)selectedPiece=slot;
                    else{Swap(s,selectedPiece,slot);selectedPiece=-1;save();}
                }
                string link=s.bookLinks[slot]<0?"ASSOCIAR PISTA":Reference(s.bookLinks[slot]);
                if(ExperimentGUI.Button(LinkRect(slot),link)&&selectedEvidence>=0)
                {if(Link(s,slot,selectedEvidence))save();selectedEvidence=-1;}
            }
            ExperimentGUI.Label(new Rect(165,491,950,30),Feedback(s),small:true);
            if(ExperimentGUI.Button(HintRect,"DICA "+(Mathf.Min(s.bookHints,2)+1))){s.bookHints=Mathf.Min(3,s.bookHints+1);save();}
            // A complete attempt can be checked even when its order or links are wrong.
            GUI.enabled=enabled&&!s.anchorFound&&s.anchorClues==7&&Array.TrueForAll(s.bookLinks,c=>c>=0);
            if(ExperimentGUI.Button(CheckRect,"CONFERIR DOCUMENTO"))confirm();
            GUI.enabled=enabled;
            if(s.bookHints>0)ExperimentGUI.Label(new Rect(165,570,940,44),Hint(s,s.bookHints-1),small:true);
        }
    }
}
