# O SEGREDO DE VARGINHA --- LEVEL DESIGN DOCUMENT

## 0. Objetivo deste documento

Este LDD converte o planejamento narrativo da campanha em especificação
de produção. Cada fase define função narrativa, espaço, objetivos,
fluxo, sistemas, puzzles, encontros, triggers, checkpoints, assets e
saída.

> Decisão de cânone a confirmar pela equipe: usar **1898** como ano do
> incidente de Zé Gomes. O GDD contém uma menção isolada a 1966, mas a
> estrutura narrativa e os finais usam 1898.

## 1. Pilares de level design

1.  Investigação antes da exposição: o jogador descobre antes de receber
    explicações.
2.  Terror por antecipação: anomalias anunciam perigo antes do inimigo
    aparecer.
3.  Recontextualização: locais, documentos e o prólogo ganham novos
    significados quando revisitados.
4.  Progressão sistêmica: cada ato adiciona uma ferramenta e depois
    combina ferramentas anteriores.
5.  Varginha como personagem: casa, Industrial, igreja, mata, casarão e
    subsolo devem possuir identidade própria.
6.  Combate tardio: Edelzio começa vulnerável; combate estruturado
    aparece após o Cerco à Industrial.
7.  Pistas opcionais importam: o Final Verdadeiro depende de
    investigação, não de uma escolha arbitrária no último minuto.

## 2. Loop principal

Explorar → observar anomalia → encontrar pista → registrar no
notebook/caderno → relacionar evidências → resolver puzzle → desbloquear
área/memória → sobreviver a encontro → retornar ao hub/avançar
narrativa.

## 3. Estrutura global

-   Perspectiva: 2D.
-   Hub narrativo inicial: Casa de Edelzio.
-   Hubs posteriores: Casa / Industrial / base improvisada do grupo.
-   Viagem: mapa de seleção + trechos especiais dirigíveis de Fusca.
-   Salvamento: autosave em transições, antes/depois de puzzles críticos
    e encontros; saves manuais em pontos seguros.
-   Pistas: obrigatórias + opcionais.
-   Final Verdadeiro: requer conjunto de `truth_clues`, memórias
    completas e documento final de 1898.
-   Party: desbloqueada na Fase 15; Edelzio + até 2 aliados ativos por
    missão (valor configurável).

# ATO I --- 1996

## FASE 01 --- O CASO DE VARGINHA

**Função:** prólogo, tom, mistério e primeira versão incompleta da
memória.

**Mapa/áreas:** tela de reportagens → sala → corredor → quarto → cozinha
→ quintal/portão.

**Objetivo principal:** investigar o clarão.

**Objetivos opcionais:** interagir com TV, jornal, computador antigo,
desenho infantil e janela.

**Fluxo:** montagem de reportagens → câmera entra na TV → controle de
Edelzio criança → exploração segura → primeira oscilação de energia → TV
com estática → luz pela janela → corredor altera discretamente → quintal
→ perda parcial de controle → silhueta incompleta → clarão → corte.

**Mecânicas:** movimento, interação, inspeção. Sem inventário completo.

**Puzzle:** nenhum obrigatório; microinterações ensinam linguagem
visual.

**Ameaça:** nenhuma combatível. Terror exclusivamente ambiental.

**Triggers:** `T01_tv_news`, `T02_power_flicker`, `T03_window_glow`,
`T04_yard_light`, `T05_memory_cut`.

**Checkpoint:** ao ganhar controle; antes de sair ao quintal.

**Assets-chave:** casa 1996, TV CRT, reportagens fictícias/estilizadas,
jornal, PC antigo, sprites Edelzio criança, mãe opcional/off-screen, VFX
clarão/estática, áudio rádio/TV.

**Conclusão:** `memory_1996_version=1`; timeskip de 30 anos.

# ATO II --- O RETORNO

## FASE 02 --- UM DIA COMUM

**Função:** tutorial formal e caracterização de Edelzio adulto.

**Mapa:** casa atual --- quarto, banheiro, cozinha, sala, garagem/saída.

**Objetivo:** preparar-se para trabalhar.

**Subobjetivos:** levantar → higiene → comer → pegar mochila/notebook →
localizar chave → sair.

**Fluxo:** rotina normal → tutorial de inventário → chave desaparece →
busca → pequenos objetos fora do lugar → chave reaparece em local
impossível → Edelzio racionaliza → garagem.

**Mecânicas:** inventário, objetivos, notebook básico, interação
contextual.

**Puzzle:** busca da chave usando pistas ambientais; não deve travar
jogador por muito tempo.

**Ameaça:** nenhuma.

**Triggers:** desaparecimento só ocorre após checklist mínimo;
reaparecimento após jogador inspecionar 3 pontos.

**Checkpoint:** início; chave encontrada.

**Assets:** casa atual, itens domésticos, mochila, notebook, chave,
Fusca exterior/interior.

**Conclusão:** desbloqueia viagem de Fusca.

## FASE 03 --- O FUSCA

**Função:** apresentar o Fusca como ferramenta e sensor sobrenatural.

**Mapa:** trecho viário linear/semilinear Varginha → Industrial.

**Objetivo:** chegar à escola.

**Fluxo:** direção normal → rádio falha → faróis → painel → velocímetro
impossível → voz infantil → motor morre → inspeção curta → silêncio →
carro reinicia.

**Mecânicas:** direção simplificada, sair/inspecionar veículo, eventos
de anomalia.

**Puzzle:** diagnóstico falso: jogador testa ignição/painel/radio;
nenhuma solução mecânica explica o evento.

**Falha:** colisões reiniciam trecho; anomalia não mata.

**Checkpoint:** início da viagem; carro parado; chegada.

**Assets:** estrada urbana, interior do Fusca, painel animado, rádio,
áudio Edelzio criança, VFX elétrico.

**Conclusão:** `fusca_anomaly_seen=true`.

## FASE 04 --- INDUSTRIAL

**Função:** apresentar escola e alunos antes do perigo.

**Mapa:** entrada, pátio, corredores, sala 3º Sistemas, laboratório,
sala dos professores.

**Objetivo:** completar o expediente.

**Fluxo:** chegada → conhecer NPCs → aula curta/interativa → intervalo →
pequenas interferências → aula final → saída.

**Mecânicas:** diálogo, exploração social, inspeção, relações.

**Interações opcionais:** conversar com alunos; observar objetos que
futuramente estarão destruídos/deslocados no Cerco.

**Ameaça:** nenhuma direta.

**Checkpoint:** chegada; intervalo; fim da aula.

**Assets:** tileset completo da Industrial em estado normal, sprites dos
alunos/professores, computadores, quadro, sinal escolar.

**Conclusão:** registra NPCs conhecidos e vínculos iniciais.

## FASE 05 --- A CAIXA DE 1996

**Função:** iniciar investigação conscientemente.

**Mapa:** casa atual, com foco em quarto/depósito.

**Objetivo:** descobrir a origem dos acontecimentos.

**Fluxo:** retorno → anomalia leve → procurar registros antigos → achar
caixa → examinar itens → montar primeiro documento → desbloquear painel
de evidências.

**Puzzle:** recompor recorte/documento rasgado; peças
rotacionáveis/encaixáveis.

**Colecionáveis:** foto, desenho, recorte, anotação, fragmento de mapa
1.

**Mecânicas novas:** quadro de evidências; documentos examináveis.

**Checkpoint:** caixa encontrada; puzzle concluído.

**Conclusão:** missão `reconstruct_1996_documents`.

# ATO III --- A INVESTIGAÇÃO

## FASE 06 --- FRAGMENTOS

**Função:** primeira investigação multi-local.

**Mapa:** mapa de Varginha + 3 microáreas (arquivo/biblioteca, ponto
urbano relacionado ao caso, arquivo da escola ou local equivalente
definido pela equipe).

**Objetivo:** reunir fragmentos suficientes para reconstruir o mapa.

**Fluxo:** escolher local → investigar → adquirir pista → notebook cruza
dados → repetir → puzzle final de sobreposição cartográfica.

**Puzzle principal:** alinhar fragmentos de mapa e coordenadas; solução
revela região da mata.

**Pistas opcionais:** 2 `truth_clues`.

**Mecânicas:** seleção de destino, notebook/análise, combinação de
pistas.

**Ameaça:** anomalias ambientais crescentes; sem combate.

**Checkpoint:** por local concluído.

**Conclusão:** `forest_location_unlocked=true`.

## FASE 07 --- A MATA

**Função:** primeira fase de terror prolongado e perseguição.

**Mapa:** entrada → trilha bifurcada → clareira → riacho → ruína/capela
→ esconderijo de Fábio.

**Objetivo:** seguir o mapa e localizar a origem dos registros.

**Puzzle:** símbolos ambientais indicam rota correta; percepção alta
mostra rotas falsas.

**Ameaças:** manifestação perseguidora não combatível; zonas de
esconderijo.

**Mecânicas novas:** furtividade, esconderijo, percepção/sanidade.

**Set piece:** perseguição entre árvores após primeira visão clara da
manifestação.

**Checkpoint:** entrada; clareira; pós-perseguição.

**Assets:** mata diurna/noturna, névoa, animais alterados, símbolos,
esconderijos, entidade parcial.

**Conclusão:** encontro com Padre Fábio.

## FASE 08 --- A ÂNCORA

**Função:** primeira grande revelação.

**Mapa:** esconderijo/arquivo de Fábio + pequeno subterrâneo religioso.

**Objetivo:** provar a ligação de Edelzio com 1996.

**Fluxo:** diálogo evasivo → explorar arquivos → encontrar índice →
decifrar referência → abrir compartimento → registro de Edelzio.

**Puzzle:** Livro do Tombo: combinar data, símbolo e número de registro.

**Revelação:** `RECEPTÁCULO: EDELZIO / ESTADO DA ÂNCORA: ESTÁVEL`.

**Escolha narrativa:** confrontar Fábio agressivamente ou pedir
explicação; altera confiança, não bloqueia história.

**Checkpoint:** entrada; documento encontrado.

**Conclusão:** Padre Fábio entra no grupo narrativo; objetivo localizar
especialista.

# ATO IV --- AS ANOMALIAS

## FASE 09 --- OUZANA

**Função:** introduzir explicação biológica e reagente.

**Mapa:** laboratório improvisado / área associada à pesquisa de Ouzana.

**Objetivo:** convencer Ouzana e testar amostra.

**Fluxo:** encontro → apresentar evidências → mini-puzzle de amostra →
reação impossível → Ouzana aceita ajudar.

**Puzzle:** organizar amostras/leituras conforme documentação; resultado
revela resíduo anômalo.

**Mecânica nova:** reagente, inicialmente em tutorial com usos
gratuitos.

**Checkpoint:** chegada; primeiro teste.

**Conclusão:** `reagent_unlocked=true`.

## FASE 10 --- O FUSCA MARCADO

**Função:** demonstrar reagente e aprofundar vínculo da entidade com
Edelzio.

**Mapa:** garagem/oficina + pequeno trecho de teste.

**Objetivo:** identificar e estabilizar anomalias do Fusca.

**Puzzle:** borrifar regiões com indícios; símbolos aparecem; conectar
sequência correta.

**Resultado:** marcas formam padrão relacionado ao mapa/selo.

**Mecânica:** reagente passa a ter carga limitada fora de tutorial.

**Upgrade:** estabilizador temporário do Fusca.

**Checkpoint:** primeira marca; reparo concluído.

**Conclusão:** veículo volta a ser confiável, mas continua sensor
narrativo.

## FASE 11 --- FALHA NO SISTEMA

**Função:** retirar temporariamente a principal ferramenta de
investigação e introduzir Renan.

**Mapa:** estrada → pane → destino em Três Corações.

**Objetivo:** salvar dados do notebook.

**Fluxo:** arquivos corrompem → tentar recuperação → falha → viagem →
anomalia na estrada → chegada.

**Puzzle:** recuperação parcial permite escolher 1 de 3 arquivos para
salvar imediatamente; restantes serão recuperados por Renan.

**Ameaça:** encontro curto de fuga na estrada.

**Checkpoint:** corrupção; pós-fuga; chegada.

**Conclusão:** localizar Renan.

## FASE 12 --- RENAN

**Função:** introduzir crafting/upgrades e recuperar investigação.

**Mapa:** casa/oficina de Renan.

**Objetivo:** reparar notebook.

**Puzzle:** diagnóstico modular simples; jogador ajuda Renan a
substituir/reativar componentes.

**Mecânica nova:** bancada de upgrades.

**Upgrades iniciais:** bateria notebook, capacidade reagente, lanterna,
inventário.

**Revelação:** arquivos foram alterados de maneira não convencional.

**Checkpoint:** chegada; reparo.

**Conclusão:** Renan entra no grupo e `upgrade_station_unlocked=true`.

# ATO V --- O CERCO

## FASE 13 --- SINAL NA INDUSTRIAL

**Função:** urgência e retorno de local familiar transformado.

**Mapa:** base/casa → trajeto → exterior Industrial.

**Objetivo:** verificar os alunos.

**Fluxo:** reportagem → contato falha → viagem urgente → perímetro vazio
→ sinais de ataque → mensagem/indício de reféns.

**Ameaça:** primeiras criaturas menores visíveis.

**Preparação:** seleção de equipamentos; Fábio/Ouzana/Renan como suporte
roteirizado.

**Checkpoint:** reportagem; portão da escola.

**Conclusão:** inicia cerco sem retorno ao mapa.

## FASE 14 --- O CERCO À INDUSTRIAL

**Função:** primeira grande sequência de combate/resgate.

**Mapa:** mesma Industrial da Fase 4, agora corrompida: pátio → bloco A
→ laboratório → corredores → sala 3º Sistemas.

**Objetivo:** libertar alunos.

**Estrutura:** localizar 3 selos/anomalias que mantêm portas bloqueadas
→ destruir/desativar → resgatar grupos → confronto central.

**Combate:** Edelzio usa ferramentas defensivas/armas improvisadas;
aliados oferecem habilidades roteirizadas. Não transformar em shooter.

**Puzzle:** energia da escola precisa ser redirecionada no laboratório
para abrir área dos reféns.

**Set piece:** corredor conhecido dobra sobre si mesmo devido à
percepção.

**Boss/elite:** manifestação física incompleta; vitória ocorre quebrando
pontos anômalos, não reduzindo HP apenas.

**Checkpoint:** entrada; cada grupo salvo; pré-boss.

**Conclusão:** alunos libertos.

## FASE 15 --- 3º SISTEMAS

**Função:** desbloquear party e habilidades.

**Mapa:** base segura improvisada.

**Objetivo:** testar compostos de Ouzana e organizar equipe.

**Fluxo:** consequências → alunos escolhem ajudar → testes individuais →
tutorial de party.

**Sistema:** cada aluno recebe `role`, `active_skill`, `passive_skill`,
`personality_tag`, `upgrade_tree`.

**Importante:** nomes, personalidades e poderes específicos devem ser
definidos pela equipe antes da implementação final.

**Tutorial:** arena controlada contra manifestação residual.

**Checkpoint:** antes dos testes; party montada.

**Conclusão:** `party_system_unlocked=true`.

# ATO VI --- O PASSADO

## FASE 16 --- ZÉ GOMES

**Função:** transformar mistério sobrenatural em conspiração histórica.

**Mapa:** base + arquivos + um local histórico.

**Objetivo:** reconstruir registros de 1898.

**Puzzle:** Renan recupera arquivos; jogador cruza nomes, datas e
propriedades em painel de evidências.

**Party:** primeira missão em que escolha de aliados altera
atalhos/interações.

**Pistas:** 2 obrigatórias + 2 opcionais `truth_clues`.

**Conclusão:** casarão localizado; suspeita sobre Igreja/Fábio.

## FASE 17 --- O CASARÃO

**Função:** dungeon investigativa principal.

**Mapa:** jardim → térreo → andar superior → porão → laboratório/arquivo
→ túnel.

**Objetivo:** descobrir o que Zé Gomes fazia.

**Gates:** chave física, símbolo revelado por reagente, terminal
adaptado por Renan, obstáculo solucionável por habilidade de aliado.

**Puzzles:** retratos/ordem cronológica; mecanismo de 1898; parede falsa
por reagente.

**Ameaças:** criaturas + perseguição de manifestação maior.

**Pistas opcionais:** documentos que contam que Zé Gomes tinha
colaboradores.

**Clímax:** fotografia/registro da Igreja ao lado de Zé Gomes.

**Cutscene:** "Você sabia disso?" → silêncio de Fábio.

**Checkpoint:** cada andar; porão; descoberta final.

## FASE 18 --- O SEGREDO DA IGREJA

**Função:** quebrar confiança em Fábio e revelar função real do selo.

**Mapa:** igreja → sacristia → arquivo proibido → cripta.

**Objetivo:** acessar registros originais.

**Puzzle principal:** linha temporal 1898 → 1996 → presente. Documentos
devem ser posicionados e relacionados corretamente.

**Escolha:** permitir que Fábio ajude ou afastá-lo temporariamente; muda
cenas e suporte posterior.

**Revelação:** o selo impede a entidade de sair.

**Truth clue:** registro de exploração da entidade.

**Checkpoint:** cripta; timeline concluída.

## FASE 19 --- 1996: SEGUNDA MEMÓRIA

**Função:** recontextualizar prólogo sem revelar tudo.

**Mapa:** versão alterada da Fase 1.

**Objetivo:** seguir a criatura ferida.

**Diferenças:** portas antes fechadas abrem; silhueta aparece mais cedo;
homens/vozes são sugeridos; criatura recua em vez de atacar.

**Puzzle:** seguir flashes corretos da memória; falsos caminhos
representam memória corrompida.

**Ameaça:** percepção, não combate.

**Conclusão:** memória interrompida antes do acordo.

# ATO VII --- O COLAPSO

## FASE 20 --- VARGINHA

**Função:** payoff sistêmico e escala máxima na superfície.

**Mapa:** sequência conectando 3 zonas urbanas + Fusca.

**Objetivo:** alcançar entrada subterrânea.

**Eventos:** apagão, rádio, criaturas, ruas deformadas, resgate opcional
de NPCs.

**Sistemas combinados:** Fusca, party, reagente, notebook, combate,
percepção.

**Escolhas:** ajudar civis/recuperar pistas ou seguir rapidamente; afeta
recursos e epílogo, não bloqueia final.

**Set piece:** perseguição de Fusca por anomalia espacial.

**Checkpoint:** cada zona.

## FASE 21 --- A DESCIDA

**Função:** remover segurança e levar jogador ao horror cósmico.

**Mapa:** instalação humana → túneis → ruínas → cavernas orgânicas →
antecâmara.

**Objetivo:** alcançar o selo.

**Progressão visual:** 100% humano → híbrido → quase totalmente
orgânico.

**Puzzles:** combinar tecnologia, reagente e símbolos históricos.

**Party split:** aliados ficam para trás por funções
narrativas/jogáveis, nunca aleatoriamente.

**Ameaças:** criaturas avançadas; sanidade alta altera rotas.

**Checkpoint:** transições de camada.

**Conclusão:** Edelzio + Fábio chegam à área final.

## FASE 22 --- O ÚLTIMO DOCUMENTO

**Função:** gate do Final Verdadeiro.

**Mapa:** arquivo selado de 1898 na antecâmara.

**Objetivo:** acessar registro original.

**Condição:** se `truth_clues >= threshold`, puzzle completo fica
disponível; caso contrário, jogador vê que faltam evidências e segue sem
solução completa.

**Puzzle:** usar pistas coletadas durante toda a campanha para
reconstruir motivação do selamento.

**Revelação:** entidade foi explorada/aprisionada e queria retornar.

**Flag:** `true_ending_unlocked=true`.

**Checkpoint:** documento resolvido.

# ATO VIII --- O SEGREDO DE VARGINHA

## FASE 23 --- A VERDADE DE 1996

**Função:** terceira versão e memória definitiva.

**Mapa:** Fase 1 completa, agora sem distorção enganosa.

**Objetivo:** reviver a noite real.

**Fluxo:** encontrar criatura ferida → perceber medo dela → chegada dos
envolvidos → contato com Edelzio → processo que o transforma em âncora →
memória reprimida.

**Interatividade:** jogador controla criança em momentos-chave, mas não
pode alterar o passado.

**Revelação:** Edelzio é a fechadura que impede a entidade de sair.

**Checkpoint:** início da memória; contato.

## FASE 24 --- O CONFRONTO FINAL

**Função:** prova final de todos os sistemas.

**Mapa:** câmara do selo com múltiplas plataformas/zonas.

**Objetivo:** alcançar e estabilizar núcleo enquanto grupo segura
manifestações.

**Fases do encontro:** 1. revelar pontos com reagente; 2. defender
Renan/Ouzana durante estabilização; 3. usar aliados para abrir rotas; 4.
sobreviver à distorção de percepção; 5. Edelzio alcança núcleo.

**Boss design:** entidade principal não deve ser tratada como monstro a
matar; antagonismo mecânico vem das manifestações e do colapso da
prisão.

**Checkpoint:** antes do confronto; entre macrofases.

**Conclusão:** menu/ação contextual do destino do selo.

## FASE 25 --- QUEBRAR O CICLO

**Função:** finais e epílogo.

**Final Sacrifício:** manter selo → Edelzio permanece como âncora.

**Final Libertação:** romper sem conhecimento → libertação
descontrolada/colapso.

**Final Verdadeiro:** exige `true_ending_unlocked`; puzzle final abre
passagem original e remove Edelzio da função de âncora.

**Puzzle verdadeiro:** alinhar três camadas --- configuração de 1898 +
frequência/anomalia de 1996 + estabilização atual de Ouzana/Renan.

**Set piece:** grupo segura criaturas enquanto jogador executa
sequência.

**Resultado verdadeiro:** entidade reconhece Edelzio, conexão rompe,
passagem abre e ela retorna.

**Epílogo:** Ouzana pesquisa resíduos; Renan estuda tecnologia; alunos
voltam à Industrial; Fábio registra verdade; Edelzio retorna ao
trabalho.

**Último plano:** câmera dentro do Fusca → rádio liga sozinho → nova voz
→ símbolo diferente aparece → corte.

------------------------------------------------------------------------

# 4. SISTEMAS NECESSÁRIOS

## 4.1 Player Controller

-   movimento 2D;
-   corrida;
-   interação contextual;
-   inspeção;
-   esconderijo;
-   estados bloqueados por cutscene;
-   dano/status;
-   percepção.

## 4.2 Interaction System

Interface `IInteractable` para portas, itens, NPCs, documentos,
terminais, puzzles, Fusca e esconderijos.

Campos mínimos: `interaction_id`, `prompt`, `required_flags`,
`required_item`, `on_interact_event`.

## 4.3 Game State / Flags

Estado persistente separado do estado da cena.

Categorias: - `story_flags`; - `collected_clues`; - `truth_clues`; -
`memory_state`; - `npc_relationships`; - `unlocked_locations`; -
`inventory`; - `party_roster`; - `upgrades`; - `ending_flags`.

## 4.4 Objective System

Objetivos com estados `locked/active/completed/failed`. Suporte a
objetivo principal, subobjetivo e opcional.

## 4.5 Inventory

Slots limitados; itens-chave podem ocupar categoria separada para evitar
softlock. Tipos: consumível, ferramenta, puzzle, documento, chave,
reagente.

## 4.6 Evidence / Notebook

Abas: - documentos; - fotos; - mapa; - pessoas; - linha temporal; -
arquivos; - quadro de conexões.

Pistas precisam possuir IDs e relações definidas em dados, não hardcoded
na UI.

## 4.7 Puzzle Framework

Base reutilizável com: - estado inicial; - regras; - solução; -
feedback; - reset; - save state; - evento `on_solved`.

Templates: documento rasgado, senha/código, símbolos, mapa, timeline,
circuito, reagente, associação de evidências.

## 4.8 Dialogue System

Nós, escolhas, condições por flags, eventos, portraits e localização.
Escolhas podem alterar confiança e flags sem criar ramificações
impossíveis de manter.

## 4.9 Cutscene / Timeline System

Controle de câmera, animação, diálogo, áudio, VFX, bloqueio de input e
flags. Toda cutscene deve ser pulável depois de vista uma vez.

## 4.10 Anomaly Director

Sistema central para anomalias. Eventos: luz, rádio, objeto deslocado,
áudio, glitch, mudança de tile, corredor impossível, spawn, percepção
falsa. Parâmetros por fase: intensidade, frequência, tipos permitidos,
scripted/random.

## 4.11 Perception/Sanity

Não usar como simples barra de "loucura". Variável de exposição
influencia apresentação e rotas falsas; pistas reais importantes nunca
podem ficar permanentemente inacessíveis.

## 4.12 Reagent System

Carga, spray/área de detecção, objetos `Revealable`, estado revelado
persistente e feedback visual/sonoro.

## 4.13 Fusca System

Trechos dirigíveis específicos, entrar/sair, painel, rádio, falhas
roteirizadas, upgrades e anomalias. Não criar mundo aberto de direção se
não houver necessidade.

## 4.14 Stealth/Chase

Linha de visão, audição simplificada, esconderijos, chase volumes, rotas
e reset por checkpoint.

## 4.15 Combat

Introduzido plenamente no Ato V. Ataque/defesa/esquiva conforme direção
final de gameplay. Inimigos devem ter estados e fraquezas
ambientais/anômalas. Evitar transformar toda criatura em saco de HP.

## 4.16 Party

Edelzio + aliados selecionados. Dados por aliado: `id`, `role`,
`active_skill`, `passive`, `cooldown`, `personality_tag`,
`upgrade_tree`, `availability_flags`.

## 4.17 Upgrade System

Bancada de Renan. Categorias: notebook, reagente, lanterna/ferramentas,
inventário, Fusca, equipamentos de combate.

## 4.18 Enemy AI

State machine: idle → patrol → suspicious → investigate → chase →
attack/search → return. Variantes podem ignorar alguns estados.

## 4.19 Save/Load

Salvar: fase, checkpoint, flags, pistas, inventário, upgrades, party,
puzzles persistentes, escolhas, final desbloqueado. Nunca salvar somente
posição do jogador.

## 4.20 Audio

Áudio é sistema de gameplay: rádio, estática, pistas direcionais,
stingers, vozes, silêncio dinâmico e música por estado.

------------------------------------------------------------------------

# 5. DADOS E CONVENÇÕES

## IDs

Fases: `LVL_01_CASE_1996`. Pistas: `CLUE_###`. Pistas verdadeiras:
`TRUTH_###`. NPC: `NPC_NAME`. Eventos: `EVT_LEVEL_DESCRIPTION`. Puzzles:
`PZL_LEVEL_NAME`. Checkpoints: `CP_LEVEL_01`.

## Estrutura sugerida de dados

`LevelDefinition` - id - display_name - scene - act - objectives -
available_party - anomaly_profile - checkpoints - required_flags -
completion_flags

`ClueDefinition` - id - title - description - category - optional -
truth_clue - relations\[\]

`PuzzleDefinition` - id - type - required_items\[\] -
required_flags\[\] - solution_data - completion_flag

------------------------------------------------------------------------

# 6. ASSETS NECESSÁRIOS

## Ambientes

Casa 1996; casa atual; Industrial normal/corrompida; ruas; estrada;
mata; igreja/cripta; laboratório Ouzana; oficina Renan; casarão;
Varginha em colapso; instalação subterrânea; cavernas orgânicas; câmara
final.

## Personagens

Edelzio criança/adulto; Padre Fábio; Ouzana; Renan; alunos do 3º
Sistemas; NPCs secundários; criatura/entidade; manifestações/inimigos.

## Props

TV CRT, PC 1996, notebook, Fusca, jornais, fotos, documentos, Livro do
Tombo, frascos/reagente, equipamentos de laboratório, ferramentas,
objetos religiosos, computadores da escola, portas/armários/chaves,
itens de puzzle.

## UI

HUD mínimo; interação; inventário; notebook; evidence board; mapa;
objetivos; diálogos; party; upgrades; reagente; save/load; opções;
créditos.

## VFX

clarão, estática, glitch, interferência elétrica, marcas reagente,
distorção de percepção, névoa, ruptura, partículas orgânicas.

## Áudio

ambientes por local, rádio, TV, estática, Fusca, passos, portas, escola,
floresta, criaturas, vozes de memória, combate, stingers e trilha.

------------------------------------------------------------------------

# 7. ORDEM REAL DE PRODUÇÃO

Não implementar as 25 fases sequencialmente antes dos sistemas-base.

## Milestone 1 --- Vertical Slice

Construir Fases 01, 02, 03 e parte da 05. Sistemas: player, interação,
diálogo, inventário, documentos, objetivos, save, cutscene, anomalias
básicas, Fusca scripted.

**Meta:** provar atmosfera, arte, narrativa e loop de investigação.

## Milestone 2 --- Investigação

Fases 05--10. Adicionar notebook/evidence, puzzles, percepção,
stealth/chase e reagente.

## Milestone 3 --- Party/Combat

Criar protótipo isolado antes das Fases 13--15. Adicionar combate, AI,
party e upgrades.

## Milestone 4 --- Conteúdo intermediário

Fases 11--19.

## Milestone 5 --- Endgame

Fases 20--25, finais e epílogos.

## Milestone 6 --- Polish

Balanceamento, áudio, acessibilidade, performance, bugs, save migration,
QA e créditos.

------------------------------------------------------------------------

# 8. DEFINITION OF DONE DE UMA FASE

Uma fase só está pronta quando: - pode ser iniciada a partir do menu de
debug; - objetivos podem ser concluídos; - não possui softlock
conhecido; - checkpoints restauram estado corretamente; -
diálogos/cutscenes funcionam; - pistas persistem; - puzzles persistem; -
transição para próxima fase funciona; - áudio/VFX mínimos estão
implementados; - assets temporários estão identificados; - há teste de
caminho crítico; - há teste de reload em cada checkpoint.

# 9. PONTOS A DEFINIR PELA EQUIPE

1.  Engine e versão.
2.  Linguagem/framework.
3.  Resolução e câmera.
4.  Pixel art: tamanho de tile e sprite.
5.  Lista final dos alunos do 3º Sistemas.
6.  Personalidade, classe e poder de cada aluno.
7.  Modelo exato de combate.
8.  Quantidade máxima de aliados ativos.
9.  Se Ouzana realmente examinou a criatura no universo ficcional e como
    isso será apresentado.
10. Localizações reais vs. versões ficcionalizadas.
11. Ano canônico de Zé Gomes: recomendação atual, 1898.
12. Duração-alvo total.
13. Quantidade exata de `truth_clues` para Final Verdadeiro.
14. Conteúdo que será vertical slice para feira/demo.
