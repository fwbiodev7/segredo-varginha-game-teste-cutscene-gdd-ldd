# Revisão visual e abertura de 1996 — 21/10/2026

Escopo aprovado nesta conversa: cabine mais cartunesca; mapa de Fragmentos ilustrado; picos de interferência próximos da referência; abertura jornal/TV coerente com as fontes; oito poses da criança e transições fluidas; fase adulta por volta de 7h e cidade baseada na fotografia enviada.

## Continuidade narrativa

Fontes: GDD original, História detalhada, Ato I e Ato II; LDD original, Fase 01 e Fases 02–04, em `Referencias/`. O prólogo permanece em 1996, à noite, com Edelzio de seis anos. A montagem tem 16 segundos: recorte de jornal → noticiário → imagem da transmissão recua até a TV da sala → criança assistindo → título e exploração livre. Só depois entram os gatilhos existentes de energia, luz e quintal.

A frase infantil do rádio não aparece na nova montagem. Nenhuma criatura completa, nave real, selo, guardião ou responsável pela chave é revelado. A reportagem é uma adaptação fictícia e genérica do conteúdo das fontes; não apresenta depoimento histórico como citação real. A construção em forma de nave da fotografia é apenas arquitetura da paisagem urbana diurna.

O antigo storyboard/dados do laboratório experimental ficam preservados. A campanha usa `CampaignOpeningCinema.Shots`, separado daquele protótipo. Saves com abertura já vista continuam na exploração; uma partida nova exibe a revisão. O botão de pular aparece apenas numa repetição, depois de vista a abertura. Pausa, controle, evidências e progresso permanecem independentes das animações.

## Arte e animação

- Cabine e painel novos, com contornos marcados, materiais foscos e sombras mais simples. Mesmos quatro frames originais do motorista, escala e encaixe com o volante.
- Criança sentada: oito poses novas em ciclo de 125 ms por pose, camiseta amarela e bermuda azul, base sentada registrada. Câmera/transmissão usam interpolação suave em tempo real; não são oito imagens da sala trocadas em sequência. O apresentador conserva o sprite e a animação de boca existentes.
- Mapa: terreno, rio e seis marcos ilustrados. Árvore, tronco, ponte, muro, capela e portão substituem os símbolos geométricos. Regras, três bifurcações, solução, prévia, dicas, desfazer, controles e saves não mudaram.
- Glitch: rajadas mais densas com faixas de espessuras diferentes, rasgos verticais, blocos escuros, deslocamentos e cores da referência. O pico é usado no painel/voz e na primeira perda de sinal. Abaixo desse pico, mantém-se a interferência distribuída. Legendas ficam fora da área afetada; movimento reduzido mantém três marcas estáveis e discretas.
- Manhã: apenas os vidros recebem vistas diurnas; os caixilhos, a planta, os móveis e os arquivos de cenário originais permanecem. Iluminação ambiente neutra/quente e material diurno levantam as sombras sem alterar os PNGs originais. O HUD identifica 07:00. A mesma paisagem de Varginha passa atrás das janelas do Fusca.

## Arquivos e cenas

Novos scripts: `CampaignOpeningCinema.cs`, `CampaignAdultMorning.cs`, `CampaignTrailMapArt.cs`. Integração: `VarginhaCampaignPhase1.cs`, `VarginhaCampaignStage.cs`, `CampaignFuscaCinema.cs`, `CampaignCinemaTextures.cs`, `CampaignIllustratedMaps.cs`, `CampaignTrailPuzzle.cs` e `CampaignSignalGlitch.cs`. Importação/build: `CampaignCinematicArtImporter.cs`, `VarginhaDailyBuild.cs`.

Arte nova em `Assets/Resources/Varginha/CinemaOpening/`: `FuscaCabinCartoon`, `FuscaDashboardCartoon`, `Newspaper1996`, `ChildRoom1996`, `ChildWatchingTV`, `MorningTown` e shader `IllustratedMorning`. Terreno e atlas de marcos em `Assets/Resources/Varginha/TrailPuzzle/`. Fontes anteriores e atlas originais de personagem/Fusca não foram sobrescritos.

Cenas afetadas em runtime: `Ato1_Fase1_O_Caso_de_Varginha`, `Ato2_Fase2_A_Chave_e_a_Caixa`, transição para `Ato2_Fase4_Entre_Aulas_e_Pistas` e `Ato3_Fase6_Fragmentos`. Nenhuma cena `.unity` de produção precisou ser regravada. Nenhuma missão ou puzzle adicional foi criado nesta revisão: houve apresentação nova sobre a progressão já aprovada.

Testes e resultados finais: `output/visual-polish-2026-10-21/RELATORIO.md`. Metadados, paletas, contact sheets, overlays e reprodução da skill ficam na pasta `art/skill-exports`. A candidata anterior 0.4.2 permanece histórica.

## Pendências reais

Playtest com pessoa sem conhecer a solução; avaliação artística e do ritmo pela equipe; desempenho/controles/som em outro PC; campanha manual completa. Dublagem permanece adiada por decisão do usuário, sem voz sintética. Cutscenes dos capítulos posteriores seguem o roteiro separado, sem serem anunciadas como concluídas nesta entrega.
