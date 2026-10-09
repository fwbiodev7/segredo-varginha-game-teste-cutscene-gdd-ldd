# Feedback de 09/10/2026 — Etapa 2

Autorização recebida após a análise, incluindo os ajustes manuais.

- Menus: navegação vertical por linhas nas configurações, foco inicial na aba correta, seleção por mouse, destaque com teclado/controle, incremento horizontal único vinculado ao slider selecionado. Mantidos os bindings A/B, teclado, mouse e PlayerPrefs.
- Transição casa → Industrial: apresentação progressiva do texto existente; o aparecimento da frase inicia a estática e uma interferência de 0,65 s, com um único clarão suavizado. Legendas ficam por cima do efeito. Reduzir distorções suprime o clarão e os glitches e reduz o som. Pausa congela o relógio e suspende o áudio. O botão Continuar não apaga o diálogo durante a transição automática.
- Colisões: 38 polígonos escolhidos manualmente nos pontos de apoio de cadeiras, vasos, troncos e bases curvas. 63 paredes têm limites explícitos, sem expansão lateral automática, sobretudo batentes. Os mesmos polígonos são usados na construção física e na consulta de circulação. A arte, os contornos visuais, pontos de interação e puzzles foram preservados.
- A escada, as colunas de Fragmentos, a vegetação sobre mesas e os objetos decorativos já corrigidos mantêm suas definições existentes. O Fusca mantém BoxCollider2D, necessário ao sistema de estacionamento.

Arquivos de jogo: VarginhaGamepadUI.cs, VarginhaGameSettings.cs, VarginhaCampaignStage.cs, CampaignSoundscape.cs, CampaignIllustratedMaps.cs, CampaignMapPlan.cs e Layouts.json.

Layouts afetados: 1, 2, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 18, 19, 112 e 121. As cenas consomem esses dados ao carregar; nenhum arquivo .unity ou prefab foi reescrito. O inventário exato está em CollidersManuais.json. Não há prefabs no projeto atual.

Esta etapa registra a implementação. Resultados de validação devem ser consultados no relatório da Etapa 3; implementação não comprova ausência de bugs.
