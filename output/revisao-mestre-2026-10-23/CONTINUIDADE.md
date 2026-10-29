# Continuidade dos serviços de Varginha

Atualização: 23/10/2026, horário de Brasília.

## Pedido vigente

Implementar as 15 correções do prompt mestre anexado em `C:/Users/Usuario/.codex/attachments/167b3bf0-7f9f-44ca-9b24-e37b2fa2edce/Texto colado.txt`. O usuário pediu continuidade entre renovações dos limites e retomada automática às 11h59. Acrescentou a redução acentuada do relatório ABNT, mantendo utilidade e apresentação profissional. Esses pedidos não substituem uns aos outros.

## Retomada automática

- Automação heartbeat neste mesmo chat: `retomar-revis-o-de-varginha`.
- Horário solicitado: 11h59, America/Sao_Paulo.
- Em 23/10 o limite de cinco horas estava 93% consumido; reset informado pela ferramenta: 23/10/2026 às 11h59min34s de Brasília. Os limites são compartilhados pela conta e devem ser consultados novamente, não inferidos deste registro.
- Não há créditos de reset disponíveis. Nenhum reset foi solicitado ou consumido.
- Ao atingir o limite, conservar este checkpoint e o estado dos arquivos. Não presumir que um processo interrompido terminou com sucesso.

## Integridade e autorização

O último commit é a2946be, de 22/10. Há alterações locais preexistentes de 23/10 em código, metadados, testes e documentos. Não executar reset, checkout destrutivo, limpeza de untracked ou restauração indiscriminada. O snapshot dos arquivos já modificados está em baseline.json.

AGENTS.md exige inspeção, plano resumido e aprovação antes de uma nova alteração no jogo. O diagnóstico e o plano estão em PENDENCIAS.md. **APROVAÇÃO RECEBIDA em 23/10/2026:** o usuário respondeu à pergunta específica do plano completo: “Sim, implementar o plano completo”. As 15 correções, seus ajustes e testes necessários estão autorizados. Não solicitar nova aprovação a cada etapa. A redução do relatório também foi solicitada expressamente. Nenhum arquivo de jogo foi alterado para este prompt mestre até este registro. A retomada automática continua agendada para 11h59, conforme pedido do usuário.

## Diagnóstico já realizado

- Lidos integralmente o prompt mestre e AGENTS.md; inspecionados o status e o diff preexistentes.
- `CampaignWorkshopVehicle` usa quatro vistas cardinais, troca instantânea de orientação e dimensões do collider; impede a troca se a nova caixa se sobrepõe a algo. Essa lógica é candidata ao travamento de manobra, ainda sem reprodução no Unity.
- `CampaignTrailPuzzle` exige prévia e TRAÇAR TRECHO, habilita somente a decisão corrente e apaga escolhas a partir do primeiro erro. Confirma o comportamento que o usuário quer substituir.
- `CampaignHouseInvestigation` já possui passos e trava a observação até pegar a mochila, porém a ordem começa colocando a chave na mesa e não contém a lembrança obrigatória na porta. Dedução fixa usa três pistas e conclusão por índice. Revisar runtime e saves em conjunto.
- `VarginhaPlayerAttack` já tem bloqueio contra reinício durante um golpe, uma entrada pendente e descarte em interrupções. Não apresentar isso como implementação nova nem reescrever sem reproduzir o defeito. É preciso verificar interação com animação e testes de spam.
- `VarginhaWorldDepth` já ordena por contato com o chão e SortingGroup. A auditoria deve corrigir footprints, pivôs, suportes e segmentos de móveis, em vez de trocar camadas globalmente.
- `CampaignFuscaCinema` possui overlay de escurecimento dependente do tempo. A causa da sombra residual ainda não foi confirmada; investigar estados e restauração no fluxo completo.
- O relatório ABNT original tinha 46 páginas. A revisão posterior baseada no modelo tem 35 páginas e cinco seções principais, mas o usuário continua considerando extenso. Não confundir as versões.

## Fontes e ferramentas

- GDD/LDD: `Docs/CampanhaOficial/Referencias/GDD_Usuario.md`, `O_Segredo_de_Varginha_LDD.md`; campanha atual em `Docs/CampanhaOficial/CAMPANHA_14_FASES.md` e `CampaignSequence.cs`.
- Candidata anterior à revisão mestre: build local 0.4.8-beta, de 23/10. Seus testes anteriores não aprovam novas mudanças.
- Scripts de QA existentes: `Tools/QA/run_dialogue_feedback.ps1` e builders em `Assets/Scripts/Editor/Testing`.
- Node: `C:/Users/Usuario/.cache/codex-runtimes/codex-primary-runtime/dependencies/node/bin/node.exe`.
- Python: `C:/Users/Usuario/.cache/codex-runtimes/codex-primary-runtime/dependencies/python/python.exe`.
- Usar `node Tools/AgentOutput/compact.mjs --question "resultado necessário" -- executável argumentos` para testes/builds extensos. Inspecionar logs dirigidos se falhar. Zero casos não é aprovação.
- Para editar o globo, ler a skill pixel-art-sprite-sheet-master antes de qualquer modificação de imagem; preservar atlas aprovados. Não foram fornecidos novos prints nesta mensagem; existem capturas históricas em output e Docs. Não afirmar correspondência com prints ausentes.

## Redução do relatório

Versão vigente: `output/relatorio-modelo-abtn-2026-10-23/relatorio_desenvolvimento_varginha_modelo_abtn.pdf` e `.docx`, 35 páginas. Fonte de autoria: `conteudo.md`, `gerar_relatorio.py`, `exportar_word.ps1` e `renderizar_conferir.py` nesse diretório.

Meta de trabalho: aproximadamente 10–12 páginas totais, ajustando se a legibilidade exigir, com as cinco seções do modelo, narrativa cronológica concisa, sistemas agrupados, resultados verificados, pendências e referências essenciais. Manter Times New Roman 12, entrelinhas 1,5 e títulos 16 centralizados; reduzir texto e repetição, não a fonte. Retirar capturas redundantes e tabelas extensas; preservar distinção entre planejado, implementado e testado. Não atualizar o recorte factual do relatório sem nova verificação. Executar exportação Word e renderização de todas as páginas antes de entregar PDF e DOCX compactos.

## Próximo passo

1. Retomar às 11h59 e conferir a disponibilidade dos limites; a aprovação já foi recebida e não é bloqueio.
2. Continuar o diagnóstico e reduzir o relatório.
3. Implementar o plano aprovado por dependência, começando por progressão/save, veículo/colisões e ataque; depois puzzles, cinema, interface/áudio e regressão.
4. Atualizar PENDENCIAS.md e este checkpoint a cada entrega parcial validada.

## Retomada efetiva após o reset e pedido de novos sons

Os limites estavam renovados (0% consumido na janela de cinco horas). O usuário acrescentou musicamenuinicialslowed.mp3 e igrejamedo.mp3 de Downloads, autorizando integração e continuidade. Foram adicionados os itens 17 e 18. Este registro posterior substitui os trechos antigos que afirmavam não haver implementação.

Implementado no workspace até agora:

- Catálogo Received20261023, MP3 copiados sem alterar bytes; MenuCinematicAtmosphere usa MenuMusic; CampaignRecordedAudio resolve ChurchAmbience em todas as bandas. Créditos de recebimento e hashes em Docs/CampanhaOficial/AUDIO_RECEBIDO_2026-10-23.md.
- Trilhas: seleção imediata de qualquer trecho, contador e conferência única; erro mantém todas as escolhas. Repair do save deixa de apagar escolhas após lacunas. Testes de regra e controle atualizados.
- Casa: bagReminder serializado, lembrança única na saída, mochila condicionada a essa lembrança, saída física condicional no vão real do mapa. Saves já avançados não repetem a lembrança. Pista do papel e dedução usam rastro observável até a cama, três pedaços adicionais da arte existente. Fluxo completo ainda sob validação.
- Áudio das ações: FaceWash começa junto da pose; CoffeeDrink no primeiro contato da xícara com a boca. Fonte própria da ação aplica volume, pausa e interrupção; retiradas emissões tardias ao completar. Testes novos em execução.
- Ataque: reprodução controlada a 10 FPS falhou no código anterior por saltar poses. Correção deixa cada pose aguardar pelo menos um frame e sua duração, sem consumir seu tempo pelo atraso da pose anterior. Buffer preexistente preservado; regressão em execução.
- Relatório reduzido concluído: PDF/DOCX com 12 páginas em output/relatorio-resumido-abnt-2026-10-23. Cinco seções do modelo, todas as páginas renderizadas e abertas individualmente, sem cortes/campos pendentes. qa-final.json registra revisão. Longos preservados.

QA usa cópia isolada, caminho salvo em qa-project.txt, para preservar o Unity aberto do usuário e seus saves. Produto da cópia Varginha_Master_QA_20261023. Assets/Packages/ProjectSettings foram copiados, sem compartilhar Library. O runner de editor recebeu proteção para não iniciar testes em cena suja ou play ativo. Uma solicitação própria de inspect não atendida foi movida para inspect-request.json; nenhuma solicitação do usuário foi removida.

Resultados desta revisão: editmode.xml 26/26 aprovados (áudio/trilhas); house-editmode.xml 48/48 aprovados (inclui regras da casa). integration-playmode.xml 5/5 aprovados (menu, igreja, controle de trilhas, mochila e casa→escola). Não somar recortes repetidos como casos únicos. punch-before.xml tem 1 falha esperada que reproduz o problema anterior; manter como evidência, não chamar de aprovação. actions-playmode.xml ainda sendo produzido; conferir resultado e falhas antes de qualquer conclusão.

Próximo: terminar regressão de ações/chave/combate, corrigir falhas se houver e registrar estados reais; depois seguir itens restantes (01–06, 10–13). Não encerrar todo o serviço nem desativar a automação com itens em aberto. Nenhum build novo ou push feito. Para QA futura, atualizar somente os arquivos alterados na cópia quando o Unity batch anterior já tiver terminado.

## Pedido posterior para a apresentação

O usuário pediu abrir a apresentação atualizada. O HTML da daily de 23/10, output/daily-sprint-2026-10-20/A_Sprint_Final_Varginha.html, foi aberto no navegador. A aba está efetivamente visível em file:///.../A_Sprint_Final_Varginha.html#1. Essa daily mantém o recorte da revisão anterior; as correções desta rodada ainda não foram adicionadas aos slides.

Depois acrescentou: “quando terminar crie um roteiro de apresentação do projeto completo desde o início; a apresentação é dia 30/10/26”. Item 19 registrado. Produzir o roteiro ao finalizar a revisão e sua validação, com base no estado real final. Usar fala natural e marcações de slide/demo; contemplar concepção, primeiros registros, evolução, narrativa, tecnologia, sistemas, audiovisual, testes, dificuldades, resultados e continuidade. Não inferir que as entregas atuais já estavam presentes em versões antigas nem inventar fatos ocorridos em 30/10.

## Marco de regressão e fechamento parcial posterior

visual-actions-playmode.xml: 16/16, zero falhas/ignorados. Inclui novo ataque com CampaignTeamEdelzio real, áudio de higiene refinado para contato e café na boca, globo e cadeira. Globo inspecionado em world-review/globe-library.png: integração aprovada. Itens 03, 14 e 15 concluídos no recorte descrito. full-editmode.xml: 412/412 com gráficos, zero falhas/ignorados. Esse recorte foi feito ANTES da posterior extração de CampaignVehiclePhysics e da atualização da estrada/pista; repetir após essas mudanças.

A inspeção de office-approach/office-notebook-seated/office-return revelou pés antigos ainda no piso. Não considerar item 02 concluído. prepare-office.py agora remove toda a silhueta antiga e NÃO repinta a cadeira no piso: o prop original a desenha uma única vez. Nova textura hash cf7f04bf6f38e6625c85cbd84d89dee1eb2f06e272e5032f53cd020aca3e3ed2. Teste Player e nova inspeção em andamento.

Veículos: CampaignVehiclePhysics compartilha aceleração/frenagem e regra de contato separante entre oficina, pista opcional e estrada legada. Pista usa SetTrackControls e FixedUpdate, sem teleporte de gameplay em Update; preserva percurso reto. Estrada usa volante progressivo, yaw, aderência, aceleração/coast/ré e collider fixo rotativo. Novos testes em CampaignVehicleContactTests cobrem estrada no limite inferior e aceleração/coast/freio da pista. Ainda não aprovados; não declarar itens 12/13 completos.

QA atual: sessão de comando 88034, StandaloneWindows64 no projeto isolado, resultados polish-player.xml e log polish-player.log. Filtro inclui veículo, cinema Fusca completo, SchoolDesk, auditoria de atmosfera de todos os capítulos/subáreas e RequestedChanges. Não inclui CampaignPuzzleDesignPlayTests (necessário segundo recorte Player para Livro/trilhas/continuar). VARGINHA_CINEMA_QA aponta para este diretório. Os screenshots de atmosfera ficam em Docs/QAAtmosfera20261009 dentro da cópia isolada; copiar evidências ao fim. Testes em execução não são aprovação. A estrada recebeu depois do início do build uma proteção extra para zerar velocidade ao encostar em limite matemático; sincronizar esse único script antes do próximo recorte. Não tocar no Unity original aberto do usuário.

## Publicação de progresso solicitada e último ajuste visual

Usuário pediu: "esses recortes na parte do notebook tá feio e muito grotesco, deixe mais suave e simples ... encaixarem certinho no mapa". Prioridade visual do item 02: remover emendas retangulares do piso e simplificar contornos/encaixe de mesa, cadeira e personagem. Ainda NÃO resolvido visualmente. Não declarar o canto do notebook pronto. A textura atual removeu os pés antigos, mas ainda tem emenda visível. OfficeTorso adicionado em CampaignSeatingLayers e usado por CampaignTeamEdelzio para ocultar pernas da vista traseira, sem redesenhar pixels; precisa inspeção real após teste. Não iniciar nova arte sem preservar originais.

Usuário autorizou explicitamente commit E publicação no GitHub da atualização de hoje e pediu parar quando restarem 5% dos limites, salvando pendências. Publicar progresso com estado parcial, sem chamar de versão final. Manter arte/saves e alterações preexistentes. Não publicar arquivos de recovery/InitTestScene, caches, logs brutos, builds, backups ou URP runtime settings temporariamente esvaziados. O arquivo original de URP deve permanecer local, sem restauração indiscriminada.

polish-player.xml terminou: 14 casos, 13 aprovados e 1 falha de fixture. Estrada legada não pertence às 19 cenas da build; o teste tentou carregá-la por nome no Player e falhou antes de exercitar a física. Corrigido fixture para criar cena isolada e instanciar o controlador/mapa real, sem adicionar fase legada à campanha. Não interpretar como falha de física nem como aprovação da estrada. Os outros casos passaram: atmosfera dos 18 ambientes de gameplay, cinema completo, RequestedChanges, SchoolDesk e contato/pista opcional do veículo. Todos os 18 PNG Atmosfera em atmosphere-review foram abertos e inspecionados. Esse recorte cobre enquadramento/legibilidade nos spawns, NÃO auditoria de cada contato em todos os móveis. Quatro poses do motorista e school-arrival-gate também abertas; itens 05 e 06 concluídos sem redesenhar a arte alinhada do motorista.

QA corrente antes do commit: sessão 66724, PlayMode Editor, checkpoint-playmode.xml/log, 5 casos esperados: 3 CampaignVehicleContactTests + OfficeSeat + recuperação repetida de glitch. Inclui limite matemático da estrada, novo OfficeTorso, fixture corrigido e captura antes/pico/depois do glitch. Conferir resultado, não presumir. A pasta Docs/QAAtmosfera20261009 da cópia isolada guarda novas capturas de glitch. Livro/trilhas/continuar ainda precisam recorte Player CampaignPuzzleDesignPlayTests para revisão visual. Full EditMode 412/412 antecede alterações finais de veículos e OfficeTorso; repetir antes da candidata final.

Automação continua ativa; retomada deve ler este checkpoint. Próximo reset atual informado pela ferramenta: 23/10 às 17:00:49 Brasília. Não comprar créditos nem consumir resets. Roteiro completo de apresentação de 30/10 permanece item 19, a produzir após finalizar correções e validar, conforme pedido do usuário.

checkpoint-playmode.xml: 5 casos, 4 passaram; recuperação repetida de glitch, assento e contato/pista da oficina aprovados. Estrada reproduziu y=0 sem sair do limite. Diagnóstico adicional: spawn do mapa em x=690 pixels fica FORA da faixa [714,960]; clamp tentava atravessar o meio-fio. BuildRoad agora posiciona o carro no centro físico da faixa, mantendo os limites. Regressão road-checkpoint.xml em execução na sessão de comando retornada; conferir antes de declarar concluído. Nova imagem office-notebook-seated.png foi inspecionada: pernas agora ocultas atrás do encosto, melhor postura; a emenda retangular do piso permanece e é prioridade do usuário. Nenhuma nova geração de arte foi iniciada nesta etapa.

## Checkpoint para commit de 23/10

road-checkpoint.xml terminou com 1/1 falha: mesmo com spawn central, y ficou 0 ao acelerar após ré. Portanto a alteração do spawn NÃO resolveu o bloqueio da estrada. Próximo diagnóstico: registrar state/title/input/Speed e contatos do BoxCast na posição inicial; distinguir ausência de input/estado e colisão. Não reduzir a asserção nem declarar item 12/13 completo. Oficina e pista continuam aprovadas nos recortes anteriores. Não há teste em execução a manter no Unity do usuário.

Publicação solicitada é checkpoint parcial. Entregas finais ainda pendentes: acabamento simples e sem emendas no canto do notebook (pedido mais recente), auditoria dos contatos de móveis, revisão visual de recuperação de glitch, Livro/trilhas/CONTINUAR no Player, física da estrada e build/regressão final. Depois produzir roteiro completo de apresentação para 30/10/26. Relatório compacto PDF/DOCX de 12 páginas concluído. Lista detalhada em PENDENCIAS.md. Não desativar a automação enquanto houver pendências. Limite consultado antes de publicar: 6% restante; parar perto dos 5% conforme usuário.

## Retomada de 26/10/2026 — execução das 11h59 de Brasília

O limite foi consultado no começo da retomada e durante o trabalho. Na última leitura atualizada, a janela de cinco horas estava com 73% consumidos (27% restantes); sem créditos de reset. O usuário pediu para parar quando restarem aproximadamente 5%, preservando este checkpoint. A automação permanece ativa porque existem itens pendentes.

### Notebook/cadeira

Após comparar `office-notebook-seated.png` com a captura anterior, confirmei que o shader aplicava uma substituição retangular de piso na silhueta antiga da cadeira. Corrigi somente `Assets/Resources/Varginha/CinemaOpening/IllustratedMorning.shader`: distância assinada de uma caixa arredondada, transição de cobertura suave e área suficiente para pernas/sombra. Não alterei texturas/sprites aprovados nem mexi em cenas/saves. O primeiro teste com uma máscara de silhueta segmentada deixou pés fantasma; descartei essa candidata e mantive a caixa arredondada maior, que limpa a silhueta.

Validação final em projeto QA isolado: `office-rounded-cutout-player.xml` aprovado, 1/1, para `OfficeSeatAlignsWithTheCushionAndReturnsToItsClearApproach` (StandaloneWindows64). A captura real é `output/revisao-final-2026-10-26/world-review/office-notebook-seated.png`. Ela confirma o fluxo e oclusão do personagem; ainda resta uma transição de piso visível em tamanho de jogo. Portanto item 02 segue Parcial, sem alegar acabamento final. Não houve alteração de cena.

Runner usado: `Tools/QA/run_master_review.ps1`, no clone `C:/Users/Usuario/AppData/Local/Temp/varginha-master-qa-20261023-120709`, com `Library` separada; Unity original do usuário não foi tocado. A execução Player final foi envolvida por `Tools/AgentOutput/compact.mjs`. Resultados tentativos de outras máscaras também passaram o assert, mas suas capturas tinham artefatos visuais e não devem ser contados como versão aceita.

### Apresentação e publicação

O usuário pediu abrir a apresentação atualizada. O caminho conhecido é `output/daily-sprint-2026-10-20/A_Sprint_Final_Varginha.html`, cujo conteúdo ainda só resume até 23/10. `cua_repl` identificou o navegador integrado vazio e a política bloqueou a abertura de `file://`, permitindo apenas `http:` e `https:` e proibindo contornar por outro navegador, servidor ou superfície. Não foi aberta. Item 20 permanece Bloqueado; nenhuma alternativa foi tentada.

Os commits de 26/10 até `43e6e3e` estão sincronizados com `origin/codex/campanha-15-fases-e-site`. O pedido explícito de publicar a atualização do dia permanece válido. O shader original e a lista/report da revisão foram enviados em `8f60fbf`/`216b2d9`; a auditoria global e seu estado foram enviados em `8a82861`; amostra de piso em `2ea8725`; conclusão do item 04 em `2677bc9`; captura da transição, conclusão do item 01 e feather do shader em `5f64ec6`, `1640b3a` e `43e6e3e`. O checkpoint desta pasta `output` continua local. Temporários, outputs de QA, `InitTestScene`, recovery e URP temporário ficaram de fora.

### Pedidos que continuam abertos

- Pendências atuais: itens 01, 02, 04 e 11 seguem parciais; item 10 foi concluído após Player/inspeção visual, 12 e 13 foram concluídos após `vehicle-fixed.xml` 3/3.
- Item 19: após finalizar e validar os serviços, criar roteiro natural da apresentação de 30/10/2026 baseado em registros e evidências, sem inventar fatos.
- A lista completa foi atualizada em `output/revisao-mestre-2026-10-23/PENDENCIAS.md`, incluindo item 20 e estado Parcial atualizado do notebook.
- Próximos passos: continuar os itens 01, 02 e 04, revisar transições restantes do item 11, executar regressões finais e preparar o roteiro quando concluir. A apresentação diária precisa receber dados posteriores a 23/10 e a abertura do HTML está bloqueada pela política `file://`.

### Regressão adicional de 26/10 — puzzles e veículos

Depois do commit `8f60fbf`, executei recortes Player adicionais na mesma cópia QA. `puzzle-design-player.xml` terminou Passed: 8/8, zero falhas e ignorados. Inclui `BookCanBeReassembledLinkedSavedAndCorrectedWithController`, `TrailMapSelectsSavesAndCorrectsAnySegmentWithController`, `CompactContinueUsesControllerAndCannotSkipTwoChapters`, navegação por memória e caminhos internos do casarão. Revisei visualmente `book-start.png`, `book-wrong-reference.png`, `book-reconstructed.png`, `trail-puzzle.png` e `compact-continue-library.png`. O Livro está legível/corrigível; CONTINUAR está no canto inferior direito e o avanço duplo permanece bloqueado. Item 10 atualizado para Concluído; item 11 segue Parcial até revisar as outras transições.

`vehicle-fixed.xml` terminou Passed: 3/3 em StandaloneWindows64. `LegacyRoadAcceleratesCoastsReversesAndEscapesItsLowerLimit`, `OptionalTrackUsesFixedStepAccelerationCoastAndBrakeBeforeChangingDirection` e `InitialContactWithTheBottomWallDoesNotTrapForwardMotion` passaram. Isso ocorreu depois do diagnóstico falho `road-input.xml` (07:48 Brasília); o teste Player final de 07:51, já com a implementação corrente, conseguiu mover e reverter na estrada e vale como evidência mais recente. Itens 12 e 13 agora são Concluídos. `full-editmode.xml`/`final-editmode-r4.xml`: 420/420 no recorte que antecede as últimas alterações visuais.

Lista local e rastreável atualizada: `output/revisao-mestre-2026-10-23/PENDENCIAS.md`; cópia rastreável e atualização diária foram publicadas no commit `216b2d9`. Branch remoto está até `216b2d9`; o working tree mantém arquivos preexistentes de QA e URP temporário fora dos commits.

### Auditoria global de colisões e proporções — 26/10

O teste Player `CampaignWorldAuditPlayTests.AllGameplayAreasAuditHumanProportionsAndRealFurnitureContacts` terminou Passed (1/1) em `world-contact-audit-player-final4.xml`, após percorrer 18 ambientes, 302 móveis e 1.018 aproximações. CSV: 51 silhuetas visíveis, altura de 0,9545 a 1,5963 m e largura de 0,4562 a 1,2977 m. Quatro móveis não tiveram nenhum lado livre para o probe — mesa redonda da fase 6, lampiões oeste/leste da fase 12 e torneira central da fase 18 — pois estão atrás de outros colliders e fora dos percursos caminháveis. Revisei visualmente as 18 capturas e não encontrei falha aparente de profundidade/encaixe; item 01 passa a Concluído com essas limitações registradas.

Corrigi o teste para medir as regiões opacas dos sprites (não o quadro transparente completo), evitar retratos de interface e tratar Padre Fábio como ator somente nas fases em que ele aparece na cena. Tentativas anteriores falharam em assertivas de medição, não em runtime do jogo; a execução final completa passou. Os CSVs de proporção/contatos, resumo JSON e capturas estão em `output/revisao-final-2026-10-26/`. A lista detalhada foi atualizada tanto em `output/revisao-mestre-2026-10-23/PENDENCIAS.md` quanto em `Docs/CampanhaOficial/PENDENCIAS_REVISAO_MESTRE_2026-10-26.md`, ainda a publicar.

O teste e os registros foram commitados e enviados como `8a82861` (`test: audit world proportions and furniture contacts`). Depois, refinei em `IllustratedMorning.shader` o deslocamento da amostra do piso (160→80 px para usar uma área mais próxima). A validação correta foi `office-nearby-floor-player-valid.xml`, 1/1 Passed; a primeira chamada do filtro selecionou zero testes e não foi contada. Inspecionei a captura nova: a diferença do recorte diminuiu, porém a borda do piso continua aparente, então item 02 permanece Parcial. Shader e registro desta etapa foram enviados em `2ea8725`.

Na sequência, ampliei a faixa de feather da máscara de 12 px para 24 px. `office-feathered-floor-player.xml` passou 1/1 no Player. Inspecionei novamente a captura; o recorte está mais suave, sem revelar as pernas antigas, mas ainda se percebe a variação da textura. Item 02 segue Parcial; shader/estado publicados em `43e6e3e`.

Correção da recuperação do glitch passou por nova inspeção visual. `visual-fixed.xml` aprovou `RepeatedGlitchesReturnToTheSameReadableSceneAfterRecovery` (1/1), que aplica três repetições e exige luminância da imagem recuperada dentro de 0,025 da cena-base; comparei as capturas `04_glitch_before.png`, `04_glitch_peak.png` e `04_glitch_recovered_2.png` no clone QA. Combinado com o recorte `vehicle-glitch-playmode.xml` (reconfiguração/repetição/cancelamento), item 04 muda para Concluído. A lista detalhada e o registro diário foram publicados em `2677bc9`.

Para item 11, ampliei `CompactContinueUsesControllerAndCannotSkipTwoChapters` com uma captura após a transição real da fase 6 à 7. `compact-continue-transition-player.xml` passou 1/1. Inspecionei `compact-continue-next-chapter.png`; a cena seguinte aparece com sua orientação inicial e HUD legível. Lista/atualização diária agora registra que essa amostra foi revisada, mas outros capítulos ainda faltam. Teste/documentação publicados em `5f64ec6`.

### Atualização 26/10 — transição adicional da fase 14

`ContinuationContinueOpensAndAdvancesToTheNextChapter` foi acrescentado a `Assets/Tests/PlayMode/CampaignPuzzleDesignPlayTests.cs` e executado em Player StandaloneWindows64. Resultado: `continuation-continue-transition-player.xml`, 1/1 Passed, zero falhas/ignorados. O caso salva a conclusão da fase 14, abre CONTINUAR, aceita apenas o primeiro comando, confirma a fase salva 15 e captura a próxima cena. Inspecionei `continuation-chapter-complete.png` e `continuation-next-chapter.png`: menu de conclusão e HUD/cena seguinte legíveis; a fase salva 15 aparece como Ato 4, fase 11. Item 11 continua Parcial até revisar transições de outros capítulos.

Os arquivos rastreáveis `Docs/CampanhaOficial/ATUALIZACAO_2026-10-26.md` e `PENDENCIAS_REVISAO_MESTRE_2026-10-26.md`, além desta lista local, receberam essa evidência. Nenhuma cena ou mecânica de jogo foi modificada nesta etapa. A captura visual do notebook ainda mostra transição de piso, portanto item 02 permanece Parcial. A daily HTML ainda tem dados antigos e a abertura de `file://` continua bloqueada pela política do navegador. Itens 19 (roteiro de apresentação) e 20 (abrir daily) continuam em aberto/bloqueado. Nenhuma automação foi desativada.

### Tentativa visual exploratória — 26/10

Testei somente no workspace local uma faixa de feather mais ampla (`smoothstep(-12,44)`) para reduzir a leitura do remendo. A primeira chamada usou filtro incorreto e selecionou zero testes, portanto não vale como aprovação. A repetição com `CampaignSchoolDeskPlayTests.OfficeSeatAlignsWithTheCushionAndReturnsToItsClearApproach` no Player isolado não recebeu atividade do Player e terminou com timeout de 180 s (`office-wide-feather-valid.log`); não houve resultado/captura utilizável. Reverti a linha para a versão validada de 24 px (`smoothstep(-4,20)`) e deixei item 02 Parcial. O clone QA usado foi encerrado; o Unity do projeto do usuário não foi iniciado nem alterado nesta tentativa. Não publicar a variação de teste.

O filtro correto do teste de notebook no Player terminou em timeout sem atividade do Player; o processo batch Unity da cópia QA permaneceu listado após o timeout e bloqueou o runner subsequente. A chamada subsequente de PlayMode Editor foi recusada antes de sincronizar/iniciar o teste, porque a cópia estava ocupada. Não atribuir aprovação a nenhuma dessas chamadas. A variação do shader foi revertida; nenhuma mudança de shader pendente. Um teste exploratório 10→11 foi retirado antes de ser validado. Reabrir QA somente depois de confirmar que o processo da cópia encerrou; não iniciar outra Unity sobre esse clone em paralelo.

## Fechamento verificado - 27/10/2026

Pedido atual: concluir as pendências e criar build atualizada. Itens 02, 11, 19 e 20 fechados; candidata 0.4.9-beta no diretório Builds do workspace. EditMode 420/420; regressão Player 15/15; saídas 15/15; HUD/travessia 3/3. Daily HTML/PDF de dez slides e roteiro completo de seis páginas concluídos. Consultar Docs/CampanhaOficial/ENTREGA_2026-10-27.md antes de retomar: diagnósticos antigos parciais não são a situação atual. Não repetir testes nem alterar jogo sem novo motivo. Preservados arquivos locais anteriores, cenas, texturas e saves. Não houve publicação externa nesta rodada.
