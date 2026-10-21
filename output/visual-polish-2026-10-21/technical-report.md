# Relatório técnico de arte — revisão visual 21/10/2026

Skills usadas: `pixel-art-sprite-sheet-master` e `imagegen`. Pedido e aprovações, narrativa, arquivos/cenas e testes estão em `RELATORIO.md` e `Docs/CampanhaOficial/POLIMENTO_VISUAL_E_ABERTURA_2026-10-21.md`. Fontes geradas e prompts resumidos: `art/PROMPTS_E_ORIGEM.md`.

## Identidade e especificações

O motorista usa `StoryCharacters/Driving.png`, linha leste, quatro frames originais, escala uniforme 2,05 e registro (400,708) no canvas 1280×720. Cabelo, rosto, óculos, roupa, mãos e volante desse atlas não foram repintados. Shell e painel cartunescos conservam a geometria da cabine anterior. O painel ocupa (552,325,780,260), com mostrador/rádio animados nos pontos já registrados.

Nova criança: oito poses sentadas de três quartos/costas, voltadas à direita; camiseta amarela, bermuda azul e cabelo curto da referência `Experiment/ChildEdelzio.png`. Fonte efetiva 1774×887, 4×2 células, oito poses a 125 ms, loop de 1 s. A base opaca medida em todos os crops fica em y=428. O atlas de QA usa crops inteiros explícitos com padding para 444×444, pivô documental (222,444), PPU documental 100 e sem resize de pixels. A integração usa recortes UV das células na textura e canvas 280×280. O apresentador preserva os três estados existentes de boca, não se declara uma animação nova de oito poses do jornalista.

Marcos: atlas 1536×1024, seis células 512×512, cada uma um objeto estático independente. Metadados não os tratam como uma animação de seis objetos trocados. Terreno opaco 2172×724. O mapa ocupa (447,262,665,214); os marcos ficam registrados nos obstáculos e cruzamentos existentes. Recortes/JSON/plano Unity estão em `art/skill-exports/landmarks`.

Shell: 1672×941; painel: 2172×724; sala de 1996, jornal e panorama: 1672×940. Importação Point, sem mipmaps, sem compressão/escala NPOT e sem cópia CPU. Limite de importação 4096 nos módulos largos, 2048 nas demais camadas. PPU 100 nos vidros adicionados; as composições cinematográficas usam UVs e coordenadas de tela, não novos clips Animator ou animações nas cenas autorais.

Novos fundos/atlas passam pelo resolvedor GPU em resolução aproximada de 1/4 da fonte, com alpha binário e Point. Sprites originais aprovados não passam por esse filtro. Paletas completas ficam compactadas em `art/skill-exports/palettes.zip`; contagem e hashes estão no inventário. As fontes têm muitos tons e pixels parcialmente transparentes; não foram anunciadas como paleta estrita.

## Revisão visual e continuidade

Foram examinados os contact sheets da criança, a sobreposição 0/7 (costura do loop), reprodução da página da skill e oito poses no Player. O plano de TV recua suavemente para a sala, com transmissão animada no CRT; a tomada permanece legível nos intervalos sem efeitos. Revisadas capturas reais da cabine, pico de sinal, ignição, escola, mapa inicial/completo, jornal, sala revelada e casa às 7h.

Os avisos de borda/alpha (8 criança + 6 marcos) são preservados: o gerador deixou alpha muito baixo fora das silhuetas. No runtime, a resolução de alpha remove esses resíduos; as capturas foram verificadas quanto a corte visível, halo e encaixe. Isso não concede aprovação artística humana, ainda pendente.

A revisão identificou janelas inferiores encobertas pela ordenação das paredes. Corrigida a camada de vidro para ficar acima do limite de ordenação da arquitetura, conservando os caixilhos. Panos adjacentes amostram uma única vista contínua. Um primeiro clareamento levantou demais os pretos; o material final usa curva suave que conserva preto e saturação. O Player final foi recompilado e capturado depois dessas correções.

Nenhuma cena de produção foi regravada. Scripts e importers foram compilados e executados em Unity 6000.6.0f1 e Player Windows. Resultados: 378 EditMode e 8 Player aprovados; build sem erros. A prévia de 72 quadros do prólogo e 114 da viagem usa imagens reais; seu intervalo de amostragem não é taxa de quadros do jogo. Fontes e camadas PNG permanecem editáveis como raster, sem alegação de arquivo Aseprite produzido.
