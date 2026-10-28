using UnityEngine;

namespace Game.Varginha.Experiment
{
    public sealed partial class VarginhaCampaignStage
    {
        View _documentReturn;
        int _schoolSelectedNote=-1;
        bool _schoolHints;
        // The shared menu theme fits labels on one line. Documents need wrapping,
        // while retaining the exact same registered mouse/keyboard/controller buttons.
        static bool SchoolCard(Rect rect,string text,bool selected=false)
        {
            bool clicked=Game.UI.PixelMenuTheme.Button(rect,"",marker:"");
            if(selected)ExperimentGUI.Box(new Rect(rect.x,rect.y,3,rect.height),ExperimentGUI.Accent);
            if(rect.width<120)Game.UI.PixelMenuTheme.Label(new Rect(rect.x+6,rect.y+2,rect.width-12,rect.height-4),text,10,ExperimentGUI.Paper);
            else ExperimentGUI.Label(new Rect(rect.x+12,rect.y+8,rect.width-24,rect.height-16),text);
            return clicked;
        }
        public bool SchoolInvestigationOpen=>_view==View.School;
        public bool SchoolDocumentOpen=>_view==View.Document;
        public bool OpenSchoolInvestigation()
        {
            if(!_ready||phase<4||_busy||_paused)return false;
            if(!_progress.school.documentsRecovered){Say("Edelzio","Preciso dos documentos que guardei na caixa em casa.");return false;}
            if(_progress.school.EnsureLinkLayout())Save();
            _dialogue=null;_textMessage.Close();_schoolHints=false;Show(View.School);return true;
        }
        public bool OpenSchoolDocument(int id)=>OpenSchoolDocument(id,View.School);
        bool OpenSchoolDocument(int id,View returnTo)
        {
            var school=_progress.school;
            if(returnTo==View.School&&!school.completed&&(school.stage==1||school.stage==3&&id!=6))return false;
            if(!_ready||_busy||_paused||!_progress.school.Examine(id))return false;
            _documentReturn=returnTo;Show(View.Document);Save();return true;
        }
        public void CloseSchoolDocument(){if(_view==View.Document){Show(_documentReturn);Save();}}
        public void CloseSchoolInvestigation(){if(_view==View.School){Save();Show(View.World);}}
        public bool SwapSchoolDocuments(int a,int b){bool ok=_progress.school.Swap(a,b);if(ok)Save();return ok;}
        public bool HighlightSchoolPassage(int passage){bool ok=_progress.school.ToggleHighlight(passage);if(ok)Save();return ok;}
        public bool ChooseSchoolComparison(int value){bool ok=_progress.school.ChooseComparison(value);if(ok)Save();return ok;}
        public bool LinkSchoolNote(int note,int passage){bool ok=_progress.school.Link(note,passage);if(ok)Save();return ok;}
        public bool ChooseSchoolDestination(int destination){bool ok=_progress.school.ChooseDestination(destination);if(ok)Save();return ok;}
        public bool SubmitSchoolStep()
        {
            if(!_ready||phase<4||_paused||_busy||_view!=View.School)return false;
            bool ok=_progress.school.Submit(out string result);_feedback=result;
            _selected=_schoolSelectedNote=-1;_progress.SyncSchoolCompletion();Save();
            if(ok)_sound?.Play("Success");return ok;
        }
        public bool PrepareLibraryDeparture()
        {
            if(!_progress.school.completed||_view!=View.School||_paused)return false;
            Show(View.Complete);return true;
        }
        void DrawSchoolInvestigation()
        {
            var s=_progress.school;
            ExperimentGUI.Box(new Rect(0,0,1280,720),new Color(0,0,0,.85f));
            ExperimentGUI.Panel(new Rect(40,45,1200,630));
            ExperimentGUI.Label(new Rect(70,65,1100,45),"PADRÕES OCULTOS • MEUS DOCUMENTOS",true);
            string[] steps={"1 ORDENAR","2 COMPARAR","3 CRUZAR","4 CONCLUIR"};
            for(int i=0;i<4;i++)
            {
                ExperimentGUI.Ribbon(new Rect(75+i*285,120,275,34));
                ExperimentGUI.Label(new Rect(84+i*285,128,255,23),(s.stage>i?"✓ ":s.stage==i?"> ":"")+steps[i],small:true);
            }
            bool enabled=GUI.enabled;GUI.enabled=enabled&&!_schoolHints;
            if(s.completed)DrawSchoolConclusion();
            else if(s.stage==0)DrawSchoolOrder();
            else if(s.stage==1)DrawSchoolComparison();
            else if(s.stage==2)DrawSchoolLinks();
            else DrawSchoolDestination();
            if(!string.IsNullOrEmpty(_feedback))ExperimentGUI.Label(new Rect(75,548,1125,40),_feedback,small:true);
            if(ExperimentGUI.Button(new Rect(75,610,200,42),"GUARDAR E VOLTAR"))CloseSchoolInvestigation();
            if(ExperimentGUI.Button(new Rect(290,610,195,42),"CONSULTAR DICA"))_schoolHints=true;
            if(!s.completed&&ExperimentGUI.Button(new Rect(910,610,290,42),"CONFERIR ETAPA"))SubmitSchoolStep();
            GUI.enabled=enabled;
            if(_schoolHints)
            {
                ExperimentGUI.Box(new Rect(40,160,1200,430),new Color(0,0,0,.94f));
                ExperimentGUI.Label(new Rect(155,220,970,45),s.hintLevel==2?"SOLUÇÃO • SPOILER":"DICA "+(s.hintLevel+1)+" DE 3",true);
                ExperimentGUI.Label(new Rect(155,290,970,140),s.Hint(s.hintLevel));
                if(ExperimentGUI.Button(new Rect(155,475,360,45),"VOLTAR À ANÁLISE"))_schoolHints=false;
                if(s.hintLevel<2&&ExperimentGUI.Button(new Rect(650,475,475,45),s.hintLevel==1?"REVELAR SOLUÇÃO • SPOILER":"MAIS UMA DICA")){s.hintLevel++;Save();}
            }
        }
        void DrawSchoolOrder()
        {
            var s=_progress.school;
            ExperimentGUI.Label(new Rect(75,170,1125,44),"Guardei estes recortes na infância. Se eu ordenar as datas, talvez encontre uma relação.");
            ExperimentGUI.Label(new Rect(75,216,1125,24),"Leia cada recorte e selecione dois cartões para trocá-los: do mais antigo à esquerda ao mais recente à direita.",small:true);
            for(int i=0;i<3;i++)
            {
                int id=s.order[i];float x=75+i*380;
                ExperimentGUI.Panel(new Rect(x,245,365,270));
                ExperimentGUI.Label(new Rect(x+18,263,329,72),CampaignSchoolDocuments.Titles[id]);
                ExperimentGUI.Label(new Rect(x+18,345,329,62),"Origem: coleção C-96\n"+((s.examined&(1<<id))!=0?"Recorte examinado":"Abra para ler o texto completo"),small:true);
                if(ExperimentGUI.Button(new Rect(x+18,416,329,36),"LER RECORTE"))OpenSchoolDocument(id);
                if(ExperimentGUI.Choice(new Rect(x+18,465,329,36),(_selected==i?"SELECIONADO":"POSIÇÃO "+(i+1))+" • TROCAR",_selected==i))
                {if(_selected<0)_selected=i;else{SwapSchoolDocuments(_selected,i);_selected=-1;}}
            }
        }
        void DrawSchoolComparison()
        {
            var s=_progress.school;
            ExperimentGUI.Label(new Rect(75,174,1125,60),"Qual destes acontecimentos não se repetiu nos registros?");
            ExperimentGUI.Label(new Rect(75,236,1125,28),"Escolha uma alternativa com base nos recortes que você leu.",small:true);
            for(int i=0;i<3;i++)
            {
                if(SchoolCard(new Rect(155,285+i*76,970,62),CampaignSchoolDocuments.ComparisonChoices[i],s.comparisonChoice==i))ChooseSchoolComparison(i);
            }
        }
        void DrawSchoolLinks()
        {
            var s=_progress.school;
            ExperimentGUI.Label(new Rect(75,170,1125,50),"Leia as anotações; selecione uma lembrança e depois o trecho que a confirma. Cada ligação pode ser corrigida.");
            for(int i=0;i<3;i++)
            {
                int note=s.noteOrder[i],passage=s.passageOrder[i];
                float y=237+i*97;
                if(SchoolCard(new Rect(75,y,390,80),CampaignSchoolDocuments.Notes[note],_schoolSelectedNote==note))_schoolSelectedNote=note;
                if(SchoolCard(new Rect(480,y,95,34),"LER"))OpenSchoolDocument(note+3);
                if(SchoolCard(new Rect(480,y+42,95,34),"DESFAZER"))LinkSchoolNote(note,-1);
                if(SchoolCard(new Rect(755,y,445,80),CampaignSchoolDocuments.Titles[passage]+"\n"+CampaignSchoolDocuments.Passages[passage][2],_schoolSelectedNote>=0&&s.links[_schoolSelectedNote]==passage))
                {if(_schoolSelectedNote>=0){LinkSchoolNote(_schoolSelectedNote,passage);_schoolSelectedNote=-1;}else _feedback="Selecione primeiro uma anotação à esquerda.";}
                if(s.links[note]>=0)
                {
                    int targetRow=System.Array.IndexOf(s.passageOrder,s.links[note]);
                    float to=277+targetRow*97,from=y+40,lane=625+i*25;
                    Color color=i==0?ExperimentGUI.Accent:i==1?new Color(.85f,.73f,.44f):new Color(.6f,.68f,.94f);
                    ExperimentGUI.Box(new Rect(590,from,lane-590,3),color);
                    ExperimentGUI.Box(new Rect(lane,Mathf.Min(from,to),3,Mathf.Max(3,Mathf.Abs(from-to))),color);
                    ExperimentGUI.Box(new Rect(lane,to,745-lane,3),color);
                    ExperimentGUI.Label(new Rect(685,from-22,70,20),(i+1)+" → "+(targetRow+1),small:true);
                }
            }
        }
        void DrawSchoolDestination()
        {
            ExperimentGUI.Label(new Rect(75,174,1125,45),"Onde Edelzio deve consultar os documentos originais?");
            ExperimentGUI.Panel(new Rect(75,235,540,290));
            ExperimentGUI.Label(new Rect(95,253,500,145),"Os recortes vieram de um acervo. Use o que você já descobriu para escolher o próximo lugar a investigar.\n\nSe precisar, consulte os serviços indicados no mural.");
            if(ExperimentGUI.Button(new Rect(95,435,500,60),"CONSULTAR MURAL DA SALA"))OpenSchoolDocument(6);
            string[] choices={"SECRETARIA DA ESCOLA","BIBLIOTECA DA INDUSTRIAL","COMPANHIA ELÉTRICA"};
            for(int i=0;i<3;i++)if(SchoolCard(new Rect(655,252+i*88,545,70),choices[i],_progress.school.destination==i))ChooseSchoolDestination(i);
        }
        void DrawSchoolConclusion()
        {
            ExperimentGUI.Label(new Rect(130,220,1020,80),"INVESTIGAR O ACERVO DA BIBLIOTECA",true);
            ExperimentGUI.Label(new Rect(130,310,1020,105),"Os registros têm uma origem comum. Preciso consultar os originais.\nAs datas organizam os relatos; as anotações coincidem com os detalhes publicados. A origem C-96 remete ao setor de memória local da Biblioteca da Industrial. A referência e os documentos continuam no caderno.");
            if(ExperimentGUI.Button(new Rect(130,457,430,55),"RELER DOCUMENTOS"))OpenSchoolDocument(0);
            if(ExperimentGUI.Button(new Rect(650,457,500,55),"SEGUIR PARA A BIBLIOTECA")&&PrepareLibraryDeparture())ContinueChapter();
        }
        void DrawSchoolDocument()
        {
            int id=_progress.school.selectedDocument;
            ExperimentGUI.Box(new Rect(0,0,1280,720),new Color(0,0,0,.9f));
            ExperimentGUI.Panel(new Rect(115,80,1050,570));
            ExperimentGUI.Label(new Rect(155,112,970,55),id<3?"RECORTE GUARDADO NA CAIXA":id<6?"MEU CADERNO DE INFÂNCIA":"MURAL DA ESCOLA",true);
            ExperimentGUI.Label(new Rect(155,190,970,340),CampaignSchoolDocuments.Text(id));
            if(ExperimentGUI.Button(new Rect(155,570,300,45),"VOLTAR À ANÁLISE"))CloseSchoolDocument();
            int first=id==6?6:0,last=id==6?6:5;
            if(id>first&&ExperimentGUI.Button(new Rect(680,570,200,45),"ANTERIOR")){_progress.school.Examine(id-1);Save();}
            if(id<last&&ExperimentGUI.Button(new Rect(905,570,220,45),"PRÓXIMO")){_progress.school.Examine(id+1);Save();}
        }
    }
}
