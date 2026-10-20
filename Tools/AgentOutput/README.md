# Saídas compactas para o agente

Este projeto usa a ideia do [Distill](https://github.com/samuelfaj/distill/blob/main/docs/token-saver.md): guardar a saída integral e mostrar primeiro somente o resultado necessário. A implementação é local, sem dependências novas e sem alterar o jogo.

## Uso

```powershell
node Tools/AgentOutput/compact.mjs --question "A apresentação passou na conferência?" -- node output/daily-sprint-2026-10-20/qa.mjs
node Tools/AgentOutput/compact.mjs --timeout-ms 120000 -- git status --short
```

O executável e seus argumentos são separados, sem shell intermediário. Para scripts PowerShell, use `pwsh.exe -NoProfile -File caminho.ps1` depois de `--`. O processo recebe a saída normal do comando e preserva seu código de saída. Tempo excedido usa 124; falha ao iniciar usa 127. A pergunta orienta a nota do modelo opcional. Sem modelo, o extrator devolve os campos verificáveis do resultado.

As saídas completas ficam em `output/agent-logs/`, com stdout, stderr, resumo JSON e hashes SHA-256. O resumo reconhece JSON de testes, resultados da apresentação e cabeçalho NUnit XML. Uma execução sem testes ou incompleta nunca vira aprovação. Se o formato é desconhecido, o resumo informa somente o estado do processo, sem afirmar sucesso nos testes. Logs grandes são inspecionados por janelas, identificadas no resumo.

Use `rg -n` no log indicado para investigar falhas. Leia fonte, diffs, instruções e trechos solicitados diretamente, sem compactação.

## Modelo local opcional

Não há modelo instalado ou baixado por esta ferramenta. A economia por extração estruturada já funciona sem modelo. Se houver um servidor local compatível com chat completions:

```powershell
node Tools/AgentOutput/compact.mjs --question "Qual é a falha principal?" --local-endpoint http://127.0.0.1:11434/v1/chat/completions --local-model SEU_MODELO -- node teste.mjs
```

Somente endereços de loopback são aceitos. O modelo acrescenta uma nota curta e não pode substituir código de saída, contagens de testes ou evidências. Timeout, JSON inválido ou servidor indisponível conservam a extração estruturada. Nenhuma chamada a serviço externo é feita.

`output_reduction_percent` mede bytes da saída reduzidos nesta execução. Não mede tokens nem economia na conta, e pode ser negativo para saídas que já eram curtas. A alegação de até 99% do post não é uma garantia deste projeto.

A versão atual do repositório Distill é um agente completo. O pacote Windows opcional da versão npm consultada (1.5.2) retornou 404. Esta ferramenta implementa a técnica no fluxo existente, sem substituir o Codex nem sua configuração global.
