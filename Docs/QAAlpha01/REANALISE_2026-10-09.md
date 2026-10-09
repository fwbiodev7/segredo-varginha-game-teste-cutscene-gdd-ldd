# Reanálise da revisão alpha 0.1 — 09/10/2026

Atualização posterior: a build Windows alpha 0.1 foi gerada com sucesso a pedido do usuário. Resultados e limites da entrega estão em `VALIDACAO.md`. As pendências abaixo descrevem o estado encontrado durante a análise anterior à geração.

Esta rodada analisou o trabalho existente e executou verificações sem editar código, layouts, PNGs originais ou cenas da campanha. Não foi gerada uma nova build.

## O que já foi feito

- 22 layouts e 318 objetos, com meshes compostos, plantas apoiadas nos móveis e refinamento das bases de colisão.
- Ajustes nas colunas e na escada de Fragmentos, ordenação visual pelo contato no chão e iluminação dos vitrais nas cenas da igreja.
- Pipeline URP 2D, filtragem Point na arena e nas vistas do Fusca, captura de pausa e transições adaptada ao URP.
- Comparação estrutural do `Layouts.json` com HEAD: imagens de origem, dimensões, limites, posições de entrada, pontos dos objetivos e iluminação ambiente preservados. O diff não contém mudanças nos PNGs originais dos mapas/Fusca nem nas cenas da campanha.

## Verificações desta rodada

| Verificação | Resultado | Evidência |
|---|---|---|
| Testes Edit Mode selecionados por `Campaign` | 131 executados, 131 aprovados | `Reanalise_EditMode.json` |
| Testes Play Mode selecionados por `CampaignVisualPolish` | 3 executados, 3 aprovados | `Reanalise_VisualPlayMode.json` |
| Movimento abaixo da coluna e bloqueio na escada | Aprovado no teste atual | Incluído nos 3 testes visuais |
| Carregamento dos 22 layouts, meshes, filtragem e colliders | Aprovado | Incluído nos 3 testes visuais |
| Cores azul/âmbar/violeta e suavização da borda no renderizador | Aprovado | Incluído nos 3 testes visuais; `Vitral_GPU.txt`, `Vitral_Edge_GPU.txt` |
| Compilação do Editor | Editor conectado, sem falha de compilação | Unity CLI `editor_status` e `console_status` |
| Seleção isolada do percurso da biblioteca | Nenhum teste executado; não constitui aprovação | `Reanalise_SelecaoVazia.xml`, `Reanalise_PercursoPlayMode.json` |

O arquivo de testes visuais foi atualizado às 08:27:48, depois do resultado anterior das 08:25:47. A nova rodada confirma que as verificações visuais atuais passaram. Isso não comprova que a regressão completa passou com o código atual.

## Pendências encontradas

1. **Percurso físico de Fragmentos:** `RegressionRun.xml` registra falha no teste `IndustrialLibraryHasPhysicalPassagesAndReachableEvidence`, no ponto mundial `(0.25, -2.38)`, equivalente aproximadamente ao pixel `(512.5, 513)`. Distância medida: 0.3413 m, limite esperado: 0.28 m. É necessário reproduzir e separar problemas no percurso automatizado de problemas de colisão antes de mudar o jogo.
2. **Seleção de testes:** as tentativas por nome completo de método e de classe retornaram zero testes, embora o catálogo e a reflexão do Editor mostrem o método. O executor permite salvar `Passed` com total zero. Um resultado vazio precisa ser tratado como verificação incompleta.
3. **Avisos nas capturas URP:** `SampleProbe` e `Capture` em `CampaignVisualPolishPlayTests.cs` criam RenderTextures com profundidade zero. O Unity registra que as saídas do Render Graph precisam de buffer de profundidade. Os testes passaram, mas os auxiliares devem usar destinos compatíveis.
4. **Aviso de compilação:** `VarginhaPixelPresentation.cs` repete `using UnityEngine.Rendering`; limpeza simples, sem efeito de gameplay.
5. **Build pendente:** não existem `Builds/jogo_varginha_alpha_0.1/jogo_varginha_alpha_0.1.exe` nem `Logs/AlphaBuildReport.json`.
6. **Documentação incompleta:** `RELATORIO.md` aponta para `VALIDACAO.md` e `ARQUIVOS_MODIFICADOS.txt`, que ainda não existem. O XML antigo de Edit Mode contém zero testes; as evidências válidas desta rodada estão no novo JSON.

O resultado histórico da regressão foi preservado em `RegressionRun.xml` e restaurado em `PlayModeResults.xml` depois das tentativas vazias. Ele registra 36 testes, 33 aprovados e 3 falhas; duas das verificações que falharam naquele resultado passaram na nova rodada visual. O histórico não foi substituído por uma alegação de sucesso geral.

## Plano proposto

1. Corrigir a seleção/registro dos testes para detectar execução vazia e reproduzir o percurso físico da biblioteca com diagnóstico dos obstáculos e do movimento.
2. Ajustar o percurso ou as colisões apenas conforme a reprodução, mantendo a arte e os pontos dos puzzles.
3. Corrigir os destinos de captura URP e o `using` duplicado.
4. Reexecutar a regressão completa relevante, incluindo isolamento dos dispositivos de entrada, pausa, transições, puzzles e combate. Confirmar que nenhum grupo terminou com zero testes.
5. Com as verificações aprovadas, gerar e conferir a build Windows `jogo_varginha_alpha_0.1`; completar a documentação com resultados reais.

Arquivos candidatos aos ajustes: `CampaignPolishRunner.cs`, `CampaignVisualPolishPlayTests.cs`, `CampaignExpansionPlayTests.cs`, `VarginhaPixelPresentation.cs` e, somente se o diagnóstico demonstrar necessidade, `CampaignMapPlan.cs`/`Layouts.json`. Cenas de atenção: Fragmentos e as cenas que reutilizam a igreja; a revisão visual já percorreu os 22 layouts listados no relatório anterior.

Não há evidência suficiente para declarar o jogo inteiro sem bugs. A partida manual completa e a execução do player Windows ainda não foram realizadas nesta rodada.
