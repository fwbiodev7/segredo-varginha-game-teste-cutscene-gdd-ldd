# Entrega final — 21/10/2026

Candidata Windows **0.4.2-beta**, com a abertura animada, cabine redesenhada a partir da fotografia do usuário e substituição aprovada do puzzle de Fragmentos. Base: `117d2eb`. Documentação, capturas e slides atualizados nesta pasta e em `output/daily-sprint-2026-10-20`.

## O que foi concluído

- Abertura adulta: depósito da chave com gesto/trajetória/repouso, desaparecimento fora do campo de observação e descoberta da caixa preservam a investigação e a ordem narrativa. Páginas continuam jogáveis.
- Fusca: embarque, janela com cenário em movimento, quatro frames originais de direção, painel/rádio animados, pane/frase infantil/ignição quente, mão soltando a chave e reinício espontâneo. A dublagem foi adiada; não foi gerada voz sintética.
- Arte final da cabine: estrutura azul e banco de vinil preto em uma camada; painel horizontal marrom/radio antigo em outra, baseados na foto real enviada. O mesmo Edelzio está maior, com painel junto ao volante original, sem haste longa ou segundo volante. Close da ignição com antebraço exposto e três poses; a chave permanece inserida.
- Glitch: focos que variam posição, comprimento, opacidade e intervalo, fragmentos da imagem deslocados e ecos/partículas verdes e violetas. Há áreas de respiro e intervalos de leitura; o modo reduzido mantém assinatura discreta estável. Nenhuma entidade ou símbolo novo é revelado.
- Desembarque: usa `IllustratedMaps/School.png` e o layout da fase 4. A caminhada segue uma rota válida das colisões da fase e termina no spawn autoral. Corpo, renderizadores e controle são restaurados na passagem à jogabilidade.
- **Novo puzzle de Fragmentos:** três bifurcações em um mapa com terreno e pistas visíveis, prévia antes de traçar, desfazer/recomeçar, feedback da primeira contradição e três dicas. Substitui ordenar marcos/girar setas. O prefixo correto e o progresso parcial são salvos; saves já concluídos conservam a mata e capítulos posteriores.
- Daily simplificada em dez slides, com capturas reais, notas, nove transições e espaço final para dúvidas, opiniões e feedbacks. A retrospectiva anterior foi preservada como HTML separado.

## Arquivos e cenas afetados

Abertura: `CampaignHouseSequence`, `CampaignStory`, `CampaignFuscaCinema`, `CampaignSignalGlitch`, `CampaignCinemaTextures`, `ExperimentFrameComposer`, `VarginhaCampaignStage` e `VarginhaPlayerActionAnimation`; assets/importer/shader em `CinemaOpening` e `Editor/Testing`.

Puzzle: novo `CampaignTrailPuzzle`, integração em `CampaignPuzzleDesign`, `CampaignExpansionState` e `CampaignExpansionController`; textos de `CampaignGuidance`, `CampaignHints` e `CampaignJournal`. Ver [projeto e regras](../../Docs/CampanhaOficial/PUZZLE_FRAGMENTOS_BIFURCACOES_2026-10-21.md) e [relatório de arte](technical-report.md).

Testes atualizam o contrato retirado de ordem/rotação e verificam a mecânica substituta, em vez de manter asserções obsoletas. Auxiliares de gravação/callback pertencem apenas ao assembly de testes. Build/distribuição: `VarginhaDailyBuild`, `Tools/QA/package_windows.py`; slides: HTML, fonte, QA e roteiro da daily.

Cenas afetadas em runtime: fase adulta da chave/caixa (2), chegada à Industrial (4), Fragmentos/biblioteca (6), gate de acesso à Mata (7) e interferência compartilhada do prólogo. **Nenhuma cena `.unity` de produção foi regravada.** A candidata mantém os 14 capítulos e as 19 entradas de build existentes.

## Verificação final

| Verificação | Resultado | Evidência |
|---|---|---|
| EditMode completo | 367/367, sem falha ou skip | `editmode-trail-photo.xml/json` |
| Player Windows direcionado | 5/5, sem falha ou skip | `player-delivery-final.xml/json` |
| Daily | 10 slides, 9 transições, sem overflow/erro JS; navegação/notas/impressão/redução de movimento aprovadas | `slides-qa.json` |
| Prévia silenciosa | 103 quadros reais; reprodução, reinício e carregamento aprovados | `preview-qa.json` |
| Build Windows | Succeeded; 0 erros, 1 aviso | `windows-build.json` |
| ZIP | CRC e manifesto SHA-256 verificados; assemblies de testes excluídos | `windows-package.json` |
| Preservação | 3 saves e bytes originais do URP restaurados por hash; configuração e cenas temporárias de QA retiradas | `preservation.json` |

Os cinco casos nativos cobrem: sequência completa com pausa/retorno de controle; pulo/redução sem perder itens; arte/alpha na GPU; exploração física e três evidências na biblioteca; puzzle por gamepad com prévia, desfazer, reload parcial, solução e gate da Mata. A solução única entre oito rotas, saves antigos/malformados, dicas e preservação do prefixo são cobertos pelos testes de regras. A rota casa-escola já passou na regressão anterior documentada; os novos casos de cinema começam com a preparação cumprida.

O aviso da build vem do pacote `Unity.Pipeline`: ausência de `RuntimePipelineConfig`, com desativação desse pacote no Player. A build foi gerada após restaurar o perfil original de produção. A placa local registra também aviso de fence DX11; as capturas e casos nativos concluíram, mas não constituem medição de FPS mínimo.

Tentativas falhas/interrompidas foram preservadas e não contam como aprovação: callback nativo/referência do gravador de QA, espera de captura em Editor batch, leitura de GPU com `-nographics`, endpoint arredondado da rota e uma build interrompida para corrigir textos cortados. A trava residual dessa instância de QA foi retirada após verificar seu encerramento; não houve edição de código para esconder falha. O corte de textos foi corrigido em frases de duas linhas e a rodada nativa final foi repetida com capturas inicial/prévia/final.

## Limites e pendências reais

Playtest com pessoa nova, avaliação da equipe e outro PC **ainda não aconteceram**. A dificuldade mediana do puzzle é uma intenção a validar; não foi atribuída aprovação humana às artes ou ao balanceamento. Dublagem adiada. Cutscenes posteriores e finais permanecem no roteiro e nas decisões editoriais da produção futura. Ver [pendências](PENDENCIAS.md) e [roteiro de playtest](../../Docs/CampanhaOficial/PLAYTEST_2026-10-21.md).

O ZIP é uma entrega local, sem publicação automática como GitHub Release. Os arquivos preexistentes do usuário (URP e Recovery) foram preservados e excluídos deste commit. Os logs completos ficam em `output/agent-logs`; a compactação mede bytes, não tokens, e nenhum modelo local foi declarado ativo.
