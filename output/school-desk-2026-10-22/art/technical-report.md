# Mesa do Renan - relatório de pixel art

## Escopo e identidade

Três objetos estáticos, mesma vista de cima em três quartos da sala: mochila cinza com bolsos/alça, notebook aberto visto pela parte traseira da tampa e pincéis com tampas azul/vermelha/preta. Fontes do Renan e do cenário foram examinadas; o plano aprovado pede essa composição. A interpretação dos objetos novos usa contornos escuros e volumes cartunescos. Nenhum rosto, pose, sprite de personagem, colisão ou puzzle foi redesenhado.

## Fonte e produção

Nova imagem produzida com image_gen em RGBA 2172x724, conservada como `generated-source.png`. O original gerado permanece no diretório de geração. `provenance.json` registra hash, origem e aprovação do escopo. A autorização para criar arte não foi rotulada como aprovação estética independente do resultado.

`Tools/SchoolDeskArt/prepare_art.py` gera a receita e copia a fonte. A skill monta os três recortes por deslocamentos inteiros, sem blur, rotação, repintura ou redimensionamento. A primeira tentativa foi recusada por alinhamento de meio pixel no recorte de largura ímpar dos pincéis. Acrescentar uma coluna transparente ao crop corrigiu esse registro; não houve exportação daquela tentativa.

## Especificação

- Atlas: **2400x672 RGBA**, três células **800x672**, uma linha, sem padding adicional. Retângulos e registro completos em `export/atlas.json`; plano de slicing em `export/unity-slices.json`.
- Três clips **estáticos**, um frame cada, hold 1000 ms, sem loop. Não são animações novas.
- PPU 100 no metadado de runtime, Pivot canônico (400,670). Recortes alpha apertados em `RenanDeskProps.json`; posicionamento no mapa pelo centro inferior visível, em (684,77), (717,77), (745,77).
- Limites visuais em world units: mochila 0,39x0,46; notebook 0,65x0,42; pincéis 0,30x0,14, preservando a proporção de cada sprite. A arte fonte não é reamostrada.
- Point, Clamp, mipmaps desligados, sem compressão, NPOT None, max texture size 4096, CPU read desligado. O Player confirmou a textura em 2400 pixels de largura, sem downscale do importador.
- O alpha original foi preservado, inclusive pixels semitransparentes de borda. O contrato declara essa condição; não se alegou uma paleta de 16/32 cores ou uma grade lógica menor do que a imagem realmente produzida. A paleta exata fica no inventário.

## Preservação e QA

`RenanProps.png`, seu JSON e `School.png` continuam intactos; a nova composição referencia apenas o atlas novo. Nenhum arquivo Aseprite foi produzido. A fonte disponível é PNG RGBA com receita reproduzível, não uma suposta fonte em camadas.

Skill pack: 3 frames e 3 clips. Validate com contrato: structural_pass, zero erros, zero warnings. Preview: `preview/contact.png`, os três clips estáticos e página HTML. O helper emitiu apenas um DeprecationWarning da API getdata do Pillow, sem afetar o resultado. O inventário preserva hashes e paleta.

Foram abertos a imagem gerada, o contact sheet, a captura `../captures/renan-desktop.png` e as capturas dos dois vidros. Todos os objetos estão visíveis, sem corte de conteúdo principal, mochila/notebook/pincéis separados e assentados no tampo. Não há loop de movimento a conferir. O notebook abre a fotografia, a colisão original da mesa não muda e uma segunda chamada não duplica objetos.

Unity 6000.6.0f1, Windows x64: importação e reprodução efetivamente exercitadas no Player de testes. 398/398 EditMode e 3/3 testes direcionados no Player passaram. A captura é da renderização do jogo, não uma montagem da prévia. Resultado: integrado e testado para o escopo aprovado; refinamentos posteriores continuam sujeitos ao playtest humano.
