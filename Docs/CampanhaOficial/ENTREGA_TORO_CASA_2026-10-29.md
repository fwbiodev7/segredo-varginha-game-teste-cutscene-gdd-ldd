# Oizlede — Fiat Toro e interações da casa

Candidata **0.5.4-beta**, em diretório separado da0.5.3-beta já entregue. A build desta rodada contém Toro/casa/interior; redesign e hitbox do boss continuam pendentes. Não é conclusão de todo o escopo ampliado.

## Escopo implementado

| Item | Estado | Implementação e verificação pendente |
| --- | --- | --- |
| Fiat Toro | Parcial | Branca/elevada,128×72/14tons; escala na cena aumentada25% (174→218 pixels), maior que o Fusca estacionado. Rodas com8fases, oscilação discreta e sombra. Estado/cinemática aprovados; captura final revisada ainda pendente de reteste. |
| Pia | Concluído | Pés(605,708), .858m mais perto; percurso real, animação, colisão, retorno do controle e reload aprovados. Captura conferida. |
| Café | Concluído | Abordagem norte(705,530), alcance.65m/LOS; consumo/reload e cancelamento por obstáculo tangencial sem consumir nem travar aprovados. |
| Notebook | Concluído | Aproximação(86,554), alcance.65m, cadeira existente/tolerância.12m; sentar, usar, levantar, mover e reload aprovados. Captura conferida. |
| Interior do Fusca | Concluído | Novas camadas azuis com instrumentos/vidros/bancos/poses registrados como antes; alpha original idêntico em teste e GPU aprovada. Originais preservados. |
| Boss final | Parcial | Protótipo largo rejeitado. Novo conceito solicitado: alto e magro, místico do submundo, olhos vermelhos fortes, sem aparência demoníaca. Nova proposta em preparação, sem integração. Hurtbox usa medidas antigas menores que o sprite; correção e teste de golpes reais pendentes. |
| Narrador | Parcial | Guia das15falas, manifesto, pasta de entrada e checker preparados (9/9 verificações). Zero áudios recebidos confirmados; sincronização/integração dependem dos takes. |

O alcance anterior era1.5m para todos os pontos, sem verificação de obstáculo. As três ações agora exigem proximidade e linha livre entre os pés e a aproximação. Pia e café só concluem após chegar ao ponto correto. Cancelamento do café libera a rotina assim que a ação termina, sem aguardar o timeout de5s.

O Fusca do protagonista e seus sprites permanecem preservados. A nova Toro usa contorno, paleta e escala de pixels da referência original; o asset foi gerado com auxílio do subagente de arte e preparado como sprite próprio, sem reconstruir o áudio da passagem.

## Validação

- Arte final:128×72, alpha binário,14cores com carroceria branca/sombras frias; rodas completas, suspensão redesenhada e comparação em escala inteira. Validação estrutural sem erros/avisos. A cor/altura atendem à orientação humana recebida durante a execução. Fonte branca SHA256 `6683abc02d58700ea63842496074a99dad66dc3cd62bd7ba7aaedfef2a492832`.
- SHA256 do Fusca original preservado: `048dfd8c37a00d2fc84407871ea931f81927efd71bc69d50c733a549292f7344`.
- Player primeiro lote: **5/5 aprovados**, incluindo aproximações reais, consumo/reload, investigação preservada e música/cutscene em duas resoluções com a primeira Toro azul.
- Player casa final: **3/3 aprovados** (`house-close-cancel-2.xml`), incluindo interrupção do café por obstáculo que bloqueia os pés sem cruzar a linha central. Inspeção das três capturas em `QA-CASA.md`. Commit doméstico `b8e55ad`.
- Player Toro branca:2/2 aprovados antes do aumento final. Escala218/animação:1/1 aprovado antes de reforçar a captura. Alpha/recorte das novas camadas de cabine:1/1 GPU aprovado.
- EditMode dirigido final:33/33 aprovados, incluindo escala/enquadramento, animação/redução de movimento e identidade de todos os valores alpha da cabine/painel. Lote anterior de casa/cinemática:54/54 aprovado.
- QA ainda pendente: a primeira captura mostrou pausa; após remover a sobreposição, uma amostra atingiu a maçaneta escura em vez da porta branca. A amostra foi corrigida, sem reduzir a exigência visual. Depois houve perda da janela curta de observação de áudio; a verificação foi alterada para fases de distância observadas. O reteste mais recente não executou: build Player falhou por DXGI device removed (0x887a0005). Não anunciar esse reteste como aprovado. Evidências e XMLs anteriores preservados.
- Build/ZIP/inicialização: **pendente**.

Em29/10 o usuário autorizou commit e push para GitHub e geração de build ao atingir10% restantes, substituindo o limite anterior de15%. Retomada deve continuar as pendências acima após a renovação real.

## Arquivos e cenas

Assets novos em `Assets/Resources/Varginha/Revision20261029/`; apresentação em `CampaignPassingToro`/`CampaignFuscaCinema`. Pontos da casa em `IllustratedMaps/Layouts.json` (somente fase2); alcance em `CampaignAdultHouse`/`VarginhaCampaignStage`; encaixe/encerramento em `VarginhaPlayerActionAnimation`. Novos testes `CampaignCloseRoutinePlayTests` e `CampaignToroRoutineTests`, mais captura da passagem existente em duas resoluções.

Cenas atingidas: casa adulta `Ato2_Fase2_A_Chave_e_a_Caixa` e cutscene de chegada externa à escola. A estrutura de14 capítulos/19 cenas, puzzles, diálogos e IDs de progresso foi mantida. Os testes usam produto/cópia isolados, preservando os saves do usuário. Build e ZIP anteriores não serão sobrescritos. Sem push.

## Executar a candidata quando disponível

Extraia toda a pasta do ZIP e abra **Oizlede - Segredo de Varginha.exe**, mantendo a pasta de dados correspondente e DLLs ao lado. A identidade interna do produto permanece igual para compatibilidade dos saves.
