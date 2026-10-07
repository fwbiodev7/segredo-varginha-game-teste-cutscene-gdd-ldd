# Relatório curto — atualização de 7 de outubro de 2026

## Resultado

Campanha organizada em **15 fases e 5 atos**, com IDs internos preservados e migração de save versão 1 → 2. O [guia de puzzles](GUIA_COMPLETO_PUZZLES.md) foi refeito para a sequência vigente. As [referências pesquisadas e melhorias gerais](REFERENCIAS_E_MELHORIAS_20261007.md) foram apresentadas antes da implementação.

- Antigas 3–5: fotografia e horário na mesma visita à Industrial; viagem e pane viram transição curta. Rotina da casa é opcional.
- Antigas 9–10: análise breve com Ouzana, estacionamento preservado e uma aplicação de reagente. Ordenações repetidas e pista de teste deixam de bloquear a saída.
- Antigas 12–13: jardim, térreo e porão conectados na mesma investigação.
- Antigas 16–17: descida direta e desafio central dos reguladores, com leituras no diagrama.
- Antigas 18–19: criatura ferida e acordo reunidos, com memória breve e evidências acessíveis.
- Antigas 20–21: batalha preservada, calibração posterior obrigatória, travessia antes de encerrar o selo, Industrial e créditos. Chefe e entidade ferida continuam distintos.

Os mapas ilustrados, Collider2D, fachada da Industrial e assets existentes foram mantidos. As cenas retiradas da sequência continuam no projeto e estão desativadas nos Build Settings.

## Bugs reproduzidos e correções

| Evidência anterior | Correção e verificação |
| --- | --- |
| Teste `ReturnCannotOpenBeforeVictoryAndCalibrationOrSkipRelease`: o mecanismo avançava para etapa 1 sem vitória/calibração. | Abertura exige vitória, circuitos preparados e calibração estável. Interações durante sequências não pulam o encerramento. |
| Testes de apresentação: sprite equipado reutilizado com pivô diferente após carregar a campanha. | Cache inclui pivô, tamanho e escala; sprite equipado preserva a geometria do corpo. |
| Teste `DifficultyOptionsAffectCampaignBoss`: Fácil e Médio tinham a mesma vida, 650. | Chefe respeita a dificuldade existente; filhotes herdam os valores da tentativa. Médio conserva 650 e o limite de cinco filhotes. |

Também foram reforçadas a retomada de áreas válidas, a conclusão do acordo fundido e a invalidação da calibração após girar reguladores. Dicas opcionais e leitura sem limite dos close-ups foram adicionadas; o caderno conserva evidências entre fases.

## Testes realmente executados

Unity **6000.6.0f1**, renderização **Built-in**. Aplicação da sequência executada no Editor, com confirmação `CAMPAIGN_SEQUENCE_READY=15`.

| Rodada | Total | Aprovados | Falhas |
| --- | ---: | ---: | ---: |
| Reprodução: retorno sem autorização | 1 | 0 | 1 |
| Reprodução: dificuldade ignorada pelo chefe | 1 | 0 | 1 |
| Play Mode inicial: campanha e regressões existentes | 36 | 32 | 4 |
| Play Mode após correções: percurso ampliado | 38 | 36 | 2 |
| Play Mode: reverificação das falhas, dicas e combate | 14 | 14 | 0 |
| Edit Mode final: progressão, migração em disco, dificuldade e câmera | 57 | 57 | 0 |

**Resultado final por teste distinto: 57 Edit Mode e 39 Play Mode aprovados**, considerando a execução mais recente de cada teste. As duas falhas da rodada de 38 foram reverificadas e aprovadas na rodada de 14; esta também inclui o novo teste das dicas. Não foi declarada aprovação da rodada com falhas.

Cobertura executada: casas de 1996/2026, paredes, móveis, profundidade, luz local, controles após pausa/diálogo/puzzle/inventário, fotografia e código, biblioteca, estacionamento/partida, áreas e pistas acessíveis, equipe/recargas, navegação e limite dos filhotes, retry, combate até vitória, recarga sem novo chefe, calibração, abertura salva, travessia salva, encerramento bloqueando saída prematura, Renan/caderno/Fusca/créditos. A câmera foi verificada nos formatos 4:3, 16:9, 21:9, 32:9 e retrato nas cenas cobertas pelos testes existentes.

Os XML completos ficam em `Logs/` nesta cópia local. [Evidência compacta versionada](EVIDENCIAS_15_FASES_20261007.json) registra contagens, datas, falhas reproduzidas, hashes dos XML e a origem da última aprovação de cada teste.

Os testes existentes foram adaptados às novas condições: rotina/pista de teste opcionais e fotografia/código na mesma visita. Duas falhas do equipamento foram diferenciadas: o pivô era um problema real de cache; a expectativa de usar a antiga textura de interação ao sentar estava desatualizada. A verificação de geometria foi mantida. A simulação de movimento da casa passou a enviar o estado completo do teclado, preservando as verificações de colisão e profundidade.

## Pendências

- Não foi gerado executável nem feita uma sessão manual completa do início ao fim. Os testes automatizados cobrem segmentos encadeados, áreas investigáveis, combate e final, sem equivaler a uma avaliação humana do ritmo.
- Falta revisão visual manual das dicas e legendas em várias resoluções e uma rodada de jogadores para calibrar Fácil/Difícil.
- Esta é uma revisão geral dos sistemas alcançados pela campanha e testes; não certifica ausência de problemas em todo o repositório.

Arquivos anteriores de recuperação e `Preview/Site/` foram preservados.
