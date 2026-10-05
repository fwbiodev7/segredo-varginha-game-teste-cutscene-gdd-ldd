# Revisão das fases 11–21

## Diagnóstico e alterações

As screenshots mostraram colisão apenas nos pés diante de pilares, barreiras ausentes no casarão/porão, água com volume de colisão abrangendo passarelas e recortes de objetos trazendo piso para a frente dos atores. A proteção do corpo e a navegação usam agora as mesmas paredes; móveis usam seu contato frontal no chão e contornos de recorte. Colunas compridas possuem trechos de profundidade independentes, compartilhando a textura original.

| Fase | Correções / preservação |
|---|---|
| 11 | Proteção do corpo nas divisórias; revisão de profundidade e caminhos. Escola existente, cadeiras e conversas preservadas. |
| 12 | Colisões das divisórias do térreo, recortes dos móveis e leitura das entradas. Jardim, escritório, chave e porta de serviço preservados. |
| 13 | Barreiras e pilares do porão corrigidos; pontos das páginas e materiais alcançáveis fora dos objetos. Ordem dos registros e memória de 1898 preservadas. |
| 14 | Igreja existente preservada, incluindo a escala original do padre e do Edelzio. Revisão de acesso às evidências e puzzle. |
| 15 | Casa infantil existente preservada. Revisão de caminhos, puzzle e memória. |
| 16 | Água bloqueia seus próprios tanques sem bloquear passarelas; proteção nas paredes internas. Padre proporcional aos adultos; leitura da água acessível no piso seco. |
| 17 | Arte mais nítida nas três câmaras, sem mudar limites do mapa. Colisão das grades, reguladores acessíveis e profundidade por trecho. Puzzle acoplado e abrigo preservados. |
| 18 | Arte de contenção mais nítida, preservando acessos laterais. Grades corrigidas; evidências e Ouzana posicionadas em chão livre. |
| 19 | Quintal e clarão existentes preservados; revisão de acesso à memória e sequência do acordo. |
| 20 | Entrada gradual do chefe, poses e dois padrões de ataque, reação a dano e mudança de ritmo. Hitboxes do corpo, efeitos acima do chefe, maior orçamento com reaproveitamento, equipe de três alunos/um apoio e comandos individuais. |
| 21 | Padre proporcional também no retorno. Travessia, encerramento do selo e retorno à Industrial preservados. Créditos maiores sobem sobre preto de tela inteira e terminam com sugestão de mistério. |

As fontes anteriores dos mapas 17/18, imagens geradas, recorte técnico e prompts ficam em [ArtSources/Polish20261004](../../ArtSources/Polish20261004/manifest.json). A câmara de alta resolução é compartilhada pelas três áreas da fase 17. Não foram incluídas fotografias pessoais nas fontes.

## Verificação

O relatório de navegação abrange 15 áreas: 11, 12/112, 13, 14, 15, 16, 17/117/217, 18, 19, 20 e 21/121. Todos os spawns e pontos do layout têm rota livre. Os testes de gameplay exercitam pistas/puzzles, troca de áreas, socos no corpo, 18 habilidades, seleção por mouse/teclado, comandos individuais, limite/reuso dos ecos, morte/repetição e travessia com recarga do save.

Resultado: **5/5 testes de gameplay do escopo aprovados em 148,57 s**. Depois do ajuste de enquadramento da entrada, o teste de arena foi repetido e passou (**1/1**, 12,29 s), incluindo câmera sobre o chefe e controles bloqueados durante a antecipação. O reposicionamento final de Ouzana na fase 18 foi conferido visualmente e possui piso livre e rota acessível.

Capturas das 15 áreas, entrada, equipe, impacto e créditos ficam em `Preview/Polish11a21`. Relatórios: `ValidacaoPolimento11a21PlayMode20261004.json`, `ValidacaoPolimentoEntradaPlayMode20261004.json`, `ValidacaoPolimentoNavegacao20261004.json` e `ValidacaoPolimentoConsole20261004.json`. A compilação terminou sem erros; a conferência do Console não encontrou erros ou warnings de gameplay nesta revisão. O progresso real do jogador foi restaurado na fase 21, etapa de encerramento concluída, antes dos créditos.

Não se declara ausência universal de bugs nem aprovação de testes antigos fora deste escopo. Duas expectativas visuais antigas do Edelzio, fora desta suíte, continuam exigindo atualização de referência; isso não foi ocultado alterando baselines nesta revisão.

## Material pendente

Falta a gravação da voz infantil do rádio da fase 16. O texto e ambiente existentes continuam disponíveis. Nenhum outro asset novo ou decisão narrativa é necessário para estas correções.
