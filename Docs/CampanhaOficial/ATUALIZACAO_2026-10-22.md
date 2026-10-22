# Revisão 0.4.5-beta — interface, rotina adulta e cutscenes — 22/10/2026

A preparação adulta agora altera os objetos do cenário: beber consome o café, concluir o notebook retira o objeto da mesa e os estados persistem no save. A chave maior fica na mesa do café, com pistas, marcador e caderno coerentes com o novo local. Ações interrompidas não concluem tarefas nem consomem o café. As poses usam a escala visual do Edelzio sem redimensionar os colliders.

Os avisos de coleta/ações mantêm o fundo pixel art em tamanho menor, desaparecem automaticamente e não exigem Continuar. Falas usam legendas compactas e progressivas; a confirmação primeiro revela o texto e depois avança. Leituras essenciais, documentos e escolhas continuam em painéis próprios. O canal de voz possui volume independente, pausa e IDs estáveis para receber as gravações humanas.

Os móveis voltados para baixo no mapa receberam vistas traseiras. A variante da imagem-base elimina as frentes antigas, incluindo a torneira e os pés que excediam os recortes. Xícara, notebook e luminária usam transparência real. Grama e plantas foram harmonizadas com os verdes oliva das árvores, preservando textura, caminhos e colisões.

O Fusca usa camadas registradas de cabine, painel/instrumentos, coluna fosca e Edelzio com volante. O velocímetro fica atrás do volante nos quatro frames. A chegada à escola usa fundo opaco em toda a área cinematográfica para impedir que a casa apareça nas margens.

## Arquivos e cenas

- Interface: `CampaignMessagePresentation.cs`, `CampaignPickupNotice.cs`, `CampaignTimedSubtitle.cs`, `CampaignDialogueVoice.cs`, `TypewriterText.cs`, `VarginhaGameHUD.cs` e pontos de chamada em controladores/itens.
- Rotina e investigação: `CampaignHouseRoutineItems.cs`, `CampaignHouseFurniture.cs`, `VarginhaCampaignStage.cs`, `VarginhaPlayerActionAnimation.cs`, `VarginhaHouseComposition.cs`, `CampaignTeamEdelzio.cs`, `CampaignHouseSequence.cs`, `CampaignHouseInvestigation.cs`, `CampaignJournal.cs` e `CampaignIllustratedMaps.cs`.
- Arte: `Assets/Resources/Varginha/HouseFurniture/`, `IllustratedMorning.shader` e `CampaignHouseFurnitureImporter.cs`. Fontes, máscaras e receitas em `output/house-interactions-2026-10-22/`.
- Cinema: `CampaignFuscaCinema.cs` e `CampaignHouseSequence.cs`; testes em `CampaignCinematicOpeningPlayTests.cs`.
- Áudio: `CampaignSoundscape.cs` e filtro de canais em `VarginhaGameSettings.cs`.
- Documentos: `Dublagem_2026-10-22/`, `output/dublagem-2026-10-22/` e daily HTML de 22/10 no diretório existente `output/daily-sprint-2026-10-20/`.
- Cenas afetadas em runtime: `Ato2_Fase2_A_Chave_e_a_Caixa`, viagem do Fusca e `Ato2_Fase4_Entre_Aulas_e_Pistas`; o componente compartilhado de mensagens também atende abertura, expansão e continuação. Nenhum arquivo de cena foi regravado para essas alterações.

Esta revisão conclui a rotina e a apresentação das missões existentes. Não adiciona uma missão ou puzzle novo; mantém a progressão aprovada e o mapa de bifurcações de Fragmentos.

## Verificação

- **398/398 EditMode** na suíte completa final, com gráficos ativos.
- **Build Windows 0.4.5-beta gerada**, zero erros e um aviso preexistente do pacote opcional `Unity.Pipeline`; o URP 2D permanece ativo. A pasta anterior 0.4.4 não foi substituída. Guia, manifesto SHA-256 e verificação CRC acompanham o ZIP.
- **16/16 casos únicos no Player Windows**, tomando a última execução de cada caso, sem somar repetições como casos novos. A rodada de interface/cinema passou 11/11; os ajustes finais de arte, opacidade e ordenação tiveram retestes direcionados.
- Cobertura: lavagem, café, notebook, interrupção, persistência após reabrir, posição da chave, colisões dos sete móveis, avisos sem modal, teclado/controle, tamanhos de legenda, canal de voz e pausa, abertura de 1996, Fusca, chegada e retorno do controle, áudio e apoios.
- **10 slides**, nove transições, imagens/fontes, navegação, notas, impressão e movimento reduzido aprovados. Revisão visual do overview realizada.
- Roteiros Word/PDF renderizados pelo Microsoft Word e conferidos em imagens: 18 páginas no completo e dez no Edelzio, sem páginas vazias. São 153 entradas: 47 existentes, 98 propostas para cenas atuais e oito esforços vocais. O separado do Edelzio inclui 78 entradas de adulto e criança.
- Saves, preferências e ProjectSettings originais restaurados após o término dos Players. A alteração anterior do usuário em `UniversalRenderPipelineGlobalSettings.asset` e os arquivos de recuperação foram preservados fora do commit.

As propostas **P** do roteiro ainda dependem de revisão antes da integração. Dublagem humana e playtest externo não foram recebidos. Detalhes e evidências em `output/house-interactions-2026-10-22/RELATORIO.md`; pendências em `PENDENCIAS_2026-10-22.md`.
