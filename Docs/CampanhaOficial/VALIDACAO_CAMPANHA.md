# Validação da campanha remasterizada

Data: 2 de outubro de 2026. Unity 6000.6.0f1; renderização Built-in.

## Resultado final

- Reconstrução das plantas e, depois, da mobília: duas execuções concluídas com código 0, sem erro de compilação.
- EditMode: **26/26 passaram**. Rotas até todos os objetivos nas nove fases reconstruídas, correspondência entre planta e colisões, progresso, salvamento, recortes dos sprites e preservação da escola original.
- PlayMode: **7/7 passaram**. Carregamento das fases 6–10, portas e paredes físicas, campanha salva, alunos sem sobreposição com Renan, fachada externa/interna, animações de ação e soco e condução do Fusca até o fim da pista.

Resultados completos: [EditMode](C:/Users/Usuario/segredo-varginha-game-teste-cutscene-gdd-ldd-main/Docs/CampanhaOficial/Validacao/atmosferas-EditMode.xml) e [PlayMode](C:/Users/Usuario/segredo-varginha-game-teste-cutscene-gdd-ldd-main/Docs/CampanhaOficial/Validacao/atmosferas-PlayMode.xml). Filtros usados: `Game.Tests.EditMode.Campaign` e `Game.Tests.PlayMode.Campaign`.

## Preservação

Os arquivos abaixo têm o mesmo conteúdo Git que `origin/main` (`1f9e5b9`):

- `Assets/Scenes/Ato2_Fase2_A_Chave_e_a_Caixa.unity`
- `Assets/Scenes/FaseTopView_Varginha.unity`
- `Assets/Resources/Varginha/Experiment/IndustrialFacade.png`
- `Assets/Resources/Varginha/Experiment/FuscaTopView.png`
- `Assets/Resources/Varginha/fusca-sprite-sheet.png`

## Limite da validação geral

Uma execução anterior, com filtro amplo aplicado por engano, também incluiu testes do protótipo: EditMode 156/159 e PlayMode 180/192. Duas falhas da campanha foram corrigidas e passaram no resultado final acima: destino de teste da escola ajustado ao ponto real da aula; compositor antigo impedido de substituir a animação nova durante o soco.

As outras 13 falhas ficaram fora da revisão visual da campanha. Não foram comparadas com uma execução limpa de `origin/main`, portanto não estão classificadas como preexistentes. A suíte geral não pode ser declarada aprovada. Elas envolvem:

- `VarginhaVisualPolishTests.EveryAttackHasDedicatedWindupContactAndRecoveryWithStableGeometry`
- `VarginhaVisualPolishTests.ProtagonistIsLargerWithoutMovingHisFootBaseline`
- `EdelzioPresentationTests.ActionsKeepNewAppearanceAndColliderSize`
- `EdelzioPresentationTests.BackpackStateControlsIntegratedAppearanceInEveryDirection`
- `EnemyAITests.Enemy_GoesToChase_WhenPlayerDetected`
- `PhysicsTests.Rigidbody2D_FallsWithGravity`
- `VarginhaInteractionTests.ClassroomETAttacksCaptiveCageAndRescueStopsThePressure`
- `VarginhaInteractionTests.PortraitUsesTheSpeakersCurrentAtlas (Edélzio e Padre Fábio; dois casos)`
- `VarginhaInventoryTests.BackpackOwnsPauseAndDoesNotOpenWithoutBackpackOrDuringDialogue`
- `VarginhaInventoryTests.NoStudentAppearsOrAttacksUntilEquipped`
- `VarginhaMenuAndDeathTests.DeathClosesPauseAndWaitsForAnExplicitChoice`
- `VarginhaRequestedGameplayTests.ChurchHasDarknessAltarPriestAndClearFlashlightCone`

Registros dessa execução anterior permanecem localmente em `Logs/remaster-EditMode.xml` e `Logs/remaster-PlayMode.xml`. As prévias foram conferidas visualmente; os testes automáticos não substituem uma partida manual completa.
