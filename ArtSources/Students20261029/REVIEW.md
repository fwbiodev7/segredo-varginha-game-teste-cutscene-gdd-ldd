# Alunos existentes — referência de 29/10/2026

## Entrega

Nove designs da referência fornecida: Fábio, Marcos, Matias, AnnaSofia,
AnaTavares, Yasmin, Pedro, LuisMartins e LuisMiguelMessias. O identificador
`AnnaSabia` foi preservado nos arquivos para compatibilidade com o catálogo e
os GUIDs existentes; a identidade visual adotada é AnnaSofia, da referência.

- Fonte original preservada integralmente em `Reference_AlliedStudents.jpeg`.
- Versões anteriores preservadas em `Before/`.
- `Assets/ArtSource/Allies/{ID}.png`: fontes RGBA 512 × 512, 16 células de 128.
- `Assets/Resources/Varginha/Allies/{ID}.png`: atlas RGBA 256 × 256, 16 células de 64.
- Ordem: S, W, E, N; quatro quadros fornecidos por direção.
- Apoio: y=58 exclusive na origem superior; equivale a yMin=6 no Unity.
- Pivô runtime (0.5, 0.5), PPU `64 / 1.35`, conforme consumidor existente.
- Altura máxima: AnnaSabia 44, AnaTavares 56, demais 50 pixels, conforme importador.

## Procedimento

Foi feita extração técnica de personagens inteiros, sem geração de desenhos,
recoloração, transplante de partes, espelhamento ou deformação anatômica.
Os RGB provêm da JPEG enviada; a remoção de fundo opera pela borda de cada
recorte e sela pequenos furos da máscara causados por compressão JPEG.
O encaixe no atlas usa uma única razão por personagem e amostragem nearest
neighbour com a mesma origem inferior do exportador Unity.

Os retângulos reais da fonte, SHA-256, razão de escala, ações, direções,
retângulos do atlas e timings de prévia constam em `{ID}/atlas.json`.
As paletas completas, sem quantização da imagem, estão em `{ID}/palette.json`.
Materiais preservados: cabelo castanho/preto, pele individual da referência,
camisa branca, mangas verdes ou amarelas conforme o aluno, calça escura e
tênis individual. A contagem de cores da JPEG não constitui paleta artística
intencional; nenhum limite arbitrário de cores foi aplicado.

## Verificações

- 9 auditorias estruturais PASS, com 0 erros.
- 144/144 quadros não vazios e distintos.
- 144/144 quadros com apoio yMin=6 e margens laterais de pelo menos 4 pixels.
- Os RGBA dos atlas entregues coincidem com os arquivos Resources na revisão.
- GUIDs originais mantidos; nenhuma alteração de código de gameplay.
- `Contact_9Students.png` e `Contact_All144Frames.png` inspecionados visualmente.
- Contatos, quatro GIFs e HTML de inspeção por personagem em `{ID}/preview/`.

Os alertas genéricos do helper relativos a `textureType`, PPU e compressão
devem ser interpretados pelo consumidor real: este projeto usa
`TextureImporterType.Default` com `Sprite.Create` em runtime; o PPU é definido
no código, e o importador existente define Point, sem mipmaps, legibilidade e
compressão Uncompressed. O helper não executa Unity nem resolve overrides.

## Limites

A origem é uma JPEG com arte já reduzida; ela tem compressão e pixels de borda
misturados com o fundo. A extração preserva os designs e não equivale à fonte
lossless original. O movimento frontal/traseiro é sutil na própria referência;
nenhum novo passo foi inventado. As sequências completas foram inspecionadas
no contato; reprodução temporal e transições no Unity ficam para a validação
do agente principal. Não há aprovação artística humana registrada e os GIFs
são prévias quantizadas. A autoridade continua sendo PNG/JSON. Status:
`visual-reviewed`, configuração de importação e runtime ainda dependem do
teste integrado do agente principal.

Reprodução: `Tools/ExtractStudentReference20261029.py` e
`Tools/ReviewExtractedStudents20261029.py` com Pillow. Os helpers da skill
geram paleta, auditoria e prévias a partir dos manifestos.
