using System.Collections.Generic;
using UnityEngine;

namespace Game.Varginha.Experiment
{
    public static class CampaignJournal
    {
        public static List<string> Entries(CampaignStory story)
        {
            var entries = new List<string>();
            if(story.phase==2)entries.Add("PRÓXIMO PASSO • "+story.house.Objective(story.routine));
            if(story.house.step>=1)entries.Add("ANTES DE SAIR • Deixei a chave do Fusca na mesa do café e voltei ao quarto para buscar a mochila.");
            if(story.house.Missing)entries.Add("CHAVE DESAPARECIDA • Ouvi metal na cozinha e papel raspando. A chave deixou de estar na mesa do café.");
            if((story.house.clues&1)!=0)entries.Add("MESA DO CAFÉ • A chave sumiu; não há nada no chão ao redor.");
            if((story.house.clues&2)!=0)entries.Add("SAÍDA • Trinco fechado por dentro. Não abri a porta desde que acordei.");
            if((story.house.clues&4)!=0)entries.Add("QUARTO • Papel envelhecido com traço azul preso junto à passagem.");
            if(story.house.deduced)entries.Add("HIPÓTESE • A saída fechada e o papel indicam investigar os objetos antigos dentro da casa.");
            if(story.boxFound)entries.Add("CAIXA ESQUECIDA • Chave, fotografia, páginas do caderno, três recortes de jornal e anotações infantis recuperados uma única vez sob a cama.");
            if(story.school.documentsRecovered)
                for(int id=0;id<6;id++)entries.Add(CampaignSchoolDocuments.Text(id));
            if(story.school.noticeRead)entries.Add(CampaignSchoolDocuments.Text(6));
            if(story.pagesSolved)entries.Add("PRIMEIRO FRAGMENTO • Casa à esquerda, árvore ao centro, figura à direita. ELA AINDA ESTÁ AQUI.");
            if(story.arrival)entries.Add("CAMINHO À INDUSTRIAL • O Fusca perdeu força perto da escola. Pelo rádio: NÃO DEIXA ELA SAIR.");
            if(story.school.stage>=1)entries.Add("CRONOLOGIA • Recortes organizados: 14/01, 20/01 e 26/01/1996.");
            if(story.school.stage>=2)entries.Add("PADRÕES • Rua das Acácias e clarão azul se repetem nos três jornais.");
            if(story.school.stage>=3)entries.Add("CRUZAMENTO • Poste, três batidas e árvore junto ao córrego: as anotações coincidem com os relatos publicados.");
            if(story.school.completed)entries.Add("SEGUNDO FRAGMENTO • Referência de origem preservada: coleção C-96, Acervo de História Local. Consultar originais na Biblioteca da Industrial, setor de memória local.");
            var e=story.expansion;
            if((e.visited&1)!=0)entries.Add("COLEÇÃO C-96 • ORIGINAL CONSULTADO. O livro da biblioteca conserva o terceiro fragmento: ÁRVORE, margem oeste, com carimbo de 23:23 da diocese.");
            if((e.visited&2)!=0)entries.Add("BIBLIOTECA • O rio atravessa o centro entre a árvore e a capela.");
            if((e.visited&4)!=0)entries.Add("RELATO DE 1996 • A capela fica a leste. O mapa segue ÁRVORE → RIO → CAPELA.");
            if((e.visited&1)!=0)entries.Add("LIVRO • A trilha começa na árvore; contorne o tronco caído.");
            if((e.visited&2)!=0)entries.Add("GLOBO • A ponte é a travessia registrada entre a árvore e a capela.");
            if((e.visited&4)!=0)entries.Add("PAINEL • Chegue à capela pelo portão; o muro bloqueia o outro acesso.");
            if(e.anchorFound)entries.Add("LIVRO DO TOMBO • Registro 23, ano 1996, símbolo ÂNCORA. Edelzio é o selo vivo desde o acidente.");
            if(e.reagentUnlocked)entries.Add("ANÁLISE DE OUZANA • Controle neutro, resíduo instável: o reagente revela as marcas de contenção.");
            if(e.sprayed!=0)entries.Add("FUSCA • Marcas registradas: "+((e.sprayed&1)!=0?"ÁRVORE • I; ":"")+((e.sprayed&2)!=0?"RIO • II; ":"")+((e.sprayed&4)!=0?"CAPELA • III.":""));
            for(int i=0;i<3;i++)if((e.truthClues&(1<<i))!=0)entries.Add(new[]{"DEPOIMENTO • Um relato foi retirado da reportagem de 1996.","RELATÓRIO • Veículo sem placa próximo ao clarão.","1898 • Zé Gomes registrou a contenção na região da mata."}[i]);
            for(int phase=11;phase<=21;phase++)
            {
                if(phase==17)continue; // Removed scene; serialized evidence slots remain untouched.
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
            ExperimentGUI.Ribbon(new Rect(bounds.x,bounds.y,bounds.width,64));
            ExperimentGUI.Label(new Rect(bounds.x+12,bounds.y+8,bounds.width-24,50),"AGORA • "+CampaignGuidance.Next(story,story.phase,story.continuation.area),small:true);
            bounds=new Rect(bounds.x,bounds.y+72,bounds.width,bounds.height-72);
            Rect previous=new(bounds.x,bounds.yMax-36,180,32),next=new(bounds.xMax-180,bounds.yMax-36,180,32);
            bounds.height-=44;
            float maximum=Mathf.Max(0,entries.Count*190-bounds.height);
            float scrollStep=Mathf.Min(190,Mathf.Max(32,bounds.height-16));
            scroll=VarginhaGamepadUI.BeginScrollView(bounds,scroll,new Rect(0,0,bounds.width-25,Mathf.Max(bounds.height,entries.Count*190)));
            for(int i=0;i<entries.Count;i++)ExperimentGUI.Label(new Rect(10,i*190+5,bounds.width-55,180),entries[i],small:true);
            VarginhaGamepadUI.EndScrollView();
            // Explicit controls keep long evidence catalogs reachable with controller and keyboard.
            bool enabled=GUI.enabled;GUI.enabled=enabled&&scroll.y>0;
            if(ExperimentGUI.Button(previous,"ANTERIOR"))scroll.y=Mathf.Max(0,scroll.y-scrollStep);
            GUI.enabled=enabled&&scroll.y<maximum;
            if(ExperimentGUI.Button(next,"PRÓXIMO"))scroll.y=Mathf.Min(maximum,scroll.y+scrollStep);
            GUI.enabled=enabled;
        }
    }
}
