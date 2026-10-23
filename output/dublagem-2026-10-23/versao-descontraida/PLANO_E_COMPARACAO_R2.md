# Revisão descontraída das falas do Edelzio

A solicitação pede um tom mais descontraído, menos específico e com menos datas. Foram reescritas as 14 falas adultas do primeiro lote. A frase infantil foi preservada. O anexo de 57 propostas futuras continua separado, sem alterações.

## Problemas encontrados

As falas atuais repetem o registro do caderno, listam itens do inventário e contêm orientações em tom de tutorial. Uma fala menciona o ano da infância e outra recita o horário resolvido. O ID da voz depende do texto exato, portanto reescrever só o roteiro não atualiza o diálogo do jogo.

## Solução aprovada e aplicada ao jogo

Substituir as falas adultas pelos textos R2 abaixo; remover apenas o complemento falado sobre registro no caderno, preservando o registro e o save; manter o horário 23:23 no puzzle e as datas nos documentos; preservar a frase infantil e a progressão das pistas. Não antecipar a causa do sumiço nem a natureza da entidade.

O usuário aprovou o plano e pediu o nome roteiro_edelzio_definitivo.pdf. As falas foram aplicadas em CampaignHouseSequence.cs e VarginhaCampaignStage.cs. A rotina de build foi atualizada para gerar uma candidata 0.4.8-beta em pasta separada.

## Textos aprovados

### ED01 Preparando o notebook

**Antes:** Vou chegar pelo lado livre da cadeira para preparar o notebook.

**R2:** Vou dar a volta pra chegar no notebook.

### ED02 Olhando a caixa antes do sumiço

**Antes:** Minha caixa de brinquedos de 1996. Faz anos que não abro. Preciso preparar a aula.

**R2:** Minha caixa de brinquedos... Faz tempo, hein? Depois eu vejo isso.

### ED03 Deixando a chave na mesa

**Antes:** Vou deixar a chave na mesa do café. Depois pego a mochila.

**R2:** Vou deixar a chave aqui e pegar a mochila.

### ED04 Voltando à mesa

**Antes:** Deixei a chave aqui. Falta buscar a mochila no quarto.

**R2:** A chave tá aqui. Só falta a mochila.

### ED05 Percebendo o sumiço

**Antes:** A chave não está na mesa do café. Não há nada no chão ao redor. A observação ficou registrada no caderno.

**R2:** Ué... cadê a chave? Nem no chão ela tá.

### ED06 Conferindo a porta

**Antes:** O trinco continua fechado por dentro. Não abri esta porta desde que acordei. A observação ficou registrada no caderno.

**R2:** A porta tá trancada por dentro. Eu nem mexi nela.

### ED07 Encontrando o papel

**Antes:** Um pedacinho de papel envelhecido ficou preso junto ao quarto. Há um traço azul, igual ao das minhas anotações de infância. A observação ficou registrada no caderno.

**R2:** Olha esse papel velho... Esse traço azul parece o dos meus desenhos de criança.

### ED08 Antes de abrir a caixa

**Antes:** A caixa continua fechada. Antes de mexer, preciso entender o que mudou na casa.

**R2:** Calma... Antes de abrir essa caixa, deixa eu entender o que tá acontecendo.

### ED09 Ouvindo os arranhões

**Antes:** A porta não foi aberta. O papel parece vir da minha caixa... Agora escuto arranhões debaixo da cama.

**R2:** A porta não abriu... Esse papel parece ser da caixa. Pera... tem algo arranhando debaixo da cama.

### ED10 Depois de resolver a caixa

**Antes:** Guardei a chave, o caderno e o primeiro fragmento de mapa. A mensagem continua ali: ELA AINDA ESTÁ AQUI.

**R2:** Pronto, guardei tudo. Mas essa frase... Ela ainda está aqui.

### ED11 Com a chave recuperada

**Antes:** A chave está guardada comigo. Não vou deixá-la aqui outra vez.

**R2:** Dessa vez, essa chave fica comigo.

### ED12 Se tentar sair antes de investigar

**Antes:** Investigue a caixa sob a cama e organize as páginas para encontrar a chave.

**R2:** Antes de sair, preciso olhar a caixa e juntar essas páginas.

### ED13 Antes de usar o notebook do Renan

**Antes:** Vou conversar com Renan antes de usar o notebook dele.

**R2:** Deixa eu falar com o Renan antes de mexer no notebook dele.

### ED14 Depois de decifrar o horário

**Antes:** 23:23. Vou mostrar essa descoberta a Renan.

**R2:** É esse o horário... O Renan precisa ver isso.

### CR01 Voz de criança no rádio do Fusca

**Antes:** Não deixa ela sair.

**R2:** Não deixa ela sair.

## Continuidade preservada

A mochila ainda motiva sair da mesa. A chave continua ausente da mesa e do chão. A porta permanece trancada por dentro. O papel é antigo, tem traço azul e remete aos desenhos de infância. A caixa e os arranhões continuam sob a cama. A frase da página é mantida. Renan continua sendo consultado antes do notebook. A fala do horário só ocorre depois da resolução; o número continua disponível na interface.

## Arquivos e validação

O PDF definitivo, TXT, ZIP, JSON de revisão e CSV com IDs novos/anterior foram gerados. O roteiro anterior permanece disponível. As novas falas não contêm datas nem números falados. Os textos estão nos scripts de casa/escola, sem alterar os arquivos de cena, arte ou as respostas dos puzzles. O PDF definitivo está em output/pdf/roteiro_edelzio_definitivo.pdf. O ZIP contém esse mesmo nome e o TXT. A linha ED03 continua como legenda de cutscene; não foi declarada uma dublagem já integrada ou recebida.

## Conferência do PDF definitivo

O usuário aprovou a aplicação ao jogo e o nome roteiro_edelzio_definitivo.pdf. O conteúdo final foi gerado nesse caminho, renderizado novamente e conferido nas três páginas. O ZIP definitivo contém esse PDF e o TXT correspondente. O CSV e o JSON distinguem a legenda ED03 do canal de voz das outras falas.
