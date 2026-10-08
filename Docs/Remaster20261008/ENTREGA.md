# Entrega para a sprint de 9 de outubro de 2026

O remaster está integrado ao projeto local, preservando a arte, a história e os controles. Esta entrega ainda não inclui uma nova alpha publicada.

Atualização após a entrega dos slides: nova build Windows x64 concluída em 08/10, com zero erros. Código, testes e materiais versionados nesta branch. Consulte as [notas da build e reprodução](BUILD_WINDOWS.md) e o [manifesto do ZIP](build-manifest.json). Os slides e o relatório da reunião registram o estado anterior à geração desta build.

- Apresentação editável: [PowerPoint](../../output/presentation/O_Segredo_de_Varginha_Sprint_2026-10-09.pptx).
- Apresentação para a reunião: [PDF](../../output/presentation/O_Segredo_de_Varginha_Sprint_2026-10-09.pdf).
- Relatório da semana: [PDF](../../output/pdf/Relatorio_Sprint_O_Segredo_de_Varginha_2026-10-09.pdf).
- Capturas reais: [índice dos prints](Prints/README.md).
- Validação: [resultados consolidados](results-summary.json) e relatórios XML nesta pasta.
- Aviso sonoro de conclusão: [WAV](../../output/aviso_conclusao.wav), registrado de forma simples no slide 6.

A compilação passou. Os 47 casos distintos de Play Mode passaram considerando o resultado mais recente de cada caso. A última rodada de Edit Mode aprovou 229 de 241 testes. As 12 falhas de sprites também ocorreram no código original.

Os sete slides e as cinco páginas do relatório foram renderizados e conferidos visualmente. O PowerPoint passou nas verificações de integridade, geometria e importação. O PDF da apresentação preserva o layout verificado e tem texto pesquisável.

As capturas históricas que os testes regravaram foram restauradas. As versões desta rodada permanecem em `Prints/Regressao`.
