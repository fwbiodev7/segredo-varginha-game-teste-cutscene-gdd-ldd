# O Segredo de Varginha — 0.4 beta

Windows x64, Unity 6000.6.0f1, versão interna `0.4.0-beta`. A build mantém as 14 fases da campanha e suas áreas adicionais.

Inclui o laboratório moderno aprovado pelo usuário, com ET na incubadora, recortes, colisões e proporção ajustada; menu cinematográfico com névoa, céu corrigido, Fusca azul e OVNI transparente que parte rapidamente; hover suave compartilhado; e novo selo místico para a **saída da criatura do nosso mundo**.

O selo tem 24 quadros: abertura, sustentação a 12 fps e fechamento. Runas ciano, coroa de três pontas, detalhes vermelhos e energia convergindo para o centro transparente. Ele acompanha a travessia, fica atrás da criatura e respeita pausa, movimento reduzido e retomada do save. Foram preservados puzzles, diálogos e progressão.

Extraia todo o ZIP `jogo_varginha_beta_0.4_Windows_x64.zip` e execute `jogo_varginha_beta_0.4.exe`. Mantenha `_Data` e bibliotecas ao lado do executável. **Continuar** retoma seu progresso; uma nova história substitui o save local.

Para reproduzir: no Unity, **Varginha → Build → Beta 0.4 Windows x64**; depois execute `Tools/PackageAlpha.py --beta`. O pacote e os executáveis ficam fora do Git, conforme `.gitignore`; o manifesto registra tamanhos, SHA-256, cenas e verificação CRC.

Arquivos principais: `CampaignReturnSeal.cs`, `CampaignContinuationController.cs`, `ReturnSealV1.png/json`, `MenuCinematicAtmosphere.cs`, `MenuAtmosphere.shader`, `MenuUfoFlight.cs`, `VarginhaMainMenu.cs`, `PixelButtonHover.cs`, `PixelButtonHoverUGUI.cs`, mapas e `Layouts.json`, `VarginhaAlphaBuild.cs`, `ProjectSettings.asset` e `PackageAlpha.py`.

Cenas afetadas em execução: `Menu_MisterioDeVarginha`, `Ato6_Fase18_A_Criatura_Ferida` (fase 12 na ordem da campanha) e `Ato6_Fase21_O_Retorno`. O hover compartilhado também atende HUD, mochila e demais botões existentes.

Testes registrados nesta entrega: 11 Play Mode do menu, três Play Mode do selo, um Play Mode do laboratório, dois Edit Mode de suspense, um Edit Mode de hover e um Edit Mode do voo do OVNI. Todos aprovados. Cobrem controles, transparência, saída do OVNI, transições, física do laboratório, puzzle, pausa, travessia e recarregamento. A regressão Edit Mode `BattleCalibrationAndCrossingRemainIndependentAndPersist` também passou. Isso não representa uma jogada completa de todas as fases nem medição de desempenho em outro computador.

O Unity concluiu a build sem erros, com um aviso sobre desativar a automação do Editor no executável, descrito em `build-diagnostics.json`. O executável permaneceu ativo no menu por 20 segundos sem erros de inicialização. O ZIP passou na verificação CRC.

Confira os resultados JSON, o manifesto da build, a conferência de inicialização do executável, [a prévia do selo](SeloRetorno.gif), [a travessia na fase](PortalRetorno.png) e [o menu](Menu.png).

Referências pesquisadas para o movimento e as runas: [Animated Portals, Heosphorus](https://heosphorus.itch.io/animated-portals) e [Casting Sigils, H7PixelForge](https://h7pixelforge.itch.io/pixel-art-casting-sigils-formula-circles-imprints-vfx-pack). Arte original gerada com imagegen e a skill `segredo-varginha-pixelart`; nenhum asset desses autores foi reutilizado.
