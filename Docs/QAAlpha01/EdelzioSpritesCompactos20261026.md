# Edelzio: animações compactas e quadros completos

## Escopo e fontes

Reduzir a caminhada do Edelzio para três quadros claros por direção e confirmar que o ataque mantenha seus três quadros. A identidade visual e os pixels aprovados foram preservados: reutilizam-se `EdelzioWalkV2.png`, `EdelzioPunchV2.png` e, com mochila, `WalkGray.png`/as folhas de ataque originais. As fontes têm quatro direções (frente, esquerda, direita e costas); diagonais continuam usando a direção cardinal dominante porque a arte diagonal anterior tinha membros incompletos.

Fontes PNG inspecionadas (RGBA): caminhada e caminhada com mochila, 1254 × 1254; ataque, 1086 × 1448. SHA-256: `EdelzioWalkV2.png` `8161f27f38fc277b758f21c5d84f785463c3dc10d20a61bf2b009dedc78893a0`; `EdelzioPunchV2.png` `d9b1fad6ec9723ec0a392c3809eaf3a93763faefb6a82fad4b66c2a6ad218714`; `WalkGray.png` `92bcd4a4cb7d84f8147b6806db1188b43d2120a15f3590944a5a7236a75bd878`. São as folhas já usadas pelo jogo e aprovadas anteriormente; elas não foram editadas.

## Alterações

- A caminhada percorre as poses de origem 0, 1 e 3, retirando a pose redundante sem redesenhar nem recortar o personagem. O ciclo mantém a duração anterior de 4/7 s com 5,25 quadros por segundo.
- O ataque conserva as três poses aprovadas da folha original em cada direção.
- O código de seleção duplicado e sem efeito da animação de ataque foi removido; a pose de ataque continua sendo escolhida pelas fases atuais do combate.
- O teste verifica caminhada e ataque com/sem mochila nas quatro direções e salva uma captura por quadro, em vez de uma única pose representativa por direção.

## Arquivos afetados

- `Assets/Scripts/Game/Varginha/Experiment/CampaignTeamEdelzio.cs`: seleção dos três quadros da caminhada e ciclo de animação.
- `Assets/Tests/PlayMode/CharacterFrameRenderingTests.cs`: validação das poses e capturas individuais.
- `Docs/QAAlpha01/EdelzioSpritesCompactos20261026.md`: este registro.
- Nenhuma cena ou prefab foi alterado.

## Verificação e evidência

As folhas originais e as 24 capturas geradas foram abertas/revisadas em escala ampliada. Unity `6000.6.0f1`, PlayMode `CharacterFrameRenderingTests`: **5 passaram, 0 falharam, 0 ignorados**. O teste valida os UVs e a geometria de caminhada/ataque com e sem mochila nas quatro direções e salva 24 capturas sem mochila: três quadros por ação/direção. A revisão visual do contact sheet confirmou cabeça, mãos e sapatos inteiros e espaçamento entre cada painel. As capturas individuais ficam em `Preview/PersonagensCorrigidos20261009/Edelzio_{Walk|Punch}_D*_F*.png` na cópia QA isolada. Contact sheets: `output/revisao-final-2026-10-26/Edelzio-walk-3frames-contact.png` e `output/revisao-final-2026-10-26/Edelzio-punch-3frames-contact.png`. Resultado final XML: `output/revisao-final-2026-10-26/edelzio-3frames-final-20261026.xml`.

O log do Unity registrou falhas de inicialização de D3D12/licenciamento no batchmode, mas o runner completou a suíte, reportou `Passed`, e as 24 imagens de renderização esperadas foram criadas e revisadas. O teste é PlayMode no Editor; não foi executado um build do jogo. Os limites de recorte e UVs são validados para cada sprite. A contagem menor refere-se aos quadros da caminhada, não ao número de direções.

### Build e teste Standalone (26/10/2026)

- Build Windows x64 `0.4.8-beta`, Unity `6000.6.0f1`: `Succeeded`, 19 cenas, 0 erros, 2 avisos, 413.535.138 bytes, 127,56 s. O executável temporário foi gerado em `%TEMP%/varginha-master-qa-20261023-120709/Builds/Varginha_0.4.8-beta_2026-10-23/SegredoDeVarginha.exe`; a build existente no projeto não foi sobrescrita.
- Os dois avisos de build são `CS0618` em `CampaignRenanTeacher.cs:21`, pelo uso obsoleto de `FindFirstObjectByType`; não apontam para os arquivos do Edelzio.
- Player Standalone `CharacterFrameRenderingTests`: **5 passaram, 0 falharam, 0 ignorados**, incluindo a validação dos quadros e renderer: `output/revisao-final-2026-10-26/edelzio-player-3frames-20261026.xml`.
- Smoke test do executável real: processo permaneceu aberto por 10 segundos e carregou o runtime. D3D12 falhou na inicialização e houve fallback para Direct3D 11 na NVIDIA GeForce G210; o log também registra `GpuFence` indisponível. O processo foi encerrado ao fim da observação. Log: `output/revisao-final-2026-10-26/edelzio-build-smoke.log`.
- A cena jogável não foi percorrida visualmente no executável. As capturas quadro a quadro foram revistas no teste PlayMode do Editor; o player Standalone passou os testes de renderização/geometry. Isso confirma compilação e inicialização, sem afirmar uma revisão manual de gameplay.

## Contrato atual

O personagem continua com quatro orientações artísticas, não oito; não foram inventadas poses diagonais. A caminhada com mochila usa a folha cinza aprovada. A arte e os metadados de importação das folhas-fonte não foram modificados. A alteração foi integrada, passou no PlayMode do Editor e no teste player Standalone, e foi incluída na build temporária Windows x64; a ressalva da inspeção manual da cena está descrita acima.
