# Pedido de efeitos sonoros - O Segredo de Varginha

Atualizado em 22/10/2026 após o playtest. Dublagem humana adiada pelo autor para **23/10/2026**, porque o microfone não estava disponível hoje. Os roteiros separados já entregues continuam sendo a base da gravação. Este pedido trata dos efeitos e ambientes.

## Mensagem pronta para encaminhar à equipe

Pessoal, seguem os sons que ainda precisamos para fechar melhor as ações do jogo. Priorizem a chave/caixa, lavar o rosto, café, mochila/notebook e passos no piso. Depois vêm ambientes internos, pistas e feedback dos puzzles. Socos, ataques inimigos, passos em madeira/terra, escola/floresta, motor e os efeitos principais dos apoios já foram recebidos; não precisamos refazer esse lote agora. A dublagem fica para amanhã, 23/10. A tabela abaixo explica o que produzir e onde será usado.

## Como ler a lista

- **P1:** primeiro lote. Ações frequentes, pistas da missão inicial e leitura dos controles.
- **P2:** segundo lote. Ambientes, puzzles, fenômeno e combate.
- **P3:** acabamento. Variações e identidade de efeitos que já têm uma gravação utilizável.
- **PROVISÓRIO:** há evento no código, mas ele usa síntese ou um som genérico. Produzir a gravação substitui esse som; a ação não está necessariamente muda.
- **GATILHO NOVO:** pedido de produção para uma ação existente, ainda sem evento sonoro dedicado no fluxo consultado. Receber o arquivo não significa que já esteja integrado.
- **COMPLEMENTO:** já existe som recebido; o novo arquivo é uma camada ou variação opcional.

Os nomes entre crases são sugestões para entrega. Só a coluna **evento atual** representa IDs já usados pelo sistema. Não há pedido de dublagem sintética ou de novas cenas.

## Efeitos e ambientes a produzir

| Prioridade | Arquivo sugerido | Cena e gatilho | Direção sonora e entrega | Situação | Evento atual |
|---|---|---|---|---|---|
| P1 | `chave_mesa_madeira_01.wav` | Casa adulta: Edelzio coloca a chave na mesa do café | Metal leve tocando madeira, pequeno chocalho da argola; 2-3 variantes de 0,3-1 s. Reconhecível, sem estrondo. | PROVISÓRIO | `Key` |
| P1 | `caixa_arranha_madeira_01.wav` | Investigação: indício vindo da caixa, fora do olhar | Atrito curto e localizado na madeira, inquietante mas plausível; 2 variantes de 0,5-1,5 s. Não revelar a criatura com rugido. | PROVISÓRIO | `Scratching` |
| P1 | `papel_manusear_01.wav` | Examinar pistas, abrir documentos e páginas | Papel seco, virar página e desdobrar em arquivos separados; 2-3 variantes de 0,3-1 s. | PROVISÓRIO | `Paper` |
| P1 | `caixa_abrir_01.wav` | Abrir a caixa de 1996 | Tampa de madeira/cardboard coerente com o objeto final, leve arrasto e apoio da tampa; abertura e fechamento separados, 0,5-1,5 s. | GATILHO NOVO | Sem ID dedicado; a inspeção usa `Paper` |
| P1 | `pia_agua_abrir.wav`, `pia_agua_loop.wav`, `pia_agua_fechar.wav` | Casa adulta: lavar o rosto na pia | Clique da torneira, água corrente e parada separados. Água em loop curto sem emenda audível, com respingos discretos. | GATILHO NOVO | Sem evento dedicado à água na rotina |
| P1 | `rosto_respingos_01.wav` | Mãos levando água ao rosto | 2-3 movimentos curtos de água e mãos molhadas; 0,3-0,8 s. Sem fala ou esforço vocal gravado junto. | GATILHO NOVO | Sem ID dedicado |
| P1 | `cafe_gole_01.wav`, `xicara_apoio_01.wav` | Casa adulta: beber café e apoiar a xícara | Gole discreto e cerâmica na mesa separados; 2 variantes de cada, 0,3-1 s. A xícara não deve soar como vidro quebrando. | GATILHO NOVO | Sem evento dedicado ao café na rotina |
| P1 | `mochila_ziper_01.wav`, `mochila_tecido_01.wav` | Pegar a mochila, guardar notebook e abrir inventário | Zíper real curto e tecido em arquivos separados; 2 variantes de 0,3-1 s. | PROVISÓRIO | `Zip` |
| P1 | `notebook_digitar_01.wav` | Preparação e investigação: digitação | Teclas curtas, 3-4 variantes ou rajadas de 0,5-1,5 s. Sem fala de fundo nem notificação de sistema. | PROVISÓRIO | `Typing` |
| P1 | `notebook_tampa_abrir.wav`, `notebook_tampa_fechar.wav` | Usar e guardar notebook | Dobradiça e toque suave da tampa, 0,3-1 s. Não usar o som de página como substituto. | GATILHO NOVO | Sem ID dedicado à tampa |
| P1 | `passo_piso_01.wav` a `passo_piso_04.wav` | Escola, laboratório e outros pisos rígidos | Passos individuais secos em cerâmica/concreto, 4 variantes de 0,15-0,5 s. Não entregar uma caminhada inteira como único arquivo. | PROVISÓRIO | `FootTile` |
| P1 | `ui_selecionar.wav`, `ui_confirmar.wav`, `ui_voltar.wav` | Interações, inventário e escolhas | Cliques curtos e discretos, 0,05-0,25 s, reconhecíveis entre si. Evitar som estridente repetido. | PROVISÓRIO; separar funções exige ajuste de gatilhos | `UI` |
| P2 | `puzzle_acerto.wav`, `puzzle_tentar_novamente.wav` | Validar dedução e rota dos Fragmentos | Confirmação curta de descoberta, 0,4-1,2 s. Erro discreto de 0,2-0,5 s, sem tom de punição. Não tocar a solução musical antes de resolver. | PROVISÓRIO / GATILHO NOVO para erro distinto | `Success` para conclusão |
| P2 | `mapa_tracar_01.wav`, `mapa_desfazer.wav` | Puzzle do mapa: escolher bifurcação e desfazer | Traço de lápis/papel ou toque suave no mapa, 0,15-0,5 s. Não produzir rotação de peças: esse puzzle foi substituído por um caminho com bifurcações. | GATILHO NOVO | Sem evento dedicado à rota |
| P2 | `casa_manha_loop.wav` | Casa adulta, aproximadamente 7h | Interior calmo, pássaros/cidade distantes e ambiente de cômodo. 30-60 s em loop. Sem grilos noturnos ou conversa inteligível. | PROVISÓRIO | `HouseAmbience` |
| P2 | `rua_manha_loop.wav` | Trajeto e exterior da Industrial de dia | Rua de cidade, tráfego distante e aves moderadas. 30-60 s em loop, sem motor de carro protagonista embutido. | PROVISÓRIO | `RoadAmbience` |
| P2 | `igreja_interior_loop.wav` | Exploração da igreja | Ar e ressonância suave de espaço vazio, 30-60 s em loop. Sem sino contínuo, coro ou pad do escudo misturado. | PROVISÓRIO | `ChurchAmbience` |
| P2 | `laboratorio_loop.wav` | Laboratório e equipamentos | Ventilação, máquinas e pulsos eletrônicos discretos, 30-60 s em loop. Avisos importantes devem vir em arquivos separados. | PROVISÓRIO | `LabAmbience` |
| P2 | `oficina_loop.wav` | Oficina | Ventilação e ruído distante de ferramentas, 30-60 s em loop. Evitar pancadas dominantes que se repitam em intervalo fixo. | PROVISÓRIO | `WorkshopAmbience` |
| P2 | `energia_corte.wav`, `energia_retorno.wav` | Infância: falha de energia após liberar exploração | Relé/estalo elétrico e queda/retorno do ruído ambiente, 0,3-1,5 s. Não repetir a explosão inteira nem antecipar a voz do Fusca. | GATILHO NOVO; hoje há ruído genérico de interferência | `RadioInterference` no apagão |
| P2 | `et_interferencia_pico.wav`, `et_interferencia_camada.wav` | Picos fortes de interferência nas cenas existentes | Textura digital rasgada com grave/ressonância estranha, 0,5-2 s, e camada sustentada separada. Manter o ruído de rádio como base. Não usar o ataque hacker do Renan como assinatura do ET. | COMPLEMENTO / novo gatilho de camada | `RadioInterference` já recebido |
| P2 | `chefe_energia_aviso.wav`, `chefe_energia_impacto.wav` | Combate final: aviso e descarga de energia | Antecipação e impacto separados, com ataque legível e cauda curta; 0,3-1,5 s. Não reutilizar a explosão narrativa da infância para todos os ataques. | PROVISÓRIO / GATILHO NOVO para antecipação separada | `AlienBurst` |
| P2 | `impacto_corpo_01.wav`, `impacto_chao_01.wav` | Encontro/combate: impactos de corpo e chão | 2-3 variantes curtas de 0,2-0,8 s. Separar contato físico de rugido. | PROVISÓRIO | `Impact` no encontro de clarão; demais impactos precisam mapear gatilhos |
| P3 | `fusca_porta_abrir.wav`, `fusca_porta_fechar.wav` | Entrar e sair do Fusca | Porta metálica de carro antigo, abertura e fechamento separados. 0,5-1,5 s. O lote atual tem porta genérica utilizável. | COMPLEMENTO; IDs separados ainda precisam ser ligados | `DoorOpen` já recebido e compartilhado |
| P3 | `fusca_chave_ignicao.wav`, `fusca_chave_soltar.wav` | Close da ignição durante o surto | Giro curto de chave, mecanismo retornando e argola, separados; 0,2-0,8 s. Motor, partida e falha já existem. | COMPLEMENTO / gatilho dedicado | `Key`, `Starter`, `CarFail` |
| P3 | `frasco_arremesso.wav`, `frasco_reagente_impacto.wav` | Apoio da Ouzana: voo e reação depois de acertar | Voo curto e reação química/mistura em arquivos separados. O vidro quebrando já cobre o impacto. Não trocar a habilidade existente. | COMPLEMENTO / gatilhos adicionais | `OuzanaGlass` já recebido |
| P3 | `escudo_ativar.wav`, `escudo_encerrar.wav` | Apoio do Padre Fábio: início e término da cúpula | Entrada e saída curtas combinando com o pad angelical recebido, 0,3-1 s. Sem sino de igreja. | COMPLEMENTO / gatilhos adicionais | `FabioShield` já recebido em loop |
| P3 | `renan_glitch_variacao_02.wav`, `renan_glitch_variacao_03.wav` | Apoio do Renan: impacto cyber/hacker no chefe | 2 variações de 0,3-1,3 s com assinatura próxima ao efeito recebido. Variação, sem mudar o alcance/dano do ataque. | COMPLEMENTO | `RenanPulse` já recebido |

Os tempos são orientações de produção. O sincronismo final será ajustado às animações e aos eventos reais, sem alongar ações apenas para caber no som.

## Já recebidos e integrados: reaproveitar

| Grupo | Material disponível | Uso correto |
|---|---|---|
| Combate | `PlayerPunch`, `EnemyAttack`, `BossRoar` | Soco, ataque inimigo e rugido. `BossClaw` e `EchoClaw` usam a mesma fonte do ataque inimigo. |
| Passos | `FootWood` e `FootNature`, três variantes de cada | Madeira e terreno natural. Ainda falta gravação própria de piso rígido. |
| Ambientes | `SchoolAmbience`, `ForestAmbience`, `TelevisionAmbience` | Escola, floresta/noite e TV distante. Não transformar floresta noturna no ambiente da casa adulta às 7h. |
| Fusca | `Engine`, `Starter`, `CarFail`, `DoorOpen` | Motor em loop, partida, falha e porta genérica. Refinos de autenticidade são P3. |
| Interferência | `RadioInterference` | Ruído de TV/rádio para o fenômeno. O arquivo glitch hit não pertence a esta função. |
| Apoios | `RenanPulse`, `OuzanaGlass`, `FabioShield` | Glitch/hacker do Renan, frasco da Ouzana no boss e escudo angelical do Padre. |
| Infância | `ChildExplosion` | Explosão do encontro na infância. Preservar o momento narrativo. |
| Outros | `ChurchBell`, `SiegeAlarm`, `ManifestationTear`, `ManifestationCollapse` | Sino no local, alarme e manifestações. `SealClose` e `ManifestationRise` usam aliases já documentados. |

O catálogo tem **25 WAV e 29 IDs contando quatro aliases**, derivados de **20 fontes únicas** do pacote recebido. Alias é outro nome para o mesmo som, não mais uma gravação diferente.

## Formato de entrega

- Preferir **WAV PCM**, 48 kHz / 24 bits. Se o material já estiver em WAV 44,1 kHz, enviar o original nessa taxa em vez de converter apenas para mudar o número. Os derivados atuais são PCM16/44,1 kHz.
- Efeitos pontuais de preferência mono; ambientes em estéreo. Manter o original sem compressão destrutiva.
- Um efeito por arquivo, nome claro e variantes numeradas. Para torneira, motor e efeitos sustentados, separar entrada, loop e saída quando houver essas partes.
- Loops de ambiente de aproximadamente 30-60 segundos, sem corte ou clique na emenda. Evitar fala identificável embutida nos ambientes.
- Sem clipping e sem silêncio longo na frente. Deixar a cauda natural dos sons e enviar os efeitos separados da música e das vozes.
- Mandar também nome do autor/fonte e informação de licença/autorização disponível. Não inferir licença a partir do nome do arquivo.

## Escopo da conferência

Lista cruzada com `CampaignSoundscape.cs`, `CampaignRecordedAudio.cs`, o catálogo `Received20261021/Catalog.json`, os eventos da investigação e os controladores atuais de infância e rotina. O levantamento é de código e catálogo: não é uma avaliação humana da mixagem. Arquivos ainda solicitados não foram inventados, recebidos ou integrados nesta entrega.

Referência do lote anterior: [créditos e mapeamento dos sons recebidos](CREDITOS_AUDIO_RECEBIDO_2026-10-21.md). Roteiros: `output/dublagem-2026-10-22/`. A lista não altera mecânicas, diálogos, duração de puzzles ou a cronologia aprovada.
