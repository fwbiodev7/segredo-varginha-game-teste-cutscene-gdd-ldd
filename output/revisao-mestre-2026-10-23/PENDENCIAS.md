# Pendências de revisão e polimento

Estados permitidos: Concluído, Parcial, Bloqueado ou Não iniciado. A leitura de código é diagnóstico, não conclusão de uma correção.

| Item | Solicitação | Estado | Solução proposta e validação |
| --- | --- | --- | --- |
| 01 | Colisões, recortes e profundidade globais | Parcial | Regressão geométrica dos mapas iniciais passou no recorte de 121 EditMode; footprint e registro da cadeira corrigidos juntos. Falta concluir auditoria visual dos 19 ambientes e demais contatos específicos. |
| 02 | Notebook, cadeira e escrivaninha | Parcial | Cadeira avançada 16 pixels sem alterar os pixels do sprite; piso antigo removido em cópia aditiva; collider acompanha o desenho e o corpo usa o assento real. Primeiras capturas expuseram pés residuais e emenda retangular no piso. Resíduo removido; usuário pediu recortes mais suaves e simples. Acabamento ainda pendente. Candidato de oclusão das pernas está em teste, sem aprovação visual. |
| 03 | Globo escolar dos fragmentos | Concluído | Novo globo esférico azul/dourado em placa localizada 44×64; original da biblioteca preservado, integração aditiva na fase 6. Pipeline sem erros e aviso esperado de placa opaca na borda. Escala, filtro, suporte, investigação e captura real aprovados; imagem world-review/globe-library.png inspecionada. |
| 04 | Sombra após glitch | Parcial | Reconfiguração agora libera volume/poeira/áudio anteriores; desativação zera pulsos e restaura máscara/post da câmera; transição cancelada limpa overlays e relógio. Testes de repetição e cancelamento aprovados no recorte 7/7. Aparência exata da ocorrência original ainda requer comparação visual. |
| 05 | Edelzio dirigindo e volante | Concluído | Compositor atual auditado no Player nas quatro poses. Mãos acompanham o volante, painel preservado e coluna conectada. Não foi necessário redesenhar a arte já alinhada. Capturas driving-pose-0 a 3 e teste do cinema aprovados. |
| 06 | Trajeto pavimentado da escola | Concluído | Caminho de chegada usa a calçada inferior e o acesso central medidos no mapa. Amostragem de todo o caminho verifica piso pavimentado e passagem livre; aprovada em book-graphics-editmode.xml (121/121). Sequência completa aprovada no Player; school-arrival-gate.png inspecionada, trajeto pelo piso pavimentado. |
| 07 | Ordem mochila → chave → saída | Concluído | Lembrança única, mochila condicionada, porta física bloqueada até a investigação e migração de saves. Regras e fluxo completo porta→mochila→pistas→caixa→páginas→saída aprovados, incluindo casa→escola. |
| 08 | Investigação intuitiva da chave | Concluído | Rastro local de papel até a cama, falas/dicas/dedução sem inferir idade nem antecipar a história. Inspeções são revisáveis; abertura e recompensa aprovadas no teste de cena e save. |
| 09 | Seleção direta das trilhas | Concluído | A/B imediato em qualquer trecho, contador, conferência única e correção sem apagar escolhas. Regras/save e navegação real por controle, edição, erro e reload aprovados. |
| 10 | Livro do Tombo visual e investigativo | Parcial | Três fragmentos reorganizáveis e associação a evidências realmente adquiridas, sem combinação numérica que pule o puzzle. Save, dicas, correções e recompensa com Ouzana aprovados por regra/controle/cena. Falta revisão visual do painel no Player. |
| 11 | Conclusão discreta das fases | Parcial | Painéis redundantes substituídos por CONTINUAR no canto inferior; ligação narrativa, calibração final e encerramento preservados. Controle e bloqueio de avanço duplicado aprovados (6→7); falta revisar apresentação e demais transições no Player. |
| 12 | Física fluida do Fusca | Parcial | Oficina usa aceleração, inércia, freio/ré, volante progressivo/retorno, aderência, collider fixo rotativo, BoxCast e yaw salvo. Estacionamento/manobra/reload aprovados. Falta unificar estrada legada e pista opcional, ainda com controladores próprios. |
| 13 | Fusca preso no limite inferior | Parcial | Reproduzido no código anterior: contato inicial impedia afastamento. O mesmo teste passa com filtro de contatos separantes e sem troca de collider ao dar ré. Limite sólido e manobra/estacionamento aprovados na oficina; falta conferir as outras áreas dirigíveis do item 12. |
| 14 | Áudio de higiene e café | Concluído | Café começa com a xícara na boca; higiene começa na fase de contato da animação. Fonte própria respeita efeitos/pausa/fim/cancelamento; regressão final aprovada em visual-actions-playmode.xml (16/16, inclui outros sistemas). |
| 15 | Soco com cliques rápidos | Concluído | Reproduzido salto de 6 para 4 poses a 10 FPS e dessincronia da camada visual da campanha. Duração mínima por pose e CombatFrame compartilhado corrigem ambos; buffer e dano único preservados. Núcleo e camada visual real aprovados, incluindo spam a 10 FPS, ciclo completo, buffer limitado, dano único e retorno ao idle (visual-actions-playmode.xml: 16/16 no recorte). |
| 16 | Reduzir bastante o relatório ABNT | Concluído | Nova versão de 12 páginas em output/relatorio-resumido-abnt-2026-10-23, PDF e DOCX. Mantidos cinco seções, capa, folha de rosto, listas e sumário; Times New Roman 12, 1,5 e margens 3/3/2/2. Todas as páginas renderizadas e inspecionadas, sem overflow nem campos pendentes. Versões longas preservadas. |
| 17 | Música recebida para o menu inicial | Concluído | MP3 original preservado e copiado sem conversão. Importação, streaming, repetição, canal musical, volume, fade de partida e parada ao desativar aprovados em EditMode/PlayMode. |
| 18 | Ambiente sombrio recebido para a igreja | Concluído | MP3 original preservado e copiado sem conversão. Igreja usa o novo ambiente nas três bandas, com sino separado; streaming/repetição/transição e parada ao voltar ao menu aprovados. |
| 19 | Roteiro da apresentação completa em 30/10/2026 | Não iniciado | Pedido acrescentado em 23/10: criar ao terminar a revisão, com linguagem natural para falar, trajetória desde a ideia/protótipo, evolução, narrativa, tecnologias, sistemas, arte/áudio, testes, resultados, dificuldades e próximos passos. Incluir indicações de slides/demo e tempo sugerido; usar o estado final verificado, sem inventar entregas futuras. |

## Plano de execução

1. Preservar baseline e mapear cenas, sistemas e evidências antes de alterar jogo.
2. Corrigir estados e compatibilidade de save, veículo/colisões e ataque.
3. Aplicar os três fluxos investigativos: chave, trilhas e Livro do Tombo.
4. Revisar profundidade, mesa, globo, glitch e cinematografia.
5. Integrar transições e sincronização de áudio.
6. Rodar testes dirigidos e regressões adequadas; compilar candidata isolada; realizar inspeção visual real dos trechos modificados.
7. Entregar relatório de estados com causas, arquivos/cenas, testes e limitações. Nenhum push ou publicação está incluído automaticamente.

## Aprovação

**Aprovado em 23/10/2026.** O usuário respondeu “Sim, implementar o plano completo” à pergunta que identifica estas 15 correções e este arquivo. Implementação, ajustes e testes necessários estão autorizados conforme AGENTS.md; não repetir o pedido a cada etapa. A redução do relatório é uma solicitação adicional expressa. Retomada automática agendada para 11h59, horário de Brasília.

## Evidências atuais

Retomada em 23/10 após renovação dos limites. Primeiro recorte novo: 26/26 EditMode aprovados, zero falhas e zero ignorados, em cópia isolada do projeto, produto Varginha_Master_QA_20261023. Evidência: editmode.xml e editmode.log neste diretório. Abrange importação/atribuição/configuração dos áudios e regras/save das trilhas; não comprova audição, gameplay completo nem as alterações posteriores de chave. Unity do usuário preservado. Resultados históricos permanecem válidos somente para seus recortes e versões anteriores.
