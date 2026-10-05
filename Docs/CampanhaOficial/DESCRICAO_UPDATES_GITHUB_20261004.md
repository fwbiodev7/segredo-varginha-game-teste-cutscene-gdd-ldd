# Campanha 11–21: continuação, combate final, arte e polimento

Esta atualização entrega a continuação da investigação até o encerramento na Industrial e corrige os problemas de sobreposição encontrados durante as partidas. A fase 20 reúne o combate contra a manifestação hostil da entidade; a fase 21 mantém a entidade ferida, a travessia segura, o encerramento do selo e a conclusão da história. O polimento trabalha sobre os mapas ilustrados, controles, inventário, sistema de saúde e ataques existentes.

## O que muda para o jogador

- A entidade grande tem uma entrada gradual com ruptura luminosa, enquadramento de câmera, antecipação e tremor. Os comandos permanecem bloqueados durante essa entrada e são liberados depois dela.
- O chefe respira, prepara os ataques, investe com deslocamento real, emite um pulso, reage a dano e muda de ritmo ao alcançar metade da vida. Os avisos no chão e a recuperação permitem entender quando desviar e atacar.
- Socos acertam o corpo visível do chefe e dos filhotes. A área que recebe golpes é separada da colisão dos pés, mantendo a movimentação top-down.
- A arena é ampla e mais clara, com espaço para Edelzio, a entidade, até cinco ecos, três alunos e um aliado de apoio.
- A mochila permite escolher três alunos e um apoio entre Renan, Ouzana e Padre Fábio. Aparecem apenas os equipados; cada aluno ataca individualmente.
- A vida de Edelzio fica no canto superior esquerdo; a vida do chefe, no topo central. Os anúncios de habilidades usam texto pixelado, bordas e cores dos personagens.
- O final apresenta créditos maiores subindo sobre preto de tela inteira, com os nomes solicitados e um último toque de mistério.

## Correções por fase

| Fase | Conteúdo e ajustes |
|---|---|
| **11 — Arquivos alterados** | Industrial à noite, comparação dos arquivos e conversas preservadas. Proteção do corpo nas divisórias e revisão da profundidade dos móveis. A turma continua sentada nas cadeiras da sala. |
| **12 — Casarão de Zé Gomes** | Jardim e térreo conectados. Colisões das divisórias, contornos dos móveis e ordem de sobreposição corrigidos. Planta, reagente, chave do escritório e passagem de serviço continuam com seus papéis atuais. |
| **13 — Registros de 1898** | Barreiras do porão e pilares corrigidos. Páginas e materiais podem ser examinados de pontos livres; permanecem a reconstrução dos registros e a lembrança de 1898. |
| **14 — O que Fábio escondeu** | Reaproveita a igreja existente e mantém suas proporções próprias. A conversa com o padre, os registros e a associação das datas foram conferidos. |
| **15 — A noite interrompida** | Mantém a casa infantil e os quatro vestígios da lembrança. Caminhos, sequência do puzzle e apresentação da memória conferidos. |
| **16 — A descida** | Colisões da água seguem os tanques e deixam as passarelas livres. Paredes internas protegem o corpo; a leitura da água fica acessível do piso seco. Padre Fábio aparece proporcional aos demais adultos. |
| **17 — Câmaras do selo** | Três áreas conectadas, reguladores acoplados e abrigo preservados. Fonte visual mais nítida, grades com colisão e pontos de leitura alcançáveis. As três áreas compartilham a textura da câmara e conservam os símbolos e leituras de cada regulador. |
| **18 — Criatura ferida** | Câmara de contenção com arte mais nítida, mantendo os acessos laterais. Colisões das grades e pontos de evidência corrigidos. Ouzana fica no corredor livre ao lado do mecanismo. |
| **19 — Acordo esquecido** | Preserva o quintal, o clarão e a reconstrução do acordo temporário. Evidências, memória e sequência de solução conferidas. |
| **20 — Verdadeiro segredo** | Arena dedicada, preparação dos três circuitos, entrada cinematográfica, chefe animado, filhotes, formação selecionável, comandos individuais, hitboxes, HUD e efeitos de combate. |
| **21 — Retorno** | Mantém o selo durante a travessia e permite encerrá-lo depois. Recarregar o save após a travessia não faz a entidade reaparecer. Padre proporcional, reunião dos amigos, retorno à escola, cópias de Renan, caderno, Fusca e créditos. |

## Personagens e ataques

Os nove alunos receberam sprites baseados nas fotografias e roupas informadas: Fabio com cabelo longo dividido ao meio e roupa preta de academia; Marcos moreno, cabelo crespo, camisa roxa e shorts de vôlei; Matias cacheado com kimono e faixa azul; Anna Sabia e Tavares cacheadas com roupa amarela de tênis de mesa; Yasmin com cabelo preto, pele mais clara e roupa de rock; Pedro com cabelo longo cacheado, óculos e roupa de rock; Luis Martins com cabelo liso, moletom vermelho e calça preta; Luis Miguel Messias com buzzcut, barba e camisa do Atlético Mineiro. Marcos e Luis Martins não usam óculos; Sabia não possui aparelho e Tavares possui. As fotografias pessoais não foram incluídas no repositório.

As duas habilidades de cada aluno mantêm os conceitos do guia: ondas e piano de Yasmin, guitarra de Pedro, jiu-jitsu de Matias, katana e anilhas de Fabio, vôlei de Marcos, tênis de mesa de Sabia/Tavares, voz de Messias e tinta de Martins. O timing separa preparação e contato; partículas, rastros, impactos e poses apresentam o golpe antes de aplicar o dano.

Renan emite interferência laranja pelo notebook; Ouzana lança um frasco e deixa uma nuvem verde de reagente; Padre Fábio cria uma proteção dourada com cruzes, utilizando seu design já existente. A proteção tem uma área limitada. O padre possui escala maior nas fases 16, 20 e 21, preservando o tamanho original da igreja.

**Comandos da equipe na fase 20:** 1, 2 e 3 acionam alunos específicos; L ou mouse direito alterna entre os equipados disponíveis, um por comando; H aciona o apoio. No controle, LB comanda um aluno e RB o apoio. Um novo comando de aluno aguarda o término do ataque atual. As recargas continuam individuais; nuvem e aura podem permanecer durante os próximos golpes. A formação escolhida é salva.

## Sobreposição, iluminação e desempenho

A profundidade segue o contato no chão usando o sistema de Y-sorting existente. Colunas longas usam trechos independentes; contornos dos recortes evitam trazer cantos de piso para a frente dos personagens. Colisão e navegação consideram as mesmas paredes, incluindo a proteção do tronco nos pilares. Não houve migração dos mapas para Tilemaps nem substituição dos sistemas de inventário, saúde ou puzzles.

Os efeitos ofensivos ficam acima dos personagens e do chefe; sombras e avisos permanecem no piso. O marcador visual comporta a extensão das animações. Os efeitos autorais usam de 20 a 32 partículas por ataque, com reutilização dos objetos de apresentação e de uma fonte de áudio por aluno. Fontes de cenário, texturas e sons são compartilhados. Dano, alcance e recargas dos alunos permanecem os mesmos.

Também foram corrigidos acessos nulos de apresentação/GUI e o crescimento recursivo das sombras dos apoios, que podia travar a Unity. Os avisos relevantes de busca de objetos nos testes foram adequados à API atual. Ajustes já preparados de compatibilidade na biblioteca, oficina/Fusca e capítulos anteriores acompanham a integração da campanha; estão documentados separadamente em `Docs/CampanhaOficial`.

## Créditos

- **devs:** fabio, joao pedro matias, asafe e marcos.
- **arte e som:** yasmin, luis martins, giovana e gustavo.
- **narração:** luis miguel messias.
- **narrativa:** bianca, joao guilherme e karol.
- **testes e Q/A:** anna sabia, tavares, moscardini e pedro.
- **com a participação de:** edelzio, renan, ouzana, professor fabio e ET de Varginha.

Os créditos aparecem depois da última fala. O encerramento com “ALGUNS SEGREDOS AINDA ESPERAM NO ESCURO.” e “FIM...?” sugere uma possível continuação sem mudar a conclusão do acordo.

## Validação e evidências

- **5/5 testes de gameplay deste escopo aprovados**, cobrindo investigação por área, puzzles, troca de mapas, 18 ataques, três apoios, mochila por mouse/teclado, comandos separados, hitboxes, limite/reuso dos ecos, pausa, derrota/repetição e retorno com recarga do save.
- Relatório de navegação dos **15 mapas/áreas** com spawns e pontos de layout alcançáveis.
- Capturas e conferência visual das fases 11–21, entrada do chefe, impacto sobre o chefe, equipe, padre e créditos.
- Compilação e Console conferidos após as alterações. As evidências finais ficam em `Docs/CampanhaOficial/ValidacaoPolimento*` e `Preview/Polish11a21`.
- A revisão não declara aprovação da suíte inteira de testes antigos nem ausência universal de bugs. O relatório detalha o que foi exercitado.

As fontes, versões anteriores e prompts dos mapas refinados ficam em `ArtSources/Polish20261004`; as fontes dos alunos e do combate permanecem em `ArtSources`. O save real do jogador é preservado após as verificações.

## Material ainda pendente

Falta a gravação da **voz infantil no rádio da fase 16**. A fala é apresentada em texto com o ambiente sonoro existente. Não é necessário decidir novos puzzles ou alterar a narrativa para utilizar as correções desta branch.
