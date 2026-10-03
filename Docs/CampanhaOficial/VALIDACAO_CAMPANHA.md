# Validação da campanha remasterizada

Data: 3 de outubro de 2026. Unity 6000.6.0f1; renderização Built-in.

## Estado atual

Compilação confirmada sem erros. EditMode passou **33/33**, incluindo caminhos livres até todos os objetivos, limites das plantas, bases de colisão dos móveis e uso das novas artes da infância, igreja e casa/laboratório. Registro: [EditMode atual](C:/Users/Usuario/segredo-varginha-game-teste-cutscene-gdd-ldd-main/Docs/CampanhaOficial/Validacao20261003EditMode.json).

O clarão passou **2/2 testes PlayMode**: explosão branca/verde, recuo físico, queda e contato da cabeça, pose deitada estável, pausa, bloqueio de controle durante a sequência e memória perdida salva. Registro: [PlayMode do clarão](C:/Users/Usuario/segredo-varginha-game-teste-cutscene-gdd-ldd-main/Docs/CampanhaOficial/Validacao20261003ClaraoPlayMode.json). Capturas reais: [explosão](C:/Users/Usuario/segredo-varginha-game-teste-cutscene-gdd-ldd-main/Preview/CampaignMapsV2/Clarao_Explosao.png) e [criança caída](C:/Users/Usuario/segredo-varginha-game-teste-cutscene-gdd-ldd-main/Preview/CampaignMapsV2/Clarao_Crianca_Caida.png).

Após os últimos ajustes, a execução completa da campanha passou **8/8 PlayMode** em 91,57 segundos. Inclui casa infantil, rotina e caixa da casa adulta, escola/fachada, separação de Renan durante todo o intervalo, condução do Fusca e carregamento das fases 6–10. Registro: [PlayMode da campanha](C:/Users/Usuario/segredo-varginha-game-teste-cutscene-gdd-ldd-main/Docs/CampanhaOficial/Validacao20261003CampanhaPlayMode.json). O cache do executor foi renovado por recompilação antes dessa execução; tentativas anteriores que selecionaram zero testes não foram consideradas aprovação.

As nove plantas reconstruídas e a casa adulta foram exportadas novamente. A igreja mede 14×10 unidades e a casa/laboratório mede 16×10; seus móveis usam escala de origem comum, com o órgão ampliado em 25%. Foram conferidos os limites visuais e os caminhos de interação. A casa adulta original salva, a fachada real da escola, o Fusca e os personagens da equipe permanecem preservados.

## Kit para organização manual

O pacote local contém **10 plantas limpas**, 10 referências mobiliadas, **161 PNGs isolados** e posições para 329 instâncias. A rua da fase 3 foi exportada inteira. Validação dos arquivos: dimensão da planta igual à referência e ao manifesto em todas as fases; móveis RGBA com transparência; dimensão de cada instância igual ao PNG correspondente; zero inconsistências. Escala comum de montagem: 48 pixels por unidade, importação das camadas a 100%.

[Galeria e instruções do kit](C:/Users/Usuario/segredo-varginha-game-teste-cutscene-gdd-ldd-main/Docs/CampanhaOficial/KIT_MAPAS_MANUAL.md). O exportador faz capturas temporárias e restaura os renderizadores; não salva modificações nas cenas originais. Os arquivos de progresso do usuário foram restaurados após as capturas. A pasta `Entregas/` fica fora do Git; o exportador e os assets usados estão versionados.

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
