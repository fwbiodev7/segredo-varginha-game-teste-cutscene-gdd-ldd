# Pendências de revisão e polimento

Estados permitidos: Concluído, Parcial, Bloqueado ou Não iniciado. A leitura de código é diagnóstico, não conclusão de uma correção.

| Item | Solicitação | Estado | Solução proposta e validação |
| --- | --- | --- | --- |
| 01 | Colisões, recortes e profundidade globais | Concluído | Auditoria Player em 18 ambientes aprovada (1/1): 302 móveis, 1.018 aproximações sem penetração e 51 silhuetas visíveis dentro das faixas dimensionais. Revisei as 18 capturas e não encontrei falhas aparentes de profundidade/encaixe. Quatro props sem lado livre para o probe ficam atrás de outros colliders e não integram os percursos caminháveis: mesa redonda da fase 6, lampiões da fase 12 e torneira central da fase 18. Evidências: `world-contact-audit-player-final4.xml`, `world-audit-counts.json`, `furniture-contacts.csv`, `character-proportions.csv` e `world-audit/` em `output/revisao-final-2026-10-26/`. |
| 02 | Notebook, cadeira e escrivaninha | Concluído | Fechado em 27/10: piso reconstruído por linha com textura e iluminação locais, sombra antiga da mesa removida. Aproximação, uso e retorno aprovados no Player e inspecionados em `output/completion-2026-10-27/world-review/`. Sprites e colisores originais preservados. |
| 03 | Globo escolar dos fragmentos | Concluído | Novo globo esférico azul/dourado em placa localizada 44×64; original da biblioteca preservado, integração aditiva na fase 6. Pipeline sem erros e aviso esperado de placa opaca na borda. Escala, filtro, suporte, investigação e captura real aprovados; imagem world-review/globe-library.png inspecionada. |
| 04 | Sombra após glitch | Concluído | Limpeza de volume/poeira/áudio ao reconfigurar; desativação zera pulsos e restaura máscara e pós-processamento; transição cancelada limpa overlays e relógio. `visual-fixed.xml` passou a repetição de três glitches e recuperação (1/1); captura de antes/pico/recuperação foi comparada visualmente, e luma recuperada ficou dentro de 0,025 da cena-base. `vehicle-glitch-playmode.xml` também aprovou reconfiguração/repetição/cancelamento. Sem resíduo após recuperação. |
| 05 | Edelzio dirigindo e volante | Concluído | Compositor atual auditado no Player nas quatro poses. Mãos acompanham o volante, painel preservado e coluna conectada. Não foi necessário redesenhar a arte já alinhada. Capturas driving-pose-0 a 3 e teste do cinema aprovados. |
| 06 | Trajeto pavimentado da escola | Concluído | Caminho de chegada usa a calçada inferior e o acesso central medidos no mapa. Amostragem de todo o caminho verifica piso pavimentado e passagem livre; aprovada em book-graphics-editmode.xml (121/121). Sequência completa aprovada no Player; school-arrival-gate.png inspecionada, trajeto pelo piso pavimentado. |
| 07 | Ordem mochila → chave → saída | Concluído | Lembrança única, mochila condicionada, porta física bloqueada até a investigação e migração de saves. Regras e fluxo completo porta→mochila→pistas→caixa→páginas→saída aprovados, incluindo casa→escola. |
| 08 | Investigação intuitiva da chave | Concluído | Rastro local de papel até a cama, falas/dicas/dedução sem inferir idade nem antecipar a história. Inspeções são revisáveis; abertura e recompensa aprovadas no teste de cena e save. |
| 09 | Seleção direta das trilhas | Concluído | A/B imediato em qualquer trecho, contador, conferência única e correção sem apagar escolhas. Regras/save e navegação real por controle, edição, erro e reload aprovados. |
| 10 | Livro do Tombo visual e investigativo | Concluído | Três fragmentos reorganizáveis e associação às evidências adquiridas, sem combinação numérica que pule o puzzle. Regras, save, dicas, correções e recompensa com Ouzana aprovados; revisão visual real do Player passou nos estados inicial, resposta incorreta e reconstrução. `puzzle-design-player.xml`: 8/8 no recorte que também cobre trilhas, retorno e navegação.
| 11 | Conclusão discreta das fases | Concluído | Fechado em 27/10: 15/15 saídas/epílogo no Player, 31 capturas revisadas, próxima investigação pendente e confirmação dupla rejeitada. Viagem casa-escola aprovada; HUD da batalha e travessia final passaram no recorte adicional 3/3. Evidências em `output/completion-2026-10-27/`. |
| 12 | Física fluida do Fusca | Concluído | A oficina, a estrada legada e a pista opcional usam aceleração, frenagem/ré e resposta física em passos fixos. Testes reais no Player cobrem aceleração/coast/ré/volante na estrada e aceleração/coast/frenagem na pista; `vehicle-fixed.xml`: 3/3 aprovados.
| 13 | Fusca preso no limite inferior | Concluído | Contato separante permite afastar-se da parede inferior sem trocar collider nem reduzir a asserção. A regressão real `InitialContactWithTheBottomWallDoesNotTrapForwardMotion` passou junto dos testes da estrada/pista em `vehicle-fixed.xml` (3/3). Falhas de fixtures/dignóstico anteriores foram superadas por este recorte final.
| 14 | Áudio de higiene e café | Concluído | Café começa com a xícara na boca; higiene começa na fase de contato da animação. Fonte própria respeita efeitos/pausa/fim/cancelamento; regressão final aprovada em visual-actions-playmode.xml (16/16, inclui outros sistemas). |
| 15 | Soco com cliques rápidos | Concluído | Reproduzido salto de 6 para 4 poses a 10 FPS e dessincronia da camada visual da campanha. Duração mínima por pose e CombatFrame compartilhado corrigem ambos; buffer e dano único preservados. Núcleo e camada visual real aprovados, incluindo spam a 10 FPS, ciclo completo, buffer limitado, dano único e retorno ao idle (visual-actions-playmode.xml: 16/16 no recorte). |
| 16 | Reduzir bastante o relatório ABNT | Concluído | Nova versão de 12 páginas em output/relatorio-resumido-abnt-2026-10-23, PDF e DOCX. Mantidos cinco seções, capa, folha de rosto, listas e sumário; Times New Roman 12, 1,5 e margens 3/3/2/2. Todas as páginas renderizadas e inspecionadas, sem overflow nem campos pendentes. Versões longas preservadas. |
| 17 | Música recebida para o menu inicial | Concluído | MP3 original preservado e copiado sem conversão. Importação, streaming, repetição, canal musical, volume, fade de partida e parada ao desativar aprovados em EditMode/PlayMode. |
| 18 | Ambiente sombrio recebido para a igreja | Concluído | MP3 original preservado e copiado sem conversão. Igreja usa o novo ambiente nas três bandas, com sino separado; streaming/repetição/transição e parada ao voltar ao menu aprovados. |
| 19 | Roteiro da apresentação completa em 30/10/2026 | Concluído | Roteiro completo em `Docs/Apresentacao/ROTEIRO_COMPLETO_30-10-2026.md` e PDF de seis páginas. Fala de 12 a 15 minutos, histórico desde setembro, narrativa, sistemas, arte/áudio, dificuldades, demo e perguntas prováveis. |
| 20 | Abrir a apresentação atualizada nesta sessão | Concluído | Daily de 27/10 atualizada e verificada em dez slides, com HTML e PDF local. A versão PDF permite leitura no painel de arquivos sem navegar para `file://`. A restrição histórica do navegador integrado não foi contornada. |

## Plano de execução

1. Preservar baseline e mapear cenas, sistemas e evidências antes de alterar jogo.
2. Corrigir estados e compatibilidade de save, veículo/colisões e ataque.
3. Aplicar os três fluxos investigativos: chave, trilhas e Livro do Tombo.
4. Revisar profundidade, mesa, globo, glitch e cinematografia.
5. Integrar transições e sincronização de áudio.
6. Rodar testes dirigidos e regressões adequadas; compilar candidata isolada; realizar inspeção visual real dos trechos modificados.
7. Entregar relatório de estados com causas, arquivos/cenas, testes e limitações. Em 26/10 o usuário pediu commit e publicação no GitHub da atualização do dia; commits até `6654a58` já constam no remoto. Publicar o shader e o checkpoint atual somente depois de validar, mantendo de fora temporários, logs brutos e arquivos de recuperação.

## Aprovação

**Aprovado em 23/10/2026.** O usuário respondeu “Sim, implementar o plano completo” à pergunta que identifica estas 15 correções e este arquivo. Implementação, ajustes e testes necessários estão autorizados conforme AGENTS.md; não repetir o pedido a cada etapa. A redução do relatório é uma solicitação adicional expressa. Retomada automática agendada para 11h59, horário de Brasília.

## Evidências atuais

Retomada em 23/10 após renovação dos limites. Primeiro recorte novo: 26/26 EditMode aprovados, zero falhas e zero ignorados, em cópia isolada do projeto, produto Varginha_Master_QA_20261023. Evidência: editmode.xml e editmode.log neste diretório. Abrange importação/atribuição/configuração dos áudios e regras/save das trilhas; não comprova audição, gameplay completo nem as alterações posteriores de chave. Unity do usuário preservado. Resultados históricos permanecem válidos somente para seus recortes e versões anteriores.

## Fechamento de 27/10/2026

Os itens restantes desta revisão foram concluídos. Estado final, arquivos, testes, build e limites em `Docs/CampanhaOficial/ENTREGA_2026-10-27.md`. As entradas de 23 e 26/10 acima permanecem como histórico.
