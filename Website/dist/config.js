// Atualize apenas com materiais públicos e links oficiais confirmados.
window.GAME_SITE = {
  name: 'O Segredo de Varginha',
  version: 'Campanha de 14 fases',
  stage: 'Em desenvolvimento',
  updated: '07.10.2026',
  available: '14 fases em 5 atos · do início ao desfecho',
  planned: 'Investigação · puzzles · aliados · combate',
  siteUrl: 'https://o-segredo-de-varginha.zfabiobrronaldo.chatgpt.site',
  download: null,
  stores: { steam: null, itch: null },
  socials: [],
  credits: [],
  // Formato: { title, description, url, poster, type: 'youtube' | 'file' }.
  // Nenhum player é exibido antes de existir um vídeo real.
  videos: [],
  screenshots: [
    { id: 'rua', src: 'assets/hero.webp', title: 'Depois que a cidade dorme', place: 'Ruas de Varginha', category: 'GAMEPLAY', alt: 'Rua vista de cima, casas brasileiras com telhados vermelhos e postes de iluminação âmbar.' },
    { id: 'casa', src: 'assets/casa.webp', title: 'O passado cabe numa caixa', place: 'A casa de Edelzio · 2026', category: 'GAMEPLAY', alt: 'Casa de Edelzio com quarto, sala, cozinha, livros e um Fusca azul no quintal.' },
    { id: 'escola', src: 'assets/escola.webp', title: 'Entre aulas e pistas', place: 'Escola Industrial', category: 'GAMEPLAY', alt: 'Escola Industrial em pixel art, com computadores, alunos, sala de aula e fachada brasileira.' },
    { id: 'floresta', src: 'assets/floresta.webp', title: 'Além do último poste', place: 'Clareira e ruínas', category: 'GAMEPLAY', alt: 'Clareira escura, trilha entre árvores, rio, ponte de madeira e um santuário em ruínas.' },
    { id: 'capela', src: 'assets/capela.webp', title: 'Luz entre os vitrais', place: 'A capela', category: 'GAMEPLAY', alt: 'Edelzio e Padre Fábio entre bancos de madeira, piso de pedra e luz colorida da capela.' },
    { id: 'laboratorio', src: 'assets/laboratorio.webp', title: 'Algo não se comporta como deveria', place: 'O laboratório de Ouzana', category: 'GAMEPLAY', alt: 'Casa-laboratório botânica com plantas, microscópios, bancadas e Ouzana de jaleco.' },
    { id: 'oficina', src: 'assets/oficina.webp', title: 'Um Fusca. Muitas perguntas.', place: 'A chegada à oficina', category: 'GAMEPLAY', alt: 'Fusca azul na rua em frente à entrada iluminada de uma oficina.' },
    { id: 'infancia', src: 'assets/infancia.webp', title: 'Antes de tudo mudar', place: 'A casa · 1996', category: 'GAMEPLAY', alt: 'Casa de infância em pixel art com TV antiga, quarto e um quintal escuro.' },
    { id: 'transmissao', src: 'assets/transmissao.webp', title: 'Uma transmissão atravessa a noite', place: 'Material da abertura', category: 'CUTSCENE', alt: 'Arte da cutscene de abertura: apresentador de telejornal diante de uma câmera, em tons azuis.' }
  ],
  historySources: [
    {
      date: '20 JAN 1996', kind: 'RELATOS DE TESTEMUNHAS',
      title: 'Caso Varginha', institution: 'Memória Globo · acervo do Fantástico',
      publication: 'Reportagem exibida em 04.02.1996',
      summary: 'Três jovens disseram ter visto um ser estranho em Varginha em 20 de janeiro. A cobertura do Fantástico registrou seus depoimentos e a repercussão local. São relatos de testemunhas; a matéria não comprova a presença de um extraterrestre.',
      url: 'https://memoriaglobo.globo.com/jornalismo/jornalismo-e-telejornais/fantastico/reportagens/noticia/caso-varginha.ghtml'
    },
    {
      date: '1997', kind: 'DOCUMENTO PÚBLICO',
      title: 'Inquérito Policial Militar n.18/1997', institution: 'Superior Tribunal Militar · ARQUIMEDES',
      publication: 'Produção catalogada: 13.02 a 19.06.1997',
      summary: 'O catálogo do STM reúne os dois volumes do inquérito sobre alegações de participação de militares na suposta captura e transporte. Os autos estão disponíveis para consulta. Um documento oficial registra uma investigação e sua versão dos acontecimentos, não confirma os relatos de aparição.',
      url: 'https://arquimedes.stm.jus.br/index.php/inquerito-policial-militar-n-18-1997'
    },
    {
      date: '08 JAN 2026', kind: 'RETROSPECTIVA JORNALÍSTICA',
      title: 'Data Venia: Inquérito sobre ET de Varginha está disponível para consulta pública no STM',
      institution: 'Correio Braziliense', publication: 'Publicação: 08.01.2026',
      summary: 'A reportagem retoma a consulta pública aos autos e informa que a investigação militar rejeitou a narrativa de captura. É uma retrospectiva publicada trinta anos depois, distinta dos depoimentos de 1996 e dos documentos produzidos em 1997.',
      url: 'https://www.correiobraziliense.com.br/direitoejustica/2026/01/amp/7328371-data-venia-inquerito-sobre-et-de-varginha-esta-disponivel-para-consulta-publica-no-stm.html'
    }
  ],
  updates: [
    { date: '07 OUT 2026', title: 'Seu controle, seus comandos', text: 'Configurações com desenho do controle em pixel art e remapeamento de botões. Pausa, mochila e caderno mostram os atalhos atuais. O Fusca original substitui o carro do cenário, com faróis e novas vistas direcionais.' },
    { date: '07 OUT 2026', title: 'Controle e passagens revisados', text: 'Menus, investigação e combate recebem suporte a controles XInput, com prompts automáticos e vibração opcional. Portas, alunos sentados, a interação da pia e a transição para 2026 foram revisados. O Fusca usa W no teclado e A no controle, preservando sua arte.' },
    { date: '07 OUT 2026', title: 'Personagens na luz dos cenários', text: 'A arte dos personagens foi preservada. Luzes do mapa, lanterna e faróis agora contribuem para suas cores, volume e direção das sombras, acompanhando o movimento durante o jogo.' },
    { date: '07 OUT 2026', title: 'Quatorze fases. Uma investigação mais direta.', text: 'A campanha agora percorre cinco atos, com fotografia e código na mesma visita à Industrial, investigação conectada no casarão e menos tarefas repetidas. Estacionamento, aliados e combate final continuam na história.' },
    { date: '07 OUT 2026', title: 'Ajuda no seu ritmo', text: 'Dicas opcionais avançam da orientação à solução, só quando você pedir. O caderno reúne as evidências, os close-ups de investigação aguardam sua leitura e a dificuldade escolhida passa a valer também no combate final.' },
    { date: '04 OUT 2026', title: 'A investigação chega à oficina', text: 'O laboratório de Ouzana e a sequência da oficina receberam revisão de progressão. A chegada agora permite conduzir e estacionar o Fusca antes de examiná-lo.' },
    { date: '03 OUT 2026', title: 'Mais presença, menos interrupções', text: 'Interface com fonte e ícones em pixel art, pausa com fundo pixelizado, transições visuais e ajustes na escola. Personagens, poses e iluminação receberam melhorias.' }
  ],
  archives: [
    { id: 'ARQ-001', title: 'TRANSMISSÃO', type: 'broadcast', description: 'O sinal é fraco. A lembrança, também.', text: 'Uma reportagem termina. A televisão permanece acesa por alguns segundos. Do outro lado da janela, a noite parece ter mudado.', image: 'assets/transmissao.webp', alt: 'Telejornal fictício da abertura do jogo.' },
    { id: 'ARQ-002', title: 'FOTOGRAFIA', type: 'compare', description: 'Duas casas. Trinta anos de distância.', text: 'A casa da infância e a casa de 2026. Compare os ambientes; esta amostra promocional não contém a solução de um puzzle.', image: 'assets/infancia.webp', second: 'assets/casa.webp', alt: 'A casa de infância em 1996 e a casa adulta em 2026.' },
    { id: 'ARQ-003', title: 'FRAGMENTO DE MAPA', type: 'zoom', description: 'As bordas não contam a história inteira.', text: 'Um trecho da trilha. Amplie e examine os detalhes. Os demais fragmentos pertencem à investigação dentro do jogo.', image: 'assets/floresta.webp', alt: 'Trecho da floresta e da ponte, apresentado como fragmento promocional de mapa.' },
    { id: 'ARQ-004', title: 'ANOTAÇÃO INCOMPLETA', type: 'note', description: 'Há uma frase no verso.', text: 'Papel dobrado. Data incompleta. A última linha foi escrita com mais força que as outras.', image: 'assets/fragmentos.webp', alt: 'Material da abertura com papéis, fotografias e fragmentos sobre uma mesa.' },
    { id: 'ARQ-███', title: 'REGISTRO INDISPONÍVEL', type: 'locked', description: 'A cópia termina aqui.', text: 'Não há mais páginas nesta amostra. Algumas perguntas ficam para o jogo.' }
  ]
};
