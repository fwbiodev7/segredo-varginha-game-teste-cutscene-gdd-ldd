# Menu Oizlede e continuidade — 28/10/2026

Plano inspecionado e aprovado pelo usuário nesta sessão. A atualização troca o título textual do menu pela logo principal Oizlede: Fragmento Perdido, neutraliza os textos em tom de papel/amarelo e consolida a correção da floresta que ainda não tinha build entregue em 27/10.

## Arte e interface

O PNG recebido em Downloads foi preservado. Seu quadriculado era opaco. A ferramenta integrada `image_gen` preparou uma cópia da logo principal com alpha real, sem as quatro variações inferiores. O resultado foi inspecionado e integrado em `Assets/Resources/Varginha/MenuLogoOizlede.png`. É uma extração gerada, não uma cópia declarada idêntica pixel a pixel. Prompt e procedência: `output/menu-continuation-2026-10-28/logo-provenance.json`.

A textura usa filtro Point, sem mipmaps/compressão e limite de 4096 para conservar as dimensões de 2172 × 724. O menu carrega a imagem uma vez e a desenha no canvas já existente, acima dos botões. Após a revisão do usuário, a marca foi reduzida de 560 para 470 unidades e alinhada à coluna dos botões. `MenuLogoGrade.shader` reduz a saturação para 22% e aplica o tom frio na exibição, conservando alpha e coordenadas. A imagem tratada é calculada uma vez por abertura, reutilizada e liberada ao sair. O PNG não foi alterado para esse tratamento. Fundo animado, rádio, música, navegação e suspense continuam no sistema existente.

As quatro frases das variações recebidas são desenhadas separadamente, em cinza discreto abaixo da logo. `MenuCaptionRotation` escolhe uma frase a cada entrada no menu e salva a última seleção em PlayerPrefs, sem consumir o gerador aleatório do gameplay. A próxima entrada, inclusive após reiniciar o jogo ou voltar de um painel como Configurações, evita repetir a frase anterior.

`PixelMenuTheme` centraliza o cinza claro para texto e o cinza mais discreto para legendas. `ExperimentGUI`, diálogos da campanha e HUD compartilham a mesma paleta. O título amarelo do HUD legado também passa a cinza. Cores funcionais de destaque, erro, acerto e combate permanecem específicas dos respectivos sistemas. Textos narrativos e regras não foram reescritos.

## Continuidade de 27/10

O redesign e o embaralhamento escolar estavam entregues na 0.5.1. O ET de seis quadros e o padre fixo estavam no código e tinham resultados de testes, mas faltavam a build 0.5.2 e seu pacote. Esta candidata reúne essas mudanças com o novo menu.

A validação com uma pessoa que desconhece os puzzles e o teste em outro computador continuam externos. Automação com mouse não equivale a playtest humano.

## Arquivos e cenas

- Menu: `VarginhaMainMenu.cs`, `MenuCaptionRotation.cs`, `MenuLogoOizlede.png`, `MenuLogoGrade.shader` e metas.
- Paleta: `PixelMenuTheme.cs`, `ExperimentGUI.cs`, `VarginhaGameHUD.cs`, `GameHUD.cs`.
- Entrega: `VarginhaDailyBuild.cs`, `Tools/QA/package_school_20261027.py` e `Tools/QA/menu_mouse_state_20261028.py` (cópia/restauração de saves da conferência).
- Evidências adicionais nos testes existentes: `MenuCinematicPlayTests.cs` e `CampaignSchoolPatternsPlayTests.cs`. Regras de rotação: `MenuCaptionRotationTests.cs`.

Nenhum `.unity` ou mapa original foi editado. Efeito visual em runtime: cena `Menu_MisterioDeVarginha` e textos das interfaces compartilhadas da campanha. A candidata inclui a correção anterior da floresta (ID 7) e mantém a escola (4) e a biblioteca (6).

## Validação e pacote

| Recorte | Resultado |
| --- | --- |
| `menu-captions-editmode-final.xml` | 451/451, zero falhas/ignorados |
| `menu-school-forest-player.xml` | 21/21, zero falhas/ignorados: menu, escola e floresta |
| `menu-captions-player-final.xml` | 13/13, zero falhas/ignorados: menu após dessaturação e rotação |

Os dois recortes principais de Player têm 12 casos em comum: são 22 casos distintos, não 34. A rodada adicional `menu-integrated-player.xml` repetiu dois casos durante o acabamento intermediário e não foi somada como casos novos. A última rodada confirma a troca de frase ao recarregar a cena, redução da cromaticidade e conservação de alpha/contornos da logo no render real.

Uma tentativa anterior contou 448 testes porque o novo arquivo de testes tinha GUID inválido em sua meta e foi ignorado pelo importador. A meta foi corrigida, e o resultado final de 451 inclui os três casos novos de rotação. Essa tentativa anterior não comprova a validação desses casos. Logs e versões visuais intermediárias foram preservados em `output/menu-continuation-2026-10-28/` e `attempts/`.

`menu-panel-return-player.xml` repetiu um caso após o ajuste de retorno de painéis e passou 1/1. Não é um caso distinto adicional.

## Interrupção da conferência e estado da build

A candidata 0.5.2 compilou com 19 cenas, zero erros e dois avisos. O ZIP de 316.428.735 bytes teve CRC e hashes críticos aprovados; 376 arquivos de fontes/recursos foram comparados com a cópia QA. Essa candidata contém a dessaturação e a troca de frase ao entrar/reabrir o jogo, mas antecede o último ajuste para trocar a frase ao voltar de Configurações. O código desse ajuste passou no Player, porém ainda precisa de recompilação e novo empacotamento. O ZIP existente não deve ser apresentado como correspondendo a todo o estado final do workspace.

Pelo mouse real, abrir/voltar de Configurações e Continuar até a escola foram conferidos. O aviso de firewall do Player de testes interrompeu o teclado; o usuário o fechou manualmente. Antes de concluir a interface escolar com mouse, o usuário interrompeu Computer Use com Escape. A automação não foi retomada após esse sinal. O executável de teste foi encerrado e os três arquivos JSON originais foram restaurados com hashes conferidos (`mouse-save-backup/manifest.json`). O save de teste não substituiu permanentemente o progresso do usuário.

Continuam pendentes: conferência completa da escola com mouse, nova build/ZIP com o retorno de painéis e validações externas com pessoa leiga/outro computador. Capturas automatizadas do menu e da escola estão em `output/menu-continuation-2026-10-28/`. Não houve publicação externa.
