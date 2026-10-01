# O SEGREDO DE VARGINHA --- PACOTE DE IMPLEMENTAÇÃO PARA CODEX

## REGRA PRINCIPAL

Não tente implementar as 25 fases de uma vez. Trabalhe por milestones e
mantenha o jogo executável após cada mudança.

## 1. CONTEXTO

Jogo 2D de terror sobrenatural, investigação e mistério ambientado em
Varginha. O protagonista, Edelzio, investiga acontecimentos ligados a
1996 e descobre que foi transformado em âncora de uma entidade. O jogo
começa focado em exploração/investigação e só introduz combate
estruturado mais tarde.

## 2. ANTES DE CODIFICAR

Inspecione o repositório inteiro e responda com: 1. engine/versão
detectada; 2. estrutura atual; 3. cenas existentes; 4. sistemas já
implementados; 5. assets existentes; 6. dependências; 7. riscos; 8.
plano incremental.

Não substitua sistemas existentes funcionais sem necessidade. Não
invente APIs da engine. Não apague assets. Não faça refatoração ampla
fora do escopo.

## 3. ARQUITETURA OBRIGATÓRIA

Separar: - dados/configuração; - estado persistente; - lógica de
gameplay; - apresentação/UI; - conteúdo específico de fase.

Evite lógica narrativa gigante dentro de um único script.

Sistemas desejados: PlayerController InteractionSystem GameState
ObjectiveSystem InventorySystem EvidenceSystem DialogueSystem
PuzzleFramework CutsceneController AnomalyDirector PerceptionSystem
ReagentSystem Vehicle/FuscaSystem StealthSystem CombatSystem PartySystem
UpgradeSystem EnemyAI SaveSystem AudioManager LevelFlowManager

Use os padrões e recursos nativos apropriados da engine encontrada.

## 4. PRIMEIRA ENTREGA --- FOUNDATION

Implemente somente: - player movement; - interação; - GameState/flags; -
objetivos; - inventário básico; - diálogo básico; - documentos
examináveis; - checkpoint/save; - LevelFlowManager; - menu debug para
carregar fases/checkpoints.

Crie testes ou ferramentas de debug adequadas à engine.

### Critérios

-   iniciar jogo;
-   mover;
-   interagir;
-   coletar item;
-   atualizar objetivo;
-   abrir documento;
-   salvar;
-   carregar;
-   trocar de cena;
-   restaurar flags.

## 5. SEGUNDA ENTREGA --- VERTICAL SLICE

Implementar `LVL_01_CASE_1996`, `LVL_02_NORMAL_DAY`, `LVL_03_FUSCA` e
núcleo de `LVL_05_BOX_1996`.

Use placeholders quando asset final não existir.

### LVL_01

TV/reportagem → exploração → oscilação → clarão → quintal →
corte/timeskip.

### LVL_02

rotina → tutorial → chave desaparece → busca → chave reaparece → saída.

### LVL_03

trecho de Fusca → rádio/faróis/painel → pane → voz → retorno do motor →
chegada.

### LVL_05 parcial

caixa → documento rasgado → coleta → Evidence/Notebook básico.

## 6. TERCEIRA ENTREGA --- INVESTIGAÇÃO

Depois que vertical slice estiver aprovado: - Evidence Board; -
Notebook; - PuzzleFramework; - AnomalyDirector; - Perception; -
Stealth/Chase; - Reagent.

Então implementar Fases 05--10.

## 7. QUARTA ENTREGA --- COMBATE

Antes de integrar à campanha, criar sandbox de combate: - player
combat; - enemy state machine; - ally; - active skill; - cooldown; -
party selection; - encounter controller.

Somente depois integrar Fases 13--15.

## 8. DADOS

Não hardcode pistas e objetivos em UI.

Criar definições equivalentes a:

LevelDefinition: id, displayName, scene, act, objectives, requiredFlags,
completionFlags, anomalyProfile, checkpoints.

ObjectiveDefinition: id, text, type, requiredCount, optional,
completionFlag.

ClueDefinition: id, title, body, category, optional, truthClue,
relations.

PuzzleDefinition: id, type, requiredItems, requiredFlags, solutionData,
completionFlag.

CharacterDefinition: id, displayName, role, activeSkill, passiveSkill,
personalityTag, upgradeTree.

## 9. FLAGS INICIAIS

`memory_1996_version` `fusca_anomaly_seen` `box_1996_found`
`evidence_system_unlocked` `forest_location_unlocked`
`edelzio_anchor_revealed` `reagent_unlocked` `renan_joined`
`upgrade_station_unlocked` `industrial_students_rescued`
`party_system_unlocked` `church_truth_revealed` `true_ending_unlocked`

## 10. CONVENÇÕES

`LVL_XX_NAME` `CP_XX_NAME` `EVT_XX_NAME` `PZL_XX_NAME` `CLUE_XXX`
`TRUTH_XXX` `NPC_NAME`

## 11. REGRAS DE QUALIDADE

-   nenhum puzzle obrigatório pode causar softlock;
-   item-chave não pode ser descartado de modo irreversível;
-   cutscene vista deve poder ser pulada;
-   save deve persistir puzzle/flags;
-   toda fase deve abrir por debug;
-   toda transição deve ter fallback;
-   logs úteis apenas em dev;
-   dados narrativos devem ser localizáveis;
-   controles remapeáveis se a engine/projeto suportar;
-   usar placeholders claramente nomeados;
-   não adicionar dependência externa sem justificar.

## 12. COMO TRABALHAR A CADA PEDIDO

Antes de editar: 1. identifique arquivos afetados; 2. descreva plano
curto; 3. implemente menor fatia funcional; 4. execute validações/testes
disponíveis; 5. corrija erros; 6. liste arquivos alterados; 7. explique
como testar manualmente; 8. indique próximo passo.

## 13. PRIMEIRO COMANDO PARA O CODEX

Leia o repositório e este documento. Não implemente ainda as 25 fases.
Faça uma auditoria técnica e proponha a arquitetura mínima necessária
para o Milestone 1/Vertical Slice, reaproveitando tudo que já existe.
Depois implemente apenas a Foundation descrita neste documento, mantenha
o projeto executável e informe exatamente como testar cada sistema.
