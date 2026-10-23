# Falas do Edelzio aprovadas em 23 de outubro de 2026

As 14 falas adultas do primeiro lote foram substituídas pela redação descontraída aprovada. A frase infantil do Fusca foi preservada. O PDF de gravação recebeu o nome exato `roteiro_edelzio_definitivo.pdf` e o pacote de envio contém esse mesmo arquivo e o TXT correspondente.

## Resultado no jogo

- Reações mais curtas e naturais, com menos descrições de inventário e sem datas ou horário recitados nessas falas.
- Observações da chave, porta e papel não acrescentam mais a frase falada sobre registro no caderno. Os registros, flags, save e gatilhos permanecem ativos.
- A idade do papel, o traço azul, a porta fechada por dentro, os arranhões sob a cama, a mochila e a descoberta do horário continuam sustentando a investigação.
- O horário 23:23 permanece no puzzle e as datas continuam nos documentos. Nenhuma resposta ou condição de resolução foi alterada.
- IDs de voz recalculados a partir das novas falas, com correspondência entre os códigos curtos do ator e os nomes finais esperados pelo runtime. A fala infantil mantém seu ID.
- ED03 foi atualizada na legenda da cutscene de colocar a chave. Essa legenda ainda não dispara a gravação automaticamente; a pendência está identificada no mapa técnico. Nenhuma dublagem humana foi declarada recebida ou integrada.

## Arquivos e cenas

- `Assets/Scripts/Game/Varginha/Experiment/CampaignHouseSequence.cs`: falas da chave, caixa, observações e legenda do depósito da chave.
- `Assets/Scripts/Game/Varginha/Experiment/VarginhaCampaignStage.cs`: falas de preparação, caixa concluída, saída e notebook da escola.
- `Assets/Scripts/Editor/Testing/VarginhaDailyBuild.cs`: candidata 0.4.8-beta de 23/10 em pasta nova.
- `Assets/Tests/PlayMode/CampaignChaptersTests.cs`: atualização da referência antiga do armário para a mesa do café, incluindo o ponto de aproximação já usado pelos testes mais recentes. O teste continua exigindo que a chave apareça à frente do móvel que a suporta.
- `Tools/Audio/build_edelzio_casual_20261023.py`: nome definitivo, validação contra os textos aplicados e mapas de voz atualizados.
- `Tools/QA/run_dialogue_feedback.ps1`: execução isolada dos testes e build, com restauração das configurações do projeto e URP.
- `output/pdf/roteiro_edelzio_definitivo.pdf`: três páginas para gravação.
- `output/dublagem-2026-10-23/versao-descontraida/`: TXT, ZIP definitivo, CSV/JSON de IDs e comparação entre os textos.
- Cenas atendidas em runtime: `Ato2_Fase2_A_Chave_e_a_Caixa` e `Ato2_Fase4_Entre_Aulas_e_Pistas`, incluindo a viagem do Fusca ligada à casa. Nenhum arquivo `.unity` de produção ou arte foi editado nesta tarefa.

## Testes realizados

- **29/29 EditMode direcionados**, zero falhas e zero skips: investigação, continuidade do save, apresentação das mensagens e IDs de voz. Evidências em `editmode.xml` e `editmode-summary.json`.
- **3/3 casos no Player Windows**, zero falhas e zero skips, na última execução: casa, investigação da chave, save/reload, viagem de Fusca, escola e retomada; canal de voz com volume e pausa; legenda com teclado/controle e diferentes tamanhos/resoluções. Evidências em `player.xml`, `player-native.xml` e `player-summary.json`.
- A primeira rodada do Player teve 2/3 casos aprovados. O percurso parou porque o teste comparava a chave com `Kitchen_Cabinet`, apesar de o jogo já apoiar a chave em `CoffeeTable_Kitchen`. O teste foi corrigido para o móvel e ponto atuais, e os três casos foram repetidos. A rodada inicial permanece em `player-initial.xml`, `player-initial-native.xml`, `player-initial-summary.json` e `Player-initial.log`; as repetições não foram somadas como casos novos.
- PDF renderizado com Poppler e três páginas inspecionadas individualmente. Presença de todas as 15 falas, códigos únicos, ausência de números falados, IDs recalculados, nome exato solicitado e CRC do ZIP conferidos. Resultados em `output/dublagem-2026-10-23/versao-descontraida/qa-final-R2.json`.

Os testes usam o produto separado `Varginha_QA_Dialogue_20261023`, isolando saves e preferências do jogo que estava aberto. As configurações do projeto e a alteração preexistente de URP são restauradas ao terminar cada execução. As demais alterações locais do usuário foram preservadas.

## Build

Build **0.4.8-beta gerada com sucesso**, zero erros e um aviso, em Unity 6000.6.0f1. O relatório está em `windows-build.json` e o executável em `Builds/Varginha_0.4.8-beta_2026-10-23/SegredoDeVarginha.exe`. O processo de build terminou com código 0 e o runner executou a restauração das configurações no bloco finally. A 0.4.7-beta permanece disponível. A instância já aberta da 0.4.7-beta não recebe alterações de código automaticamente.
