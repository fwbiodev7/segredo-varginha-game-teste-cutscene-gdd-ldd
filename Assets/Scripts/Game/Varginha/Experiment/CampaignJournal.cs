using System.Collections.Generic;
using UnityEngine;

namespace Game.Varginha.Experiment
{
    public static class CampaignJournal
    {
        public static List<string> Entries(CampaignStory story)
        {
            var entries = new List<string>();
            if(story.boxFound)entries.Add("CAIXA ESQUECIDA • Fotografia e páginas do caderno de 1996 encontradas sob a cama.");
            if(story.pagesSolved)entries.Add("PRIMEIRO FRAGMENTO • Casa à esquerda, árvore ao centro, figura à direita. ELA AINDA ESTÁ AQUI.");
            if(story.arrival)entries.Add("CAMINHO À INDUSTRIAL • O Fusca perdeu força perto da escola. Pelo rádio: NÃO DEIXA ELA SAIR.");
            if(story.buildingSolved)entries.Add("FOTOGRAFIA • O vulto no portão de 1996 não deveria estar ali.");
            if(story.codeSolved)entries.Add("SEGUNDO FRAGMENTO • As duas anotações na fotografia formam 23:23.");
            var e=story.expansion;
            if((e.visited&1)!=0)entries.Add("TERCEIRO FRAGMENTO • O livro da biblioteca conserva ÁRVORE, margem oeste, com carimbo de 23:23 da diocese.");
            if((e.visited&2)!=0)entries.Add("BIBLIOTECA • O rio atravessa o centro entre a árvore e a capela.");
            if((e.visited&4)!=0)entries.Add("RELATO DE 1996 • A capela fica a leste. O mapa segue ÁRVORE → RIO → CAPELA.");
            if(e.anchorFound)entries.Add("LIVRO DO TOMBO • Registro 23, ano 1996, símbolo ÂNCORA. Edelzio é o selo vivo desde o acidente.");
            if(e.reagentUnlocked)entries.Add("ANÁLISE DE OUZANA • Controle neutro, resíduo instável: o reagente revela as marcas de contenção.");
            if(e.sprayed!=0)entries.Add("FUSCA • Marcas registradas: "+((e.sprayed&1)!=0?"ÁRVORE • I; ":"")+((e.sprayed&2)!=0?"RIO • II; ":"")+((e.sprayed&4)!=0?"CAPELA • III.":""));
            for(int i=0;i<3;i++)if((e.truthClues&(1<<i))!=0)entries.Add(new[]{"DEPOIMENTO • Um relato foi retirado da reportagem de 1996.","RELATÓRIO • Veículo sem placa próximo ao clarão.","1898 • Zé Gomes registrou a contenção na região da mata."}[i]);
            for(int phase=11;phase<=21;phase++)
            {
                var d=CampaignContinuationDefinition.Get(phase);
                for(int i=0;i<d.documents.Length;i++)if((story.continuation.clues[phase-11]&(1<<i))!=0)
                    entries.Add(d.labels[i]+" • "+d.documents[i]);
            }
            if(story.continuation.finalCalibrated)entries.Add("RETORNO CALIBRADO • A manifestação foi dissipada. Edelzio mantém o selo até a entidade ferida atravessar.");
            if(story.continuation.finalStep>=2)entries.Add("TRAVESSIA • A entidade ferida atravessou. A ligação pode ser encerrada.");
            if(story.continuation.finalStep==4)entries.Add("ACORDO CUMPRIDO • A ruptura se fechou. Edelzio está livre.");
            return entries;
        }

        public static void Draw(CampaignStory story, ref Vector2 scroll, Rect bounds)
        {
            var entries=Entries(story);
            scroll=GUI.BeginScrollView(bounds,scroll,new Rect(0,0,bounds.width-25,Mathf.Max(bounds.height,entries.Count*190)));
            for(int i=0;i<entries.Count;i++)ExperimentGUI.Label(new Rect(10,i*190+5,bounds.width-55,180),entries[i],small:true);
            GUI.EndScrollView();
        }
    }
}
