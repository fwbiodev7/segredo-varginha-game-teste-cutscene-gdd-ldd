# O Segredo de Varginha

Roteiro de apresentação para 30/10/2026. Preparado em 27/10. Duração sugerida: 12 a 15 minutos, incluindo uma demonstração de até 2 minutos. As telas indicadas abaixo são uma sequência de apoio sugerida para a apresentação completa; a daily de 27/10 resume a entrega mais recente.

## 1. A proposta do jogo

Tempo: 1 minuto. Tela de apoio: menu do jogo.

“Bom dia. Nosso projeto é O Segredo de Varginha, um jogo 2D de terror, investigação e aventura narrativa para PC. A ideia parte do imaginário em torno do caso de Varginha e cria uma história fictícia sobre memória, tecnologia e um segredo escondido na cidade.

O jogador controla Edelzio, um professor de Desenvolvimento de Sistemas. Quando adulto, ele encontra vestígios de uma noite da infância que não consegue lembrar direito. A partir daí, objetos comuns, como um caderno, um notebook e o Fusca, passam a fazer parte da investigação.

Nosso objetivo foi fazer o jogador descobrir a história participando dela: observar o cenário, conversar, comparar pistas e agir quando a situação exige. Essa combinação orientou tanto os sistemas quanto a construção dos mapas.”

## 2. A ideia e o primeiro protótipo

Tempo: 1 minuto e 20 segundos. Tela de apoio: registro do protótipo e linha do tempo.

“O planejamento reuniu referências de exploração e terror, como Resident Evil, Silent Hill, Outlast e Stranger Things. O que aproveitamos dessas referências foi a descoberta gradual do mistério, o uso de documentos e a tensão durante a exploração. A ambientação e os personagens seguem a proposta própria do nosso jogo.

O primeiro registro disponível no histórico do projeto é de 11 de setembro. Naquele momento, já havia um protótipo com gameplay e pixel art. Nos dias seguintes, o trabalho avançou sobre os controles, os aliados e os testes. Durante setembro, várias revisões corrigiram sprites, ataques, iluminação e interação com os cenários.

Esse começo ajudou a entender o tamanho real do trabalho. Uma mecânica funcionar sozinha não garantia que ela funcionaria junto com o diálogo, a câmera e o save. Por isso, o protótipo também serviu para encontrar problemas de integração antes de ampliar a campanha.”

## 3. A evolução da campanha

Tempo: 1 minuto e 20 segundos. Tela de apoio: sequência dos capítulos.

“No início de outubro, o projeto ganhou uma campanha experimental orientada pelo GDD e pelo LDD. O GDD organiza a proposta do jogo, os sistemas e a narrativa. O LDD detalha o percurso, os objetivos e os encontros de cada ambiente.

A campanha chegou a ser organizada em 21 fases internas. Depois, revisamos esse percurso para diminuir repetições e manter o que fazia a investigação avançar. O resultado atual tem 14 capítulos para o jogador. Alguns capítulos possuem mais de uma área, por isso o número de cenas do Unity é maior que o número de capítulos.

Essa mudança exigiu cuidado com os saves. Os identificadores antigos continuam reconhecidos, e a migração aproveita pistas e puzzles já concluídos. Assim, a reorganização do percurso não obriga quem já jogou a repetir tudo. Também mantivemos os registros antigos no projeto para acompanhar como as decisões mudaram ao longo do desenvolvimento.”

## 4. A história e o papel do jogador

Tempo: 1 minuto e 20 segundos. Tela de apoio: infância, casa adulta e personagens.

“A história começa em 1996, com Edelzio criança. Um clarão interrompe aquela noite e deixa uma lembrança incompleta. Trinta anos depois, a rotina antes de ir para a escola apresenta as ações básicas ao jogador. O desaparecimento da chave do Fusca leva até uma caixa e às páginas de um caderno antigo.

Renan ajuda a comparar registros e arquivos. Padre Fábio guarda informações do Livro do Tombo. Ouzana investiga as alterações biológicas e participa da análise das marcas. Os documentos conectam os acontecimentos de 1898, a noite de 1996 e as interferências do presente.

Vou evitar explicar todas as revelações para preservar a experiência de quem for jogar. Mas existe uma distinção importante no final: a criatura ferida e a manifestação enfrentada em combate têm papéis diferentes. A investigação ajuda o jogador a entender essa diferença e a completar o procedimento de retorno. A versão atual possui uma conclusão implementada; os finais alternativos que aparecem nos documentos iniciais continuam sendo uma possibilidade de expansão.”

## 5. Investigação, puzzles e continuidade

Tempo: 1 minuto e 20 segundos. Tela de apoio: pistas da casa, mapa das trilhas e Livro do Tombo.

“A investigação precisa dar informação suficiente para o jogador raciocinar. Na casa, as pistas observáveis conduzem à chave e à caixa. Na biblioteca, os registros ajudam a escolher uma passagem para cada trecho da trilha. O jogador pode corrigir uma escolha sem perder as outras.

No Livro do Tombo, é necessário reconstruir fragmentos e relacioná-los às evidências encontradas. Isso aproxima o puzzle da história, em vez de usar uma senha sem contexto. O caderno e as dicas ajudam quem precisa retomar uma informação.

Também cuidamos da continuidade. O save guarda o progresso e os checkpoints, e os capítulos usam saídas controladas. O botão de continuar precisa aceitar um avanço por vez. Quando o jogador aperta rapidamente, o comando seguinte não pode pular outra fase. As revisões mais recentes verificaram tanto a tela de conclusão quanto a chegada à cena seguinte.”

## 6. Exploração, combate e o Fusca

Tempo: 1 minuto e 10 segundos. Tela de apoio: mata, aliados e viagem.

“A exploração muda de ritmo ao longo da campanha. Na mata, a perseguição e os checkpoints criam tensão. Em outros trechos, o foco é ler o ambiente e conversar. Quando há combate, o jogador conta com golpes direcionais, esquiva e ajuda dos alunos e personagens de apoio.

Os ataques têm preparação, impacto e recuperação. A intenção é deixar a ação compreensível, inclusive quando alguém aperta o botão várias vezes. Um problema que corrigimos foi o salto de poses em uma taxa baixa de quadros, que podia tornar o movimento difícil de acompanhar.

O Fusca conecta a narrativa e os deslocamentos. A viagem cinematográfica apresenta a pane, o rádio e a chegada à escola. A oficina tem suas interações próprias. Os testes de física também verificam aceleração, frenagem, ré e a saída do contato com os limites. Isso não significa que todas as viagens sejam trechos de direção livre: cada passagem usa a mecânica prevista para ela.”

## 7. Tecnologia, arte e áudio

Tempo: 1 minuto e 10 segundos. Tela de apoio: cenário e animação em jogo.

“O jogo utiliza Unity e C#. O código separa o progresso da campanha, a apresentação dos diálogos, os puzzles e os controladores dos personagens. Git registra as mudanças e permite relacionar uma correção à versão que foi testada.

A apresentação visual usa cenários ilustrados e personagens em pixel art. Uma parte importante do trabalho foi encaixar esses recursos: profundidade dos móveis, tamanho dos personagens, ponto de apoio dos pés e recortes das animações. Os ajustes mais recentes preservam os desenhos e os quadros completos dos personagens.

No áudio, o projeto combina ambientes, música e efeitos. Sons de ações precisam coincidir com o contato mostrado na animação, como a xícara chegando à boca. Também integramos os arquivos recebidos para o menu e a igreja. Há preparação de roteiros e canais para voz, mas isso não equivale a dizer que toda a dublagem humana já foi gravada. Ferramentas de IA apoiaram partes do processo de código e recursos visuais; os materiais e as integrações têm registros próprios no projeto.”

## 8. Testes e dificuldades encontradas

Tempo: 1 minuto e 20 segundos. Tela de apoio: exemplos de antes e depois.

“A dificuldade principal foi fazer os sistemas funcionarem juntos. Encontramos personagens recortados nos limites de um quadro, detalhes do piso que pareciam um remendo, objetos que precisavam respeitar a profundidade e comandos repetidos que não podiam causar ações duplicadas.

Para investigar esses problemas, usamos testes no Editor e no Player Windows, além de capturas reais. Os testes verificam regras, interação, save e transições. A inspeção das imagens verifica aspectos que um resultado automático não resolve sozinho, como uma emenda no piso ou uma pose visualmente estranha.

A auditoria de cenário de 26 de outubro percorreu 18 ambientes, 302 móveis e 1.018 aproximações físicas. Quatro objetos sem uma aproximação livre ficaram registrados como limite daquela auditoria. Essas contagens descrevem o recorte executado, e não a experiência de todos os jogadores. O teste com outras pessoas e em outros computadores continua importante para avaliar clareza, dificuldade e compatibilidade.”

## 9. Demonstração e entrega atual

Tempo: 2 minutos. Tela de apoio: build Windows 0.4.9-beta de 27/10.

“Agora vou mostrar um trecho da versão atual. Primeiro, observem como a investigação começa com elementos do próprio cenário. A rotina apresenta as interações, e as pistas levam à caixa sem precisar revelar a história inteira de uma vez.

Neste exemplo de puzzle, o jogador pode mudar uma associação e tentar novamente. O progresso guarda o que já foi encontrado. Ao concluir uma etapa, a saída leva ao próximo ambiente.

A entrega que acompanha esta apresentação é uma build Windows separada das versões anteriores. Gerar um novo executável foi necessário porque a build de 26 de outubro havia sido feita antes de algumas correções daquele mesmo dia. A nova versão reúne essas alterações e o fechamento desta revisão.”

Orientação para a demo: preparar previamente o trecho escolhido. Mostrar uma interação e um puzzle curto. Se o save disponível estiver no início, usar o menu e a investigação da casa, sem tentar percorrer a campanha inteira em dois minutos. Guardar capturas como apoio caso o computador da apresentação não consiga executar o jogo. Não usar a opção Nova História sobre um progresso que se pretende preservar.

## 10. Resultado e próximos passos

Tempo: 1 minuto. Tela de apoio: menu ou cena final escolhida sem spoilers.

“O resultado é uma campanha jogável que conecta a proposta inicial aos sistemas de investigação, exploração e combate. O projeto também mostra o quanto o acabamento depende de revisão: um personagem inteiro, uma pista legível e uma transição correta fazem diferença na compreensão do jogo.

Os próximos passos incluem colher feedback de jogadores, testar em outros computadores e ajustar a dificuldade e a mixagem a partir desses resultados. As ideias de expansão precisam respeitar esse trabalho de validação, para que o conteúdo novo não comprometa o que já funciona.

Para a equipe, o maior aprendizado foi transformar uma ideia de jogo em sistemas que precisam conversar entre si. A história orienta o percurso, a arte precisa funcionar com a câmera, e o teste precisa verificar a experiência que o jogador realmente recebe. Obrigado. Agora podemos responder às perguntas e deixar o jogo disponível para vocês experimentarem.”

## Preparação antes de apresentar

- Extrair o pacote completo da build e testar o executável no computador que será usado.
- Conferir som, volume e resolução antes da chegada do público.
- Selecionar o trecho da demo e preservar o save original.
- Abrir a daily atualizada como apoio aos resultados recentes.
- Deixar capturas acessíveis para continuar a explicação se a demonstração falhar.
- Ensaiar com cronômetro. Se houver apenas 10 minutos, reduzir a evolução histórica e demonstrar somente uma interação.

## Perguntas prováveis

**É uma reprodução histórica do caso de Varginha?** Não. O caso e a cultura regional inspiram uma narrativa fictícia de terror sobrenatural.

**Por que 14 capítulos se alguns arquivos falam em 21 fases?** A campanha foi reorganizada. Os IDs antigos continuam no código e nos saves para manter compatibilidade.

**O jogo tem vários finais?** A versão atual implementa uma conclusão. Os finais alternativos dos documentos iniciais permanecem como proposta de expansão.

**Toda a arte e toda a voz foram produzidas pela equipe?** O projeto reúne contribuições da equipe e recursos produzidos com apoio de ferramentas. Os registros de arte e áudio documentam integrações e fontes. Não apresentar a dublagem completa como entregue sem confirmar os takes.

**Os testes garantem que não existem bugs?** Eles reduzem regressões nos recortes exercitados. A validação manual e o teste com outros jogadores continuam necessários.

## Referências de preparação

Histórico Git desde 11/09/2026; README do projeto; Docs/CampanhaOficial/Referencias/GDD_Usuario.md; Docs/CampanhaOficial/CAMPANHA_14_FASES.md; Docs/CampanhaOficial/RENDERIZACAO_SPRITES_2026-10-26.md; Docs/CampanhaOficial/ATUALIZACAO_2026-10-26.md; registros da revisão e manifesto da entrega de 27/10/2026. A data da apresentação é futura em relação à preparação deste roteiro.
