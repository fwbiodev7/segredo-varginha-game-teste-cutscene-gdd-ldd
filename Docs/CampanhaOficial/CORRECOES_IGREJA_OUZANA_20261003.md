# Fechamento de 3/10/2026 — igreja, escala e Ouzana

## Livro do Tombo

A resposta **1996 / ÂNCORA / 23** abre o compartimento sem depender de três gatilhos de leitura separados ou de flags antigas da conversa com Fábio. Respostas incorretas continuam sendo recusadas. Uma solução confirmada registra a combinação e permanece válida ao salvar, carregar ou revisitar o livro. Saves antigos com o compartimento já resolvido também recuperam os valores confirmados.

## Escala de Edelzio

**Somente na igreja, fase 8**, as animações de Edelzio usam a altura visível do padre: **42/44 = aproximadamente 0,955 unidade**. Em todas as outras fases, Edelzio conserva seu tamanho normal de **1,50 unidade**. A normalização ocorre nos pixels por unidade dos sprites; os arquivos originais, o transform, a velocidade e o apoio dos pés permanecem preservados. Os caches mantêm as duas escalas separadas, evitando que a redução persista ao sair da igreja. Poses, mochila, socos e ações seguem a escala da fase atual.

## Ouzana

Novo atlas RGBA `StoryCharacters/OuzanaBiologist.png`, com **16 quadros**, quatro vistas (frente, esquerda, direita e costas), respiração, caderno científico e amostra botânica. A referência pessoal orienta os óculos e o cabelo cacheado; a revisão solicitada adiciona pele castanha mais escura e traços mais maduros. Usa jaleco branco, calça cinza e tênis pretos.

O laboratório utiliza o atlas dedicado, com altura normal de adulto de **1,50 unidade**, a colisão existente, ordenação pelo chão, iluminação e sombras já implementadas. Os sprites são reutilizados em cache; a animação verifica a pose a cada 0,35 segundo, sem copiar a textura para a CPU. Importação Point, sem mipmaps, sem compressão, limite de 1024 pixels. O manifesto registra os recortes reais, respeitando os espaços transparentes entre as linhas.

A foto pessoal não é incluída no repositório. A arte foi criada e revisada pela ferramenta integrada `image_gen.imagegen`.

## Prompt final de revisão da arte

> Edit the FIRST attached transparent 4x4 Ouzana biologist sprite sheet, using the SECOND photo as her identity reference. Make only the woman's skin a little darker and her face visibly a little older/more mature, as the user requested. Warm medium-dark brown skin with restrained lighter brown highlights instead of orange/tan. A mature adult woman, subtly fuller cheeks with gentle smile lines and tiny under-eye age marks that remain readable at game pixel size, not exaggerated wrinkles. Keep the same smiling expression, rectangular dark glasses, shoulder-length naturally curly mostly black hair, white open laboratory coat, white shirt, charcoal gray jeans, black sneakers, notebook and plant. Preserve EXACTLY the existing 4 columns by 4 rows layout, four directions S W E N, all poses, complete silhouettes, pixel density, head/body proportions and transparent margins. Apply consistent darker skin to all faces, ears and hands and the same mature identity across all sixteen frames. Same crisp RPG pixel art style as input, no blur, no continuous photo shading, no labels, no grid, no background, no cast shadows. Real alpha transparency; opaque white coat.

## Continuidade

A nova branch parte da campanha já integrada, preservando as alterações de hoje em mapas, colisões, luzes, sombras, menu/HUD, transições, animações, escola, Fusca vazio e tutorial. Não inclui arquivos pessoais de recuperação de cenas.

Branch: `codex/fechamento-2026-10-03`.

Validação final: **66/66 testes de edição** e **15/15 testes de execução**, incluindo resposta correta com flags antigas ausentes, persistência da solução, escala exclusiva da igreja, restauração do tamanho ao entrar no laboratório, atlas de Ouzana, colisões, pausa, animações e fases da campanha. Relatórios: `ValidacaoIgrejaOuzana20261003EditMode.json` e `ValidacaoIgrejaOuzana20261003PlayMode.json`.

Conferência visual em execução: igreja com Edelzio e padre lado a lado, mensagem de sucesso do Livro do Tombo, Ouzana com amostra botânica no laboratório e Edelzio no assento do notebook em tamanho normal. Os dois arquivos de progresso do usuário foram restaurados e conferidos byte a byte. O Editor terminou no menu, fora do modo de execução, sem erro de compilação ou cena não salva. As nove imagens de cenário originais e os cinco arquivos de cena/arte protegidos permanecem idênticos às fontes verificadas.

## Arquivos desta correção

- `CampaignExpansionState.cs` e `CampaignExpansionController.cs`: solução da igreja, gravação e integração da bióloga.
- `CampaignTeamEdelzio.cs`, `CampaignStorySprites.cs` e `CampaignWallBody.cs`: escala somente na igreja e caches separados.
- `CampaignOuzanaBiologist.cs` e `StoryCharacters/OuzanaBiologist.png/.json`: personagem e poses de laboratório.
- `CampaignExpansionTests.cs`, `CampaignIllustratedMapTests.cs`, `CampaignExpansionPlayTests.cs`: regressões e execução.
- Guia de puzzles, documentação de animações, proveniência, este registro e relatórios de validação.
- Capturas em `Preview/IllustratedMaps` e `Preview/UI`: igreja, puzzle resolvido, Ouzana e assento com escala normal.
