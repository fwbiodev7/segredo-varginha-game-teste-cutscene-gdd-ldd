# Animações, sala de aula e interface — 3/10/2026

## Personagens

As novas folhas RGBA estão em `Assets/Resources/Varginha/StoryCharacters`. Foram geradas com as artes existentes como referência e importadas com Point, sem mipmaps, sem compressão e sem cópia legível na CPU. Os manifestos guardam recortes e ancoragem da cabeça; a execução reutiliza sprites e texturas. Os arquivos originais de `TeamArt` permanecem intactos.

| Folha | Quadros | Uso |
| --- | ---: | --- |
| WalkGray | 16 | Caminhada em quatro direções com mochila cinza |
| Life / LifeGray | 48 cada | Respiração, piscar, pulo e lavar o rosto, com e sem mochila |
| Seated / SeatedGray | 48 cada | Oito direções sentado; repouso e digitação, com e sem mochila |
| Punch | 32 | Preparação, contato, recuo e guarda em quatro direções, com e sem mochila |
| Driving | 16 | Edelzio ao volante, quatro vistas, direção à esquerda e à direita |
| SeatedDeskNorth | 6 | Tronco completo de costas: repouso e digitação no notebook, encaixado no assento |
| RenanTeaching | 16 | Escrever, apontar e explicar em quatro vistas |
| RenanProps | 3 | Mochila cinza, notebook e material de aula |
| OuzanaBiologist | 16 | Bióloga com jaleco: quatro vistas, respiração, caderno e amostra botânica |

Total: **297 quadros**. O pulo padrão é **K**, remapeável nas configurações; mantém a colisão no chão. O soco conserva **J**. Lavar o rosto e usar o notebook são ações da rotina da fase 2. O Fusca das fases 3 e 10 é independente e vazio: nenhuma camada de motorista é desenhada no carro. A folha Driving permanece disponível, sem ser aplicada ao veículo.

O Renan foi recriado a partir da referência enviada: cabelo cacheado, barba, óculos pretos, camiseta laranja, jaqueta assimétrica xadrez/escura e tênis claros. A fotografia pessoal não faz parte dos arquivos versionados. Nas fases 4 e 5 ele fica ao lado do quadro, alternando escrita e explicação. Sua mesa contém os três objetos próprios; os nove alunos permanecem sentados.

Ao sentar de costas, o corpo ocupa o espaço entre a cadeira e a mesa. Um recorte do encosto aparece à frente da parte inferior das costas. A cabeça permanece acima do encosto; a ação do notebook retorna à posição de pé e restaura a colisão ao terminar.

Somente na igreja (fase 8), Edelzio usa a altura visível do padre, aproximadamente 0,955 unidade. Nas demais fases conserva o tamanho normal de 1,50 unidade, também usado por Ouzana. Os pés mantêm seu apoio original; os caches separam as duas escalas.

## Interface e apresentação

O HUD usa ícones em pixel art para caderno, mochila e pausa, com dicas pequenas de tecla. A fonte Press Start 2P também aparece nos títulos, diálogos, botões e painéis. O menu conserva a imagem original do mistério e apresenta CONTINUAR, NOVA HISTÓRIA e CONFIGURAÇÕES.

A pausa captura o cenário uma vez em uma textura com 90 pixels de altura e largura ajustada ao formato da tela. O fundo desfocado cobre toda a tela, inclusive as laterais em tela cheia. Retomar libera essa textura. Os botões do HUD cobertos pela pausa não recebem cliques.

As entradas das fases 2 a 10 usam nove prévias em pixel art do próximo cenário, compartilhando um único atlas. Cada uma traz uma frase curta; a fase 2 usa “CADE A CHAVE?”. Um movimento discreto e fades curtos acompanham as imagens, sem antecipar revelações. As transições entre cenas usam escurecimento gradual. A opção de movimento reduzido encurta as transições. Os efeitos compartilham materiais e malhas; não usam uma captura de tela por quadro nem uma luz Unity por personagem. Não foi realizada uma medição de FPS em hardware diferente.

As referências visuais aprovadas foram as galerias oficiais de [Eastward](https://eastwardgame.com/media/) e [Stardew Valley](https://www.stardewvalley.net/media/). A composição e as artes do próprio jogo foram mantidas.

## Soluções e verificação

O [guia completo](GUIA_COMPLETO_PUZZLES.md) explica cada fase, os requisitos dos puzzles e as respostas. A fase 11 permanece planejada. Os resultados nativos da Unity e os limites da validação estão em [VALIDACAO_CAMPANHA.md](VALIDACAO_CAMPANHA.md).
