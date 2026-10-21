# Novo puzzle de Fragmentos — 21/10/2026

Proposta aprovada pelo usuário: mapa com bifurcações. Substitui a permutação de marcos e rotação de setas; não é apenas a mesma combinação com menos cliques.

O jogador parte da árvore e traça três trechos. O terreno mostra tronco bloqueando uma passagem, um rio com uma ponte utilizável e o muro da capela com acesso pelo portão. As pistas do livro, globo e painel ficam ao lado do mapa. Selecionar uma alternativa apenas destaca a prévia; TRAÇAR TRECHO a confirma. DESFAZER e RECOMEÇAR permitem rever decisões. CONFERIR ROTA aponta a primeira contradição e mantém os trechos anteriores válidos.

Existem oito combinações e uma rota compatível com as evidências. A primeira bifurcação ensina o controle; as seguintes pedem comparar caminho, travessia e entrada. Três dicas progressivas começam pela observação, depois destacam a região e finalmente indicam uma alternativa. O objetivo é dificuldade mediana no começo da campanha; o playtest humano ainda precisa confirmar tempo/compreensão.

## Continuidade e saves

O desbloqueio continua sendo a localização da mata, com árvore, rio/ponte e capela. A fase 7 e suas marcas permanecem. Os três registros físicos continuam necessários; nenhum clique isolado concede a investigação completa. Não foram acrescentadas revelações sobre a entidade.

O save recebe `trailChoices`, `trailHints` e `trailFeedback`. Campos antigos de ordem/direção permanecem para leitura de saves, mas não resolvem o novo enigma. Saves concluídos conservam a mata e os capítulos posteriores; saves incompletos começam a nova mecânica sem receber solução automática. Traçado parcial, dicas e correções ficam salvos.

## Arquivos e cenas

Produção: novo `CampaignTrailPuzzle.cs`; integração em `CampaignPuzzleDesign.cs`, `CampaignExpansionState.cs` e `CampaignExpansionController.cs`; textos em `CampaignGuidance.cs`, `CampaignHints.cs` e `CampaignJournal.cs`.

Testes: novos contratos em `CampaignTrailPuzzleTests.cs`; atualização dos testes que exigiam ordenar/girar; teste de interface cobre prévia, desfazer, retomada parcial e confirmação por gamepad. Cenário afetado: Fragmentos/biblioteca (fase 6), com preservação do acesso à Mata (fase 7). Nenhuma cena `.unity` de produção foi regravada.

A nova tela é um mapa esquemático desenhado pela UI do jogo, com sua fonte, paleta e controles existentes. Não é uma nova ilustração raster nem substitui os sprites aprovados dos cenários.

Resultados finais: `output/cinematic-opening-2026-10-21/RELATORIO.md`. Roteiro externo: observar se o jogador entende prévia versus confirmação, reconhece o obstáculo de cada trecho e usa as dicas sem ajuda verbal.
