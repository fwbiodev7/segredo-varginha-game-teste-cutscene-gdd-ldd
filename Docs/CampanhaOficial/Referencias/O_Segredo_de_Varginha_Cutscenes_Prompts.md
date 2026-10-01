# O SEGREDO DE VARGINHA --- CUTSCENE DESIGN + PROMPTS DE PRODUÇÃO

## 1. DIREÇÃO VISUAL

As cutscenes devem parecer parte do mesmo jogo, mesmo quando combinarem
arte manual e IA.

### Camada A --- Pixel art manual

Responsável por tudo que precisa permanecer consistente: - rostos e
silhuetas dos personagens; - roupas; - objetos importantes; - Fusca; -
símbolos narrativos; - poses-chave; - foreground; - elementos que
precisam casar perfeitamente com o gameplay.

### Camada B --- IA

Usar principalmente para: - concept/background base; - céu, névoa,
floresta, cavernas e texturas; - iluminação sobrenatural; - massas de
sombra; - frames intermediários abstratos; - flashes de memória; -
distorções; - reportagens/recortes fictícios; - composições de câmera
para depois redesenhar em pixel art.

**Regra:** IA não define o design final dos personagens. O resultado
gerado serve como base, matte/background, referência ou camada tratada.
Personagens recorrentes devem receber correção/redesenho manual.

### Linguagem visual

-   2D pixel art cinematográfica.
-   Horror sobrenatural brasileiro.
-   Varginha/MG como identidade.
-   Paleta inicialmente cotidiana; verde/frio e contrastes anormais
    aumentam com a entidade.
-   Granulação, CRT, aberração cromática e glitch usados com moderação.
-   Nada de estética genérica de "alien sci-fi".
-   A entidade deve permanecer parcialmente incompreensível até o final.
-   Evitar texto gerado dentro de imagens por IA; inserir textos
    manualmente no jogo.

## 2. MECÂNICA DO SISTEMA DE CUTSCENES

Criar `CutsceneController` orientado por dados.

Cada cutscene possui: - `cutscene_id` - `timeline` - `shots[]` -
`audio_cues[]` - `dialogue_events[]` - `camera_events[]` -
`vfx_events[]` - `gameplay_events[]` - `completion_flag` -
`skippable_after_first_view` - `next_state`

### Tipos

**FULL_CINEMATIC:** jogador sem controle; composição cinematográfica.
**HYBRID:** alterna shots e pequenos trechos controláveis.
**IN_ENGINE:** câmera/atores dentro da fase; ideal para diálogos e
eventos curtos. **MEMORY:** pode alterar FPS, paleta, câmera, som e
lógica de cenário. **MONTAGE:** sequência curta de imagens, reportagens
ou passagem de tempo.

### Regras de implementação

1.  Nunca carregar progresso narrativo apenas dentro da animação: ao
    finalizar/pular, aplicar todas as flags necessárias.
2.  Skip só depois da primeira visualização, salvo opção de
    acessibilidade.
3.  Ao pular, restaurar posições, inventário, NPCs e estado de câmera.
4.  Gameplay deve começar e terminar em poses compatíveis com o
    primeiro/último frame.
5.  Letterbox opcional; não usar em todo evento.
6.  Fade e glitch devem ser eventos independentes, não baked
    permanentemente na arte.
7.  Diálogo deve permanecer localizável fora do vídeo.
8.  Para cenas importantes, separar background, personagem, foreground,
    luz e VFX para permitir parallax.

## 3. TEMPLATE DE PROMPT VISUAL

Use este prefixo antes dos prompts específicos:

> Cinematic 2D pixel-art scene for a Brazilian supernatural horror
> investigation game set in Varginha, Minas Gerais. Mature atmospheric
> pixel art, grounded Brazilian architecture and objects, dramatic but
> believable lighting, strong silhouettes, subtle film composition,
> detailed environment, restrained supernatural horror, no generic
> spaceship aesthetic, no text, no watermark. Characters must match the
> approved character reference sheets exactly; do not redesign faces,
> hair, clothing or body proportions. Compose in separate visual depth
> layers suitable for parallax animation.

Para backgrounds gerados por IA, acrescentar: \> Environment/background
emphasis, leave clean readable space for manually drawn pixel-art
character sprites in foreground. No prominent human characters.

Para memória: \> Fragmented childhood-memory aesthetic, unstable
exposure, subtle frame discontinuity, dreamlike but still readable,
visual information intentionally incomplete.

Para entidade: \> Never fully explain the creature's anatomy; use
silhouette, partial reflections, occlusion and unnatural light. Avoid
stereotypical grey-alien imagery unless explicitly required by the
scene.

------------------------------------------------------------------------

# CUTSCENES POR FASE

## FASE 01 --- O CASO DE VARGINHA

### CS01 --- Reportagens de 1996

**Tipo:** MONTAGE. **Duração:** 35--50 s.

**Storyboard:** 1. preto + áudio de rádio; 2. CRT acende; 3.
recortes/reportagens fictícias; 4. mapa estilizado de Varginha; 5.
flashes de rua, multidão e autoridades; 6. estática aumenta; 7. câmera
revela que as imagens estão na TV da casa; 8. Edelzio criança sentado
diante dela.

**Manual:** interface da TV, manchetes legíveis, Edelzio, props
próximos. **IA:** backgrounds de reportagem e imagens atmosféricas,
depois pixelizadas/retrabalhadas. **Transição:** zoom-out da tela da TV
diretamente para gameplay.

**Prompt específico:** \> 1996 Brazilian television-news montage about
mysterious sightings in Varginha, Minas Gerais; CRT television imagery,
rainy/overcast urban streets, anxious crowds seen from distance, old
Brazilian cars, analog broadcast noise, newspaper-photo feeling,
ambiguous mysterious event, no readable generated text, ominous but
documentary-like.

### CS02 --- O Clarão

**Tipo:** HYBRID. **Duração cinematográfica:** 20--30 s, cercada por
gameplay.

Gameplay leva Edelzio até o quintal. Ao cruzar trigger: - input
desacelera; - som ambiente some; - câmera aproxima; - clarão invade
quadro; - sombra incompleta; - câmera perde foco; - frame branco; -
corte.

**Mecânica especial:** reduzir gradualmente `player_input_multiplier`
1.0 → 0.0, dando sensação de que algo assume o momento sem fingir
travamento.

**Prompt:** \> Night outside a modest Brazilian home in 1996, viewed
from a six-year-old child's perspective, overwhelming unnatural pale
light entering from the street, long impossible shadows, vegetation
frozen as if time stopped, barely visible unknown silhouette obscured by
overexposure, terrifying through uncertainty rather than gore.

### CS03 --- Timeskip

**Tipo:** TRANSITION. Clarão branco → ruído → som de despertador atual →
mesmo enquadramento aproximado, 30 anos depois. **Duração:** 5--8 s.

------------------------------------------------------------------------

## FASE 02 --- UM DIA COMUM

### CS04 --- Edelzio Adulto

**Tipo:** IN_ENGINE. **Duração:** 8--12 s. Despertador → mão desliga →
câmera abre → Edelzio levanta → controle entregue.

Sem IA necessária além de background/concept.

### CS05 --- A Chave Impossível

**Tipo:** IN_ENGINE. **Duração:** 8--15 s. Após busca, jogador retorna;
câmera destaca a chave em local já inspecionado. Edelzio hesita. Sem
explicação verbal excessiva.

------------------------------------------------------------------------

## FASE 03 --- O FUSCA

### CS06 --- Surto do Fusca

**Tipo:** HYBRID. **Duração:** 35--50 s.

Alternar direção jogável com tomadas: - rádio; - velocímetro; -
faróis; - mão na ignição; - retrovisor; - estrada vazia; - voz
infantil; - pane; - silêncio.

**Mecânica:** eventos são disparados por distância, não por vídeo único.
O trecho termina em shot cinematográfico após pane.

**Prompt:** \> Interior of an old Volkswagen Beetle driving through
Varginha at dusk/night, cinematic pixel art, dashboard lights
malfunctioning, analog radio producing static, speedometer needle moving
impossibly, headlights flickering across an empty Brazilian street,
driver tense but grounded, supernatural electrical interference,
claustrophobic framing.

------------------------------------------------------------------------

## FASE 04 --- INDUSTRIAL

### CS07 --- Apresentação da Industrial

**Tipo:** IN_ENGINE/MONTAGE. **Duração:** 15--25 s. Fusca estaciona →
fachada → alunos → laboratório → Edelzio entrando na sala. Objetivo:
estabelecer normalidade e geografia que será destruída na Fase 14.

**Prompt de background:** \> Daytime Brazilian public technical school,
grounded Minas Gerais architecture, students moving through corridors,
computer lab, ordinary warm school atmosphere, cinematic pixel-art
establishing shots, absolutely no horror elements.

------------------------------------------------------------------------

## FASE 05 --- A CAIXA DE 1996

### CS08 --- A Caixa

**Tipo:** HYBRID. **Duração:** 20--30 s. Edelzio puxa caixa antiga →
poeira → brinquedos → fotografia → recortes → desenho → fragmento de
mapa.

Depois, controle retorna para o jogador examinar cada item.

**Prompt:** \> Old childhood cardboard box opened inside a dim modern
bedroom, 1990s Brazilian toys, faded photographs, folded newspaper
clippings, childish drawings and torn map fragments, dust in flashlight
beam, intimate investigative horror, objects arranged naturally, no
readable generated text.

------------------------------------------------------------------------

## FASE 06 --- FRAGMENTOS

Sem cutscene longa obrigatória. Usar microeventos in-engine.

### CS09 --- O Mapa Completo

**Tipo:** IN_ENGINE. Fragmentos encaixam → câmera sobe → linhas/símbolos
convergem → local da mata destacado. **Duração:** 8--12 s.

------------------------------------------------------------------------

## FASE 07 --- A MATA

### CS10 --- Primeira Manifestação

**Tipo:** HYBRID. **Duração:** 15--20 s. Jogador vê movimento entre
árvores → câmera tenta acompanhar → nada → sombra surge atrás → gameplay
de perseguição começa imediatamente.

**Prompt:** \> Dense rural forest in Minas Gerais at night, wet foliage,
narrow trail, weak flashlight, something tall and anatomically unclear
partially hidden behind multiple tree trunks, only fragments visible,
unnatural stillness, cinematic pixel-art horror, no gore.

### CS11 --- Padre Fábio

**Tipo:** IN_ENGINE. Após perseguição, Edelzio cai/entra em área segura.
Uma lanterna/luz aparece. Padre Fábio: "Demorou mais do que eu
esperava." Corte curto para rosto de Edelzio.

------------------------------------------------------------------------

## FASE 08 --- A ÂNCORA

### CS12 --- O Registro

**Tipo:** IN_ENGINE + CLOSE-UP. **Duração:** 20--35 s.

Documento manualmente desenhado aparece:
`1996 / 6 ANOS / RECEPTÁCULO: EDELZIO / ÂNCORA: ESTÁVEL`.

Silêncio antes da reação.

**Importante:** documento e tipografia 100% manuais.

Fábio aparece atrás: "Então você finalmente encontrou."

------------------------------------------------------------------------

## FASE 09 --- OUZANA

### CS13 --- Apresentação de Ouzana

**Tipo:** IN_ENGINE. Ouzana trabalhando → Edelzio/Fábio entram → ela
inicialmente não quer envolvimento → evidência muda postura.

### CS14 --- Primeira reação

**Tipo:** CLOSE-UP/MONTAGE. Reagente toca amostra → fluorescência
anormal percorre material.

**Prompt:** \> Small improvised biological laboratory, anomalous sample
reacting to a chemical reagent, organic residue revealing subtle
luminous geometric patterns under controlled light, grounded scientific
equipment, eerie scientific discovery rather than magic potion,
pixel-art cinematic close-up.

------------------------------------------------------------------------

## FASE 10 --- O FUSCA MARCADO

### CS15 --- As Marcas

**Tipo:** HYBRID. Jogador borrifa manualmente pontos do Fusca. Último
ponto ativa sequência: marcas separadas começam a se conectar pela
lataria.

**Prompt:** \> Old Volkswagen Beetle in a dark garage, chemical reagent
revealing hidden luminous scars and geometric markings across metal
panels, markings feel like damage to reality rather than painted
symbols, scientists/investigators kept secondary, cinematic pixel art.

------------------------------------------------------------------------

## FASE 11 --- FALHA NO SISTEMA

### CS16 --- Corrupção

**Tipo:** IN_ENGINE. Notebook começa a corromper arquivos diante do
jogador. Imagens mudam sozinhas; datas trocam; um frame mostra algo
impossível.

Não gerar UI por IA; UI feita no jogo.

### CS17 --- Estrada para Três Corações

**Tipo:** MONTAGE curto. Fusca na estrada → placa → noite →
interferência no rádio → destino.

------------------------------------------------------------------------

## FASE 12 --- RENAN

### CS18 --- Renan encontra o impossível

**Tipo:** IN_ENGINE. Após reparo, Renan abre log/arquivo que deveria
estar destruído. Pausa. "Isso aqui não foi corrompido." Edelzio: "Então
foi o quê?" Renan olha para ele. "Alterado."

------------------------------------------------------------------------

## FASE 13 --- SINAL NA INDUSTRIAL

### CS19 --- A Reportagem

**Tipo:** IN_ENGINE/TV. Noticiário mostra perímetro da escola e
interferências. Edelzio reconhece local/alunos. Corta imediatamente para
preparação.

### CS20 --- Chegada

**Tipo:** FULL/IN_ENGINE. Fusca freia diante da escola → portão →
mochila no chão → luz piscando → corredor vazio → som distante.

**Prompt:** \> Familiar Brazilian technical school at night after a
supernatural incident, abandoned entrance, scattered backpacks and
papers, intermittent fluorescent lights, no visible bodies, subtle
damage, ominous silence, cinematic pixel-art establishing shot.

------------------------------------------------------------------------

## FASE 14 --- O CERCO À INDUSTRIAL

### CS21 --- Revelação dos Reféns

**Tipo:** IN_ENGINE. Edelzio observa sala/área através de janela ou
câmera. Alunos vivos, cercados por manifestações.

### CS22 --- Manifestação Elite

**Tipo:** TRANSITION TO BOSS. Corredor se distorce → portas alongam →
criatura se forma de sombras/anomalias → HUD volta → combate.

**Prompt:** \> School corridor folding unnaturally into itself,
fluorescent lights stretching into impossible perspective, incomplete
humanoid manifestation forming from darkness and electrical distortion,
pixel-art supernatural horror, readable combat arena, no gore.

### CS23 --- Pós-resgate

**Tipo:** IN_ENGINE. Edelzio tenta mandar alunos para segurança; eles
veem escala da situação. Cena prepara decisão posterior de ajudar.

------------------------------------------------------------------------

## FASE 15 --- 3º SISTEMAS

### CS24 --- Os Compostos

**Tipo:** MONTAGE. Ouzana prepara compostos → cada aluno recebe um →
reações diferentes → pequenos flashes dos poderes.

**Direção:** não parecer super-herói colorido; efeitos devem derivar da
mesma anomalia.

**Prompt-base para cada aluno:** \> Controlled anomalous biological
reaction around a Brazilian technical-school student, subtle
supernatural ability emerging in a way that reflects the character's
personality, same visual language as the entity's anomalies, grounded
horror-adventure rather than superhero transformation, cinematic pixel
art. Character appearance must exactly follow approved reference sheet.

Gerar prompts individuais só depois da ficha final de cada aluno.

------------------------------------------------------------------------

## FASE 16 --- ZÉ GOMES

### CS25 --- O Nome

**Tipo:** INVESTIGATION MONTAGE. Documentos deslizam no evidence board →
datas → assinaturas → fotos → o nome Zé Gomes se repete. Texto real
inserido manualmente.

------------------------------------------------------------------------

## FASE 17 --- O CASARÃO

### CS26 --- Chegada ao Casarão

**Tipo:** FULL ESTABLISHING. **Duração:** 15--20 s. Mata abre → casarão
surge → janela parece conter silhueta → relâmpago/luz → nada.

**Prompt:** \> Abandoned late-19th-century rural mansion hidden by dense
vegetation in Minas Gerais, Brazil, architecture decayed but believable,
humid night, vines, broken windows, faint unnatural illumination from
somewhere inside, cinematic 2D pixel art, supernatural dread, no gothic
European castle styling.

### CS27 --- A Fotografia

**Tipo:** IN_ENGINE. Foto/documento liga Zé Gomes a membros da Igreja.
Edelzio: "Você sabia disso?" Close em Fábio. Sem resposta. Corte.

------------------------------------------------------------------------

## FASE 18 --- O SEGREDO DA IGREJA

### CS28 --- Confissão de Fábio

**Tipo:** DIALOGUE CINEMATIC. Planos fechados, pouco movimento, peso no
diálogo. Fábio admite ter escondido registros para preservar o selo.

### CS29 --- A Linha do Tempo

**Tipo:** PUZZLE PAYOFF. Ao solucionar timeline, imagens de
1898/1996/presente sobrepõem-se. Uma animação mostra graficamente:
passagem aberta → entidade → passagem fechada → âncora. Sem explicar
tudo verbalmente.

**Prompt:** \> Symbolic layered historical montage connecting 1898, 1996
and present-day Varginha: old rural records, mysterious subterranean
opening, frightened wounded unknown being, sealing ritual/process, child
silhouette decades later, visual storytelling through overlapping
documents and memories, pixel-art horror, no readable text.

------------------------------------------------------------------------

## FASE 19 --- SEGUNDA MEMÓRIA

### CS30 --- A Criatura Ferida

**Tipo:** MEMORY/HYBRID. Reutiliza composição da Fase 1, mas muda
enquadramentos. Agora vemos criatura recuando, ferida e assustada.

**Prompt:** \> Fragmented 1996 childhood memory in Varginha, same
mysterious being previously framed as threatening now revealed as
wounded and frightened, retreating from approaching human lights, viewed
incompletely through a child's eyes, emotional ambiguity, supernatural
pixel-art horror, no explicit gore.

Termina antes de revelar o acordo.

------------------------------------------------------------------------

## FASE 20 --- VARGINHA

### CS31 --- O Colapso

**Tipo:** MONTAGE. **Duração:** 30--45 s. Cidade → apagões em cadeia →
rádio → pessoas olhando céu/ruas → Industrial → igreja → Fusca →
anomalias.

**Prompt:** \> Nighttime Varginha, Minas Gerais undergoing a
supernatural citywide blackout, Brazilian streets and architecture,
traffic lights failing, radios and televisions flickering, distant
impossible distortions, residents confused and frightened, no mass
destruction, atmospheric escalating supernatural crisis, cinematic pixel
art.

### CS32 --- Perseguição de Fusca

**Tipo:** HYBRID. Gameplay de direção intercalado com planos do
retrovisor, rodas, rua deformando e anomalia perseguindo.

------------------------------------------------------------------------

## FASE 21 --- A DESCIDA

### CS33 --- Do Humano ao Impossível

**Tipo:** ENVIRONMENTAL MONTAGE. Estrutura humana → concreto antigo →
pedra → material orgânico → geometria impossível.

**Prompt:** \> Progressive descent beneath Varginha from abandoned
human-built tunnels into ancient subterranean formations, architecture
gradually merging with organic stone and impossible geometry, old tools
and religious artifacts becoming rarer, claustrophobic cinematic pixel
art, cosmic supernatural horror.

### CS34 --- Separação do Grupo

**Tipo:** série de IN_ENGINE events. Cada aliado fica para cumprir
função concreta. Evitar "vou ficar aqui porque o roteiro quer".

Último plano: Edelzio e Padre Fábio seguindo sozinhos.

------------------------------------------------------------------------

## FASE 22 --- O ÚLTIMO DOCUMENTO

### CS35 --- 1898

**Tipo:** MEMORY/HISTORICAL MONTAGE. Desbloqueada com pistas
suficientes.

Mostra sem texto: Zé Gomes + colaboradores → descoberta → interesse →
entidade → tentativa de retorno → selamento.

**Prompt:** \> Late-19th-century Minas Gerais historical supernatural
sequence, rural Brazilian men and institutional figures discovering an
impossible subterranean phenomenon, curiosity turning into exploitation,
an unknown wounded entity trying to reach a passage, humans sealing the
route for their own interests, aged photographic/pixel-art aesthetic,
morally ambiguous, no readable text.

------------------------------------------------------------------------

## FASE 23 --- A VERDADE DE 1996

### CS36 --- Memória Completa

**Tipo:** MEMORY/HYBRID. **Duração total:** 2--4 min com pequenos
trechos jogáveis.

É a cutscene mais importante do jogo.

**Storyboard:** 1. mesma TV do início; 2. mesmo clarão; 3. Edelzio sai;
4. criatura está ferida; 5. ela não avança; 6. Edelzio se aproxima; 7.
contato visual/tátil; 8. imagens mentais da passagem; 9. homens chegam;
10. medo; 11. processo do selo; 12. Edelzio criança aceita/participa
conforme cânone final; 13. memória fragmenta; 14. Edelzio adulto abre os
olhos.

**Prompt:** \> Climactic recovered childhood memory, Varginha 1996. A
six-year-old Brazilian boy encounters a wounded incomprehensible being
in unnatural light outside his home. The being is frightened rather than
predatory and communicates through abstract images of a sealed passage
and a desire to return. Human figures approach in the distance.
Emotional supernatural horror, intimate and tragic, cinematic pixel art,
creature mostly obscured, no spaceship, no gore, no text.

### CS37 --- "A Fechadura"

**Tipo:** IN_ENGINE. Edelzio percebe função real. Evitar monólogo
enorme. Visual + poucas falas.

------------------------------------------------------------------------

## FASE 24 --- O CONFRONTO FINAL

### CS38 --- A Entidade

**Tipo:** FULL CINEMATIC. Primeira vez que jogador vê sua escala real,
mas ainda sem anatomia completamente legível.

**Prompt:** \> Monumental subterranean chamber beneath Varginha
containing a tear-like opening in reality, an ancient incomprehensible
entity partially emerging through layers of light, shadow and organic
stone, scale enormous but anatomy never fully readable, small human
silhouettes emphasizing scale, cinematic 2D pixel art, cosmic
supernatural awe and fear, no generic alien spaceship imagery.

### CS39 --- Todos Retornam

**Tipo:** MONTAGE TO GAMEPLAY. Ouzana, Renan, Fábio e alunos assumem
posições. Cada personagem usa sua função. Câmera termina atrás/ao lado
de Edelzio → HUD aparece → batalha.

------------------------------------------------------------------------

## FASE 25 --- QUEBRAR O CICLO

### CS40A --- Sacrifício

**Tipo:** ENDING. Edelzio permanece → grupo recua → câmara fecha → anos
depois, caderno. Tom: trágico/calmo.

**Prompt:** \> Tragic supernatural ending in an underground chamber,
lone adult man remaining beside a sealed impossible phenomenon as others
escape, light fading into darkness, later an old notebook left behind,
restrained melancholy, cinematic pixel art, no text.

### CS40B --- Libertação

**Tipo:** ENDING. Selo rompe → cidade apaga → Fusca abandonado → rádio.
Não mostrar destruição exagerada.

**Prompt:** \> Dark alternate ending: supernatural containment fails
beneath Varginha, city lights shutting off in waves at night, abandoned
Volkswagen Beetle on an empty street, radio static glowing faintly
inside, ominous uncertainty rather than explosive destruction, cinematic
pixel art.

### CS40C --- QUEBRAR O CICLO

**Tipo:** TRUE ENDING. **Duração:** 2--3 min.

Storyboard: 1. Edelzio completa puzzle; 2. Fábio alerta; 3. "Então é bom
eu estar certo." 4. silêncio; 5. manifestações congelam; 6. entidade
aproxima; 7. Edelzio não recua; 8. ligação visual entre os dois rompe;
9. passagem abre; 10. entidade olha uma última vez; 11. atravessa; 12.
luz fecha; 13. Edelzio continua vivo.

**Prompt:** \> True ending of a supernatural pixel-art horror game:
ancient incomprehensible being finally offered a route home through a
controlled tear in reality, adult Brazilian protagonist standing calmly
before it after decades of connection, hostile manifestations becoming
still, connection represented by subtle luminous threads breaking, being
departs rather than attacks, emotional release, eerie beauty, cinematic
2D pixel art.

### CS41 --- Epílogo

**Tipo:** MONTAGE. Ouzana → Renan → alunos → Fábio escrevendo → Edelzio
voltando à Industrial.

### CS42 --- O Rádio

**Tipo:** POST-CREDIT/FINAL STINGER. Fusca estacionado. Edelzio sai.
Câmera não acompanha. Silêncio. Rádio liga. Estática. Nova voz. Marca
diferente surge. Preto.

**Prompt:** \> Empty interior of an old Volkswagen Beetle parked outside
a Brazilian technical school in daylight or late afternoon, driver has
just walked away, ordinary peaceful atmosphere, analog radio suddenly
glowing with faint static, a tiny unfamiliar anomalous mark slowly
becoming visible on the dashboard, subtle sequel tease, cinematic pixel
art.

------------------------------------------------------------------------

# 4. PIPELINE ARTE MANUAL + IA

## Etapa 1 --- Storyboard manual

Para cada `CSXX`, criar thumbnails simples de composição. Não gerar a
cena final antes de decidir enquadramento.

## Etapa 2 --- Reference Lock

Criar folhas aprovadas: - Edelzio criança; - Edelzio adulto; - Fábio; -
Ouzana; - Renan; - cada aluno; - Fusca; - criatura/manifestações; -
ambientes recorrentes.

## Etapa 3 --- IA para composição/background

Gerar opções usando prompt-base + prompt específico. Escolher
composição, não "personagem mais bonito".

## Etapa 4 --- Pixelização/redesenho

Artista: - corrige perspectiva; - redesenha personagens; - simplifica
detalhes; - ajusta paleta; - remove artefatos; - adequa pixel density; -
cria tiles/layers quando necessário.

## Etapa 5 --- Separação de layers

Exemplo: `BG_far` `BG_near` `character_back` `character_main`
`foreground` `light` `fog` `anomaly` `particles`

## Etapa 6 --- Animação

Preferir: - parallax; - camera pan; - zoom lento; - 2--6 frames de
animação manual; - blinking; - cabelo/roupa; - luz; - partículas; -
glitch; - shake.

Não é necessário gerar vídeo inteiro por IA.

## Etapa 7 --- Integração

CutsceneController executa layers + sprites + áudio + diálogo + VFX em
tempo real.

------------------------------------------------------------------------

# 5. PROMPT PARA CODEX --- SISTEMA DE CUTSCENES

Copiar para o Codex depois que ele inspecionar a engine/projeto:

Implemente um sistema de cutscenes orientado por dados para O Segredo de
Varginha, respeitando a arquitetura existente do projeto. Não crie um
sistema monolítico e não hardcode a campanha inteira em um único script.

Requisitos: 1. suportar cutscenes FULL_CINEMATIC, HYBRID, IN_ENGINE,
MEMORY e MONTAGE; 2. timeline de eventos; 3. camera events; 4.
sprite/actor events; 5. dialogue events; 6. audio events; 7. VFX events;
8. gameplay/flag events; 9. fade/letterbox/glitch; 10. bloquear/liberar
input; 11. pular cutscene; 12. ao pular, aplicar estado final
deterministicamente; 13. marcar cutscene como vista; 14. integração com
save/load; 15. integração com ObjectiveSystem e GameState; 16. suporte a
layers de parallax; 17. suporte a callbacks para começar perseguição,
combate, puzzle ou troca de fase; 18. modo debug para iniciar qualquer
cutscene isoladamente.

Antes de implementar, inspecione o projeto e use os recursos nativos da
engine quando existirem. Crie primeiro uma prova funcional usando CS02
--- O Clarão. Depois implemente CS01 e CS06. Não implemente todas as
cutscenes até a prova ser aprovada.

Critérios de aceite: - CS02 pode ser iniciada por trigger; - input reduz
progressivamente e depois bloqueia; - câmera muda; - áudio/VFX são
disparados; - cutscene termina no estado correto; - skip produz
exatamente o mesmo estado final; - save após a cutscene preserva a
flag; - recarregar não dispara a cena indevidamente; - modo debug
permite repetir a cena.

------------------------------------------------------------------------

# 6. NOMENCLATURA DE ARQUIVOS

`CS_01_NEWS_1996` `CS_02_FLASH` `CS_03_TIMESKIP` ... `CS_40A_SACRIFICE`
`CS_40B_RELEASE` `CS_40C_BREAK_CYCLE` `CS_41_EPILOGUE`
`CS_42_RADIO_STINGER`

Arte: `CS02_BG_01` `CS02_EDELZIO_CHILD_01` `CS02_LIGHT_MASK`
`CS02_ENTITY_SHADOW` `CS02_FG_VEGETATION`

Áudio: `CS02_AMB_NIGHT` `CS02_SFX_ELECTRIC` `CS02_SFX_FLASH`
`CS02_VOX_MEMORY_01`

------------------------------------------------------------------------

# 7. CUTSCENES QUE NÃO DEVEM VIRAR VÍDEO PRÉ-RENDERIZADO

Preferir execução in-engine para: - CS04; - CS05; - CS09; - CS11; -
CS12; - CS13; - CS16; - CS18; - CS21; - CS23; - CS25; - CS27; - CS28; -
CS34; - CS37.

Isso reduz tamanho do jogo, facilita mudança de diálogo e mantém
consistência com sprites.

Cutscenes mais apropriadas para composição cinematográfica pesada: -
CS01; - CS02; - CS06; - CS20; - CS26; - CS29; - CS30; - CS31; - CS33; -
CS35; - CS36; - CS38; - CS40A/B/C; - CS41; - CS42.

Mesmo essas podem ser montadas em tempo real com layers 2D em vez de
vídeo.
