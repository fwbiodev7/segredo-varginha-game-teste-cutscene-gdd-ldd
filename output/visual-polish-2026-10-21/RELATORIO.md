# Entrega visual 0.4.3-beta — 21/10/2026

Concluído: cabine cartunesca, mapa de Fragmentos ilustrado, abertura jornal/TV de 1996 em camadas, oito poses da criança, picos de glitch e manhã às 7h com paisagem baseada na fotografia enviada. A implementação foi aprovada na conversa e integra a campanha existente. O áudio recebido posteriormente permanece em planejamento, sem fazer parte desta build.

## O que o jogador encontra

**1996:** montagem de 16 s com jornal, apresentador, transmissão recuando para a TV e Edelzio criança assistindo. O personagem novo mantém cabelo, camiseta amarela e bermuda azul. Oito poses a 125 ms formam o ciclo de atenção/respiração; a transição da transmissão é interpolada em tempo real. O apresentador e sua animação de boca existentes foram preservados. Não há frase do rádio, criatura completa nem clarão antecipado. Depois do título, o jogador explora; a perda de sinal usa o gatilho jogável existente. Saves com abertura vista não precisam repetir a montagem. Pausa e pulo de repetição foram testados.

**2026:** a casa adulta tem vistas de manhã somente nos vidros, luz ambiente mais neutra e sombras pintadas aclaradas por material próprio. HUD 07:00. Planta, móveis, colisões, objetos, pistas e PNGs originais permanecem. A cidade enviada pelo usuário foi interpretada em pixel art e usada também atrás dos vidros do Fusca. O prólogo de 1996 continua à noite.

**Fusca:** banco/porta e painel em camadas novas, foscos e cartunescos. Mesmos quatro frames originais do motorista, posição e escala já aprovadas, sem outro volante. Rádio/painel falham, surge a frase legendada, ignição esquenta, mão se afasta, motor retorna e Edelzio desembarca no mapa real da Industrial. A mão sem manga comprida e a chegada já aprovadas foram preservadas. Rajadas fortes combinam faixas densas, rasgos verticais, blocos escuros, cor e deslocamento; os intervalos e o modo de movimento reduzido continuam legíveis.

**Fragmentos:** o esquema geométrico deu lugar a terreno/riverbank, árvore, tronco, ponte, muro, portão e capela em pixel art. As três bifurcações, solução, prévia, traçar, desfazer, dicas, controles e progresso parcial não foram redesenhados novamente. A única ponte fica visível; a trilha incorreta termina na água.

## Validação final

| Verificação | Resultado / evidência |
|---|---|
| EditMode completo de regras e importação | **378/378**, zero falhas/skips — `editmode-delivery.xml/json` |
| Player Windows, contratos direcionados | **8/8**, zero falhas/skips — `player-delivery.xml/json` |
| Skill: export/validação dos atlas novos | 2 atlas, zero erros; 14 avisos de alpha/borda preservados e revisados — `art/skill-exports/` |
| Reprodução do ciclo da criança | 8 poses observadas na prévia — `art/playback.json`; oito capturas nativas `child-pose-*` |
| Prólogo amostrado do jogo | 72 quadros reais; reprodução/reset aprovados — `opening-preview-qa.json` |
| Viagem amostrada do jogo | 114 quadros reais; reprodução/reset aprovados — `preview-qa.json` |
| Daily | 10 slides, fontes/imagens, navegação, nove transições, notas, limites e movimento reduzido aprovados — `slides-qa.json` |
| Build Windows Unity 6000.6.0f1 | `Succeeded`, **0 erros / 1 aviso** — `windows-build.json` |
| ZIP | CRC e manifesto SHA-256 por arquivo conferidos — `windows-package.json` |
| Preservação | Três saves e configurações originais restaurados byte a byte — `preservation.json` |

Os oito casos incluem sequência completa do Fusca e retorno ao controle, pulo de cena vista/movimento reduzido, leitura das camadas na GPU, montagem inicial com oito poses/pausa/controle, casa diurna, percurso físico da biblioteca, puzzle por gamepad com retomada parcial e controle inicial da criança sem combate. Não equivalem a uma partida humana completa de toda a campanha.

O aviso de build é do pacote opcional `Unity.Pipeline`: falta `RuntimePipelineConfig`, portanto esse pacote fica desativado no Player. Não é uma indicação de URP desativada nem falha da renderização testada. Logs integrais permanecem em `output/agent-logs/`, ignorados pelo Git. Não foi declarado modelo local de resumo nem economia de tokens a partir da redução de bytes.

## Arte, cenas e arquivos

Detalhes de fontes, resoluções, hashes, paletas, prompts, recortes, pivôs e avisos: `art/PROMPTS_E_ORIGEM.md`, `art/skill-exports/source-inventory.json` e `technical-report.md`. PNGs novos foram gerados com `image_gen` integrada e preparados/verificados com a skill `pixel-art-sprite-sheet-master`. Os arquivos não são fontes Aseprite em camadas nem uma paleta estrita de 16/32 cores. Originais de personagem, Fusca e mapas continuam intactos.

Novos componentes principais: `CampaignOpeningCinema`, `CampaignAdultMorning`, `CampaignTrailMapArt` e shader `IllustratedMorning`. Lista completa: `changed-files.json`. Documentação funcional: `Docs/CampanhaOficial/POLIMENTO_VISUAL_E_ABERTURA_2026-10-21.md`.

Cenas afetadas em runtime: fase 1/prólogo, fase 2/casa, viagem/chegada da fase 4 e interface da fase 6/Fragmentos. Nenhuma cena `.unity` de produção foi reserializada. Não houve missão nova nem mudança da solução do puzzle nesta revisão.

## Distribuição e limites

Executável: `Builds/Varginha_0.4.3-beta_2026-10-21/SegredoDeVarginha.exe`. Pacote local: `Builds/Varginha_0.4.3-beta_2026-10-21_Windows_x64.zip`, 282.745.053 bytes, SHA-256 `4f96f7866f3ea725ee1411a6e37ef5d1ae885d77f247c68dac41a2ae8b8b9437`. A build não foi publicada como Release público.

As prévias são silenciosas e amostradas; não medem FPS nem substituem o executável para som e controles. Dublagem segue adiada, sem voz sintética. Playtest com pessoas, aprovação artística da equipe, outro PC e cutscenes dos capítulos posteriores continuam em `PENDENCIAS.md`. O ZIP de áudio enviado no fim desta etapa foi inventariado separadamente; seu plano ainda será apresentado antes da integração.
