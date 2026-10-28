using System;
using UnityEngine;

namespace Game.Varginha.Experiment
{
    // Stable document IDs: 0..2 newspapers, 3..5 childhood notes, 6 school notice.
    public static class CampaignSchoolDocuments
    {
        public static readonly string[] Titles = {
            "14/01/1996 • FALHA DE ENERGIA NO BAIRRO",
            "20/01/1996 • MORADORES RELATAM BATIDAS",
            "26/01/1996 • ARQUIVO REÚNE OS RELATOS",
            "ANOTAÇÃO INFANTIL • O POSTE", "ANOTAÇÃO INFANTIL • O PORTÃO",
            "ANOTAÇÃO INFANTIL • A ÁRVORE", "MURAL • CONSULTA A DOCUMENTOS" };
        public static readonly string[][] Passages = {
            new[]{"Local: Rua das Acácias.", "Moradores descrevem um clarão azul.", "O poste apagou antes das 19h."},
            new[]{"Local: Rua das Acácias.", "Moradores descrevem um clarão azul.", "Três batidas foram ouvidas junto ao portão."},
            new[]{"Local: Rua das Acácias.", "Moradores descrevem um clarão azul.", "O desenho entregue marca uma árvore ao lado do córrego."} };
        public static readonly string[] Notes = {
            "Eu estava desenhando quando o poste apagou. Ainda não eram sete da noite. Será que mais alguém viu?",
            "Contei três batidas no portão. Escrevi aqui para não esquecer quantas foram.",
            "Desenhei a árvore que fica ao lado do córrego. Guardei uma cópia junto dos recortes." };
        public const string Source = "Origem: coleção C-96 • Acervo de História Local. Cópia para consulta; o original permanece no acervo.";
        public const string Notice = "SERVIÇOS DE CONSULTA\nBiblioteca da Industrial: acervos de história local e originais de jornais.\nSecretaria da escola: projetos escolares e notas da turma.\nCompanhia elétrica: manutenção da rede e pedidos atuais.\nPara pesquisar um documento, leve sua referência de origem.";
        public static readonly string[] ComparisonChoices={"O local: Rua das Acácias.","A descrição de um clarão azul.","Três batidas ouvidas no portão."};
        public const int UniqueEvent=2;
        public static string Text(int id)
        {
            if(id<0||id>=Titles.Length)return "";
            if(id<3)return "JORNAL DA CIDADE • RECORTE FICCIONAL DE 1996\n"+Titles[id]+"\n\n"+string.Join("\n",Passages[id])+"\n\n"+Source;
            return id<6?Titles[id]+"\n\n"+Notes[id-3]:Notice;
        }
    }

    [Serializable]
    public sealed class CampaignSchoolInvestigation
    {
        public bool documentsRecovered, noticeRead, completed;
        public int examined, stage, highlights, hintLevel, destination=-1, selectedDocument;
        public int comparisonChoice=-1;
        public bool comparisonMigrated;
        public int[] order={2,0,1}, links={-1,-1,-1};
        // Display rows are separate from document IDs: saved associations retain their meaning.
        public int[] noteOrder, passageOrder;
        public bool RecoverDocuments()
        { if(documentsRecovered)return false;documentsRecovered=true;EnsureLinkLayout();return true; }
        public bool EnsureLinkLayout(System.Random random=null)
        {
            if(ValidLinkLayout())return false;
            random??=new System.Random(Guid.NewGuid().GetHashCode());
            do
            {
                noteOrder=new[]{0,1,2};
                for(int i=2;i>0;i--){int j=random.Next(i+1);(noteOrder[i],noteOrder[j])=(noteOrder[j],noteOrder[i]);}
            }while(CampaignStory.Sequence(noteOrder,new[]{0,1,2}));
            // Rotations have no correct pair on the same row. Exclude the canonical
            // right column too, so both sides are shuffled rather than just one.
            int[] first={noteOrder[1],noteOrder[2],noteOrder[0]},second={noteOrder[2],noteOrder[0],noteOrder[1]};
            passageOrder=CampaignStory.Sequence(first,new[]{0,1,2})?second
                :CampaignStory.Sequence(second,new[]{0,1,2})?first:random.Next(2)==0?first:second;
            return true;
        }
        bool ValidLinkLayout()
        {
            if(noteOrder==null||noteOrder.Length!=3||!Permutation(noteOrder)
                ||passageOrder==null||passageOrder.Length!=3||!Permutation(passageOrder)
                ||CampaignStory.Sequence(noteOrder,new[]{0,1,2})||CampaignStory.Sequence(passageOrder,new[]{0,1,2}))return false;
            for(int i=0;i<3;i++)if(noteOrder[i]==passageOrder[i])return false;
            return true;
        }
        public bool Examine(int id)
        {
            if(id<0||id>6||id<6&&!documentsRecovered)return false;
            examined|=1<<id;if(id==6)noticeRead=true;selectedDocument=id;return true;
        }
        public bool Swap(int a,int b)
        {
            if(!documentsRecovered||stage!=0||a<0||a>2||b<0||b>2)return false;
            (order[a],order[b])=(order[b],order[a]);return true;
        }
        public bool ToggleHighlight(int passage)
        {
            // Old save bits are retained for migration; the revised puzzle uses one answer.
            return false;
        }
        public bool ChooseComparison(int value)
        {if(!documentsRecovered||stage!=1||value<0||value>=CampaignSchoolDocuments.ComparisonChoices.Length)return false;comparisonChoice=value;return true;}
        public bool Link(int note,int passage)
        {
            if(!documentsRecovered||stage!=2||note<0||note>2||passage< -1||passage>2)return false;
            links[note]=passage;return true;
        }
        public bool ChooseDestination(int value)
        {if(stage!=3||value<0||value>2)return false;destination=value;return true;}
        public bool Submit(out string feedback)
        {
            feedback="";
            if(!documentsRecovered){feedback="Os materiais precisam estar guardados na mochila.";return false;}
            if(completed)return false;
            switch(stage)
            {
                case 0:
                    if((examined&7)!=7){feedback="Abra os três recortes; as datas estão impressas no cabeçalho.";return false;}
                    if(!CampaignStory.Sequence(order,new[]{0,1,2})){feedback="Compare dia, mês e ano. Coloque o recorte mais antigo à esquerda.";return false;}
                    break;
                case 1:
                    if(comparisonChoice!=CampaignSchoolDocuments.UniqueEvent){feedback="Lembre os fatos presentes nos três registros e procure o acontecimento mencionado em apenas um deles.";return false;}
                    highlights=219;comparisonMigrated=true;
                    break;
                case 2:
                    if(!CampaignStory.Sequence(links,new[]{0,1,2})){feedback="Compare cada lembrança com o trecho que descreve o mesmo fato. Você pode corrigir ou desfazer uma ligação.";return false;}
                    break;
                case 3:
                    if(destination!=1){feedback="Pense no tipo de acervo que conserva os originais. O mural pode ajudar a comparar os serviços de consulta.";return false;}
                    completed=true;break;
                default:return false;
            }
            stage++;hintLevel=0;feedback=completed?"INVESTIGAR O ACERVO DA BIBLIOTECA":"Relação registrada. Continue a análise.";return true;
        }
        public string Objective => completed?"Investigar o acervo da biblioteca. Retome a mesa para partir.":
            new[]{"Na mesa do professor, organize seus recortes pelas datas.","Lembre os recortes e identifique o acontecimento que não se repetiu.","Ligue cada anotação infantil ao trecho que a sustenta.","Decida onde consultar os originais; o mural pode ajudar."}[Mathf.Clamp(stage,0,3)];
        public string Hint(int level)
        {
            string[][] tips={
                new[]{"O cabeçalho traz a data completa.","Compare o dia: os três recortes são de janeiro de 1996.","A ordem é 14/01 → 20/01 → 26/01. Abra cada recorte e troque os cartões."},
                new[]{"Relembre o que os três relatos tinham em comum.","O mesmo local e a mesma luz aparecem mais de uma vez.","As três batidas no portão aparecem em apenas um dos recortes."},
                new[]{"A lembrança e o jornal descrevem o mesmo acontecimento.","Compare poste/horário, número de batidas e posição da árvore.","Poste → falha de energia; portão → três batidas; árvore → desenho junto ao córrego."},
                new[]{"O tipo de acervo importa mais que o assunto da notícia.","Onde se conservam registros de história local e jornais antigos?", "A Biblioteca da Industrial guarda esse acervo."}};
            return completed?"A referência da coleção ficou registrada no caderno. Siga para a biblioteca.":tips[Mathf.Clamp(stage,0,3)][Mathf.Clamp(level,0,2)];
        }
        public void Repair()
        {
            EnsureLinkLayout();
            examined&=127;highlights&=511;hintLevel=Mathf.Clamp(hintLevel,0,2);selectedDocument=Mathf.Clamp(selectedDocument,0,6);
            if(order==null||order.Length!=3||!Permutation(order))order=new[]{2,0,1};
            if(links==null||links.Length!=3)links=new[]{-1,-1,-1};
            for(int i=0;i<3;i++)links[i]=Mathf.Clamp(links[i],-1,2);
            destination=Mathf.Clamp(destination,-1,2);
            comparisonChoice=Mathf.Clamp(comparisonChoice,-1,2);
            if(!comparisonMigrated)
            {
                if(stage>=2&&highlights==219)comparisonChoice=CampaignSchoolDocuments.UniqueEvent;
                comparisonMigrated=true;
            }
            if(!documentsRecovered){stage=0;completed=false;examined&=64;highlights=0;comparisonChoice=-1;links=new[]{-1,-1,-1};}
            // Reconstruct the highest stage supported by its evidence. Corrupt saves cannot skip steps.
            int valid=0;
            if(documentsRecovered&&(examined&7)==7&&CampaignStory.Sequence(order,new[]{0,1,2}))
            {valid=1;if(comparisonChoice==CampaignSchoolDocuments.UniqueEvent){valid=2;if(CampaignStory.Sequence(links,new[]{0,1,2})){valid=3;if(destination==1)valid=4;}}}
            if(completed&&valid==4&&stage>=4)stage=4;
            else{stage=Mathf.Clamp(stage,0,Mathf.Min(valid,3));completed=false;}
        }
        public void MigrateCompleted()
        {
            EnsureLinkLayout();
            documentsRecovered=noticeRead=completed=true;examined=127;stage=4;
            order=new[]{0,1,2};highlights=219;links=new[]{0,1,2};destination=1;
            comparisonChoice=CampaignSchoolDocuments.UniqueEvent;comparisonMigrated=true;
        }
        static bool Permutation(int[] a)=>a[0]>=0&&a[0]<3&&a[1]>=0&&a[1]<3&&a[2]>=0&&a[2]<3&&a[0]!=a[1]&&a[1]!=a[2]&&a[0]!=a[2];
    }
}
