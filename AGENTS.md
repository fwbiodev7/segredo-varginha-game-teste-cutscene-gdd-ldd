# Preferência de colaboração

Antes de implementar uma nova solicitação de alteração, inspecione o projeto e apresente um plano resumido com os problemas encontrados e as soluções propostas. Aguarde a aprovação do usuário antes de modificar o jogo. Uma aprovação já dada para o trabalho atual também autoriza os ajustes e testes necessários para concluí-lo; não peça aprovação novamente a cada etapa.

Preserve a arte pixel art original e as mecânicas, diálogos e puzzles que já funcionam. Ao entregar, informe os arquivos e cenas afetados e os testes realizados.

# Economia de saída de comandos

Para testes, builds e comandos com saída extensa, prefira `node Tools/AgentOutput/compact.mjs --question "resultado necessário" -- executável argumentos`. O wrapper guarda stdout/stderr completos em `output/agent-logs/` e devolve um resumo com código de saída, falhas, contagens e caminhos das evidências. Use o Node disponível no ambiente.

Investigue falhas com buscas direcionadas no log preservado. Não compacte leitura de código, diffs, AGENTS.md/SKILL.md ou conteúdo solicitado literalmente. Zero testes, execução incompleta e formato desconhecido não comprovam aprovação. O percentual registrado mede bytes de saída, não tokens ou economia garantida na conta. Modelo local é opcional e só deve ser declarado ativo quando configurado e verificado.
