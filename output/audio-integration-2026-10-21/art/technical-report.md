# Artes dos ataques de apoio

Pedido: novos VFX para Renan, Ouzana e Padre Fábio, com Renan cyber/hacker, frasco verde preservado e escudo transparente. A autorização veio da aprovação das correções e das instruções posteriores de estilo e criação das artes. Skill aplicada: `pixel-art-sprite-sheet-master`; geração pelo `image_gen` nativo.

Referência observada: `Assets/Resources/Varginha/StoryCharacters/SupportEffects.png`, três linhas de seis quadros, células de 128 pixels, consumidas a 48 PPU. O atlas original e os sprites de rostos/roupas/poses não foram alterados.

## Entrega

- Fonte gerada RGBA: `source/generated-source.png`, 1024×1536. É uma fonte raster achatada; não há arquivo Aseprite com camadas.
- A geração não respeitou perfeitamente os intervalos da grade. A receita mede seis faixas de conteúdo, registra os desenhos sem reamostragem e alinha o apoio do escudo. Não houve rotação arbitrária nem repintura de personagens.
- Atlas final: `export/sheet.png`, 1285×1927, quatro colunas, seis linhas, 24 canvases de 320×320 e padding de 1 pixel. Oito desenhos por apoio; pivô central [160,160]; 96 PPU. O conteúdo conserva os pixels da fonte. As margens medidas dos núcleos no cutoff do runtime são de pelo menos 32 pixels.
- `export/atlas.json` e `unity-slices.json`: metadados canônicos e plano Unity; `recipe.json`: montagem reprodutível; `preview-final/index.html`: animações, pausa e inspeção dos quadros; `preview-final/contact.png`: contact sheet.
- No projeto: `Assets/Resources/Varginha/StoryCharacters/SupportEffectsV2.png`. Importação Point, sem mipmaps/compressão, alpha preservado, recurso não legível pela CPU.

## Animação e renderização

Renan usa disparo de quatro quadros em 0,55 s e quatro desenhos de dissipação em 0,45 s, acrescidos do mesh cyber no alvo efetivamente atingido. Ouzana usa dois desenhos durante o voo de 0,6 s e seis de quebra/nuvem nos 2,6 s seguintes. Padre usa subida, dois quadros de sustentação e saída; a proteção dura seis segundos ativos. As poses originais dos professores continuam separadas dos VFX.

Cyber e frasco usam o shader pixel existente com cutoff 0,5, retirando o halo de alpha residual da geração. O escudo usa `SupportShieldClean`, preservando contorno e alpha sem preencher o centro. Os avisos de canvas nos três desenhos de Renan se referem ao halo residual; os núcleos não estão cortados. A variação de área do frasco entre carga e arremesso é intencional. Os quatro avisos do validador ficam preservados, com essa disposição, em `validation-final/validation.json`.

Contact sheet e sequências de avanço/dissipação foram abertos para revisão. Os quatro clipes da página foram reproduzidos e verificados por Playwright; a montagem canônica conserva o loop final/inicial de `fabio_hold`. A reprodução no Player, transparência, acertos, pausa e capturas estão no relatório principal. Prévia da fonte não substitui essa verificação no jogo.

## Prompt utilizado

Spritesheet VFX pixel art cartunesco, referência do atlas original apenas para estilo e paleta; 24 células, quatro colunas e seis linhas, oito desenhos consecutivos por apoio; alpha real, sem personagens ou fundo. Renan: dados ciano/turquesa, acentos magenta/verde, lançamento e ruptura digital, sem arcos de fogo. Ouzana: frasco de vidro verde com rolha, arremesso, quebra e névoa reagente, preservando a identidade do efeito. Padre: cúpula dourada com contorno fino, elipse no chão, cruzes pequenas na borda, centro vazio/transparente, expansão e desaparecimento. Desenhos distintos, perspectiva consistente, margens e centro de célula alinhados. O resultado bruto foi registrado e montado pela receita após inspeção das faixas.
