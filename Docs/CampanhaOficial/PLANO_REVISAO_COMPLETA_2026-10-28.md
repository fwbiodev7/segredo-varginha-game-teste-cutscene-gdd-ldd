# Plano da revisão completa — Oizlede: Fragmento Perdido

Estado: plano completo aprovado pelo usuário em 28/10/2026 (“aprovado” nesta conversa). A aprovação abrange os ajustes e testes necessários para executar o escopo completo abaixo, sem novo pedido a cada etapa.

Fonte: prompt integral em `output/revisao-completa-2026-10-28/PROMPT_ORIGINAL.md`, 30 capturas do jogo, referência arquitetônica de igreja e áudio recebidos. O prompt detalha 23 solicitações e complementos numerados até 36. As instruções do anexo são tratadas como escopo e critérios de aceitação, preservando a exigência de aprovação do AGENTS.md. O GDD localizado tem nome `GDD.md`, não literalmente `GDD(1).md`; sua referência histórica será cruzada com a campanha atual de 14 capítulos/19 cenas.

## Problemas confirmados na inspeção

- `CampaignPuzzleDesign.CardButton`: cartões de 39 unidades; título em y+16..34 e detalhe em y+27..38, com sobreposição de 7 unidades. Corrigir diagramação comum, não apenas reduzir fontes. Preservar as soluções históricas de causas/datas.
- `CampaignHouseInvestigation`: `PlaceKey` precede o lembrete da mochila e o objetivo leva à porta/mesa, contrariando a nova primeira ida ao Fusca. Reorganizar o estado existente e sua migração, em vez de acrescentar flags independentes.
- `CampaignHouseSequence`: papéis/fragmentos são criados sem uma condição visual vinculada ao desaparecimento; o embarque ainda chama `FuscaDoorMotion`. Adaptar os pontos existentes, ocultação e fluxo de entrada.
- `CampaignGuidance.DrawMarker`: objetivo usa um retângulo ciano de 6×6. Substituir nessa origem comum por ! amarelo contornado ~15×15, sem mexer no marcador do jogador nem em triggers.
- `CampaignSchoolSequence` / `CampaignSchoolInvestigation`: a interface e as regras de Comparar/Concluir ainda correspondem ao redesign anterior. O novo prompt explicitamente pede escolha única baseada em memória e restrições de leitura. Manter IDs independentes da ordem visual.
- `CampaignPuzzleDesign.Comparison`: alternativas usam índice visual fixo como bit de seleção. Acrescentar ordem de tentativa persistente sem alterar IDs/resposta.
- `VarginhaMainMenu`: frases com fonte 8 e alpha 0,85 explicam a baixa visibilidade. Aumentar para aproximadamente 10, cinza frio com maior contraste; preservar rotação. Retirar CASO VARGINHA somente da região exibida por UV, sem recriar/substituir o PNG. Conservar dessaturação, fundo, som e controles.
- Colisões: o controlador usa Rigidbody2D/velocidade; há também movimento roteirizado e configuração de corpo/pés/layers. Ainda NÃO está confirmada a causa raiz de todas as travessias relatadas. A auditoria precisa exercitar o jogador real e o Fusca, incluindo saída cinematográfica que desativa simulação. Os testes antigos de probes não comprovam os casos agora mostrados.

## Ordem de implementação proposta

1. **Checkpoint e estabilidade:** preservar baseline local/rastreado/não rastreado, mapear triggers e colisões por ambiente, reproduzir biblioteca/igreja/calçada/oficina e corrigir as causas comuns.
2. **Progressão:** sequência explícita chave → tentativa no Fusca → retorno/mochila/chave na mesa → desaparecimento → papéis/investigação → saída; embarque instantâneo sem porta; prólogo motivado pelo clarão; perseguição encerrada ao entrar na igreja, com Fábio no interior e save compatível.
3. **Puzzles:** diagramação legível de Fábio/1898; escola em quatro etapas com novas regras de Comparar e Concluir; leitura estável por ID; Tombo com pistas claras sem mudar campos/respostas; Arquivos Alterados embaralhado por tentativa com estado persistente.
4. **Arte investigativa com subagentes:** biblioteca seletiva, três vestígios reais na mata, igreja externa antiga preservada, objetos reconhecíveis na igreja/casarão/porão e chave física distinta da chave do Fusca. Reutilizar atlas, contornos, pivôs, escala e profundidade aprovados; gerar raster apenas onde faltar arte adequada.
5. **Menu e audiovisual:** frases mais perceptíveis em cinza; logo sem o rodapé CASO VARGINHA; ! amarelo de objetivos; veículo musical na rua externa da escola durante chegada, com fonte associada ao carro, atenuação, panorama, Doppler suave, pausa/volumes e limpeza.
6. **Integração e entrega:** EditMode, Player, resoluções, save/reload e percurso integrado da campanha. Produzir nova candidata com `Oizlede - Segredo de Varginha.exe`, pasta de dados correspondente, ZIP/CRC/hashes e relatório por item. Preservar identidade interna de saves e builds anteriores. Não anunciar campanha completa com base apenas em cenas isoladas.

## Inspeção artística pelos subagentes

`artes_cenarios_auditoria` e `artes_ui_auditoria` concluíram somente leitura. Nenhum arquivo alterado, bitmap gerado ou teste/build executado por eles.

- **Biblioteca (6):** `IllustratedMaps/IndustrialLibrary.png`, `Layouts.json` e `SchoolGlobe/Globe` fornecem base. Destacar somente os pontos ativos `archive`, `school`, `square`, `map`, `truth0`, `truth1`, reaproveitando documentos/livros e evitando brilho em decoração irrelevante.
- **Mata (7):** `Forest.png`, 1536×1024/24×16 unidades, com `sign0=[249,578]`, `sign1=[833,546]`, `sign2=[1130,417]`. Propor três grupos distintos de vestígios, coerentes com árvore/solo-rio/capela. Não espalhar indícios sem função.
- **Igreja externa:** nova peça localizada adaptada à câmera top-down, com cobertura íntegra, cruz, porta e vitrais, preservando floresta/ponte/rota. Reutilizar identidade de `IllustratedMaps/Church.png`, `ChurchArt/ChurchFurniture.png`, `ChurchArt/Atlas.json` e `WorldArtV3/Church.png`. Não inserir diretamente o JPEG isométrico. Reutilizar Fábio interno e eliminar duplicação externa conforme a nova sequência.
- **Tombo (8):** materializar `index`, `record`, `symbol` em suportes visíveis. Solução atual identificada: 1996, ÂNCORA e registro 23; não confundir o ano do registro de Edelzio com a origem histórica de 1898.
- **Casarão/porão (12,112,13):** reaproveitar `CasaraoExteriorBase.png`, `CasaraoInteriorBase.png`, `CasaraoEsconderijoBase.png` e `CasaraoProps.png`. Chave `key=[1082,655]`: sprite reconhecível sobre gaveteiro. Materiais `materials=[945,624]`: alinhar representação sobre o suporte; não mover aproximação para dentro do collider da caixa `[912,475,79,102]`.
- **UI:** nenhum bitmap novo necessário para cartões, frases, recorte da logo ou exclamação. Usar fonte e painéis existentes, com estilos próprios de cartões; não mudar globalmente o autoajuste de todos os labels.

## Escopo verificável por item

Primeiro lote implementado: itens 6–9 parcialmente (casa, papéis e embarque; 24/24 EditMode); itens 1–3 e 24–25 parcialmente (UI implementada, aguardando Player); item 17 com arte da igreja pronta, ainda sem integração. Os demais continuam não iniciados. A tabela abaixo define escopo/validação, não declara conclusão. Estado detalhado em `output/revisao-completa-2026-10-28/RETOMADA.md`.

| Itens do prompt | Trabalho e integração principal | Validação necessária |
| --- | --- | --- |
| 1–2 | `CampaignPuzzleDesign` e painel do `CampaignContinuationController`: diagramação/associação/textos | Sem vínculos, erro, correção, desfazer, solução e resoluções |
| 3 | `CampaignGuidance`: ! de objetivo ativo | Disponibilidade, conclusão, sem duplicatas/mapa do jogador intacto |
| 4 | `VarginhaCampaignPhase1` e sequência audiovisual existente | Luz/áudio graduais, fala, movimento livre, disparo único/save |
| 5,15 | Controlador, `CampaignWallBody`, `CampaignMapConstruction`, `CampaignIllustratedMaps`, layers/rotas | Inventário por mapa; jogador real andando/correndo/esquivando, reload/transições |
| 6–9 | `CampaignHouseInvestigation`, `CampaignHouseSequence`, estágio/save | Nova ordem, papéis condicionais, acesso ao Fusca e embarque sem porta |
| 10 | Chegada escolar em `CampaignFuscaCinema`/`CampaignHouseSequence`, catálogo/AudioSource do veículo | Trajetória livre, volume aproximação/pico/afastamento, pausa/skip/limpeza e captura |
| 11–13 | `CampaignSchoolInvestigation`, `CampaignSchoolSequence`, caderno/save | IDs de leitura, novas regras, bloqueios de consulta, erro/acerto/reload |
| 14,16 | Overlays e objetos vinculados a pontos reais de biblioteca/mata | Arte/sorting/escala, acesso e progresso no Player |
| 17 | Igreja externa localizada, `CampaignForestPresence`/expansão/transição | Fuga completa, entrada segura, captura/retry, Fábio único e continuidade |
| 18 | `CampaignBookPuzzle`, objetos index/record/symbol e estado da expansão | Explorar/coletar/associar/conferir/recarregar sem alterar solução |
| 19 | `CampaignWorkshopVehicle`, `CampaignVehiclePhysics`, rotas/colliders | Direção/curvas/saída/reinício contra obstáculos reais |
| 20 | Ordem visual em `CampaignPuzzleProgress`/`CampaignPuzzleDesign` | Várias tentativas, IDs/seleção, persistência e solução independente da posição |
| 21–23 | Objetos de casarão/porão e chave por ID próprio | Visibilidade, suporte, trigger correspondente, sem acesso por paredes, coleta/save |
| 24–26 | Menu/rotação/shader; `VarginhaDailyBuild` e empacotador | Legibilidade, rotação/recorte, executável renomeado abrindo fora do Editor |
| 27–30 | Reutilizar arquitetura/dados/evidências, recursos Point, lifecycle/performance | Sem gestores duplicados, objetos/fontes/colliders/áudios vazando ou recriados por frame |
| 31–34 | Testes reais e campanha integrada, resoluções 1280×720/1920×1080 e janela configurada | Evidências atuais, incluindo falhas e limites; não confundir mocks/probes com percurso |
| 35–36 | Nova build/ZIP e tabela de resultados por solicitação | Relatório honesto com arquivos/cenas, testes, evidências, estados e pendências |

## Continuidade e economia

Checkpoint e política de 15%: `output/revisao-completa-2026-10-28/RETOMADA.md`. Automação de retomada já criada para 28/10 às 11:57:43 de São Paulo, um minuto após a renovação inicialmente informada pela conta. Consultar novamente o saldo entre lotes; ao atingir 15% ou menos, preservar estado e interromper novos lotes pesados. Usar wrapper para comandos extensos, aproveitar resultados relevantes e delegar artes em escopos separados.

Não houve edição de jogo, cena, arte ou áudio, nem execução de novos testes nesta rodada de inspeção. Só foram criados registros de continuidade/plano e o agendamento pedido pelo usuário.
