# Correção de recorte e espaçamento dos sprites — 26/10/2026

Os personagens do atlas direcional estavam organizados em células de 224×224 px. A grade foi ampliada para 256×256 px, mantendo o atlas 8×8 em 2048×2048 px e preservando o tamanho visual em jogo: o PPU continua sendo calculado pela altura da silhueta, sem escalar o personagem para preencher a célula.

O novo enquadramento deixa ao menos 68 px de margem transparente em cada lado dos frames do Edelzio e mais de 100 px acima das figuras. Os frames permanecem em uma textura compartilhada por personagem, com filtro Point, sem mipmaps e sem compressão, para evitar amostragem de quadros vizinhos e manter a renderização pixel art nítida. No Edelzio com e sem mochila, pixels pretos soltos além do contorno imediato foram removidos; detalhes escuros da roupa e do rosto foram preservados.

## Arquivos e recursos

- `Tools/Art/prepare_directional_20261026.py`: grade 256 px, limpeza localizada do contorno do Edelzio e geração dos manifestos/pivôs.
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
