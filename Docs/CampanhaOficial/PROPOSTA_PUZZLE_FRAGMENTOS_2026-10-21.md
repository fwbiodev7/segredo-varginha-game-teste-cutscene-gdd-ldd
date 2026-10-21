# Proposta — novo puzzle de Fragmentos (fase 6)

Status: aprovado pelo usuário em 21/10/2026 e implementado. Resultado em PUZZLE_FRAGMENTOS_BIFURCACOES_2026-10-21.md.

## Diagnóstico

A tela atual exige permutar três marcos e girar três setas, enquanto as pistas ficam em outros objetos. O jogador precisa entender seleção, troca, direção de saída e orientação do mapa ao mesmo tempo. O desenho da trilha pouco explica essas regras.

## Substituição proposta: encontrar uma passagem até a capela

Um mapa ilustrado ocupa o centro da tela. Árvore, rio, ponte e capela ficam em posições fixas. A árvore é o início; a entrada da capela é o destino. O jogador traça o caminho escolhendo trechos em três bifurcações, em vez de ordenar peças ou girar setas.

As três pistas recolhidas no livro, globo e painel ficam resumidas ao lado do mapa. Elas permitem reconhecer o marco inicial, a travessia utilizável e a continuação até a entrada. A adaptação conserva a descoberta da região da mata e não adiciona revelações sobre a entidade.

- Primeiro trecho ensina a selecionar um caminho e exibe uma prévia clara antes da confirmação.
- As bifurcações seguintes combinam observação do terreno e comparação das pistas. Não exigem pontos cardeais, números ou memorização de falas.
- Caminhos alternativos compartilham parte dos marcos; a solução precisa satisfazer as três evidências, não apenas reconhecer um ícone.
- O trajeto escolhido fica iluminado. Quando houver contradição, o feedback aponta o trecho conflitante e mantém o progresso anterior.
- Desfazer último trecho, recomeçar e voltar à exploração ficam sempre visíveis. Mouse, teclado e gamepad seguem os adaptadores existentes.
- Três dicas progressivas: orientar a observação, destacar uma região relevante, indicar a relação entre pista e obstáculo. A solução completa não aparece na primeira dica.

Dificuldade pretendida: mediana para o início da campanha. Meta provisória de 2–4 minutos, a validar com playtest humano; não é resultado medido.

## Integração e preservação

Manter os três locais investigados (`visited`), o desbloqueio da mata (`mapSolved`), a ordem narrativa árvore → ponte/rio → capela e os saves já concluídos. Um novo estado aditivo guarda o traçado; campos antigos não dão solução automática a partidas incompletas. Revisar as pistas, caderno, objetivos e dicas para remover instruções de permutação/rotação.

Arquivos previstos: `CampaignPuzzleDesign.cs` ou componente dedicado para a nova tela, `CampaignExpansionState.cs`, `CampaignExpansionController.cs`, textos de caderno/dicas/objetivos e testes correspondentes. Fase afetada: Fragmentos (6); preservar a entrada e progressão da Mata (7).

## Validação prevista

Verificar solução única com as três evidências, rejeição de atalhos incompatíveis, desfazer/recomeçar, dicas graduais, save/reload no meio, migração de save concluído e acesso à Mata. Testar a tela no Player, sem texto cortado e com navegação por teclado/gamepad. Atualizar daily, relatório e pendências; playtest humano continua necessário para confirmar a dificuldade.

Fontes: GDD original e LDD, seção Fase 06 — Fragmentos. O LDD pede combinar pistas para localizar a mata; o usuário autorizou propor uma mecânica diferente da versão atual.
