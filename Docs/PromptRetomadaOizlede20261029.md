# OIZLEDE: FRAGMENTO PERDIDO
## PROMPT MESTRE DE RETOMADA — REDESIGN DOS ALUNOS, COMBATE, MOCHILA, CRÉDITOS CINEMATOGRÁFICOS E BUILD FINAL

Retome o desenvolvimento de **Oizlede: Fragmento Perdido** neste mesmo chat, workspace e repositório, aproveitando todo o código, assets, documentação, histórico de desenvolvimento, GDD, LDD e funcionalidades já existentes.

Esta rodada tem como objetivo realizar uma atualização geral dos personagens aliados, integrar suas novas artes ampliadas, melhorar os sprites de combate, implementar os ataques especiais e transformar os créditos do jogo em uma verdadeira sequência cinematográfica.

**O objetivo final é entregar uma build Windows estável, jogável e pronta para ser transportada por pendrive e apresentada em outro computador.**

Trabalhe com autonomia, mas preserve todas as funcionalidades já aprovadas.

Não reinicie o projeto, não substitua sistemas funcionais por versões simplificadas e não altere a narrativa sem necessidade.

---

# ETAPA 1 — AUDITORIA INICIAL E SEGURANÇA

Antes de iniciar qualquer implementação:

1. Inspecione o estado atual do repositório.
2. Confirme a versão do Unity e os sistemas utilizados.
3. Identifique a última build estável.
4. Localize todos os personagens, sprites, retratos, animações, controladores e poderes existentes.
5. Analise o sistema de combate e as interações com inimigos.
6. Investigue a estrutura atual do menu da mochila.
7. Localize a implementação atual dos créditos.
8. Verifique a existência de testes automatizados.
9. Identifique assets duplicados, obsoletos e referências quebradas.
10. Crie um checkpoint Git antes de modificar qualquer coisa.

Não apague assets que possam estar referenciados por cenas, prefabs, animações ou recursos de carregamento dinâmico.

Registre o estado inicial e defina quais alterações realmente precisam ser feitas.

Utilize commits pequenos, organizados por funcionalidade e sempre que possível após validações.

**Regra importante:** não considere uma funcionalidade concluída apenas porque o código foi escrito. Ela deverá funcionar no jogo.

---

# ETAPA 2 — INTEGRAÇÃO DAS NOVAS ARTES DOS ALUNOS

Criei novas artes ampliadas em pixel art dos personagens e enviarei todos os arquivos PNG.

Essas imagens representam os retratos definitivos dos alunos e deverão ser usadas principalmente no **menu da mochila**.

## 2.1. Personagens

Integre as artes dos seguintes 12 alunos:

1. Fábio
2. Matias
3. Anna Sabiá
4. Ana Tavares
5. Pedro
6. Yasmin
7. Luís Martins
8. Luís Messias
9. Marcos
10. Moscardini
11. Bianca
12. Giovana

Cada personagem deve receber seu retrato correto, com os traços individuais preservados.

## 2.2. Padrão visual obrigatório

Utilize a arte ampliada da Bianca como principal referência do estilo adotado.

Todos os retratos devem manter:

- Estilo pixel art consistente.
- Contornos pixelados nítidos.
- Proporções faciais semelhantes às artes aprovadas.
- Enquadramento do peito para cima.
- Camiseta escolar branca com mangas verdes.
- Gola e detalhes verdes.
- Pequeno emblema verde da camiseta.
- Iluminação e sombreamento coerentes.
- Expressões neutras ou fiéis às referências.
- Resolução adequada à interface.
- Fundo transparente, quando necessário para a UI.

Preserve as características individuais de cada aluno.

Não altere cabelo, olhos, tom de pele, formato do rosto ou acessórios sem justificativa.

### Atenção especial às referências

**Yasmin:** cabelo preto, longo e cacheado; olhos azul-esverdeados.

**Giovana:** cabelo preto liso, franja, presilha rosa e olhos castanhos, conforme o sprite de referência.

**Marcos:** preservar o cabelo cacheado com burst fade nas laterais da versão aprovada.

**Moscardini:** respeitar o cabelo castanho-escuro volumoso e bagunçado, com franja lateral, conforme o sprite enviado. Não utilizar o cabelo do Marcos.

Para os outros personagens, seguir rigorosamente os PNGs individuais fornecidos, sem inventar características.

## 2.3. Importação no Unity

Configurar adequadamente os PNGs:

- Texture Type: Sprite (2D and UI).
- Filter Mode: Point.
- Compression: None, quando tecnicamente apropriado.
- Generate Mip Maps: Off.
- Alpha transparente preservado.
- Pixels Per Unit coerente com os sprites existentes.
- Sprite slicing correto para arquivos que contenham múltiplos frames.

Não ampliar imagens com filtros bilineares que provoquem borrões.

Mantenha os originais preservados e organize os recursos em pastas identificáveis.

Não gere retratos substitutos para personagens cujas artes ainda não tenham sido enviadas. Registre a dependência e continue trabalhando nas demais etapas.

---

# ETAPA 3 — REDESIGN EXCLUSIVO DOS SPRITES DA YASMIN

**Refaça completamente apenas os sprites básicos da Yasmin**, utilizando sua nova arte ampliada como referência visual definitiva.

Os demais personagens devem manter seus sprites normais existentes.

Apenas os sprites das habilidades e animações de ataques deverão ser criados ou aprimorados para todos os alunos.

## 3.1. Sprites da Yasmin

Reconstrua as animações necessárias para o funcionamento atual:

- Idle.
- Caminhada.
- Corrida.
- Ataque.
- Dano recebido.
- Morte.
- Interação.
- Utilização do piano.
- Animações especiais já existentes.

Preserve:

- Cabelo preto cacheado.
- Olhos azul-esverdeados.
- Camiseta branca e verde.
- Proporções corporais existentes.
- Altura consistente em todos os quadros.
- Movimento natural dos cabelos.
- Posição dos pés e pivôs.
- Identidade visual de sua arte ampliada.

Crie as oito direções reais caso o sistema de movimentação necessite delas:

N, NE, E, SE, S, SW, W e NW.

Não invente quadros direcionais desnecessários, mas também não substitua direções importantes por espelhamentos inadequados.

## 3.2. Integração

Atualize corretamente:

- Sprite Renderer.
- Animator Controller.
- Animation Clips.
- Transições de estados.
- Eventos de animação.
- Colisores.
- Pontos de origem dos ataques.
- Referências nos prefabs.

Teste o movimento da Yasmin dentro das cenas existentes para confirmar que o redesign não alterou sua escala, posição ou comportamento.

---

# ETAPA 4 — SPRITE SHEETS DOS ATAQUES DOS 12 ALUNOS

Crie ou atualize as folhas de sprites dos poderes especiais dos 12 personagens.

**Os novos poderes são apenas os de Moscardini, Bianca e Giovana. Os demais deverão manter suas habilidades originais.**

Cada habilidade deve possuir animações visualmente reconhecíveis, efeitos próprios e integração correta ao sistema de combate.

## 4.1. Moscardini — SONO PROFUNDO

Novo poder: fazer o inimigo adormecer temporariamente.

### Animação

Moscardini executa um gesto de sonolência, liberando uma pequena onda de energia que se dirige ao inimigo.

Quando o poder atinge o alvo:

- Aparecem partículas pixeladas em formato de `Z`, `z` e `zZ`.
- O inimigo entra no estado de sono.
- Seus movimentos são interrompidos.
- A IA interrompe temporariamente perseguições e ataques.
- O inimigo permanece adormecido por alguns segundos.
- Ao acordar, recupera seu comportamento normal.

Criar uma animação visual clara para o início, duração e encerramento do estado de sono.

Chefes e inimigos especiais devem possuir resistência ou imunidade conforme o balanceamento.

Não permitir que o poder cause travamentos permanentes na IA.

## 4.2. Bianca — CHUVA DE CONHECIMENTO

Novo poder: lançar livros de estudo contra os inimigos.

### Animação

Bianca realiza um movimento de arremesso e conjura ou lança livros pixelados.

Efeitos obrigatórios:

- Livros com capas e páginas identificáveis.
- Animação do lançamento.
- Rotação dos livros durante o percurso.
- Trajetória em direção ao inimigo.
- Colisão e impacto.
- Partículas de papel.
- Reação do inimigo ao receber o golpe.

Criar uma pequena sequência de livros, respeitando o tempo de recarga e o balanceamento.

O poder deverá ter uma estética divertida e relacionada à escola.

## 4.3. Giovana — PROTEÇÃO ARCANA

Novo poder inspirado na magia dourada de **Mel Medarda**, da série Arcane.

Criar uma habilidade original baseada no conceito de barreira mágica, sem copiar diretamente assets protegidos da série.

### Animação

Giovana levanta as mãos e invoca símbolos mágicos dourados.

Uma barreira de energia surge ao redor dela.

A habilidade deverá:

- Gerar partículas douradas.
- Apresentar símbolos mágicos pixelados.
- Criar um escudo temporário.
- Repelir inimigos próximos.
- Bloquear ataques compatíveis.
- Refletir determinados projéteis durante uma janela curta.
- Emitir um efeito luminoso quando um golpe é bloqueado.
- Desaparecer com uma animação própria.

Aplicar duração limitada e cooldown apropriado.

A barreira não pode criar invulnerabilidade permanente.

O visual deve ser elegante, mágico e consistente com o universo de Oizlede.

## 4.4. Fábio — HALTERES DE ACADEMIA

Manter o poder existente: arremessar halteres nos inimigos.

Criar ou atualizar:

- Animação de preparação.
- Arremesso.
- Sprite do halter.
- Movimento do projétil.
- Colisão.
- Efeito de impacto.

Preservar o funcionamento e o dano originais, exceto quando ajustes técnicos comprovadamente necessários forem identificados.

## 4.5. Matias — JIU-JÍTSU

Manter os golpes de jiu-jítsu existentes.

Criar sprites representando:

- Postura de combate.
- Preparação.
- Aproximação.
- Agarrão ou técnica já implementada.
- Execução.
- Recuperação.

Garantir que os personagens não atravessem paredes, inimigos ou objetos durante as técnicas.

Não criar novas mecânicas obrigatórias que substituam o poder original.

## 4.6. Anna Sabiá — PINGUE-PONGUE

Manter sua habilidade de pingue-pongue.

Criar ou atualizar:

- Movimento da raquete.
- Bolinha pixelada.
- Animação do golpe.
- Trajetória da bolinha.
- Impacto no inimigo.

## 4.7. Ana Tavares — PINGUE-PONGUE

Manter o mesmo conceito de poder, mas preservar a identidade e os sprites próprios de Ana Tavares.

Criar suas animações de raquete, golpe e projétil.

Caso já exista uma habilidade combinada entre Anna Sabiá e Ana Tavares, mantê-la funcional.

Não introduzir uma dependência obrigatória entre as duas se ela não existir no sistema original.

## 4.8. Pedro — GUITARRA

Manter o poder original da guitarra.

Criar ou atualizar:

- Animação tocando.
- Movimento das mãos.
- Ondas sonoras.
- Notas musicais pixeladas.
- Efeito de impacto.
- Feedback visual do ataque.

Sincronizar a animação com os efeitos sonoros disponíveis.

## 4.9. Yasmin — PIANO

Manter o poder original do piano.

Criar as animações correspondentes ao uso do instrumento, respeitando o redesign da Yasmin.

Adicionar:

- Piano pixelado, se necessário à mecânica existente.
- Movimento de execução.
- Notas musicais.
- Efeitos sonoros visuais.
- Animação de impacto.

O instrumento não deverá atravessar o cenário nem aparecer incorretamente posicionado.

## 4.10. Luís Martins — ARTES COLORIDAS

Manter os golpes artísticos existentes.

Melhorar as animações utilizando:

- Pinceladas coloridas.
- Traços artísticos.
- Partículas de tinta.
- Efeitos multicoloridos.
- Impactos visuais coerentes com o ataque.

Preservar o conceito de golpes baseados em arte.

## 4.11. Luís Messias — CANTO E MICROFONE

Manter o poder original de canto com microfone.

Criar:

- Animação segurando o microfone.
- Movimento cantando.
- Ondas sonoras.
- Notas musicais.
- Partículas.
- Efeitos de impacto.

Sincronizar a animação com os sons existentes.

## 4.12. Marcos — XINGAMENTOS E BOLADA DE VÔLEI

Preservar os dois poderes.

### Ataque 1 — Xingamentos

Criar animação de Marcos gritando ou falando.

Adicionar:

- Balões pixelados.
- Símbolos cômicos de censura.
- Ondas de impacto.
- Reações dos inimigos.

Evitar textos desnecessariamente ofensivos.

### Ataque 2 — Bolada de vôlei

Criar:

- Preparação do ataque.
- Movimento do braço.
- Bola de vôlei pixelada.
- Trajetória da bola.
- Impacto contra o inimigo.
- Reação ao acerto.

Preservar danos e cooldowns existentes.

---

# ETAPA 5 — PADRONIZAÇÃO DAS ANIMAÇÕES DE COMBATE

Todos os ataques devem seguir os mesmos padrões técnicos e artísticos.

Produzir, quando necessário:

1. Preparação.
2. Execução.
3. Projétil ou efeito principal.
4. Impacto.
5. Recuperação.

Não definir uma quantidade fixa e arbitrária de quadros. A quantidade deverá respeitar a complexidade de cada movimento.

## Requisitos técnicos

- PNG com transparência.
- Quadros uniformemente organizados.
- Sprite slicing adequado.
- Pivôs consistentes.
- Animation Clips corretamente configurados.
- Transições de Animator funcionais.
- Eventos sincronizados com hitboxes.
- Efeitos que respeitam profundidade e ordem de renderização.
- Escala visual consistente.
- Sem distorções ou quadros cortados.
- Sem referências perdidas.

Sempre que possível, reaproveitar a arquitetura existente.

Não criar vários sistemas independentes de combate para resolver problemas semelhantes.

Organize as habilidades por personagem, mantendo responsabilidades claras no código.

Use configurações reutilizáveis, como ScriptableObjects, quando isso se integrar bem à arquitetura existente.

## IA e física

Verificar:

- Detecção de inimigos.
- Aplicação de dano.
- Estados temporários.
- Cooldowns.
- Hitboxes.
- Interrupção de ataques.
- Reações a impactos.
- Recuperação da IA.
- Limpeza de projéteis e partículas.
- Compatibilidade com inimigos especiais e chefes.

O ataque de sono do Moscardini não deverá causar estados permanentes na IA.

O escudo da Giovana deverá terminar corretamente.

Os livros da Bianca devem desaparecer depois de atingir um alvo ou exceder sua duração.

---

# ETAPA 6 — REDESIGN DO MENU DA MOCHILA

Atualize o menu da mochila para apresentar as novas artes dos alunos de forma mais organizada, bonita e legível.

A interface deve manter o estilo pixel art de Oizlede.

## Layout

Ao selecionar um aluno, apresentar:

- Retrato ampliado.
- Nome do personagem.
- Poder especial.
- Descrição breve da habilidade.
- Informações já existentes, quando aplicáveis.

Criar um enquadramento consistente para os retratos.

Manter espaços adequados e evitar elementos sobrepostos.

## Melhorias visuais

- Animação suave na troca dos personagens.
- Destaque discreto para o aluno selecionado.
- Transições de fade quando apropriadas.
- Tipografia pixel art legível.
- Cores coerentes com o menu principal.
- Melhor organização das informações.
- Identidade visual consistente entre os 12 alunos.

Não redesenhe desnecessariamente a mochila inteira caso sua estrutura já funcione bem.

Priorize a integração das artes, a legibilidade e o acabamento.

## Validação

Testar em:

- 1280 × 720.
- 1920 × 1080.

Verificar seleção, carregamento, posicionamento dos retratos, salvamento e restauração de estados.

---

# ETAPA 7 — REDESIGN CINEMATOGRÁFICO DOS CRÉDITOS

Esta etapa deverá receber atenção especial.

**Quero transformar os créditos de Oizlede: Fragmento Perdido em uma sequência cinematográfica semelhante aos créditos finais de filmes.**

Não quero uma simples lista estática de nomes.

Os créditos deverão subir lentamente pela tela, com animações próprias, transições suaves, trilha sonora apropriada e uma apresentação visual que valorize todas as pessoas envolvidas.

## 7.1. Créditos oficiais

Utilize exatamente as equipes e integrantes abaixo.

### ARTE

Yasmin  
Luís Martins  
Giovana

### SONOPLASTIA

Luís Messias  
Gustavo

### DESENVOLVEDORES

Marcos  
Fábio  
João Pedro  
Asafe

### TESTES E QA

Moscardini  
Anna Sabiá  
Ana Tavares  
Karol  
Pedro

### NARRATIVA

João Guilherme  
Bianca

### EM HOMENAGEM A

Edelzio  
Ouzana  
Professor Fábio  
Renan

### SUPERVISIONADO POR

Renan  
Alex

Preserve exatamente a divisão, os nomes e a acentuação.

Não omita pessoas e não altere a ordem das equipes.

A repetição de Renan nas seções de homenagem e supervisão é intencional e deverá ser mantida.

## 7.2. Identidade visual dos créditos

Criar uma tela escura, elegante e coerente com a atmosfera do jogo.

Utilizar:

- Fonte em pixel art no estilo Minecraft.
- Textos claros, preferencialmente brancos.
- Destaques suaves em verde.
- Pequenos detalhes dourados quando apropriados.
- Títulos de equipes maiores do que os nomes.
- Alinhamento centralizado.
- Espaçamento adequado.
- Partículas pixeladas discretas.
- Fundo com profundidade visual sutil.
- Transições suaves.

Não utilizar fontes pixeladas borradas.

Se uma fonte apropriada não estiver disponível ou licenciada no projeto, usar uma alternativa compatível e registrar a escolha.

## 7.3. Abertura cinematográfica

Quando os créditos forem iniciados pelo menu ou após o encerramento do jogo:

1. A tela escurece gradualmente.
2. A trilha instrumental começa suavemente.
3. O logotipo de Oizlede: Fragmento Perdido aparece no centro.
4. Pequenas partículas se movimentam lentamente no fundo.
5. O logotipo recebe uma iluminação discreta.
6. Depois de alguns segundos, desaparece com fade-out.

Em seguida, apresentar:

**UMA HISTÓRIA CRIADA POR ALUNOS**

Depois:

**OIZLEDE: FRAGMENTO PERDIDO**

Esses textos deverão aparecer individualmente com transições suaves.

Após a abertura, iniciar a rolagem dos créditos.

## 7.4. Rolagem cinematográfica

Os créditos devem entrar pela parte inferior da tela e subir progressivamente até desaparecerem na parte superior.

A velocidade deve ser lenta, contínua e legível.

Cada equipe deverá aparecer com o seguinte formato:

**NOME DA EQUIPE**

Nome do integrante  
Nome do integrante  
Nome do integrante

Depois de um espaço visual adequado, começa a próxima equipe.

Não utilizar efeitos exagerados.

Aplicar fades discretos nas bordas superior e inferior.

## 7.5. Animações próprias

Criar animações para:

- Entrada dos títulos.
- Rolagem vertical.
- Surgimento das partículas.
- Transição entre os blocos.
- Encerramento dos créditos.
- Aparição do logotipo final.

O fundo pode apresentar um leve efeito parallax, desde que não prejudique a leitura.

As partículas deverão utilizar sprites pixelados e movimentos sutis.

A velocidade da rolagem deverá ser independente do FPS do computador.

## 7.6. Destaque especial para as homenagens

Após a seção de Narrativa, inserir um espaço maior antes dos créditos de homenagem.

Apresentar:

**EM HOMENAGEM A**

Edelzio  
Ouzana  
Professor Fábio  
Renan

Esse bloco deverá transmitir maior solenidade.

Reduzir levemente a velocidade visual da apresentação, sem comprometer a fluidez.

Aplicar iluminação discreta ou partículas suaves.

Em seguida, apresentar:

**SUPERVISIONADO POR**

Renan  
Alex

Destacar esses nomes com o mesmo cuidado visual.

Não transformar a homenagem em uma cena excessivamente longa.

O resultado deve ser emocionante, porém elegante.

---

# ETAPA 8 — ENCERRAMENTO ESPECIAL DOS CRÉDITOS

Este será um dos momentos visualmente mais importantes do jogo.

Após todos os nomes subirem e desaparecerem, criar uma sequência final própria.

## 8.1. Transição

A tela escurece lentamente.

A música diminui de intensidade por um breve momento.

Pequenas partículas verdes começam a surgir no centro da tela.

Essas partículas se movimentam suavemente, como pequenos fragmentos luminosos pixelados.

## 8.2. Aparição do logotipo

O logotipo oficial de **Oizlede: Fragmento Perdido** aparece lentamente no centro.

Utilizar o PNG oficial existente, preservando sua identidade visual.

Adicionar:

- Fade-in gradual.
- Brilho sutil.
- Partículas verdes flutuando.
- Leve movimento ou efeito de profundidade.
- Iluminação suave.
- Ausência de distorções ou borrões.

O logo deverá ser o elemento principal da cena.

## 8.3. Homenagem ao professor Edelzio

Logo abaixo do logotipo, apresentar a frase:

**"Uma homenagem ao professor Edelzio, que inspirou esta jornada."**

O texto deverá aparecer lentamente, com tipografia pixel art legível.

Essa mensagem deverá permanecer visível por alguns segundos, permitindo que o público a leia com tranquilidade.

A homenagem deve reforçar a origem do projeto: uma história que nasceu de brincadeiras dentro da sala de aula, do folclore de Varginha e da vontade dos alunos de divertir, aprender e homenagear seu professor.

A sequência deverá ser emocional e respeitosa, sem exageros dramáticos.

## 8.4. Mensagem final

Após a homenagem, apresentar:

**FEITO COM CRIATIVIDADE, APRENDIZADO E MUITA DEDICAÇÃO.**

A frase aparece e desaparece suavemente.

Depois, manter o logotipo centralizado e exibir:

**OBRIGADO POR JOGAR!**

As partículas continuam se movimentando discretamente.

O quadro final permanece visível por alguns segundos.

Após isso, permitir o retorno ao menu principal, sem interromper saves ou estados importantes.

---

# ETAPA 9 — IMPLEMENTAÇÃO TÉCNICA DOS CRÉDITOS

Antes de reconstruir os créditos, inspecione como funcionam atualmente.

Reutilize componentes e scripts existentes sempre que possível.

Implementar uma arquitetura robusta utilizando os recursos adequados do Unity.

Componentes possíveis:

- Canvas.
- Canvas Scaler.
- TextMeshPro.
- RectTransform.
- ScrollRect ou sistema equivalente.
- RectMask2D.
- CanvasGroup.
- Animator.
- Timeline, se realmente necessário.
- Sistema de partículas 2D.
- AudioSource.

Separe os dados dos créditos da lógica de apresentação.

Permita editar os nomes e equipes sem precisar alterar diretamente o código responsável pelas animações.

## Controles

Durante a sequência de créditos, permitir:

- Pausar.
- Continuar.
- Acelerar a rolagem.
- Pular os créditos.
- Voltar ao menu.

Os controles devem ser discretos e não atrapalhar a apresentação.

Se os créditos forem exibidos depois do final da campanha, assegure que o estado de conclusão esteja salvo antes de permitir ignorá-los.

Utilize tempo não escalado quando necessário para impedir que o sistema de pausa congele indevidamente a animação.

## Áudio

Utilize uma trilha instrumental apropriada, desde que seja original, esteja licenciada ou já esteja autorizada para uso no projeto.

A música deverá acompanhar a sequência inteira, com transições de volume suaves.

Evite reiniciar a música inesperadamente durante a rolagem.

Caso não exista uma trilha adequada, registre a pendência. Não utilize músicas comerciais sem autorização.

---

# ETAPA 10 — TESTES COMPLETOS

Após concluir a implementação, execute verificações técnicas e visuais.

## 10.1. Personagens

Confirmar:

- Os 12 retratos foram corretamente associados.
- Cada personagem apresenta sua própria identidade visual.
- A camiseta escolar está correta.
- Não existem retratos trocados.
- Não há distorções.
- Todos aparecem adequadamente na mochila.

## 10.2. Yasmin

Confirmar:

- Sprites refeitos com fidelidade.
- Cabelo e olhos corretos.
- Animações completas.
- Movimentação funcional.
- Colisores posicionados.
- Piano funcionando.
- Ausência de frames faltantes.

## 10.3. Combate

Testar os 12 alunos.

Confirmar:

- Ataques ativam corretamente.
- Animações são executadas.
- Efeitos aparecem na posição certa.
- Projéteis atingem os inimigos.
- O dano é aplicado corretamente.
- Cooldowns funcionam.
- Inimigos reagem adequadamente.
- IA não fica travada.
- Partículas desaparecem.
- Personagens recuperam o controle.

Priorizar testes específicos dos novos poderes:

**Moscardini:** estado de sono e recuperação da IA.

**Bianca:** lançamento, trajetória e colisão dos livros.

**Giovana:** duração do escudo, bloqueio e reflexão de projéteis.

## 10.4. Créditos

Verificar:

1. Todas as equipes estão presentes.
2. Todos os integrantes foram incluídos corretamente.
3. Os nomes e acentos estão corretos.
4. Os textos sobem suavemente.
5. A velocidade é constante.
6. A música toca corretamente.
7. As transições funcionam.
8. A homenagem ao Edelzio aparece.
9. O logotipo final surge com partículas verdes.
10. A mensagem "OBRIGADO POR JOGAR!" aparece corretamente.
11. É possível pular e retornar ao menu.
12. Os créditos funcionam tanto pelo menu quanto no encerramento da campanha.
13. Nenhum texto fica cortado.
14. Não existem telas pretas permanentes.
15. Não existem erros no Console.

Testar especialmente as resoluções:

- 1280 × 720.
- 1920 × 1080.

---

# ETAPA 11 — BUILD FINAL EXECUTÁVEL

Após os testes, gerar uma nova build de apresentação para Windows.

## Configuração

Utilizar:

- Plataforma: Windows.
- Arquitetura: x86_64.
- Resolução inicial adequada à apresentação.
- Todas as cenas necessárias incluídas.
- Recursos de áudio corretamente incorporados.
- Assets de personagens e animações incluídos.
- Sistema de salvamento funcional.

Gerar a build em:

`Builds/Oizlede-Fragmento-Perdido-Apresentacao/`

Preservar a build estável anterior.

Não sobrescrever o executável aprovado antes de validar a nova versão.

## Verificação da build

Executar o jogo fora do Editor.

Testar:

- Menu principal.
- Carregamento.
- Mochila.
- Retratos.
- Personagens.
- Poderes.
- Combate.
- Sons.
- Créditos.
- Homenagem.
- Salvamento.
- Retorno ao menu.

Não considerar a build aprovada apenas porque o Unity concluiu o processo de compilação.

## Distribuição por pendrive

Organizar a versão final em uma pasta única contendo:

- Executável `.exe`.
- Pasta `_Data`.
- DLLs necessárias.
- Outros arquivos gerados pelo Unity.

Não distribuir somente o `.exe`.

Criar também um arquivo ZIP da pasta completa para backup, caso as ferramentas disponíveis permitam.

Se possível, validar a build em outro computador Windows, sem o Unity instalado.

Se esse teste externo não estiver disponível, informar explicitamente essa limitação.

---

# ETAPA 12 — ORGANIZAÇÃO, COMMITS E ENTREGA

Durante a execução:

- Evite alterações desnecessárias no código.
- Preserve sistemas existentes.
- Não crie assets duplicados sem necessidade.
- Não introduza dependências externas desnecessárias.
- Mantenha o código legível.
- Reutilize os controladores existentes.
- Evite soluções improvisadas.
- Faça commits por etapas validadas.
- Realize push para o repositório correto quando houver acesso.
- Não altere a branch principal sem confirmar o contexto e as regras atuais do projeto.
- Preserve as referências canônicas fornecidas.

Sugestão de divisão dos commits:

1. Auditoria e preparação.
2. Integração dos retratos.
3. Redesign da Yasmin.
4. Novas habilidades de Moscardini, Bianca e Giovana.
5. Atualização das animações dos poderes antigos.
6. Polimento do menu da mochila.
7. Créditos cinematográficos.
8. Homenagem e encerramento.
9. Correções e testes.
10. Build final.

Se o trabalho precisar ser interrompido, deixe um checkpoint claro e publique as alterações já verificadas, conforme as permissões e o fluxo Git do projeto.

---

# ETAPA 13 — RELATÓRIO FINAL OBRIGATÓRIO

Ao terminar, apresente um relatório com:

### Artes e personagens
- Retratos integrados.
- Sprites da Yasmin concluídos.
- Animações dos demais alunos.
- Arquivos utilizados.

### Combate
- Poderes implementados.
- Efeitos criados.
- Problemas corrigidos.
- Testes executados.

### Interface
- Melhorias na mochila.
- Compatibilidade com diferentes resoluções.
- Ajustes visuais realizados.

### Créditos
- Equipes incluídas.
- Animação de rolagem.
- Trilha utilizada.
- Homenagens.
- Supervisores.
- Logo com partículas verdes.
- Mensagem de encerramento.

### Build
- Caminho do executável.
- Resultado dos testes.
- Possíveis problemas.
- Instruções para transferência por pendrive.

### Versionamento
- Commits realizados.
- Branch utilizada.
- Situação do push.
- Pendências restantes.

Use os estados:

**Concluído, Parcial, Bloqueado ou Não iniciado.**

Não declare resultados que não tenham sido efetivamente produzidos e verificados.

---

# ORDEM DE PRIORIDADE DA RETOMADA

Caso haja limitações de tempo, contexto, ferramentas ou capacidade de execução, trabalhe nesta ordem:

**Prioridade 1 — Integração visual**
- Retratos dos 12 alunos.
- Mochila.
- Redesign da Yasmin.

**Prioridade 2 — Combate**
- Novos poderes de Moscardini, Bianca e Giovana.
- Animações dos poderes antigos.
- Sprite sheets.
- Integração e testes.

**Prioridade 3 — Créditos**
- Todas as equipes.
- Rolagem cinematográfica.
- Animações.
- Homenagens.
- Supervisão.
- Logo final com partículas verdes.

**Prioridade 4 — Entrega**
- Testes de regressão.
- Correções.
- Build Windows.
- Backup.
- Commits.
- Push.
- Relatório final.

Não sacrifique a estabilidade da build para declarar uma quantidade maior de tarefas concluídas.

# RESULTADO FINAL ESPERADO

Quero que Oizlede: Fragmento Perdido apresente:

- Os 12 alunos com suas novas artes ampliadas na mochila.
- Os sprites da Yasmin refeitos com fidelidade.
- Todos os poderes originais preservados.
- Três novas habilidades para Moscardini, Bianca e Giovana.
- Animações próprias e efeitos pixelados para os ataques.
- Um sistema de combate estável.
- Um menu da mochila visualmente consistente.
- Créditos cinematográficos completos com todos os participantes.
- Uma homenagem especial ao professor Edelzio.
- Um encerramento emocionante com o logotipo e partículas verdes.
- Uma build Windows jogável, organizada e pronta para apresentação.

**A prioridade é concluir o máximo possível com qualidade, sem comprometer a identidade visual, a história ou a estabilidade do jogo.**

Comece pela auditoria, aproveite as artes enviadas e avance autonomamente pelas etapas, realizando testes, commits e checkpoints verificáveis.

**Não termine apenas com código ou documentação: sempre que o ambiente permitir, entregue também uma build realmente executável e testada.**