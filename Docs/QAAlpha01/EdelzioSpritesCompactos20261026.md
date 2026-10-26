# Edelzio: sprites compactos sem cortes

## Problema

A folha gerada de 8 × 8 tinha poses com membros e calçados incompletos na própria imagem de origem. Aumentar as margens entre as células não recuperava os pixels que faltavam e alguns fragmentos continuavam visíveis no jogo.

## Alteração

- A caminhada e a ociosidade agora reutilizam a arte original aprovada de quatro direções, com quatro quadros de caminhada por direção.
- O ataque usa a tira original de três quadros por direção.
- As oito direções solicitadas anteriormente foram reduzidas às quatro orientações originais (frente, esquerda, direita e costas); diagonais usam a orientação cardinal dominante. Essa redução evita exibir poses diagonais incompletas.
- A animação preserva o equipamento: caminhada com mochila usa `WalkGray`; ataque usa os sprites originais com mochila.

## Arquivos e validação

- `Assets/Scripts/Game/Varginha/Experiment/CampaignTeamEdelzio.cs`: seleção de caminhada, repouso e ataque.
- `Assets/Tests/PlayMode/CharacterFrameRenderingTests.cs`: teste e capturas Unity dos quadros originais nas quatro direções.
- PlayMode `CharacterFrameRenderingTests`: 5 testes aprovados, 0 falhas. As capturas de caminhada e ataque foram revisadas ampliadas; corpo, mãos e calçados aparecem inteiros.

Capturas do teste ficam em `Preview/PersonagensCorrigidos20261009/Edelzio_Original_*.png` dentro da cópia isolada de QA.
