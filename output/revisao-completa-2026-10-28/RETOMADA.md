# CONFERÊNCIA FINAL ENCERRADA —29/10/2026

Usuário fechou o aviso. Build0.5.3-beta conferida com mouse: Configurações abriu, Voltar retornou ao menu e referência cinza rotativa mudou. Três saves JSON permaneceram idênticos por SHA256. Não foi iniciada nova história. Automação retomar-revis-o-oizlede-ap-s-renova-o REMOVIDA após concluir essa pendência. Não há trabalho a executar automaticamente às11:56:59. Relatório de entrega e native-smoke.json atualizados. ZIP com LEIA-ME atualizado recebe sufixo_QA-final, preservando ZIP anterior. Roteiro para dublador já entregue; vozes aguardam gravação. Limitações de QA permanecem expressas no relatório, sem afirmar jogada humana integral.

O restante deste arquivo é histórico; não retomar suas instruções antigas.

---
# Retomada — estado atualizado em29/10/2026

Saldo final:15% restantes; nenhum lote pesado novo. A MESMA automação foi atualizada para29/10/2026 às11:56:59 (Brasília), um minuto após reset real11:55:59. Commit da entrega:58b3aff. ZIP323227116bytes, SHA25636024b1da133c64553dfea213af25fc1d0aa98a88ffb64fd6ba46e9926d2db95.

O texto de28/10 abaixo é histórico. A revisão de código e a build de hoje já foram concluídas. NÃO repetir lotes antigos nem regenerar artes.

- Branch codex/campanha-15-fases-e-site. Checkpoint de ontem b29ef8d; commit de hoje registrado na entrega/Git.
- EditMode final458/458. Player consolidado342 casos distintos aprovados após retestes; não foi uma única execução verde. Último apoio de documentos revalidado1/1. Fontes e limitações em Docs/CampanhaOficial/ENTREGA_REVISAO_2026-10-29.md e output/revisao-final-2026-10-29.
- Igreja ampliada30% testada com percurso real e Fábio somente dentro. Casa até escola, colisões, puzzles, investigação e veículo com saída física/retry validados. Não há falhas automatizadas pendentes.
- Build0.5.3-beta: Builds/Oizlede_0.5.3-beta_2026-10-29/Oizlede - Segredo de Varginha.exe. Zero erros,2 avisos de API obsoleta. ZIP/hash/CRC em windows-package.json. Saves reais: três JSONs idênticos após abertura.
- Pedido humano novo atendido: Docs/CampanhaOficial/ROTEIRO_PARA_DUBLADOR_2026-10-29.txt, duas falas de repórter e13 de narração; gravar antes de integrar voz. Não adicionar voz sintética ou alterar diálogos existentes.
- Pendência de QA manual: o jogo final abriu e exibiu menu, mas um aviso de firewall do Windows para playerwithtests cobre a janela. Foi pedida intervenção ao usuário para Cancelar. NÃO automatizar esse aviso nem mudar permissões de rede. Assim que fechado, usar skill computer-use/sky para clicar Configurações e voltar, sem iniciar nova história/sobrescrever saves. Conferir native-smoke.json, atualizar relatório e LEIA-ME do ZIP (novo manifesto/CRC se mudar), depois remover automação obsoleta.
- Ressalva visual documentada: ator encobre planta do banco no enquadramento; interação e posição no assento aprovadas, captura desobstruída ainda não conferida. Chave e páginas com apoios corretos confirmados pelo subagente.
- Nenhum runner Unity ativo ao iniciar a build final; subagentes de arte concluídos. Preserve apps/processos do usuário. Jogo final aberto pelo agente para conferência pode continuar no menu.
- Não houve push. Preservar dirty pré-existente de URP/ProjectSettings/DocsQA/Tools package_school/output antigo e cenas de recuperação. Não usar reset/clean.

---
# Retomada da revisão completa de Oizlede — 28/10/2026

## PARADA ATUAL — lote2 salvo; retomada16:59:20

Saldo consultado ao fechar: **14% restantes**. Não começar lote pesado antes de renovar. A MESMA automação foi atualizada com sucesso para **28/10/2026 às16:59:20 (Brasília)**, um minuto após reset real16:58:20. Branch/HEAD/index do usuário preservados; checkpoint seguro em `refs/codex/checkpoints/revisao-20261028-lote2`, hash atualizado em `checkpoint-lote2-commit.txt`.

**Última orientação humana recebida durante o fechamento: “deixe maior a igreja ta pequena ainda”.** Ajuste curto aplicado: igreja **30% maior** (325×499.39024 pixels lógicos) no mesmo PNG, sem regenerar. `CampaignAncientChurch.cs` e JSON canônico agora usam rect[1172.5,10,325,499.39024], sólido[1172.5,51.6,325,418.6], porta[1334,522], contato de sorting[1334,470.2], sign2[1205,535]. Verificação estática de retângulos em `church-larger-static.json`; **NÃO houve Player/build depois desse aumento**. Primeira tarefa da próxima retomada: repetir percurso da mata/porta e captura para esse tamanho, corrigir qualquer obstrução e só depois considerar validado.

### QA final do segundo lote antes do aumento

- `physical-evidence-music-player-r2-native.xml`: **7 casos efetivos,5 aprovados,2 falhas**; cópia do resultado nativo preservada pois o Editor perdeu conexão após180s. Cinco aprovações: prólogo, passagem musical completa com pausa/volume/3D, leitura física de evidências da igreja com reload, cinco interfaces por controle e capturas720p/1080p. As duas falhas foram mata por timeout e rota da mesa em casa. Não anunciar7/7.
- Rota da chave: o ponto749,600 era livre geometricamente, mas sua célula da grade entrava no sólido da mesa. Abordagem movida para **749,620**, sem deslocar a chave da mesa. Isto foi uma correção concreta de acesso encontrada com controle real.
- Infra QA: Player deve continuar em background; `CinematicPlayerResultWriter` agora garante isso e grava XML por caso em `per-case/`. Timeout de conexão do runner ampliado180→360s para cobrir os testes físicos com deadline180s. Assertivas e timeouts dos próprios testes NÃO foram afrouxados.
- `physical-routes-r3.xml`: **3/3 aprovados**, após correção da abordagem/infra: casa sem teleporte (chave→Fusca→retorno→mesa→mochila), mata com vestígios e perseguição/ponte/porta, exterior intacto e Fábio somente dentro. Estes resultados são **anteriores ao aumento de30% pedido pelo usuário**. A captura `forest-intact-church.png` também mostra o tamanho menor anterior.
- `artes-ui/QA-RESOLUCOES.md`: quatro capturas1280×720 e1920×1080 inspecionadas pelo subagente; logo sem CASO VARGINHA, duas frases cinzas legíveis/discretas, cartões de Fábio sem cortes/sobreposição. Rotação4frases já testada separadamente; a inspeção visual não viu as outras2 nem1898/desfazer ativo.
- Continua válido `integrated-editmode.xml` **455/455**, no escopo de sua revisão. As edições posteriores compilaram no Player, mas não houve nova suíte EditMode inteira após elas.

**Ainda não há nova build/ZIP0.5.3.** O nome do EXE está configurado corretamente; a build0.5.2 anterior está desatualizada. Não gastar a margem de14% iniciando uma build agora. Após retomar, priorizar aumento da igreja, trecho restante da casa/caixa/páginas, objetos/chave do casarão e porão com movimento/save, saída física da oficina (bug identificado, não corrigido), regressão integrada e então build/ZIP/inicialização.

Os subagentes artísticos concluíram e estão ociosos. Nenhuma geração artística adicional necessária neste momento. Consultar relatório de QA visual e catálogos entregues; preservar tudo.

## Retomada automática efetiva — segundo lote (28/10, a partir de 11:58)

O heartbeat de 11:57 executou neste chat às **11:58:02**; o saldo de 5h havia renovado (100% disponível). HEAD/branch preservados. Próximo reset real informado pela conta: **28/10 às 16:58:20**, horário de Brasília; se houver nova parada por saldo, reagendar esta mesma automação para **16:59:20**. Não repetir a programação antiga de 11:57.

### Implementação adicional

- Escola: COMPARAR agora tem escolha única sobre um acontecimento não repetido, sem leitor de documentos nessa etapa. CONCLUIR usa pergunta/alternativas e mural opcional, sem reabrir recortes nem mostrar a origem automaticamente. Somente ORDENAR exige primeira leitura. IDs e resposta do destino preservados; migração dos bits antigos mantém análises já concluídas.
- Arquivos Alterados: ordem visual embaralhada por tentativa errada; IDs, seleção e ordem da tentativa persistem ao reabrir/recarregar. Solução continua bitmask3.
- Igreja exterior: `CampaignAncientChurch.cs` integra a arte já gerada, cobre somente os três elementos antigos com pavimento reaproveitado, remove seus sólidos, acrescenta sólido do edifício e acesso sul. `fabio` e `exit` continuam IDs legados, agora na porta[1334,395]. Fábio aparece somente dentro; entrar interrompe perseguição e transfere ao interior. `sign2` movido para[1205,417], fora da ponte.
- Evidências: `CampaignPhysicalEvidence.cs` integra catálogo canônico de15 posições (reaproveita livro/globo/mural existentes; desenha somente overlays faltantes), três vestígios da mata e sprites físicos na igreja/casarão/porão. Chave antiga do escritório difere da chave azul do Fusca; some ao obter `continuation.serviceKey`. Papéis/fotos continuam consultáveis. Colisores de evidências são triggers alinhados ao sprite; sólidos do suporte preservados. Pontos indoor plan/photo corrigidos conforme JSON. Ainda testar fisicamente casarão/porão.
- Música recebida: cópia sem alteração do arquivo MPEG em `Revision20261028/OEdelzio.mp3`, importação mono/streaming. `CampaignSchoolMusicCar` pertence à passagem rápida de outro Fusca na rua da chegada à escola; posição acompanha carro da composição, fonte3D com atenuação linear e Doppler0.12, volume de música/pausa, fade e limpeza. Não foi inserida na pane anterior. Validação de áudio/visual em andamento no Player.
- Prólogo: frase “Ué... que luz é aquela?” ao sair em direção ao brilho, em legenda não modal, flag persistente `lightNoticed`. Halo/reflexos e ruído elétrico aumentam ao aproximar; param ao iniciar o encontro. Respeita pausa e volume de efeitos. Feixe/figura/corte existentes preservados.
- Build configurada para versão0.5.3-beta em `Builds/Oizlede_0.5.3-beta_2026-10-28`, EXE `Oizlede - Segredo de Varginha.exe`; ainda NÃO gerada. ProductName interno preservado para os saves.

### Evidência confirmada até agora

- `lot1-player.xml`: 15 executados,13 aprovados,2 falhas em fixtures de hover/HUD que conservavam um HUD de outra cena no runner. Corrigido isolamento dos dois testes, sem mudar comportamento do produto para satisfazê-los.
- `lot1-physical.xml`: **5/5 aprovados**, incluindo correção dessas duas fixtures e movimento real na casa, biblioteca e portão da escola. Biblioteca percorreu livro/globo/painel, respeitou cadeira/estante/parede e saída; não confundir isso com auditoria integral de todos os mapas.
- `school-rules-editmode.xml`: **47/47 aprovados**, investigação escolar + casa, saves e migração.
- `school-forest-player.xml`: **6/6 aprovados**, escola completa por teclado e controle, estado parcial/reload e diário; percurso real pelas três pistas da mata, ponte e porta da igreja, perseguição encerrada ao entrar. Esse teste foi anterior aos overlays físicos da mata e igreja, que são revalidados no próximo lote.
- `integrated-editmode.xml`: **455/455 aprovados** com novos estados/regras/esquema de evidências e embaralhamento. Antes da inclusão do último teste de prólogo/alterações mais recentes de fixtures; recompilar no QA seguinte.
- `physical-evidence-music-player` primeira tentativa: **zero testes**, erro de compilação no teste novo por falta de `using UnityEngine.InputSystem.LowLevel`. Corrigido. Não contar como aprovação.
- `physical-evidence-music-player-r2`: em execução,7 casos previstos: casa sem teleporte até mochila, evidências da igreja com movimento e reload, mata com overlays/entrada, música na chegada, associações/embaralhamento, menu/cartões720p1080p e prólogo. Ler XML/summary real quando encerrar.

Os três PlayersWithTests órfãos das rodadas já concluídas foram encerrados após verificar seus ExecutablePaths dentro da cópia QA/Temp (PIDs31760,40824,20092). Unity do usuário13132, worker9956 e Hub27032 preservados. Nenhum save real do usuário foi tocado: QA mantém produto separado.

Subagentes concluíram novos lotes somente de assets: `artes-mata/technical-report.md` e `artes-evidencias/RELATORIO.md`. Não gerar novas artes antes de avaliar estas. Fonte canônica em `Assets/Resources/Varginha/Revision20261028/`.

### Pendências reais prioritárias

1. Concluir/dignosticar QA Player atual, inspecionar screenshots de menu/cartões/igreja/música e testar casarão/porão fisicamente, inclusive chave/reload.
2. Testar sequência completa da casa com controles além do trecho inicial até mochila; coletar/examinar pistas/caixa/páginas e partir. Fixtures antigas de fluxo estão sendo atualizadas conforme a mudança aprovada, sem enfraquecer assertivas de progressão.
3. Oficina: identificada saída cinematográfica que desliga Rigidbody2D e interpola para leste atravessando paredes. Ainda **não corrigida**. Propor rota de ré pelo portão sul, virar só na rua e sair pela faixa com casts físicos; preservar estacionamento/inércia/faróis e abrir apenas passagem autorizada na borda da rua. Não liberar sólidos indiscriminadamente.
4. Completar colisões e sorting em todos os percursos, clareza do Tombo e encontro/perseguição, regressão integrada das19 cenas, inventário/save/retry e apresentação com mouse.
5. Gerar/abrir/empacotar a candidata0.5.3 somente após os lotes executados passarem; ainda faltam itens para revisão COMPLETA. Não anunciar conclusão total com os455 testes ou com a build antiga0.5.2.

O texto de parada do primeiro lote abaixo é histórico e foi superado por esta retomada.

## Autorização e estado atual

O usuário solicitou revisão segundo o prompt anexado, subagentes para as artes, economia de saída e retomada automática um minuto após renovar o limite de 5 horas. **PLANO COMPLETO APROVADO: o usuário respondeu “aprovado” nesta conversa, após a apresentação do plano completo e do agendamento.** Implementação, ajustes e testes necessários estão autorizados conforme AGENTS.md; não repetir pedidos de autorização a cada etapa.

Implementação iniciada após aprovação. Root cuida da progressão/casa e diagnóstico de colisões. Os dois subagentes receberam lotes artísticos separados: igreja (novo overlay, sem integração de mapas/scripts) e UI (somente VarginhaMainMenu, CampaignGuidance, CampaignPuzzleDesign). Evitar edições concorrentes nesses arquivos.

## Checkpoint de parada antes do reset

Consulta ao terminar o primeiro lote: **16% restantes**; a margem final fica reservada ao salvamento deste checkpoint, sem iniciar outro lote pesado. A retomada automática existente continua para **11:57:43 de 28/10**. O plano está aprovado; retomar implementação, NÃO pedir nova aprovação.

Saldo após fechar e salvar o checkpoint Git/documental: **13% restantes**. Trabalho pesado interrompido. `save_lote1.ps1` terminou com código 0, gravou o checkpoint pós-lote e preservou index/branch ativos. Não executar mais lotes antes da renovação, salvo nova orientação explícita do usuário.

### Implementado neste lote

- **Casa (itens 6–9, parcialmente):** `CampaignHouseInvestigation.cs` preserva IDs antigos 0..5 e acrescenta estados explícitos 6 (chave no bolso) e 7 (retorno pela mochila). Nova ordem: preparar rotina → pegar chave na mesa → primeira interação com Fusca/lembrete → voltar e deixar chave → mochila/desaparecimento → pistas/caixa/páginas → saída. Reparo de saves conserva estados novos e progresso legado.
- `CampaignHouseSequence.cs`: todos os quatro papéis são controlados por estado, somem antes da investigação e após conclusão; chave sobre a mesa/inventário acompanha estado. Removidos o collider narrativo `Porta_Saida_Condicional` e o teleporte de save do quintal. Embarque desta sequência oculta Edelzio imediatamente, mantém carro fechado e segue a viagem sem abrir/fechar porta ou caminhada intermediária. `VarginhaCampaignStage.cs` atualiza inventário/visuais e não lembra mochila automaticamente na porta.
- **UI (itens 1–3,24–25, parcialmente):** subagente alterou `VarginhaMainMenu.cs`, `CampaignGuidance.cs`, `CampaignPuzzleDesign.cs` e teste `CampaignPuzzleDesignPlayTests.cs`. Logo recortada apenas na exibição (primeiras 576 linhas do PNG), frase cinza fonte 10 com maior contraste, ! amarelo contornado 15×15 cacheado, cartões de associação 64px e DESFAZER por destino. Não alterou solução/IDs. Relatório: `artes-ui/RELATORIO.md`.
- **Igreja (item 17, arte pronta para integração):** `Assets/Resources/Varginha/Revision20261028/AncientChurchExteriorV1.png` + JSON/metas, uma geração image_gen, PNG 1254×1254 RGBA, Point/sem mipmaps/compressão. Metadados incluem prompt/fonte/crop e sugestão de colocação; relatório e preview em `artes-igreja/`. Ainda NÃO foi integrada no Forest.png/runtime nem validada em colisão/sorting/transição. Usar JSON como fonte das coordenadas (top-left e Unity diferem). Pode exigir cutoff de alpha e cobertura localizada da arte antiga.

### Validação realizada e limite da evidência

- Primeira chamada `house-order-editmode` não executou testes: compilação falhou por referência remanescente a `_houseExitGate` em `VarginhaCampaignStage.Save`. Referência corrigida; logs preservados. Não contar a tentativa como aprovação.
- Repetição `house-order-editmode-r2`: **24/24 aprovados, zero falhas/ignorados**, filtro `Game.Tests.EditMode.CampaignHouseInvestigationTests`. Abrange regras de nova ordem, investigação, saves novos/legados e acessos do plano. Isso compila os scripts atuais, mas NÃO confirma física do jogador real, UI no Player nem a campanha completa.
- `CampaignHouseInvestigationTests.cs` foi atualizado para a nova ordem e acrescenta save/reparo dos estados 6/7. `CampaignMessagePlayTests.DoorBagAndKeyFollowTheRequiredOrderAndReleaseTheExit` foi adaptado para ida ao Fusca, ausência de gate e papéis inicialmente invisíveis, mas **ainda não foi executado**.
- Outros fixtures antigos de Player ainda podem esperar colocar a chave diretamente, lembrar a mochila na porta ou encontrar o collider removido. Atualizá-los somente após avaliar o propósito de cada teste; não desativar assertivas para esconder regressões. Verificar também testes antigos de porta/embarque agora substituídos por decisão explícita do usuário.
- Root não executou build/ZIP novo, não iniciou Player nem retomou Computer Use nesta rodada. Nenhum save do usuário foi alterado. O Unity aberto do workspace é preexistente do usuário; não encerrar. A cópia QA foi usada apenas pelo runner isolado; confirmar que não está ocupada antes de sincronizar novamente.

### Próximo passo concreto

1. Ler o summary `house-order-editmode-r2-summary.json` e os diffs deste lote; não refazer a inspeção artística.
2. Executar Player dirigido para a nova sequência da casa e UI do menu/associações, por exemplo via `Tools/QA/run_master_review.ps1`, filtro `CampaignMessagePlayTests.DoorBagAndKeyFollowTheRequiredOrderAndReleaseTheExit`, nome `house-order-player`, EvidencePath `output/revisao-completa-2026-10-28`. Usar filtro qualificado `Game.Tests.PlayMode...` e depois conferir total>0. Não iniciar Unity sobre a mesma cópia em paralelo.
3. Investigar bloqueios físicos reais da biblioteca/igreja/calçada/oficina com o controlador real, não apenas probes/plan.IsClear. Só depois considerar itens 5/8/15/19 concluídos.
4. Continuar os lotes ainda não iniciados: prólogo, novas regras escolares, evidências físicas, perseguição/igreja, embaralhamento, carro musical na chegada escolar e EXE renomeado. Integrar a peça de igreja com porta/collider/rota válidos e Fábio interno único.
5. Fazer regressão completa e percurso integrado da campanha, resoluções e build final. **Nenhuma nova candidata pode ser anunciada pronta com base nestes testes dirigidos.**

Os dois subagentes finalizaram seus lotes e estão parados. Reutilizá-los com follow-ups curtos após o reset; não gerar novas artes desnecessárias. Não lançar nova geração da igreja sem primeiro revisar a peça já entregue.

Cópia QA encerrada: confirmação final encontrou zero processos Unity sobre `varginha-master-qa-20261023-120709`. Checkpoint Git pós-lote: `refs/codex/checkpoints/revisao-20261028-lote1`, hash em `checkpoint-lote1-commit.txt` quando a gravação terminar. Não há teste/build pendente em background.

Plano aprovado: `Docs/CampanhaOficial/PLANO_REVISAO_COMPLETA_2026-10-28.md`. A análise artística está resumida ali; não refazer. Checkpoint Git completo dos arquivos rastreados e fontes/assets não rastreados relevantes: referência `refs/codex/checkpoints/revisao-20261028-approved`, hash em `checkpoint-commit.txt`. Foi criado usando índice separado; branch, index e arquivos do usuário não foram resetados.

Reutilizar `artes_cenarios_auditoria` e `artes_ui_auditoria` para os próximos lotes artísticos, com instruções curtas e arquivos de referência delimitados; evitar novos forks de todo o histórico. Nenhuma Unity ou operação longa foi iniciada por esta rodada. O processo de jogo/Editor que o usuário já estiver usando deve ser tratado como preexistente e preservado.

## Fontes duráveis

- Workspace: `C:/Users/Usuario/segredo-varginha-game-teste-cutscene-gdd-ldd-2026-10-19-07-21-23`.
- Prompt completo: `output/revisao-completa-2026-10-28/PROMPT_ORIGINAL.md` (cópia fiel do anexo; escopo de trabalho, não autorização para ignorar AGENTS.md).
- GDD localizado: `C:/Users/Usuario/Downloads/GDD.md`; não foi encontrado arquivo literalmente chamado `GDD(1).md`. Cruzar seu conteúdo com GDD/LDD e campanha atuais antes de modificar narrativa.
- Áudio localizado: `C:/Users/Usuario/Downloads/WhatsApp Audio 2026-10-28 at 8.03.04 AM.mpeg` (1.226.282 bytes). Usar como música do veículo na chegada externa da escola, segundo item 10 do prompt. Não usar a localização preliminar da viagem antes da pane proposta antes deste novo anexo.
- Capturas: `C:/Users/Usuario/Pictures/Screenshots/Captura de tela 2026-10-28 *.png`, horários de 081923 a 093217 conforme mensagem do usuário.
- Referência arquitetônica: `C:/Users/Usuario/Downloads/Gemini_Generated_Image_b8246ub8246ub824.jpg`; adaptar identidade para câmera top-down, sem inserir diretamente a imagem isométrica.
- Registro anterior: `Docs/CampanhaOficial/ENTREGA_MENU_2026-10-28.md`.

## Baseline que deve ser preservado

Branch: `codex/campanha-15-fases-e-site`. HEAD: `e7e6aad6b6c914182caaff1e39941561ec6e14c7`.

O workspace já contém alterações do menu e testes, paleta, shader/logo, empacotamento, arquivos anteriores de QA/URP/recuperação e metas criadas durante o uso do Unity pelo usuário. Não restaurar nem sobrescrever globalmente. Status inicial e diff de arquivos rastreados: `baseline-status.txt` e `baseline-tracked.patch`. Esses dois arquivos não constituem um snapshot completo dos assets não rastreados; fazer checkpoint Git e cópia dos não rastreados relevantes antes de mudanças arriscadas, após aprovação.

Campanha vigente: 14 capítulos, 19 cenas de build, IDs internos preservados. Não assumir 15 fases apenas pelo cabeçalho histórico do GDD.

A build local 0.5.2 de 28/10 contém logo dessaturada e frase rotativa ao entrar/reabrir. O ajuste mais recente de retorno de painéis está no código e passou no Player, mas essa build não foi recompilada com ele. Solicitações atuais de maior legibilidade, retirada de CASO VARGINHA e novo nome de EXE também não foram implementadas.

Testes da rodada anterior: EditMode 451/451; 22 casos distintos de Player entre menu/escola/floresta, mais repetições dirigidas. Não apresentar esses resultados como validação da nova revisão ainda não implementada.

O usuário interrompeu Computer Use na rodada anterior. Não retomar automação de janelas por conta deste checkpoint. Se a tarefa atual autorizar uma nova conferência, seguir a skill, observar estado atual e respeitar qualquer novo Escape. Não agir em avisos de segurança do Windows. Os três JSONs originais do save foram restaurados e tiveram hashes conferidos no registro anterior.

## Organização do trabalho após aprovação

1. Segurança: checkpoint, mapa de todos os itens, inventário de triggers e diagnóstico de colisões reais do jogador/veículo.
2. Progressão: prólogo/luz; chave → primeira ida ao Fusca → mochila esquecida → retorno/chave na mesa → mochila → desaparecimento/papéis → investigação → saída; acesso físico ao carro e embarque sem porta; perseguição até igreja segura; oficina.
3. Puzzles: cartões de 1898/Fábio legíveis sem trocar soluções; escola com leitura obrigatória apenas em Ordenar, Comparar por escolha única sem releitura e Concluir sem entregar destino; Livro do Tombo com evidências físicas/linguagem simples; Arquivos Alterados com IDs estáveis e ordem por tentativa.
4. Artes com subagentes: biblioteca seletiva, vestígios da mata, fachada de igreja antiga preservada, evidências de igreja/casarão/porão e chave do casarão. Reutilizar assets canônicos e registrar procedência. Não confundir chave do casarão com chave do Fusca.
5. Apresentação: trocar marcador de objetivo ciano por ! amarelo contornado ~2,5x, sem alterar marcador do jogador; frases cinzas mais visíveis e logo sem CASO VARGINHA; carro musical na escola com fonte espacial, atenuação/distância, Doppler sutil, pausa/volume/limpeza; EXE `Oizlede - Segredo de Varginha.exe` e pasta de dados correspondente, preservando identidade de saves.
6. QA: testes reais em cópia isolada, 1280×720 e 1920×1080, estados de erro/solução, save/reload, percursos físicos e campanha integrada. Testes isolados ou saves preparados não equivalem a campanha completa. Gerar build/ZIP apenas após integrar alterações e relatar limites com fidelidade.

## Política de economia e retomada

Consulta inicial: limite de 5 horas com 42% consumido / 58% restante. Renovação informada pela conta: **28/10/2026 11:56:43 America/Sao_Paulo** (14:56:43 UTC, Unix 1793199403). Retomada desejada: **11:57:43** (14:57:43 UTC).

O indicador consultável é percentual de uso da janela, não número exato de tokens. Consultar `get_usage_limits` ao fim de cada lote relevante e antes de delegação onerosa. Ao alcançar 15% restante ou menos, finalizar operações curtas já iniciadas, preservar mudanças, atualizar este checkpoint com item/arquivos/testes/processos/subagentes/pendências/próximo comando, interromper novos lotes pesados e aguardar renovação. Não consumir reset de conta ou créditos automaticamente.

Automação criada e conferida no arquivo local: **`retomar-revis-o-oizlede-ap-s-renova-o`**, heartbeat ACTIVE neste chat, execução única em 28/10 às **11:57:43 America/Sao_Paulo**. A agenda usa a hora informada pela conta e não uma estimativa relativa. Não criar duplicata. A execução local depende de o computador estar ligado e do aplicativo aberto.

Usar `node Tools/AgentOutput/compact.mjs --question ... -- executável argumentos` para testes/builds/saídas extensas. Ler código/diffs/documentos normalmente. Não lançar Unity em paralelo sobre a mesma cópia QA. Reutilizar evidências válidas sem confundir versões e não repetir testes já aprovados sem alteração relevante.

A retomada deve ler esta seção, conferir autorização, Git, processos e limites atuais. Se a renovação não ocorreu, reagendar para um minuto depois do reset realmente informado. Se ainda faltar aprovação inicial, avisar somente que a aprovação é necessária e não modificar o jogo. Quando houver autorização, continuar do ponto registrado sem refazer trabalho aprovado. Notificar apenas conclusão, mudança relevante, falha ou ação necessária do usuário.
