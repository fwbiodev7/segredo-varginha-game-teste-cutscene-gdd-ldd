# Build Windows x64 do remaster - 08/10/2026

Esta build inclui o remaster cinematográfico desta entrega: enquadramentos nas pistas, fade/crossfade por contexto, movimento curto de câmera, continuidade dos ambientes sonoros, sons posicionais e iluminação discreta. Preserva a campanha de 14 fases, os sprites, o Fusca, os diálogos, os controles e os saves existentes.

## Resultado

- Unity 6000.6.0f1, Windows x64, renderização Built-in.
- Build concluída com sucesso: **0 erros e 1 aviso**.
- Tempo informado pelo BuildPipeline: **74,70 segundos**. O tempo de inicialização/importação do Editor não está incluído nessa medida.
- Tamanho informado pelo Unity: **294.953.581 bytes**.
- 19 cenas: menu, 14 fases e quatro áreas adicionais necessárias à campanha.
- Compressão LZ4HC, usando o gerador existente `Game.Editor.Testing.VarginhaAlphaBuild.Build`.

O aviso é: `Pipeline: No RuntimePipelineConfig asset found (Project Settings > Pipeline > Runtime). Pipeline will be disabled in Player builds.` Ele se refere ao pacote de automação `com.unity.pipeline`, não à renderização Built-in. A build não precisa habilitar a automação do Editor no jogo distribuído.

## Como jogar

1. Extraia todo o arquivo `jogo_varginha_remaster_2026-10-08.zip`.
2. Abra `jogo_varginha_build_alpha.exe`.
3. Mantenha o executável junto de `jogo_varginha_build_alpha_Data`, `UnityPlayer.dll`, `MonoBleedingEdge` e das demais bibliotecas extraídas.
4. Use **Continuar** para retomar seu progresso. **Iniciar campanha** substitui o save local.

O Unity Editor não é necessário para jogar. O ZIP contém os arquivos de execução, os guias existentes e estas notas. A pasta de backup que o Unity marca com `BackUpThisFolder_ButDontShipItWithYourGame` fica fora da distribuição.

## Validação e limites

A nova build inicializou com Direct3D 12 e permaneceu aberta durante 15 segundos. O teste ocorreu no menu, sem iniciar campanha, e o processo de teste foi encerrado ao final. O log não apresentou exceções do jogo nem falha fatal. Registrou o diagnóstico gráfico `d3d12: failed to query info queue interface (0x80004002).`; o dispositivo gráfico inicializou em seguida e o processo continuou ativo. Isso confirma a abertura do executável; não representa uma jogada completa.

Os testes do remaster foram executados antes desta build. Considerando o resultado mais recente de cada caso, os 47 casos distintos de Play Mode passaram. A última rodada completa de Edit Mode aprovou 229 de 241 casos. As 12 falhas de sprites também ocorreram no código original. Consulte [resultados consolidados](results-summary.json), [relatório da sprint](RELATORIO_SPRINT_2026-10-09.md) e [decisões da implementação](IMPLEMENTACAO.md).

A integridade de todas as entradas do ZIP foi verificada por CRC. O [manifesto](build-manifest.json) informa o SHA-256 do pacote, do executável e da biblioteca de código do jogo. O [relatório do Unity](build-results.json) contém as cenas e medidas da build; o [resumo da abertura](build-smoke-results.json) registra o teste de inicialização.

## Reproduzir

Abra o projeto no Unity 6000.6.0f1, encerre o modo Play e escolha **Varginha → Build → Alpha Windows x64**. A saída fica em `Builds/jogo_varginha_build_alpha/`.

Também é possível executar o gerador em lote:

```powershell
& 'C:\Program Files\Unity\Hub\Editor\6000.6.0f1\Editor\Unity.exe' -batchmode -quit -projectPath (Get-Location).Path -buildTarget Win64 -executeMethod Game.Editor.Testing.VarginhaAlphaBuild.Build -logFile 'Logs/remaster-build.log'
```

Distribua os arquivos de execução em ZIP, excluindo a pasta de backup do Unity. Os executáveis ficam fora do histórico de código do Git. O código, os testes, as evidências e os materiais da sprint são versionados na branch `codex/campanha-15-fases-e-site` do repositório `fwbiodev7/segredo-varginha-game-teste-cutscene-gdd-ldd`.

Esta entrega gera o ZIP local e envia as alterações de código/documentação ao GitHub. O link de download da alpha publicada em 07/10 continua apontando para aquela versão; não foi substituído por esta build.
