# Plano de port completo para mobile — O Segredo de Varginha

Status: plano técnico, ainda não implementado. Base inspecionada: Unity 6000.6.0f1, Input System 1.20.0 e campanha com 14 capítulos, menu, investigação, direção do Fusca, minijogos, mochila, combate final e epílogo. Proposta: Android primeiro, em paisagem; iOS em uma etapa posterior. Não reduzir capítulos nem trocar o jogo por uma versão simplificada.

## O que já pode ser reutilizado

`VarginhaInputRuntime.cs` já centraliza ações Move, Aim, Attack, Interact, Dodge, Run, Jump, Inventory, Journal, Flashlight, Pause e comandos de aliados. O joystick pode alimentar essas ações, mantendo o mesmo controle de Edélzio. O Input System oferece controles de tela que produzem eventos de dispositivos virtuais; a aparência do joystick e dos botões deve ser construída no HUD. Fonte: [Unity — On-screen Controls](https://docs.unity3d.com/Packages/com.unity.inputsystem@1.19/manual/OnScreen.html). A implementação será conferida também na documentação local do pacote 1.20 instalado, sem pressupor que as versões são idênticas.

Ainda existem leituras diretas de teclado/mouse e interfaces OnGUI em `CampaignFinalAllies`, `CampaignExpansionController`, `CampaignContinuationController`, `VarginhaCampaignDrive`, `VarginhaGameHUD`, `VarginhaNotebookQuiz`, `ExperimentGUI` e cursores. Portanto, colocar só um joystick na tela não cobre o jogo inteiro.

Nesta máquina foi encontrado somente o módulo Windows do Editor. Antes do primeiro APK será necessário instalar Android Build Support, SDK/NDK e OpenJDK pelo Unity Hub. O port para iOS precisará também de ambiente macOS/Xcode e acesso à assinatura apropriada; isso é dependência futura, não instalação realizada nesta tarefa.

## HUD e comandos por toque

Tela horizontal com área segura para câmera/notch e barras do sistema, calculada com Screen.safeArea. A API retorna o retângulo utilizável da tela; o layout deve responder a mudanças de resolução e recortes. Fonte: [Unity 6000.6 — Screen.safeArea](https://docs.unity3d.com/6000.6/Documentation/ScriptReference/Screen-safeArea.html).

| Região | Controle | Comportamento proposto |
| --- | --- | --- |
| Inferior esquerda | Joystick virtual | Fixo por padrão, opção flutuante; deadzone ajustável, limite circular e retorno imediato a zero ao soltar/cancelar toque. |
| Inferior direita | Atacar | Botão principal; mira pela direção de movimento e seleção assistida de alvo visível. Opção de arrastar para mirar, sem acertar através de paredes. |
| Direita, perto do ataque | Esquiva e interação | Botões distintos; Interagir mostra o alvo/ação contextual. Não disparar ataque ao tocar em uma janela. |
| Direita, contextual | Pular/correr | Só ocultar uma ação se ela não for necessária na situação. Correr pode ser botão alternável ou manter pressionado. |
| Superior direita | Pausa, mochila e caderno | Ícones com rótulo, alvos amplos e retorno claro. |
| Superior esquerda | Vida/sanidade e objetivo | Texto escalável; limites de linha para não cobrir o cenário. |
| Próximo à área de combate | Lanterna | Botão com estado ligado/desligado e equipamento respeitado. |
| Combate final | Três retratos e um apoio | Tocar no retrato comanda aquela vaga; recarga individual em preenchimento circular. Equipe é escolhida na mochila, mantendo os 12 alunos e limite 3 + 1. |

Permitir mover com um dedo e atacar/interagir com outro. Cada controle deve manter o ID do seu toque. Cancelar movimentos, botões segurados e vibração ao abrir mochila, trocar de cena, receber interrupção ou voltar do segundo plano. Adicionar opção canhoto, tamanho/opacidade dos controles e sensibilidade; começar com alvos de toque de aproximadamente 48–64 unidades lógicas e validar em aparelhos reais, não tratar esse valor como medida física garantida.

Criar modo de entrada Touch separado de KeyboardMouse e Gamepad. O joystick virtual não pode fazer a interface alternar continuamente entre rótulos de controle físico e toque. Ocultar HUD de jogo durante diálogos/cinematics e trocar os prompts E/TAB/ESC/G/1/2/3 por ações e ícones adequados.

### Adaptações específicas de telas

Na mochila, usar uma lista de retratos com rolagem e seleção por toque, sem comprimir os 12cartões da versão desktop em uma tela pequena. Manter as três vagas de alunos e a vaga de apoio sempre visíveis; abrir detalhes do personagem em painel expansível, com botão claro de equipar/remover. A seleção continua bloqueada fora da batalha final. No caderno e nos documentos, permitir rolagem, zoom e fechar sem depender de ESC. Nos puzzles, substituir arrastar muito preciso e hover por seleção/toque com confirmação quando necessário. Na direção do Fusca, o joystick controla direção e aceleração/freio por ações ou botões próprios; inspeções usam pontos de toque amplos com rótulos. Nas cutscenes, disponibilizar avançar/pular conforme as regras atuais, sem mostrar botões de ataque por cima da cena.

## Plano de execução

Estimativas iniciais para uma pessoa dedicada, sujeitas ao primeiro perfilamento em aparelho; não são prazos medidos nem compromisso de entrega.

| Etapa | Trabalho e arquivos principais | Entrega e condição de aceite | Esforço estimado |
| --- | --- | --- | --- |
| 1. Preparação | Instalar suporte Android; criar perfil Android separado; inventariar cenas, leituras de entrada e plugins; preservar saves e build Windows de referência. | APK inicial abre menu e carrega o primeiro capítulo em aparelho físico. | 1–2 dias |
| 2. Entrada por ações | Expandir VarginhaInputRuntime com modo Touch e ações de vagas AllySlot1/2/3; substituir atalhos diretos; implementar cancelamento de toque/foco. | Nenhuma tarefa obrigatória exige teclado ou mouse; controle físico continua funcionando. | 2–3 dias |
| 3. Joystick e HUD | Criar MobileHUD, MobileControls e SafeAreaLayout persistentes; OnScreenStick/OnScreenButton ou adaptadores equivalentes; layout canhoto, escala/opacidade e multitouch. | Andar, correr, atacar, mirar, esquivar, pular, interagir, usar lanterna e abrir menus com dois dedos sem conflito. | 3–4 dias |
| 4. Interfaces e puzzles | Adaptar menu, pausa, caderno, documentos, mochila12 alunos, diálogos, quiz e cada puzzle; migrar telas OnGUI críticas para Canvas/uGUI reutilizando a arte atual. | Texto legível e todas as escolhas acessíveis em 16:9,19.5:9,20:9 e tablet; rolagem e voltar consistentes. | 4–6 dias |
| 5. Campanha completa | Revisar os 14 capítulos, áreas extras, Fusca e inspeções, rituais, minijogos, cutscenes, aliados, batalha final e epílogo. | Percurso integral do início ao final apenas por toque; comandos contextuais corretos em cada cena. | 3–5 dias |
| 6. Desempenho e ciclo do app | Perfilar CPU/GPU/memória; reduzir overdraw, luzes/partículas e alocações; carregar/descarregar recursos por fase; compressão de áudio e texturas apropriada; autosave e pausa ao suspender. | Meta proposta 60 fps em aparelho intermediário e modo 30 fps em aparelho de entrada, após definir modelos reais; sessão prolongada sem vazamento/crescimento contínuo. | 3–5 dias |
| 7. QA e distribuição Android | Testes de instalação/atualização, voltar do sistema, falta de espaço, perda de foco, Bluetooth, saves e campanha integral; APK para testes e AAB para distribuição assinada. | Relatório de aparelhos, builds reproduzíveis e regressão Windows aprovada; nenhuma falha que bloqueie a campanha. | 3–5 dias |
| 8. iOS | Configurar módulo, projeto Xcode, assinatura, safe area e ciclo do app específicos; repetir QA e gerar distribuição de teste. | Build instalada e campanha validada em iPhone/iPad selecionados; depende de Mac e contas. | Estimar após Android |

Ordem de grandeza para Android:19–30 dias úteis, incluindo adaptação das telas e QA, com margem adicional se houver reestruturação de carregamento ou regressões antigas. Revisar a estimativa depois da etapa 1 e da primeira medição em hardware.

## Compatibilidade e desempenho

Definir a lista de aparelhos-alvo antes de prometer suporte mínimo. Unity 6000.6 documenta suporte Android 8/API 26 ou superior, mas esse é o limite do motor, não uma garantia de que este jogo já roda bem em qualquer aparelho dessa versão. Conferir APIs gráficas, formatos de textura e requisitos de empacotamento compatíveis com os aparelhos e com a distribuição escolhida. Fonte: [Unity 6000.6 — Android requirements and compatibility](https://docs.unity3d.com/6000.6/Documentation/Manual/android-requirements-and-compatibility.html).

Manter Point/nearest nos sprites e retratos. Perfil de textura mobile separado para evitar borrar pixel art com compressão inadequada. Reduzir pós-processamento/escala de render e efeitos em perfis de qualidade quando as medições exigirem. Medir batalha final com 3 alunos + 1 apoio, multidão da escola, trajetos de carro e trocas de cenas. Estabelecer orçamento de memória após medir o primeiro APK; não importar automaticamente todas as texturas de Resources durante o início do jogo. Evitar adicionar um sistema novo de assets antes de testar se o carregamento atual já atende ao orçamento.

Adaptar músicas e vídeos ao formato do dispositivo; testar interrupções de áudio, fones e retorno do app. Fazer autosave em transições seguras e antes de suspender, gravação atômica e recuperação de arquivo inválido. Não presumir que saves Windows migram automaticamente para o telefone; exportação/importação ou nuvem é uma funcionalidade separada a especificar.

## Testes que liberam a versão mobile

- Dois toques simultâneos: mover+atacar, mover+esquivar e mirar+atacar; soltar qualquer dedo não deixa entrada presa.
- Janela de diálogo/puzzle/mochila bloqueia ataque e movimento; tocar um aluno não aciona o cenário atrás da UI.
- Todos os puzzles, minijogos, portas, documentos e inspeções do Fusca são concluídos sem atalhos de teclado.
- Seleção dos 12 alunos apenas na luta final, limite 3 alunos + 1 apoio, recargas e equipe restauradas após salvar/carregar.
- Passar por toda a campanha em uma instalação limpa e depois continuar de cada capítulo salvo; morte/recomeço não apaga pistas indevidamente.
- Tamanhos pequenos, aparelhos com recorte, tablets, configurações de texto e esquema canhoto sem controles sobrepostos.
- Pausa por botão voltar, bloqueio de tela, chamada/interrupção, desconexão de controle e retorno após sistema encerrar o processo.
- Sessões de 30–60 min nos aparelhos escolhidos para medir aquecimento, bateria, fps e memória; simulador ajuda no layout, mas não substitui o dispositivo real.
- Repetir a regressão Windows. As seis falhas antigas identificadas na revisão atual devem ser triadas antes de uma versão de loja, com correção ou critérios de teste atualizados e justificados.

Primeiro marco prático: APK com menu, primeiro capítulo, joystick, interação e pausa. Só então expandir para mochila/puzzles e campanha inteira. O HUD mobile e o port não foram implementados na build Windows entregue nesta revisão.
