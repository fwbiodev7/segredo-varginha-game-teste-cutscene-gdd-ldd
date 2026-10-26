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

## Contrato atual

O personagem continua com quatro orientações artísticas, não oito; não foram inventadas poses diagonais. A caminhada com mochila usa a folha cinza aprovada. A arte e os metadados de importação das folhas-fonte não foram modificados. A alteração foi integrada e validada no teste PlayMode descrito acima; build de jogador/plataforma não foi executado.
