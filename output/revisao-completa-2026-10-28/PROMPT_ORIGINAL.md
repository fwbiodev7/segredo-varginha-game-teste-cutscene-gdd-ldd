# PROMPT MESTRE DEFINITIVO — OIZLEDE: FRAGMENTO PERDIDO
## Revisão completa de gameplay, narrativa, puzzles, cenários, colisões e cutscenes
**Data:** 28/10/2026  
**Destino:** Codex — projeto Unity existente  
**Prioridade:** implementação real, validação funcional e entrega de build jogável.

---

# 0. MISSÃO E INSTRUÇÕES OBRIGATÓRIAS

Retome o desenvolvimento de **OIZLEDE: FRAGMENTO PERDIDO** no mesmo workspace Unity, utilizando o projeto existente como fonte principal de implementação.

Esta tarefa reúne **23 solicitações de correção e melhoria** identificadas durante testes reais do jogo, além de ajustes de apresentação solicitados anteriormente.

**Não quero apenas um planejamento, uma auditoria documental ou sugestões de implementação. Quero que você investigue, modifique, integre, teste e entregue as correções efetivamente.**

Trabalhe autonomamente, tomando decisões técnicas fundamentadas, sem interromper a execução para pedir autorização sobre detalhes que possam ser resolvidos pela inspeção do projeto.

## 0.1. Fontes obrigatórias

Utilize os seguintes materiais:

1. **Todas as imagens anexadas nesta conversa:** são referências diretas dos problemas de interface, colisão, progressão, cenários e objetos invisíveis.
2. **`GDD(1).md`:** documento de referência narrativa e mecânica fornecido pelo usuário.
3. **`WhatsApp Audio 2026-10-28 at 8.03.04 AM.mpeg`:** áudio a integrar na cutscene da escola.
4. GDDs, LDDs, documentos de campanha, registros de QA e arquivos de continuidade existentes no repositório.
5. Scripts, cenas, prefabs, assets, materiais e testes atuais do Unity.

Caso os anexos da conversa não estejam materialmente disponíveis no ambiente do Codex, localize as cópias no workspace e use as descrições de referência deste prompt. Não declare ter inspecionado arquivos que não estejam acessíveis.

**Regra de precedência:** as mudanças explícitas deste prompt prevalecem sobre comportamentos antigos conflitantes. O GDD continua sendo a referência para fatos narrativos, cronologia, personagens e revelações não modificados expressamente aqui.

O GDD fornecido descreve seis atos narrativos, enquanto aponta para uma campanha experimental mais recente de 15 fases em cinco atos. Portanto, inspecione a campanha atual antes de alterar a numeração de fases ou assumir que a estrutura antiga ainda corresponde integralmente à implementação.

## 0.2. Preservação

- Não recrie o jogo do zero.
- Não substitua mapas inteiros quando uma correção localizada for suficiente.
- Não descaracterize o estilo pixel art existente.
- Não remova funcionalidades aprovadas para simplificar a implementação.
- Não modifique a solução de puzzles quando a solicitação for apenas de apresentação.
- Não altere os arquivos originais de referência sem necessidade.
- Não utilize filtros ou redimensionamentos que prejudiquem a nitidez dos sprites.
- Não introduza soluções temporárias que escondam o defeito sem resolver sua causa.
- Preserve alterações de outras branches ou agentes que ainda não tenham sido integradas, evitando sobrescritas indevidas.

Faça um checkpoint Git antes de alterações arriscadas e mantenha as mudanças organizadas por sistema.

---

# PARTE I — PUZZLES E INTERFACES

## 1. Puzzle “O que Fábio escondeu” — Igreja

### Problema identificado

A interface apresenta textos cortados, cartões muito próximos, descrições pouco legíveis e uma organização que dificulta compreender o fluxo de associação.

Na captura anexada, aparecem os campos de provas e destinos relativos aos registros de 1898, 1996 e 2026.

A própria instrução concorre visualmente com o título, e algumas descrições ultrapassam as áreas reservadas aos cartões.

### Alterações obrigatórias

**A. Reorganização da interface**

- Separar claramente o título, a explicação, as provas, os destinos e os controles.
- Aumentar a altura útil dos cartões.
- Ajustar margens internas e espaçamento vertical.
- Utilizar quebras de linha controladas.
- Corrigir clipping, sobreposições e textos truncados.
- Manter alinhamento visual consistente.
- Garantir boa legibilidade em todas as resoluções suportadas.

**B. Instruções intuitivas**

O jogador precisa entender que deve escolher uma prova e relacioná-la ao acontecimento ou período correspondente.

Sugestão:

“Cada documento revela uma parte da história. Relacione as pistas aos acontecimentos que elas ajudam a explicar.”

Evite apresentar diretamente as combinações corretas.

**C. Interação**

- Destacar a prova selecionada.
- Indicar claramente o destino escolhido.
- Exibir o número de vínculos concluídos.
- Permitir desfazer uma associação.
- Manter a conferência das respostas como etapa separada.
- Evitar que o erro revele automaticamente a solução.

### Preservar

Não alterar os acontecimentos históricos, a identidade das provas, a solução original nem a progressão associada ao puzzle.

---

## 2. Puzzle “Os registros de 1898” — Causas e consequências

### Problema identificado

As descrições aparecem cortadas, os cartões têm pouco espaço e determinados termos tornam a associação artificialmente confusa.

O objetivo não é tornar o puzzle fácil, mas eliminar a dificuldade causada pela interface.

### Correções visuais

- Corrigir todos os cartões de provas e destinos.
- Aumentar a altura dos campos que contêm descrições.
- Separar visualmente os dois grupos.
- Evitar fontes pequenas demais ou com baixo contraste.
- Melhorar a hierarquia entre nome, descrição e instruções.
- Preservar o estilo pixel art.

### Revisão textual

Utilize linguagem mais natural. Sugestões:

| Expressão atual | Alternativa |
|---|---|
| Reconstruir causas e consequências | Reconstrua o que aconteceu |
| Vestígios | Pistas, quando apropriado |
| A descoberta | O primeiro indício |
| As experiências | Os experimentos |
| A ruptura | O incidente |
| A resposta: contenção | A tentativa de contenção |
| Instrumentos • ligação a pessoas | Instrumentos • marcas de uso |
| Sala • danos da ruptura | Sala • sinais de destruição |

Essas substituições são propostas de clareza, não autorização para alterar a história. Verifique o significado de cada termo antes de aplicar.

### Mecânica

O jogador deve continuar relacionando evidências aos acontecimentos correspondentes.

Preservar todas as combinações corretas e as condições de conclusão.

### Validação

Testar três situações: sem associações, associações incorretas e solução correta. Verificar ausência de textos cortados em todos os estados.

---

## 3. Marcadores de objetivos — Substituir quadrados ciano

### Referência visual

Foi anexada uma captura aproximada mostrando um pequeno quadrado azul-claro/ciano no chão.

**É esse quadrado que deve ser substituído.**

Não confunda com o marcador azul que representa a posição do jogador no mapa.

### Novo indicador

Substitua os quadrados ciano que representam objetivos por:

- Símbolo: ponto de exclamação `!`.
- Cor: amarelo.
- Contorno: escuro, para melhorar a legibilidade.
- Estilo: pixel art.
- Tamanho: aproximadamente **2,5 vezes o marcador ciano original**.
- Posição: centralizada no local do objetivo.

### Comportamento

- Exibir somente nos objetivos ativos.
- Atualizar quando a missão mudar.
- Ocultar após a conclusão.
- Evitar duplicatas.
- Não deslocar os triggers associados.
- Não alterar os indicadores de localização do jogador.

Inspecione primeiro o sistema existente de objetivos, evitando criar um segundo gerenciador desnecessário.

---

## 4. Prólogo de 1996 — Feixe de luz e motivação narrativa

### Problema identificado

Na cena em que Edelzio criança está próximo ao quintal, falta uma motivação clara para investigar a luz ou a explosão.

O jogador não deve precisar adivinhar para onde ir.

### Referência do GDD

O Ato I — A Lembrança descreve:

- Oscilações elétricas.
- Interferência na televisão.
- Um brilho que atrai Edelzio ao quintal.
- Um feixe de luz.
- Uma figura indistinta.
- Fragmentação das memórias.

Esses elementos devem orientar a implementação.

### Nova sequência

**Momento 1 — A anomalia**

Depois que Edelzio sai de casa, a iluminação começa a oscilar discretamente.

O ambiente fica mais silencioso, enquanto um ruído elétrico distante surge.

**Momento 2 — O primeiro clarão**

Uma luz anormal aparece no quintal, refletindo parcialmente na vegetação.

O efeito deve ser forte o suficiente para atrair a atenção, mas não deve revelar sua origem.

**Momento 3 — Reação de Edelzio**

Adicionar uma fala natural, como:

“Ué... que luz é aquela?”

**Momento 4 — Aproximação**

Enquanto o jogador se desloca em direção ao brilho:

- O ruído aumenta.
- A vegetação recebe reflexos discretos.
- Pequenas partículas podem surgir.
- A iluminação pulsa gradualmente.
- A sensação de anomalia se intensifica.

**Momento 5 — Manifestação**

Ao alcançar a área apropriada, ativar a cutscene do feixe e da figura indistinta.

Integrar à sequência já existente, preservando o corte narrativo.

### Requisitos técnicos

- Utilizar estados de evento para impedir disparos duplicados.
- Preservar liberdade de movimento antes da cutscene.
- Não teletransportar o jogador sem justificativa.
- Respeitar colisões e posição do personagem.
- Sincronizar iluminação, som e transição.
- Não revelar prematuramente a identidade da entidade.

---

# PARTE II — PROGRESSÃO, TRIGGERS E FUSCA

## 5. Auditoria de triggers em todos os mapas

### Objetivo

Verificar e corrigir os triggers de **todas as fases e ambientes** do projeto.

O problema pode envolver eventos de progressão, portas, objetos interativos e cutscenes, não apenas colisões físicas.

### Auditar

- Entradas e saídas.
- Passagens entre mapas.
- Portas e escadas.
- Objetivos narrativos.
- Coleta de documentos.
- Puzzles.
- Interações com NPCs.
- Cutscenes.
- Combates.
- Eventos sobrenaturais.
- Fusca e viagens.
- Conclusão de fases.

### Diagnóstico técnico

Inspecionar:

- `Collider2D`.
- `Rigidbody2D`.
- `isTrigger`.
- Layers e Physics 2D Collision Matrix.
- `OnTriggerEnter2D`.
- `OnTriggerStay2D`.
- `OnTriggerExit2D`.
- Flags narrativas.
- Ordem de inicialização.
- Objetos criados em runtime.
- Inscrição e remoção de listeners.
- Sistemas de save/load.

### Casos obrigatórios

- Trigger inacessível.
- Trigger deslocado em relação ao objeto.
- Evento disparado várias vezes.
- Evento disparado antes da condição narrativa.
- Evento que deixa de funcionar após reiniciar.
- Transição para posição incorreta.
- Interação possível através de paredes.
- Dois triggers ativando eventos conflitantes.

### Entrega

Gerar um inventário de triggers por fase, identificando o comportamento esperado e a correção aplicada.

Não declarar a auditoria concluída apenas porque o código compila.

---

## 6. Corrigir a sequência narrativa da chave e da mochila

### Problema

A ordem atual está invertida.

Edelzio deve primeiro tentar sair de casa e somente depois perceber que esqueceu a mochila.

### Ordem obrigatória

**Estado 1 — Preparação para sair**

Edelzio pega a chave do Fusca e se prepara para sair.

**Estado 2 — Primeira aproximação ao Fusca**

O jogador vai até o carro.

A interação inicia o evento em que Edelzio percebe que esqueceu a mochila.

Ele ainda não embarca definitivamente.

**Estado 3 — Retorno à casa**

Edelzio volta para buscar a mochila.

Durante essa sequência, deixa a chave do Fusca em cima da mesa, na posição mostrada nas referências.

**Estado 4 — Buscar a mochila**

O jogador recupera a mochila.

**Estado 5 — Desaparecimento**

Quando Edelzio retorna ou procura a chave, percebe que ela não está mais na mesa.

**Estado 6 — Investigação**

A investigação do desaparecimento é iniciada.

Preservar a relação com a caixa de brinquedos, o caderno de 1996, as anotações e as informações recuperadas, conforme o GDD e a implementação atual.

**Estado 7 — Recuperação e saída**

Após completar os requisitos, Edelzio pode retornar ao Fusca e prosseguir.

### Requisitos técnicos

Representar a progressão por estados explícitos, em vez de verificações independentes que permitam acontecimentos fora da ordem.

Exemplo conceitual:

`ChaveObtida → TentativaDeSaida → MochilaEsquecida → ChaveNaMesa → MochilaRecuperada → ChaveDesaparecida → Investigacao → ChaveRecuperada → SaidaLiberada`

Os nomes reais devem seguir a arquitetura existente.

### Evitar

- Duplicação física da chave.
- Inventário indicando posse quando a chave está na mesa.
- Mochila sendo exigida antes da primeira aproximação ao Fusca.
- Eventos ocorrendo fora da sequência.
- Perda de progresso após salvar ou reiniciar.

---

## 7. Folhas espalhadas no chão — Aparição condicionada

### Problema

As folhas aparecem antes do momento correto.

Isso antecipa uma pista da investigação.

### Correção

As folhas mostradas na captura devem aparecer **somente após o desaparecimento da chave**.

Antes disso, devem permanecer invisíveis.

Após o evento, devem ser ativadas na posição prevista, sem mudanças indevidas na decoração.

### Requisitos

- Vincular visibilidade ao estado narrativo.
- Preservar persistência.
- Não gerar duplicatas.
- Não reativar após o evento já estar concluído, exceto quando a lógica da fase exigir.
- Não esconder outros documentos decorativos que não pertençam a essa sequência.

---

## 8. Corrigir acesso físico ao Fusca

### Problema

Na cena externa da casa, Edelzio fica bloqueado pela região da calçada e não consegue alcançar o veículo.

As imagens mostram o personagem tentando atravessar a transição entre o passeio e a área onde o carro está estacionado.

### Correções

- Inspecionar colisões da calçada.
- Verificar o meio-fio.
- Verificar obstáculos invisíveis.
- Conferir limites da rua.
- Verificar a área de interação do veículo.
- Corrigir diferenças entre collider visual e físico.

O personagem precisa alcançar normalmente a região próxima à porta do motorista.

### Atenção

**Bloqueio narrativo não deve ser implementado como bloqueio físico arbitrário.**

Se Edelzio ainda não pode partir, ele deve conseguir chegar ao Fusca e receber o evento ou diálogo apropriado.

Preservar colisões reais com a carroceria e com construções.

---

## 9. Embarque simplificado no Fusca — Sem animação de porta

### Decisão definitiva

Remover a animação de abertura e fechamento da porta na sequência indicada.

O Fusca deve permanecer fechado.

### Funcionamento

1. Edelzio se aproxima da região do motorista.
2. Interage com o veículo.
3. O sistema verifica os requisitos narrativos.
4. Se estiverem incompletos, apresenta o diálogo correspondente.
5. Se estiverem completos, o personagem entra instantaneamente no estado de motorista.
6. Seu sprite externo desaparece.
7. A viagem existente continua normalmente.

### Não fazer

- Não criar animações intermediárias de caminhada.
- Não abrir ou fechar portas.
- Não deixar uma segunda cópia de Edelzio no cenário.
- Não alterar a aparência aprovada do Fusca.

### Garantias

Sincronizar câmera, controles, colisão, inventário e transição.

Preservar o funcionamento do veículo nas demais fases, adaptando a simplificação às sequências relevantes sem quebrar cutscenes distintas.

---

## 10. Carro passando com música — Cutscene da escola

### Referência

Cena externa da **E.E. Dep. Domingos de Figueiredo**, com Edelzio próximo à faixa de pedestres, Fusca estacionado e fachada da escola ao fundo.

### Áudio fornecido

`WhatsApp Audio 2026-10-28 at 8.03.04 AM.mpeg`

Utilize esse arquivo como áudio da música que toca no carro.

### Comportamento desejado

Adicionar um carro que atravesse a rua durante a cutscene.

O veículo deve estar com o som alto, como alguém passando rapidamente pela cidade ouvindo música.

**Sequência sonora:**

- Ao longe: música pouco audível.
- Aproximação: volume aumentando.
- Passagem próxima: volume alto.
- Afastamento: volume diminuindo gradualmente.
- Saída da cena: áudio desaparece naturalmente.

### Implementação

- Criar ou reutilizar um veículo coerente com a estética pixel art.
- Movimentá-lo por trajetória validada.
- Respeitar a perspectiva top-down.
- Não atravessar o Fusca estacionado.
- Não passar sobre o personagem.
- Não atravessar construções e calçadas indevidamente.
- Usar `AudioSource` associado ao veículo.
- Configurar atenuação espacial apropriada.
- Aplicar Doppler sutil, se adequado.
- Preservar a mixagem da trilha e dos diálogos.

O som deve parecer originar-se do próprio carro, e não da música ambiente global.

### Evento

Executar como acontecimento ambiental da cutscene, evitando reinícios involuntários.

Liberar adequadamente áudio e objeto após a passagem.

### Validação

Capturar ou gravar a cena em execução, conferindo movimento e progressão do volume.

---

# PARTE III — PUZZLE DA ESCOLA: PADRÕES OCULTOS

## 11. Redesign geral de “Padrões Ocultos + Meus Documentos”

### Problema

A interface está muito carregada, com títulos grandes, informações redundantes, cartões excessivos e muitas ações simultaneamente visíveis.

A mecânica deve permanecer dividida em quatro etapas:

1. Ordenar.
2. Comparar.
3. Cruzar.
4. Concluir.

### Nova organização

**Cabeçalho:** título compacto e indicador das quatro etapas.

**Conteúdo central:** apenas a mecânica relevante para a etapa atual.

**Rodapé:** botões essenciais, com espaçamento adequado.

Não exibir simultaneamente controles de etapas diferentes.

### Etapa Ordenar

Preservar a ordenação cronológica dos documentos de 1996.

Mostrar de forma clara:

- Data.
- Título.
- Opção de leitura.
- Posição atual do documento.

Evitar repetir descrições que não contribuam para a ordenação.

### Obrigatoriedade de leitura

A leitura obrigatória de cada documento deve ocorrer **somente na primeira etapa**.

Depois que o documento foi lido, seu estado deve ser preservado.

Não obrigar o jogador a repetir o processo nas etapas seguintes.

### Corrigir abertura dos documentos

Há um defeito em que clicar para ler um documento pode pular o arquivo esperado ou exigir uma segunda interação.

Corrigir para que:

- Um clique abra o arquivo correto.
- Nenhum documento seja ignorado.
- As anotações não alterem o índice do documento.
- A ordem canônica seja estável.
- A ordem visual embaralhada não seja usada como identidade.
- O estado de leitura pertença ao documento, não à posição do cartão.

Usar IDs persistentes.

### Exceções

As etapas Comparar e Concluir terão restrições específicas de leitura, descritas nos itens seguintes.

Essas restrições prevalecem sobre a regra geral de consulta opcional.

---

## 12. Etapa Comparar — Múltipla escolha baseada em memória

### Alteração obrigatória

Remover a mecânica atual de selecionar fatos repetidos diretamente em vários cartões.

Substituí-la por uma única questão de múltipla escolha:

**“Qual destes acontecimentos não se repetiu nos registros?”**

### Mecânica

- Apresentar alternativas baseadas nos acontecimentos encontrados durante Ordenar.
- Permitir selecionar somente uma alternativa.
- Confirmar a resposta.
- Se estiver correta, liberar Cruzar.
- Se estiver incorreta, permitir nova tentativa sem revelar a solução.

### Regra principal

**O jogador não poderá ler novamente os arquivos durante Comparar.**

Essa etapa deve testar atenção, memória e compreensão do que foi lido anteriormente.

### Interface

Remover:

- Cartões completos dos documentos.
- Botões “Ler”.
- Caixas individuais de seleção de trechos.
- Descrições repetidas.

Manter:

- Pergunta.
- Alternativas.
- Confirmação.
- Navegação essencial.
- Indicador da etapa.

### Importante

Criar distratores plausíveis a partir dos documentos existentes.

Não inventar acontecimentos que contradigam o GDD.

Não tornar a pergunta óbvia com alternativas absurdas.

---

## 13. Etapa Concluir — Não entregar a resposta

### Problema

O texto apresentado explica antecipadamente a relação entre o acervo, a coleção C-96 e a localização dos originais.

Isso elimina a necessidade de raciocínio.

### Nova interface

Manter somente:

- Pergunta principal.
- Alternativas existentes.
- Botão **“Consultar mural da sala”**.
- Confirmação.
- Navegação essencial.

### Remover

- A descrição extensa sobre o acervo.
- A explicação que antecipa onde estão os documentos.
- O botão “Ler recortes / origem”.
- Informações que identificam diretamente a resposta.

### Investigação

O mural deve continuar disponível como consulta contextual.

Entretanto, seu conteúdo não deve indicar de maneira explícita a alternativa correta.

O jogador deve interpretar as informações aprendidas nas etapas anteriores.

### Regra

Não permitir abrir arquivos completos nessa etapa.

Não exigir novas leituras.

Preservar as alternativas, a resposta correta e a conclusão original.

---

# PARTE IV — CENÁRIOS, INVESTIGAÇÃO E COLISÕES

## 14. Biblioteca — Destacar somente os itens interativos

### Referência

Na imagem da biblioteca, o globo apresenta um acabamento visual mais chamativo do que outros objetos do cenário.

Utilize-o como referência de qualidade artística.

### Objetivo

Diferenciar objetos interativos de elementos decorativos sem transformar a fase em um cenário cheio de brilhos artificiais.

### Alterações

- Identificar os objetos necessários à investigação.
- Melhorar a arte e a legibilidade visual desses objetos.
- Utilizar contraste, saturação ou acabamento moderadamente mais perceptível.
- Preservar o tamanho e a perspectiva.
- Manter coerência com a iluminação e as sombras.
- Evitar destacar objetos decorativos irrelevantes.

Exemplos condicionados à lógica real da fase:

- Globo.
- Livros específicos.
- Registros.
- Murais.
- Objetos de consulta.

### Regra

Não adicionar destaques indiscriminadamente a todos os móveis.

A diferença visual deve corresponder à função real de cada objeto.

---

## 15. Auditoria global de colisões — Todos os mapas

### Problema

Na biblioteca, Edelzio consegue caminhar sobre estantes e ocupar regiões que deveriam estar bloqueadas.

A imagem mostra o personagem na altura da estante e da janela, onde não deveria existir área transitável.

O usuário também identificou o mesmo problema na igreja.

### Escopo

Revisar **todos os mapas e fases**, não apenas biblioteca e igreja.

### Verificações

- Paredes externas.
- Paredes internas.
- Estantes.
- Mesas.
- Armários.
- Colunas.
- Altares.
- Bancos.
- Móveis.
- Árvores.
- Rochas.
- Cercas.
- Limites de terreno.
- Portas.
- Corredores.
- Passagens legítimas.
- Objetos criados proceduralmente.

### Causa raiz

Investigar a movimentação do personagem e o modo como o projeto cria ou atualiza colisores.

Verificar especialmente:

- Alterações diretas em `transform.position`.
- Movimento que ignora resolução de colisões.
- Colliders ausentes.
- Layers incorretas.
- Colliders desativados.
- Desalinhamento de pivôs.
- Posição ou escala incorreta de objetos criados em runtime.

### Correção

Aplicar movimentação e colisões físicas consistentes com o controlador atual.

Não modificar a sensação de movimentação sem necessidade.

Não criar paredes invisíveis arbitrárias que impeçam o jogador de acessar áreas legítimas.

### Testes

Verificar:

- Caminhada nas quatro direções.
- Movimento diagonal.
- Corrida.
- Esquiva.
- Colisão em cantos.
- Aproximação de objetos.
- Transições.
- Teletransportes de cutscenes.
- Retorno após carregar saves.

O jogador deve permanecer dentro das regiões realmente transitáveis.

---

## 16. Mata — Tornar os pontos de investigação intrigantes

### Problema

Na fase da mata, o jogador encontra pequenos pontos indicando que pode examinar a vegetação.

Esses indicadores não comunicam o que há de suspeito no local.

### Objetivo

Transformar a investigação em algo visual e ambiental.

O jogador deve querer examinar uma região porque percebe que existe algo estranho ali.

### Novas pistas visuais

Utilizar, conforme o contexto narrativo:

**Pegadas estranhas**

Marcas de pés ou garras incomuns no barro, com proporções que provoquem dúvida.

**Marcas de arrasto**

Solo revolvido, vegetação esmagada e pequenos sulcos.

**Vestígios de sangue**

Pequenas manchas escuras ou avermelhadas, usadas com moderação.

**Vegetação alterada**

Folhas amassadas, plantas mortas ou crescimentos anormais.

**Galhos quebrados**

Sinais de passagem de algo pesado.

**Marcas geométricas**

Padrões discretos na vegetação, coerentes com as anomalias descritas no GDD.

**Substâncias estranhas**

Resíduos luminosos ou alterações no solo.

### Integração com o GDD

A mata está associada a alterações na natureza, à investigação de Ouzana e às consequências da ruptura.

Preservar essa coerência.

### Implementação

- Associar cada vestígio a um ponto investigável real.
- Posicionar sprites sobre o terreno correto.
- Ajustar `sortingOrder`.
- Preservar vegetação e ambientação.
- Evitar marcadores genéricos como única indicação.
- Não impedir navegação.
- Sincronizar estado de investigação e progressão.

### Validação

O jogador deve conseguir identificar a região suspeita antes de receber o prompt de interação.

Não espalhar pistas sem função narrativa.

---

## 17. Igreja antiga de Padre Fábio — Redesign e perseguição

### Problema visual

O esconderijo atual parece uma estrutura pequena, deteriorada e pouco reconhecível como igreja.

Foi fornecida uma imagem de referência mostrando uma igreja de arquitetura gótica, com:

- Fachada religiosa.
- Telhado preservado.
- Vitrais.
- Portas de madeira.
- Cruz.
- Paredes antigas.
- Vegetação.
- Elementos arquitetônicos tradicionais.

### Direção artística

**A igreja deve ser antiga, mas não destruída.**

Ela pode apresentar:

- Pedra envelhecida.
- Musgo.
- Madeira antiga.
- Vitrais preservados.
- Iluminação fraca.
- Algumas marcas do tempo.

Não utilizar:

- Telhados desabados.
- Grandes paredes destruídas.
- Excesso de entulho.
- Estrutura que pareça um templo em ruínas.

### Adaptação

A referência é arquitetônica.

Não inserir diretamente a imagem isométrica como cenário top-down.

Reconstruir ou adaptar a apresentação respeitando a câmera e a linguagem pixel art do projeto.

### Continuidade com as demais fases

A igreja antiga deve ser reconhecível como o local religioso já apresentado em outras partes da campanha.

Reutilizar sua identidade visual, arquitetura e lógica espacial.

### Nova sequência de perseguição

Durante a fuga do ET, **Edelzio deve correr até a igreja antiga para se proteger**.

1. O ET começa a perseguir Edelzio.
2. O jogador percebe que a igreja pode servir de refúgio.
3. Edelzio alcança a entrada.
4. O trigger da porta inicia a transição.
5. O personagem entra na igreja.
6. A perseguição externa termina.
7. Padre Fábio está no interior.
8. O encontro narrativo continua conforme a história.

### Requisitos

- A porta precisa ser acessível.
- O ET não pode continuar aplicando dano após a transição segura.
- Não duplicar Padre Fábio.
- Não executar novamente o evento da perseguição depois de concluído.
- Preservar sanidade e progresso.
- Posicionar Edelzio corretamente no interior.
- Preservar a cronologia da campanha.

### Validação

Testar a perseguição completa, a chegada à igreja, a transição, o encontro com Padre Fábio e a continuidade da fase.

---

## 18. Igreja — Evidências visíveis e puzzle “Livro do Tombo”

### Problema

No puzzle “Livro do Tombo + Reconstruir Registro”, as evidências aparecem como não encontradas, mas não estão suficientemente visíveis ou identificáveis no cenário.

Além disso, a apresentação das associações é confusa.

### A. Materializar as evidências

Identificar os três elementos necessários para resolver o puzzle.

Garantir que sejam representados por objetos visíveis, acessíveis e coerentes com a igreja.

Utilizar locais narrativamente adequados, como arquivos, mesas, livros, móveis ou áreas de investigação.

Não criar interações que aparentem ocorrer no vazio.

### B. Preservar os campos originais

Manter os campos:

**Carimbo do arquivo — Número do registro**

**Diocese + Livro do Tombo — Ano do registro**

**Identificação: Edelzio — Símbolo do compartilhamento**

Não alterar os títulos, a ordem, os temas nem as respostas corretas.

### C. Simplificar somente o bloco “Associar pista”

Ao abrir a lista de evidências disponíveis, substituir descrições excessivamente abstratas por palavras mais compreensíveis.

Ajustar apenas a linguagem das evidências selecionáveis.

Não alterar os campos centrais.

Não alterar as combinações corretas.

Não transformar o puzzle em múltipla escolha.

### D. Estado das evidências

Verificar:

- Criação dos objetos.
- Trigger de coleta.
- ID da evidência.
- Registro no inventário ou sistema de investigação.
- Atualização do puzzle.
- Persistência entre carregamentos.
- Possibilidade de concluir a associação.

### Validação

Executar uma rota completa de exploração, coleta das três evidências, abertura do Livro do Tombo, associação e conferência.

---

## 19. Oficina — Fusca atravessando paredes

### Problema

Na fase da oficina, o Fusca se movimenta atravessando estruturas físicas e aparenta andar pelas paredes.

Isso indica possível problema de trajetória, controlador, colisão ou posicionamento.

### Investigação

Inspecionar:

- Sistema de deslocamento do veículo.
- Trajetórias de cutscene.
- Colliders.
- Rigidbody2D.
- Layers.
- Pontos de origem e destino.
- Pivôs.
- Escala do veículo.
- Limites transitáveis.
- Obstáculos da oficina.

### Correção

Garantir que o Fusca permaneça nas regiões transitáveis.

Se o movimento for cinematográfico, validar a trajetória antes da execução e impedir interseção com obstáculos.

Se houver direção controlada pelo jogador, respeitar colisões sem causar travamentos.

### Preservar

- Orientação visual.
- Proporções do Fusca.
- Faróis.
- Movimento fluido.
- Progressão da cena.

### Testes

Verificar movimento, curvas, retorno, reinício e colisão com obstáculos sob diferentes condições de execução.

---

## 20. Puzzle “Arquivos Alterados” — Embaralhar alternativas

### Problema

As seis alternativas aparecem sempre na mesma posição.

### Alternativas existentes

- Acesso lateral.
- Data do registro.
- Porta principal.
- Jardim.
- Assinatura.
- Escada interna.

### Alteração

Embaralhar a ordem visual a cada nova tentativa.

### Implementação

Utilizar Fisher–Yates ou algoritmo equivalente.

Manter identificadores estáveis e separar:

- ID da alternativa.
- Posição visual.
- Estado de seleção.
- Validação da resposta.

### Persistência

Ao abrir a tentativa pela primeira vez, gerar a ordem.

Ao fechar e reabrir sem reiniciar, manter a ordem atual.

Ao iniciar uma nova tentativa, embaralhar novamente.

Se existir persistência de tentativas, salvar a ordem e os estados correspondentes.

### Não alterar

- Desenhos das plantas.
- Solução correta.
- Diferenças entre documentos.
- Regras narrativas.
- Botão de confirmação.

### Validação

Executar múltiplas tentativas e confirmar que a resposta correta continua válida independentemente da posição.

---

## 21. Casarão de Zé Gomes — Objetos físicos de investigação

### Problema

Várias evidências são representadas apenas por pequenos pontos.

O jogador recebe uma interação, mas o cenário não comunica claramente o que está sendo examinado.

### Nova abordagem

Materializar cada objeto investigável.

Dependendo da evidência real:

- Documentos antigos.
- Mapas.
- Plantas arquitetônicas.
- Cadernos.
- Fotografias.
- Livros de pesquisa.
- Instrumentos.
- Objetos ligados às experiências.
- Marcas no ambiente.
- Vestígios de passagens ocultas.

### Requisitos visuais

- Sprites com identidade própria.
- Proporções corretas.
- Iluminação coerente.
- Profundidade adequada.
- Contraste suficiente.
- Integração com mesas, estantes e pisos.

### Requisitos funcionais

- O trigger deve corresponder à posição visual.
- A interação não pode ocorrer através de paredes.
- Objetos obrigatórios devem estar acessíveis.
- O jogador deve perceber o objeto antes do texto de interação.
- Não alterar progressão e soluções existentes.

---

## 22. Chave física no casarão — Substituir pequeno ponto

### Referência

Na captura, um pequeno ponto ciano aparece sobre ou próximo a um gaveteiro, mas a chave não possui uma representação física perceptível.

### Correção

Substituir a representação abstrata por uma chave em pixel art.

### Visual

- Forma reconhecível de chave.
- Tamanho proporcional ao móvel.
- Contorno e contraste adequados.
- Posição coerente sobre a superfície.
- Iluminação e sombras consistentes.
- Sem aparência de objeto flutuante.

### Interação

- Mostrar a chave antes da coleta.
- Exibir o prompt ao se aproximar.
- Registrar a interação corretamente.
- Atualizar o inventário ou estado da investigação.
- Ocultar a chave quando ela for coletada, se a mecânica exigir.
- Preservar seu estado após salvar e carregar.

### Atenção

Não confundir essa evidência do casarão com a chave do Fusca na sequência da casa.

São referências narrativas e contextos diferentes; preserve os identificadores corretos.

---

## 23. Porão / Sala dos Materiais — Corrigir objetos invisíveis

### Problema

Na captura da Sala dos Materiais, o prompt indica **“Materiais do mecanismo”**, mas não existe uma representação visual clara do item correspondente.

Edelzio parece estar examinando o ar.

### Correção obrigatória

Todos os objetos importantes dessa sala devem aparecer fisicamente no cenário.

### Possíveis representações

Conforme os itens reais da fase:

- Peças do mecanismo.
- Componentes metálicos.
- Engrenagens.
- Documentos.
- Anotações.
- Frascos.
- Instrumentos.
- Materiais de pesquisa.
- Objetos utilizados em experimentos.

Não adicionar elementos sem relação com a história.

### Implementação

Para cada interação:

1. Identificar o objeto narrativo correspondente.
2. Localizar o trigger.
3. Verificar a existência de sprite.
4. Criar ou reutilizar representação visual adequada.
5. Posicionar o objeto corretamente.
6. Ajustar profundidade e escala.
7. Alinhar a área de interação.
8. Validar exame, coleta ou mudança de estado.

### Auditoria complementar

Procurar a mesma falha em outros ambientes:

- Depósitos.
- Laboratórios.
- Salas de arquivo.
- Porões.
- Salas técnicas.
- Áreas subterrâneas.

### Regra definitiva

**Nenhuma evidência obrigatória deve existir apenas como um trigger invisível ou um ponto genérico quando a narrativa pressupõe um objeto físico examinável.**

---

# PARTE V — AJUSTES ANTERIORES DE MENU E EXECUTÁVEL

Estas solicitações também foram feitas nesta conversa. Verifique sua implementação atual e preserve o que já estiver corretamente integrado.

## 24. Frases rotativas do menu

Manter as quatro frases rotativas existentes e torná-las um pouco mais perceptíveis, sem retirar sua discrição.

Elas devem funcionar como pequenas referências na interface.

- Preservar rotação entre entradas no menu.
- Evitar repetir imediatamente a mesma frase.
- Melhorar contraste sem transformar as legendas em títulos.
- Não comprometer a leitura da logo.
- Preservar a identidade fria e a paleta aprovada.

## 25. Logo e identidade visual

Preservar a logo **OIZLEDE: FRAGMENTO PERDIDO** já integrada, incluindo o arquivo `Assets/Resources/Varginha/MenuLogoOizlede.png`, seu tratamento visual, transparência e posicionamento aprovado.

Não recriar a logo sem necessidade.

Preservar o fundo animado, a música do menu, os controles e a navegação.

## 26. Nome do executável

A saída final para Windows deve utilizar:

`Oizlede - Segredo de Varginha.exe`

Preservar **Oizlede: Fragmento Perdido** como identidade principal apresentada no menu, salvo se a configuração do projeto já estabelecer outra distinção aprovada entre título comercial e nome do executável.

Verificar também nome da pasta de entrega, metadados e atalhos quando aplicável.

---

# PARTE VI — DIRETRIZES TÉCNICAS GLOBAIS

## 27. Arquitetura e qualidade do código

Antes de modificar os sistemas, faça uma leitura direcionada da arquitetura atual.

Identifique os componentes responsáveis por:

- Progressão da campanha.
- Triggers.
- Colisões.
- Movimento de Edelzio.
- Fusca.
- Cutscenes.
- Gerenciamento dos puzzles.
- Evidências e inventário.
- Salvamento.
- Renderização de objetos.
- Áudio.
- HUD.
- Construção procedural dos ambientes.

### Evitar

- Criar scripts duplicados para problemas já resolvidos por sistemas existentes.
- Colocar regras narrativas em componentes puramente visuais.
- Criar flags locais inconsistentes com o estado global.
- Usar IDs derivados da posição visual de cartões.
- Criar colliders invisíveis improvisados sem diagnóstico.
- Corrigir problemas de arte apenas com filtros quando o objeto original estiver ausente.

### Preferir

- Reutilização dos controladores existentes.
- Estados narrativos explícitos.
- Identificadores persistentes.
- Dados separados da apresentação.
- Testes de regressão.
- Logs de eventos críticos.
- Correções na fonte quando o defeito for compartilhado.

---

## 28. Sistema de evidências — Padronização visual

Os problemas da biblioteca, igreja, mata, casarão e porão indicam a necessidade de revisar a apresentação dos objetos investigáveis.

Crie uma abordagem consistente para evidências físicas, sem obrigatoriamente criar um novo framework.

Cada evidência deve possuir, quando aplicável:

- Identificador único.
- Referência ao sprite.
- Posição.
- Estado de disponibilidade.
- Estado de descoberta.
- Interação associada.
- Requisitos narrativos.
- Comportamento após coleta.
- Persistência.

### Regra visual

A evidência deve ser reconhecível antes que o jogador se aproxime.

Marcadores podem complementar a descoberta, mas não substituir a representação do objeto.

### Regra funcional

O objeto visual e a área de interação precisam corresponder ao mesmo elemento narrativo.

---

## 29. Renderização e fidelidade pixel art

Preservar a estética existente.

Priorizar:

- Filtragem Point para sprites pixel art.
- Ausência de mipmaps nos sprites apropriados.
- Escalas coerentes.
- Pivôs corretos.
- Ordenação por profundidade.
- Transparência adequada.
- Ausência de halos indesejados.
- Iluminação consistente.

Não reconstruir arbitrariamente personagens, móveis ou mapas que já tenham aparência aprovada.

Se uma nova arte for necessária, usar como referência os sprites canônicos existentes e registrar sua procedência.

---

## 30. Performance

As correções não devem comprometer a fluidez.

Evitar:

- Alocações repetidas na renderização de interface.
- Carregamento contínuo de sprites.
- Instanciação de efeitos a cada frame.
- Recriação desnecessária de colliders.
- `FindObjectsOfType` ou buscas globais recorrentes em loops críticos.
- Duplicação de AudioSources.
- Vazamento de objetos temporários entre cenas.

Reutilizar e liberar recursos adequadamente.

---

# PARTE VII — PLANO DE EXECUÇÃO

Execute os trabalhos por dependência técnica e risco de regressão.

## Etapa A — Segurança e diagnóstico

1. Verificar estado do repositório.
2. Registrar branch, commit e alterações existentes.
3. Criar checkpoint seguro.
4. Inspecionar os arquivos relevantes.
5. Mapear os problemas relatados.
6. Identificar componentes compartilhados.

## Etapa B — Bloqueios críticos de gameplay

Priorizar:

- Auditoria de colisões.
- Auditoria de triggers.
- Acesso ao Fusca.
- Sequência da chave e mochila.
- Folhas condicionadas ao desaparecimento.
- Embarque instantâneo.
- Trajetória do Fusca na oficina.
- Perseguição e entrada na igreja.

Esses problemas podem impedir o avanço normal na campanha.

## Etapa C — Puzzles

Corrigir:

- O que Fábio escondeu.
- Registros de 1898.
- Padrões Ocultos.
- Etapa Comparar.
- Etapa Concluir.
- Livro do Tombo.
- Arquivos Alterados.

## Etapa D — Arte investigativa

Implementar os objetos físicos e destaques seletivos na:

- Biblioteca.
- Igreja.
- Mata.
- Casarão.
- Sala dos Materiais.

## Etapa E — Apresentação e cinematografia

Concluir:

- Feixe de luz do prólogo.
- Redesign da igreja antiga.
- Carro musical na escola.
- Ajustes de indicadores.
- Revisão do menu.
- Nome do executável.

## Etapa F — Integração e regressão

Executar testes completos e corrigir problemas detectados antes de gerar a build.

---

# PARTE VIII — VALIDAÇÃO OBRIGATÓRIA

## 31. Testes automatizados

Criar ou expandir testes Unity Edit Mode e Play Mode conforme necessário.

Cobrir:

- Estados da chave e mochila.
- Aparição das folhas.
- Interação com o Fusca.
- Triggers de transição.
- Identificadores das evidências.
- Persistência de documentos.
- Restrições de leitura dos puzzles.
- Embaralhamento de alternativas.
- Condições de conclusão.
- Finalização de perseguições.
- Colisões críticas.

Não substituir testes reais por mocks que ocultem falhas do cenário.

---

## 32. Testes visuais

Capturar evidências reais em runtime.

### Casos obrigatórios

**Casa:**
- Primeira tentativa de sair.
- Retorno para buscar a mochila.
- Chave aparecendo na mesa.
- Folhas inicialmente ausentes.
- Folhas aparecendo após o desaparecimento.
- Acesso à rua e ao Fusca.
- Embarque sem animação de porta.

**Escola:**
- Nova interface dos documentos.
- Comparar com múltipla escolha.
- Concluir sem informações que entreguem a resposta.
- Veículo passando com áudio.

**Biblioteca:**
- Objetos interativos destacados.
- Globo.
- Personagem bloqueado corretamente por estantes.
- Ausência de movimentação sobre paredes.

**Mata:**
- Pegadas ou outros vestígios físicos.
- Interações corretamente posicionadas.
- Progressão da investigação.

**Igreja:**
- Identidade arquitetônica preservada.
- Evidências visíveis.
- Puzzle Livro do Tombo.
- Colisões.
- Encontro com Padre Fábio.

**Casarão e subsolo:**
- Chave física.
- Evidências representadas por objetos.
- Materiais do mecanismo.
- Ausência de interações no vazio.

**Oficina:**
- Fusca movimentando-se sem atravessar estruturas.

---

## 33. Testes de resolução

Conferir o comportamento dos puzzles e interfaces em diferentes dimensões, incluindo:

- 1280 × 720.
- 1920 × 1080.
- Resolução principal configurada no projeto.
- Janela redimensionada, quando suportado.

Verificar especialmente:

- Clipping.
- Sobreposição.
- Escala.
- Fontes.
- Botões.
- Margens.
- Descrições.
- Seleção.
- Elementos que escapam do painel.

---

## 34. Testes de campanha

Percorrer a campanha jogável completa, desde o início até a última fase disponível.

Validar transições e objetivos na ordem real estabelecida pela versão atual do projeto.

Verificar:

- Novo jogo.
- Continuação.
- Reinício.
- Game over.
- Salvamento.
- Carregamento.
- Retorno ao menu.
- Troca de cenas.
- Interações obrigatórias.
- Puzzles.
- Cutscenes.
- Combate.
- Progressão da história.

### Importante

Não considerar a campanha validada com base apenas em testes individuais.

Um teste focado pode aprovar um componente enquanto a transição entre fases permanece quebrada.

---

# PARTE IX — BUILD FINAL E RELATÓRIO

## 35. Build jogável

Após integrar e testar as correções, gerar uma build Windows executável.

### Nome

`Oizlede - Segredo de Varginha.exe`

A pasta da build deve incluir todos os arquivos necessários para a execução independente do editor Unity.

### Entrega

Preparar, quando possível:

- Build jogável.
- Pacote ZIP.
- Changelog.
- Relatório de QA.
- Evidências visuais.
- Relação de commits.
- Pendências remanescentes.

Não entregar somente uma prévia HTML, um relatório ou um conjunto de scripts.

---

## 36. Relatório obrigatório

Ao final, apresentar uma tabela com:

| Campo | Descrição |
|---|---|
| Item | Número da correção |
| Solicitação | Problema original |
| Arquivos | Scripts, assets e cenas alterados |
| Implementação | O que foi realmente modificado |
| Teste | Como a alteração foi validada |
| Evidência | Print, log ou resultado |
| Estado | Concluído, Parcial, Bloqueado ou Não iniciado |

### Regras de classificação

**Concluído:** implementação integrada e validação suficiente executada.

**Parcial:** parte da correção implementada, mas ainda faltam elementos ou testes.

**Bloqueado:** impedimento específico identificado e documentado.

**Não iniciado:** nenhuma implementação realizada.

Não marcar itens como concluídos somente porque existe código relacionado.

---

# PARTE X — CRITÉRIOS DE ACEITAÇÃO

A entrega será considerada satisfatória quando:

1. Os principais bloqueios de progressão estiverem corrigidos.
2. Edelzio não conseguir atravessar paredes ou ocupar áreas inválidas.
3. O Fusca não atravessar obstáculos.
4. A sequência da chave e mochila estiver na ordem solicitada.
5. As folhas aparecerem no momento correto.
6. O embarque funcionar sem animações de porta.
7. Os puzzles estiverem legíveis, compreensíveis e funcionais.
8. O puzzle da escola utilizar as novas regras de Comparar e Concluir.
9. Os objetos investigáveis existirem visualmente nos mapas.
10. A mata apresentar vestígios que despertem curiosidade.
11. A igreja antiga estiver arquitetonicamente reconhecível e preservada.
12. A fuga do ET terminar corretamente ao entrar na igreja.
13. As evidências do Livro do Tombo estiverem acessíveis.
14. O carro musical da escola estiver integrado e com áudio espacial.
15. Os marcadores ciano de objetivos tiverem sido substituídos.
16. Os testes automatizados relevantes estiverem aprovados.
17. A campanha tiver sido percorrida em validação funcional.
18. Houver uma build executável e um relatório fiel dos resultados.

---

# INSTRUÇÃO FINAL AO CODEX

**Execute a implementação real dessas correções, não apenas uma análise.**

Utilize o GDD, todas as capturas fornecidas, o áudio anexado e os arquivos existentes do projeto como referências.

Priorize primeiro a estabilidade, depois a progressão narrativa, a clareza dos puzzles e o acabamento artístico.

Sempre que identificar uma falha compartilhada entre várias fases, corrija sua origem em vez de repetir remendos individuais.

Preserve o trabalho já aprovado e não reduza arbitrariamente o escopo.

Se houver limitações de tempo, ferramentas ou tokens, maximize as correções efetivamente integradas, mantenha o repositório em estado consistente e registre claramente o que ainda não foi concluído.

**A meta final é entregar OIZLEDE: FRAGMENTO PERDIDO como uma experiência investigativa mais intuitiva, visualmente coerente, narrativamente consistente e tecnicamente estável, acompanhada de uma build jogável realmente validada.**