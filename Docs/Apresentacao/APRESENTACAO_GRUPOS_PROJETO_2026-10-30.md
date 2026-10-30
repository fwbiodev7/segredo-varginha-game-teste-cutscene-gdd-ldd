# O Segredo de Varginha — Apresentação dos Grupos do Projeto
**Data da Apresentação:** 30 de Outubro de 2026  
**Projeto:** Oizlede: O Segredo de Varginha (Jogo 2D de Terror, Aventura Narrativa e Investigação)  
**Motor:** Unity 6 / C# • **Plataforma:** PC Windows  

---

## Estrutura Geral da Apresentação

| Bloco | Grupo / Área | Integrantes | Foco Principal |
| :--- | :--- | :--- | :--- |
| **01** | **Arte e Design Geral** *(Destaque Expandido)* | **Yasmin, Giovana e Luís Martins** | Criação de pixel art e sprites no Pixilart (Pixil), animações de 4 direções, retratos canônicos e cenários. |
| **02** | **Desenvolvimento e Engenharia** | **Fábio, João Pedro Matias, Asafe e Marcos** | Programação na Unity (C#), uso pioneiro de IA (Codex e Antigravity), mecânicas e automação. |
| **03** | **Narrativa e Roteiro** | **Bianca e João Guilherme** | Concepção do GDD e LDD, histórias reais contadas por Edelzio e Professor Fábio, 14 capítulos e lore. |
| **04** | **Sonoplastia e Dublagem** | **Luís Messias e Gustavo** | Efeitos sonoros garimpados no Pixabay, sintetizadores procedurais e narração dublada por Luís Messias. |
| **05** | **Testes e Garantia de Qualidade (QA)** | **Moscardini, Anna Sabiá, Ana Tavares, Karol e Pedro** | Testes sistemáticos desde o primeiro protótipo, auditoria de colisões, balanceamento e feedback contínuo. |

---

## 🎨 1. Arte e Design Geral (Bloco Expandido)
**Integrantes:** Yasmin, Giovana e Luís Martins  
**Tempo Sugerido:** 4 a 5 minutos  
**Ferramentas Centrais:** Pixilart (Pixil), Krita, Aseprite e Unity Sprite Editor  

### A Escolha do Pixilart (Pixil)
> *"Para dar vida ao universo visual de O Segredo de Varginha, nossa equipe optou pelo **Pixilart (Pixil)** — uma ferramenta online simples, leve, acessível e extremamente poderosa para criação de pixel art e spritesheets."*

- **Acessibilidade e Rapidez:** Por rodar diretamente no navegador, permitiu que todos os artistas trabalhassem de forma sincronizada, testando paletas e exportando quadros sem sobrecarga de máquina.
- **Controle de Grid e Camadas:** Organização rigorosa em grades de 16×16, 32×32 e células finais de 64×64 pixels, fundamental para encaixar com precisão no sistema de física e iluminação da Unity.
- **Preservação da Estética Retrô:** O Pixil garantiu pixels nítidos (Point Filter, sem interpolação borrada), mantendo a fidelidade visual dos clássicos do suspense dos anos 90.

### O Desenvolvimento dos Sprites e Ciclos de Caminhada
- **Animações em 4 Direções:** Cada um dos personagens principais e alunos possui ciclos completos de movimento:
  - **Frente (Sul):** Visão frontal com balanço de ombros e passadas rítmicas.
  - **Costas (Norte):** Visão dorsal com movimentação de mochilas e cabelos.
  - **Direita (Leste) e Esquerda (Oeste):** Perfis desenhados respeitando a silhueta, calçados e adereços característicos.
- **Alinhamento e Baseline:** Padrão rigoroso de contato com o chão (`baseline y = 6`), garantindo que nenhum personagem pareça "flutuar" ou afundar no piso dos cenários.
- **Elenco Completo de 12 Alunos:**
  1. **Yasmin:** Cachos longos pretos, olhos azul-esverdeados, postura elegante e ataque especial com piano descendente.
  2. **Giovana:** Uniforme com detalhes dinâmicos, ginga inspirada na capoeira e projeção de barreira protetora dourada.
  3. **Bianca:** Jaleco e acessórios de estudante pesquisadora, ataque tático de arremesso de livros e poções químicas.
  4. **Moscardini:** Porte atlético, postura de luta corporal (wrestling) e poder tático de adormecer oponentes.
  5. **Pedro, Matias, Fábio, Marcos, Anna Sabiá, Ana Tavares e Luís Miguel Messias:** Cada um com suas vestimentas de época, instrumentos (guitarra, vôlei, raquete de ping-pong, microfone e artes).

### Retratos Canônicos para a Mochila e Diálogos
- Criação de 12 retratos individuais em alta fidelidade pixel art (resolução 256×256 pixels, 128 PPU).
- Cada retrato captura a expressão facial, traços de personalidade e identidade dos alunos da vida real homenageados no game.
- Grade de 3 colunas por 4 linhas na mochila, permitindo inspeção individual de perfil, histórico e habilidades antes do confronto final.

### Cenários, Iluminação e Atmosfera
- **Recriação Histórica:** Sala de aula da Escola Industrial, laboratório secreto, casarão antigo de 1898, igreja com sinos e o emblemático Fusca azul.
- **Interiores e Objetos Vivos:** O café quente na mesa, a pia com marcas de uso, o notebook conectado e pastas de documentos que convidam o jogador à investigação.

---

## 💻 2. Desenvolvimento e Engenharia de Software
**Integrantes:** Fábio, João Pedro Matias, Asafe e Marcos  
**Tempo Sugerido:** 3 minutos  
**Ferramentas Centrais:** Unity 6, C#, Antigravity (Google DeepMind) e OpenAI Codex  

### Arquitetura na Unity 6
- **Top-Down Controller Preciso:** Movimentação fluida em 8 direções para Edelzio, com tratamento de aceleração, desaceleração, corrida, agachamento e interações contextuais (tecla `E`).
- **Sistema de Mochila e Inventário:** Gerenciamento dinâmico de slots, abas de Alunos, Apoio (Renan, Padre Fábio, Ouzana) e Pistas Investigativas, com transições em tempo real.
- **Compatibilidade de Saves:** Arquitetura robusta de salvamento contínuo em JSON, preservando o progresso do jogador mesmo com adições de novas fases e personagens extras.

### Uso Pioneiro de Inteligências Artificiais (Codex & Antigravity)
- **Pair Programming com Antigravity:** O assistente autônomo da Google DeepMind atuou lado a lado com os desenvolvedores, auxiliando na arquitetura de scripts, resolução de concorrência em corrotinas, unificação de branches do Git e automação de testes.
- **Aceleração com Codex:** Geração ágil de boilerplate, testes de unidade EditMode e PlayMode, cálculos de colisão por raycast e importadores automatizados de texturas.
- **Automação de Builds:** Scripts de compilação em batchmode (`VarginhaDailyBuild` e `VarginhaAlphaBuild`) que geram o executável standalone para Windows x64 com relatórios detalhados de tamanho e tempo.

---

## 📜 3. Narrativa, GDD e Roteiro
**Integrantes:** Bianca e João Guilherme  
**Tempo Sugerido:** 2 a 3 minutos  
**Documentos Base:** GDD (Game Design Document) e LDD (Level Design Document)  

### Construção do Universo e Inspiração Real
- **Histórias e Causos:** A trama foi concebida a partir de memórias, conversas e relatos compartilhados pelo professor Edelzio e pelo professor Fábio, mesclando fatos do imaginário popular de Varginha em 1996 com uma envolvente ficção de suspense sobrenatural.
- **Dualidade de 1996 e 2026:** O prólogo na infância apresenta o evento traumático que marcou o protagonista; trinta anos depois, o professor revisita suas origens e decifra o mistério que permaneceu adormecido.
- **A Diferença Crucial do Enredo:** O roteiro estabelece com clareza o respeito à criatura indefesa e machucada, diferenciando-a da manifestação agressiva que surge das fendas cósmicas.

### O GDD e a Estrutura dos 14 Capítulos
- O GDD detalha os pilares do jogo: **Observação, Investigação, Tensão e Cooperação**.
- O LDD garantiu que cada fase oferecesse um gancho narrativo próprio: da busca da chave do Fusca na casa, passando pela invasão à escola, a travessia na floresta, até o ritual de retorno no casarão.
- Quebra-cabeças integrados à história (Livro do Tombo, caderno antigo, alinhamento de reguladores de frequência).

---

## 🎧 4. Sonoplastia e Dublagem
**Integrantes:** Luís Messias e Gustavo  
**Tempo Sugerido:** 2 minutos  
**Recursos Utilizados:** Pixabay Audio, Audacity, Microfones de Estúdio e Sintetizadores Unity  

### Criação da Paisagem Sonora
- **Pesquisa e Curadoria no Pixabay:** Coleta e tratamento de efeitos sonoros orgânicos sob licença aberta (passos em piso de madeira, folhas secas estalando na mata, motor de partida e estalos do Fusca, chuva e trovões).
- **Áudio Procedural em Tempo Real:** Desenvolvimento de sintetizadores matemáticos dentro da Unity via `CampaignSoundscape` para os golpes dos aliados (o estrondo metálico dos halteres, o zumbido do escudo arcano, a reverberação do piano).
- **Músicas de Tensão:** Integração das trilhas temáticas de menu, suspense da capela e o tema épico dos créditos finais.

### Narração Dublada por Luís Messias
- Gravação de 15 faixas completas de narração (`Narration01.wav` a `Narration15.wav`).
- A voz de Luís Messias dá profundidade emocional aos pensamentos de Edelzio, orientando o jogador em marcos importantes da campanha.
- Sistema `CampaignNarration` com suporte a pausa no inventário e controle independente de volume nas configurações.

---

## 🔍 5. Testes e Garantia de Qualidade (QA)
**Integrantes:** Moscardini, Anna Sabiá, Ana Tavares, Karol e Pedro  
**Tempo Sugerido:** 2 minutos  
**Metodologia:** Testes de Caixa-Preta, PlayMode automatizado, EditMode e Auditoria de Colisão  

### A Importância dos Testes Desde o Início
- **Detecção Precoce de Falhas:** O grupo acompanhou todas as versões desde setembro, testando cada protótipo antes de se tornar uma fase oficial.
- **Auditoria de Ambientes:** Testagem exaustiva de mais de 18 ambientes e centenas de móveis para evitar que Edelzio ou os inimigos ficassem presos em paredes ou móveis.
- **Feedbacks Fundamentais:**
  - Balanceamento de vida e dano dos inimigos para manter o terror sem se tornar punitivo demais.
  - Ajuste do tempo de corrida e recarga das habilidades dos aliados.
  - Clareza visual dos textos das pistas para garantir leitura confortável em monitores e projetores.
  - Validação da estabilidade do savegame e transições de fase sem telas pretas ou congelamentos.

---

## 💡 Dicas de Apresentação em Conjunto
1. **Dinâmica de Palco:** Quando cada grupo for falar, os integrantes dão um passo à frente ou apontam para a tela no momento em que seu trabalho for destacado.
2. **Uso dos Slides / Página Interativa:** Projete a página de apresentação interativa (`apresentacao_interativa.html`), passando de aba em aba para exibir as artes, capturas de tela e créditos de cada área.
3. **Encerramento Unido:** Ao final, todos os integrantes agradecem juntos, convidando os professores, colegas e jurados para jogarem a build Windows recém-gerada.
