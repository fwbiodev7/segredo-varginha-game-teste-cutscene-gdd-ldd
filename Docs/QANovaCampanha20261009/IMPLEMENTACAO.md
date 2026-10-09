# Campanha — implementação de 09/10/2026

Este registro descreve o comportamento presente no código e nos layouts. As execuções de testes e a build alpha0.1 precisam ser conferidas nos relatórios de validação que acompanham a entrega. Um arquivo de progresso com `status: running`, uma captura de tela ou o recebimento de um comando não comprova que uma suíte terminou com sucesso.

## Puzzles e investigação

| Fase | Ação do jogador | Condição para concluir |
| --- | --- | --- |
| 6 — Fragmentos | Escolher um marco, colocá-lo em um lugar do mapa e girar sua seta. Livro, globo e painel registram as pistas no caderno. | Ler os três documentos, posicionar ÁRVORE → RIO → CAPELA e orientar as setas LESTE → LESTE → NORTE. A posição correta com setas erradas continua incompleta. |
| 11 — Arquivos Alterados | Comparar uma planta preservada com a cópia alterada e marcar detalhes em uma lista com seis opções. | Marcar somente ACESSO LATERAL e DATA DO REGISTRO. Marcar um detalhe preservado, como porta principal, impede a conclusão. Conversar com Renan e examinar as três versões continuam fazendo parte da investigação. |
| 13 — Registros de 1898 | Associar provas a causas e consequências: descoberta, experiências, ruptura e contenção. | Página → descoberta; fotografia → experiências; danos da sala → ruptura; instrumentos ligados a pessoas → contenção. Uma prova pode ocupar somente uma associação; atribuí-la a outro acontecimento libera a ligação anterior. |
| 14 — O Que Fábio Escondeu | Associar os documentos às datas do Tombo. | Instrumentos ligados a pessoas vivas → 1898; transferência da ligação para Edelzio → 1996; interferência e falha do vínculo → 2026. É necessário apresentar os registros a Fábio e examinar as três evidências. |
| 15 — A Noite Interrompida | Acionar os quatro vestígios para reconstruir a lembrança. Os controles usam os sprites existentes da casa. | TV → desenho → energia apagada → presença no quintal. Cada vestígio entra uma vez na sequência; RECOMEÇAR LEMBRANÇA permite refazê-la. |
| 18 — A Criatura e o Acordo | Sustentar três conclusões com provas distintas. | Ferimentos → a contenção feriu a criatura; reagente → circuito externo abre o retorno; registros → o selo permanece até a travessia. A conclusão também registra a parte do acordo que já foi unificada nesta fase. |

Os puzzles mantêm o botão de conferência e oferecem mensagens específicas para respostas incorretas. As listas de provas e os vestígios visuais aparecem em ordem diferente da resposta. Os controles passam pelo mesmo adaptador IMGUI de mouse, teclado e controle usado no restante da campanha.

As etapas já simplificadas do Tombo inicial, da comparação com Ouzana e da oficina continuam com seu fluxo existente: combinação 1996 / ÂNCORA / REGISTRO 23; apresentação das provas a Ouzana; estacionamento do Fusca e uma aplicação de reagente para registrar as três marcas. A oficina continua permitindo repor reagente e partir após a análise.

## Orientação, saídas e casarão

`CampaignGuidance` calcula o próximo objetivo a partir das evidências e tarefas concluídas. O HUD alterna entre procurar uma prova, resolver o puzzle e seguir para o destino. Nas configurações que habilitam dicas de interação, um marcador identifica o próximo objeto; quando ele está fora da região central da tela, o marcador indica esquerda, direita, acima ou abaixo.

As saídas e os botões de continuação nomeiam o destino, como MATA, CASA DE OUZANA, OFICINA, CASARÃO ou PORÃO. A conclusão da investigação na Industrial orienta o jogador a conversar com Renan para chegar à biblioteca. Os atalhos do caderno de Fragmentos agora marcam o documento escolhido para orientar a caminhada.

O térreo do casarão usa o layout 112. O hall central dá acesso à sala de estar à esquerda e ao escritório à direita. O escritório contém a chave; o corredor de serviço, também à direita, leva ao acesso do porão. A planta, a revelação com reagente e a chave continuam sendo os requisitos da passagem. As paredes nos batentes têm limites explícitos para manter as aberturas físicas consistentes com o desenho.

O HUD identifica Hall central, Sala de estar, Escritório e Corredor de serviço. No porão, layout 13, identifica Arquivo das experiências, Área de análise, Sala dos instrumentos e Acesso pela escada. Uma zona menor, como o corredor dentro do escritório, tem preferência na escolha do nome exibido.

No porão, o ponto de saída que abre a continuação para Fábio fica separado do ponto opcional ESCADA • VOLTAR AO TÉRREO. O retorno ao térreo foi deslocado 1,65 unidade para baixo em relação à entrada da fase 13, permitindo escolher cada interação ao se aproximar dela e manter os registros concluídos quando se volta ao casarão.

Os nomes dos cômodos são zonas de informação, não novas superfícies físicas: `CampaignIllustratedMaps` os acrescenta ao plano e preserva a construção a partir da imagem ilustrada. A circulação depende das paredes, dos móveis e das formas de contato do mesmo layout.

## Progresso salvo e compatibilidade

`CampaignStory2026.json` recebe `puzzles` com as marcações da comparação, associações de causas, associações de datas, sequência de memória e associações da criatura. O estado de expansão recebe `mapDirections`. Posicionar/girar marcos, alterar marcações, associar provas, limpar associações e acrescentar/recomeçar a memória chamam o salvamento. Abrir um puzzle limpa a seleção de prova em andamento, mantendo as respostas já colocadas.

Ao carregar, `CampaignStory.Repair` inicializa campos ausentes e repara respostas inválidas. Marcações ficam limitadas às seis opções. Associações com valores fora da faixa ou provas repetidas são liberadas, preservando ligações válidas; uma memória inválida volta a ficar vazia. Direções ficam na faixa NORTE, LESTE, SUL e OESTE.

Saves antigos com investigações concluídas mantêm seus indicadores de conclusão. As respostas novas correspondentes são preenchidas na recuperação, e um mapa já concluído recebe as direções corretas. Saves antigos ainda em andamento conservam a colocação válida do mapa e recebem respostas editáveis para os puzzles novos. Os testes de compatibilidade usam JSON anterior sem os campos novos e também JSON com respostas inválidas.

## Correções anteriores mantidas nesta entrega

O registro detalhado está em `../QAFeedback20261009/IMPLEMENTACAO.md` e no inventário `../QAFeedback20261009/CollidersManuais.json`.

- Áudio: navegação vertical pelas quatro linhas de volume, alteração horizontal vinculada ao slider selecionado, seleção por mouse e persistência das configurações.
- Rádio na transição casa → Industrial: frase apresentada progressivamente, estática e interferência de 0,65 s no relógio da frase, clarão suavizado, legendas por cima do efeito, opção Reduzir distorções e pausa que congela a sequência e suspende o áudio.
- Contato com o cenário: 38 polígonos desenhados para os apoios de cadeiras, vasos, troncos e bases curvas, além de 63 paredes com limites explícitos no inventário anterior. O Fusca conserva BoxCollider2D para estacionamento. Esse número descreve a etapa anterior; o arquivo de layouts também recebe ajustes de circulação do casarão nesta etapa.

## Arquivos e cenas afetados

Os comportamentos novos ficam principalmente em `CampaignPuzzleDesign.cs` e `CampaignGuidance.cs`. A integração modifica `CampaignExpansionController.cs`, `CampaignContinuationController.cs`, `CampaignExpansionState.cs`, `CampaignStory.cs`, `CampaignContinuationDefinition.cs`, `CampaignHints.cs`, `CampaignJournal.cs`, `CampaignSequence.cs` e `VarginhaCampaignStage.cs`. O carregamento de zonas e paredes usa `CampaignIllustratedMaps.cs`, `CampaignMapPlan.cs` e `Assets/Resources/Varginha/IllustratedMaps/Layouts.json`.

As correções de áudio e rádio envolvem `VarginhaGamepadUI.cs`, `VarginhaGameSettings.cs`, `VarginhaCampaignStage.cs` e `CampaignSoundscape.cs`. A memória visual reutiliza TV e mesa de `CampaignInteriorArt` e os motivos existentes de lâmpada e árvore de `CampaignVisualAssets`, sem acrescentar imagem raster para os controles.

As cenas consumidoras dos puzzles são a cena de Fragmentos (fase 6), `Ato4_Fase11_Arquivos_Alterados`, `Ato4_Fase13_Registros_de_1898`, `Ato5_Fase14_O_Que_Fabio_Escondeu`, `Ato5_Fase15_A_Noite_Interrompida` e `Ato6_Fase18_A_Criatura_Ferida`. A circulação do casarão usa `Ato4_Fase12_Casarao_de_Ze_Gomes_Area1` (layout 112), sua chegada exterior na fase 12 e o porão na fase 13. Os objetivos e rótulos de saída também afetam outras cenas da campanha ao carregar os controladores comuns.

## Cobertura de regressão a conferir

| Suíte | Evidência que procura |
| --- | --- |
| `CampaignPuzzleDesignTests` | Respostas certas e erradas, direções do mapa, progresso parcial, saves anteriores concluídos/incompletos, dados inválidos, alvos de orientação e rotas do casarão. |
| `CampaignPuzzleDesignPlayTests` — quatro testes | Colocação e rotação do mapa com controle; resolução das cinco interfaces com controle e releitura de uma associação parcial após recarregar; caminhada física pelo hall, sala, escritório e corredor e acesso ao porão; saída do porão e retorno opcional ao térreo como interações separadas, conservando a investigação concluída. |
| `CampaignFeedbackPlayTests` | Quatro volumes por controle/analógico/teclado, arraste por mouse, rádio sincronizado/pausa/transição e contato físico com os polígonos. |
| `CampaignExpansionPlayTests`, `CampaignLabWorkshopAuditTests` | Passagens da biblioteca, evidências, Ouzana, estacionamento/análise/partida do Fusca e retomada de progresso. |
| `CampaignContinuationPlayTests` | Áreas de investigação, soluções, combate da manifestação, calibração, travessia, epílogo, mochila e aliados. |
| `CampaignRequestedChangesTests`, `CampaignControllerRemapTests` | Menus com controle, remapeamento, chegadas de Ouzana/casarão, assentos e conservação das referências visuais. |
| `CampaignChaptersTests`, `CampaignRemasterPlayTests` | Fluxo casa/Industrial, portas e paredes, dicas, pausa, transições e áudio ambiente. |
| EditMode completo e validação de build | Contratos estáticos de saves, mapas, arte, equipamentos e cenas; compilação do jogador e abertura do pacote alpha0.1. |

Os resultados existentes no momento deste registro são `EditMode_Final.json` com filtro Campaign (169 testes aprovados) e `Puzzles_PlayMode_Passed.json` (os três primeiros testes da suíte nova aprovados). O quarto teste, `BasementStairsKeepTheNextChapterSeparateFromReturningUpstairs`, foi acrescentado depois desse resultado 3/3 e depende de sua execução posterior. A regressão ampla em PlayMode e a build final devem ter resultados próprios antes de serem consideradas validadas.

O relatório antigo `Regression_Previous.json` contém cinco falhas e deve permanecer como diagnóstico histórico. A entrega final deve citar os resultados posteriores das mesmas áreas. O arquivo `PlayMode_FirstRun.json` desta pasta registra a recusa do transporte síncrono do Unity para PlayMode e executou zero testes; ele não é um resultado de aprovação.
