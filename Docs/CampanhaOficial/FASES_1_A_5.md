# Campanha experimental — fases 1 a 5

O menu Jogar → Iniciar campanha começa no jornal e na infância de Edelzio. Ao concluir a lembrança, **Continuar • Ato II** abre a rotina adulta. O botão Continuar do menu recupera a fase atual, as pistas, o arranjo dos puzzles e a posição salva. Iniciar campanha reinicia apenas os dois arquivos desta campanha experimental.

## Ato I — A Lembrança

**1 — O Caso de Varginha.** Casa existente em 1996, Edelzio criança, televisão/jornal/desenho/brinquedos opcionais, perda do sinal e encontro incompleto no quintal. Sem combate. Abertura com legendas e estática; a voz sintética foi removida. Os quadros são compostos em 384 × 216. Pivôs dos passos são alinhados pelo corpo e pela linha dos pés.

## Ato II — O Chamado

**2 — A Chave e a Caixa.** A mesma casa em 2026. Examine lavatório da cozinha, café, notebook e mochila. Depois investigue a caixa sob a cama. A fotografia posiciona casa, árvore e figura; clique em duas páginas para trocá-las e conferir a sequência. A solução revela “ELA AINDA ESTÁ AQUI”, libera chave/caderno e o **fragmento de mapa 1**. Saia pelo Fusca no quintal à direita. O close da caixa usa arte já existente e sobreposição do caderno; ainda não é uma animação desenhada de mãos/objetos.

**3 — Não Deixa Ela Sair.** Trecho novo de rua visto de cima, com cores e motivos do mapa existente. W/seta acima acelera, A/D ou setas laterais dirigem; S/seta abaixo recua. O rádio perde sinal gradualmente. Na pane, E abre inspeção de ignição, rádio e painel. Após observar os três, feche a inspeção: quatro segundos de silêncio antecedem o reinício. Dirija até a Industrial. A lembrança da fala infantil aparece como legenda; não há voz sintética substituta.

**4 — Entre Aulas e Pistas.** Reutiliza a fábrica do mapa da Industrial, mesas, corredores, fachada externa e alunos existentes. Sem inimigos ou jaulas neste ato. Encontre Renan na entrada, inicie a aula na mesa do professor, examine o armário dos arquivos e use o notebook. Compare os três marcos da fotografia com as plantas. A construção identificada fornece o **fragmento de mapa 2**. O close do notebook indica uma imagem alterada e conduz à fase seguinte.

**5 — O Código das 23:23.** Continuação no mesmo mapa. Amplie a foto no notebook e consulte a legenda do levantamento no mural. Organize os símbolos por I–IV e relacione círculo/triângulo aos números. O resultado revela **23:23 • DIOCESE ANTIGA**. Volte a Renan para confirmar. A conclusão do Ato II permite rever as pistas ou salvar e voltar ao menu. A Fase 6 não está implementada; nenhum final alternativo foi acrescentado.

## Controles e progresso

A mochila abre com **G** ou pelo botão MOCHILA. Mantém os ícones, itens e retratos; a aba de alunos registra conversas e atividades da Industrial. A aparência, suas oito direções e os atlas de equipamento foram recuperados do original atualizado (`5df6ec4`, incluindo o trabalho de `3034592`). Os sprites preservam as células originais de 64 × 64 e a ancoragem do corpo. Café, mochila e notebook usam as animações originais, incluindo a cadeira.

A chegada e as fases 4–5 usam **SchoolIndustrialFacadeV1**, a fachada real da Industrial em pixel art recuperada do mesmo commit original. Seus dois painéis mantêm o portão livre e ficam ocultos quando Edelzio entra na escola. A arte gerada anteriormente `IndustrialFacade.png` permanece como referência do laboratório, mas não é usada na chegada da campanha nem no mapa escolar.

Os nove alunos trabalham em postos definidos ou fazem trajetos curtos de intervalo. Cada um tem fala própria ligada aos arquivos, fotografia, símbolos ou projetor. Renan oferece tópicos de fotografia, arquivos e Ouzana. A correspondência de Ouzana é opcional; seu encontro permanece na futura Fase 10.

A Fase 3 usa um novo Fusca em pixel art topview e planos da pane em 384 × 216: rua, ignição, rádio e motor. Há efeitos originais de passos, papel, zíper, chave, digitação e partida, além de ambientes da casa, escola e rua e motor com variação de rotação. As configurações gerais controlam o áudio. A abertura usa legendas e ruído discreto, sem a voz robótica anterior.

WASD/setas movem Edelzio; E examina; Tab abre o caderno; Escape pausa ou fecha uma investigação. Os controles de condução estão indicados na Fase 3. Configurações compartilhadas ficam no menu e na pausa. O caderno reúne evidências e os dois fragmentos; o terceiro permanece associado à futura investigação na diocese.

`CampaignMemory1996.json` guarda a infância. `CampaignStory2026.json` guarda fases 2–5, rotina, inspeção da pane, evidências, puzzles e posição. Ambos ficam em `Application.persistentDataPath`, separados do laboratório e do protótipo original.

As colisões de Edelzio usam uma área pequena junto aos pés, mantendo os limites e portas dos mapas. Os sprites são alinhados em runtime; as imagens de origem permanecem preservadas. Isso corrige a ancoragem, mas não substitui uma revisão artística manual de cada pose.
