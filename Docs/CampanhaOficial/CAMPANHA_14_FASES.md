# Campanha experimental — 14 fases em 5 atos

Atualização de 7 de outubro de 2026. Esta é a sequência vigente; os planos e relatórios anteriores registram a campanha de 21 fases.

| Ato | Fase | Objetivo principal | IDs internos preservados |
| --- | --- | --- | --- |
| 1 — A lembrança volta | 1. O Caso de Varginha | Abertura, casa infantil, brilho, explosão e perda de memória | 1 |
| 1 | 2. A Caixa Esquecida | Investigar caixa, fotografia e páginas; obter primeiro fragmento | 2 |
| 1 | 3. Entre Aulas e Pistas | Comparar foto, vulto e horário no notebook de Renan; segundo fragmento | 4; viagem antiga 3 vira transição curta e código antigo 5 ocorre na mesma sala |
| 2 — O caminho da capela | 4. Fragmentos | Reunir documentos e terceiro fragmento na biblioteca da Industrial; montar mapa | 6 |
| 2 | 5. A Mata | Seguir mapa, sobreviver à perseguição e encontrar Fábio | 7 |
| 2 | 6. A Âncora | Livro do Tombo, compartimento e registro de Edelzio | 8 |
| 3 — Evidências que mudam | 7. Marcas no Fusca | Comparar leituras com Ouzana, obter reagente e revelar marcas numa aplicação | 9 e 10, laboratório e oficina como áreas do mesmo capítulo |
| 3 | 8. Arquivos Alterados | Comparar papel e arquivo digital com Renan; descobrir acesso lateral | 11 |
| 3 | 9. O Casarão de 1898 | Planta, passagem, chave e registros; jardim, térreo e porão conectados | 12, 12 área 1 e 13 |
| 4 — A noite esquecida | 10. O Que Fábio Escondeu | Provas, página ocultada e continuidade do selo vivo | 14 |
| 4 | 11. Antes da Explosão | Lembrança breve na casa de 1996 | 15 |
| 5 — O verdadeiro segredo | 12. A Criatura e o Acordo | Investigar a criatura ferida e reconstruir o acordo; memória completa breve | 18; evidências do antigo 19 integradas, sem repetir exploração da casa |
| 5 | 13. A Batalha da Manifestação | Preparar circuitos, dissipar manifestação e calibrar retorno | 20 |
| 5 | 14. O Retorno | Abrir passagem, aguardar ET atravessar, encerrar ligação e voltar à Industrial | 21 e 21 área 1 |

## Alterações no percurso

Pia, café, preparação do notebook e coleta isolada da mochila são opcionais. Investigar a caixa entrega os equipamentos necessários; organizar as páginas continua sendo a primeira investigação. A viagem dura poucos segundos, conserva Fusca, pane e voz no rádio e leva à Industrial sem três inspeções obrigatórias. Fotografia, vulto, código e confirmação de Renan acontecem no mesmo mapa.

Ouzana apresenta a comparação do controle e resíduo num diálogo curto. O reagente revela as três marcas com uma carga; a ordenação de amostras, segunda ordenação das marcas e pista de teste deixam de bloquear a saída. O estacionamento e a partida cinematográfica do Fusca continuam. Na fase do casarão, a passagem revelada e a chave permitem descer diretamente do térreo ao porão; a escada permite voltar. A biblioteca, mapa, perseguição e checkpoint da mata foram conservados.

A antiga descida com puzzle de água/alimentação sai da sequência. A fase das Câmaras do Selo foi removida completamente, incluindo suas três cenas e mapa exclusivo. A investigação da criatura inclui página ocultada, registros, caderno e memória completa do acordo, preparando a batalha sem outra exploração longa.

## Retorno verdadeiro

A manifestação combatida e a criatura ferida permanecem distintas. A vitória é salva imediatamente e não cria outro chefe ao recarregar. Depois da vitória, o mecanismo central exige calibração. Só então a passagem pode abrir. Edelzio permanece como selo vivo até a criatura atravessar. O encerramento da ligação precisa terminar antes de liberar a Industrial. Renan, caderno, Fusca e créditos concluem a única conclusão disponível.

A equipe mantém três alunos e um apoio entre Renan, Ouzana e Fábio, habilidades e recargas, ataques com preparação/recuperação, limite de cinco filhotes, entrada/dissipação cinematográficas, pausa, movimento reduzido e restauração da câmera.

## Saves e cenas

`CampaignSequence` separa a fase exibida dos IDs serializados. Campos `phase`, `positionPhase`, índices `phase - 11`, mapas, arrays de documentos/puzzles, nomes de cenas e referências do combate conservam seus IDs anteriores. São 14 capítulos e várias áreas; a quantidade de cenas habilitadas pode ser maior que 14.

O save `CampaignStory2026.json` migra das versões 1/2 para 3 uma única vez, gravando atomicamente. IDs removidos 3/5 passam a 4, 16/17/19 a 18. Apenas posições das cenas substituídas são descartadas; os demais controles de posição e checkpoint continuam válidos. Documentos, fragmentos, puzzles, reagente, cargas, equipe, vitória e etapas do retorno permanecem. Conclusões antigas restauram suas evidências necessárias, evitando que a retirada de tarefas apague progresso.

Saves antigos que já iniciaram a travessia conservam autorização e etapa, com reguladores normalizados para a configuração estável; uma calibração antiga é reconhecida quando a vitória e o registro de calibração ou conclusão do painel estão salvos, normalizando os reguladores antigos. Isso vale apenas durante a migração de saves da versão 1. Uma vitória sem calibração verificável exige calibrar, sem repetir o combate. Girar um regulador invalida a confirmação anterior. O acordo fundido conserva os puzzles antigos concluídos e exige concluir o procedimento ainda pendente. A área retomada é limitada às áreas existentes na fase. O caderno reúne evidências dos capítulos atuais; os slots antigos continuam no save para compatibilidade.

As três cenas do antigo ID 17 e sua imagem exclusiva foram excluídas. As cenas antigas 3, 5, 16 e 19 são preservadas no projeto e desativadas nos Build Settings. O comando **Varginha → Campanha → Aplicar sequência de 14 fases** reaplica a lista, também usada pelos construtores existentes. Não houve conversão para Tilemaps nem reconstrução dos assets ilustrados.

Veja [relatório de execução](RELATORIO_REVISAO_20261007.md) para resultados confirmados e limitações.
