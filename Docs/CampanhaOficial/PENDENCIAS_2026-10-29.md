# Pendências adicionais — 29/10/2026

Pedidos recebidos após a entrega e conferência da build 0.5.3-beta. Retomada automática iniciada em29/10 após confirmar a renovação real (0% usado). Baseline Git `f73cae2`; alterações anteriores preservadas.

| Item | Estado atual | Evidência disponível |
| --- | --- | --- |
| Fiat Toro | Parcial | Variante azul passou na cutscene em720p/1080p; orientação humana posterior: tornar **branca e mais alta**. Nova arte em preparação, sem substituir o Fusca. |
| Pia | Concluído | Pés(605,708), alcance0,65m, chegada validada, percurso real/ação completa/retorno do controle e reload aprovados; captura conferida. |
| Café | Concluído | Abordagem norte(705,530), alcance0,65m, consumo/reload e cancelamento rápido sem consumo após obstrução física aprovados. |
| Notebook | Concluído | Aproximação(86,554), alcance0,65m, encaixe0,12m; sentar, usar, levantar, movimento e reload aprovados; captura conferida. |

Implementação e arte em `output/toro-casa-2026-10-29/`; a build0.5.3 e seus ZIPs continuam preservados. Nenhuma nova candidata foi entregue até este registro.

## 1. Fiat Toro na passagem com música

- Substituir somente o veículo que passa com a música “O Edelzio”, na chegada externa à escola, por uma **Fiat Toro reconhecível como picape**.
- Orientação posterior do usuário durante a execução: **Toro branca e mais alta**, com carroceria elevada e vão livre coerente, sem esticar as rodas.
- Basear a arte integralmente na linguagem visual do Fusca original aprovado: paleta, tamanho dos pixels, contornos, sombreamento, perspectiva e acabamento. A referência é o asset original do projeto.
- Preservar o Fusca do protagonista e seus arquivos originais. A Toro será um asset separado, usado apenas nessa passagem.
- Manter a música e a passagem já aprovadas: trajetória rápida, sincronização, áudio espacial, aproximação/afastamento, Doppler sutil, volume, pausa e limpeza das fontes.
- Conferir enquadramento, rodas inteiras, recorte nas bordas e separação do Fusca estacionado e do personagem em 1280×720 e 1920×1080.
- Pontos de integração: `CampaignFuscaCinema.cs` (`PassingCarRect` e desenho da passagem), `CampaignSchoolMusicCar.cs` e referência `CampaignOriginalFusca.Side`. Não trocar globalmente o sprite do Fusca.

## 2. Aproximar Edelzio da pia ao lavar o rosto

- A captura enviada mostra Edelzio lavando o rosto com um espaço visível entre o corpo/mãos e a pia.
- Reposicionar o ponto de aproximação e o encaixe da animação para ele ficar junto à borda da pia, com orientação e mãos coerentes com o uso da torneira.
- Preservar a colisão sólida do móvel; aproximar o personagem sem atravessar a pia, a parede ou os objetos próximos.
- Conferir chegada com movimento real, execução completa, saída da animação e retorno do controle, inclusive após save/reload.
- Inspecionar `CampaignAdultHouse.WashApproach`, o ponto `wash` do layout da casa adulta e a ação de rotina em `VarginhaCampaignStage.cs`.

## 3. Interação do café mais próxima

- Ajustar a hitbox/área de ativação e o ponto de aproximação para beber café junto à caneca e à mesa, evitando iniciar a ação de longe.
- Manter o objeto visível e a animação alinhada, sem interação através de móveis ou paredes.
- Preservar consumo, progressão da rotina, ordem da investigação e persistência.

## 4. Interação do notebook mais próxima

- Ajustar a hitbox/área de ativação e o ponto de aproximação para usar o notebook junto à escrivaninha, com encaixe coerente na cadeira e orientação para a tela.
- Evitar ativação à distância ou através da mesa/parede, preservando um acesso confortável e sem travar o personagem ao levantar.
- Preservar animações, colisões, estados da rotina, save/reload e as interações investigativas posteriores.

Para café e notebook, inspecionar os pontos de rotina `food`/`work`, seus objetos e alcances em `VarginhaCampaignStage.cs`, `CampaignAdultHouse.cs` e `CampaignHouseRoutineItems.cs`; distinguir esses pontos do notebook investigativo da escola.

## Referência e validação futura

Captura fornecida pelo usuário: `C:/Users/Usuario/AppData/Local/Temp/codex-clipboard-fa35393c-d356-4b3c-a0b6-1a9faac3bd63.png`.

`toro-house-player-1.xml`:5/5 aprovados, incluindo percurso real da rotina, consumo/reload, gates e cutscene nas duas resoluções com a primeira Toro azul. `house-close-cancel-2.xml`:3/3 aprovados com a correção final de cancelamento, em Player. `QA-CASA.md` registra a conferência das três capturas. Commit doméstico `b8e55ad`. A variante branca/mais alta ainda deve passar na cutscene e nas verificações de arte; build0.5.4 pendente.
