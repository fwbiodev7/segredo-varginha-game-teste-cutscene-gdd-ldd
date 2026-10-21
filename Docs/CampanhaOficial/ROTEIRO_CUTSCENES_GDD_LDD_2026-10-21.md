# Roteiro e plano de produção das cutscenes

Data: 21/10/2026. **Documento de planejamento; não adiciona fases nem muda o jogo por si só.** Contém spoilers de toda a história e dos finais previstos.

**Status: aguardando aprovação do usuário.** Conforme orientação mais recente, o fechamento da entrega atual tem prioridade; a implementação de novas cutscenes será uma etapa posterior.

## Fontes e regra de adaptação

- **GDD original:** [GDD_Usuario.md](Referencias/GDD_Usuario.md), História detalhada, Atos I–VI, Clímax e finais e Gameplay detalhado.
- **LDD original:** [O_Segredo_de_Varginha_LDD.md](Referencias/O_Segredo_de_Varginha_LDD.md), fases 01–25, sistemas 4.9, 4.19 e 4.20 e ordem de produção.
- **Compatibilidade com a entrega:** [campanha vigente de 14 capítulos](CAMPANHA_14_FASES.md) e [abertura adulta aprovada](CHAVE_DESAPARECIDA.md). Estes dois documentos informam onde integrar ou evitar repetição; não acrescentam acontecimentos ao roteiro das fontes originais.

Acontecimentos, personagens, objetos e revelações abaixo vêm do GDD/LDD. Enquadramentos e durações são **propostas de encenação**, não fatos adicionais do universo. Só há falas fechadas quando o texto está nas fontes. Quando elas apresentam apenas o conteúdo de uma conversa, este roteiro indica sua função e deixa o diálogo para redação posterior, sem inventar falas canônicas.

Os IDs `CS` identificam sequências de produção propostas, não IDs de cenas Unity ou flags já existentes. A numeração das fases nesta parte segue o **LDD de 25 fases**, para permitir rastrear cada origem. Não significa restaurar esse escopo na campanha atual de 14 capítulos.

## Divergências que precisam de decisão antes de produzir conteúdo novo

| Assunto | O que dizem as fontes | Tratamento neste roteiro |
|---|---|---|
| Ano de Zé Gomes | GDD usa 1898 na história geral e nos finais, mas menciona 1966 no Ato III. LDD recomenda 1898 e pede confirmação | Usar 1898 como referência de trabalho, mantendo a decisão editorial registrada; não criar dois incidentes para explicar a diferença |
| Horário decifrado | GDD original, Ato III: **23:23**. O GDD normalizado do projeto contém **23:17** | Não alterar o puzzle aprovado. Fechar qual documento deve ser corrigido antes de gravar fala ou desenhar insert com horário |
| Caixa de 1996 | GDD coloca caixa/caderno antes da primeira viagem. LDD reserva outra busca à fase 05, depois da escola | Na campanha atual, conservar a abertura aprovada em 20/10. Usar a fase 05 como referência do puzzle documental, sem uma segunda descoberta idêntica |
| Ordem de Fábio | GDD apresenta diocese antes da mata; LDD 07 leva ao encontro na mata e LDD 08 ao arquivo | Para expansão futura, escolher a ordem antes de montar novas transições. Na versão atual, manter a ordem da campanha vigente |
| Quantidade de atos | GDD organiza seis atos; LDD desdobra a produção em oito | Identificar sempre a fonte. Não renomear capítulos salvos para fazê-los coincidir |
| Renan | LDD 11–12 propõe introdução posterior, em Três Corações; a versão atual já o apresenta na escola | Evitar apresentar o mesmo personagem como desconhecido novamente. As cenas de reparo são planejamento futuro, não requisito da abertura atual |
| Final Verdadeiro | GDD exige todas as pistas opcionais; LDD usa conjunto de pistas e `threshold` ainda não definido | Não inventar quantidade. O gate narrativo permanece pendente de definição para a versão com três finais |
| Escopo de finais | GDD/LDD preveem três; a campanha atual entrega um retorno verdadeiro | Planejar as três variantes sem anunciar as outras duas como implementadas |

## Regras comuns de direção e integração

Manter câmera 2D e identidade dos sprites existentes. Priorizar enquadramento, pose, luz e som; aproximar a câmera não exige redesenhar o rosto. Ampliações de pixel art devem preservar pixels e proporções. Arte adicional deve partir das fichas e frames existentes, com a skill `pixel-art-sprite-sheet-master`, prévias e revisão própria.

Não resolver puzzles durante uma cutscene. O jogador observa, relaciona e age; a tomada confirma a consequência. Não congelar controles enquanto uma ameaça continua causando dano. Entrar em modo cinematográfico deve tratar câmera, input, física, IA e áudio como uma transição coordenada.

Conservar pause, legendas legíveis e movimento reduzido. Nesse modo, substituir tremor, deslocamentos intensos e flashes por enquadramentos estáveis e som moderado. O mesmo acontecimento e a mesma informação precisam continuar compreensíveis.

O LDD 4.9 exige que cutscenes possam ser puladas depois de vistas uma vez. Planejar estado de início e de conclusão, sem duplicar itens ou eventos ao pular. **Este requisito é do plano de produção; não afirma que todas as cenas atuais já possuem esse recurso.** Saves precisam conservar descoberta, puzzle e checkpoint, não apenas a posição do ator.

Por decisão do usuário em 21/10, a voz infantil fica para uma eventual dublagem futura. A sequência atual usa legenda e efeitos. Não produzir voz sintética para preencher essa pendência.

## Primeira missão: roteiro de planos

### CS01 — Reportagens e normalidade de 1996

**Fonte:** GDD Ato I; LDD 01. **Duração proposta:** 10–16 s de apresentação, seguida de exploração livre. **Personagem:** Edelzio criança.

| Plano | Ação / imagem | Som e texto | Saída |
|---|---|---|---|
| 1 | Reportagens estilizadas sobre o avistamento em Varginha | Noticiário fictício conforme LDD; texto jornalístico ainda não fornecido, sem inventar depoimentos reais | Entrar na imagem da TV |
| 2 | Revelar TV CRT na sala e Edelzio no ambiente doméstico | Som da TV e da casa; sem exposição sobre a entidade | Devolver movimento e interação |
| 3, jogável | TV, jornal, PC, desenho e janela podem ser observados | Feedback contextual curto | O evento de energia inicia a próxima sequência |

Não mostrar nave confirmada, acordo, responsáveis pelo selo ou criatura completa.

### CS02 — Clarão e corte da memória

**Fonte:** GDD Ato I; LDD 01, triggers `T02_power_flicker` até `T05_memory_cut`. **Duração proposta:** 12–20 s, sem contar a caminhada do jogador.

| Plano | Ação / imagem | Som e texto | Controle / continuidade |
|---|---|---|---|
| 1 | Energia oscila; a TV perde o sinal | Estática substitui o noticiário | Preservar exploração até a aproximação do quintal |
| 2 | Brilho pela janela aponta a direção | Som da casa perde presença | O jogador segue a pista, sem teletransporte |
| 3 | Quintal; feixe e silhueta indistinta | Suspensão do ambiente antes do clarão | Perda parcial de controle somente no gatilho previsto |
| 4 | Clarão e corte brusco; Edelzio na cama | Corte sonoro | Registrar primeira versão incompleta da memória e salto de 30 anos |

As revisitas posteriores precisam repetir marcos desse enquadramento, acrescentando informação apenas nas etapas previstas.

### CS03 — A chave sobre o balcão

**Fonte:** GDD Ato II e Gameplay 1–2; LDD 02. **Duração proposta:** 3–5 s. **Entrada:** preparação mínima concluída e interação voluntária com o balcão.

| Plano | Ação / imagem | Som / fala | Resultado persistente |
|---|---|---|---|
| 1 | Enquadrar Edelzio e a superfície do balcão; aproximar-se com pose existente | Ambiente doméstico | Ainda não consumir nem mover a chave no save |
| 2 | Mão alcança a superfície; chave é depositada e fica legível | Pequeno contato do objeto, sem música de ameaça reveladora | Confirmar chave no balcão |
| 3 | Retomar enquadramento de exploração | Objetivo de buscar a mochila | Devolver controle; reload mantém a chave visível |

É **Edelzio** quem coloca a chave **do Fusca**. O carro não executa a ação. Não antecipar reação a um desaparecimento que ainda não aconteceu.

### CS04 — O desaparecimento percebido

**Fonte:** GDD Ato II e Gameplay 2; LDD 02. **Duração proposta:** 5–9 s. **Entrada:** jogador foi buscar a mochila após deixar a chave.

| Plano | Ação / imagem | Som / fala | Controle / continuidade |
|---|---|---|---|
| 1 | Edelzio pega a mochila no quarto | Ambiente da casa; nenhuma voz explicativa | A chave sai do campo de observação |
| 2 | Retorno ao balcão: mesmo copo e superfície, espaço agora vazio | Valorizar a ausência; não mostrar mão, criatura ou teleporte responsável | Persistir desaparecimento |
| 3 | Suspender brevemente no balcão vazio | Sem explicação sobre quem moveu o objeto | Devolver investigação com objetivo claro |

A procura continua jogável. As três inspeções e sua interpretação conduzem ao som sob a cama. Não transformar a investigação numa montagem que coleta pistas sozinha.

### CS05 — Caixa, caderno e escrita recente

**Fonte:** GDD Ato II; LDD 02 e 05. **Duração proposta:** 8–14 s em duas tomadas separadas pelo puzzle.

1. Após a investigação, arranhões apontam para debaixo da cama. Edelzio se abaixa usando a pose já estabelecida. Mostrar a caixa e abrir sua tampa; a chave e o caderno de 1996 estão dentro. Não revelar o agente do deslocamento.
2. Insert legível da escrita infantil: **“Eu sei que vi alguma coisa.”** Pausa suficiente para leitura. Registrar descoberta e entregar o documento à interface jogável.
3. **O jogador recompõe o documento.** Só depois da solução, o insert revela **“ELA AINDA ESTÁ AQUI.”** Não apresentar a ordem das peças como instrução cinematográfica.
4. Retomar Edelzio no mesmo lugar, com inventário e objetivo de saída consistentes. Não exigir que saves antigos com evidências adquiridas repitam toda a descoberta.

### CS06 — O surto do Fusca

**Fonte:** GDD Ato II, “O Surto do Fusca”, e Gameplay 3; LDD 03. **Duração proposta:** 25–40 s. O LDD descreve direção/diagnóstico jogável; a versão aprovada mantém esse acontecimento como transição cinematográfica, sem reintroduzir inspeções obrigatórias.

| Ordem | Enquadramento / ação | Áudio e informação | Regra de continuidade |
|---|---|---|---|
| 1 | Exterior: embarque, porta e partida com Fusca original | Porta, ignição e motor | Chave e mochila já adquiridas |
| 2 | Rua: viagem habitual | Motor e trânsito normais | Estabelecer normalidade antes da interferência |
| 3 | Rádio: sinal falha | Estática, sem revelar emissor | Não mostrar entidade no rádio |
| 4 | Faróis/painel; velocímetro se comporta de modo impossível | Interferência elétrica | Sem minijogo de conserto que explique a anomalia |
| 5 | Veículo perde força e para | Motor morre | Não matar nem punir o jogador pela pane roteirizada |
| 6 | Rádio: legenda **“Não deixa ela sair.”** | Efeitos atuais; voz infantil reservada à dublagem futura | Não esclarecer o referente de “ela” |
| 7 | Ignição quente; Edelzio afasta a mão, sem redesenhar sua identidade | Silêncio sustentado | Não fabricar ferimento ou consequência inexistente no GDD |
| 8 | Motor volta sem intervenção explicativa | Partida repentina e retorno do ambiente | Registrar anomalia vista |
| 9 | Fachada da Industrial; desembarque | Ambiente escolar | Restaurar câmera, física, visibilidade e input; salvar chegada |

## Continuação: sequências por acontecimento do LDD

Cada duração abaixo é uma estimativa de edição. A exploração, os diálogos com escolhas, puzzles, perseguições e combates continuam jogáveis e não entram nessa conta.

| ID / fonte | Gatilho e planos propostos | Texto / revelação permitida | Entrega ao jogador |
|---|---|---|---|
| **CS07 — Industrial**; LDD 04, GDD Gameplay 4; 6–10 s | Fachada → entrada/pátio → sala familiar. Mostrar escola e alunos em atividades normais | Sem fala nova fechada; apresentar o cotidiano, não o cerco antecipado | Controle para conhecer NPCs e cumprir o expediente |
| **CS08 — A mensagem na fotografia**; GDD Ato III/Gameplay 5, LDD 05–06; 6–10 s | Após solução no notebook, destacar fotografia e resultado; depois destino ligado à diocese | Conteúdo: horário e localização. Horário é divergência editorial registrada acima | Salvar informação decifrada e objetivo seguinte; não executar o algoritmo pelo jogador |
| **CS09 — Fragmentos do mapa**; LDD 06; 5–8 s | Puzzle concluído → fragmentos agora alinhados → região da mata identificada | Mostrar apenas a descoberta cartográfica permitida | Liberar destino e preservar pistas opcionais coletadas |
| **CS10 — Manifestação na mata**; LDD 07, GDD Ato IV; 8–14 s | Marcas/alterações ambientais já observadas → primeira manifestação parcial → abertura da rota de fuga | A forma não é confirmada como a entidade inteira | Devolver controle antes da perseguição; checkpoint após fuga |
| **CS11 — Fábio e a âncora**; LDD 08, GDD Ato III; 15–25 s | Encontro evasivo → arquivos jogáveis → compartimento aberto pelo puzzle → registro de Edelzio → reação de Fábio | Texto literal do LDD: **“RECEPTÁCULO: EDELZIO / ESTADO DA ÂNCORA: ESTÁVEL”**. Conteúdo da explicação vem do GDD, sem inventar fala | Escolha de confronto/pedido de explicação pode alterar confiança, nunca bloquear a história |
| **CS12 — A amostra de Ouzana**; LDD 09, GDD Ato IV; 10–16 s | Evidências apresentadas → teste realizado pelo jogador → reação anômala → Ouzana aceita ajudar | Explicar a função do reagente, sem antecipar exploração humana da entidade | Entregar reagente e tutorial contextual; não repetir uma comparação já concluída |
| **CS13 — Fusca marcado**; LDD 10, GDD Ato IV; 8–12 s | Aplicação pelo jogador → marcas reveladas na lataria → relação com padrão do mapa/selo | GDD descreve cicatrizes do tecido da realidade; não acrescentar símbolos com significados novos | Registrar revelação e condição do carro; preservar estacionamento e partida |
| **CS14 — Falha nos arquivos**; LDD 11; 8–12 s | Corrupção aparece → jogador tenta salvar um arquivo → falha de recuperação completa → viagem | Nenhum documento essencial é perdido definitivamente; Renan recuperará os restantes | Controle para fuga/viagem; autosave antes da falha e depois do encontro |
| **CS15 — Reparo com Renan**; LDD 12; 10–18 s | Oficina → diagnóstico jogável → restauração → evidência de alteração não convencional | Sem diálogo literal nas fontes. Não inventar culpado ou tecnologia explicativa | Recuperar investigação e liberar bancada; na campanha atual seria reencontro, não apresentação como desconhecido |
| **CS16 — Sinal na Industrial**; LDD 13; 10–16 s | Reportagem → contato falha → chegada a perímetro vazio → indícios dos reféns | Ameaça e urgência, sem resumir o resgate antes de acontecer | Controle no portão; preparação e checkpoint previstos no LDD |
| **CS17 — O cerco**; LDD 14; 6–10 s de entrada e 6–10 s de conclusão | Repetir enquadramento da escola normal, agora corrompida. Após resgate jogável, mostrar os alunos libertos | Não substituir combate/puzzle por montagem; manifestação incompleta não é a entidade final | Checkpoint de cada grupo e retorno seguro após confronto |
| **CS18 — 3º Sistemas**; LDD 15; 8–14 s | Consequências do resgate → alunos decidem ajudar → preparação da arena | Não inventar novos nomes, personalidades ou poderes: o próprio LDD deixa essa definição à equipe | Tutorial de party jogável; registrar desbloqueio só após concluído |
| **CS19 — Registros de Zé Gomes**; LDD 16, GDD Gameplay 10; 8–14 s | Após cruzamento de evidências, destacar relações entre nomes, datas e propriedade | 1898 é referência de trabalho. Documentos sugerem colaboradores; não apresentar um vilão vivo novo | Casarão localizado e suspeita sobre ligação com a Igreja |
| **CS20 — A fotografia do casarão**; LDD 17; 12–20 s | Descoberta final após exploração → foto/registro da Igreja com Zé Gomes → Edelzio → Fábio | Fala literal do LDD: **“Você sabia disso?”**. Resposta: **silêncio de Fábio** | Salvar descoberta; abrir objetivo de buscar registros originais |
| **CS21 — O segredo da Igreja**; LDD 18, GDD Atos III/VI; 15–25 s | Linha temporal resolvida → registro original → reação entre Edelzio e Fábio | Mostrar função de contenção do selo e a informação obtida. Não redigir confissão detalhada sem texto aprovado | Escolha sobre ajuda de Fábio, com continuidade garantida; conservar truth clue |
| **CS22 — Segunda memória**; LDD 19, GDD Gameplay 9; 12–20 s de inserts | Repetir a casa de CS02; portas antes fechadas se abrem; criatura ferida recua; sugerir homens/vozes conforme LDD | A criatura demonstra medo; memória interrompe **antes do acordo**. Não nomear envolvidos não identificados pelas fontes | Controle nos flashes corretos; sem combate e sem alterar o passado |
| **CS23 — Varginha em colapso**; LDD 20; 10–18 s repartidos pela travessia | Apagão urbano → rádio → ruas deformadas → perseguição espacial do Fusca | Som e alterações mostram escala da crise. Não atribuir mortes ou destruição específica não prevista | Fusca, resgates opcionais e deslocamento continuam jogáveis; checkpoint por zona |
| **CS24 — A descida**; LDD 21, GDD Ato V; três inserts de 3–5 s | Instalação humana → transição híbrida → caverna quase orgânica | Som abafado, objetos humanos/religiosos e memória infantil; não acrescentar lore verbal | Devolver controle em cada camada. Separações da equipe precisam ter função narrativa/jogável identificável |
| **CS25 — O último documento**; LDD 22, GDD Final Verdadeiro; 12–20 s | Gate de evidências satisfeito → puzzle resolvido → registro original legível | Entidade explorada/aprisionada e desejo de retorno. Quantidade de pistas é decisão pendente, não número inventado | Registrar acesso ao final verdadeiro. Sem evidências suficientes, continuar a história sem fabricar solução |
| **CS26 — A verdade de 1996**; LDD 23, GDD Ato VI; 20–35 s de inserts | Mesmos marcos de CS02 → criatura ferida e assustada → contato com Edelzio → processo da âncora → memória reprimida | GDD: houve acordo e escolha consciente de Edelzio criança. Não inventar termos contratuais, fala da criatura ou nomes dos presentes | Criança é controlável em momentos previstos, mas o jogador não muda o passado; checkpoint antes e no contato |
| **CS27 — O confronto final**; LDD 24; 8–12 s de entrada e 8–12 s de saída | Câmara → ameaça das manifestações → controle no encontro. Após estabilização jogável, acesso ao núcleo | A entidade principal não é um monstro a executar. Antagonismo mecânico: manifestações e prisão em colapso | Confronto combina sistemas; vitória não escolhe o destino pelo jogador |

### CS28A — Sacrifício

**Fonte:** GDD Final 1; LDD 25. **Duração proposta:** 25–40 s.

Escolha confirmada → Edelzio permanece como âncora → entidade contida → Fábio e Ouzana escapam → passagem de tempo → outra criança encontra o caderno. Insert literal: **“Se você está lendo isso, significa que ela ainda está dormindo.”** Encerrar e salvar o final. Não substituir a criança por personagem conhecido sem previsão nas fontes.

### CS28B — Libertação sem conhecimento

**Fonte:** GDD Final 2; LDD 25. **Duração proposta:** 20–35 s.

Ruptura do selo → travessia descontrolada → colapso sobrenatural e apagão de Varginha → Fusca abandonado → rádio. Fala literal: **“Não estamos sozinhos.”** Encerrar sem acrescentar sobreviventes, mortes ou responsáveis específicos que as fontes não definem.

### CS28C — Quebrar o ciclo

**Fonte:** GDD Final 3; LDD 25. **Duração proposta:** 35–55 s, além do puzzle final jogável.

1. Com o conhecimento necessário, o jogador alinha configuração de 1898, anomalia de 1996 e estabilização de Ouzana/Renan. O grupo contém as manifestações durante a ação.
   O acesso exige as evidências, as memórias completas e o documento final previstos no LDD; a quantidade de pistas ainda precisa da decisão editorial registrada acima.
2. Passagem original estabilizada; entidade reconhece Edelzio e retorna. O encerramento da ligação o liberta da função de âncora. Na versão atual, **conservar a ligação até a travessia terminar**, como já implementado; não transformar o plano em uma mudança silenciosa dessa ordem.
3. Epílogo do LDD: Ouzana pesquisa resíduos; Renan estuda a tecnologia; alunos voltam à Industrial; Fábio registra a verdade; Edelzio retorna ao trabalho. Enquadramentos breves, sem falas novas fechadas.
4. Último plano previsto no LDD: câmera dentro do Fusca → rádio liga sozinho → nova voz → símbolo diferente → corte. **O texto da voz e o desenho/significado do símbolo não foram definidos.** Reservar placeholders de produção, sem escrever uma revelação adicional. Esse gancho não deve ser anunciado como presente na build atual.

## Matriz para integrar à campanha vigente

| Capítulo atual / IDs preservados | Sequências de referência | Decisão de integração |
|---|---|---|
| Infância / 1 | CS01–02 | Reutilizar prólogo e seus marcos nas memórias |
| Abertura adulta / 2 | CS03–06 e parte documental de CS08 | Conservar preparação, pistas, caixa e viagem aprovadas; não repetir descoberta depois da escola |
| Escola / 4 | CS07–08 | Reutilizar Renan e o puzzle existentes; resolver divergência de horário antes de nova arte/fala |
| Fragmentos / 6 | CS09 | Confirmação curta após o mapa jogável |
| Mata / 7 | CS10 e encontro de CS11 | Não retirar controle durante perseguição |
| Âncora / 8 | CS11 | Preservar descoberta pelo Livro do Tombo |
| Marcas no Fusca / 9–10 | CS12–13 | Não restaurar ordenações/inspeções que foram removidas do caminho obrigatório |
| Arquivos alterados / 11 | Conteúdo compatível de CS14–15 | A campanha atual já conhece Renan; não exigir viagem inédita para reparar o notebook |
| Casarão / 12–13 | CS19–20 | Reutilizar áreas e fluxo atuais; não acrescentar andares ou gates apenas porque o LDD os planeja |
| Fábio / 14 | CS21 | Preservar a ordem de revelações já aprovada |
| Memória / 15 | CS22 | Não concluir o acordo antes da memória completa |
| Criatura e acordo / 18 | CS24–26, somente conteúdo compatível | Não recriar os capítulos removidos da descida ou das câmaras |
| Batalha / 20 | CS27 | Distinguir manifestação combatida da criatura ferida |
| Retorno / 21 | CS28C, trecho compatível | Preservar calibração → abrir → atravessar → encerrar ligação → Industrial |
| Sem equivalente ativo | CS16–18, CS23, CS28A/B e trechos expandidos | Backlog narrativo. Exigem escopo próprio antes de implementação; não estão incluídos como novos capítulos nesta entrega |

## Ordem de produção e critérios de revisão

1. **Abertura e vertical slice:** revisar CS01–06 e chegada à escola. Base: milestone 1 do LDD. Validar leitura, controle devolvido, primeira partida, pausa e reload; dublagem fica fora da entrega atual.
2. **Investigação:** CS08–15 e CS19–22. Base: milestones 2 e 4. Antes de produzir, fechar cronologia/horário; conferir o que cabe na campanha de 14 capítulos e aprovar textos que não existem literalmente nas fontes.
3. **Confrontos e expansão:** CS16–18, CS23–24 e CS27. Base: milestones 3 e 4. Exigem decisão de escopo; não criar novas fases automaticamente a partir deste roteiro.
4. **Memória completa e finais:** CS25–28. Base: milestone 5. Definir gate do conhecimento, variantes e epílogos; conservar a distinção entre retorno seguro e libertação descontrolada.
5. **Polimento:** acessibilidade, legendas, áudio, desempenho, pulo após primeira exibição, migração e QA. Base: milestone 6.

Para cada sequência que entrar em produção, anexar storyboard com os sprites existentes; lista de falas aprovadas e IDs; áudio previsto; gatilho e condição de conclusão; checkpoint; comportamento ao pausar, pular, recarregar e usar movimento reduzido; captura em gameplay e resultado do teste. Separar estado “planejado”, “implementado”, “testado” e “aprovado pela equipe”.

## Revisões pendentes do roteiro

- Decisão editorial sobre 1898/1966 e 23:23/23:17.
- Compatibilização formal das ordens GDD/LDD com os 14 capítulos vigentes, sem desfazer a abertura atual.
- Texto completo dos diálogos apenas resumidos pelas fontes, sem antecipar revelações.
- Condição exata do final verdadeiro na futura versão com ramificações.
- Conteúdo da última voz e do símbolo do gancho, caso a equipe decida produzi-lo.
- Aprovação de storyboard, duração e novas artes; gravação/dublagem em etapa futura.

Essas decisões não impedem usar as cutscenes já integradas na build de hoje. São limites explícitos para a próxima produção baseada nas fontes.
