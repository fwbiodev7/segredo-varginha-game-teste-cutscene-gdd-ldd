# Revisão — controle, portas e sprites

Capturas reais do projeto no Unity em 7 de outubro de 2026.

- [Controle e remapeamento](Controle_remapeamento.png): tela Game, menus e arte efetivamente integrados.
- [Alunos sentados](Alunos_sentados.png): apoio nas cadeiras e ordenação em profundidade.
- [Edelzio na pia](Edelzio_pia.png): alinhamento durante a animação de lavar o rosto.
- [Fusca original no mapa](Fusca_original_no_mapa.png): sprite lateral fornecido pelo usuário, com iluminação do cenário.

## Fusca

A cópia `Assets/Resources/Varginha/StoryEffects/FuscaOriginal.png` é idêntica ao arquivo fornecido, SHA-256 `048dfd8c37a00d2fc84407871ea931f81927efd71bc69d50c733a549292f7344`. A vista lateral recorta o primeiro quadro sem modificar pixels. O arquivo original de três quadros permanece completo.

As vistas frontal e superior foram geradas a partir dessa referência; superfícies ocultas precisaram ser inferidas. A superior possui orientações cardeais por rotações de 90 graus sem interpolação. Os sprites preservam proporção com escala uniforme, filtro Point, sem mipmaps nem compressão. Fontes antigas continuam disponíveis para recuperação.

Veja o manifesto em `Assets/Resources/Varginha/StoryEffects/FuscaOriginalManifest.json`. A arte-fonte das novas vistas está em `Assets/ArtSource/FuscaOriginalViewsSource.png`.

O carro e o brilho dos faróis antigos estavam pintados no fundo. A revisão cobre somente essas regiões com asfalto do próprio mapa. O feixe lateral parte do farol visível do sprite original; na oficina, os dois emissores acompanham as vistas direcionais. Alcance e origem foram ajustados sem alterar o arquivo original do usuário.
