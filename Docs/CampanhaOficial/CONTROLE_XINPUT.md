# Controle — Redragon Harrow G808 e outros XInput

O jogo usa **Unity Input System 1.20.0**, abstração `Gamepad`, sem depender do nome ou fabricante do controle. O Harrow G808 deve estar conectado pelo receptor USB 2.4 GHz e reconhecido pelo Windows em **XInput**. Não é necessário instalar um pacote adicional no projeto. Teclado e mouse permanecem disponíveis.

## Como testar no Unity Editor

1. Conecte o receptor USB e ligue o Harrow. Configure o modo XInput conforme o manual do seu modelo; não presumimos uma combinação de botões específica do firmware.
2. No Windows, abra `joy.cpl` e confira se os botões e analógicos respondem.
3. Abra o projeto no **Unity 6000.6.0f1**. Em **Window → Analysis → Input Debugger → Devices**, confirme que aparece um `Gamepad` / `XInputControllerWindows`. Se aparecer apenas `Joystick`, o dispositivo não está sendo apresentado como XInput.
4. Abra `Assets/Scenes/Menu_MisterioDeVarginha.unity`, entre em Play e dê foco ao painel Game. Use analógico/D-Pad para selecionar **Nova história**, **Configurações**, abas, opções e botões; A confirma e B volta. Ajuste um slider com esquerda/direita.
5. Na campanha, verifique movimentação, diálogos e puzzles com A. Abra o caderno com Back/View e acesse **Preciso de uma dica**. Abra a mochila com Y: analógico/D-Pad seleciona, A equipa/remove, RB alterna abas e B/Y fecha.
6. Ao chegar ao Fusca, use A para entrar/estacionar/examinar/confirmar. Volte ao teclado: o prompt deve mudar para **W**. E, Enter e cliques não confirmam as interações do carro.
7. Na batalha, teste X (combo), B (esquiva quando liberada), LB (aluno), RB (apoio), analógico direito (mira) e Start (pausa). LT corre; RT alterna a lanterna equipada; L3 pula somente onde o jogo já permite.
8. Ligue **Vibração** nas configurações e ajuste a intensidade. Teste um impacto/susto, pausa, perda de foco e desconexão. Os motores devem parar ao terminar o efeito ou nesses eventos. Sem suporte, o efeito é ignorado.
9. Alterne entre controle, teclado e mouse: os prompts devem mudar na mesma sessão. Repita com outro controle XInput. Não manter o analógico deslocado ao alternar dispositivos evita eventos simultâneos competindo pelo último comando.

## Remapear o controle

Em **Configurações → Controles → Controle**, selecione a ação ao lado do desenho, solte o botão usado para confirmar e pressione o novo botão. O mapeamento é salvo automaticamente. Quando um botão já controla outra ação, as duas ações trocam de botão. Movimento e mira preservam os analógicos; A/B dos menus e Start ficam fixos para manter a navegação disponível. Use **Start / Esc** para cancelar, ou aguarde 15 segundos. A captura também termina ao desconectar o controle ou perder o foco.

**Restaurar botões do controle** recupera os padrões sem alterar as teclas. O Fusca acompanha o botão de interação escolhido; **W continua obrigatório no teclado**. Pausa, mochila e caderno mostram o dispositivo e a configuração atual em tempo real. O remapeamento está disponível também nas configurações da pausa.

[Captura da tela no Unity](../QARevisao20261007/Controle_remapeamento.png).

## Input Actions

O asset `VarginhaRuntimeInputs` é construído pelo código existente de remapeamento e disponibilizado por `VarginhaInputActions.Asset`. Não havia um arquivo `.inputactions` antes desta revisão. O JSON em [INPUT_ACTIONS.json](INPUT_ACTIONS.json) documenta o asset efetivamente gerado; o runtime usa as preferências salvas para construir os bindings de teclado/mouse e aplica os overrides de gamepad nas mesmas ações, sem criar um segundo sistema.

| Mapa | Ações |
| --- | --- |
| Gameplay | Move, MoveUp/Down/Left/Right, Run, Interact, Attack, AllyCommand, Dodge, Jump, SupportCommand, Aim, CarInteract, Journal, Inventory, Flashlight, Pause |
| UI | Navigate, Submit, Cancel, KeyboardActivity, MouseActivity, PointerActivity |

O mapping completo está no [README](../../README.md#controles). `CarInteract` tem somente `<Keyboard>/w` e o botão de controle associado a `Interact` (A por padrão). Os overrides usam `ApplyBindingOverride`; as preferências ficam em `Varginha.Controls.Gamepad.<ação>`. A deadzone mínima é **0,20**, máxima **0,95**. Os prompts usam o último comando com atividade real; controles abaixo da deadzone não substituem o dispositivo ativo. A navegação possui repetição em tempo real e funciona também com o jogo pausado.

`VarginhaRumble.Play(low, high, seconds)` aplica intensidade configurável, duração limitada a dois segundos e usa `Gamepad.current.SetMotorSpeeds`. `Stop` é chamado em pausa, troca de cena, desconexão, perda de foco, encerramento e desativação do serviço. O efeito segue os eventos existentes de impacto e câmera, sem adicionar ações ao jogador.

## Cobertura e limite

Os testes usam um `Gamepad` simulado para verificar bindings, deadzone, troca de prompts, menus reais e encerramento de vibração. Isso não substitui o teste físico do receptor e dos motores do Harrow. O usuário confirmou nesta conversa que o controle físico funcionou no jogo. Os testes automatizados verificam o novo remapeamento; a resposta dos motores no aparelho deve ser conferida separadamente.

Referências: [Unity: gamepads e haptics](https://github.com/Unity-Technologies/InputSystem/blob/develop/Packages/com.unity.inputsystem/Documentation~/gamepad-haptics.md), [dispositivos suportados](https://github.com/Unity-Technologies/InputSystem/blob/develop/Packages/com.unity.inputsystem/Documentation~/supported-devices-reference.md).
