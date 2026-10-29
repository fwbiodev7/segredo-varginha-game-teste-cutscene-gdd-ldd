# Artes/UI — lote de 28/10/2026

Plano completo aprovado pelo usuário; lote implementado pelo subagente `artes_ui_auditoria`. Nenhum bitmap redesenhado, nenhum save/cena/trigger alterado neste lote.

| Arquivo | Alteração |
| --- | --- |
| `Assets/Scripts/Game/Varginha/VarginhaMainMenu.cs` | A exibição usa somente as primeiras 576 de 724 linhas do PNG, retirando o rodapé CASO VARGINHA sem modificar o arquivo, shader ou rotação. Largura/posição superior preservadas; altura recalculada conforme aspecto. Frases passam de fonte 8/alpha 0,85 a fonte 10/alpha 0,98, em cinza frio, numa linha própria abaixo da marca. |
| `Assets/Scripts/Game/Varginha/Experiment/CampaignGuidance.cs` | Substituição do quadrado ciano 6×6 por exclamação amarela contornada em máscara nativa 15×15. Textura RGBA criada uma vez, filtro Point, sem mipmaps; reutilizada entre mapas. Mantidos `Target`, condições, posição de referência, dicas e triggers. Nenhuma alteração do marcador de jogador no mapa ou no diagrama de controles. |
| `Assets/Scripts/Game/Varginha/Experiment/CampaignPuzzleDesign.cs` | Associações de 1898/Fábio e hipótese da criatura recebem painel vertical ampliado, cabeçalhos compactos e cartões de 64px em vez de 39px. Nome, prova, destino e vínculo têm áreas próprias sem sobreposição. Cada destino tem DESFAZER independente; conferir continua separado. Soluções, IDs e estados de save preservados. Título de 1898 simplificado e instrução genérica sem fornecer pares corretos. Comparação de Arquivos Alterados e memória conservam o layout anterior neste lote. |
| `Assets/Tests/PlayMode/CampaignPuzzleDesignPlayTests.cs` | O teste existente `FiveDifferentPuzzleInterfacesAcceptControllerAnswersAndPreservePartialProgress` foi adaptado às novas coordenadas. Verifica botão DESFAZER real no primeiro vínculo da fase 13 e posterior religação antes do save/reload. |

## Verificação feita

- `git -c core.safecrlf=false diff --check` dirigido aos quatro arquivos: código 0, sem falhas de whitespace.
- Conferência estática da geometria: quatro linhas `y=240+70*row`, altura 64, fim da última linha em 514; conferir em 530 e voltar do controlador existente em 570, sem interseção.
- PNG da logo inspecionado; texto turquesa do rodapé começa abaixo da linha recortada. Fonte original, alpha e shader permanecem intactos; o recorte é somente de apresentação.

Não foram executados Unity, testes, builds ou sessão visual interativa pelo subagente. Estado: **implementado, aguardando QA do root**. Validar menu e fases 13/14/18 em 1280×720 e 1920×1080, inclusive cartões não ligados, incorretos/corretos, desfazer, teclado/controle e save/reload. O root serializa a execução na cópia isolada de QA.

## Continuidade

Não iniciar novo lote neste subagente: root informou aproximação do limite de uso e está preparando compilação/QA. Não há código parcialmente escrito nem dependências faltantes deste lote. Trabalhos de escola, colisões, áudio, narrativa e assets de cenário pertencem ao root/outro subagente.
