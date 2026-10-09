using UnityEngine;

namespace Game.Varginha.Experiment
{
    public static class CampaignHints
    {
        public static string Get(CampaignStory s,int id,int level)
        {
            string[] tips;
            switch(id)
            {
                case 1:tips=new[]{"Observe a luz e a televisão.","Quando a energia falhar, siga a luz pela porta à direita.","Examine a TV e alcance o quintal para concluir a lembrança."};break;
                case 2:tips=s.pagesSolved?new[]{"As pistas estão guardadas.","O Fusca espera no quintal à direita.","Vá ao Fusca para levar a fotografia à Industrial."}:new[]{"A fotografia pode organizar a lembrança.","Abra a caixa sob a cama e compare as páginas com a foto.","Organize CASA → ÁRVORE → FIGURA. A rotina da casa é opcional."};break;
                case 3:case 4:case 5:
                    tips=!s.renanMet?new[]{"Renan preservou uma pista.","Procure Renan junto ao quadro branco.","Converse com Renan e abra o notebook do professor."}
                        :!s.buildingSolved?new[]{"Algo na fotografia não pertence ao portão.","Observe a figura junto ao portão no notebook de Renan.","Escolha a resposta sobre o VULTO NO PORTÃO, a segunda alternativa."}
                        :!s.codeSolved?new[]{"As duas anotações parecem formar uma única informação.","Reabra o notebook e compare os dois grupos separados por dois pontos.","O horário é 23:23. Mostre a descoberta a Renan para seguir à biblioteca."}
                        :new[]{"A ampliação revelou um fragmento.","Renan pode confirmar o horário.","Fale com Renan e continue para Fragmentos."};break;
                case 6:tips=new[]{"Os três documentos descrevem o mesmo trajeto.","Compare livro, globo e painel na biblioteca; monte os fragmentos na mesa.","Posicione ÁRVORE → RIO → CAPELA. Setas da árvore e do rio: LESTE. Chegada à capela: NORTE."};break;
                case 7:tips=new[]{"O caminho repete os símbolos do mapa.","Siga as marcas e use o esconderijo quando a presença se aproximar.","Examine ÁRVORE → RIO → CAPELA e converse com Fábio. O rio registra o checkpoint."};break;
                case 8:tips=new[]{"O registro de Edelzio pertence à noite do acidente.","Cruze o índice, a inscrição do compartimento e o número da ficha.","Use 1996 • ÂNCORA • REGISTRO 23 no Livro do Tombo."};break;
                case 9:tips=new[]{"Ouzana pode interpretar o registro e as marcas.","Apresente a Ouzana o registro de Edelzio obtido no Tombo.","A comparação curta com Ouzana entrega o reagente. Siga à oficina; não há ordenação obrigatória de amostras."};break;
                case 10:tips=!s.expansion.workshopParked?new[]{"É preciso deixar o Fusca na oficina.","Conduza até a vaga, voltado para cima.","Pare na vaga e acione a interação para sair. Depois aplique reagente na lataria."}
                    :s.expansion.stabilized?new[]{"As marcas já estão no caderno.","O Fusca está pronto para partir.","Use a saída e aguarde a partida. A pista de teste é dispensável."}
                    :new[]{"O reagente revela o que a lataria esconde.","Aplique o reagente em um ponto do Fusca. A reserva repõe cargas se necessário.","Uma aplicação revela ÁRVORE • I, RIO • II e CAPELA • III. Não há segunda ordenação."};break;
                case 11:tips=new[]{"Uma cópia foi alterada; as outras preservam a referência.","Converse com Renan e compare impressão, original e cópia alterada no notebook.","Marque ACESSO LATERAL e DATA DO REGISTRO: são os dois detalhes apagados na cópia."};break;
                case 12:tips=new[]{"A planta e a chave indicam o mesmo acesso.","Revele a parede com reagente e procure a chave no escritório do térreo.","Com passagem revelada e chave, use o acesso de serviço para descer diretamente ao porão."};break;
                case 13:tips=new[]{"A contenção respondeu a um problema anterior.","Compare páginas, fotografias e materiais na mesa de investigação.","Associe página à descoberta, fotografia às experiências, danos à ruptura e instrumentos à contenção."};break;
                case 14:tips=new[]{"As três datas mostram como a ligação permaneceu.","Apresente as provas a Fábio e compare os registros no Tombo.","Associe instrumentos a 1898, ligação de Edelzio a 1996 e interferência atual a 2026."};break;
                case 15:tips=new[]{"A presença apareceu depois que a energia falhou.","Compare TV, desenho, falta de energia e presença no quintal.","Acione TV → DESENHO → ENERGIA APAGADA → PRESENÇA para reconstruir a lembrança."};break;
                case 18:case 19:tips=new[]{"Retorno e contenção têm funções diferentes.","Compare registros humanos, ferimentos e leituras; a memória explica o acordo temporário.","Associe ferimentos à contenção, reagente ao circuito externo e registros à manutenção do selo até a travessia."};break;
                case 20:tips=s.continuation.manifestationDispelled
                    ?s.continuation.finalCalibrated?new[]{"O caminho seguro foi preparado.","A calibração está confirmada.","Siga pela saída para abrir a passagem de retorno."}:new[]{"A dissipação permite preparar o retorno.","No mecanismo, mantenha o selo de Edelzio ativo e ajuste as referências 3, 1, 2.","Use reguladores ÁRVORE 1, RIO 0, CAPELA 1; mantenha o selo ATIVO e valide o retorno seguro."}
                    :new[]{"Os avisos revelam a preparação dos ataques.","Prepare ÁRVORE, RIO e CAPELA; equipe três alunos e um apoio na mochila.","Saia da área de aviso e ataque na recuperação. Respeite as recargas; FÁCIL nas configurações reduz a resistência e o dano dos inimigos."};break;
                default:tips=s.continuation.area==1?new[]{"A história ainda precisa ser registrada.","Renan e o caderno guardam o fechamento da investigação.","Converse com Renan, abra o caderno e volte ao Fusca; avance a fala final para os créditos."}
                    :new[]{"A ligação precisa durar até a travessia terminar.","Abra a passagem, aguarde a criatura e só depois encerre o selo.",s.continuation.finalStep==0?"Abra o retorno no mecanismo. Vitória e calibração são obrigatórias.":s.continuation.finalStep==1?"Acione AGUARDAR A TRAVESSIA e espere a criatura atravessar.":s.continuation.finalStep==2?"A criatura atravessou. Agora acione ENCERRAR O SELO.":s.continuation.finalStep==3?"Aguarde o fechamento terminar; a Industrial permanece bloqueada durante a sequência.":"O acordo terminou. Use a saída para voltar à Industrial."};break;
            }
            return tips[Mathf.Clamp(level,0,2)];
        }
        public static void Draw(CampaignStory story,int id,ref int level)
        {
            ExperimentGUI.Label(new Rect(145,250,980,55),level==2?"SOLUÇÃO • SPOILER":"DICA "+(level+1)+" DE 3",true);
            ExperimentGUI.Label(new Rect(145,325,980,160),Get(story,id,level));
            if(level<2&&ExperimentGUI.Button(new Rect(650,510,460,45),level==1?"MOSTRAR A SOLUÇÃO":"QUERO MAIS UMA DICA"))level++;
            if(level>0&&ExperimentGUI.Button(new Rect(145,510,380,45),"VOLTAR À DICA ANTERIOR"))level--;
        }
    }
}
