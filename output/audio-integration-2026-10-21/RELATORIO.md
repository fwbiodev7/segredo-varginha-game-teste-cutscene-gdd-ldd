# Entrega 0.4.4-beta — áudio e apoios

Integração e correções aprovadas pelo usuário, seguidas do pedido explícito de novas artes e publicação no GitHub. [Resumo verificável](summary.json).

## Resultado

- **25 WAV, 20 fontes únicas e quatro aliases**; todos os arquivos recebidos têm uso documentado. Glitch variation é Renan; vidro é Ouzana; angelical é escudo do Padre; explosão é infância. [Fontes, cortes e créditos](../../Docs/CampanhaOficial/CREDITOS_AUDIO_RECEBIDO_2026-10-21.md).
- **24 desenhos novos, oito por apoio**, no atlas `SupportEffectsV2.png`. Renan cyber com glitch no alvo atingido; frasco/nuvem verde da Ouzana; contorno dourado transparente do Padre. [Fonte, receita, slicing, prévia e revisão](art/technical-report.md).
- Corrigida a perda de tempo da proteção durante mensagem modal; legenda do apoio reposicionada; efeitos alinhados ao alcance e cúpula encaixada nos personagens. Dano, recarga, personagens, puzzles e narrativa foram preservados. [Auditoria e comparação](../../Docs/CampanhaOficial/AUDITORIA_APOIOS_2026-10-21.md).
- Cada apoio usa um objeto de efeito, sem ParticleSystem; pulso cyber tem um mesh com teto de 256 quads/seis alvos. Foram verificados orçamento de 208 quads com seis alvos, três apoios simultâneos, limpeza e emissão sonora única. [Medições no Player](support-audit.txt).

## Testes e build

| Verificação | Resultado | Evidência |
|---|---|---|
| EditMode final | **389/389** | [XML](editmode-accepted.xml), [resumo](editmode-summary.json) |
| Player Windows, rodada ampla | **10/10** | [XML](player-full-before-shield-position.xml), [resumo](player-full-summary.json) |
| Player Windows após encaixe da cúpula e guarda de pausas combinadas | **4/4** | [XML](player-native.xml), [resumo](player-summary.json) |
| Integridade/nível dos WAV | **25/25** | [waveform-qa.json](waveform-qa.json) |
| Reprodução da prévia dos VFX | **4 clipes** | [playback-qa.json](art/preview-final/playback-qa.json) |
| Daily | **10 slides, nove transições**, imagens, notas, layout, impressão e movimento reduzido | [slides-qa.json](slides-qa.json), [visão geral](slides-overview.png) |
| Build Windows x64 | **Succeeded, zero erros, um aviso** | [windows-build.json](windows-build.json) |
| Pacote | **CRC/manifesto conferidos; sem assemblies de testes** | [windows-package.json](windows-package.json) |
| Preservação | **Três saves e duas configurações restaurados byte a byte** | [preservation.json](preservation.json) |

As rodadas de dez e quatro casos são distintas: não representam 14 testes únicos. A rodada ampla cobre abertura de 1996, Fusca, aluno/apoio no combate e áudio; o reteste cobre os quatro casos de áudio/apoios após subir a cúpula. Falhas intermediárias permanecem nos XML/resumos anteriores: API do importador ajustada à Unity 6, contratos antigos atualizados para gravações, conflito entre SpriteRenderer/MeshRenderer corrigido com objeto filho e espera de limpeza do teste tornada limitada pelo evento de destruição, em vez de um prazo nominal apertado durante screenshots. As rodadas finais listadas passaram.

O aviso da build é preexistente: o pacote opcional `Unity.Pipeline` não possui `RuntimePipelineConfig` e fica desativado no Player. O URP 2D continua ativo. Nenhuma avaliação humana da mixagem, benchmark em outro PC ou nova dublagem foi inventada.

## Distribuição e daily

Pacote local: `Builds/Varginha_0.4.4-beta_2026-10-21_Windows_x64.zip` — 288,987,909 bytes, 198 arquivos, com guia de playtest, créditos e manifesto.

SHA-256: `90a99897fa7f6b05aa05fcec182509910b8418d9b0cbffbca73bc1bfbcb590c2`.

[Beta no GitHub](https://github.com/fwbiodev7/segredo-varginha-game-teste-cutscene-gdd-ldd/releases/tag/v0.4.4-beta). [Daily atualizada](../daily-sprint-2026-10-20/A_Sprint_Final_Varginha.html), [notas para apresentar](../daily-sprint-2026-10-20/Roteiro_da_daily.md). Apenas o último slide abre dúvidas, opiniões e feedbacks; não há respostas presumidas da equipe.

## Arquivos e cenas

Código: `CampaignRecordedAudio`, `CampaignSoundscape`, `CampaignAtmosphereAudio`, `CampaignFinalAllies`, `CampaignCyberPulse`, `CampaignAttackCaption`, `CampaignFlashEncounter`, `CampaignHouseSequence`, `CampaignContinuationController`, `VarginhaCampaignPhase1`, `VarginhaPlayerAttack`, `VarginhaGameSettings`, importadores, build e testes. Recursos: catálogo/WAV em `Assets/Resources/Varginha/Audio/Received20261021/`, atlas novo e shader `SupportShieldClean`. Ferramentas de preparo, validação e prévia em `Tools/Audio/`.

Cenas afetadas pelo runtime compartilhado: prólogo/infância, casa adulta, Fusca, escola normal, mata, igreja, cerco e arena final; também os mapas que já usavam combate da manifestação. **Nenhum `.unity` foi regravado.** Os arquivos Recovery e a alteração inicial do usuário no URP ficaram fora do commit. O atlas original de efeitos e as poses/rostos dos professores foram preservados.

## Pendências

[Lista atual](PENDENCIAS.md): playtest de alguém sem conhecer as soluções, mixagem em caixas/fones, equilíbrio contra chefe móvel/alunos, campanha manual e outro PC. Dublagem adiada por decisão do usuário; outras cutscenes/finais permanecem no roteiro, fora deste escopo.
