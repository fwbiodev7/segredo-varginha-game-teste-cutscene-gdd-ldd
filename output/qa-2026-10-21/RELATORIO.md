# Fechamento técnico — 21/10/2026

Continuação da abertura adulta aprovada em 20/10. Base inicial: `44a80af`, branch `codex/campanha-15-fases-e-site`. A dublagem foi adiada pelo usuário; testes externos ainda não aconteceram.

## Resultado da implementação

- Dica de higiene aponta à pia da cozinha, junto ao fogão, conforme o mapa adulto aprovado. Dicas agora têm três níveis distintos em cada etapa: higiene, café, notebook, balcão, mochila, investigação, caixa, páginas e saída. Consultar dicas não muda o progresso.
- Poses antigas de soco usam o mesmo alinhamento corporal da caminhada, sem editar sprites ou mudar sua escala.
- Aliados ocultam imediatamente a sombra ao mudar apresentação. A morte de Edelzio cancela ataques pendentes mesmo com o tempo pausado; uma pausa com o personagem vivo continua preservando o ataque.
- Corrigida uma falha exclusiva do Player: adicionar `PolygonCollider2D` ao lado do sprite provocava leitura automática de textura não legível. O sprite fica momentaneamente desvinculado durante a criação e aplicação do polígono já definido no manifesto. Depois é restaurado. Vértices, arte, GUIDs e compressão permanecem os existentes.
- Testes legados passaram a usar layouts, portraits e ataques A/B vigentes. Fixtures de componentes recebem uma cena isolada, evitando menus/diálogos deixados pelos testes da campanha. Física começa o teste de queda acima do chão; perseguição começa dentro do alcance. Os limiares visuais conservam a variação das poses originais e uma tolerância de um nível RGB24 na leitura de escuridão.

Não foram acrescentados capítulos ou puzzles novos hoje. Foram concluídas e verificadas as pendências técnicas da missão integrada ontem: preparar-se para a escola → chave no balcão → mochila/desaparecimento → três pistas e dedução → caixa/caderno → páginas rasgadas → Fusca → escola/Renan. O roteiro novo de cutscenes aguarda aprovação do usuário para uma etapa posterior; nenhuma nova cutscene foi implementada hoje.

## Arquivos e cenas afetados

| Grupo | Arquivos |
|---|---|
| Estado/dicas | `Assets/Scripts/Game/Varginha/Experiment/CampaignHouseInvestigation.cs` |
| Colisão no Player | `Assets/Scripts/Game/Varginha/Experiment/CampaignIllustratedMaps.cs` |
| Apresentação/ataques | `Assets/Scripts/Game/Varginha/VarginhaReferenceSprites.cs`, `VarginhaStudentAlly.cs` |
| Build | `Assets/Scripts/Editor/Testing/VarginhaDailyBuild.cs` e meta |
| EditMode | `CampaignHouseInvestigationTests`, `CampaignExpansionTests`, `CampaignIllustratedMapTests`, `CampaignPuzzleDesignTests`, `VarginhaVisualPolishTests` em `Assets/Tests/EditMode` |
| PlayMode | `CampaignExpansionPlayTests`, `CampaignFeedbackPlayTests`, `CampaignLabWorkshopAuditTests`, `CampaignPuzzleDesignPlayTests`, `CampaignVisualPolishPlayTests`, `EnemyAITests`, `PhysicsTests`, `VarginhaDifficultyAndPresentationTests`, `VarginhaExperimentIntegrationTests`, `VarginhaInteractionTests`, `VarginhaInventoryTests`, `VarginhaMenuAndDeathTests`, `VarginhaRequestedGameplayTests`, `VarginhaStudentAllyTests` e novo `IsolatedPlayModeTests` em `Assets/Tests/PlayMode` |
| QA reproduzível | `Tools/KeyInvestigationArt/audit_art.py`; `Tools/QA/summarize_unity.py`, `test_summarize_unity.py`, `campaign_coverage.py`, `art_preview.mjs`, `package_windows.py` e README |
| Documentação | GDD, abertura adulta, pendências de 20/21, roteiro de playtest e roteiro GDD/LDD em `Docs/CampanhaOficial` |
| Daily | HTML, fonte `build.mjs` e roteiro em `output/daily-sprint-2026-10-20`; atualização de 21/10 preserva retrospectiva de 09–19/10 |

Cenas `.unity` de produção não foram regravadas. A abertura envolve `Ato2_Fase2_A_Chave_e_a_Caixa` e `Ato2_Fase4_Entre_Aulas_e_Pistas`. A correção do construtor de polígonos vale para os cenários ilustrados. Os 14 capítulos, IDs persistentes e 19 entradas da build (menu + 18 áreas) permanecem. Protótipos arquivados são testados no Editor sem serem acrescentados à distribuição.

## Testes e evidências

| Verificação | Resultado | Evidência |
|---|---|---|
| EditMode completo final, após correção da dica | **348/348**, sem skips | `editmode-delivery.json` e XML |
| PlayMode, último resultado por caso | **270/270**, sem skips | `playmode-coverage.json` |
| Player Windows: percurso completo e colisões das duas casas | **3/3** | `player-final.json` e XML |
| Player Windows: 18 áreas distribuídas | **1/1** caso percorreu todas as áreas | `player-maps.json` e XML |
| Auxiliares da skill de pixel art | **30/30** | Logs completos em `output/agent-logs`, execução de 21/10 às 10:01 UTC |
| Wrapper de saída | **12/12** | Log `2026-10-21T10-15-05-510Z-b30d80` |
| Resumo NUnit: zero testes, falhas e saída extensa | **6/6** | Log `2026-10-21T10-16-53-907Z-e89667` |
| Arte | Oito frames preservados; duas animações avançam e param corretamente | `ARTE.md`, `art/preservation.json`, `art/preview/playback.json` |

**Como ler os 270 casos:** a rodada completa terminou em 241 aprovados e 29 falhas. Foram corrigidos os contratos desatualizados e o isolamento; os grupos afetados foram reexecutados. Um ajuste inicial na ordem dos callbacks de isolamento gerou falhas de setup, depois corrigidas. A rodada de 40 componentes terminou em 39 aprovados e uma comparação de brilho; esta passou no reteste específico. `playmode-coverage.json` conserva cada rodada, resultados originais e aliases dos cinco testes renomeados. Não se trata de uma única execução completa sem falhas. Nenhum teste foi desativado ou marcado como ignorado para fechar a cobertura.

O primeiro Player falhou na geração automática de contorno da textura. Depois da correção, a abertura completa e os testes físicos infantil/adulto passaram no executável. A investigação inclui save/reload da chave, hipóteses incorretas sem perda de pistas, páginas, inventário e continuidade na escola.

A última alteração de código depois dos testes nativos foi somente a localização da pia no texto da dica; a suíte EditMode de 348 casos foi repetida integralmente e a distribuição recompilada. Os quatro casos Windows pertencem a dois executáveis de teste; a inspeção da distribuição é registrada separadamente em `release-smoke.json`.

## Arte e continuidade narrativa

Usada a skill `pixel-art-sprite-sheet-master`, agora com auxiliares completos disponíveis. Não foi necessário gerar novas imagens: contato, overlays, empacotamento canônico e reprodução temporal foram verificados. O SHA-256 do atlas recombinado é idêntico ao do asset usado pelo jogo. Três avisos textuais de preset Unity foram analisados e explicados em `ARTE.md`; o caminho runtime usa Texture2D e PPU 128 explícito, com compressão padrão desativada e override Standalone inativo.

O [roteiro de cutscenes](../../Docs/CampanhaOficial/ROTEIRO_CUTSCENES_GDD_LDD_2026-10-21.md) usa o GDD e LDD originais, com planos detalhados da chave/Fusca e planejamento até os três finais previstos. Separa encenação proposta de fatos/falas das fontes. Registra as divergências 1898/1966, 23:23/23:17 e ordem de encontros, sem resolvê-las inventando acontecimentos. Também distingue os 25 níveis planejados no LDD dos 14 capítulos atuais.

## Build e pacote

**0.4.1-beta, Windows x64**, Unity 6000.6.0f1. Compilação `Succeeded`: zero erros e 1 aviso. O aviso informa ausência de `RuntimePipelineConfig` do pacote Unity Pipeline; não foi alterada a configuração gráfica do usuário para silenciá-lo.

Pacote local: [Varginha_0.4.1-beta_2026-10-21_Windows_x64.zip](../../Builds/Varginha_0.4.1-beta_2026-10-21_Windows_x64.zip), 249,589,255 bytes (238.0 MiB), 196 arquivos. Inclui `LEIA-ME_PLAYTEST.md` e manifesto SHA-256 por arquivo. ZIP reaberto com CRC validado; pastas explicitamente marcadas `DoNotShip` e assemblies de testes ficam fora da distribuição. Não foi criada uma GitHub Release nova.

SHA-256 do ZIP: `426ac9784a89a85e46ccb94ad1d894447a6e817fb79e49da5b8239d82cb1f9b2`.

A inspeção visual do executável confirmou menu, arte, fundo animado, botão Continuar, casa adulta e tutorial de movimento. Capturas: `release-menu.png` e `release-adult.png`; escopo e limites em `release-smoke.json`. Depois a automação detectou interação do usuário e interrompeu novas entradas. F1/pausa não foram conferidos manualmente nessa sessão; a progressão e a pausa já têm cobertura automatizada.

A GPU local GeForce G210 registrou `GpuFence::Create` / `ID3D11Fence` com código `0x80070057`. O jogo renderizou, mas esta inspeção não certifica desempenho ou compatibilidade em outros PCs. Não foram encontrados `NullReferenceException`, outras exceptions C# ou o erro anterior de leitura do polígono no log da inspeção até esse ponto.

Evidências de build e pacote: `windows-build.json`, `windows-package.json`.

## Daily atualizada

[Apresentação HTML](../daily-sprint-2026-10-20/A_Sprint_Final_Varginha.html) com 17 slides, retrospectiva de 09–19/10 e entregas identificadas de 20/21. O slide de fechamento mostra testes, arte e candidata Windows. O plano das novas cutscenes aparece como aguardando aprovação. O encerramento permanece dedicado a dúvidas, opiniões e feedbacks reais.

**QA aprovado:** fontes e imagens, zero overflow, JavaScript sem erros, teclado, notas, nove transições, navegação rápida, impressão dos 17 slides e movimento reduzido. O texto da rota de demonstração foi ajustado depois que a primeira conferência detectou overflow; o QA completo passou no reteste. Evidência `slides-qa.json`; cinco slides alterados revisados visualmente em `slides/`. O roteiro da apresentação também foi atualizado.

## Preservação e limites

Os três saves originais foram restaurados depois que o usuário confirmou o fechamento da candidata; SHA-256 conferido byte a byte com as cópias anteriores aos testes. Evidência: `preservation.json`. A Unity recompôs automaticamente a lista runtime de `UniversalRenderPipelineGlobalSettings.asset`; os bytes originais do usuário foram restaurados, e esse arquivo continua fora do commit, junto das cenas de recuperação. Foram preservados 103 arquivos gerados pela regressão em `output/agent-logs/2026-10-21-regression-captures`; as capturas históricas e ProjectSettings foram devolvidos ao conteúdo versionado. A alpha histórica já aberta pelo usuário não foi encerrada.

Não houve playtest com outra pessoa, avaliação da equipe ou teste em outro PC. O [roteiro de playtest](../../Docs/CampanhaOficial/PLAYTEST_2026-10-21.md) acompanha a entrega. Esses resultados e a eventual dublagem estão na [lista atual de pendências](../../Docs/CampanhaOficial/PENDENCIAS_2026-10-21.md). A validação funcional local não certifica desempenho mínimo nem substitui uma partida humana completa da campanha.

Saídas extensas foram guardadas em disco; resumos mostram contagens e falhas, sem descartar a evidência. O wrapper e `-logFile` são complementares. Nenhum modelo local foi configurado, e não há alegação de economia garantida de 99% de tokens.
