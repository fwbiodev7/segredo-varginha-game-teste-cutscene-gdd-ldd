# A chave desaparecida — abertura adulta (20/10/2026)

## Referências e continuidade

Fonte principal: `Referencias/GDD_Usuario.md`, Ato II. Edelzio é professor, tem 36 anos em 2026 e reencontra o caderno de 1996. É Edelzio quem coloca a chave **do Fusca** no balcão. O desaparecimento não revela o responsável nem explica a entidade. Padre Fábio e Ouzana conservam suas entradas posteriores; Renan participa da investigação na escola, não da casa vazia.

A solicitação aprovada de 20/10 restabelece uma preparação curta e jogável na abertura, substituindo a decisão anterior de tornar toda a rotina opcional. Não restaura fases removidas nem três inspeções obrigatórias do carro. Saves com a caixa ou as páginas já obtidas conservam seu progresso. A inconsistência 1898/1966 nos documentos originais não foi propagada: esta missão não introduz outra data para Zé Gomes.

## Percurso

1. Movimento e interação contextual levam à pia, café e material de aula no notebook, usando as animações existentes.
2. No balcão, Edelzio estende a mão e deposita a chave. O objeto permanece visível e salvo enquanto ele busca a mochila.
3. Ao pegar a mochila no quarto, um som metálico e papel raspando anunciam o desaparecimento fora de vista. A câmera retorna ao balcão vazio. A mudança é persistida antes de terminar a tomada.
4. O jogador observa balcão, trinco de saída e papel envelhecido junto ao quarto. O caderno registra cada observação. Reunir as pistas não basta: é necessário interpretar a relação entre saída fechada e papel da infância. Hipóteses incorretas não consomem pistas.
5. Arranhões indicam a caixa sob a cama. Edelzio se abaixa; a chave e o caderno reaparecem dentro da caixa. A anotação infantil é “Eu sei que vi alguma coisa.”
6. Três páginas rasgadas têm desenho contínuo, muro e galhos. O jogador troca suas posições para reconstruir casa → árvore → figura. O texto recente “ELA AINDA ESTÁ AQUI” e o primeiro fragmento são revelados na conclusão.
7. O Fusca usa o sprite original, abertura da porta, embarque e partida. Tomadas de rua, rádio, painel, pane, silêncio, reinício e chegada à Industrial devolvem o jogador à escola e ao fluxo existente com Renan.

## Dicas e controles

As instruções usam os rótulos das configurações atuais de teclado/controle. A preparação ensina interação; caderno e mochila são apresentados no contexto. F1 e a página de dicas fornecem orientação, pista mais específica e solução identificada como spoiler. Na investigação, o HUD aponta as áreas a observar, mas não escolhe a hipótese pelo jogador. Pausa congela as animações e o áudio; movimento reduzido remove a movimentação da câmera e a rolagem da rua.

Revisão de 21/10: cada etapa da preparação, depósito da chave, busca da mochila, dedução, caixa, páginas e saída possui três dicas distintas. Consultá-las não modifica o estado da missão. A dublagem infantil foi adiada pelo usuário; a cutscene continua com legenda e efeitos.

## Estado e arte

`CampaignHouseInvestigation` serializa etapa, pistas, interpretação e tutorial dentro de `CampaignStory2026.json`. A chave não pode duplicar ao recarregar; dados antigos com evidências adquiridas são normalizados. Os IDs das cenas e os 14 capítulos continuam estáveis.

Arte nova: `Assets/Resources/Varginha/KeyInvestigation/HouseEvidence.png` + JSON, oito células 128×128, atlas 512×256, PPU 128, alpha binário, filtro Point, sem mipmaps ou compressão. Fonte gerada, crops, prévias e hashes ficam em `output/key-investigation-2026-10-20/art`. O script reproduzível de montagem é `Tools/KeyInvestigationArt/build_art.py`. As fontes originais da casa, do Fusca e dos personagens não foram alteradas.

A skill `pixel-art-sprite-sheet-master` orientou referência, identidade, registro e QA. Nesta instalação, `scripts/sprite_pipeline.py` e três referências citadas pelo SKILL.md estavam ausentes; a montagem e validação equivalentes foram implementadas no projeto. A fonte nova foi normalizada separadamente, com padding, redução inteira nearest-neighbor, paleta limitada e alpha binário. A revisão visual não equivale à aprovação artística do usuário.

## Áudio e limites

Há efeitos de chave, papel, arranhões, interferência, motor e partida. A fala infantil é legendada. Uma gravação real opcional pode ser adicionada em `Resources/Varginha/Audio/EdelzioCrianca_NaoDeixaElaSair`; nenhuma voz sintética foi criada. As tomadas internas reutilizam a arte de `FuscaBreakdown`. O trecho de viagem é cinematográfico e não adiciona uma fase obrigatória de direção.

## Verificação

Testes de estado: `CampaignHouseInvestigationTests`. Percurso completo: `CampaignChaptersTests.HouseDriveSchoolAndCodeFormAPlayableSavedCampaign`, incluindo infância, preparação, save/reload da chave, investigação, páginas, mochila, Fusca e pistas da escola. `CampaignOpeningAudit` executa o mesmo percurso fora do executor PlayMode caso a infraestrutura do editor falhe durante a recarga de domínio. Resultados finais e limitações estão em `output/key-investigation-2026-10-20/RELATORIO.md`.

A continuação e a verificação em sessão limpa de 21/10 estão em `output/qa-2026-10-21/RELATORIO.md`. A skill de pixel art está completa nesta sessão: 30 testes dos auxiliares e validação dos oito frames concluídos, sem alterar seus pixels. Ver `output/qa-2026-10-21/ARTE.md`. O roteiro para pessoas da equipe está em `Docs/CampanhaOficial/PLAYTEST_2026-10-21.md`.
