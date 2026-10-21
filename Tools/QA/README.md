# Verificações da entrega

Use o Node e Python disponíveis no ambiente e o wrapper `Tools/AgentOutput/compact.mjs` para execuções extensas.

## Unity

Execute uma instância por projeto. Encerre o Editor interativo antes dos comandos batch. Preserve os saves antes dos testes e restaure-os ao terminar; o runner não substitui esse cuidado.

```powershell
node Tools/AgentOutput/compact.mjs --question "A regressão passou?" -- "C:\Program Files\Unity\Hub\Editor\6000.6.0f1\Editor\Unity.exe" -batchmode -projectPath CAMINHO_ABSOLUTO_DO_PROJETO -runTests -testPlatform PlayMode -testResults output/qa-2026-10-21/playmode-all.xml -logFile output/agent-logs/playmode.log
python Tools/QA/summarize_unity.py output/qa-2026-10-21/playmode-all.xml --out output/qa-2026-10-21/playmode-all.json
```

Não acrescente `-quit` a `-runTests`. O runner encerra o processo depois de salvar o XML. Use `EditMode` para a suíte de regras. `StandaloneWindows64` compila e executa os testes num Player; a entrega de distribuição usa `Game.Editor.Testing.VarginhaDailyBuild.Build` separadamente.

O resumo requer resultado global aprovado, pelo menos um teste aprovado e nenhuma falha. Mostra contagens e no máximo 500 caracteres por mensagem no console, guardando mensagens e stacks completos no JSON solicitado. Não trate zero testes, resultado desconhecido, execução incompleta ou código de saída isolado como aprovação. Para investigar, busque o teste específico no log preservado.

```powershell
python Tools/QA/test_summarize_unity.py
```

Esses testes verificam aprovação falsa, suíte vazia, skips, estado desconhecido e preservação de mensagens extensas.

`python Tools/QA/campaign_coverage.py` consolida o último resultado de cada caso da rodada completa de 21/10 e dos retestes explícitos. Conserva os resultados originais das rodadas e lista os cinco contratos renomeados. Exige execuções finalizadas, contagens completas e nenhum caso final com falha/skip. O resultado é cobertura consolidada, **não** uma execução única de toda a suíte aprovada.

Fixtures de componentes usam `IsolatedPlayModeTests`. Setups baseados em `UnitySetUp` (física/IA) chamam o reset explicitamente antes de criar os objetos; fixtures com `SetUp` síncrono usam a classe base. Isso evita que callbacks de tipos diferentes removam os objetos do próprio teste.

## Arte

```powershell
python Tools/KeyInvestigationArt/audit_art.py --out output/NOVA_PASTA_DE_QA
node Tools/QA/art_preview.mjs output/NOVA_PASTA_DE_QA/preview
```

O primeiro comando usa os auxiliares da skill `pixel-art-sprite-sheet-master`, Pillow e as fontes da entrega de 20/10. Exige pasta nova e não modifica os assets. O segundo usa Playwright/Chrome do ambiente para observar avanço e parada dos clipes da prévia. Ambos exigem revisão visual das imagens geradas e não certificam aprovação da equipe.

O percentual do wrapper mede bytes da saída, não tokens. Nenhum modelo local foi configurado para esta entrega.

## Distribuição e daily

`Game.Editor.Testing.VarginhaDailyBuild.Build` gera a candidata 0.4.2-beta em pasta nova e restaura a versão do projeto. Execute com `-batchmode -quit -executeMethod`, sem `-runTests`. O relatório deve terminar em `Succeeded`, com zero erros; confira seus avisos antes de entregar.

`python Tools/QA/package_windows.py --qa output/cinematic-opening-2026-10-21` exige esse relatório, inclui o guia de playtest, calcula SHA-256 por arquivo e do ZIP, reabre o pacote para conferir CRC e rejeita assemblies de testes. Exclui as pastas de backup/debug explicitamente identificadas pela Unity como `DoNotShip`. O pacote permanece local até uma publicação autorizada.

`node output/daily-sprint-2026-10-20/build.mjs` usa `output/cinematic-opening-2026-10-21/summary.json` para atualizar a daily. Depois, execute `qa.mjs` da mesma pasta com o wrapper e revise as imagens. O QA valida dez slides, nove transições, navegação rápida, fontes/imagens, limites de texto, notas, impressão e movimento reduzido. A apresentação preserva a retrospectiva histórica e identifica as novidades por data; contagens consolidadas de PlayMode não são apresentadas como uma única rodada completa aprovada.


`node Tools/QA/cinema_preview.mjs` monta e verifica a prévia silenciosa a partir de pelo menos 40 capturas reais em `output/cinematic-opening-2026-10-21/film`. Não recria a cena nem substitui teste de áudio. Os clipes-fonte da skill possuem seus próprios atlas/JSON/planos Unity e prévias na pasta `art`.


A revisão final de 21/10 está em `output/cinematic-opening-2026-10-21`: 367 casos EditMode e cinco casos direcionados no Player Windows. Os XML/JSON identificam cada execução. O caso de UI de Fragmentos agora se chama `TrailMapCanPreviewUndoAndTraceWithController`, substituindo o contrato antigo de ordenar/girar; os novos contratos verificam solução única, feedback com prefixo preservado, dicas e migração. A cobertura histórica em `output/qa-2026-10-21` permanece histórica, sem ser reinterpretada como teste da revisão nova.
