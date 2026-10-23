# Revisão do roteiro do Edelzio em 23 de outubro de 2026

O PDF de Downloads é idêntico ao PDF de 22/10 mantido no projeto. As 78 entradas foram conferidas contra o texto extraído das dez páginas e indexadas individualmente. A história geral acompanha a campanha atual, mas o documento não é uma lista de 78 falas já prontas para integração.

## Problemas corrigidos na entrega para o ator

- O roteiro antigo mistura 15 entradas marcadas E, 58 marcadas P e cinco esforços vocais. Uma entrada E é inválida: `exitclue A observação ficou registrada no caderno.`. O extrator incluiu a string de uma comparação condicional como se fosse uma alternativa de fala. Essa entrada foi excluída da gravação, sem substituir a fala válida da porta.
- As 13 falas adultas com canal de diálogo atual e a frase infantil do rádio foram preservadas literalmente, incluindo pontuação, caixa e os complementos que participam do hash do áudio.
- A antiga proposta `Vou deixar a chave na mesa do café. Depois pego a mochila.` já consta da cutscene `ANTES DE SAIR`. Ela foi colocada no roteiro principal como ED03. O texto existe, mas `CampaignTimedSubtitle` usa owner nulo e não liga sua voz automaticamente. O mapa técnico registra essa pendência, sem declarar integração concluída.
- As outras 57 entradas de propostas foram preservadas em um anexo separado para revisão. Algumas reproduzem ou dramatizam textos já visíveis em leituras. Isso não as transforma em chamadas de voz atribuídas ao personagem. Não pedir ao ator que decida entre as versões equivalentes.
- Os cinco esforços de voz ficam opcionais e separados das falas. O efeito `EdelzioFear` recebido em 22/10 não substitui dublagem textual, mas deve ser ouvido antes de pedir esforços redundantes.
- O ator recebe números curtos, uma orientação breve e o texto em destaque. O CSV mantém a correspondência com os IDs originais. Uma tomada boa basta; gravações extras são opcionais.

## Continuidade verificada

A chave usa a mesa do café e os fragmentos mantêm as bifurcações aprovadas. O horário continua 23:23. A frase infantil `Não deixa ela sair.` pertence ao rádio do Fusca e à lembrança correspondente, sem antecipação na abertura. Os anos de referência são 1898, 1996 e 2026. O roteiro de propostas mantém a travessia antes de encerrar o selo; não transforma a criatura ferida em alvo de extermínio nem acrescenta finais não implementados. Os documentos GDD/LDD têm divergências já registradas, portanto a implementação atual orientou esta conferência.

## Entrega

- Principal: `output/pdf/ROTEIRO_EDELZIO_SIMPLES_23-10-2026.pdf`. Quatro páginas, 14 falas adultas e uma infantil, com cinco esforços opcionais na última página.
- Texto para celular: `output/dublagem-2026-10-23/ROTEIRO_EDELZIO_SIMPLES_23-10-2026.txt`.
- Pacote para enviar ao ator: `output/dublagem-2026-10-23/PARA_ENVIAR_AO_EDELZIO_23-10-2026.zip`, contendo somente o PDF principal e o TXT.
- Anexo de revisão da equipe: `output/pdf/EDELZIO_PROPOSTAS_PARA_REVISAO_23-10-2026.pdf`. 9 páginas com as 57 propostas preservadas.
- Mapa de integração: `MAPA_TECNICO_AUDIOS.csv` e `mapa_de_audios.json` nesta pasta. Não enviar os códigos longos ao ator como instrução de gravação.

## Fontes e limites

Fontes: PDF fornecido; `Docs/CampanhaOficial/Dublagem_2026-10-22/roteiro_gravacao.json`; `Assets/Scripts/Game/Varginha/Experiment/CampaignHouseSequence.cs`; `VarginhaCampaignStage.cs`; `CampaignFuscaCinema.cs`; `CampaignTimedSubtitle.cs`; `CampaignMessagePresentation.cs`; `CampaignContinuationController.cs`; `CampaignContinuationDefinition.cs`; GDD/LDD em `Docs/CampanhaOficial/Referencias/`; `PUZZLE_FRAGMENTOS_BIFURCACOES_2026-10-21.md`; créditos de áudio de 22/10.

O prefixo `Edelzio:` no texto de sucesso da criatura ferida aparece em mensagem de leitura da investigação, não numa chamada Speak. O epílogo também mistura narração e frase do personagem em leitura. Essas propostas não foram anunciadas como áudio automaticamente utilizável. ED12 e ED01 soam como orientação, mas o código atual as atribui a Edelzio no canal de diálogo; foram preservadas para evitar mudar o jogo nesta tarefa.

Nenhuma fala, mecânica, arte ou cena do jogo foi alterada. O PDF original em Downloads foi preservado. O extrator geral e os roteiros completos de 22/10 também foram preservados; esta revisão documenta o erro e gera uma seleção rastreável sem reescrever a documentação do elenco inteiro.

## Verificação final

A geração e as verificações de conteúdo terminaram com código 0. As 78 entradas originais estão contabilizadas: 15 falas principais, 57 propostas preservadas, cinco esforços opcionais e uma entrada inválida excluída. Os IDs das falas foram recalculados e comparados com os originais. Todos os textos constam dos PDFs correspondentes, os números curtos são únicos e o ZIP passou na verificação CRC. Os dois PDFs foram renderizados com Poppler e as 13 páginas foram inspecionadas individualmente, sem cortes, sobreposições, caracteres ausentes ou páginas vazias. Resultados em `qa-conteudo.json` e `qa-final.json`. Nenhum teste ou build do jogo foi executado.
