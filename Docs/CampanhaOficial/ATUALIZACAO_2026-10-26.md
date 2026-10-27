# Atualização parcial — 26/10/2026

O recorte antigo de piso em torno da cadeira do notebook chamava atenção como um retângulo. A máscara em `IllustratedMorning.shader` agora acompanha uma caixa arredondada e suaviza a borda; a mudança preserva as texturas e sprites originais. A imagem de jogo confirma a postura sentada e a oclusão das pernas atrás do encosto, mas ainda deixa uma transição de piso perceptível. Por isso, o acabamento permanece parcial e precisa de nova revisão visual.

Validação feita em cópia QA isolada: `office-rounded-cutout-player.xml`, teste `OfficeSeatAlignsWithTheCushionAndReturnsToItsClearApproach`, 1 aprovado, 0 falhas e 0 ignorados. Captura: `output/revisao-final-2026-10-26/world-review/office-notebook-seated.png`. O teste cobre interação, retorno à posição de aproximação e alinhamento do corpo com a cadeira; não comprova que a transição do piso está finalizada.

A suíte de navegação do Livro, trilhas e continuidade foi executada no Player: `puzzle-design-player.xml`, 8/8 aprovados. As capturas dos estados do Livro (inicial, associação incorreta e reconstituição), da trilha e do botão CONTINUAR foram inspecionadas. O painel do Livro está legível e suas associações são corrigíveis; CONTINUAR aparece no canto inferior direito e não permite pular duas fases. A apresentação visual das demais transições de capítulo ainda precisa de revisão.

Os testes de veículos da cópia isolada também passaram no Player: `vehicle-fixed.xml`, 3/3, cobrindo movimento da estrada, pista opcional e saída do contato na parede inferior. Esse recorte aprovado substitui as tentativas de diagnóstico anteriores que falharam antes de exercitar a estrada. Itens 12 e 13 passam a Concluído na lista de pendências.

Auditoria global de colisões e proporções no Player terminou aprovada (1/1), percorrendo 18 ambientes, 302 móveis e 1.018 aproximações físicas; 51 silhuetas visíveis ficaram entre 0,9545 m e 1,5963 m de altura e entre 0,4562 m e 1,2977 m de largura. Revisei as 18 capturas de mapas e não encontrei falha aparente de profundidade. Quatro props não tiveram lado livre para aproximar o probe porque ficam atrás de outros colliders e fora dos percursos caminháveis: mesa redonda da fase 6, lampiões da fase 12 e torneira central da fase 18. Com esses limites registrados, o item 01 passa a Concluído. A execução final está em `output/revisao-final-2026-10-26/world-contact-audit-player-final4.xml`; CSV e resumo de contagens na mesma pasta.

Também foi salva a lista detalhada de estados e próximos passos em `PENDENCIAS_REVISAO_MESTRE_2026-10-26.md`. A apresentação de daily ainda precisa receber as atualizações posteriores a 23/10. A abertura do HTML local foi recusada pela política do navegador para `file://`; não foi tentado contornar essa restrição. O roteiro completo para a apresentação de 30/10 continua previsto depois de concluir e validar a revisão.

Após inspecionar a captura do notebook, o shader passou a amostrar o piso próximo à cadeira e usa uma transição arredondada de 24 px. `office-nearby-floor-player-valid.xml` e `office-feathered-floor-player.xml` confirmaram 1/1 no Player. A borda está mais suave e as pernas continuam ocultas, mas ainda há mudança de piso visível ao redor da cadeira; item 02 permanece Parcial. Captura: `output/revisao-final-2026-10-26/world-review/office-notebook-seated.png`.

O item 04, recuperação após glitch, foi concluído: `visual-fixed.xml` aprovou 1/1 a sequência de três glitches, e comparei antes, pico e recuperação. A cena recuperada volta à luminância-base dentro de 0,025; `vehicle-glitch-playmode.xml` já havia confirmado que desativação, reconfiguração e cancelamento limpam os estados visuais e de áudio.

Para o item 11, ampliei `CompactContinueUsesControllerAndCannotSkipTwoChapters` para capturar a chegada à próxima fase após a transição real 6→7. `compact-continue-transition-player.xml` passou 1/1; comparei `compact-continue-next-chapter.png`. O controle e a chegada estão claros nesse caso, mas ainda falta verificar as transições próprias dos demais capítulos, por isso a pendência continua Parcial.

Também acrescentei uma regressão Player para a continuidade na fase 14: salva a conclusão, abre o painel CONTINUAR, confirma que uma segunda ativação não agenda avanço duplo e verifica a chegada à fase salva 15. `continuation-continue-transition-player.xml` passou 1/1. Inspecionei `continuation-chapter-complete.png` e `continuation-next-chapter.png`; o painel de conclusão e a cena seguinte estão legíveis. A captura da nova cena identifica Ato 4, fase 11 (fase de campanha 15). O item 11 permanece Parcial porque ainda faltam as transições dos demais capítulos.

## Fechamento posterior em 27/10/2026

As pendências deste registro foram encerradas na entrega 0.4.9-beta. Consulte [a entrega de 27/10](ENTREGA_2026-10-27.md) para estado atual, evidências, arquivos e limitações. Este texto conserva o histórico do trabalho de 26/10.
