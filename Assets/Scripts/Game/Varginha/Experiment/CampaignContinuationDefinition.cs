using UnityEngine;

namespace Game.Varginha.Experiment
{
    public sealed class CampaignContinuationDefinition
    {
        public int phase, map, required;
        public string title, goal, question, success, intro;
        public string[] ids, labels, documents, cards;
        public bool choice;
        public static CampaignContinuationDefinition Get(int phase,int area=0)
        {
            if(phase==18)
            {
                var d=Later(18,area);var agreement=Later(19,0);
                d.title="A CRIATURA E O ACORDO";
                d.goal="Investigue a criatura ferida e reconstrua o procedimento do acordo temporário.";
                d.documents[0]+="\n"+agreement.documents[0]+"\n"+agreement.documents[1];
                d.documents[1]+="\n"+agreement.documents[3];
                d.documents[2]+="\n"+agreement.documents[2];
                d.question=agreement.question;d.cards=agreement.cards;d.choice=false;
                d.success="A memória completa chega ao clarão: Edelzio aceitou ser um selo temporário para proteger Varginha. A criatura ferida precisa voltar; a manifestação do mecanismo impede o retorno. Dissipe a manifestação, calibre a passagem, aguarde a travessia e só então encerre a ligação.";
                return d;
            }
            if(phase>=13)return Later(phase,area);
            if(phase==12)return new CampaignContinuationDefinition {
                phase=12,map=area==0?12:112,title="O CASARÃO DE ZÉ GOMES",required=3,choice=true,
                intro=area==0?"A planta preservada por Renan indica um acesso de serviço. O jardim e o escritório ainda guardam sinais das reformas.":"O térreo foi adaptado para experiências. Examine o escritório e compare a parede com a planta antiga.",
                goal="Compare a planta, revele a passagem com reagente e encontre a chave no escritório.",
                ids=new[]{"plan","reveal","key","photo"},labels=new[]{"PLANTA PRESERVADA","PAREDE DE SERVIÇO","CHAVE DO ESCRITÓRIO","FOTOGRAFIA DO GRUPO"},
                documents=new[]{"A planta identifica uma passagem de serviço ao lado direito do casarão. A porta principal leva ao térreo; o acesso lateral foi escondido durante uma reforma.","O reagente revela o contorno da porta perdida. A passagem existe atrás da parede; uma fechadura antiga ainda segura o mecanismo.","Uma chave antiga ficou junto às gavetas do escritório. Ela corresponde à fechadura do acesso de serviço.","Zé Gomes aparece acompanhado por outras pessoas. Atrás do grupo, o mesmo símbolo registrado no Livro do Tombo. Ele não operava sozinho."},
                question="A planta e o reagente concordam: por onde os registros do porão eram transportados?",
                cards=new[]{"PASSAGEM DE SERVIÇO","CISTERNA DO JARDIM","PORTA PRINCIPAL"},
                success="A chave abre o acesso de serviço. A fotografia associa Zé Gomes ao grupo do Tombo. Os registros das experiências estão no porão."
            };
            if(phase!=11)throw new System.ArgumentOutOfRangeException(nameof(phase));
            return new CampaignContinuationDefinition {
                phase=11,map=11,title="ARQUIVOS ALTERADOS",required=3,choice=true,
                intro="Renan: A cópia impressa não mudou. Guardei o original e o arquivo alterado. Vamos comparar antes de perder outra pista.",
                goal="Converse com Renan. Compare as três versões no notebook do professor.",
                ids=new[]{"print","original","altered"},labels=new[]{"IMPRESSÃO PRESERVADA","ARQUIVO ORIGINAL","CÓPIA ALTERADA"},
                documents=new[]{
                    "Na impressão, a margem conserva 1898 e uma linha aponta para a passagem de serviço ao lado do casarão de Zé Gomes. Renan anotou os dois detalhes antes da interferência.",
                    "O original confirma a mesma linha lateral e a data 1898. A porta principal está no centro; a referência investigada é a passagem de serviço à direita.",
                    "Na cópia alterada, a linha do acesso de serviço foi apagada e a data ficou ilegível. A fachada e a porta principal continuam iguais. Compare com o papel para reconstruir o caminho."},
                question="Qual referência foi apagada, mas continua nas duas cópias preservadas?",
                cards=new[]{"ACESSO DE SERVIÇO • 1898","PORTA PRINCIPAL • 2026","QUADRO DA ESCOLA • 23:23"},
                success="Renan salva a planta reconstruída do casarão. Ele desliga o notebook; por alguns segundos, a imagem continua na tela. Renan: Leve a impressão. Eu fico com as cópias confiáveis."
            };
        }
        private static CampaignContinuationDefinition D(int phase,int map,string title,string goal,string intro,string[] ids,string[] labels,string[] documents,string question,string[] cards,string success,bool choice=false,int required=-1)
            =>new CampaignContinuationDefinition{phase=phase,map=map,title=title,goal=goal,intro=intro,ids=ids,labels=labels,documents=documents,question=question,cards=cards,success=success,choice=choice,required=required<0?ids.Length:required};
        private static CampaignContinuationDefinition Later(int phase,int area)
        {
            switch(phase)
            {
                case 13:return D(13,13,"OS REGISTROS DE 1898","Examine páginas, fotografias e materiais. Reconstrua a ordem dos acontecimentos.","As páginas do porão descrevem a passagem encontrada pelo grupo de Zé Gomes. A mesa central permite reunir os registros.",
                    new[]{"pages","photos","materials","optional"},new[]{"PÁGINAS DO ARQUIVO","FOTOGRAFIAS DAS EXPERIÊNCIAS","MATERIAIS DO MECANISMO","ANOTAÇÃO DA BANCADA"},
                    new[]{"1898: primeiro, o grupo descobriu a passagem. Depois, iniciou experiências para estudá-la. Uma anotação marca o início das alterações.","As fotografias mostram experiências antes dos danos na sala. A ruptura atingiu o grupo; só depois foram instalados os dispositivos de contenção.","Os instrumentos ligavam o mecanismo a pessoas vivas. A contenção foi uma resposta à ruptura, não o propósito original da descoberta.","Os materiais foram transportados pelo acesso de serviço. Esta anotação complementa o arquivo e não é necessária para continuar."},
                    "Ordene os registros do início ao fim. Troque dois cartões para mudar sua posição.",new[]{"DESCOBERTA","EXPERIÊNCIAS","RUPTURA","CONTENÇÃO"},"A mesma sequência aparece nas páginas e nas fotografias. O sistema passou a depender de pessoas vivas. Fábio precisa explicar o que aconteceu depois.",false,3);
                case 14:return D(14,14,"O QUE FÁBIO ESCONDEU","Apresente os registros a Fábio. Compare 1898, 1996 e 2026 no Tombo.","Fábio reconhece os documentos do casarão. O Livro do Tombo conserva as datas; uma página retirada explica a transferência do selo.",
                    new[]{"records","page","current"},new[]{"TOMBO • 1898","PÁGINA OCULTADA • 1996","ANOTAÇÃO ATUAL • 2026"},
                    new[]{"1898: a ruptura foi contida com um mecanismo ligado a pessoas vivas. Os guardiões conservaram essa dependência.","1996: o sistema falhou. Edelzio criança tornou-se o selo vivo num acordo temporário. Depois do acidente, os guardiões mantiveram a ligação e ocultaram esta página.","2026: a ligação está falhando novamente. O reagente, os arquivos alterados e a marca de Edelzio registram o mesmo processo."},
                    "Associe as três datas à continuidade do mecanismo: do registro mais antigo ao atual.",new[]{"1898 • MECANISMO","1996 • EDELZIO, SELO VIVO","2026 • FALHA DA LIGAÇÃO"},"Fábio: Eu escondi a página. Você aceitou uma ligação temporária; nós a mantivemos. O acesso ao subsolo está aqui. Você precisa lembrar a conversa daquela noite.");
                case 15:return D(15,15,"A NOITE INTERROMPIDA","Reúna os quatro vestígios da casa e organize a lembrança de 1996.","A casa reaparece como Edelzio a viu quando criança. As lembranças ainda estão incompletas: notícia, desenho, falta de energia e a presença no quintal.",
                    new[]{"report","drawing","power","presence"},new[]{"NOTÍCIA NA TELEVISÃO","DESENHO INFANTIL","FALHA DE ENERGIA","PRESENÇA NO QUINTAL"},
                    new[]{"A notícia veio primeiro. As vozes falavam de algo visto na cidade naquela noite.","Depois da notícia, Edelzio desenhou a figura. O desenho existe antes de a luz apagar.","A energia falhou após o desenho. A luz do quintal não vinha da rede elétrica.","Quando a energia já tinha falhado, a presença ferida apareceu. Edelzio ouviu um pedido de ajuda antes do clarão."},
                    "Em que ordem Edelzio viveu estes momentos?",new[]{"NOTÍCIA","DESENHO","FALTA DE ENERGIA","PRESENÇA"},"A criatura estava ferida. Edelzio criança ouviu que a passagem precisava ser estabilizada. A lembrança da conversa voltou; o acidente ainda encobre seu final.");
                case 16:return D(16,16,"A DESCIDA","Leia o esquema e a área inundada. Desvie a alimentação no painel de manutenção.","Fábio indicou o acesso. Ouzana deixou suprimentos na entrada. Renan mantém as cópias disponíveis. O corredor norte está bloqueado pelo sistema antigo.",
                    new[]{"scheme","water","panel","supplies"},new[]{"ESQUEMA PRESERVADO","ÁREA INUNDADA","PAINEL DE MANUTENÇÃO","SUPRIMENTOS DE OUZANA"},
                    new[]{"O esquema separa dois circuitos: passagem norte e área inundada. A alimentação da porta pode ser ligada sem energizar a água.","Os condutores expostos entram na água. Este circuito precisa permanecer desligado enquanto a passagem recebe alimentação.","PAINEL: desviar alimentação para PASSAGEM; manter ÁREA INUNDADA desligada. O circuito geral não é seguro.","Ouzana deixou reagente e as leituras essenciais. Renan preservou cópias do mapa e do arquivo; nenhum documento opcional é exigido para o retorno."},
                    "Qual circuito abre a passagem sem energizar a água?",new[]{"PASSAGEM LIGADA • ÁGUA DESLIGADA","CIRCUITO GERAL LIGADO","ÁGUA LIGADA • PASSAGEM DESLIGADA"},"A passagem recebe alimentação, a água permanece isolada. O rádio repete uma voz infantil. Além da alvenaria, as paredes mudam de aparência.",true,3);
                case 17:return D(17,17+area*100,"AS CÂMARAS DO SELO","Consulte o diagrama e estabilize ligação, circulação e contenção no painel central.","Os fragmentos identificam ÁRVORE, RIO e CAPELA. Cada regulador afeta sua câmara e a seguinte. Observe a ameaça; o abrigo interrompe a perseguição.",
                    new[]{"reading0","reading1","reading2","diagram"},new[]{"ÁRVORE • LIGAÇÃO","RIO • CIRCULAÇÃO","CAPELA • CONTENÇÃO","DIAGRAMA DOS REGULADORES"},
                    new[]{"ÁRVORE: a leitura estável de ligação é 1. Ela soma os reguladores da árvore e da capela; valores acima de 2 retornam a 0.","RIO: a circulação estável é 2. Ela soma os reguladores do rio e da árvore; valores acima de 2 retornam a 0.","CAPELA: a contenção estável é 0. Ela soma os reguladores da capela e do rio; valores acima de 2 retornam a 0.","Cada regulador possui 0, 1 e 2. A configuração segura precisa produzir leituras ÁRVORE 1, RIO 2, CAPELA 0 simultaneamente. O diagrama reúne as leituras; o painel central comanda os três reguladores."},
                    "Use o painel central para ajustar os três reguladores e conferir as leituras do conjunto.",new[]{"ÁRVORE • 1","RIO • 2","CAPELA • 0"},"As três leituras se estabilizam. A presença permanece visível e aponta para uma abertura além do mecanismo.",false,3);
                case 18:return D(18,18,"A CRIATURA FERIDA","Compare os registros humanos, as marcas de ferimento e a leitura do reagente.","O complexo foi construído sobre a ruptura. Os instrumentos mantêm a entidade presa; Ouzana acompanha as leituras.",
                    new[]{"human","wounds","reading"},new[]{"REGISTROS HUMANOS","MARCAS DA CONTENÇÃO","LEITURAS DE OUZANA"},
                    new[]{"Os registros diferenciam a passagem de retorno da ligação de contenção. Edelzio é o selo vivo; romper sua ligação antes da travessia deixaria a ruptura instável.","Os ferimentos coincidem com os pontos de contenção. A presença procura Edelzio para recuperar o vínculo necessário à passagem.","O reagente destaca dois circuitos: símbolos externos abrem a passagem; marcas internas mantêm o selo. É necessário estabilizar, abrir o retorno e só então encerrar a ligação."},
                    "Qual interpretação reúne os registros e as leituras?",new[]{"PASSAGEM EXTERNA • SELO INTERNO","ROMPER O SELO ANTES DE ABRIR","MANTER A CONTENÇÃO PARA SEMPRE"},"Edelzio: Você está tentando voltar. A entidade projeta a memória de 1996. Uma ruptura sem preparação colocaria a cidade em risco.",true);
                case 19:return D(19,19,"O ACORDO ESQUECIDO","Reúna a página, o arquivo, o caderno e a memória. Reconstrua o acordo temporário.","O quintal reaparece até o instante do acidente. A página ocultada e as cópias preservadas ajudam a completar a conversa.",
                    new[]{"hiddenpage","records","notebook","memory"},new[]{"PÁGINA OCULTADA","REGISTROS DE 1898","CADERNO PRESERVADO","LEMBRANÇA COMPLETA"},
                    new[]{"A página registra uma ligação temporária: ajudar a impedir que a ruptura atingisse as pessoas enquanto o retorno fosse preparado.","1898 explica o risco de uma ruptura aberta sem estabilidade. A contenção permanente foi uma decisão posterior dos guardiões.","As anotações recentes eram lembranças que voltavam durante a interferência. As cópias de Renan recuperam qualquer prova essencial que Edelzio tenha deixado para trás.","Não deixa ela sair: pela ruptura instável. A frase pedia que Edelzio evitasse o desastre; não ordenava destruir a criatura. O acordo terminaria depois de um retorno seguro."},
                    "Reconstrua o procedimento que cumpre o acordo.",new[]{"ESTABILIZAR","ABRIR RETORNO","ENTIDADE ATRAVESSA","ENCERRAR LIGAÇÃO"},"A memória alcança o clarão. Edelzio reconhece a origem de suas anotações. Ouzana, Fábio e Renan confirmam o procedimento: ele é o selo da entidade e poderá se libertar após a travessia.");
                case 20:return D(20,20,"O VERDADEIRO SEGREDO","Prepare ÁRVORE, RIO e CAPELA. Enfrente a manifestação antes de realizar o retorno seguro.",area==2?"De volta à Industrial, a turma retoma suas atividades. Renan conserva os registros. O Fusca espera na rua.":"As câmaras estão ligadas à ruptura central. Prepare os três pontos. Na mochila, escolha um aluno ou um dos amigos para apoiar o combate.",
                    new[]{"tree","river","chapel"},new[]{"ÁRVORE • PREPARAR LIGAÇÃO","RIO • PREPARAR CIRCULAÇÃO","CAPELA • PREPARAR CONTENÇÃO"},
                    new[]{"O mapa e o arquivo identificam a ligação. As marcas de contenção permanecem até a travessia.","O reagente confirma a circulação estável para a passagem. Ouzana acompanha as leituras.","Fábio utiliza o Tombo para preparar a interrupção da ligação imposta a Edelzio. O selo será encerrado depois que a entidade atravessar."},
                    "Procedimento de retorno",new[]{"ESTABILIZAR","ABRIR PASSAGEM","AGUARDAR TRAVESSIA","ENCERRAR SELO"},"A manifestação se dissipa. Com a ruptura estável, Edelzio e os amigos podem ajudar a entidade a retornar.");
                case 21:return D(21,area==1?121:21,"O RETORNO",area==1?"Converse com Renan, abra o caderno e volte ao Fusca.":"Abra a passagem estável. Aguarde a travessia para encerrar o selo.",area==1?"A Industrial retoma suas atividades. Renan preservou as cópias. Edelzio pode finalmente escrever sobre 1996.":"A manifestação se dissipou. A entidade ferida permanece. Agora é possível cumprir o acordo sem romper a ligação antes da hora.",
                    new[]{"procedure","entity","friends"},new[]{"MECANISMO DE RETORNO","ENTIDADE FERIDA","OUZANA E FÁBIO"},new[]{"Os três circuitos estão estáveis. Abra o retorno mantendo o selo durante a travessia.","A entidade consegue alcançar a abertura preparada. Espere que ela atravesse antes de interromper a ligação.","Ouzana confirma as leituras; Fábio acompanha o mecanismo. Eles ficam ao lado de Edelzio enquanto a ruptura se fecha."},"Cumpra o acordo temporário",new[]{"ABRIR RETORNO","AGUARDAR TRAVESSIA","ENCERRAR SELO"},"Eu sei o que vi. E agora sei por que voltei.");
                default:throw new System.ArgumentOutOfRangeException(nameof(phase));
            }
        }
        public static CampaignMapPlan Plan(int phase,int area=0)
        {
            var definition=Get(phase,area);var plan=new CampaignMapPlan{phase=definition.map,title=definition.title};
            CampaignIllustratedMaps.Apply(plan);
            if(phase==13)plan.points.Add(new CampaignMapPlan.Point("ground","ESCADA • VOLTAR AO TÉRREO",plan.spawn.x,plan.spawn.y));
            foreach(var point in plan.points)
            {
                if(phase==17&&point.id=="reading")point.id="reading"+area;
                int index=System.Array.IndexOf(definition.ids,point.id);
                if(index>=0)point.label=definition.labels[index];
                if(point.id=="renan")point.label="RENAN • CONVERSAR";
                else if(point.id=="notebook")point.label="NOTEBOOK • COMPARAR EVIDÊNCIAS";
                else if(point.id=="exit")point.label=phase==11?"PARTIR PARA O CASARÃO":phase==12&&area==1?"ACESSO DE SERVIÇO • DESCER AO PORÃO":"SAÍDA • CONTINUAR A INVESTIGAÇÃO";
                else if(point.id=="entrance")point.label="PORTA PRINCIPAL • ENTRAR NO TÉRREO";
                else if(point.id=="service")point.label="PORTA DE SERVIÇO • ABRIR PASSAGEM";
                else if(point.id=="puzzle")point.label="CONFERIR AS EVIDÊNCIAS";
                else if(point.id=="fabio")point.label="FÁBIO • APRESENTAR REGISTROS";
                else if(point.id=="timeline")point.label="TOMBO • COMPARAR DATAS";
                else if(point.id=="hide")point.label="ABRIGO • AGUARDAR A AMEAÇA";
                else if(point.id=="previous")point.label="CÂMARA ANTERIOR";
                else if(point.id=="next")point.label="PRÓXIMA CÂMARA";
                else if(point.id=="procedure")point.label=phase==20?"MECANISMO • CALIBRAR RETORNO":"PROCEDIMENTO DE RETORNO";
            }
            return plan;
        }
        public static string SceneName(int phase,int area=0)=>"Ato"+(phase<14?4:phase<18?5:6)+"_Fase"+phase+"_"+new[]{"Arquivos_Alterados","Casarao_de_Ze_Gomes","Registros_de_1898","O_Que_Fabio_Escondeu","A_Noite_Interrompida","A_Descida","Camaras_do_Selo","A_Criatura_Ferida","O_Acordo_Esquecido","O_Verdadeiro_Segredo","O_Retorno"}[phase-11]+(area>0?"_Area"+area:"");
    }
}
