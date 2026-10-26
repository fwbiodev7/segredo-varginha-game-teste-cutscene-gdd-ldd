# Correção de recorte e espaçamento dos sprites — 26/10/2026

Os personagens do atlas direcional estavam organizados em células de 224×224 px. A grade foi ampliada para 256×256 px, mantendo o atlas 8×8 em 2048×2048 px e preservando o tamanho visual em jogo: o PPU continua sendo calculado pela altura da silhueta, sem escalar o personagem para preencher a célula. A extração também lê 12 px além da grade original e escolhe a silhueta ancorada dentro da célula; isso recupera partes de poses que tocavam o limite da imagem-fonte, como o nariz e o cabelo nos perfis do Edelzio, sem incorporar a pose ao lado.

O novo enquadramento deixa ao menos 68 px de margem transparente em cada lado dos frames do Edelzio e pelo menos 93 px acima das figuras. Os frames permanecem em uma textura compartilhada por personagem, com filtro Point, sem mipmaps e sem compressão, para evitar amostragem de quadros vizinhos e manter a renderização pixel art nítida. No Edelzio com e sem mochila, pixels pretos soltos além do contorno imediato foram removidos; detalhes escuros da roupa e do rosto foram preservados.

## Arquivos e recursos

- `Tools/Art/prepare_directional_20261026.py`: faixa de extração sobreposta, grade 256 px, limpeza localizada do contorno do Edelzio e geração dos manifestos/pivôs.
- `Assets/Resources/Varginha/StoryCharacters/Directional20261026/`: 15 atlas 8×8 e manifestos de 64 poses cada, reprocessados sem alterar os GUIDs existentes.
- `Assets/Scripts/Game/Varginha/Experiment/CampaignStorySprites.cs`: identidade de textura isolada para impedir colisão no cache geométrico de atlas.
- `Assets/Scripts/Game/Varginha/Experiment/CampaignTeamEdelzio.cs`: seleção das poses direcionais no movimento e ataque.
- `Assets/Tests/PlayMode/CharacterFrameRenderingTests.cs`: teste de renderização Unity dos atlas direcionais.
- `Assets/Tests/PlayMode/CampaignForestReviewPlayTests.cs`: cenário de floresta, captura e retorno ao checkpoint.

Nenhuma cena foi editada. A validação de floresta carrega a fase 7 em runtime; não altera o arquivo de cena.

## Verificação

- Player Unity: `RendersEveryDirectional20261026FrameThroughUnitySpriteRenderer` — passou. Renderizou 15×64 quadros sem células vazias, conferiu filtro Point, ausência de mipmaps, geometria da textura e conteúdo dentro de cada célula. Contact sheets em `output/revisao-final-2026-10-26/directional-unity-render/`.
- Player Unity: `CaptureShowsGameOverAndRetryReloadsTheSavedRiverCheckpoint` — passou. A captura confirma o Edelzio inteiro na floresta e o reinício preserva o último checkpoint.
- Prévia dos 64 quadros do Edelzio inspecionada visualmente após a importação no Unity.

Logs, XMLs e capturas completos estão em `output/revisao-final-2026-10-26/`; são evidências locais de QA e não fazem parte dos recursos do jogo.

## Ajustes de recorte, ciclos e papéis da fase 2

Na revisão seguinte, os 15 atlas direcionais passaram a usar os retângulos completos declarados em seus manifestos, com malha Full Rect. Isso preserva pixels de cabelo, mãos, roupas e calçados que podiam ser descartados pela geometria apertada. A caminhada percorre três poses selecionadas (0, 1 e 3) mantendo a duração do ciclo anterior; ataques direcionais usam preparação, impacto e recuperação (colunas 4, 5 e 7). O mesmo padrão foi aplicado à Ouzana, ao padre e ao Renan no combate final, às manifestações maior e menor, e à entidade híbrida da floresta. O desenho original e as regras de combate foram preservados.

Na investigação da fase 2, o papel principal e os três fragmentos agora têm tamanho legível, rotação variada e posição independente no mundo. Antes, os fragmentos herdavam a escala reduzida do papel principal e podiam parecer sobrepostos. Eles continuam surgindo somente quando a pista está ativa.

Arquivos de código alterados: `CampaignStorySprites.cs`, `VarginhaStudentAnimation.cs`, `CampaignHeroArt.cs`, `CampaignOuzanaVisual.cs`, `CampaignFinalAllies.cs`, `CampaignManifestationCombat.cs`, `CampaignExpansionController.cs`, `CampaignFuscaCinema.cs` e `CampaignHouseSequence.cs`. Os testes de renderização e integração correspondentes foram atualizados em `CharacterFrameRenderingTests.cs` e `CampaignMessagePlayTests.cs`. Nenhuma cena ou textura-fonte foi editada.

Verificação adicional em 26/10/2026:

- Unity PlayMode (Editor): 6 testes passaram, incluindo renderização dos 960 quadros dos 15 atlas e validação dos quadros completos da Ouzana e da entidade da floresta.
- Unity PlayMode (Player Windows): os mesmos 6 testes passaram.
- Integração da fase 2: o percurso das pistas passou; confirmou sprites compartilhados, três fragmentos separados, escala independente e exibição condicional.
- Prévias dos 15 atlas inspecionadas após renderização pelo `SpriteRenderer` do Unity. XMLs e imagens de evidência permanecem em `output/revisao-final-2026-10-26/`.
