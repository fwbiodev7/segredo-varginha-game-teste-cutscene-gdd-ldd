# Plano de integração dos sons recebidos — 21/10/2026

Fechamento: execução e atribuições em [INTEGRACAO_AUDIO_2026-10-21.md](INTEGRACAO_AUDIO_2026-10-21.md); fontes e cortes em [CREDITOS_AUDIO_RECEBIDO_2026-10-21.md](CREDITOS_AUDIO_RECEBIDO_2026-10-21.md). O texto abaixo conserva o plano aprovado; os resultados efetivos ficam no [relatório da candidata 0.4.4](../../output/audio-integration-2026-10-21/RELATORIO.md).

Estado: integração concluída para a candidata **0.4.4-beta**, após aprovação do usuário e correções autorais. A build visual anterior 0.4.3 permanece histórica e não contém esse pacote de áudio. O ZIP possui **20 MP3 e um M4A** (`abrindo_portawav.m4a`). Os dois `tutmozis-metal-crack-606770` têm bytes idênticos e compartilham uma fonte. Nomes e hashes ficam em `output/audio-integration-2026-10-21/archive-inventory.json`.

O mapeamento abaixo conserva o plano aprovado. Duração, nível, início audível e trechos de loop foram medidos e registrados no manifesto dos derivados; a avaliação auditiva da mixagem continua parte do playtest.

Correções autorais do usuário incorporadas: glitch variation é ataque do Renan; vidro é o frasco da Ouzana no boss; explosão pertence à sequência inicial da criança; som angelical sem sinos é o escudo do Padre Fábio na luta final. Essas atribuições prevalecem sobre a sugestão preliminar de classificação pelo nome.

| Arquivo / tipo | Uso proposto no jogo | Tratamento / gatilho |
|---|---|---|
| mixkit-impact-of-a-strong-punch | Impacto do soco de Edelzio | Tocar no impacto confirmado, sem som de acerto ao golpear o vazio |
| yodguard-powerful-bear-attack-with-roar | Ataque da manifestação/inimigo | Preparar trecho curto e sincronizar com o ataque; não introduzir urso na história |
| yarzur-sfx11-boss_roar | Aparição/ação forte do chefe | Evento pontual, sem repetir o rugido a cada frame |
| dragon-footsteps-on-wood | Passos dentro da casa/madeira | Extrair variações curtas e alternar no contato com o chão |
| dragon-footsteps-on-the-nature-trail | Passos na mata/terra | Separar passos utilizáveis; não tocar a gravação inteira a cada passada |
| freesound-loyola-high-school-soundwalk | Ambiente da escola normal | Loop com transição; não usar como ambiente da escola já tomada |
| eryliaa-evening-nature-with-crickets... | Floresta/noite nos ambientes adequados | Loop suave; não colocar grilos noturnos na casa adulta às 7h |
| arunangshu-tv-playing-in-next-room | TV doméstica na abertura de 1996 | Ambiente discreto da transmissão, sem competir com legendas |
| freesound-tv-no-signal | Perda de sinal da TV/rádio | Preparar trecho curto; interromper/transicionar na mudança de evento |
| wayofx-glitch_hit_variation1 | **Ataque do Renan** | Sincronizar com a ativação/impacto do ataque; não usar como ruído dos glitches visuais |
| dragon-angelical-synth-pad | **Escudo do Padre Fábio na luta final** | Iniciar ao erguer o escudo, sustentar somente enquanto estiver ativo e terminar com fade; separado dos sinos |
| 350chevo-1982-chevy...fail-start | Falha de partida/colapso do Fusca | Recortar tentativa falha; não usar como confirmação de partida normal |
| magiaz-carro-velho | Motor/Fusca | Selecionar trecho coerente para partida/marcha e loop do motor |
| abrindo_portawav.m4a | Abertura de porta | Converter cópia para formato importável e ligar ao evento real de abertura |
| alex_jauk-old-town-church-bells | Igreja/entrada ou referência sonora local | Toque pontual de sino, com intervalo e volume controlados |
| lazychillzone-air-raid-siren | Alarme em tensão avançada | Reservar ao acontecimento de cerco/ameaça já existente, sem alarme constante na rotina inicial |
| magiaz-vidro-quebrando | **Frasco da Ouzana atingindo o boss** | Tocar na colisão/ruptura do frasco, sem usar como janela quebrada ou som genérico |
| tutmozis-metal-crack (duas cópias idênticas) | Rasgo/impacto metálico | Uma fonte compartilhada; preservar ambos os registros no inventário |
| yarzur-sfx17-break | Colapso/quebra na manifestação | Variante pontual ligada à ação existente, sem criar puzzle/objeto destrutível novo |
| soundreality-explosion-fx | **Explosão inicial com Edelzio criança** | Sincronizar com a explosão/clarão do prólogo; volume ajustado, sem deslocar o som para o ataque do boss |

## Integração proposta

1. Preservar arquivos recebidos e inventário; preparar cópias apenas quando houver corte, fade, conversão ou ajuste de nível necessário.
2. Acrescentar catálogo de áudio gravado para `CampaignSoundscape` e combate; os sons gerados atuais permanecem fallback para categorias sem equivalente no pacote, como UI e algumas superfícies.
3. Ambientes em fonte própria com crossfade/ducking; motor separado; efeitos pontuais controlados por evento. Pausa suspende som; troca de fase encerra loops anteriores. Evitar fontes duplicadas e repetição por frame.
4. Reaproveitar os gatilhos da rotina, investigação, prólogo, Fusca, escola, floresta e combate. Não adicionar acontecimentos ao GDD apenas para usar um arquivo.
5. Importar conforme uso (ambientes longos versus efeitos curtos), validar referências e gerar build Windows. Documentar exatamente quais arquivos foram aproveitados, derivados e cortes usados.
6. Testar soco acertando/errando, ataque do inimigo, passos parado/andando/pausado, escola versus floresta/noite, transições de ambiente, TV e surto do Fusca. Verificar ausência de sobreposição acumulada, referências nulas e loops após sair da cena.

Dublagem continua adiada; o pacote não exige gerar voz. O plano cobre integração técnica dos sons recebidos; balanço artístico, volume confortável e desempenho em outra máquina serão avaliados no playtest.
