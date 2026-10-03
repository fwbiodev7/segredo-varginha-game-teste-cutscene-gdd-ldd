# Correções da casa adulta e mochila V3

Ferramenta: ImageGen integrada. As texturas WallRequested.png e WoodRequested.png são cópias das duas imagens indicadas pelo usuário; o jogo recorta apenas sua borda e repete o material.

A casa adulta recebeu ajustes visuais e vãos de passagem por autorização posterior do usuário. A cena original continua salva sem alteração. A mesa é menor, acessórios respeitam sua superfície, sofá olha para a TV, tapetes mantêm a proporção e portas foram substituídas por portais sem folha, com colisão somente nos batentes.

Edélzio mede 1,50 unidade; a geladeira mede 1,70. A mochila V3 é desenhada nos quatro atlas novos, em 76 poses, usando exclusivamente o novo personagem como referência. O compositor antigo de mochila não participa dessas animações.

Os pivôs de caminhada agora seguem o centro da cabeça em cada quadro, com escala constante e pés na mesma base. As junções de paredes usam cantos contínuos sem criar colisões nos vãos. A luz adulta recebeu menor intensidade e respeita paredes ativas e as bases dos móveis. Os passos têm menor volume e menos ruído agudo.

## Conferência do feedback da escola

| Observação | Ajuste nesta versão |
| --- | --- |
| Jornalista estático | Boca animada em duas aberturas, com fechamento entre poses e ao fim da legenda. Cabeça, câmera e cenário originais permanecem fixos. Não há dublagem de fala; o movimento acompanha a reportagem legendada. |
| “Não deixe ela sair” | Frase corrigida na abertura e na travessia urbana. |
| Movimentação e oscilação lateral | Pivôs dos novos quadros alinhados à cabeça; teste nas quatro direções com e sem mochila. Portais atravessados com movimento físico por teclado. |
| Proporção e móveis da casa infantil | Móveis mantêm a proporção de cada sprite; sofá orientado para a TV e papéis sobre as mesas. Rotas até todos os objetivos validadas. |
| Personagens da equipe de arte | Fontes originais dos alunos, Renan e Padre Fábio preservadas; Edélzio novo usa a referência enviada e atlas próprios. |
| Luz e projeção da casa adulta | Luz reduzida e sombras pelas bases físicas, mesa menor, acessórios sobre o tampo, sofá de frente para a TV e tapetes sem esticamento. |
| Passos altos | Volume do passo reduzido de 0,28 para 0,16 na fonte de efeitos; ruído filtrado e impacto suavizado. |
| Renan sobreposto | Posições da turma separadas, destino do intervalo retirado da área de um banco e rotas reservando espaço para toda a silhueta de Renan. Verificação de distância durante o movimento dos alunos, além da posição inicial. |
| Diálogos longos | Falas de Renan e dos nove alunos encurtadas, preservando as pistas necessárias. |

## Jornalista — movimento da boca

Arquivo: `Assets/Resources/Varginha/HouseFeedback/ReporterMouthV3.png`. Ferramenta: ImageGen integrada. Apenas a região da boca dos quadros gerados é usada; o restante da imagem original não muda entre poses. Prévia: `Preview/CampaignMapsV2/Jornalista_Boca.png`.

```text
Edit target: the attached OpeningStoryboard, specifically its upper-left television news studio panel. Produce one pixel-art animation sheet with exactly THREE vertically stacked equal rectangular 16:9 frames, each reproducing that same entire studio shot and identical grey-haired male journalist at the desk, camera foreground on left, blue Brazil map, blue CRT screens, navy suit and tie, paper in hands. Lock camera, face identity, head and body position, background and palette in all frames. Change ONLY the journalist's tiny mouth: frame 1 closed neutral mouth; frame 2 small open speaking mouth; frame 3 slightly wider speaking mouth. No jaw/body bobbing, no new people, no extra text, no white borders or spaces between frames. Crisp deliberate 2D pixel art consistent with the existing news panel, hard pixel edges, no photorealism. Each complete studio frame fills its own equal-height third of the canvas.
```

## Móveis

Arquivo: `Assets/Resources/Varginha/HouseFeedback/FurnitureCorrections.png`.

```text
Production sprite atlas for the existing Brazilian mystery Unity 2D pixel art game. Reference image is style reference only: match its crisp black outlines, blue upholstered furniture and warm dark wood, but MUCH more visibly low-resolution pixel clusters, no realistic texture, no painterly gradients, no smooth edges. A transparent 1024x1024 atlas with EXACTLY 2 columns and 2 rows, one separated complete item in each cell with generous transparent margins. Row1 column1: the REAR of a blue two-seat sofa with wooden legs, viewed from a straight-on 3/4 overhead RPG camera; sofa faces NORTH away from the viewer toward a TV above it. Show the closed blue upholstered back exterior, tiny visible top edges of the two cushions on far side, outer wooden armrests, wood legs. Do NOT show the fronts of seat cushions or an inviting front-facing seat. Sofa full rear view only, no TV or character. Row1 column2: rectangular teal blue area rug, intact natural 3:2 width-to-height proportions, subtle geometric cream diamond center, cream border and short fringe on left/right, straight-on top-down view. Row2 column1: small wooden framed Minas Gerais countryside landscape composed ONLY of large crisp pixel clusters: green hills, a tiny cottage, pale blue sky and a few square clouds; absolutely no photograph, realistic landscape or smooth painted detail. Row2 column2: small dark-wood cork bulletin board with three plain cream paper notes and colored square pins, no text. Logical pixels approximately 48-64px per object visibly enlarged nearest-neighbor. Rich restrained 4-6 colors per material. NO floor, background, grid, labels, furniture duplicate ghosting, cast shadows outside sprites. Transparent background. All items stay strictly within their cells.
```

## Mochila — Walk

Arquivo: `Assets/Resources/Varginha/TeamArt/EdelzioWalkBackpackV3.png`.

```text
Edit target: the attached NEW Edélzio character animation sheet. Create a NEW complete animation sprite sheet WITH a newly designed fitted backpack in every frame. EXACT four columns, four rows. Keep all 16 existing walk/idle poses and direction order. Rows SOUTH front, WEST left-facing, EAST right-facing, NORTH back-facing. Preserve the reference character's exact current proportions, full rounded brown hair, glasses, moustache/goatee, ochre polo, dark pants, blue-black shoes, wristwatch, silhouette and existing action directions. Do not substitute the older character. The backpack is newly drawn for THIS body: compact charcoal/navy canvas, small front pocket, dark shoulder straps snugly following both shoulders. It sits on the UPPER BACK and ends above the belt, at most half the torso height and no wider than the torso. For SOUTH only shoulder straps visible, NO bag on chest. For WEST show the pack behind the torso on image right; for EAST behind torso on image left; for NORTH center the small pack on the upper back, hands and belt unobstructed. Backpack follows all movement, punches, sitting, leaning and crouching without floating or enlarging the head/body. No reuse of any old backpack asset. Keep original canvas organization with generous padding around EACH full head, hands, feet and backpack. Clean crisp 2D pixel art, consistent logical pixel size, nearest-neighbor edges, dark outlines, restricted palette. Completely transparent background: no ghost figures, colored backdrop, shadows, text, grid lines or labels. No objects or furniture beyond the existing held coffee cup where present. Only add the newly fitted backpack; preserve all existing character poses and appearance.
```

## Mochila — Punch

Arquivo: `Assets/Resources/Varginha/TeamArt/EdelzioPunchBackpackV3.png`.

```text
Edit target: the attached NEW Edélzio character animation sheet. Create a NEW complete animation sprite sheet WITH a newly designed fitted backpack in every frame. EXACT three columns, four rows. Keep all 12 guard, extended punch, recovery poses and direction order. Rows SOUTH front, WEST left-facing, EAST right-facing, NORTH back-facing. Preserve the reference character's exact current proportions, full rounded brown hair, glasses, moustache/goatee, ochre polo, dark pants, blue-black shoes, wristwatch, silhouette and existing action directions. Do not substitute the older character. The backpack is newly drawn for THIS body: compact charcoal/navy canvas, small front pocket, dark shoulder straps snugly following both shoulders. It sits on the UPPER BACK and ends above the belt, at most half the torso height and no wider than the torso. For SOUTH only shoulder straps visible, NO bag on chest. For WEST show the pack behind the torso on image right; for EAST behind torso on image left; for NORTH center the small pack on the upper back, hands and belt unobstructed. Backpack follows all movement, punches, sitting, leaning and crouching without floating or enlarging the head/body. No reuse of any old backpack asset. Keep original canvas organization with generous padding around EACH full head, hands, feet and backpack. Clean crisp 2D pixel art, consistent logical pixel size, nearest-neighbor edges, dark outlines, restricted palette. Completely transparent background: no ghost figures, colored backdrop, shadows, text, grid lines or labels. No objects or furniture beyond the existing held coffee cup where present. Only add the newly fitted backpack; preserve all existing character poses and appearance.
```

## Mochila — Actions

Arquivo: `Assets/Resources/Varginha/TeamArt/EdelzioActionsBackpackV3.png`.

```text
Edit target: the attached NEW Edélzio character animation sheet. Create a NEW complete animation sprite sheet WITH a newly designed fitted backpack in every frame. EXACT six columns, four rows. Keep all 24 existing poses: first three sitting/typing, final three holding/lifting/drinking coffee, direction order unchanged. Rows SOUTH front, WEST left-facing, EAST right-facing, NORTH back-facing. Preserve the reference character's exact current proportions, full rounded brown hair, glasses, moustache/goatee, ochre polo, dark pants, blue-black shoes, wristwatch, silhouette and existing action directions. Do not substitute the older character. The backpack is newly drawn for THIS body: compact charcoal/navy canvas, small front pocket, dark shoulder straps snugly following both shoulders. It sits on the UPPER BACK and ends above the belt, at most half the torso height and no wider than the torso. For SOUTH only shoulder straps visible, NO bag on chest. For WEST show the pack behind the torso on image right; for EAST behind torso on image left; for NORTH center the small pack on the upper back, hands and belt unobstructed. Backpack follows all movement, punches, sitting, leaning and crouching without floating or enlarging the head/body. No reuse of any old backpack asset. Keep original canvas organization with generous padding around EACH full head, hands, feet and backpack. Clean crisp 2D pixel art, consistent logical pixel size, nearest-neighbor edges, dark outlines, restricted palette. Completely transparent background: no ghost figures, colored backdrop, shadows, text, grid lines or labels. No objects or furniture beyond the existing held coffee cup where present. Only add the newly fitted backpack; preserve all existing character poses and appearance.
```

## Mochila — Interactions

Arquivo: `Assets/Resources/Varginha/TeamArt/EdelzioInteractionsBackpackV3.png`.

```text
Edit target: the attached NEW Edélzio character animation sheet. Create a NEW complete animation sprite sheet WITH a newly designed fitted backpack in every frame. EXACT six columns, four rows. Keep all 24 existing crouch/collect and standing reach poses, direction order unchanged. Rows SOUTH front, WEST left-facing, EAST right-facing, NORTH back-facing. Preserve the reference character's exact current proportions, full rounded brown hair, glasses, moustache/goatee, ochre polo, dark pants, blue-black shoes, wristwatch, silhouette and existing action directions. Do not substitute the older character. The backpack is newly drawn for THIS body: compact charcoal/navy canvas, small front pocket, dark shoulder straps snugly following both shoulders. It sits on the UPPER BACK and ends above the belt, at most half the torso height and no wider than the torso. For SOUTH only shoulder straps visible, NO bag on chest. For WEST show the pack behind the torso on image right; for EAST behind torso on image left; for NORTH center the small pack on the upper back, hands and belt unobstructed. Backpack follows all movement, punches, sitting, leaning and crouching without floating or enlarging the head/body. No reuse of any old backpack asset. Keep original canvas organization with generous padding around EACH full head, hands, feet and backpack. Clean crisp 2D pixel art, consistent logical pixel size, nearest-neighbor edges, dark outlines, restricted palette. Completely transparent background: no ghost figures, colored backdrop, shadows, text, grid lines or labels. No objects or furniture beyond the existing held coffee cup where present. Only add the newly fitted backpack; preserve all existing character poses and appearance.
```
