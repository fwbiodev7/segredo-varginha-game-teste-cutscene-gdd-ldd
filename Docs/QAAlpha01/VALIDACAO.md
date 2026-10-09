# Validação e entrega — alpha 0.1

Gerada em 09/10/2026, sem alterações adicionais no código, na arte ou nas cenas do jogo.

- Plataforma: Windows x64; Unity 6000.6.0f1.
- Build: `Succeeded`, zero erros, dois avisos, 165,39 segundos.
- Conteúdo reportado pelo Unity: 300.598.632 bytes; 19 cenas definidas pelo exportador existente. A sequência de gameplay foi preservada.
- Executável: `Builds/jogo_varginha_alpha_0.1/jogo_varginha_alpha_0.1.exe`.
- Pacote completo: `Builds/jogo_varginha_alpha_0.1_Windows_x64.zip`.
- Relatório do Unity: `AlphaBuildReport.json` nesta pasta.

## Verificações

131 testes de edição passaram; os três testes visuais atuais passaram, incluindo movimento em Fragmentos, cores/bordas dos vitrais e carregamento dos 22 layouts. Consulte `Reanalise_EditMode.json` e `Reanalise_VisualPlayMode.json`.

O player foi iniciado em modo automatizado com gráficos, permaneceu em execução e seu log não registrou exceções, erros de shader ou falha de carregamento durante a observação inicial. O processo usado para o teste foi encerrado depois da coleta do log. Evidências: `Alpha01SmokeResult.json` e `Alpha01PlayerSmoke.log`.

A placa desta máquina não criou o dispositivo Direct3D 12; o player usou Direct3D 11. O driver também registrou indisponibilidade de `ID3D11Fence`. A verificação de inicialização não substitui uma inspeção visual ou partida manual completa.

## Avisos de build

1. O pacote de controle Unity Pipeline não tem `RuntimePipelineConfig` e ficará desativado no player. Esse aviso se refere ao pacote de automação do Editor.
2. `VarginhaPixelPresentation.cs` contém um `using UnityEngine.Rendering` duplicado (CS0105).

## Limites da validação

A regressão histórica em `RegressionRun.xml` tem 33 aprovações e 3 falhas; duas dessas verificações passaram na nova rodada visual. A falha do percurso da biblioteca ainda precisa ser reproduzida. As tentativas de seleção isolada retornaram zero testes, portanto não foram contadas como aprovação. A regressão completa atual e uma partida manual completa continuam pendentes.

Arquivos produzidos nesta entrega: pasta e ZIP da build, `LEIA-ME.txt`, relatório de build, resultado/log da inicialização e este documento. Nenhum arquivo `.unity` da campanha foi alterado para gerar a entrega.
