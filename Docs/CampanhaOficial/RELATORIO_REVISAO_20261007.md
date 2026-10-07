# Revisão da campanha, controles e Fusca — 07/10/2026

Campanha com **14 fases em cinco atos**, preservando IDs e mecânicas existentes. A fase **Câmaras do Selo** (ID 17 e suas duas áreas) foi retirada; os circuitos do confronto final permanecem. Saves antigos migram para a versão 3.

## Resultado

Saída da Ouzana, passagens do casarão e retorno do porão possuem destinos e chegadas verificadas. Alunos usam os sprites atuais com alinhamento/oclusão da cadeira. A lavagem do rosto se aproxima da pia e termina sem travar o movimento. A transição para 2026 usa cartão sóbrio no tema pixel art.

O Fusca lateral é o arquivo original fornecido, sem mudança de pixels. O carro e o brilho antigos desenhados no fundo são cobertos com recortes de asfalto do próprio cenário; apenas a região correspondente recebe a correção. O feixe parte do farol visível do novo sprite e tem alcance limitado. Novas vistas frontal e superior inferem as superfícies ocultas da referência; rotações superiores usam 90 graus sem interpolação. O manifesto registra essa distinção.

## Input Actions e mapeamento

O projeto usa o Input System 1.20.0 já instalado e a abstração Gamepad. O asset runtime VarginhaRuntimeInputs reutiliza as ações do sistema existente. Gameplay inclui Move/Aim, comandos já existentes, CarInteract, Journal, Inventory, Flashlight e Pause; UI inclui Navigate, Submit/Cancel e atividade de dispositivo. [Exportação](INPUT_ACTIONS.json).

Padrões: analógico esquerdo/D-Pad movimenta/navega; A interage/confirma; B volta/esquiva; X ataca; Y abre mochila; Back/View abre caderno; Start pausa; LT corre; RT alterna lanterna; LB/RB comandam aluno/apoio; L3 pula onde já permitido; analógico direito mira. Teclado/mouse conservam seus bindings; Fusca usa exclusivamente W. A/B dos menus e Start permanecem fixos.

Remapeamento de dez ações aplica overrides somente nos bindings Gamepad existentes; CarInteract acompanha Interact. Preferências persistem, conflitos trocam os botões e a captura aguarda a liberação inicial. Start/Esc, 15 segundos, desconexão e perda de foco cancelam. HUD e prompts refletem dispositivo/remapeamento. Deadzone 0,20–0,95. Vibração configurável desliga em expiração, pausa, mudança de cena, foco, desconexão e encerramento. [Tutorial do Harrow no Editor](CONTROLE_XINPUT.md).

## Validação

- Compilação do Editor sem erros.

- Edit Mode: **99/99** aprovados.

- Play Mode: **45 casos distintos aprovados** considerando o resultado mais recente de cada caso. A rodada completa teve 44/45; o caso da oficina foi corrigido no teste e aprovado na bateria posterior. O teste usava W para caminhar, abrindo o contexto do carro; passou a usar as setas no percurso e W na confirmação. O reteste dos faróis/portas cobre os últimos ajustes visuais. As rodadas e resultados individuais estão em [evidências](EVIDENCIAS_REVISAO_20261007.json).

- Build Windows x64: **Succeeded**, 19 cenas da campanha/menu/áreas, 0 erros e 1 avisos. [Instruções da alpha](NOTAS_BUILD_ALPHA.md).

O único aviso da build informa que o conector de automação Pipeline do Editor estará desativado no executável. Os controles do jogo usam o Input System e não dependem desse conector.

- [Capturas no Unity](../QARevisao20261007/README.md): controle, alunos, pia e Fusca.

O único aviso da build informa que o conector de automação Pipeline do Editor ficará desativado no executável. Os controles usam o Input System e não dependem desse conector.

O usuário confirmou funcionamento do controle físico. Gamepad simulado verifica software; motores do Harrow e novo remapeamento no dispositivo exigem conferência física. Não há uma certificação de hardware, requisitos mínimos medidos ou jogada manual completa em todas as fases. A build Windows abriu com motor e Input System inicializados, sem exceções no log do teste de abertura. Isso não substitui uma jogada completa. Capturas históricas permanecem preservadas.

## Download e apresentação

A [alpha Windows está publicada no GitHub](https://github.com/fwbiodev7/segredo-varginha-game-teste-cutscene-gdd-ldd/releases/tag/jogo_varginha_build_alpha), com ZIP de 209.392.131 bytes (199,7 MiB). O GitHub confirmou o digest SHA-256 do pacote. O código da build e os seis slides estão no commit `a9f7e374a15c67c8c558c21a1ae40d2017a59263`; o executável é distribuído como asset da release.

O [site oficial](https://o-segredo-de-varginha.zfabiobrronaldo.chatgpt.site) foi publicado na versão 5 com botão para o pacote confirmado. As verificações estáticas passaram. A ferramenta de inspeção visual do navegador estava indisponível, portanto não foi realizada uma revisão manual final do site no navegador.

A [apresentação de seis slides](../Apresentacao/Atualizacoes_O_Segredo_de_Varginha_Alpha_Final.pptx) inclui texto editável, capturas do projeto e notas para o apresentador. Estrutura e geometria do PPTX foram verificadas, e todos os slides finais foram renderizados e inspecionados. Não foi executado o aplicativo PowerPoint nativo durante essa verificação.

## Arquivos desta revisão

Base de comparação: 910a790. A tabela inclui cada arquivo criado, modificado e removido; a publicação posterior acrescenta URLs e evidências do pacote. Arquivos de recuperação do usuário e materiais locais de Preview não fazem parte do commit.

| Estado | Arquivo | Alteração |

| --- | --- | --- |

| Criado | [Assets/ArtSource/FuscaOriginalViewsSource.png](<../../Assets/ArtSource/FuscaOriginalViewsSource.png>) | Arte-fonte das novas vistas frontal e superior, derivadas da referência fornecida. |
| Criado | [Assets/ArtSource/FuscaOriginalViewsSource.png.meta](<../../Assets/ArtSource/FuscaOriginalViewsSource.png.meta>) | Metadados Unity do arquivo associado: GUID e configuração de importação preservados no versionamento. |
| Removido | [Assets/Resources/Varginha/IllustratedMaps/Continuation17.png](<../../Assets/Resources/Varginha/IllustratedMaps/Continuation17.png>) | Cena/mapa exclusivo de Câmaras do Selo, retirado junto às referências e configuração de build. |
| Removido | [Assets/Resources/Varginha/IllustratedMaps/Continuation17.png.meta](<../../Assets/Resources/Varginha/IllustratedMaps/Continuation17.png.meta>) | Cena/mapa exclusivo de Câmaras do Selo, retirado junto às referências e configuração de build. |
| Modificado | [Assets/Resources/Varginha/IllustratedMaps/Layouts.json](<../../Assets/Resources/Varginha/IllustratedMaps/Layouts.json>) | Retira layouts exclusivos da fase removida, abre passagens do casarão e cobre o carro/brilho antigos com asfalto dos próprios mapas. |
| Criado | [Assets/Resources/Varginha/Interface/ControllerDiagram.png](<../../Assets/Resources/Varginha/Interface/ControllerDiagram.png>) | Desenho do controle em pixel art integrado à tela de configurações. |
| Criado | [Assets/Resources/Varginha/Interface/ControllerDiagram.png.meta](<../../Assets/Resources/Varginha/Interface/ControllerDiagram.png.meta>) | Metadados Unity do arquivo associado: GUID e configuração de importação preservados no versionamento. |
| Criado | [Assets/Resources/Varginha/StoryEffects/FuscaOriginal.png](<../../Assets/Resources/Varginha/StoryEffects/FuscaOriginal.png>) | Cópia integral e byte a byte do sprite original de três quadros fornecido pelo usuário. |
| Criado | [Assets/Resources/Varginha/StoryEffects/FuscaOriginal.png.meta](<../../Assets/Resources/Varginha/StoryEffects/FuscaOriginal.png.meta>) | Metadados Unity do arquivo associado: GUID e configuração de importação preservados no versionamento. |
| Criado | [Assets/Resources/Varginha/StoryEffects/FuscaOriginalFront.png](<../../Assets/Resources/Varginha/StoryEffects/FuscaOriginalFront.png>) | Nova vista frontal derivada do modelo, com importação pixel art. |
| Criado | [Assets/Resources/Varginha/StoryEffects/FuscaOriginalFront.png.meta](<../../Assets/Resources/Varginha/StoryEffects/FuscaOriginalFront.png.meta>) | Metadados Unity do arquivo associado: GUID e configuração de importação preservados no versionamento. |
| Criado | [Assets/Resources/Varginha/StoryEffects/FuscaOriginalManifest.json](<../../Assets/Resources/Varginha/StoryEffects/FuscaOriginalManifest.json>) | Registra hash, recorte original e procedência/dimensões das novas vistas. |
| Criado | [Assets/Resources/Varginha/StoryEffects/FuscaOriginalManifest.json.meta](<../../Assets/Resources/Varginha/StoryEffects/FuscaOriginalManifest.json.meta>) | Metadados Unity do arquivo associado: GUID e configuração de importação preservados no versionamento. |
| Criado | [Assets/Resources/Varginha/StoryEffects/FuscaOriginalTopEast.png](<../../Assets/Resources/Varginha/StoryEffects/FuscaOriginalTopEast.png>) | Rotação exata da vista superior para leste, sem interpolação. |
| Criado | [Assets/Resources/Varginha/StoryEffects/FuscaOriginalTopEast.png.meta](<../../Assets/Resources/Varginha/StoryEffects/FuscaOriginalTopEast.png.meta>) | Metadados Unity do arquivo associado: GUID e configuração de importação preservados no versionamento. |
| Criado | [Assets/Resources/Varginha/StoryEffects/FuscaOriginalTopNorth.png](<../../Assets/Resources/Varginha/StoryEffects/FuscaOriginalTopNorth.png>) | Nova vista superior voltada ao norte, usada na oficina. |
| Criado | [Assets/Resources/Varginha/StoryEffects/FuscaOriginalTopNorth.png.meta](<../../Assets/Resources/Varginha/StoryEffects/FuscaOriginalTopNorth.png.meta>) | Metadados Unity do arquivo associado: GUID e configuração de importação preservados no versionamento. |
| Criado | [Assets/Resources/Varginha/StoryEffects/FuscaOriginalTopSouth.png](<../../Assets/Resources/Varginha/StoryEffects/FuscaOriginalTopSouth.png>) | Rotação exata da vista superior para sul, sem interpolação. |
| Criado | [Assets/Resources/Varginha/StoryEffects/FuscaOriginalTopSouth.png.meta](<../../Assets/Resources/Varginha/StoryEffects/FuscaOriginalTopSouth.png.meta>) | Metadados Unity do arquivo associado: GUID e configuração de importação preservados no versionamento. |
| Criado | [Assets/Resources/Varginha/StoryEffects/FuscaOriginalTopWest.png](<../../Assets/Resources/Varginha/StoryEffects/FuscaOriginalTopWest.png>) | Rotação exata da vista superior para oeste, sem interpolação. |
| Criado | [Assets/Resources/Varginha/StoryEffects/FuscaOriginalTopWest.png.meta](<../../Assets/Resources/Varginha/StoryEffects/FuscaOriginalTopWest.png.meta>) | Metadados Unity do arquivo associado: GUID e configuração de importação preservados no versionamento. |
| Removido | [Assets/Scenes/Ato5_Fase17_Camaras_do_Selo.unity](<../../Assets/Scenes/Ato5_Fase17_Camaras_do_Selo.unity>) | Cena/mapa exclusivo de Câmaras do Selo, retirado junto às referências e configuração de build. |
| Removido | [Assets/Scenes/Ato5_Fase17_Camaras_do_Selo.unity.meta](<../../Assets/Scenes/Ato5_Fase17_Camaras_do_Selo.unity.meta>) | Cena/mapa exclusivo de Câmaras do Selo, retirado junto às referências e configuração de build. |
| Removido | [Assets/Scenes/Ato5_Fase17_Camaras_do_Selo_Area1.unity](<../../Assets/Scenes/Ato5_Fase17_Camaras_do_Selo_Area1.unity>) | Cena/mapa exclusivo de Câmaras do Selo, retirado junto às referências e configuração de build. |
| Removido | [Assets/Scenes/Ato5_Fase17_Camaras_do_Selo_Area1.unity.meta](<../../Assets/Scenes/Ato5_Fase17_Camaras_do_Selo_Area1.unity.meta>) | Cena/mapa exclusivo de Câmaras do Selo, retirado junto às referências e configuração de build. |
| Removido | [Assets/Scenes/Ato5_Fase17_Camaras_do_Selo_Area2.unity](<../../Assets/Scenes/Ato5_Fase17_Camaras_do_Selo_Area2.unity>) | Cena/mapa exclusivo de Câmaras do Selo, retirado junto às referências e configuração de build. |
| Removido | [Assets/Scenes/Ato5_Fase17_Camaras_do_Selo_Area2.unity.meta](<../../Assets/Scenes/Ato5_Fase17_Camaras_do_Selo_Area2.unity.meta>) | Cena/mapa exclusivo de Câmaras do Selo, retirado junto às referências e configuração de build. |
| Modificado | [Assets/Scripts/Editor/Testing/CampaignSequenceBuilder.cs](<../../Assets/Scripts/Editor/Testing/CampaignSequenceBuilder.cs>) | Remove cenas e mapa da fase Câmaras do Selo pelo Editor e atualiza a sequência de 14 fases. |
| Criado | [Assets/Scripts/Editor/Testing/VarginhaAlphaBuild.cs](<../../Assets/Scripts/Editor/Testing/VarginhaAlphaBuild.cs>) | Gera a alpha Windows x64 com menu, campanha atual e áreas conectadas, validando cenas e registrando o resultado da build. |
| Criado | [Assets/Scripts/Editor/Testing/VarginhaAlphaBuild.cs.meta](<../../Assets/Scripts/Editor/Testing/VarginhaAlphaBuild.cs.meta>) | Metadados Unity do arquivo associado: GUID e configuração de importação preservados no versionamento. |
| Modificado | [Assets/Scripts/Game/UI/PixelMenuTheme.cs](<../../Assets/Scripts/Game/UI/PixelMenuTheme.cs>) | Encaminha botões e labels ao adaptador de navegação e prompts sem alterar o tema existente. |
| Modificado | [Assets/Scripts/Game/Varginha/EdelzioTopDownController.cs](<../../Assets/Scripts/Game/Varginha/EdelzioTopDownController.cs>) | Reutiliza Input Actions para movimento, corrida, interação, mochila, caderno e lanterna; respeita W do Fusca. |
| Modificado | [Assets/Scripts/Game/Varginha/Experiment/CampaignChapterPreview.cs](<../../Assets/Scripts/Game/Varginha/Experiment/CampaignChapterPreview.cs>) | Revisa o cartão pixel art de transição para 2026 e a entrada/saída suave. |
| Modificado | [Assets/Scripts/Game/Varginha/Experiment/CampaignCinematics.cs](<../../Assets/Scripts/Game/Varginha/Experiment/CampaignCinematics.cs>) | Garante desligamento de vibração nas transições e pausas. |
| Modificado | [Assets/Scripts/Game/Varginha/Experiment/CampaignContinuationController.cs](<../../Assets/Scripts/Game/Varginha/Experiment/CampaignContinuationController.cs>) | Remove a lógica exclusiva das câmaras; corrige passagens/chegadas do casarão, alunos, navegação e prompts do controle. |
| Modificado | [Assets/Scripts/Game/Varginha/Experiment/CampaignContinuationDefinition.cs](<../../Assets/Scripts/Game/Varginha/Experiment/CampaignContinuationDefinition.cs>) | Retira a definição da fase removida e identifica a passagem do jardim do casarão. |
| Modificado | [Assets/Scripts/Game/Varginha/Experiment/CampaignContinuationState.cs](<../../Assets/Scripts/Game/Varginha/Experiment/CampaignContinuationState.cs>) | Retira a validação exclusiva da fase removida, conservando slots necessários à migração de saves. |
| Criado | [Assets/Scripts/Game/Varginha/Experiment/CampaignControllerSettings.cs](<../../Assets/Scripts/Game/Varginha/Experiment/CampaignControllerSettings.cs>) | Desenha o controle na UI pixel art com ações, linhas de indicação, captura e restauração dos botões. |
| Criado | [Assets/Scripts/Game/Varginha/Experiment/CampaignControllerSettings.cs.meta](<../../Assets/Scripts/Game/Varginha/Experiment/CampaignControllerSettings.cs.meta>) | Metadados Unity do arquivo associado: GUID e configuração de importação preservados no versionamento. |
| Modificado | [Assets/Scripts/Game/Varginha/Experiment/CampaignExpansionController.cs](<../../Assets/Scripts/Game/Varginha/Experiment/CampaignExpansionController.cs>) | Cria saída da Ouzana; adapta oficina a W/controle, menus, pausa e confirmações. |
| Modificado | [Assets/Scripts/Game/Varginha/Experiment/CampaignFinalAllies.cs](<../../Assets/Scripts/Game/Varginha/Experiment/CampaignFinalAllies.cs>) | Usa bindings e prompts do controle remapeados para os comandos dos aliados. |
| Modificado | [Assets/Scripts/Game/Varginha/Experiment/CampaignHints.cs](<../../Assets/Scripts/Game/Varginha/Experiment/CampaignHints.cs>) | Retira dicas exclusivas da fase removida. |
| Modificado | [Assets/Scripts/Game/Varginha/Experiment/CampaignHudIcons.cs](<../../Assets/Scripts/Game/Varginha/Experiment/CampaignHudIcons.cs>) | Mostra botões atuais de pausa, mochila e caderno conforme o último dispositivo e remapeamento. |
| Modificado | [Assets/Scripts/Game/Varginha/Experiment/CampaignIllustratedMaps.cs](<../../Assets/Scripts/Game/Varginha/Experiment/CampaignIllustratedMaps.cs>) | Integra o Fusca original com escala uniforme e faróis posicionados no sprite; preserva a composição dos mapas. |
| Modificado | [Assets/Scripts/Game/Varginha/Experiment/CampaignJournal.cs](<../../Assets/Scripts/Game/Varginha/Experiment/CampaignJournal.cs>) | Atualiza prompts do caderno e retira entradas exclusivas da fase removida. |
| Modificado | [Assets/Scripts/Game/Varginha/Experiment/CampaignMapConstruction.cs](<../../Assets/Scripts/Game/Varginha/Experiment/CampaignMapConstruction.cs>) | Usa o Fusca original também na construção alternativa dos mapas. |
| Criado | [Assets/Scripts/Game/Varginha/Experiment/CampaignOriginalFusca.cs](<../../Assets/Scripts/Game/Varginha/Experiment/CampaignOriginalFusca.cs>) | Centraliza e reaproveita sprites lateral/frontal/superiores, sem cópias de textura por quadro. |
| Criado | [Assets/Scripts/Game/Varginha/Experiment/CampaignOriginalFusca.cs.meta](<../../Assets/Scripts/Game/Varginha/Experiment/CampaignOriginalFusca.cs.meta>) | Metadados Unity do arquivo associado: GUID e configuração de importação preservados no versionamento. |
| Modificado | [Assets/Scripts/Game/Varginha/Experiment/CampaignSeatingLayers.cs](<../../Assets/Scripts/Game/Varginha/Experiment/CampaignSeatingLayers.cs>) | Alinha o torso à cadeira, oculta pernas que aparentavam estar em pé e separa a colisão dos pés da cadeira. |
| Modificado | [Assets/Scripts/Game/Varginha/Experiment/CampaignSequence.cs](<../../Assets/Scripts/Game/Varginha/Experiment/CampaignSequence.cs>) | Define 14 capítulos, remove o ID 17 da progressão e migra saves para versão 3 sem perder evidências. |
| Modificado | [Assets/Scripts/Game/Varginha/Experiment/CampaignStory.cs](<../../Assets/Scripts/Game/Varginha/Experiment/CampaignStory.cs>) | Atualiza versão e retomada do save. |
| Modificado | [Assets/Scripts/Game/Varginha/Experiment/CampaignTeamEdelzio.cs](<../../Assets/Scripts/Game/Varginha/Experiment/CampaignTeamEdelzio.cs>) | Adapta os comandos de aluno/apoio ao gamepad e à navegação existente. |
| Modificado | [Assets/Scripts/Game/Varginha/Experiment/CampaignWorkshopVehicle.cs](<../../Assets/Scripts/Game/Varginha/Experiment/CampaignWorkshopVehicle.cs>) | Reutiliza vistas superiores do Fusca original e Input Actions, preservando estacionamento e física existentes. |
| Modificado | [Assets/Scripts/Game/Varginha/Experiment/ExperimentGUI.cs](<../../Assets/Scripts/Game/Varginha/Experiment/ExperimentGUI.cs>) | Encaminha botões e textos à navegação e aos prompts do dispositivo ativo. |
| Modificado | [Assets/Scripts/Game/Varginha/Experiment/VarginhaCampaignDrive.cs](<../../Assets/Scripts/Game/Varginha/Experiment/VarginhaCampaignDrive.cs>) | Usa Fusca original e comando W/controle no trecho legado da viagem. |
| Modificado | [Assets/Scripts/Game/Varginha/Experiment/VarginhaCampaignPhase1.cs](<../../Assets/Scripts/Game/Varginha/Experiment/VarginhaCampaignPhase1.cs>) | Adapta pausa/interação e contexto de navegação da abertura. |
| Modificado | [Assets/Scripts/Game/Varginha/Experiment/VarginhaCampaignStage.cs](<../../Assets/Scripts/Game/Varginha/Experiment/VarginhaCampaignStage.cs>) | Adapta escola, alunos sentados, diálogos, pausa e Fusca ao controle e aos prompts. |
| Modificado | [Assets/Scripts/Game/Varginha/Experiment/VarginhaExperimentLab.cs](<../../Assets/Scripts/Game/Varginha/Experiment/VarginhaExperimentLab.cs>) | Mantém compatibilidade da interface do laboratório com navegação por controle. |
| Modificado | [Assets/Scripts/Game/Varginha/Experiment/VarginhaGameSettings.cs](<../../Assets/Scripts/Game/Varginha/Experiment/VarginhaGameSettings.cs>) | Adiciona vibração/intensidade, navegação em sliders e a tela de controles com diagrama e remapeamento. |
| Modificado | [Assets/Scripts/Game/Varginha/FuscaLevelExit.cs](<../../Assets/Scripts/Game/Varginha/FuscaLevelExit.cs>) | Garante entrada/confirmação pelo comando exclusivo do Fusca, sem E/Enter/clique. |
| Modificado | [Assets/Scripts/Game/Varginha/VarginhaBackpackInventory.cs](<../../Assets/Scripts/Game/Varginha/VarginhaBackpackInventory.cs>) | Seleciona itens e abas por gamepad, equipa com A e fecha com B; atualiza instruções. |
| Modificado | [Assets/Scripts/Game/Varginha/VarginhaCameraShake.cs](<../../Assets/Scripts/Game/Varginha/VarginhaCameraShake.cs>) | Reutiliza eventos de impacto/susto para vibração configurável. |
| Modificado | [Assets/Scripts/Game/Varginha/VarginhaCombatCursor.cs](<../../Assets/Scripts/Game/Varginha/VarginhaCombatCursor.cs>) | Usa o analógico direito nas mecânicas de mira existentes. |
| Modificado | [Assets/Scripts/Game/Varginha/VarginhaGameHUD.cs](<../../Assets/Scripts/Game/Varginha/VarginhaGameHUD.cs>) | Atualiza HUD e navegação de diálogos, pausa, vitória e derrota para o dispositivo ativo. |
| Criado | [Assets/Scripts/Game/Varginha/VarginhaGamepadBindings.cs](<../../Assets/Scripts/Game/Varginha/VarginhaGamepadBindings.cs>) | Aplica overrides nas ações existentes, salva preferências, troca conflitos e cancela captura com segurança. |
| Criado | [Assets/Scripts/Game/Varginha/VarginhaGamepadBindings.cs.meta](<../../Assets/Scripts/Game/Varginha/VarginhaGamepadBindings.cs.meta>) | Metadados Unity do arquivo associado: GUID e configuração de importação preservados no versionamento. |
| Criado | [Assets/Scripts/Game/Varginha/VarginhaGamepadUI.cs](<../../Assets/Scripts/Game/Varginha/VarginhaGamepadUI.cs>) | Navega pelos botões/sliders/abas da UI existente com seleção inicial, repetição e contexto exclusivo do carro. |
| Criado | [Assets/Scripts/Game/Varginha/VarginhaGamepadUI.cs.meta](<../../Assets/Scripts/Game/Varginha/VarginhaGamepadUI.cs.meta>) | Metadados Unity do arquivo associado: GUID e configuração de importação preservados no versionamento. |
| Modificado | [Assets/Scripts/Game/Varginha/VarginhaInputBindings.cs](<../../Assets/Scripts/Game/Varginha/VarginhaInputBindings.cs>) | Conserva remapeamento de teclado/mouse e conecta os mesmos comandos ao Input System. |
| Criado | [Assets/Scripts/Game/Varginha/VarginhaInputRuntime.cs](<../../Assets/Scripts/Game/Varginha/VarginhaInputRuntime.cs>) | Cria os mapas Gameplay/UI modernos, detecta último dispositivo, aplica deadzone e gerencia vibração e ciclo de vida. |
| Criado | [Assets/Scripts/Game/Varginha/VarginhaInputRuntime.cs.meta](<../../Assets/Scripts/Game/Varginha/VarginhaInputRuntime.cs.meta>) | Metadados Unity do arquivo associado: GUID e configuração de importação preservados no versionamento. |
| Modificado | [Assets/Scripts/Game/Varginha/VarginhaMainMenu.cs](<../../Assets/Scripts/Game/Varginha/VarginhaMainMenu.cs>) | Adapta seleção, voltar, sliders e edição de controles sem mouse. |
| Modificado | [Assets/Scripts/Game/Varginha/VarginhaNotebookQuiz.cs](<../../Assets/Scripts/Game/Varginha/VarginhaNotebookQuiz.cs>) | Adapta navegação e confirmação do puzzle ao controle. |
| Modificado | [Assets/Scripts/Game/Varginha/VarginhaPlayerActionAnimation.cs](<../../Assets/Scripts/Game/Varginha/VarginhaPlayerActionAnimation.cs>) | Aproxima Edelzio da pia e usa poses/tempos suaves existentes ao lavar o rosto. |
| Modificado | [Assets/Scripts/Game/Varginha/VarginhaPlayerSpriteAnimation.cs](<../../Assets/Scripts/Game/Varginha/VarginhaPlayerSpriteAnimation.cs>) | Corrige direção e pose da ação na pia. |
| Modificado | [Assets/Scripts/Game/Varginha/VarginhaTravelCinematic.cs](<../../Assets/Scripts/Game/Varginha/VarginhaTravelCinematic.cs>) | Carrega o arquivo original do Fusca na transição de viagem. |
| Modificado | [Assets/Scripts/Game/Varginha/VarginhaTravelPixelArt.cs](<../../Assets/Scripts/Game/Varginha/VarginhaTravelPixelArt.cs>) | Recorta a lateral sem alterar os pixels originais e preserva a proporção. |
| Modificado | [Assets/Tests/EditMode/CampaignExpansionTests.cs](<../../Assets/Tests/EditMode/CampaignExpansionTests.cs>) | Preserva validação de colisões/plantas e passa a verificar a textura do Fusca original em vez do carro antigo do mapa. |
| Modificado | [Assets/Tests/EditMode/CampaignIllustratedMapTests.cs](<../../Assets/Tests/EditMode/CampaignIllustratedMapTests.cs>) | Verifica o sprite original compartilhado do Fusca e mantém as verificações de contato, profundidade e atlas dos demais móveis. |
| Modificado | [Assets/Tests/EditMode/CampaignSequenceTests.cs](<../../Assets/Tests/EditMode/CampaignSequenceTests.cs>) | Verifica sequência, versão e migração da campanha de 14 fases. |
| Modificado | [Assets/Tests/EditMode/CampaignWorkshopStateTests.cs](<../../Assets/Tests/EditMode/CampaignWorkshopStateTests.cs>) | Verifica importação, cache, escala e orientações do Fusca original. |
| Modificado | [Assets/Tests/PlayMode/CampaignChaptersTests.cs](<../../Assets/Tests/PlayMode/CampaignChaptersTests.cs>) | Atualiza capítulos e isolamento do Input System nos testes. |
| Modificado | [Assets/Tests/PlayMode/CampaignContinuationPlayTests.cs](<../../Assets/Tests/PlayMode/CampaignContinuationPlayTests.cs>) | Retira teste da fase removida e conserva validação de progressão, final e áreas conectadas. |
| Criado | [Assets/Tests/PlayMode/CampaignControllerRemapTests.cs](<../../Assets/Tests/PlayMode/CampaignControllerRemapTests.cs>) | Seis testes de remapeamento, persistência, conflito, captura/cancelamento, HUD, arte original e tela real no Unity. |
| Criado | [Assets/Tests/PlayMode/CampaignControllerRemapTests.cs.meta](<../../Assets/Tests/PlayMode/CampaignControllerRemapTests.cs.meta>) | Metadados Unity do arquivo associado: GUID e configuração de importação preservados no versionamento. |
| Modificado | [Assets/Tests/PlayMode/CampaignExpansionPlayTests.cs](<../../Assets/Tests/PlayMode/CampaignExpansionPlayTests.cs>) | Verifica assentos, lavagem do rosto e progressão com os tempos/poses atuais; isola Input Actions. |
| Modificado | [Assets/Tests/PlayMode/CampaignLabWorkshopAuditTests.cs](<../../Assets/Tests/PlayMode/CampaignLabWorkshopAuditTests.cs>) | Percorre laboratório/oficina com setas para movimento e W na oficina, validando carga, reload, estacionamento e saída. |
| Criado | [Assets/Tests/PlayMode/CampaignRequestedChangesTests.cs](<../../Assets/Tests/PlayMode/CampaignRequestedChangesTests.cs>) | Cinco testes de portas/retornos, menu, deadzone, exclusividade do carro, assentos/pia e encerramento de vibração; gera capturas. |
| Criado | [Assets/Tests/PlayMode/CampaignRequestedChangesTests.cs.meta](<../../Assets/Tests/PlayMode/CampaignRequestedChangesTests.cs.meta>) | Metadados Unity do arquivo associado: GUID e configuração de importação preservados no versionamento. |
| Modificado | [Assets/Tests/PlayMode/VarginhaCampaignTests.cs](<../../Assets/Tests/PlayMode/VarginhaCampaignTests.cs>) | Isola o asset de entrada entre os testes da campanha. |
| Modificado | [Assets/TextMesh Pro/Shaders/TMP_SDF-Mobile.shader](<../../Assets/TextMesh Pro/Shaders/TMP_SDF-Mobile.shader>) | Corrige o caminho do include de propriedades e atualiza a diretiva de depuração para compilar o texto na build Windows sem erro de shader. |
| Criado | [Docs/Apresentacao/Atualizacoes_O_Segredo_de_Varginha_Alpha_Final.pptx](<../../Docs/Apresentacao/Atualizacoes_O_Segredo_de_Varginha_Alpha_Final.pptx>) | Apresentação de seis slides com imagens reais, notas para apresentar e instruções da alpha. |
| Criado | [Docs/Apresentacao/README.md](<../../Docs/Apresentacao/README.md>) | Apresentação de seis slides com imagens reais, notas para apresentar e instruções da alpha. |
| Criado | [Docs/CampanhaOficial/BUILD_ALPHA_MANIFEST.json](<../../Docs/CampanhaOficial/BUILD_ALPHA_MANIFEST.json>) | Registra plataforma, cenas, resultado da build, tamanho e SHA-256 do pacote para download. |
| Criado | [Docs/CampanhaOficial/CAMPANHA_14_FASES.md](<../../Docs/CampanhaOficial/CAMPANHA_14_FASES.md>) | Documenta a sequência atual, áreas e compatibilidade dos saves. |
| Modificado | [Docs/CampanhaOficial/CAMPANHA_15_FASES.md](<../../Docs/CampanhaOficial/CAMPANHA_15_FASES.md>) | Aponta o plano antigo para a sequência vigente de 14 fases. |
| Criado | [Docs/CampanhaOficial/CONTROLE_XINPUT.md](<../../Docs/CampanhaOficial/CONTROLE_XINPUT.md>) | Explica conexão/teste do Harrow, mapeamento, remapeamento, HUD, deadzone e vibração. |
| Criado | [Docs/CampanhaOficial/EVIDENCIAS_REVISAO_20261007.json](<../../Docs/CampanhaOficial/EVIDENCIAS_REVISAO_20261007.json>) | Registra resultados verificáveis de compilação, testes, build e preservação do original. |
| Modificado | [Docs/CampanhaOficial/GUIA_COMPLETO_PUZZLES.md](<../../Docs/CampanhaOficial/GUIA_COMPLETO_PUZZLES.md>) | Atualiza as soluções e números dos 14 capítulos, incluindo oficina, casarão e final. |
| Criado | [Docs/CampanhaOficial/INPUT_ACTIONS.json](<../../Docs/CampanhaOficial/INPUT_ACTIONS.json>) | Registra o asset runtime de Input Actions para consulta dos mapas e bindings padrão. |
| Criado | [Docs/CampanhaOficial/NOTAS_BUILD_ALPHA.md](<../../Docs/CampanhaOficial/NOTAS_BUILD_ALPHA.md>) | Instruções de instalação, controle, testagem e reprodução da alpha Windows. |
| Criado | [Docs/CampanhaOficial/RELATORIO_REVISAO_20261007.md](<../../Docs/CampanhaOficial/RELATORIO_REVISAO_20261007.md>) | Reúne escopo, validação, limites e relação exata de arquivos desta revisão. |
| Criado | [Docs/QARevisao20261007/Alunos_sentados.png](<../../Docs/QARevisao20261007/Alunos_sentados.png>) | Captura real do Unity da revisão e notas de procedência/limites. |
| Criado | [Docs/QARevisao20261007/Controle_remapeamento.png](<../../Docs/QARevisao20261007/Controle_remapeamento.png>) | Captura real do Unity da revisão e notas de procedência/limites. |
| Criado | [Docs/QARevisao20261007/Edelzio_pia.png](<../../Docs/QARevisao20261007/Edelzio_pia.png>) | Captura real do Unity da revisão e notas de procedência/limites. |
| Criado | [Docs/QARevisao20261007/Fusca_original_no_mapa.png](<../../Docs/QARevisao20261007/Fusca_original_no_mapa.png>) | Captura real do Unity da revisão e notas de procedência/limites. |
| Criado | [Docs/QARevisao20261007/README.md](<../../Docs/QARevisao20261007/README.md>) | Captura real do Unity da revisão e notas de procedência/limites. |
| Modificado | [ProjectSettings/EditorBuildSettings.asset](<../../ProjectSettings/EditorBuildSettings.asset>) | Remove as três cenas da fase retirada da configuração de build. |
| Modificado | [README.md](<../../README.md>) | Guia do jogador: início, controles, configuração, puzzles, saves e links da revisão. |
| Modificado | [Website/README.md](<../../Website/README.md>) | Cópia versionada do site: 14 fases, novidades de controle/remapeamento e links de instruções; download atualizado após publicação da alpha. |
| Modificado | [Website/dist/config.js](<../../Website/dist/config.js>) | Cópia versionada do site: 14 fases, novidades de controle/remapeamento e links de instruções; download atualizado após publicação da alpha. |
| Modificado | [Website/dist/index.html](<../../Website/dist/index.html>) | Cópia versionada do site: 14 fases, novidades de controle/remapeamento e links de instruções; download atualizado após publicação da alpha. |
