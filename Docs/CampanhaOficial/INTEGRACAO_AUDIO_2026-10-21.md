# Integração dos sons recebidos — 21/10/2026

Implementação autorizada após o [plano e as correções autorais](PLANO_AUDIO_RECEBIDO_2026-10-21.md). A candidata 0.4.4-beta acrescenta gravações aos eventos já existentes, preservando arte, diálogos, puzzles e valores de combate.

## Eventos integrados

- Renan: `glitch_hit_variation1` no disparo do pulso, uma vez por invocação.
- Ouzana: vidro quebrando no fim do arremesso, uma vez; os ticks da nuvem não repetem a quebra.
- Padre Fábio: pad angelical durante a proteção na luta final, com pausa e término controlados; separado dos sinos da igreja.
- Infância: explosão no lançamento do encontro inicial. O efeito não foi atribuído ao surto do chefe.
- Combate: soco no impacto confirmado; rugido, ataque e colapso da manifestação nos gatilhos existentes.
- Exploração: três variantes de passos na madeira e três na terra; escola normal, floresta noturna, TV, interferência, motor, falha de partida, porta, sino e alarme do cerco.

A casa adulta às 7h não recebe grilos noturnos. Escola invadida mantém o ambiente de tensão, sem a gravação de escola movimentada. Efeitos sem equivalente recebido continuam com o fallback anterior.

## Preparação e reprodução

O pacote contém 20 MP3 e um M4A; duas cópias de metal-crack são idênticas. Vinte fontes únicas deram origem a 25 WAV e quatro aliases. Cortes, hashes e atribuições estão nos [créditos e manifesto](CREDITOS_AUDIO_RECEBIDO_2026-10-21.md).

`CampaignRecordedAudio` resolve os recursos por catálogo e mantém cache. Efeitos curtos usam PCM descomprimido; camas e loops usam Vorbis em streaming. `CampaignSoundscape` reaproveita fontes para efeitos, som posicional, motor e escudo. O ambiente conserva o crossfade/ducking existente. Motor e escudo respondem ao controle de efeitos, sem disputar a automação de volume com o controlador global.

A reprodução da proteção acompanha o tempo ativo da animação. A fonte persistente de socos pausa com o jogo e encerra a cauda ao mudar de cena. O pacote não acrescenta dublagem sintética.

## Cenas e arquivos

As cenas são afetadas pelo código compartilhado; nenhum arquivo `.unity` precisa ser regravado. Pró­logo/infância, casa adulta, viagem do Fusca, escola normal, mata, igreja, cerco e arena final recebem os eventos correspondentes. O catálogo também atende os ataques da manifestação nas fases que já os utilizavam.

Principais arquivos: `CampaignRecordedAudio`, `CampaignSoundscape`, `CampaignAtmosphereAudio`, `CampaignFlashEncounter`, `CampaignHouseSequence`, `CampaignFinalAllies`, `VarginhaCampaignPhase1`, `CampaignContinuationController`, `VarginhaPlayerAttack`, `VarginhaGameSettings`, importador e testes. Os novos WAV ficam em `Assets/Resources/Varginha/Audio/Received20261021/`.

Resultados verificáveis, build e capturas: [relatório da entrega](../../output/audio-integration-2026-10-21/RELATORIO.md). A auditoria de design dos apoios está documentada separadamente; integração de áudio não significa aprovação de balanceamento ou correção automática de alcance.
