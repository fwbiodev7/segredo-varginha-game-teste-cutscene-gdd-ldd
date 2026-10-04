# Mapas enviados pelo autor — integração de 3/10/2026

As nove imagens mobiliadas enviadas pelo autor são a arte dos cenários das dez fases. A escola é compartilhada pelas fases 4 e 5. A escala mantém as proporções de cada imagem; a planta, as passagens e os móveis seguem a composição recebida.

| Fase | Imagem recebida | Arte no projeto | Captura Unity |
| --- | --- | --- | --- |
| 1 | Casa Aconchegante Sob o Luar | Child1996.png | [Infância](../../Preview/IllustratedMaps/Fase1_Integrada.png) |
| 2 | Casa Aconchegante sob a Luz da Lua | Adult2026.png | [Casa adulta](../../Preview/IllustratedMaps/Fase2_Integrada.png) |
| 3 | Rua Urbana Noturna em Pixel Art | Street.png | [Rua](../../Preview/IllustratedMaps/Fase3_Integrada.png) |
| 4–5 | Mapa Escolar Pixelado ao Amanhecer | School.png | [Escola](../../Preview/IllustratedMaps/Fase4_Integrada.png) |
| 6 | Pátio da Biblioteca em Pixel Art | Library.png | [Biblioteca](../../Preview/IllustratedMaps/Fase6_Integrada.png) |
| 7 | Clareira Florestal e Santuário em Ruínas | Forest.png | [Mata](../../Preview/IllustratedMaps/Fase7_Integrada.png) |
| 8 | Capela Medieval em Luz de Vitral | Church.png | [Capela](../../Preview/IllustratedMaps/Fase8_Integrada.png) |
| 9 | Casa-Laboratório Botânico Aconchegante | Ouzana.png | [Casa/laboratório](../../Preview/IllustratedMaps/Fase9_Integrada.png) |
| 10 | Oficina Vintage Sob a Chuva | Workshop.png | [Oficina](../../Preview/IllustratedMaps/Fase10_Integrada.png) |

As fontes estão em `Assets/Resources/Varginha/IllustratedMaps`. Os originais enviados permanecem intactos. Na fase 3, o trecho urbano se repete ao longo do percurso de 150 unidades para conservar a pane aos 55 e a chegada aos 120.

## Colisão e profundidade

`Layouts.json` contém dimensões, plantas, pontos de interação, luzes e retângulos de cada móvel nas coordenadas da imagem original. Há 156 peças e 114 trechos de parede antes da repetição da rua. As colisões usam o contato dos móveis com o piso, permitindo passar atrás dos encostos e das copas. As quatro cadeiras da cozinha/escritório da casa adulta têm contatos próprios, separados das mesas. Tapetes, quadros, janelas e outras peças decorativas não bloqueiam o personagem.

Recortes sobrepostos do mesmo cenário cobrem o personagem quando ele passa atrás de paredes, fachadas, árvores e móveis. Esses recortes compartilham a textura na GPU; não produzem cópias da imagem. O manifesto guarda contornos nas coordenadas normalizadas da imagem, e malhas trianguladas removem as margens de piso e os vãos côncavos dos recortes. A transparência vem da geometria; os PNGs recebidos permanecem intactos. A escala usa o retângulo de origem, mantendo-se estável ao reutilizar a malha entre fases.

O protagonista original da casa adulta e os personagens da igreja/laboratório também usam ordenação pelo contato dos pés. As divisórias verticais da casa recebem margem lateral de 0,12 unidade e uma cápsula adicional no corpo do personagem: a cabeça e os ombros ficam fora da parede sem fechar as portas. Os portais recebem trechos de piso contínuos. Na escola, nove alunos ficam sentados entre as mesas e os encostos, e Renan ensina junto ao quadro branco. O Fusca circula dentro dos meios-fios da rua enviada.

Os objetivos foram reposicionados: preparo e caixa na casa adulta, notebook do professor e conversa com Renan na escola, arquivo e banco no pátio, percurso e esconderijo na mata, altar e registros na igreja, experimentos da Ouzana e ferramentas na oficina. As ações, pistas, salvamento e transições da campanha continuam no sistema existente.

## Edélzio, Fusca e iluminação

As artes originais de Edélzio e do Fusca foram preservadas; novas animações do personagem ficam em `StoryCharacters`. Um material compartilhado aplica cor ambiente e luz local coerentes com cada imagem, com atualização a cada 0,1 segundo. A luz quente dos abajures e do banheiro e a luz azul das janelas alteram a cor do personagem conforme ele caminha. O carro recebe realces discretos, dois fachos alinhados aos faróis, feitos com uma malha compartilhada. Sombras de personagens apontam para longe da luminária mais próxima; ao ar livre, usam uma sombra curta sob os pés. Esse desenho limita o custo; não equivale a uma medição de FPS em todas as máquinas.

As texturas usam Point, sem mipmaps, sem compressão destrutiva e sem uma cópia de leitura na CPU. Os cenários são carregados sob demanda; os recortes são reutilizados. A geometria de contorno dos móveis e árvores é aplicada uma vez no início da execução, em coordenadas do retângulo do sprite, conforme a [API oficial da Unity](https://docs.unity3d.com/6000.0/Documentation/ScriptReference/Sprite.OverrideGeometry.html).

## Edições necessárias para objetos móveis

Foram produzidas duas variantes pela ferramenta integrada de edição de imagens, preservando as fontes e a composição. Os prompts abaixo registram a intenção das edições. A posição do objeto móvel vem do manifesto e sua aparência vem do original.

- `WorkshopEmpty.png`: retirar apenas o Fusca azul central da oficina e recompor o concreto rachado e suas manchas no lugar ocupado pelo carro. Preservar exatamente paredes, móveis, lâmpadas, rua, escala, enquadramento, resolução e estilo pixel art. Referência: `Oficina Vintage Sob a Chuva.png`. Resultado de geração: `exec-301118b4-e29e-45d7-8b5a-b31aeaa77906.png`.
- `AdultEmpty.png`: retirar apenas a pequena mochila azul/cinza ao lado direito do criado-mudo, no quarto superior esquerdo, recompondo o piso de madeira. Preservar todos os outros objetos, paredes, iluminação, jardim, Fusca externo, enquadramento e estilo. Referência: `Casa Aconchegante sob a Luz da Lua.png`. Resultado de geração: `exec-b4772857-4044-42dc-a211-9e534d4fa1a1.png`.

Assim, a mochila desaparece ao ser recolhida e o Fusca deixa a vaga da oficina sem uma segunda cópia pintada no piso. As imagens foram inspecionadas antes da importação.

## Reprodução

A integração acontece no carregamento de cada fase; não altera os arquivos originais `.unity`. O manifesto e os PNGs já estão versionados e não dependem da pasta Downloads em outra máquina. `CampaignIllustratedMapBuilder.Capture` gera as referências temporárias no Editor; `CaptureRuntime` registra uma fase em execução. A ferramenta de importação é destinada às fontes locais desta sessão e não precisa ser executada para jogar uma cópia do repositório.

O kit anterior para organização manual representa as plantas anteriores a estas imagens. Ele permanece como entrega histórica e não foi substituído por falsos móveis transparentes extraídos de uma composição opaca.

## Correções da entrada e do assento

Na escola, o travessão da entrada tem uma camada em primeiro plano ancorada à base da fachada; as duas colunas do portão têm colisões próprias, mantendo o vão central atravessável. A abertura lateral direita foi fechada com recorte da mesma parede original e uma base física. Os PNGs enviados continuam intactos.

A pose SeatedDeskNorth mostra cabeça, tronco e pernas dobradas. O encosto é uma camada separada que cruza naturalmente a parte inferior das costas, mantendo o corpo entre a cadeira e a mesa.
