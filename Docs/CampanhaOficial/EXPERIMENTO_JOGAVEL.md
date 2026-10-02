# Experimento jogável — 01/10/2026

O laboratório reaproveita a construção da escola do protótipo, a câmera ortográfica 2D, movimento, colisões, animação e mochila de Edelzio. Os layouts originais da casa, escola e igreja não foram substituídos. A fachada fotográfica serve de referência para uma imagem da cutscene; não altera a perspectiva topview do mapa.

## Validação desta entrega

Compilação no Unity 6000.6.0f1. Nove testes de EditMode passaram, cobrindo dependências dos puzzles, respostas incorretas, salvamento parcial/corrompido, pausa/pular e limites das configurações. Um teste de PlayMode passou com deslocamento real pela tecla D após a abertura, sprite infantil, ausência de combate, pausa/retorno do controle e pista da TV salva. A janela Game foi maximizada e a reprodução corrigida foi conferida com o Console sem erros ou avisos. As imagens da entrega estão em `Preview/UnityMenu.png` e `Preview/UnityPhase1Fixed.png`.

O teste separado do laboratório foi incluído, mas o comando do Editor não chegou a executar casos nessa rodada; não conta entre os dez testes aprovados. A campanha completa, um build executável e as 19 fases seguintes não foram validados nesta entrega.

## Entrar na campanha

O menu principal agora possui somente **Jogar** e **Configurações**. Dentro de Jogar, **Iniciar campanha** cria uma nova memória e **Continuar** recupera a exploração salva. Iniciar campanha apresenta os seis primeiros planos da abertura (55 segundos), encerrando na televisão da criança, sem antecipar a escola ou o salto temporal.

Depois aparece **Ato I — A Lembrança / Fase 1 — O Caso de Varginha**. Edelzio tem seis anos, um sprite infantil com quatro direções e passos animados, e explora a casa existente. Pode examinar televisão, jornal, desenho e brinquedos. A televisão perde sinal, a iluminação oscila lentamente e o objetivo aponta para o quintal, pela porta da direita. Ao sair, há silêncio e uma presença parcialmente visível, seguida do corte que encerra a memória. Não há combate nem puzzle obrigatório nessa fase. Ela termina indicando o próximo ato, que ainda permanece planejado.

Escape pausa. Pode continuar, alterar configurações ou salvar e voltar ao menu. `CampaignMemory1996.json` guarda posição, objetos examinados, abertura vista e conclusão. É independente do laboratório e do progresso do protótipo original.

Configurações: remapeamento de teclado/mouse no menu principal, dificuldade das fases com combate, volumes geral/ambiência/efeitos/vozes, resoluções 16:9, janela/sem bordas/tela cheia, sincronização vertical, limite 60/120 FPS, legendas e tamanho, indicações de interação e redução de distorções/clarão. Alterações são persistentes. Exibição é aplicada pelo botão próprio; no Editor o tamanho da janela Game é controlado pelo Unity. Na pausa, o remapeamento completo é indicado como disponível no menu principal.

## Entrar no laboratório de investigação

Abra a cópia no Unity 6000.6.0f1. Use `Varginha > Experimentos > Abrir laboratório`, depois Play. A cena é `Assets/Scenes/Laboratorio_GDD_Cutscene.unity`. Essa ferramenta reúne puzzles de atos posteriores para testes; não é a Fase 1 da campanha.

Ande com WASD/setas. Perto de Renan, pressione E. Os botões do laboratório abrem as investigações, a galeria e a abertura. Escape fecha as janelas de investigação. As ações de combate permanecem nas fases originais; o laboratório da escola desativa inimigos e jaulas para testar investigação.

## O que existe nesta versão

- Abertura de 70 segundos, nove planos: transmissão, reportagem ficcional de 1996, testemunho, documentos contraditórios, interferência, memória infantil, clarão lento, salto temporal e fachada da Industrial em 2026. Quadros compostos em 384 × 216, atualizados a 12 fps; legendas, pausa, pular, som e redução de distorções. É uma prévia com ilustrações e efeitos, sem animação completa de personagens.
- Voz temporária sintética portuguesa do Windows, sem imitação de vozes pessoais, e estática discreta.
- Fragmentos do mapa: trocar duas peças até conectar rio, estrada e igreja; libera documentos.
- Documentos: ordenar registros históricos, mantendo 1898 e 1996 como épocas distintas; libera o código.
- Fotografia: decifrar a sequência de símbolos e abrir o próximo caminho de investigação. Tentativas incorretas não destroem pistas.
- Renan no mapa da Industrial; aparência do Padre Fábio adaptada no mapa existente da igreja; quatro personagens na galeria, usando as referências fornecidas. Ouzana ainda não possui fase jogável própria. Os novos corpos são estáticos; as animações existentes de Edelzio continuam reutilizadas.
- Diálogo da igreja corrigido para Edelzio como selo vivo ligado a 1996.

O arquivo `ExperimentGDDLDDSave.json` é separado do progresso tradicional. Guarda a abertura vista, peças parcialmente montadas e pistas resolvidas. **Novo teste** reinicia apenas o experimento. A página auxiliar usa seu próprio armazenamento no navegador, independente do Unity.

## O que permanece planejado

As 20 fases em seis atos, a revelação completa do pacto, o encontro jogável com Ouzana, novos mapas futuros e o único Final Verdadeiro estão descritos no planejamento. Completar os três puzzles conclui esta amostra e indica a rota seguinte; não executa o final da campanha. As fases antigas continuam com seus sistemas de combate/resgate para comparação.

## Importação no Windows

Prefira um caminho curto para a cópia. Nesta máquina, `V:\` aponta para ela temporariamente. A primeira importação no caminho extenso encontrou arquivos de pacote acima do limite usual do Windows; o cache foi guardado e reconstruído. O script `Tools/OpenLaboratory.ps1` usa esse caminho curto e abre o laboratório. Não aplica mudanças ao projeto principal.
