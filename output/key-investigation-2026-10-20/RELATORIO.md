# Entrega — abertura adulta e chave desaparecida

Data: 20/10/2026. Base: `df2b091`, branch `codex/campanha-15-fases-e-site`.

## Implementado

- Preparação para a escola com higiene, café, notebook e mochila; instruções contextuais consultam os controles configurados.
- Chave do Fusca visível no balcão, animação de colocação, desaparecimento fora de vista e pistas sonoras posicionais.
- Investigação de balcão, trinco e papel. As observações precisam ser interpretadas antes de abrir a caixa; respostas erradas conservam o progresso.
- Caixa com chave e caderno de 1996. Reconstrução das páginas pelas ilustrações, muro e galhos, sem mostrar a ordem nas instruções iniciais.
- Dicas progressivas, objetivos e registros atualizados; saves intermediários e progresso antigo preservados.
- Embarque, partida, rádio, painel, pane, silêncio, reinício e chegada à Industrial. O trecho é cinematográfico; não foi acrescentado um capítulo obrigatório de direção.
- Oito sprites novos: chave em repouso/colocação, papel, caixa fechada/aberta e três páginas. Atlas 512×256, células 128×128, PPU 128, filtro Point e alpha binário.

## Arquivos e cenas

Produção alterada: `CampaignStory.cs`, `VarginhaCampaignStage.cs`, `CampaignGuidance.cs`, `CampaignHints.cs`, `CampaignJournal.cs`, `CampaignSoundscape.cs`, em `Assets/Scripts/Game/Varginha/Experiment`.

Produção adicionada: `CampaignHouseInvestigation.cs`, `CampaignHouseSequence.cs`, `CampaignHouseArt.cs`; importador `Assets/Scripts/Editor/Testing/CampaignKeyArtImporter.cs`; atlas/JSON em `Assets/Resources/Varginha/KeyInvestigation`.

Testes: `CampaignHouseInvestigationTests.cs`, percurso atualizado em `CampaignChaptersTests.cs` e executor alternativo `CampaignOpeningAudit.cs`. Montagem reproduzível da arte: `Tools/KeyInvestigationArt/build_art.py`.

Cenas afetadas em runtime: `Ato2_Fase2_A_Chave_e_a_Caixa` e transição para `Ato2_Fase4_Entre_Aulas_e_Pistas`. Não foi necessário regravar arquivos `.unity`. IDs e ordem dos 14 capítulos foram preservados. Arte original, cenas de recuperação e alterações anteriores nas configurações URP ficaram fora deste commit.

Documentação: `Docs/GDD.md`, `Docs/CampanhaOficial/CAMPANHA_14_FASES.md`, `CHAVE_DESAPARECIDA.md` e `PENDENCIAS_2026-10-20.md`.

## Resultados

| Verificação | Resultado | Evidência |
|---|---|---|
| Investigação, saves e rotas físicas | 11/11 aprovados | `editmode-house-final.json` |
| História existente | 4/4 aprovados | `editmode-story.json` |
| Sequência e migração existentes | 16/16 aprovados | `editmode-sequence.json` |
| Percurso jogável no Editor | Aprovado | `playable-audit.json` |
| Compilação Unity 6000.6.0f1 | Sem erros | `compile-final.json` |
| Integridade da arte | 8 frames não vazios, alpha binário; hashes registrados | `art/validation.json` |
| Apresentação HTML | 16 slides, imagens/fontes/navegação/transições conferidas | `slides-qa.json` |

Total EditMode: **31/31**. A auditoria executou as verificações do percurso de infância, preparação, colocação da chave, pausa, save/reload, desaparecimento, hipótese incorreta/correta, caixa, páginas, inventário, viagem e investigação da escola. Inclui verificação de que a chave fica acima do balcão na ordenação visual. A auditoria também registra erros/exceções de runtime.

O runner PlayMode padrão da Unity apresentou erro de infraestrutura em `PlayModeRunTask` durante a troca de modo. O percurso aprovado foi executado por `CampaignOpeningAudit`, com contexto de testes e os mesmos asserts. Isso não é apresentado como um resultado aprovado daquele runner. Não houve nova build Player nem teste físico de controle nesta entrega.

As capturas `counter-gameplay.png`, `pages-gameplay.png` e `fusca-gameplay.png` mostram o runtime atual. Estados foram preparados para as capturas após o percurso automático; elas não substituem os testes. A fonte de arte e as prévias foram inspecionadas; aprovação artística final fica com a equipe.

## Apresentação e fluxo de trabalho

A daily em `output/daily-sprint-2026-10-20/A_Sprint_Final_Varginha.html` inclui três slides das novidades de hoje, as pendências e o encerramento exclusivo para dúvidas, opiniões e feedbacks. Os nove efeitos de transição e o movimento reduzido foram preservados. O roteiro de fala foi atualizado.

O commit também inclui o wrapper `Tools/AgentOutput` e os arquivos da apresentação produzidos nesta conversa. Os 12 testes do wrapper passaram. Ele preserva logs completos e entrega resumos; não há modelo local configurado nem promessa de economia de 99% de tokens.

## Pendências

Lista versionada: [PENDENCIAS_2026-10-20.md](../../Docs/CampanhaOficial/PENDENCIAS_2026-10-20.md). Principal limitação: a fala infantil está legendada e acompanhada por interferência, mas ainda não possui gravação real. Também ficam playtest externo, runner padrão, nova build Windows e regressão da campanha completa.
