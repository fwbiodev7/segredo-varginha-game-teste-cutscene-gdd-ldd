# Retomada e plano de implementação — 26/10/2026

Registro da análise inicial da sessão de 23/10/2026, encerrada com o commit `f5a7f0f9cf4b2c2a047e7e86196ebbd89f315ec9`. O diagnóstico abaixo é um retrato daquele ponto; a implementação posterior e seus resultados estão registrados em [RENDERIZACAO_SPRITES_2026-10-26.md](RENDERIZACAO_SPRITES_2026-10-26.md).

## Base da análise

- Histórico da conversa que terminou com a publicação do checkpoint parcial de 23/10.
- `output/revisao-mestre-2026-10-23/PENDENCIAS.md` e `CONTINUIDADE.md`.
- XML de testes da revisão mestre e relatório da revisão das falas.
- Código atual dos controladores do Fusca, física compartilhada e testes de contato.
- Configuração atual das cenas da build, histórico Git e alterações locais.

As entradas mais recentes de CONTINUIDADE.md prevalecem sobre diagnósticos iniciais do mesmo arquivo. Há aprovação registrada em 23/10 para as 15 correções originais e seus ajustes/testes necessários. Este pedido é de análise e plano; não inicia a implementação.

## O que foi feito

O checklist tem 19 itens: **11 concluídos, sete parciais e um não iniciado**. Entre as 15 correções originais, oito foram concluídas e sete ficaram parciais. Os outros três concluídos são o relatório compacto e os dois novos áudios.

| Item | Entrega concluída no recorte registrado |
| --- | --- |
| 03 | Globo escolar substituído de forma localizada, preservando biblioteca, escala e investigação. |
| 05 | Quatro poses de Edelzio dirigindo conferidas no Player; mãos, volante e painel alinhados. |
| 06 | Chegada à escola pelo caminho pavimentado, com percurso e captura verificados. |
| 07 | Ordem lembrança na saída → mochila → investigação da chave → liberação da porta, com compatibilidade de save. |
| 08 | Investigação da chave orientada por pistas observáveis, incluindo o rastro de papel até a cama. |
| 09 | Trilhas com seleção A/B direta em qualquer trecho, conferência única e preservação das escolhas após erro. |
| 14 | Sons de café e higiene sincronizados com o contato da animação; pausa e interrupções verificadas. |
| 15 | Soco com cliques rápidos corrigido no núcleo e na camada visual; seis poses preservadas a 10 FPS. |
| 16 | Relatório ABNT reduzido para 12 páginas, em PDF e DOCX, com revisão visual registrada. |
| 17 | Nova música do menu integrada, com volume, repetição, fade e parada. |
| 18 | Novo ambiente sombrio da igreja integrado, preservando o sino e as transições. |

Outras entregas da sessão:

- Quatorze falas adultas aprovadas aplicadas à casa/escola; frase infantil preservada. PDF definitivo e pacote para gravação preparados. Isso não comprova recebimento de dublagem humana.
- Build 0.4.8-beta gerada com zero erros e um aviso, antes da conclusão da revisão mestre. Não é a candidata final das alterações posteriores.
- Daily de 23/10 atualizada e verificada; seu conteúdo ainda não incorpora o fechamento da revisão mestre.
- Checkpoint parcial e lista de pendências publicados no GitHub, com parada solicitada perto de 5% de limite restante.

## Pendências confirmadas

| Itens | Estado | Problema ou validação faltante |
| --- | --- | --- |
| 12–13 | Parcial | Oficina e pista têm recortes aprovados; estrada legada continua sem avançar após ré no limite inferior. O ajuste do spawn não resolveu. A estrada legada está fora das 19 cenas habilitadas da build; não adicioná-la à campanha apenas para satisfazer o teste. |
| 02 | Parcial | Postura e ocultação das pernas melhoraram, mas permanece a emenda retangular no piso do notebook. Falta o acabamento simples e suave pedido pelo usuário. |
| 01 | Parcial | Falta auditoria de contatos, colisores, recortes e profundidade dos móveis. As capturas dos 18 ambientes de gameplay cobrem enquadramento/legibilidade na entrada, não todos os contatos. A build tem 19 cenas contando o menu. |
| 04 | Parcial | Recuperação repetida/cancelamento do glitch passou em testes; falta comparar visualmente antes, pico e depois com a ocorrência original. |
| 10 | Parcial | Livro do Tombo tem fragmentos, evidências, save e controles implementados/testados; falta revisão visual do painel no Player. |
| 11 | Parcial | CONTINUAR discreto e proteção contra avanço duplicado passaram em recorte; faltam apresentação e demais transições no Player. |
| 19 | Não iniciado | Roteiro completo da apresentação de 30/10/2026, solicitado para depois do fechamento e validação da revisão. |

Também falta a regressão e a build final que consolidem as correções restantes.

## Plano de execução de hoje, por prioridade

### 1. Preparar uma base segura de QA

Preservar alterações locais, saves, arte e arquivos de recuperação. Conferir a disponibilidade da cópia isolada de QA e sincronizar o código atual antes dos testes. Inspecionar o erro histórico de runner sem presumir que ainda ocorre: o código atual já recebeu proteção contra Play Mode/cena suja.

**Pronto quando:** versão testada e ambiente isolado estiverem identificados, sem interferência na sessão Unity do usuário.

### 2. Fechar o diagnóstico e corrigir a estrada do Fusca — itens 12–13

Reproduzir o caso existente e registrar entrada, estado, título/transição, posição, velocidade e contatos do BoxCast. Distinguir bloqueio de input/estado de bloqueio geométrico antes de alterar a física. Corrigir a causa mantendo aceleração, inércia, freio/ré e limites sólidos.

**Arquivos principais:** `VarginhaCampaignDrive.cs`, `CampaignVehiclePhysics.cs`, `CampaignWorkshopVehicle.cs` e `CampaignVehicleContactTests.cs`.

**Pronto quando:** teste de afastamento após ré passar sem enfraquecer a asserção; aceleração, desaceleração, frenagem, direção e limites funcionarem; oficina, estacionamento, pista e reload permanecerem aprovados. Exercitar estrada legada em cena isolada, mantendo a seleção atual da campanha.

### 3. Finalizar o canto do notebook — item 02

Inspecionar o recorte atual, simplificar as bordas e eliminar a emenda de piso preservando os pixels originais. Conferir cadeira, mesa, personagem e colisores em conjunto, com aproximação, uso e retorno.

**Arquivos principais:** `CampaignSeatingLayers.cs`, `CampaignTeamEdelzio.cs`, recursos de `HouseFurniture` e preparação visual existente em `output/revisao-mestre-2026-10-23/prepare-office.py`, conforme a causa confirmada.

**Pronto quando:** capturas reais de aproximação, personagem sentado e saída mostrarem piso contínuo, sem pés residuais, cadeira duplicada ou sobreposição incorreta.

### 4. Concluir revisão visual e de interações — itens 01, 04, 10–11

- Auditar contatos dos móveis e profundidade por ambiente, começando pelos trechos alterados.
- Comparar o glitch antes/durante/depois, inclusive repetição, cancelamento e troca de área.
- Revisar Livro do Tombo, trilhas e CONTINUAR no Player, com teclado e controle, save/reload, erro/correção e prevenção de recompensa/avanço duplicados.

**Arquivos principais:** `CampaignAtmosphereDirector.cs`, `CampaignAtmosphereLayer.cs`, `CampaignCinematics.cs`, `CampaignBookPuzzle.cs`, `CampaignPuzzleDesign.cs`, `CampaignTrailPuzzle.cs` e controladores de progressão envolvidos.

**Pronto quando:** cada item tiver evidência visual e funcional específica; capturas de entrada não substituem auditoria dos móveis. Trilhas já concluídas recebem regressão integrada, sem reimplementação.

### 5. Consolidar regressão e candidata final

Rodar os testes dirigidos afetados e a suíte completa sobre o código final. Executar os fluxos relevantes no Player e gerar uma candidata em pasta própria. Usar `Tools/AgentOutput/compact.mjs` para testes/builds extensos e investigar falhas nos logs preservados.

**Pronto quando:** não houver falhas pendentes no escopo, a build estiver concluída e os fluxos modificados tiverem sido conferidos nela. Registrar versão, avisos, arquivos/cenas afetados e limitações; não somar testes repetidos como casos únicos.

### 6. Atualizar registros e preparar a apresentação de 30/10

Atualizar PENDENCIAS.md e CONTINUIDADE.md com o resultado verificado. Produzir o roteiro completo desde a ideia/protótipo, com linguagem natural, marcações de slides/demo, duração sugerida, evolução, narrativa, tecnologia, audiovisual, testes, dificuldades e próximos passos. Atualizar o conteúdo da apresentação com o estado final confirmado, preservando sua arte e estrutura.

**Pronto quando:** roteiro e apresentação distinguirem claramente entregas, resultados de teste e trabalho futuro. Se o fechamento técnico não terminar hoje, registrar o ponto exato de retomada e manter o roteiro final como dependente desse fechamento.

## Pendências externas e herdadas a reconciliar

- Recebimento/seleção/edição e integração da dublagem humana: não há comprovação nas evidências consultadas de que os takes foram recebidos. A legenda ED03 ainda não dispara a gravação automaticamente, conforme o mapa técnico da sessão.
- Reconciliar as 98 propostas do roteiro antigo com as 14 falas adultas já aprovadas/aplicadas, evitando reapresentar textos aprovados como pendentes.
- Playtest com outra pessoa, mixagem em fones/caixas e desempenho em outro PC continuam sem conclusão comprovada.
- Conferir o catálogo para identificar exatamente quais efeitos adicionais ainda faltam; o backlog antigo cita água/café, passos e interface, mas sons de café/higiene receberam alterações posteriores.

Esses pontos dependem de materiais, feedback ou equipamento e devem ser acompanhados sem inventar conclusão.

## Evidências históricas verificadas nesta análise

| Evidência | Resultado | Limite de validade |
| --- | --- | --- |
| `full-editmode.xml` | 412/412, zero falhas/ignorados | Anterior às alterações finais de veículo e OfficeTorso. |
| `visual-actions-playmode.xml` | 16/16, zero falhas/ignorados | Recorte de ações, combate e objetos. |
| `polish-player.xml` | 13/14 | Um caso falhou ao tentar carregar a cena legada ausente da build; fixture corrigido depois. |
| `checkpoint-playmode.xml` | 4/5 | Estrada falhou; assento, recuperação repetida de glitch e oficina/pista passaram no recorte. |
| `road-checkpoint.xml` | 0/1 | Esperava posição Y maior que 0,2; permaneceu 0 após ré e aceleração. |
| Revisão das falas | 29/29 EditMode e 3/3 Player na rodada final | Anterior às alterações posteriores da revisão mestre. |

Na análise inicial, nenhum teste ou build havia sido executado em 26/10 e somente este plano tinha sido criado. Consulte o registro de implementação para os testes executados depois desse retrato.
