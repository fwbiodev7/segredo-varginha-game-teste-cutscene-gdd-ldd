# 1 INTRODUÇÃO

O Segredo de Varginha é um jogo digital de terror, investigação e aventura narrativa em duas dimensões, desenvolvido em Unity e C#, com perspectiva superior e identidade em pixel art. O projeto aproxima conteúdos do curso de Desenvolvimento de Sistemas de uma produção cultural situada em Varginha, Minas Gerais. A escola, o cotidiano de um professor, o Fusca e os espaços de investigação articulam a ambientação local a uma história ficcional sobre memória, tecnologia e acontecimentos sobrenaturais.

Este relatório apresenta o desenvolvimento desde a concepção registrada nos documentos de design e os primeiros passos verificáveis no repositório até a situação local de 23 de outubro de 2026. O primeiro commit disponível é de 11 de setembro e já contém um protótipo. Portanto, essa data marca o início do histórico acessível, e não comprova o dia em que a ideia surgiu. A análise considera 49 commits alcançáveis, documentos de planejamento, código, registros de testes, capturas e relatórios de compilação (Projeto O Segredo de Varginha, 2026c, 2026e).

O resultado atual é uma campanha organizada em 14 capítulos e cinco atos, com investigação, enigmas, combate, aliados, persistência de progresso e uma conclusão implementada. A build local mais recente identificada é a 0.4.8-beta, gerada em 23 de outubro. O último commit é o a2946be, de 22 de outubro; parte do estado atual permanece como alteração local. Assim, o relatório distingue intenção de design, implementação, verificação técnica e publicação.

## 1.1 Problema e justificativa

O problema de desenvolvimento consistiu em transformar uma proposta narrativa ampla em uma experiência jogável que mantivesse coerência entre história, objetivos, pistas, mapas e sistemas. A produção exigiu integrar programação, desenho de níveis, arte, animação, som, interface e testes dentro de um contexto escolar. A existência de versões diferentes do documento de design de jogo e do documento de design de níveis tornou necessária uma revisão contínua do escopo.

A relevância educacional está na aplicação prática de lógica, orientação a objetos, tratamento de estados, organização de arquivos, controle de versão e validação de software. A ambientação regional também permite trabalhar identidade cultural e comunicação. Trata-se de uma interpretação artística: os documentos, acontecimentos e explicações sobrenaturais apresentados no jogo não constituem investigação histórica ou demonstração de fatos reais.

## 1.2 Objetivos

O objetivo geral é documentar e analisar a evolução do projeto, demonstrando o que foi construído, como as decisões alteraram o produto e quais limitações permanecem. Os objetivos específicos são reconstruir a cronologia; confrontar planejamento e campanha vigente; descrever arquitetura e mecânicas; registrar a produção visual e sonora; examinar interface, acessibilidade e controles; interpretar as evidências de testes; e apresentar prioridades para a continuidade.

O relatório destina-se à apresentação escolar da turma do 3º ano de Desenvolvimento de Sistemas da Escola Estadual Deputado Domingos de Figueiredo, com os professores Renan e Alex. A autoria foi identificada coletivamente conforme os dados fornecidos. Não foram atribuídas tarefas individuais a estudantes sem registro correspondente.

# 2 REFERENCIAL TEÓRICO

## 2.1 Design de jogos e experiência do jogador

Hunicke, LeBlanc e Zubek (2004) propõem a análise de jogos pelas relações entre mecânicas, dinâmicas e experiência estética, conhecida pela sigla MDA. As mecânicas correspondem às regras e aos componentes; as dinâmicas surgem de sua operação durante a partida; a dimensão estética se refere às respostas desejadas no jogador. Neste relatório, esse referencial é usado retrospectivamente para interpretar o produto, sem afirmar que orientou formalmente toda a equipe desde o início.

No projeto, coletar pistas, administrar recursos e confirmar hipóteses são mecânicas. A alternância entre exploração, suspeita e confronto constitui uma dinâmica observável no percurso planejado. Tensão e curiosidade são objetivos de experiência. A retirada de inspeções repetitivas pode favorecer o ritmo, mas seu efeito real sobre diversão e compreensão ainda precisa ser observado com jogadores externos. Uma intenção narrativa não equivale a um resultado de experiência medido.

## 2.2 Versionamento e validação de software

O controle de versão permite recuperar estados de arquivos e comparar alterações ao longo do tempo (Chacon; Straub, 2014). No projeto, o Git oferece marcos verificáveis de integração, correções e mudanças de escopo. Mensagens de commit são indícios do trabalho registrado; a análise também considera o conteúdo e os documentos associados. A quantidade de commits não representa horas trabalhadas nem produtividade individual.

A documentação do Unity distingue testes executados no ambiente de edição daqueles que dependem do comportamento em execução (Unity Technologies, [s. d.]). O projeto também utiliza casos em um Player Windows de testes. Essa separação importa porque uma regra pode funcionar isoladamente e ainda falhar na integração de cena, entrada, apresentação ou persistência. Compilar com sucesso, por sua vez, comprova a geração do programa, mas não demonstra a conclusão manual de toda a campanha.

# 3 PROCEDIMENTOS METODOLÓGICOS

## 3.1 Pesquisa documental e critérios de análise

Foi realizada uma análise documental e técnica do acervo local. O procedimento reuniu leitura do modelo de formatação, levantamento do histórico Git, confronto dos documentos de design com a sequência atual, inspeção de configurações e consulta aos relatórios e resultados de teste existentes. Não foi aplicada pesquisa com participantes nem foram produzidos novos resultados experimentais de desempenho durante a elaboração deste relatório.

Para decidir o estado atual, foram priorizados o código e o manifesto da build de 23 de outubro, seguidos por relatórios recentes e suas evidências. Os documentos originais preservam a intenção e as versões anteriores. Textos como README e ARCHITECTURE possuem trechos históricos: a descrição de apenas três fases, por exemplo, não substitui a sequência atual. Divergências foram tratadas como evolução ou pendência documental, conforme o caso.

As contagens de testes foram mantidas com seus escopos. Um resultado consolidado pela última execução de cada caso não foi apresentado como uma única rodada integral. Repetições não foram somadas como casos novos. Capturas demonstram aparência em situações específicas; não comprovam estabilidade universal, desempenho em qualquer computador ou aprovação de usuários.

O recorte encerra-se em 23 de outubro de 2026. As informações anteriores ao primeiro commit são descritas apenas pelas intenções preservadas nos documentos. Custos efetivos, horas individuais, requisitos mínimos medidos e resultados de testes externos não estavam demonstrados e não receberam valores estimados. A apresentação segue a estrutura institucional do arquivo Modelo ABTN.pdf, com fonte Times New Roman, corpo 12, entrelinhas de 1,5 e títulos principais centralizados em tamanho 16. As referências identificam as fontes consultadas (Universidade Federal do Espírito Santo, 2026).

## 3.2 Concepção narrativa e evolução do escopo

### 3.2.1 Proposta original e identidade regional

O documento original apresenta Edelzio como professor de Desenvolvimento de Sistemas que retorna a memórias da infância após encontrar materiais associados a 1996. A investigação envolve sua casa, a escola, registros religiosos, a mata, um casarão e acontecimentos subterrâneos. O notebook aproxima a profissão do protagonista da resolução de problemas. O Fusca funciona como elemento de identidade e ligação entre os espaços (Projeto O Segredo de Varginha, 2026d).

O Game Design Document (GDD), ou documento de design de jogo, descreve a proposta geral, os personagens, os sistemas e a progressão narrativa. O Level Design Document (LDD), ou documento de design de níveis, detalha espaços, objetivos e sequência de desafios. O material original cita obras como Stranger Things, Silent Hill, Outlast e Resident Evil como referências de atmosfera, investigação e exploração. Essas menções registram influências declaradas e não autorizam a reprodução de seus recursos artísticos.

A trama usa ficcionalmente os acontecimentos de 1996 como ponto de partida. Edelzio, Renan, Ouzana e Fábio têm funções distintas na investigação: memória e experiência do protagonista, análise tecnológica, observação biológica e acesso a registros. As revelações devem surgir do encontro de evidências. A identidade escolar e os personagens não dispensam cuidados com autorização de uso de imagem e com a separação entre pessoas reais e acontecimentos inventados.

### 3.2.2 Mudanças entre o planejamento e a campanha

O acervo contém escopos de tamanhos diferentes. O LDD original apresenta 25 fases, enquanto o GDD organiza a história em seis atos. A cópia experimental iniciou um plano de 20 fases, passou por uma implementação com 21 e recebeu condensações intermediárias, incluindo um documento de 15 fases. A sequência vigente possui 14 capítulos em cinco atos. Esses números se referem a versões e conceitos diferentes, e não devem ser usados como se fossem simultaneamente a estrutura atual.

A redução reuniu tarefas no mesmo espaço e retirou bloqueios redundantes. Fotografia, vulto, horário e confirmação de Renan passaram a compor um mesmo percurso escolar. Laboratório e oficina integram um capítulo; jardim, térreo e porão formam o percurso do casarão. As antigas Câmaras do Selo foram retiradas, e a investigação da criatura incorporou evidências antes distribuídas em outras etapas (Projeto O Segredo de Varginha, 2026b).

Essa condensação preservou o núcleo de investigação e o retorno final, mas alterou a forma de percorrê-los. O critério de coerência adotado é verificar se cada transição conserva motivo, informação e condição de avanço. Não se exige correspondência literal com todos os mapas e bloqueios da primeira proposta, pois houve revisões explícitas de escopo.

### 3.2.3 Coerência atual e diferenças documentais

Os principais eixos permanecem alinhados à proposta: protagonista professor, ligação com 1996, investigação por documentos, auxílio de personagens, casarão, selo e confronto com uma manifestação. A criatura ferida e a manifestação combatida são entidades narrativamente distintas. O encerramento implementado exige vitória, calibração, passagem da criatura e fechamento da ligação antes do retorno à escola. Essa ordem evita que derrotar o inimigo seja confundido com concluir toda a história.

Há diferenças que precisam continuar registradas. O planejamento previa três finais, mas existe uma conclusão disponível no percurso atual. O casarão utiliza 1898; fontes antigas apresentam conflito com 1966. O horário adotado no puzzle é 23:23, embora uma versão consolidada antiga mencione 23:17. O relatório registra a escolha vigente sem alterar os documentos históricos para simular uniformidade retroativa.

A abertura e as falas foram revistas sem eliminar as informações dos documentos e enigmas. A versão de 23 de outubro simplifica as reações adultas de Edelzio, retirando a repetição oral de datas e explicações de inventário. Isso não remove o horário do puzzle nem muda as condições de resolução. A coerência técnica é sustentada pelas flags e pelos testes relacionados; a compreensão por quem joga pela primeira vez permanece uma questão para avaliação externa (Projeto O Segredo de Varginha, 2026h, 2026k).

## 3.3 Trajetória do desenvolvimento

### 3.3.1 Primeiros registros e base jogável em setembro

O commit 8dd459a, de 11 de setembro, registra um protótipo com jogabilidade e identidade em pixel art. O registro seguinte, em 12 de setembro, acrescenta testes de fases e de aliados. Em 14 de setembro aparecem ajustes de entrada, apresentação dos pixels, cenário e validação. Essa sequência demonstra que a preocupação com controle e teste acompanha os primeiros estados disponíveis, embora não permita reconstruir integralmente a concepção anterior ao repositório.

Em 15 de setembro foi registrado um guia de prólogo com Timeline e orientações de qualidade. Esse documento é uma proposta técnica de sua época. A presença do pacote Timeline e do guia não comprova que todas as cenas cinematográficas atuais sejam implementadas por ele. Em 17 de setembro, o histórico registra trabalho em derrota, aliados e testes (Projeto O Segredo de Varginha, 2026a, 2026e).

Entre 22 e 25 de setembro, o foco incluiu sprites, referências visuais, correções e combate. Os golpes receberam padronização e efeitos; o registro de Punch V2 documenta antecipação, impacto e recuperação. A lanterna passou a ter representação física e seleção na barra de itens. A integração de personagens e aliados ampliou a identidade do elenco e exigiu compatibilizar animação, equipamento e ação.

Nos dias 28 e 29 ocorreram correções, refatoração e integração de arte para resgate, iluminação e diocese, além de um atlas de interações de Edelzio. Em 30 de setembro, a sala da segunda fase e a animação sentada receberam trabalho específico. Nesse período, consolidou-se uma base que combina cenas relativamente leves com construção de objetos e ambientação por controladores em execução.

### 3.3.2 Expansão experimental entre 1 e 3 de outubro

Em 1º de outubro foi registrada a cópia experimental com plano de 20 fases baseado no GDD e no LDD. A abertura incluiu Edelzio criança e houve integração dos modelos originais de casa, escola e Fusca. A expansão precisava transformar uma sequência inicial curta em campanha, preservando a identidade dos recursos já aprovados.

Em 2 de outubro, o histórico registra mapas e conteúdo das etapas 6 a 10, além de portais da casa e mochila. Em 3 de outubro foram trabalhados interiores mais compactos, efeitos, ferramentas de mapa, animações e investigação escolar. O Livro do Tombo, a escala da igreja e a participação de Ouzana também aparecem nesse conjunto de registros.

O efeito organizacional foi ampliar o volume de estados e de dependências: cada pista passou a precisar de origem, registro, condição de uso e retomada após salvar. A presença de mais ambientes também elevou o risco de trajetos longos e repetição de inspeções. A revisão posterior do percurso responde a esse crescimento, reunindo ações que podiam ocorrer no mesmo local.

### 3.3.3 Conclusão do percurso e condensação entre 4 e 7 de outubro

Em 4 de outubro foram registradas as etapas finais da campanha experimental e o combate conclusivo. No dia seguinte, o chefe recebeu apresentação cinematográfica e ajustes de fluidez. O histórico também inclui integrações por merge, que representam incorporação de trabalho e não novas funcionalidades isoladas em todos os casos.

Em 7 de outubro, as revisões de campanha, puzzles e usabilidade condensaram o percurso até a sequência vigente de 14 capítulos. Foram conservados o mapa, a perseguição na mata, o checkpoint, o encontro com Fábio e os componentes essenciais do retorno. Etapas retiradas deixaram de bloquear o avanço, enquanto seus identificadores e dados puderam ser mantidos por compatibilidade.

Essa alteração exigiu migração de save. A sequência exibida ao jogador foi separada dos IDs serializados, evitando renumerar indiscriminadamente arquivos e dados já existentes. A vitória contra a manifestação passou a ser persistida, para impedir que o chefe reaparecesse apenas porque o jogador recarregou. A calibração e a travessia mantiveram seus próprios estados.

Os relatórios dessa etapa registram correções de retorno, dificuldade, caches e posicionamento, além de iluminação dinâmica de personagens, suporte a controle e material de divulgação. Um registro final de 7 de outubro informa 102 casos EditMode e 47 PlayMode aprovados em seus recortes. Esses valores descrevem aquela etapa; não devem ser adicionados aos resultados posteriores como se todos fossem casos inéditos (Projeto O Segredo de Varginha, 2026g).

### 3.3.4 Remasterização e transição de alpha para beta

Entre 8 e 10 de outubro, o projeto recebeu revisão cinematográfica, capturas, apresentação visual e preparação de versões executáveis. A Figura 1 exemplifica a escola no acervo da remasterização. Ela é uma evidência histórica da composição, não uma captura produzida especificamente da build 0.4.8.

@fig|Escola Industrial no acervo da remasterização|Docs/Remaster20261008/Prints/02_Escola_Industrial.png|Projeto O Segredo de Varginha (2026g), acervo Remaster20261008.

Os registros de alpha documentam correções de mapas, interface e recortes de sprites. A alpha 0.3 reúne 55 folhas e 928 quadros de animação no levantamento correspondente. A passagem para beta 0.4 incluiu laboratório, menu, portal e selo de retorno. Esses números caracterizam uma entrega histórica de arte; não equivalem a uma medição de qualidade de cada quadro.

As validações visuais tiveram limites expressos. Há execuções curtas de menu e capturas direcionadas; elas não representam uma sessão completa de campanha. Também existem falhas preservadas e casos sem seleção efetiva. Um processo sem testes executados não foi considerado aprovação. Esse cuidado impede transformar disponibilidade de ferramentas em evidência de comportamento validado.

### 3.3.5 Investigação da chave e revisão da abertura

Em 20 de outubro, a abertura adulta foi reorganizada para ensinar controles durante atividades do cotidiano: pia, café, preparação do notebook e procura da mochila. A chave é colocada em um móvel, desaparece e passa a motivar uma investigação ambiental antes da abertura da caixa. O objetivo foi fazer o tutorial surgir das ações do protagonista, mantendo a ligação com as páginas e os acontecimentos de 1996.

A disposição física também evoluiu. Referências antigas mencionam o balcão; a apresentação posterior utiliza a mesa do café. Essa mudança visual precisou ser acompanhada por testes e por instruções consistentes. Saves que já obtiveram as evidências não devem obrigar o jogador a repetir toda a preparação (Projeto O Segredo de Varginha, 2026b, 2026i, 2026j).

Em 21 de outubro foram registradas as versões 0.4.1 a 0.4.4-beta. A 0.4.1 consolida a abertura adulta; a 0.4.2 amplia a apresentação do Fusca e o puzzle de bifurcações; a 0.4.3 revisa o prólogo de 1996 e a manhã; a 0.4.4 integra efeitos e ambientação recebidos, além de apoios. A numeração identifica entregas sucessivas do mesmo produto, sem pressupor que cada versão tenha repetido toda a validação anterior.

### 3.3.6 Polimento de 22 de outubro e estado de 23 de outubro

Em 22 de outubro, o trabalho concentrou-se em rotina da casa, apresentação cinematográfica, janelas, mesa escolar e áudio. Os commits eb2f8ea, 4362b53 e a2946be correspondem às entregas 0.4.5, 0.4.6 e 0.4.7-beta. Os registros mostram que problemas identificados em uso motivaram ajustes de profundidade, posição de interação, visualização do notebook e preparação de sons.

Em 23 de outubro, as 14 falas adultas do primeiro lote de Edelzio foram substituídas pela redação descontraída aprovada. A frase infantil permaneceu. O material de gravação recebeu o nome roteiro_edelzio_definitivo.pdf, com três páginas, além de texto e mapas técnicos de IDs. A Figura 2 mostra a apresentação de diálogo na escola no conjunto de capturas desse dia; a fala visível é de Renan e não deve ser atribuída a Edelzio.

@fig|Diálogo escolar registrado na revisão de 23 de outubro|output/dialogue-casual-2026-10-23/captures/renan-subtitle-revealed.png|Projeto O Segredo de Varginha (2026k), captura da revisão de falas.

O resultado técnico dessa rodada inclui 29 testes EditMode direcionados, três casos no Player Windows e build 0.4.8-beta com sucesso. A primeira rodada do Player apresentou uma falha de referência antiga do teste, posteriormente corrigida e repetida. A legenda ED03 foi atualizada, mas ainda não dispara automaticamente uma gravação. Não existe evidência de recebimento e integração da dublagem humana desse lote até o recorte do relatório.

## 3.4 Organização da campanha e dos níveis

### 3.4.1 Percurso vigente e função dos ambientes

A campanha atual organiza a progressão em cinco atos: A lembrança volta, O caminho da capela, Evidências que mudam, A noite esquecida e O verdadeiro segredo. O Quadro 1 apresenta os 14 capítulos e seus objetivos, sintetizados do documento vigente e conferidos em CampaignSequence. Os IDs preservados servem à implementação e não indicam capítulos ausentes na experiência do jogador.

@campaign

A progressão parte de memórias e objetos cotidianos, passa por comparação de registros e alcança a reconstrução do acordo. A escola funciona como espaço de pesquisa; a biblioteca reúne fragmentos; a mata introduz orientação e perseguição; a capela e o casarão relacionam documentos antigos ao presente. O laboratório e o Fusca permitem comparar evidências materiais. Essas funções conectam cenário e objetivo, reduzindo a dependência de instruções sem contexto.

### 3.4.2 Capítulos cenas e compatibilidade

Há 19 cenas incluídas no manifesto da build 0.4.8-beta. Esse total contempla menu e áreas complementares, por isso não deve ser comparado diretamente com os 14 capítulos. A contagem estática encontrou 30 arquivos de cena em Assets/Scenes; ela inclui recursos que não entram na distribuição atual. Cenas antigas e arquivos de recuperação não demonstram novas fases disponíveis ao público.

O save da campanha utiliza versão 3. Na migração, IDs antigos 3 e 5 passam para 4; 16, 17 e 19 passam para 18. Posições de cenas substituídas são descartadas quando necessário, enquanto evidências e estados úteis permanecem. O sistema reconhece conquistas anteriores, evitando apagar progresso por causa de uma revisão do percurso. A gravação atômica registrada no projeto busca impedir arquivos parciais durante a substituição do save (Projeto O Segredo de Varginha, 2026b, 2026c).

### 3.4.3 Ritmo orientação e resolução de enigmas

O desenho dos níveis alterna exploração, leitura, interpretação e execução. O caderno reúne evidências; as dicas oferecem orientação gradual; retornos e checkpoints reduzem repetição desnecessária. O desafio deve depender de relacionar as informações disponíveis, e não de descobrir um ponto de interação invisível ou repetir uma tarefa já concluída.

As revisões retiraram barreiras intermediárias do laboratório e uniram partes do casarão. O reagente revela as marcas em uma aplicação; a passagem e a chave permitem o acesso ao porão. A retirada de tarefas não implica apagar seus registros antigos do save. O percurso final conserva calibração, travessia e encerramento como ações distintas, porque cada uma possui significado narrativo e estado próprio.

## 3.5 Arquitetura e tecnologias

### 3.5.1 Ambiente de desenvolvimento

O arquivo ProjectVersion.txt identifica Unity 6000.6.0f1 como versão do projeto. A programação principal é realizada em C#, com componentes vinculados aos objetos de cena e controladores específicos da campanha. A plataforma comprovada pelas builds analisadas é Windows de 64 bits. Não foi demonstrada distribuição equivalente para Linux, macOS, navegador ou dispositivos móveis.

O manifesto de pacotes contém Input System 1.20.0, Universal Render Pipeline 17.6.0, Test Framework 1.8.0, Timeline 6.6.0 e uGUI 2.6.0. Também inclui 2D Animation 16.0.0 e 2D Aseprite 6.0.0. A presença de um pacote indica dependência configurada; seu uso integral em todas as funcionalidades não pode ser inferido apenas da instalação.

A Tabela 1 apresenta um levantamento estático do acervo. Os valores não medem linhas de código, complexidade ou cobertura. Arquivos históricos e auxiliares podem compor essas contagens; por isso, elas servem somente para dimensionar a organização disponível em 23 de outubro.

@inventory

### 3.5.2 Separação de responsabilidades

Os componentes genéricos de gerenciamento, jogador, níveis e interface convivem com a implementação específica em Assets/Scripts/Game/Varginha. A expansão narrativa se concentra no diretório Experiment. CampaignSequence define a relação entre capítulos e IDs; CampaignHouseSequence conduz a rotina e as pistas da casa; VarginhaCampaignStage coordena estados de etapas; CampaignContinuationController organiza a continuidade posterior.

CampaignPuzzleDesign concentra regras e apresentação de enigmas; CampaignCinematics e controladores associados trabalham sequências de apresentação; CampaignSoundscape e CampaignRecordedAudio tratam eventos sonoros. VarginhaPlayerAttack executa ações de combate; componentes de alvo e inimigo relacionam acerto, vida e comportamento. Essa distribuição permite localizar problemas por sistema, mas não elimina o acoplamento entre estado narrativo, interface e cenário (Projeto O Segredo de Varginha, 2026a, 2026c).

Parte dos ambientes é montada por fábricas e controladores em execução, permitindo cenas leves e composições reutilizáveis. Essa abordagem exige controlar inicialização, restauração de estado e duplicações. Os mecanismos idempotentes documentados reconhecem elementos existentes ao reaplicar a montagem.

### 3.5.3 Estado persistência e retomada

O fluxo narrativo depende de flags de aquisição, inspeção, resolução e conclusão. Ter visto uma pista não significa necessariamente ter resolvido o enigma associado. Essa separação permite bloquear uma porta pela condição correta e reconstruir a interface após carregar. Inventário, evidências, puzzles, equipe, vitória e etapas de retorno precisam permanecer consistentes entre si.

Os testes verificam a retomada da casa sem repetir a preparação concluída e do final sem recriar a manifestação derrotada. As migrações acompanham as mudanças de percurso, preservando dados anteriores. Esses mecanismos reduzem riscos de incompatibilidade.

### 3.5.4 Renderização ferramentas e manutenção

A apresentação preserva a nitidez da pixel art e reutiliza a biblioteca original, com filtro Point nos recursos documentados. A configuração atual inclui URP; relatórios anteriores descrevem decisões históricas de renderização.

As ferramentas em Tools automatizam preparação de materiais, testes, builds e relatórios. AgentOutput preserva os logs completos para rastrear falhas; seus resumos não substituem as evidências originais.

## 3.6 Mecânicas e sistemas de jogo

### 3.6.1 Exploração inventário e investigação

O ciclo de exploração combina deslocamento em perspectiva superior, aproximação de objetos, interação e registro de informações. Mochila e notebook possuem representação visual e funções ligadas à preparação e à investigação. O inventário e a barra de itens permitem acessar equipamentos e recursos; a seleção da lanterna foi trabalhada ainda em setembro. O estado deve ser compartilhado entre personagem e interface para evitar itens exibidos sem função ou equipamentos adquiridos que desapareçam visualmente.

Na abertura adulta, atividades simples introduzem comandos antes de elevar a tensão. A investigação da chave usa alterações no ambiente, observação e comparação. Detalhes como o papel, o traço azul, a porta e os arranhões sustentam a descoberta. As falas mais curtas do lote atual deixam parte da informação nos documentos e no caderno, reduzindo explicações repetidas em voz alta.

### 3.6.2 Puzzles e progressão de informações

Os enigmas abrangem reunião de fragmentos, comparação de registros, identificação de marcas e calibração. O puzzle de bifurcações revisado em 21 de outubro oferece três decisões binárias, totalizando oito combinações possíveis, das quais uma atende à solução. A implementação documenta prévia, traçado do percurso, desfazer, reinício e dicas progressivas. Os estados de tentativa e conclusão integram o sistema de persistência (Projeto O Segredo de Varginha, 2026i).

A solução não deve depender de uma fala ausente. Por isso, o horário permanece nos materiais de investigação mesmo após a simplificação das reações de Edelzio. Da mesma forma, retirar uma frase sobre o caderno não significa desativar o registro técnico. Essa distinção entre texto apresentado e condição de jogo foi central na revisão de 23 de outubro.

### 3.6.3 Combate saúde sanidade e aliados

O combate utiliza golpes encadeados, preparação, impacto e recuperação. A base de três golpes de Edelzio foi desenvolvida com hitbox por ataque, efeitos de impacto e janela de continuidade. Os inimigos apresentam avisos e intervalos de recuperação para permitir reação. Vida, dano e sanidade são sistemas relacionados no projeto; os relatórios de implementação também registram esquiva, recuo e interrupção de ataques.

Os alunos e apoios acrescentam possibilidades de ação, com recargas para limitar o uso contínuo. A campanha vigente mantém composição de três alunos e um apoio entre Renan, Ouzana e Fábio nos trechos correspondentes. Os documentos antigos descrevem elencos e regras de protótipo mais amplos; essas descrições não devem substituir automaticamente a composição atual.

A auditoria de 21 de outubro distingue três perfis: Renan atua com pulso e interrupção; Ouzana usa área com dano ao longo do tempo; Fábio fornece proteção e recuperação. A integração visual usa sequências curtas de efeitos e limites de alvos ou geometria. Esses limites demonstram decisões de implementação, mas não constituem benchmark de desempenho em todas as máquinas (Projeto O Segredo de Varginha, 2026f).

### 3.6.4 Confronto final e encerramento

A batalha da manifestação exige preparação e gerenciamento de ataques. A entrada, a dissipação e o retorno possuem apresentação cinematográfica, com cuidados de pausa, câmera e movimento reduzido. O limite documentado de cinco filhotes evita crescimento irrestrito desses inimigos durante o confronto.

Depois da vitória, a calibração do mecanismo permanece necessária. Abrir a passagem não conclui imediatamente a campanha: a criatura precisa atravessar, Edelzio sustenta o selo vivo durante a etapa e o encerramento da ligação antecede a liberação do retorno. Renan, o caderno, o Fusca e os créditos compõem a conclusão disponível. Há uma cadeia de estados finais, e não somente uma tela de vitória após o combate.

## 3.7 Produção visual cinematográfica e sonora

### 3.7.1 Pixel art animação e composição dos ambientes

A identidade visual reúne personagens em pixel art, objetos reconhecíveis do cotidiano e ambientes associados a Varginha. Os registros de setembro documentam revisões de personagens, golpes, equipamento e cenário. A composição precisa conciliar leitura em escala pequena, distinção entre planos e preservação da aparência aprovada. Os recursos originais de Edelzio, escola, mochila e Fusca foram reaproveitados em diferentes momentos.

As animações de caminhada, interação, sentar, dirigir e atacar precisam manter identidade, escala e ponto de apoio. Equipamentos separados do corpo permitem conservar mochila e notebook durante mudanças de ação. Problemas de oclusão e profundidade também foram tratados em móveis, janelas e mesas, porque uma pista encoberta ou um personagem aparentemente atravessando um objeto prejudica a leitura da cena.

A Figura 3 apresenta a investigação junto à mesa do café no conjunto de capturas de 22 de outubro. O registro exemplifica a importância de alinhar suporte visual, posição da chave e ponto de aproximação. O teste posterior que ainda comparava o objeto ao armário precisou acompanhar essa disposição.

@fig|Mesa do café e apresentação da investigação da chave|output/house-interactions-2026-10-22/captures/key-coffee-table-larger.png|Projeto O Segredo de Varginha (2026j), captura da revisão de interações da casa.

### 3.7.2 Recursos assistidos por inteligência artificial

O documento ASSETS_IA registra o uso de geração assistida para recursos complementares, como storyboard, planos do Fusca e personagens da experiência. Os prompts e as especificações preservam referências de cor, proporção, perspectiva e identidade. A fachada original da Industrial foi reaproveitada por solicitação do responsável, em vez de ser substituída pela alternativa experimental (Projeto O Segredo de Varginha, 2026f).

O acervo registra que as reportagens e imagens de 1996 são ficcionais. A voz sintética foi retirada e não houve clonagem de voz pessoal. O uso de ferramentas de geração não elimina a necessidade de curadoria: imagens precisam de recorte, importação, validação de quadros, escala e adequação ao contexto. Tampouco permite atribuir autoria humana individual a partir do nome de um arquivo.

### 3.7.3 Cinemáticas e passagem entre momentos

As cenas cinematográficas apresentam a infância, a rotina adulta, o deslocamento do Fusca e acontecimentos do final. Sua função é orientar o olhar e ligar espaços sem transformar toda passagem em tarefa repetitiva. O trabalho de outubro revisou jornal, televisão, manhã, cabine e pane do carro, mantendo a participação do jogador nos trechos interativos.

A documentação também exige preservar pausa, restauração de câmera e opção de movimento reduzido. Uma sequência que bloqueia entrada precisa liberar o controle ao terminar ou ao retomar corretamente. A legenda deve permanecer sincronizada ao evento e não antecipar informação incompatível com o estágio da investigação. O guia de Timeline de setembro é referência histórica; a implementação atual distribui a apresentação por componentes próprios e recursos de cena.

### 3.7.4 Efeitos ambientes e integração de áudio

O pacote recebido em 21 de outubro contém 21 arquivos, correspondentes a 20 fontes únicas. A preparação resultou em 25 arquivos WAV e quatro aliases de eventos sem duplicação física adicional. Os sons atendem a golpes, passos, ambientes, televisão, rádio, Fusca, igreja e efeitos de apoios. Os identificadores recebidos, cortes e derivados foram registrados no catálogo e nos créditos (Projeto O Segredo de Varginha, 2026f, 2026i).

O preparo documenta áudio PCM de 16 bits e 44,1 kHz, variantes de passos, ajustes de ganho, fades curtos e tratamento de emenda para ambientes. A integração relaciona o som ao evento, evitando usos incoerentes: o som do escudo pertence a Fábio, os sinos ao ambiente da igreja e o pulso a Renan. Em 22 de outubro houve um novo conjunto de 16 efeitos e ambientes, registrado separadamente.

Ter recebido um arquivo não comprova automaticamente sua licença de redistribuição. Os créditos preservam a identificação fornecida e não inventam autores ou permissões. A documentação de origem e os comprovantes de autorização precisam acompanhar a preparação de uma publicação ampla. Essa pendência documental é distinta do funcionamento técnico dos clipes no jogo.

### 3.7.5 Dublagem e roteiro de gravação

O levantamento de falas e eventos utiliza códigos para ligar roteiro, nome de arquivo e evento no runtime. Um inventário amplo registrou 153 entradas de tipos diferentes: 47 na categoria E, 98 na categoria P e oito na categoria V. Esse total não representa 153 falas de Edelzio nem 153 gravações já integradas. O acervo também contempla vozes de outros personagens e revisões de escopo.

O primeiro lote destinado a Edelzio contém 15 falas: 14 adultas e uma infantil, além de cinco vocalizações opcionais. As propostas futuras foram separadas desse lote para evitar gravação desnecessária. Em 23 de outubro, as falas adultas ficaram mais curtas, naturais e com menos datas pronunciadas; a fala infantil foi preservada. O PDF definitivo tem três páginas e mantém correspondência com os IDs esperados (Projeto O Segredo de Varginha, 2026h, 2026k).

O material facilita gravar arquivos individuais com identificação inequívoca e revisar o texto antes da edição. Entretanto, roteiro aprovado e suporte de reprodução não equivalem a dublagem concluída. As gravações humanas ainda dependem de recebimento, conferência, preparação e integração. A legenda ED03 possui uma pendência específica de acionamento automático de voz na cutscene de colocar a chave.

## 3.8 Interface controles e acessibilidade

### 3.8.1 Comunicação de objetivos e estados

A interface apresenta objetivos, interações, caderno, inventário, barra de itens, vida, sanidade e combate. A adaptação a janelas busca impedir recortes e sobreposição. Os registros recentes verificam tamanho de legenda e resoluções distintas, sem certificar todas as telas possíveis.

As revisões da mesa escolar e do notebook ajudam a identificar onde interagir e o resultado da ação. Caderno e dicas reduzem a dependência de memorizar falas. As mensagens devem orientar sem entregar toda a solução.

### 3.8.2 Teclado mouse e controle

O projeto utiliza Input System e remapeamento de comandos. A documentação XInput inclui diagrama de botões, navegação e ajustes de resposta. Há confirmação de controle físico pelo responsável e testes com entrada simulada. A simulação valida fluxos, mas não certifica todos os modelos de hardware.

A revisão recente ajustou a interação por mouse em trechos do Fusca. As instruções antigas precisam ser conferidas contra a versão da build, mantendo persistência de preferências e retorno visual coerentes (Projeto O Segredo de Varginha, 2026c, 2026j).

### 3.8.3 Recursos disponíveis e limites de avaliação

Legenda, escolha de tamanho, pausa, dicas progressivas, movimento reduzido e remapeamento ampliam possibilidades de uso. A separação de volumes e os controles de voz e pausa também foram verificados em casos direcionados. Esses recursos são relevantes para leitura, conforto e adaptação de comandos.

Não foi conduzida avaliação formal com grupos de pessoas com deficiência, leitores de tela ou uma amostra de jogadores iniciantes. Assim, o relatório reconhece funcionalidades de acessibilidade sem declarar conformidade universal. A próxima etapa deve observar legibilidade, descoberta dos comandos, fadiga, tempo de leitura e compreensão das pistas com pessoas que ainda não conhecem a campanha.

## 3.9 Qualidade testes e resultados técnicos

### 3.9.1 Estratégia de validação

A validação combina regras isoladas, comportamento em execução, casos no Player, captura de telas e geração de builds. Foram consultados relatórios, JSONs de resumo e resultados XML preservados. Cada artefato deve ser associado à sua data, versão e recorte, pois arquivos genéricos podem ser sobrescritos por rodadas menores.

As evidências de 21 de outubro são particularmente claras sobre o método de consolidação. Os 270 casos PlayMode da entrega 0.4.1 representam o último resultado de cada caso a partir da suíte e de repetições de grupos afetados. O próprio resumo informa que não houve uma única execução integral limpa correspondente a esse total. Já os quatro casos no Player foram distribuídos em duas execuções nativas. Essa distinção foi mantida neste relatório (Projeto O Segredo de Varginha, 2026i).

### 3.9.2 Evolução das verificações

A Tabela 2 sintetiza resultados selecionados das entregas recentes. Os valores não são cumulativos. A cobertura de 270 casos PlayMode não foi novamente atribuída à build 0.4.8, que recebeu verificação direcionada de seu escopo. Da mesma forma, 10 casos e quatro repetições de áudio não constituem 14 casos inéditos.

@tests

Os registros de 22 de outubro incluem uma rodada de 12 testes EditMode de áudio e três PlayMode de combate em arquivos genéricos. A existência desses XMLs menores não prova uma nova suíte integral de 399 casos. Um registro de execução iniciada sem resultado final também não é evidência suficiente de aprovação.

Na build 0.4.8, os 29 casos EditMode cobrem investigação, continuidade de save, apresentação de mensagens e IDs de voz. Os três casos no Player tratam da casa e da escola, incluindo retomada, canal de voz, pausa, teclado, controle e dimensões de legenda. Saves e preferências foram isolados em um produto de testes para preservar o estado do jogo do usuário.

### 3.9.3 Falhas observadas e correções verificadas

A primeira execução do Player em 23 de outubro aprovou dois dos três casos. O caso restante ainda comparava a chave com Kitchen_Cabinet, embora o jogo já a apoiasse em CoffeeTable_Kitchen. O teste foi corrigido para o móvel e o ponto de aproximação atuais; a repetição aprovou os três casos. A rodada inicial permaneceu preservada, permitindo distinguir falha de expectativa antiga de um defeito confirmado na disposição vigente.

Esse exemplo mostra a necessidade de manter os testes alinhados à regra de produto. Atualizar uma referência só é válido quando a condição continua exigindo o comportamento desejado. Nesse caso, o teste permanece verificando que a chave esteja à frente do móvel que a sustenta. A correção não foi apresentada como aumento artificial do total de casos.

Os relatórios anteriores também registram correções de fluxo de retorno, migração de save, profundidade visual e interação. A manutenção das falhas no acervo permite analisar o processo de correção. Relatar apenas o último número aprovado ocultaria problemas que explicam decisões importantes do desenvolvimento.

### 3.9.4 Build mais recente e interpretação dos indicadores

O manifesto de 23 de outubro registra build Succeeded da versão 0.4.8-beta, em Unity 6000.6.0f1, com zero erros e um aviso. O processo durou aproximadamente 109,20 segundos e informou 398.224.136 bytes para a saída da build. Esse total não é o tamanho de um ZIP publicado nem uma medição do executável isolado. Foram incluídas 19 cenas (Projeto O Segredo de Varginha, 2026k).

O aviso relacionado a configuração opcional de automação de pipeline é preexistente nos registros de entrega. Ele não deve ser interpretado como prova de que a renderização URP não funciona. Também não foi tratado como erro de compilação, pois o resultado final é explicitamente de sucesso.

Não há benchmark recente completo de taxa de quadros, consumo de memória ou desempenho em outro computador que fundamente requisitos mínimos. Execuções curtas de menu e inspeções de capturas têm utilidade, mas não comprovam ausência de travamentos em uma partida inteira. A rodada humana completa, o teste com iniciantes e a comparação em outro hardware permanecem necessários.

## 3.10 Entregas documentação e gestão

### 3.10.1 Organização dos artefatos e distribuição

O acervo reúne projeto Unity, executáveis Windows, pacotes de distribuição, relatórios, resultados de testes, capturas, apresentações e materiais de gravação. A build 0.4.8 possui pasta própria, preservando a 0.4.7. Uma instância já aberta de uma versão anterior não recebe automaticamente alterações de código da nova compilação.

Os relatórios de 21 de outubro registram validação de pacote e publicação de uma entrega 0.4.4. Isso não demonstra que a 0.4.8 esteja publicada. O estado mais recente aqui comprovado é local. Para divulgar uma nova versão, é preciso confirmar o arquivo distribuído, o manifesto correspondente, o link efetivo e a versão apresentada ao público.

O site do projeto é estático, com HTML, CSS e JavaScript, sem dependências de execução. Seu README descreve configuração centralizada, capturas do jogo, documentos ficcionais, galeria, preferência de movimento reduzido e navegação por teclado. O registro de 7 de outubro informa atualização promocional e acesso à alpha. O site tem cronologia própria e deve ser sincronizado com a versão realmente disponibilizada, sem expor guias internos ou soluções dos enigmas (Projeto O Segredo de Varginha, 2026l).

### 3.10.2 Comunicação e documentação de acompanhamento

As apresentações e roteiros de daily organizam mudanças, situação atual e próximos passos. Elas auxiliam a comunicação escolar, mas devem citar a data e a versão a que se referem. O mesmo vale para diários de desenvolvimento: um documento limitado a 9 a 22 de outubro não substitui a história completa de setembro nem incorpora automaticamente a revisão de 23 de outubro.

A documentação existente é extensa, porém distribuída por períodos e entregas. Há relatórios mais atuais do que o README geral e a arquitetura do protótipo. A manutenção deve criar uma entrada de consulta que indique o documento vigente e preserve os antigos como histórico. Essa medida reduz o risco de apresentar três fases, três finais ou uma versão antiga como situação atual.

O roteiro definitivo de Edelzio é um artefato operacional específico: ajuda a gravar o lote aprovado. Os arquivos de mapeamento ligam códigos de ator e nomes esperados pelo jogo. Mantê-los junto da versão textual evita receber gravações de frases já substituídas e reduz retrabalho de edição e integração.

### 3.10.3 Equipe recursos e aprendizagem

A autoria deste relatório é atribuída à turma do 3º ano de Desenvolvimento de Sistemas. Os professores informados são Renan e Alex. O repositório não fornece uma matriz consolidada e confirmada de funções individuais; aliases de commits não foram transformados em nomes de integrantes. Também não foram atribuídas atividades de programação, arte ou roteiro a pessoas sem comprovação.

As ferramentas documentadas incluem Unity, C#, Git, recursos de preparação de imagem e áudio, automações de teste e assistência por inteligência artificial. O GDD contém campos de custo e prazo de caráter preliminar, inclusive exemplos e valores não acompanhados de comprovação. Esses campos não constituem orçamento executado. Não há base para afirmar gasto total, remuneração, economia financeira ou número de horas trabalhadas.

Como resultado formativo, o acervo demonstra aplicação de programação de estados, componentes, entrada, persistência, leitura de logs e controle de versão. A produção também exigiu adaptar narrativas ao tempo de jogo, integrar som a eventos e comunicar problemas em relatórios. Essa análise deriva dos artefatos entregues, sem substituir avaliação individual de aprendizagem feita pelos professores.

### 3.10.4 Riscos e cuidados de continuidade

Os principais riscos combinam divergência documental, compatibilidade de save, regressões de interação, volume de conteúdo e validação insuficiente da experiência. A autoria e as permissões de uso de sons e referências pessoais também precisam permanecer rastreáveis. O Quadro 2 relaciona esses riscos a medidas de acompanhamento; são recomendações, e não tarefas já concluídas.

@risks

Uma política de preservar a pixel art aprovada, os puzzles e os diálogos funcionais ajuda a limitar mudanças desnecessárias. Correções futuras devem começar por inspeção, plano e autorização conforme o fluxo de colaboração do projeto. O presente relatório não modificou o jogo, suas cenas ou seus recursos.

## 3.11 Resultados obtidos e estágio atual

O projeto ultrapassou a condição de protótipo restrito às primeiras fases. O percurso atual possui início, investigação intermediária, confronto e retorno, com persistência e uma conclusão implementada. A produção acumulou arte, animação, som, interface, controles, ferramentas e registros de qualidade. A revisão recente demonstra uma fase de estabilização e polimento, com atenção a interações concretas e clareza de apresentação.

O Quadro 3 separa os resultados existentes das validações ainda pendentes. Essa apresentação evita tratar a expressão beta como garantia de acabamento completo ou, no sentido oposto, ignorar o conteúdo já funcional. O estágio é de beta local em evolução, com evidências técnicas relevantes e lacunas de avaliação humana e documentação de publicação.

@status

# 4 RESULTADOS ESPERADOS

O resultado esperado é consolidar uma versão estável e compreensível da campanha já implementada, preservando sua identidade visual, a investigação e a continuidade narrativa. Os resultados apresentados na seção anterior sustentam essa expectativa, mas também mostram os limites que precisam orientar a próxima entrega.

## 4.1 Continuidade do projeto

A primeira prioridade é uma partida manual do início ao fim na build candidata, com registro de bloqueios, clareza das pistas, retomadas e conclusão. Deve-se repetir os trechos de casa, escola e final usando saves novos e antigos. O teste com pessoas que não conhecem as soluções precisa observar onde há dúvida, quanto auxílio é necessário e quais informações são esquecidas.

A segunda prioridade é concluir o fluxo de dublagem do lote aprovado: receber os arquivos, conferir texto e identificação, preparar volume e ruído, integrar eventos e resolver o acionamento de ED03. Depois, a revisão deve avaliar sincronização, pausa, repetição, volume e continuidade com legendas. A gravação de propostas futuras deve ocorrer somente após definição de escopo, evitando produzir falas descartáveis.

A terceira prioridade é consolidar uma execução de regressão identificada pelo mesmo estado de código da próxima entrega e verificar a candidata em outro computador. Devem ser medidos desempenho, memória e tempo de carregamento com condições registradas. Esses dados permitirão discutir requisitos mínimos com base observável, em vez de estimá-los pelo tamanho dos arquivos.

Por fim, recomenda-se atualizar o documento de entrada do projeto, consolidar créditos e permissões, conferir o pacote público e sincronizar site e apresentação. Finais alternativos e expansões permanecem propostas de produto; não devem competir automaticamente com a correção dos bloqueios e a validação da campanha já existente.

## 4.2 Síntese conclusiva

O desenvolvimento de O Segredo de Varginha demonstra a transformação de uma proposta ampla de terror e investigação em uma campanha com percurso definido e build Windows verificável. Entre o primeiro estado disponível de setembro e a revisão de 23 de outubro, a equipe ampliou conteúdo, integrou sistemas e depois condensou o escopo para melhorar continuidade e reduzir repetição.

A coerência com GDD e LDD deve ser compreendida a partir dessa evolução. A campanha conserva protagonistas, contexto regional, investigação tecnológica, registros antigos, selo e retorno, mas não implementa literalmente todas as fases e finais previstos no material inicial. Registrar essas diferenças torna a avaliação mais precisa e permite manter um histórico honesto das decisões.

O estágio atual é sustentado pela sequência de 14 capítulos, pelos mecanismos de persistência, pelas revisões visuais e sonoras e pela geração bem-sucedida da 0.4.8-beta. Os testes recentes verificam o escopo alterado; não autorizam declarar toda a campanha aprovada em uma única rodada nem a experiência concluída para qualquer público. A dublagem humana, a avaliação externa e a consolidação documental continuam como frentes de trabalho.

O principal aprendizado do processo é que qualidade depende da relação entre conteúdo, comportamento e evidência. Uma pista precisa existir, ser compreensível e funcionar após salvar; uma fala precisa corresponder ao estado narrativo; uma versão precisa ser identificável e verificável. A continuidade do projeto deve apoiar-se nessa relação, preservando os recursos aprovados e priorizando a experiência completa do jogador.
