# Oizlede: Fragmento Perdido — revisão de 29/10/2026

Candidata Windows x64 **0.5.3-beta** gerada. Verificações automatizadas aprovadas e inicialização do menu confirmada.

## Abrir o jogo

Extraia a pasta completa do ZIP e execute **Oizlede - Segredo de Varginha.exe**. Mantenha ao lado dele a pasta **Oizlede - Segredo de Varginha_Data**, UnityPlayer.dll, MonoBleedingEdge e os demais arquivos distribuídos. A identidade interna do aplicativo foi preservada para manter os saves existentes.

## Alterações integradas

| Itens do pedido | Implementação e validação |
| --- | --- |
| 1–2 | Cartões de causas de1898 e datas de Fábio mais altos, campos separados, indicação de vínculo e Desfazer individual. Respostas e IDs mantidos. |
| 3 | Objetivo ativo com exclamação amarela contornada15×15; marcador do jogador preservado. |
| 4 | Reação à luz no prólogo sem bloquear movimento; ruído e reflexos graduais, flag persistente e encerramento na manifestação. |
| 5,15 | Colisões compartilhadas preservadas. Percursos reais dirigidos de casa, escola, biblioteca, mata, igreja, casarão/porão e veículo; auditoria adicional de contatos físicos dos móveis. |
| 6–9 | Rotina, chave, primeira ida ao Fusca, retorno/mochila, desaparecimento e investigação em ordem. Papéis condicionados ao estado. Aproximação da mesa corrigida; embarque imediato sem abrir a porta. |
| 10 | Música “O Edelzio” no carro da rua na chegada externa à escola. Fonte espacial segue o veículo, atenuação por distância, Doppler suave, volume de música, pausa e encerramento. |
| 11–13 | Escola mantém quatro etapas. Comparar usa escolha única de memória; Concluir não entrega origem nem reabre recortes. Leitura obrigatória restrita a Ordenar; vínculos e estado legado preservados. |
| 14,16 | Livro, globo e mural da biblioteca reaproveitados; foco localizado em regiões investigáveis. Três vestígios distintos na mata, com pontos correspondentes e sem alterar a ponte. |
| 17 | Igreja antiga íntegra, ampliada30%, acesso sul e pista ajustados. Fuga e entrada verificadas com controlador; Fábio somente dentro e perseguição encerrada na entrada. |
| 18 | Índice/livro existente, ficha e inscrição física da igreja. Mantidos ano1996, símbolo ÂNCORA e registro23, campos e montagem originais. |
| 19 | Saída da oficina usa Rigidbody2D e varreduras físicas: ré pelo portão, giro na rua e término antes da parede do limite. Obstrução interrompe a saída e devolve controle; nova tentativa funciona. Direção, inércia, estacionamento e faróis preservados. |
| 20 | Arquivos Alterados embaralha alternativas por tentativa, mantendo IDs, seleção, ordem salva e solução. |
| 21–23 | Planta, foto, chave antiga e evidências do porão visíveis. Apoios de planta/páginas/chave ajustados visualmente. Chave coletada some e continua coletada após reload. Evidências opcionais continuam opcionais. |
| 24–26 | Logo dessaturada sem rodapé CASO VARGINHA, quatro referências rotativas cinzas; EXE renomeado sem mudar a identidade dos saves. |
| 27–30 | Recursos e atlas reaproveitados, importação Point/sem mipmaps/compressão inadequada, recursos gráficos cacheados e áudio com ciclo de vida/pausa. |
| 31–36 | QA automatizado, capturas, percurso físico, relatório, build independente e ZIP com hashes/CRC. Resultados finais abaixo. |

## QA registrado

- EditMode final em29/10: **458/458 aprovados** (`final-editmode-29-r2.xml`).
- Player dirigido em29/10: **4/4 aprovados** para igreja ampliada/perseguição, encontro interno, casarão/porão com coleta/reload e saída física da oficina com obstáculo e repetição.
- Primeiro lote de29/10: casa até escola e consumo de itens aprovados; foi encontrada oscilação do condutor automático de testes na fuga. O condutor foi ajustado ao deslocamento de15cm por passo; a tolerância de colisão e as assertivas de interação/progressão foram preservadas. A perseguição não foi facilitada para fabricar aprovação.
- Regressão Player inicial: **341 executados, 327 aprovados e14 falhas**. Após correções, o primeiro reteste executou18 casos (14 aprovados/4 falhas); o reteste seguinte aprovou4/4. A investigação doméstica adicional também aprovou.
- Cobertura consolidada Player: **342 casos distintos aprovados em seus resultados mais recentes, sem falhas pendentes**, conforme `player-consolidated.json`. Esse total reúne execuções e retestes; não representa uma única execução completa verde. O último ajuste de apoio visual dos documentos foi revalidado em Player: **1/1 aprovado** (`final-props-29.xml`), incluindo coleta, interações e reload no casarão/porão.
- Resoluções: capturas1280×720 e1920×1080; resolução principal1920×1080. Conferência visual documentada nos relatórios de QA.
- Build Windows: **Succeeded, zero erros, dois avisos** de API Unity obsoleta em código existente. Preservadas19 cenas e14 capítulos. Menu do executável final iniciou; três JSONs de save permaneceram idênticos por SHA256.
- Conferência final com mouse: **pendente de fechar aviso de firewall do Windows para playerwithtests**, que cobre a janela. A ferramenta não alterou a permissão de rede. Testes automatizados do menu e configurações foram aprovados.
- ZIP gerado pelo empacotador com manifestos SHA256 e verificação CRC; resultado e hash em `windows-package.json`.

## Roteiro para dublagem

`ROTEIRO_PARA_DUBLADOR_2026-10-29.txt`: duas falas curtas para o repórter da abertura e13 entradas do narrador, com indicações de cena. O roteiro respeita a abertura atual de16 segundos e a ordem das revelações. Vozes novas aguardam gravação; este material não substitui os áudios existentes.

As contagens pertencem aos XMLs de cada revisão. A auditoria de todos os móveis usa um probe físico complementar; ela não equivale a percorrer a campanha inteira com o personagem. Os percursos do controlador real estão identificados separadamente nos testes e neste relatório. Não houve jogada humana integral e ininterrupta da campanha.

A chave e as páginas tiveram seus apoios visuais conferidos nas novas capturas. Na captura final do banco, o personagem encobre o documento: a oclusão é normal pelo ator, mas esse enquadramento não comprova sua legibilidade desobstruída. Posição/tamanho foram ajustados ao assento e a interação física foi aprovada.

## Arquivos e cenas

Principais arquivos: CampaignAncientChurch, CampaignPhysicalEvidence, CampaignWorkshopVehicle, CampaignExpansionController, CampaignHouseSequence/Investigation, CampaignSchoolSequence/Investigation, CampaignPuzzleDesign, VarginhaCampaignPhase1, VarginhaMainMenu e CampaignSchoolMusicCar. Assets canônicos em Assets/Resources/Varginha/Revision20261028, preservando os originais.

Cenas afetadas por sistemas compartilhados: menu, prólogo/casa, escola, biblioteca, mata, igreja, oficina, casarão, porão e interfaces investigativas posteriores. Estrutura preservada:14 capítulos e19 cenas de build. Evidências: output/revisao-final-2026-10-29 e relatórios anteriores de28/10. Saves reais do usuário preservados; os testes usam identidade de produto isolada.
