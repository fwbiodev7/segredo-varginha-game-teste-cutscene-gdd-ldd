# Validação da campanha experimental — 1 de outubro de 2026

Projeto: cópia `segredo-varginha-game-teste-cutscene-gdd-ldd`, Unity 6000.6.0f1. A validação refere-se à campanha implementada até a Fase 5; as fases 6–20 continuam como planejamento.

Base inicial: `f34e0db`. Após conferir o GitHub original, os modelos atualizados da mochila, os atlas de Edelzio/Ana Tavares/Luis Miguel Messias e a fachada real pixelada foram recuperados de `5df6ec4`. O projeto original não recebeu alterações.

## Escopo

## Resultados obtidos antes do encerramento dos testes

| Verificação | Resultado |
| --- | --- |
| Dependências dos puzzles e respostas sem evidências | Aprovado |
| Recuperação de progresso corrompido | Aprovado |
| Salvamento/recarregamento de documentos e dois fragmentos | Aprovado |
| Percurso jogável Fases 2–5, mochila, rotina, pane, alunos, código e retorno ao save | Aprovado — 38,48 segundos, após importar a fachada e a mochila atualizadas |

Total confirmado nesta rodada: **4 testes aprovados**. A compilação após a importação dos modelos originais terminou sem erros; o Editor ainda apresenta avisos de APIs obsoletas.

O teste de colisão isolado teve uma aprovação anterior durante o desenvolvimento. Nas tentativas posteriores, a ferramenta retornou zero casos. A checagem da infância foi incluída também no percurso completo, junto com verificações explícitas da fachada e dos nove alunos; essa última ampliação do teste **não foi executada**, pois o usuário pediu para encerrar os testes e enviar o commit. Não é contada como aprovação adicional.

- Fluxo da rotina adulta: higiene, café, notebook na cadeira e coleta da mochila original.
- Mochila: abrir, fechar e manter o contrato das células originais dos sprites.
- Investigação: dependências dos puzzles, tentativas incorretas, documentos e dois fragmentos de mapa.
- Fusca: movimento real por teclado, pane, três inspeções, pausa, retomada e chegada à Industrial.
- Escola: conversa individual registrada, correspondência de Ouzana, Renan e identificação da planta.
- Código das 23:23: legenda, ordenação dos símbolos, confirmação e continuidade após salvar/voltar ao menu.
- Infância: colisão contra parede e passagem pela porta existente com movimento real por teclado.
- Recuperação de dados salvos e estados inválidos.

## Problemas encontrados durante a validação

A primeira integração desativava o HUD antigo e perdia a referência necessária à mochila. O HUD agora mantém apenas o inventário na campanha, sem desenhar a interface antiga.

O novo colisor dos pés encostava na mesa durante a animação de sentar. O destino da pose foi ajustado para manter a colisão fora da mesa; a cadeira e os quadros originais são reutilizados. Tentativas por um lado bloqueado mostram orientação na interface da campanha.

As tentativas de execução que retornaram zero casos ou excederam o prazo da conexão do Editor não contam como testes aprovados. Esses registros de conexão são separados das falhas do jogo.

## Limites

Esta entrega não inclui um executável empacotado nem validação em outros computadores. Os sons são efeitos e ambientes sintetizados para a prévia; a reportagem permanece com legendas, sem locução robótica. A arte e a animação ainda podem receber refinamento manual após o teste do usuário.
