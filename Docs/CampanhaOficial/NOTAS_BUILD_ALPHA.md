# jogo_varginha_build_alpha

Versão alpha para testes de **O Segredo de Varginha**, Windows 64 bits, gerada com Unity 6000.6.0f1. Campanha de 14 fases em cinco atos.

## Como jogar

1. Baixe `jogo_varginha_build_alpha.zip` nos arquivos desta versão do GitHub.
2. Extraia **todo** o ZIP em uma pasta. Não execute o jogo dentro do arquivo compactado.
3. Abra `jogo_varginha_build_alpha.exe`. Mantenha o executável junto de `jogo_varginha_build_alpha_Data`, `UnityPlayer.dll` e das demais pastas e bibliotecas extraídas.
4. Escolha **Jogar → Iniciar campanha** para começar ou **Continuar** para retomar o save deste computador. Iniciar campanha substitui esse progresso.

O Unity Editor não é necessário para jogar a build. Os requisitos mínimos de hardware ainda não foram medidos. O save é local; esta alpha utiliza a identidade de armazenamento já existente no projeto para preservar a compatibilidade.

## Controle e teclado

Conecte o receptor USB do Harrow G808, ligue o controle em XInput e use o analógico esquerdo/D-Pad para selecionar. A confirma, B volta e Start pausa. No padrão, Y abre a mochila e Back/View abre o caderno. No teclado, G, Tab e Esc fazem essas ações. **O Fusca usa W no teclado e A no controle**, acompanhando o remapeamento de interação no gamepad.

Abra **Configurações → Controles → Controle** para ver o desenho e trocar botões. Selecione a ação, solte os botões e pressione o novo botão. Start/Esc cancela; os menus mantêm A/B e Start fixos. Há restauração dos padrões e opção de vibração. [Tutorial completo de controle](CONTROLE_XINPUT.md).

## O que mudou

- Fase Câmaras do Selo removida, com progressão e migração de saves ajustadas.
- Saída da casa da Ouzana e passagens do casarão, jardim e porão revisadas.
- Alunos alinhados às cadeiras, animação da pia e transição para 2026 revisados.
- Fusca lateral original preservado; novas vistas direcionais e faróis alinhados ao veículo. Carro e brilho antigos retirados do fundo.
- Suporte XInput, menus sem mouse, remapeamento visual e HUD que acompanha o dispositivo e os botões escolhidos.
- README e guia de puzzles atualizados.

## Roteiro de testagem

Confira entrada/saída das casas, ida e volta do porão, lavagem do rosto, estacionamento e saída da oficina. Alterne teclado/controle e remapeie mochila, caderno e interação. Teste pausa, desconexão e vibração no aparelho. Em caso de problema, registre fase, objetivo, passos e resultado em uma [Issue](https://github.com/fwbiodev7/segredo-varginha-game-teste-cutscene-gdd-ldd/issues), anexando a mensagem ou captura relevante.

Esta alpha é uma versão de desenvolvimento. Os testes automatizados e a abertura do executável cobrem os fluxos descritos no relatório; ainda é necessária uma jogada manual completa e a avaliação em outros computadores. A vibração física do Harrow precisa ser conferida no dispositivo.

## Reproduzir a build

No Unity, fora do Play, use **Varginha → Build → Alpha Windows x64**. O gerador inclui o menu, a sequência atual e as áreas adicionais necessárias; não inclui cenas de testes ou a fase removida. A saída fica em `Builds/jogo_varginha_build_alpha/`. O código e estas instruções são versionados; o pacote executável é disponibilizado nos assets da versão do GitHub.
