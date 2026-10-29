# ET da perseguição e padre fixo — 27/10/2026

Plano aprovado pelo usuário após inspeção do vídeo de 18,1 segundos e dos scripts/atlas existentes. O vídeo registra a presença na floresta e o padre acompanhando o jogador com o olhar. O código anterior usava âncoras horizontais relativas à célula como se fossem globais: na segunda coluna, anchorX=111,5 e recorte x=280. Isso deslocava a imagem ao trocar os quadros.

Foi criado ForestPresenceSix20261027.png + JSON e metas, com exatamente seis quadros: três de frente/lado e três de costas/lado. Os pixels vêm das linhas 1 e 5, colunas 0, 1 e 3 do atlas existente. A vista posterior foi espelhada para a mesma orientação de base. Sem redimensionar, borrar ou redesenhar a identidade. Original e GUID preservados; fonte SHA-256 8d75ef752f9bcca031e6e11335b2c0b606b1b3ecb4274fc2a5cfe8dc67484487. A aprovação nesta rodada autoriza essa reutilização e o reparo; não se declara aprovação histórica do atlas pelo nome do arquivo.

Identidade observada: silhueta baixa e inclinada, pele marrom/alaranjada, olhos vermelhos, cristas na cabeça e placas escuras com veios azuis. Ambos os grupos mantêm braços e pernas nas três poses de corrida.

Especificação: PNG RGBA 480x320, seis células de 160x160, PPU 100, Point, sem mipmaps/compressão. Apoio no pixel y=144, pivô canônico [80,86], pivô Unity [80,74]. Marcos antigos de meio pixel foram registrados em coordenadas inteiras; pixels da arte não foram reamostrados. Metadados de runtime usam âncora X global correta por coluna. Clips com 167/167/166 ms, ciclo de 500 ms; runtime conserva 6 fps. Dois grupos com espelhamento horizontal para a direita e tolerância para pequenos desvios verticais evitam troca excessiva de vista.

O padre da floresta conserva posição, escala e sprite frontal. Foi retirada apenas sua orientação automática para o jogador. Igreja, diálogos, rotas, velocidade, alcance de captura, pausa, morte e checkpoint não foram alterados.

Testes: {'forest-six-editmode': {'total': '448', 'passed': '448', 'failed': '0', 'skipped': '0'}, 'forest-six-player': {'total': '4', 'passed': '4', 'failed': '0', 'skipped': '0'}}. Os quatro testes de Player incluem pivôs e registro dos seis quadros nas duas orientações/espelhamentos, padre fixo com jogador em quatro posições, escala original na igreja, captura/retry/checkpoint e perseguição com pausa. Validação estrutural strict: seis quadros, dois clips, zero erros/avisos. Atlas nativo, contact sheet e sobreposições último/primeiro foram abertos para revisão visual. O Player executou o percurso e produziu capturas; não se anuncia uma sessão humana com mouse.

Arquivos: CampaignForestPresence.cs, CampaignStorySprites.cs, CampaignExpansionController.cs, novo atlas/JSON/metas, CampaignForestSixFrameTests.cs, CampaignForestReviewPlayTests.cs e Tools/Art/build_forest_six_20261027.py. Cena afetada: floresta, ID interno 7 (fase exibida 5). A igreja, ID 8, foi carregada para regressão.

Fontes e entrega técnica: output/forest-six-2026-10-27/recipe.json, export/atlas.json, export/unity-slices.json, export/sheet.png, preview/index.html, contact sheet, dois APNGs, overlays, paleta e validação. A fonte editável desta reutilização é a receita e o script; não foi criado um arquivo Aseprite. A integração segue o carregador de sprites em runtime do projeto, não slicing automático pelo Sprite Editor.

A build 0.5.1 anterior contém o embaralhamento escolar. O código desta correção está validado para a próxima candidata 0.5.2; seu resultado de build deve ser conferido separadamente em output/forest-six-2026-10-27/windows-build.json.
