# 1 INTRODUÇÃO

O Segredo de Varginha é um jogo de terror, investigação e aventura narrativa em duas dimensões, desenvolvido em Unity e C#, com perspectiva superior e arte em pixels. A produção escolar articula programação, narrativa, desenho de níveis, arte, som e testes. A ambientação em Varginha, o cotidiano de Edelzio e o Fusca aproximam uma história ficcional de referências locais.

Este relatório sintetiza a trajetória documentada até 23 de outubro de 2026. O primeiro commit acessível, de 11 de setembro, já contém um protótipo; portanto, não comprova quando a ideia surgiu. O acervo consultado reúne 49 commits alcançáveis, documentos de design, código, capturas e resultados técnicos. O último commit identificado é a2946be, de 22 de outubro, e existem alterações locais posteriores (Projeto O Segredo de Varginha, 2026a, 2026c).

O resultado consolidado é uma campanha de 14 capítulos em cinco atos, com exploração, enigmas, combate, aliados, persistência e uma conclusão implementada. A última build local identificada no levantamento é a 0.4.8-beta. A revisão mestre solicitada em seguida está em andamento e ainda não representa uma nova versão integralmente validada.

O objetivo geral é explicar como o projeto evoluiu, quais sistemas compõem o produto e quais limitações orientam sua continuidade. A análise distingue planejamento, implementação e verificação. Não atribui ao projeto finais alternativos apenas previstos no GDD, nem transforma resultados de testes parciais em aprovação de toda a experiência.

A relevância educacional está na aplicação de lógica, orientação a objetos, organização de estados, controle de versão e comunicação técnica. A autoria é apresentada coletivamente como Turma do 3º ano de Desenvolvimento de Sistemas da Escola Estadual Deputado Domingos de Figueiredo, sob acompanhamento dos professores Renan e Alex. Não foram inventadas divisões individuais de tarefas, carga horária ou orçamento.

# 2 REFERENCIAL TEÓRICO

O desenvolvimento de um jogo exige coerência entre regras, comportamento dos sistemas e experiência pretendida. Hunicke, LeBlanc e Zubek (2004) organizam essa relação pelo modelo MDA: mecânicas, dinâmicas e estética. Neste relatório, o modelo serve à análise retrospectiva; não se afirma que tenha sido adotado formalmente desde a concepção.

Em O Segredo de Varginha, examinar objetos, registrar pistas, resolver combinações e enfrentar ameaças são mecânicas. A alternância entre cotidiano, descoberta e perigo produz dinâmicas de investigação. Curiosidade e tensão são efeitos desejados, cuja intensidade depende da apresentação das pistas, do ritmo e da resposta dos controles. Esses efeitos não foram medidos com uma amostra de jogadores externos.

Os documentos de design de jogo e de níveis cumprem funções complementares. O GDD descreve proposta, personagens, sistemas e objetivos; o LDD organiza espaços, percursos e situações de interação. Para verificar o produto, ambos devem ser confrontados com a campanha em execução. Documentos históricos preservam decisões anteriores e não substituem automaticamente a configuração atual (Projeto O Segredo de Varginha, 2026b).

O versionamento registra alterações e permite comparar estados dos arquivos (Chacon; Straub, 2014). No projeto, o Git oferece marcos verificáveis da evolução. Quantidade de commits não equivale a produtividade individual ou tempo de trabalho; arquivos modificados localmente também precisam ser considerados antes de caracterizar a versão disponível.

A validação combina regras isoladas, integração em execução, compilação e inspeção visual. Um teste pode comprovar que um save mantém uma escolha sem demonstrar que a pista é compreensível. Da mesma forma, gerar um executável não comprova uma partida completa. Essa distinção fundamenta a leitura dos resultados e a identificação das pendências.

# 3 PROCEDIMENTOS METODOLÓGICOS

## 3.1 Levantamento e trajetória

Foi realizada análise documental do repositório, dos documentos de design e das evidências das entregas. O modelo fornecido orientou a apresentação. Código e manifestos recentes foram priorizados para descrever o estado implementado; documentos anteriores foram tratados como histórico. A Tabela 1 resume os principais marcos (Projeto O Segredo de Varginha, 2026a, 2026c).

@timeline

O desenvolvimento foi iterativo: novos conteúdos revelaram problemas de passagem, visibilidade, sequência de ações e compatibilidade de saves. A redução da campanha para 14 capítulos reuniu etapas e preservou identificadores internos, evitando que a renumeração visual exigisse descartar o progresso existente.

O levantamento não incluiu pesquisa com participantes, medição de desempenho em diferentes computadores ou cálculo de custos. A data inicial indica o primeiro registro disponível, e a síntese técnica não substitui uma auditoria histórica completa.

@page

## 3.2 Campanha arquitetura e mecânicas

A narrativa acompanha uma lembrança da infância de Edelzio, sua rotina adulta e a investigação de acontecimentos ligados a 1996 e ao casarão de 1898. Casa, escola, biblioteca, mata, igreja, oficina, arquivos e casarão organizam a descoberta de informações. O confronto com a manifestação e o retorno encerram o percurso disponível. O GDD prevê possibilidades mais amplas, mas somente uma conclusão foi identificada como implementada (Projeto O Segredo de Varginha, 2026b).

A campanha utiliza 19 cenas na build consultada. Capítulos apresentados ao jogador não correspondem individualmente a todas as cenas: existem áreas complementares e identificadores preservados. Controladores constroem parte dos objetos e da ambientação em execução. Scripts separam progressão, interação, puzzles, apresentação, áudio e persistência; alguns controladores ainda concentram várias responsabilidades.

A exploração usa movimento superior, aproximação de objetos, inventário, registro de evidências, objetivos e dicas. Os enigmas vinculam documentos e elementos do cenário a respostas verificáveis. Entre eles estão o mapa de trilhas, o Livro do Tombo, a comparação de arquivos, a memória e os mecanismos da investigação final. A qualidade desses sistemas depende da ordem de aquisição das pistas e da clareza das consequências.

O combate integra golpes, saúde, sanidade, inimigos e participação dos aliados. Renan, Ouzana e Fábio possuem apoios associados ao confronto final. O Fusca participa de deslocamentos e sequências cinematográficas. A rotina doméstica introduz controles e investigação por ações cotidianas, incluindo pia, café, notebook, mochila e chave.

Os saves registram avanço, respostas e estados de eventos. Rotinas de reparo tratam dados incompletos e versões anteriores. Teclado, mouse e controle compartilham os fluxos de interface. Opções de volume e redução de movimento ampliam a adaptação da apresentação; não constituem, por si só, comprovação de acessibilidade integral.

@page

## 3.3 Produção comunicação e qualidade

A identidade em pixel art combina personagens, cenários, objetos e animações. A Figura 1 registra a escola no acervo da remasterização; não é uma captura da build mais recente. O processo inclui organização de atlas, revisão de pivôs, escala e profundidade. Recursos assistidos por inteligência artificial estão documentados no acervo, mas a origem e as permissões de cada recurso precisam permanecer rastreáveis.

@fig

O áudio reúne efeitos, ambientes e canais com controle de volume. Cinemáticas articulam deslocamento, enquadramento, interferência e diálogo. O roteiro definitivo de Edelzio foi simplificado para gravação; a entrega do roteiro não comprova integração de todas as vozes. Os novos arquivos de música do menu e ambiente sombrio da igreja foram recebidos em 23 de outubro e entraram na revisão em curso.

A validação utiliza EditMode, PlayMode, Player Windows, capturas e builds. O recorte da versão 0.4.8 registra 29 testes EditMode e três casos no Player aprovados após correção de uma referência antiga no teste. Esses números não são cumulativos nem aprovam as alterações posteriores. Documentação, apresentações de acompanhamento, créditos, roteiros e materiais do website complementam o executável (Projeto O Segredo de Varginha, 2026c).

# 4 RESULTADOS ESPERADOS

O projeto alcançou uma campanha com começo, desenvolvimento e encerramento, ampliando um protótipo inicial por sucessivas integrações e revisões. O acervo evidencia aprendizagem em programação, organização de estados, composição audiovisual, investigação de defeitos e comunicação de entregas. A quantidade de sistemas exige, porém, consolidação e verificação conjunta antes de caracterizar o produto como concluído.

A revisão aprovada contempla colisões e profundidade, notebook e mobiliário, globo, restauração visual após glitch, composição do motorista e trajeto escolar. Também inclui a sequência mochila e chave, a investigação local das pistas, seleção direta de trilhas e reconstrução visual do Livro do Tombo. Complementam o escopo a conclusão discreta das fases, condução e destravamento do Fusca, sincronização dos sons cotidianos e consistência dos golpes sob entradas rápidas.

Na elaboração desta síntese, os dois novos áudios e a seleção direta das trilhas estão em implementação. A aprovação do plano não equivale à aprovação técnica das correções. Cada item deve receber evidências próprias, preservando arte, mecânicas funcionais, alterações locais e saves. O presente documento reduz o volume do relatório anterior sem apresentar como concluído o trabalho ainda pendente.

Espera-se uma candidata com regressão dirigida, inspeção visual dos trechos alterados e partida completa. Testes com iniciantes devem examinar compreensão das pistas, orientação, dificuldade e ritmo. A integração das vozes, a revisão de créditos e permissões e a correspondência entre documentos e versão distribuída permanecem necessárias para uma entrega consistente.

Conclui-se que a principal contribuição foi transformar uma proposta narrativa regional em um produto jogável documentado e verificável. A prioridade atual é consolidar essa base, com pendências rastreáveis e resultados demonstráveis. Não há, neste relatório, evidência de publicação final, avaliação externa abrangente ou implementação de todos os finais originalmente planejados.
