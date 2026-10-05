# Manifestação da fase 20 — entrada e saída

## Continuação e conclusão do combate

O mesmo monstro com chifres, olhos vermelhos e fissuras cian aparece durante o combate. Boss e filhos têm oito poses principais e seis intermediárias em quatro direções. A sequência alterna passos, preparação, golpe e recuperação sem alterar a escala do corpo entre os quadros. A cápsula de dano acompanha o agachamento e mantém a largura do torso: garras levantadas, chifres e rastros de energia não aumentam a área vulnerável. O círculo dos pés cuida dos obstáculos separadamente.

O deslocamento usa `Rigidbody2D.position` em `FixedUpdate`, com interpolação da apresentação. Isso evita usar a posição visual atrasada como origem da física seguinte. O passo de investida termina ao iniciar a recuperação, a caminhada mantém um relógio próprio, os filhos reutilizados reiniciam seus estados e a repulsão do escudo é consumida uma vez na física, sem ser sobrescrita pela perseguição. A navegação usa o raio efetivo do colisor dos pés.

Foi gerada uma revisão transparente de `BossFlowSource.png` com o ImageGen integrado, preservando a fonte anterior. A inspeção do alfa confirma que parte do fundo colorido visto no preview original já era transparente; o aspecto final é verificado na captura renderizada pelo Unity, não pelo RGB de pixels invisíveis. Fonte selecionada: `ArtSources/BossCinematic20261005/BossFlowSourceV2.png`. Prompt integral: [FlowCleanupPrompt.json](../../ArtSources/BossCinematic20261005/FlowCleanupPrompt.json). Empacotamento: `unity command eval_file --file Tools/PackManifestationFlow.cs --caller plugin --skill unity-cli --format json`.

Os sprites finais ficam em `Assets/Resources/Varginha/StoryCharacters/ManifestationBossCombatV2`, `ManifestationChildCombatV2`, `ManifestationBossFlowV1` e `ManifestationChildFlowV1` (PNG e JSON de medidas). [Combate.png](Combate.png) mostra o boss e um filho na arena durante uma pose intermediária do ataque.

A entrada agora dura 6,2 segundos: o selo abre, a silhueta emerge em seis poses, os olhos vermelhos aparecem e uma ruptura cian precede a formação completa. A saída dura 4,7 segundos: seis poses desmancham o corpo, a cinza converge para a fenda e o selo se apaga. Os sprites finais de entrada e combate se sobrepõem brevemente para suavizar a troca.

A câmera enquadra a manifestação, aproxima lentamente e retorna ao jogador antes de liberar o próximo passo. Barras de cinema, escurecimento das bordas, legendas, partículas de cinza, um clarão breve e tremor pontual acompanham sons originais de respiração grave, ruptura e fechamento. A ambiência baixa durante a sequência e retorna depois.

Pausa congela poses, câmera, partículas e áudio. Movimento reduzido remove partículas, tremor, clarão e deslocamento gradual da câmera; mantém as poses e a história. O combate aguarda a entrada inteira. Na derrota, a dissipação é salva imediatamente, os ecos desaparecem e a calibração só pode ser validada depois da saída. Recarregar um save já dissipado não repete o chefe. A calibração e a ligação de Edelzio continuam necessárias.

## Arte e reprodução

- Fontes transparentes e prompts integrais: [ArtSources/BossCinematic20261005](../../ArtSources/BossCinematic20261005/Prompts.json). Geradas pela ferramenta ImageGen integrada, com `EntityManifestation.png` como referência de identidade para o corpo. Fontes anteriores preservadas.
- Produção: `BossManifestationCinematicV1.png` e `BossRiftCinematicV1.png`, em `Assets/Resources/Varginha/StoryEffects`. Duas linhas de seis poses cada; 34 pixels por unidade; filtro Point; sem mipmaps nem compressão.
- Empacotamento reproduzível no Editor: `unity command eval_file --file Tools/PackBossCinematic.cs --caller plugin --skill unity-cli --format json`. Uma escala comum preserva a altura relativa das poses que emergem e se desfazem.
- [Entrada](Entrada.png) e [saída](Saida.png): capturas da câmera da arena em 960 × 720. Estas imagens mostram a arte no cenário; a sobreposição de interface é desenhada pelo IMGUI fora da captura da câmera.

## Verificação

Na conclusão, seis casos de EditMode passaram, cobrindo os 136 quadros dos seis atlas, margens transparentes, poses não vazias, filtro Point, ausência de mipmaps e compressão. Relatório: [ArtEditMode.json](ArtEditMode.json).

Nove casos distintos de PlayMode têm resultado final aprovado: golpes e suporte dos aliados, colisões e reutilização dos filhos, mochila, dimensões do corpo em todas as poses/direções, deslocamento físico e repulsão, progressão 11–19, cinematográficas e calibração, ataques naturais do boss/filho e retorno/epílogo. Índice dos resultados finais: [CombatPlayMode.json](CombatPlayMode.json).

A rodada completa terminou com oito aprovados e uma falha no novo cenário de ataques naturais: ao desativar o controlador de Edelzio, o alvo retinha a velocidade causada pela colisão anterior e se afastava do filho. O cenário passou a congelar a posição do alvo deliberadamente estacionário; o caso foi repetido e passou, sem alteração adicional do código de jogo. Os relatórios brutos são [CombatSuitePlayMode.json](CombatSuitePlayMode.json) e [NaturalAttackPlayMode.json](NaturalAttackPlayMode.json). Essa repetição verifica preparação sem dano, um único dano por investida (24 no boss, 10 no filho), uso das poses intermediárias, recuperação vulnerável e ausência de deslocamento residual.

Unity 6000.6.0f1. O teste `ManifestationCinematicsFreezeOnPauseRestoreCameraAndRequireCalibration` passou nas opções normal e movimento reduzido, incluindo pausa nas duas sequências, invulnerabilidade na entrada, restauração da câmera, encerramento dos efeitos, persistência da derrota e calibração bloqueada durante a animação. Relatório: [CinematicsPlayMode.json](CinematicsPlayMode.json).

O progresso real do jogador é preservado e restaurado após a verificação. As capturas e testes foram feitos no Editor; não foi gerado um executável standalone.
