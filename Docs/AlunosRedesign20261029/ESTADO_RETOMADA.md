# Oizlede — estado de entrega e retomada

## Concluído nesta entrega visual

- 12 retratos canônicos enviados pelo usuário integrados à mochila; originais preservados em ArtSources/CanonicalPortraits20261029. Recortes de pares usam o busto inteiro. Os arquivos derivados são256 × 256, sem suavização, Sprite 2D/UI, Point, sem compressão nem mipmaps. Fontes com fundo opaco tiveram apenas máscara técnica de fundo conectado, preservando os desenhos.
- Nome Anna Sabiá corrigido; identificadores de save anteriores preservados.
- 12 alunos inspecionáveis; seleção somente na luta final, até 3 alunos e 1 apoio. Novos índices 9–11 compatíveis com saves.
- Yasmin: caminhada/repouso em quatro direções com cachos longos pretos e olhos azul-esverdeados, baseada no retrato canônico enviado.16 quadros completos, pés alinhados e escala do jogo. Fonte gerada preservada. Não há novos estados de dano/morte/corrida/interação ou ataque de piano especificamente desenhados nesta rodada.
- Outros 11 sprites básicos conservados no estado do checkpoint anterior, conforme a instrução mais recente de refazer somente Yasmin.
- Plano mobile em Docs/PLANO_PORT_MOBILE.md; implementação mobile não iniciada.

## Validação executada

Unity 6000.6.0f1. EditMode: 19/19 passaram. PlayMode específico desta atualização: 7/7 passaram, incluindo seleção em todas dificuldades, saves, retratos/grade, bloqueio fora da final e ataques atualmente existentes dos três alunos novos. Isso NÃO valida os poderes novos pedidos no prompt.

Regressão anterior mais ampla: 24/30 passaram. As 6 falhas remanescentes foram reproduzidas com o código runtime original 412cd5f, com nomes e mensagens idênticos: visibilidade inicial de um renderer auxiliar; ricochetes de Anna; cancelamento por morte durante pausa; perfuração da katana; cor do brilho de Marcos; descrição antiga de jiujítsu. Não foram corrigidas nesta rodada de conservação de uso.

Captura da mochila gerada no Unity em Preview/Students/Redesign20261029/Mochila_LutaFinal.png. Contato dos retratos finais: Retratos_Canonicos_12.png na mesma pasta. A captura da mochila antecede a troca do importador Default para Sprite, e os19 testes finais confirmaram o carregamento com esse importador atualizado.

## Parcial / não iniciado

| Item | Estado | O que resta |
| --- | --- | --- |
| Yasmin completa | Parcial | Novos desenhos de corrida, dano, morte, interação e execução do piano, conforme consumidor necessário. |
| Sono Profundo de Moscardini | Não iniciado | Estado temporário da IA, partículas Z, resistências de chefes e testes. Continua com golpe de apoio existente. |
| Livros da Bianca | Não iniciado | Projetis/livros, colisão, páginas, sequenciamento e limpeza. Continua com golpe de apoio existente. |
| Barreira da Giovana | Não iniciado | Escudo temporário, repulsão, bloqueio/reflexão e testes. Continua com golpe de apoio existente. |
| Animações especiais dos 12 | Parcial | Os poderes antigos mantêm regras/efeitos existentes; figuras usam atlas de uniforme. Faltam folhas novas de execução, instrumentos e movimentos próprios. |
| Créditos cinematográficos | Não iniciado | Implementar a sequência, equipes exatas, trilha autorizada, homenagem, logo e controles descritos no prompt preservado. Créditos existentes mantidos. |
| QA integral em outro PC | Não iniciado | Build não validada em outro computador nem campanha inteira novamente. |

O pedido mestre integral está preservado em Docs/PromptRetomadaOizlede20261029.md, com a ordem de prioridades e a lista completa dos créditos. Não substituir isso por uma declaração de conclusão geral.

## Restrição de uso

Na consulta desta rodada, o limite semanal estava 90% consumido e os subagentes retornaram bloqueio de uso. Para atender ao pedido de economizar e não esgotar o restante, foram reutilizadas as artes enviadas e feita uma única geração adicional para a caminhada da Yasmin. Nenhuma garantia numérica de economia é alegada. A geração foi pela ferramenta integrada, sem API paga alternativa.

## Build e Git

A build desta entrega é PARCIAL: atualização visual com gameplay anterior preservado. Exportada em Builds/Oizlede-Fragmento-Perdido-Apresentacao, mantendo Builds/jogo_varginha_build_alpha anterior. Distribuir toda a pasta ou ZIP, nunca somente o executável. Resultado e smoke test serão registrados em BUILD.md.

Checkpoint local inicial: f59b3f6. Branch de trabalho: codex/alunos-oizlede-20261029. Referências fotográficas pessoais serão mantidas locais e excluídas da publicação. Commits e push finais serão registrados em BUILD.md.
