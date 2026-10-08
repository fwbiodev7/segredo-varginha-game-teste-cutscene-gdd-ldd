# Remaster cinematográfico - 08/10/2026

## Escopo e análise

Base: `412cd5f`. A campanha atual tem 14 fases em cinco atos, com IDs serializados preservados. A revisão usa as cenas existentes, os sprites, a paleta, os diálogos e o Input System 1.20.0. Unity 6000.6.0f1. Timeline 6.6.0 e URP 17.6.0 estão instalados; Cinemachine não está instalado. O renderizador efetivo é registrado durante os testes em `Prints/runtime-pipeline.txt`.

Plano executado: reutilizar o carregamento assíncrono e as câmeras; acrescentar direção por contexto, continuidade de áudio e variações leves na luz; testar pausa, controles, transições e campanha; capturar exemplos e produzir relatório. Riscos principais: disputa entre escritores da câmera, recursos de captura retidos e estado de pausa atravessando cenas.

## Implementação

- `CampaignTransitionProfile`: seleção por cena, com opção de configurar estilo, duração, barras e ambiente por chamada. Fade nas mudanças de capítulo, crossfade nos interiores, movimento curto de câmera em deslocamentos e interferência breve na lembrança da fase 15. Movimento reduzido converte os estilos em fade curto.
- `CampaignCinematics`: carrega de forma assíncrona, antecipa o som do próximo ambiente, captura apenas o mundo para crossfade e libera a textura no fim ou ao destruir o componente. Menus e inventário mantêm seus próprios desenhos. A rotina restaura a escala de tempo em `finally`; cenas inexistentes são rejeitadas antes de bloquear a transição.
- `CampaignCameraDirector`: aproxima o enquadramento das pistas em 1996, na casa adulta, na escola e nas fases posteriores. O seguidor continua responsável por limites e posição final. O evento termina suavemente, permite cancelamento e congela na pausa. Não há alteração contínua do zoom nem adoção de outra grade de pixels: os mapas combinam PPUs diferentes e a identidade atual tem prioridade.
- `CameraFollow2D`: respeita a pausa. O controlador de Edelzio bloqueia comandos de gameplay durante transições, inclusive no pequeno movimento anterior ao carregamento.
- `CampaignAmbientBridge`: dois canais persistentes sobrepõem ambientes, mantêm o som anterior durante o carregamento e antecipam o seguinte. Pausa congela a mistura; diálogos e cutscenes atenuam o ambiente. Entrar no menu encerra o ambiente da campanha.
- `CampaignSoundscape`: reaproveita os sons existentes em cache entre cenas; elimina a lista de notas criada a cada amostra do som de sucesso. Passos e o clarão recebem emissão posicional moderada. O áudio não adiciona fala sintética.
- `VarginhaSettingsAudio`: reconhece os canais cujo volume muda durante o crossfade. Esses canais aplicam o controle de música diretamente no mixer de ambiente; o controle global continua atendendo as outras fontes. Isso evita sobrescrever o fade e alterar o volume de uma mistura pausada.
- `CampaignAtmosphereLayer`: pequenas variações sobre a luz já composta, sem reconstruir sombras nem enviar textura a cada quadro. A falta de energia e o clarão aplicam alteração discreta de cor; pausa e movimento reduzido são respeitados. Faróis, iluminação dos personagens e névoa existentes são preservados.
- `VarginhaCameraShake`: respeita movimento reduzido; o clarão usa impacto leve quantizado em pixels da tela. Câmeras dos mapas da campanha recebem o componente.

## Decisões de preservação

A cinematográfica de abertura, os cartões de passagem de tempo, a viagem do Fusca, as memórias e o combate final já possuem sistemas próprios. Eles foram reutilizados. Não se reconstruiu o roteiro em Timeline, não se instalou Cinemachine e não se migrou o render pipeline. Também não se acrescentaram bloom ou filtros sobre os sprites.

Referências oficiais consultadas:

- [Cinemachine e Pixel Perfect](https://docs.unity3d.com/ja/Packages/com.unity.cinemachine%402.6/manual/CinemachinePixelPerfect.html): controle compartilhado do tamanho ortográfico e limitações dos blends.
- [Luzes e materiais 2D em URP](https://docs.unity.com/en-us/engine/6000.0/manual/unity2d/2d-urp/2d-index/2d-light-properties-explained): iluminação depende do renderizador e de materiais compatíveis.
- [Timeline](https://docs.unity3d.com/Packages/com.unity.timeline@1.8/manual/index.html): avaliação como alternativa de sequenciamento, preservando os sistemas já funcionais neste projeto.

Os resultados efetivamente executados e limitações estão no relatório e nos arquivos XML desta pasta. Não há certificação de hardware nem benchmark de desempenho no computador do jogador.
