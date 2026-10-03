# Validação da campanha remasterizada

Data: 2 de outubro de 2026. Unity 6000.6.0f1; renderização Built-in.

## Estado salvo após o feedback — trabalho interrompido a pedido do usuário

Compilação do estado atual confirmada sem erros. A última execução EditMode concluída passou **32/32**; o registro completo preservado da execução imediatamente anterior contém **31/31** em [feedback-EditMode.json](C:/Users/Usuario/segredo-varginha-game-teste-cutscene-gdd-ldd-main/Docs/CampanhaOficial/Validacao/feedback-EditMode.json). O teste adicional da proporção/colisão dos móveis infantis e os últimos ajustes dessas bases ainda precisam de uma execução final.

O teste isolado da casa adulta passou: personagem menor que a geladeira, texturas pedidas, tapetes proporcionais, portas removidas, travessia física dos dois portais e rotina salva em 14 completada pela higiene, liberando a caixa.

A última execução completa PlayMode passou **7/8**, com uma falha na verificação da troca da fachada após teletransportar o Rigidbody, antes de aguardar um passo de física. O teste agora aguarda esse passo. As novas rotas dos alunos reservam a silhueta de Renan e têm teste de caminho aprovado; a nova verificação durante o intervalo ainda não foi concluída. Registro: [feedback-PlayMode-parcial.json](C:/Users/Usuario/segredo-varginha-game-teste-cutscene-gdd-ldd-main/Docs/CampanhaOficial/Validacao/feedback-PlayMode-parcial.json).

Os resultados abaixo pertencem ao remaster anterior. Não constituem aprovação dos últimos ajustes. As prévias da casa e dos atlas foram capturadas antes da última revisão de junções/pivôs; sua regeneração também fica pendente. Alterações, imagens e prompts estão em [CORRECOES_CASA_E_MOCHILA_V3.md](C:/Users/Usuario/segredo-varginha-game-teste-cutscene-gdd-ldd-main/Docs/CampanhaOficial/CORRECOES_CASA_E_MOCHILA_V3.md).

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
