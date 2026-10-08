# Relatório da sprint - O Segredo de Varginha

Período: 2 a 8 de outubro de 2026. Apresentação: sexta-feira, 9 de outubro de 2026.

## Resultado atual

O remaster está integrado ao projeto Unity local. A essência de aventura, mistério, terror investigativo e pixel art foi preservada. A campanha atual mantém 14 fases em cinco atos, os diálogos, as mecânicas, os sprites, o Fusca e os controles existentes.

Melhorias: fade/crossfade contextual, movimento curto de câmera em deslocamentos, interferência paranormal rara, enquadramentos de pistas, áudio antecipado e crossfade de ambientes, atenuação em diálogos, sons posicionais, variação discreta da luz e respeito à pausa e ao movimento reduzido. Não houve migração de render pipeline nem reconstrução das cutscenes em Timeline/Cinemachine. Zoom contínuo foi evitado para preservar a escala da pixel art.

## Trabalho da semana

| Data | Área | Entrega | Evidência |
| --- | --- | --- | --- |
| 02/10 | Mapas e capítulos | Remaster dos mapas, expansão dos capítulos 6 a 10, portais da casa e animações da mochila. | 1c1d0ad; c8ac550 |
| 03/10 | Arte e investigação | Interiores, clarão alienígena, animações, investigação escolar, Tombo, escala na igreja e Ouzana. | 39da4b4; e97aa4c; 1b43d2c |
| 04-05/10 | Continuação e batalha | Integração dos capítulos posteriores, navegação, combate da manifestação e entrada/saída cinematográfica do chefe. | ecabd92; 479328d |
| 07/10 | Campanha e alpha | Sequência consolidada em 14 fases, puzzles e usabilidade, luz dos personagens, remapeamento, Fusca original, build alpha e publicação do download/site. | 97c8ef9; 910a790; a9f7e37; 412cd5f |
| 08/10 | Remaster atual | Transições por contexto, enquadramentos nas pistas, continuidade de áudio, efeitos posicionais, atmosfera discreta, testes, prints e apresentação atualizada. | Alterações locais desta entrega |

## Validação executada nesta entrega

Unity 6000.6.0f1, renderização BuiltIn. Compilação sem erros de C# na rodada final.

| Bateria | Total | Aprovados | Falhas |
| --- | ---: | ---: | ---: |
| Edit Mode completo final | 241 | 229 | 12 |
| Comparação das duas classes de sprites no código original | 16 | 4 | 12 |
| Play Mode inicial da campanha | 47 | 46 | 1 |
| Rodada isolada do remaster | 4 | 3 | 1 |
| Rodada de capturas do remaster | 4 | 3 | 1 |
| Verificação final: remaster, controles e iluminação | 14 | 14 | 0 |
| Casos distintos de Play Mode, resultado mais recente | 47 | 47 | 0 |

As 12 falhas de Edit Mode são exatamente as mesmas reproduzidas no código original, sem o remaster: nove verificações do alinhamento dos alunos, diferença de altura de Anna/Ana e duas verificações de pivô/tamanho de Edelzio. A arte não foi alterada para satisfazer esses contratos antigos. O conjunto completo de Edit Mode continua com essas pendências.

A primeira rodada de Play Mode falhou em um teste novo que esperava liberar o personagem antes de dispensar o diálogo de chegada. O teste foi corrigido para verificar o bloqueio durante o diálogo e a liberação ao fechá-lo. Nas rodadas isoladas, o teste de pausa do ambiente revelou uma disputa real: o sistema global de volume sobrescrevia os canais de crossfade. A integração foi corrigida para o ambiente aplicar o mesmo controle de música sem dois escritores do volume. Remaster, controles e iluminação foram reexecutados e aprovados na verificação final; todas as rodadas com falha permanecem registradas.

Cobertura: paredes e móveis, profundidade e luz dos personagens, escola e investigação, progressão/saves, laboratório da Ouzana, estacionamento e partida do Fusca, aliados, chefe, calibração/travessia/epílogo, gamepad simulado, remapeamento, pause/cancel, enquadramento, crossfade, recursos de captura e continuidade dos ambientes. Nenhuma aprovação é atribuída a uma jogada manual completa ou a teste físico do controle.

## Entregas e estado de publicação

- Código do remaster e testes: alterações locais, sem novo commit ou publicação.
- Cinco capturas reais: `Prints/01_Prologo_1996.png` até `Prints/05_Transicao_Casarao.png`.
- Apresentação de sete slides: PPTX editável e PDF, baseada no PDF fornecido pelo usuário. Cores, estrutura e imagens de referência preservadas; texto refeito como objetos editáveis com Arial disponível no ambiente.
- Relatório em Markdown e PDF.
- Aviso sonoro de conclusão: WAV de volume alto preparado para reprodução ao finalizar todos os arquivos.
- A alpha publicada em 07/10 é a entrega anterior e não inclui automaticamente estas alterações locais.

## Pendências reais

Revisar os contratos dos testes antigos de sprites; medir desempenho e carregamento no computador do jogador; conferir o controle físico e áudio; fazer uma jogada manual completa; preparar/publicar uma nova alpha quando autorizado. Não foram gerados novos assets artísticos, substituídos sons originais ou alterados puzzles para esconder falhas.

Evidências atuais: `results-summary.json`, os quatro relatórios XML e seus logs nesta pasta. Histórico da semana: Git e `Docs/CampanhaOficial/RELATORIO_REVISAO_20261007.md`. Os resultados de 07/10 são registros históricos, não uma execução adicional desta entrega.
