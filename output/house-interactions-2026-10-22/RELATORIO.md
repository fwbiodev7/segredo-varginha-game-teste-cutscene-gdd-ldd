# Entrega de 22/10/2026

**Resultado técnico:** interface compacta, rotina com itens reais e persistentes, chave na mesa do café, recortes limpos, móveis de costas, paleta do quintal e camadas do Fusca integrados.

| Verificação | Resultado | Evidência |
|---|---|---|
| Suíte EditMode completa final | 398/398 | `editmode-delivery.xml`, `editmode-summary.json` |
| Player Windows, casos únicos | 16/16, última execução de cada caso | `summary.json` lista o XML de cada resultado |
| Interface e cinema | 11/11 | `delivery/player-native.xml` |
| Rotina e coluna fosca | 2/2 | `style-final/player-native.xml` |
| Fundo opaco da chegada | 1/1 | `opaque-final/player-native.xml` |
| Ordenação final do painel/volante | 1/1 | `layers-final/player-native.xml` |
| Build Windows 0.4.5-beta | Succeeded, zero erros, um aviso | `windows-build.json` |
| Áudio e apoios sem alterações posteriores | 5/5 casos correspondentes | `clean-final/player-native.xml` |
| Artes | 12 frames, zero erros | Relatórios em `art/` e `fusca/`; quatro avisos de apoio revisados |
| Daily | 10 slides, nove transições, zero overflow | `daily-qa.json` |
| Roteiros | Word/PDF: 18 e 10 páginas revisadas | `../dublagem-2026-10-22/document-qa.json` |

As rodadas e retestes se sobrepõem; **não** são somados para inflar a contagem de casos únicos. A rodada ampla anterior continha uma falha na asserção de escala do teste: os nós originais possuem escala 1,91. O teste final verifica os limites físicos efetivos, que permanecem iguais ao plano do mapa. As falhas iniciais de recorte/geometria foram corrigidas; resultados intermediários permanecem no diretório para rastreabilidade.

O único aviso da build é preexistente: o pacote opcional `Unity.Pipeline` não encontra `RuntimePipelineConfig` e se desativa no Player. O renderizador URP 2D usado pelo jogo permanece ativo. A candidata normal foi compilada; os testes de execução foram realizados no Player de testes, sem alegar um playtest humano separado desta candidata. O pacote contém guia, créditos e manifesto SHA-256, e sua integridade CRC foi verificada (`package.json`).

## Comportamento conferido

- Tutorial/rotina: lavar o rosto, beber café e preparar o notebook concluem por ação real. Interromper não consome o café nem confirma a lavagem. Após concluir, xícara cheia e notebook não reaparecem ao reabrir o save.
- Chave: colocação maior na mesa do café; posição e estado mantidos após reload. Textos não dependem mais de um copo que deveria continuar cheio no balcão.
- Móveis: colliders dos sete objetos mantêm posição/tamanho do mapa; o sprite antigo é desativado e o visual novo fica em um filho. A imagem-base também foi limpa, incluindo áreas externas aos antigos crops.
- Interface: coleta automática com o mesmo fundo menor; fala progressiva, revelação/avanço por teclado e controle, tamanhos em 960/1280/1920, pausa e canal de voz independente. O clip de teste é silencioso, sem dublagem sintética.
- Cinema: quatro frames visíveis de direção, suporte fosco, instrumentos atrás do volante, pausa, sequência completa, margens opacas na escola e retorno do controle. A captura preta do fade deixou de ser aceita como evidência de um frame de direção.
- Regressão: abertura de 1996, áudio gravado, passos/eventos, alunos e três apoios.

## Capturas e arte

`captures/sources.json` registra a rodada de origem de cada print final. Inclui rotina antes/depois, chave, legendas, avisos, casa, quintal, quatro poses do motorista e chegada à escola. `technical-report.md` descreve fontes, hashes, prompts, resoluções, pivôs, alpha, revisão visual e limitações das ferramentas.

As cenas e os arquivos de implementação estão relacionados em `Docs/CampanhaOficial/ATUALIZACAO_2026-10-22.md`. Não foram criados puzzles ou missões adicionais nesta revisão.

## Material de gravação e pendências

PDF, Word, Markdown e ZIP em `output/dublagem-2026-10-22/`. O elenco contém somente personagens atuais. As 98 falas **P** são propostas, as 47 **E** preservam texto existente e as oito **V** indicam esforço vocal. Nenhuma proposta foi integrada sem revisão.

Faltam os takes humanos, a revisão das propostas, o playtest externo, a mixagem em fones/caixas e a avaliação em outro PC. Não há resultado humano presumido. Lista em `Docs/CampanhaOficial/PENDENCIAS_2026-10-22.md`.

Ao finalizar os testes foram restaurados os três saves JSON, as preferências de registro, os 28 arquivos de ProjectSettings copiados antes dos testes e a configuração URP original do usuário. Backups e logs brutos estão em `output/agent-logs/`, ignorados pelo Git.
