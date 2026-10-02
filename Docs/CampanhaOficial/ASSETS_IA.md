# Assets gerados para a cópia experimental

Modo utilizado: ferramenta integrada ImageGen. As imagens finais estão em `Assets/Resources/Varginha/Experiment/`. As fotografias originais não foram copiadas para o repositório. A importação do Unity utiliza filtro Point, sem mipmaps e sem compressão.

| Arquivo | Uso |
| --- | --- |
| OpeningStoryboard.png | Atlas 2 × 2: reportagem, rua, documentos e criança diante da TV. O compositor produz quadros reais em 384 × 216. |
| IndustrialFacade.png | Fachada da Industrial; imagem de referência para o laboratório, não substitui o mapa topview. |
| ReferenceCast.png | Referência visual dos quatro personagens derivados das fotos. |
| GameCast.png | Versão simplificada para o jogo, atlas importado em 128 × 128, células de 64 × 64. |
| ChildEdelzio.png | Edelzio aos seis anos, 3 poses por direção, 4 direções. Atlas importado com limite de 256; animado na Fase 1. |

As ilustrações geradas são arte de uma experiência em desenvolvimento. Não são fotografias nem filmagens reais dos eventos de 1996. As reportagens e falas da abertura são ficcionais. Voz temporária: Microsoft Maria Desktop, gerada localmente por `Tools/GeneratePreviewSpeech.ps1`; nenhuma voz pessoal foi clonada.

## Prompts e especificações

### Personagens: prompt da referência

Create ONE coherent transparent pixel-art character atlas for a Unity Brazilian 2D investigation horror game. Use the four reference photographs in the supplied order as identity reference, not as backgrounds. Exactly 2 by 2 equal-size cells with generous padding within each quadrant and no separators, no labels, no text. Each quadrant contains exactly ONE complete full-body person in a slightly top-down frontal view with both feet visible. Top-left = Edelzio from reference 1: slim Brazilian man, short dark hair, black rectangular glasses, mustache and short goatee, faded mustard/yellow T-shirt, dark jeans, dark shoes. Top-right = Renan from reference 2: heavier stocky Brazilian man, rounded body and face, thick dark beard, dark curly hair, black glasses, orange T-shirt and open brown/checkered overshirt like his photo, dark trousers and sneakers. Bottom-left = Padre Fabio from reference 3: stocky Brazilian man with black glasses, short dark curly hair and trimmed beard, grey polo shirt with blue collar and sleeve trim, dark grey jeans and dark sneakers. Keep the grey polo; DO NOT turn him into a priest in a cassock. Bottom-right = Ouzana from reference 4: older Black Brazilian woman with shoulder-length black curly hair and large glasses, white long-sleeved blouse, dark grey jeans, black sneakers with white soles, warm composed face. Preserve her actual age, dark skin, hair and glasses; no fieldwork jacket and no fantasy costume. Style: recognizable portraits translated into crisp square-pixel art with chunky visible pixels, 32-bit adventure RPG quality, dark outlines, grounded warm colors. Calm neutral standing poses, arms relaxed, human proportions. No props that obscure faces or shirts. Transparent alpha background everywhere outside the four figures, no shadows, no floor, no glow. Identifiable likeness and each correct quadrant are essential.

### Fachada: prompt da referência

Create ONE 16:9 landscape cinematic pixel-art establishing-shot background for a Brazilian 2D supernatural investigation game. Use the attached school facade photograph as an architectural reference. Preserve the actual low white concrete street-front school wall, dark green base strip, central beige metal pedestrian gate, flat top with wire fencing, higher white school block on right, sidewalk, asphalt road, utility poles and overhead power lines. Camera straight-on, readable full facade composition, game-art rendering with deliberate visible square pixels, grounded Minas Gerais urban architecture. Late afternoon just before dusk in 2026, warm light and subtle long shadows, ordinary peaceful school atmosphere, no horror manifestation, no characters, no car. Keep the characteristic painted blue/green geometric mural shapes to the right, but LEAVE LETTERING AREAS BLANK: school name will be added manually inside the game, so no readable text, no phone numbers, no tiny random letters, no watermark. Restrained navy/ochre/teal palette compatible with the game's existing pixel-art house and road. This is an original game background based on the reference facade, not a photograph.

### Edelzio criança: prompt final

Create one production pixel-art character sprite sheet for a 2D TOP-DOWN Brazilian mystery game. Use the referenced existing game sprite only as pixel scale, palette and viewpoint style reference. Subject is Edelzio as a SIX-YEAR-OLD BOY in 1996, short dark brown hair, medium tan skin, plain faded mustard yellow T-shirt, dark blue shorts, small brown shoes. NO glasses, NO facial hair, NO backpack. Recognizably a young child with small body. Transparent background. Exact 3 columns x 4 rows equal cells, all cells identical size, evenly aligned feet and scale, no borders or text. Each row has idle, left-foot step, right-foot step. Row1 facing down/front, row2 facing left, row3 facing right, row4 facing up/back. View from high overhead with face visible only down/front and sides, classic topview RPG, chunky 32x48 logical pixel sprite per cell, hard square pixels, tiny two-tone shading, no blur, no gradients, no anti-aliasing, no big portrait, no equipment. All twelve frames of the same child, detached with transparent margins, stay within cells. Preserve brown hair/yellow shirt/navy shorts identically in each frame. This must be usable small in an existing gameplay map, not illustration or concept artwork.

### Abertura e simplificação do elenco: especificações registradas

Storyboard de quatro quadrantes iguais, sem textos: apresentador em estúdio brasileiro de 1996, rua de Varginha à noite, documentos/mapas contraditórios e menino diante de televisão CRT. Estilo pixel art sóbrio para terror/investigação, sem reproduzir cenas de outro jogo. Legendas são desenhadas pelo próprio jogo.

O elenco foi editado a partir de ReferenceCast usando o sprite existente de Edelzio como referência de estilo: pixels grandes, sombreamento simples e transparência; mesma ordem dos quatro quadrantes. Óculos, roupas, cabelo, proporções e cor da pele seguem as fotos fornecidas. Essa especificação registra o resultado e as restrições; a transcrição integral dessas duas primeiras chamadas não foi preservada neste documento.
