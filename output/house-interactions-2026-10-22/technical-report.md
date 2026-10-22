# Arte e integração visual — 22/10/2026

**Escopo aprovado:** móveis de costas, itens removíveis, chave na mesa do café, escala das poses, paleta do quintal e camadas do Fusca. Arte criada pelo `image_gen` integrado, seguindo a skill `pixel-art-sprite-sheet-master`. As revisões da coluna fosca e do fundo opaco da chegada corrigem o mesmo trabalho aprovado.

## Fontes e preservação

- Fontes originais `Adult2026.png`, `AdultEmptyDivisoes.png`, `Driving.png` e seus GUIDs preservados. Não foi reconstruído o rosto ou as mãos do protagonista.
- A variante `AdultInteractiveBase.png` retira os objetos antigos somente nas regiões de `art/clean-base-provenance.json`. O script verifica igualdade exata de todos os pixels fora da máscara. Inclui a torneira inteira e os pés da mesa, que ultrapassavam os recortes anteriores.
- Café, notebook e luminária são sprites RGBA separados. Não se reutiliza mais um retângulo do mapa com pixels de madeira junto à xícara.
- Os PNGs de origem gerados, hashes, receitas e metadados foram preservados. Não foi criado um arquivo Aseprite com camadas; os PNGs não são apresentados como fonte editável desse formato.

## Especificações

| Grupo | Geometria e registro | Runtime |
|---|---|---|
| Sete móveis | Canvas 210×130 por frame, padding 1, PPU 100, pivô de apoio [105,129] | Atlas `HouseFurnitureBacks.png` + JSON de recortes alpha |
| Quatro itens | Canvas 64×64, padding 1, PPU 100, pivô inferior [32,64] | `HouseRoutineItems.png` + JSON; xícara jogável 23×25 pixels do mapa |
| Coluna do Fusca | Sprite 32×12; canvas canônico 34×14, padding 1; visual 72×27 no canvas 1280×720 | `FuscaSteeringColumn.png`, textura Point, sem mipmaps |
| Base da casa | 1888×833, mesma extensão da imagem original | Nova variante, sem substituir a fonte antiga |

Normalização mecânica: crop alpha, limiar 128 antes do recorte, nearest-neighbor, montagem sem interpolação. A paleta é a dos PNGs inspecionados; não há alegação de que o gerador tenha limitado a arte a quatro cores exatas.

## Revisão visual

Foram abertos os contact sheets dos sete móveis, dos quatro itens e da coluna revisada. Os móveis usam vistas traseiras, com tampos, planta/luminária e escala compatíveis com suas regiões. A integração visual fica em filhos dos nós originais; o ajuste de desenho não escala os colliders.

No Fusca, os quatro sprites originais de direção são registrados pelo cubo do volante. A sequência de desenho é paisagem, cabine, painel/instrumentos, coluna, Edelzio com volante e acabamento dianteiro. Ponteiro e marcações não são desenhados sobre o aro do volante. O painel foi reposicionado para deixar o velocímetro atrás do volante. O primeiro suporte foi descartado por excesso de detalhe; a versão final é menor, fosca e sem os parafusos e brilhos destacados.

A chegada usa o mapa real da escola. O fundo de toda a área cinematográfica é opaco, incluindo as margens que antes revelavam a casa. A tomada `world`, usada nas ações da casa, preserva a visão da gameplay.

A grama e as plantas aproximam-se dos verdes oliva das árvores por correção de matiz que conserva textura e luminosidade. Foram removidas as exclusões retangulares que produziam manchas no quintal.

## Validação e evidências

- `art/furniture-v2/validation/validation.json`: sete frames, zero erros e zero warnings.
- `art/items/validation-v2/validation.json`: quatro frames, zero erros; quatro avisos de contato com a borda inferior do canvas. Revisados: são os apoios inteiros no último pixel, sem corte de conteúdo. O atlas possui padding e o recorte de runtime usa o bbox alpha completo.
- `fusca/validation-v2/validation.json`: um frame, zero erros e zero warnings.
- As prévias são estáticas, não animações novas dos móveis. A animação de direção reutiliza os quatro frames originais e foi conferida no Player Windows, incluindo os frames em movimento e a retomada na escola.
- Os resultados finais de Unity e os caminhos das capturas estão em `summary.json` e `RELATORIO.md`. Falhas intermediárias foram preservadas nos relatórios, corrigidas e retestadas; elas não são contadas como aprovação.

## Prompts utilizados

1. **Móveis:** atlas transparente de geladeira, fogão, pia/armário, estantes e cômoda vistos de costas; mesa de café e escrivaninha vazias; perspectiva alta do mapa e estilo pixel art original. Preservar planta e luminária da cômoda.
2. **Base limpa:** editar a casa original para retirar somente os sete grupos substituídos, incluindo torneira, acessórios e sombras; preencher com o mesmo piso horizontal e preservar paredes, caminhos e demais objetos. A edição gerada foi restringida pela máscara mecânica na composição final.
3. **Itens:** atlas transparente 2×2 com xícara cheia, notebook aberto, luminária de mesa e xícara vazia. Nenhuma mesa, piso, placa ou fundo junto aos objetos.
4. **Coluna revisada:** refazer o suporte usando a cena real como referência, curto, fosco, preto/carvão, com clusters grandes de pixel; remover cromado, brilhos brancos, parafusos e detalhes salientes; sem volante, mãos ou personagem. Fontes e revisão ficam em `fusca/provenance-v2.json`.

Ferramentas: Unity 6000.6.0f1/Windows x64, `sprite_pipeline.py` da skill e Pillow para normalização/montagem. A validação estrutural não substitui a revisão visual nem o playtest humano.
