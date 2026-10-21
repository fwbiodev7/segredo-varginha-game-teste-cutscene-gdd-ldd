# Abertura cinematográfica e glitches — 21/10/2026

Escopo autorizado: aprimorar CS03–06 e a chegada à Industrial, preservando o fluxo da missão adulta. A adaptação usa o GDD original, Ato II, o LDD 02–04/4.9 e o roteiro já elaborado. Não expande os capítulos nem implementa os três finais previstos nas fontes.

## O que mudou

- Depósito da chave com gesto, trajetória curta e repouso visível. O desaparecimento permanece fora do campo de observação; não identifica um responsável.
- Caixa e caderno só aparecem depois que o jogador examina e interpreta as pistas. A cutscene não resolve o puzzle.
- Fusca: embarque com porta/partida, Edelzio dirigindo com seus quatro frames originais, exterior em camada atrás das janelas, instrumentos animados e interferência. O mesmo sprite de Edelzio foi ampliado em cerca de 24% e reposicionado no banco. O fundo foi corrigido para aproximar painel e coluna do volante já desenhado no sprite.
- Ignição quente com três poses: segurar, soltar e afastar a mão; chave permanece inserida. Antebraço exposto, coerente com a camisa de manga curta, sem ferimento visível. Motor volta sem reparo que explique a anomalia.
- Desembarque no mesmo mapa da escola usado na fase 4, com caminhada da vaga até o ponto inicial da jogabilidade; o sistema restaura corpo, renderizadores e controle na transição.
- Glitches com focos variáveis distribuídos pela cabine, rasgos de tamanhos/opacidades diferentes, ecos da imagem e partículas verdes/violetas, sugerindo interferência externa sem revelar a entidade. Rajadas breves preservam a leitura; o quadriculado da referência não entra no jogo.
- Pausa congela o relógio das tomadas. Movimento reduzido usa sinal discreto estável e reduz deslocamentos. Cenas já vistas nesse save oferecem pulo, preservando seus estados finais.

## Continuidade preservada

Rotina normal, chave no balcão, mochila, desaparecimento, observação das três pistas, dedução, caixa/caderno, páginas reconstruídas pelo jogador, viagem e escola. A viagem segue: normalidade, rádio/painel em colapso, frase infantil, ignição quente e silêncio, reinício espontâneo, chegada.

A fala “Não deixa ela sair.” continua legendada e com efeitos. A dublagem está adiada por decisão do usuário. Não foram inventados emissor da voz, culpado pelo deslocamento, ferimento, entidade visível ou solução mecânica para a pane.

As divergências de datas, encontros e finais listadas no roteiro pertencem à produção posterior. Mantêm-se os 14 capítulos e seus IDs. A prioridade atual é a abertura já aprovada.

## Arquivos e evidências

Código: `CampaignHouseSequence`, `CampaignStory`, `CampaignFuscaCinema`, `CampaignSignalGlitch`, `CampaignCinemaTextures`, `ExperimentFrameComposer`, `VarginhaCampaignStage` e ajuste de desbloqueio em `VarginhaPlayerActionAnimation`. Assets novos em `Assets/Resources/Varginha/CinemaOpening`; importer específico em `Assets/Scripts/Editor/Testing`.

A documentação técnica, resultados e prévia ficam em `output/cinematic-opening-2026-10-21`. Nenhuma cena `.unity` de produção é regravada. Os assets originais continuam presentes e preservados; o cenário/recorte de mão gerado é adicional.


Redesenho final solicitado: a cabine foi refeita do zero a partir da foto real enviada pelo usuário, em duas camadas: estrutura azul/banco de vinil preto e painel horizontal marrom. O painel é registrado logo atrás do volante original, sem haste longa ou segundo volante. A revisão também substitui o puzzle de Fragmentos, conforme [plano aprovado e implementação](PUZZLE_FRAGMENTOS_BIFURCACOES_2026-10-21.md).
