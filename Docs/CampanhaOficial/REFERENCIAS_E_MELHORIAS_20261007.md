# Referências e melhorias gerais — 7 de outubro de 2026

As referências abaixo foram pesquisadas e apresentadas no chat antes de aplicar as melhorias gerais. Foram usadas como princípios de usabilidade; os mapas, personagens e arte do jogo foram conservados.

| Referência | Aplicação nesta atualização |
| --- | --- |
| [Thimbleweed Park: discussão técnica do sistema de dicas, com resposta de Ron Gilbert](https://forums.thimbleweedpark.com/t/hint-system-tech/3139) e [publicação oficial sobre a inclusão das dicas](https://blog.thimbleweedpark.com/happy_anniversary2.html) | Dicas dependem do progresso atual: caixa, estacionamento, reagente, vitória e travessia. Três níveis revelados pelo jogador, com aviso antes da solução. |
| [Game Accessibility Guidelines — lista completa](https://gameaccessibilityguidelines.com/full-list/) | Objetivos e indicações consistentes, ajuda opcional, leitura sem limite nos close-ups da caixa e da fotografia, evidências reunidas no caderno. A opção existente de ocultar indicações passa a ser respeitada na expansão e no encerramento. |
| [Hades — FAQ oficial](https://www.supergiantgames.com/blog/hades-faq/) | A referência motivou verificar se a dificuldade oferecida realmente ajudava. Foi corrigido o chefe que ignorava Fácil/Médio/Difícil, usando os ajustes já existentes no projeto. |

## Melhorias aplicadas

- Caderno global com documentos das fases atuais e das etapas antigas preservadas. A fala breve do rádio também pode ser relida.
- A partir da fase 2, ajuda em **TAB → PRECISO DE UMA DICA**, ou **F1** na exploração e batalha. A ajuda pausa as ações do personagem e não concede evidências, cargas ou conclusões.
- Close-ups de investigação da casa e da fotografia aguardam **CONTINUAR** ou a interação configurada. Aberturas cinematográficas e ações físicas mantêm suas durações.
- Dificuldade do chefe e dos filhotes influencia resistência, dano, deslocamento de aproximação, intervalo entre ataques e tempo de aviso. Médio mantém os valores anteriores. Uma mudança vale para inimigos criados na tentativa seguinte.
- Sprites equipados preservam pivô, tamanho e escala do corpo. O cache distingue sprites com geometria diferente, evitando reutilizar um alinhamento de outra cena.
- Saves limitam a área retomada às áreas existentes na fase. O acordo fundido exige também a conclusão do procedimento antigo. Girar um regulador invalida a confirmação anterior do retorno.

## Limites

Esta revisão priorizou progressão, interação, leitura, combate e problemas reproduzidos pelos testes existentes. Não é uma auditoria exaustiva de todo o conteúdo. O relatório de execução separa verificações automatizadas concluídas de validações ainda pendentes.

Veja o [guia das 15 fases](GUIA_COMPLETO_PUZZLES.md) e o [relatório desta atualização](RELATORIO_15_FASES_20261007.md).
