# Plano do redesign escolar: Padrões Ocultos

Estado: plano completo aprovado pelo usuário em 27/10/2026; implementação autorizada, incluindo ajustes e testes necessários. Este é um pedido novo, posterior ao fechamento das pendências de 26/10. A build 0.4.9-beta de 27/10 preserva o fluxo anterior da escola; ainda não contém este redesign.

## Fontes e precedência narrativa

Foram comparados o GDD anexado em Downloads, os dois textos Item 02 fornecidos pelo usuário, o LDD atual, a campanha de 14 capítulos e os scripts de casa/escola/progresso/caderno.

O texto que declara a “correção narrativa fundamental” resolve a divergência entre os anexos: jornais e anotações são recuperados uma única vez na caixa sob a cama. “Descobrir” na escola passa a significar reconhecer relações nos materiais que Edelzio já possui. Não haverá segunda coleta dos mesmos documentos.

O GDD anexado registra o protótipo de setembro, com referências a 15 fases, fotografia/23:17, resgate e três finais previstos. A campanha vigente tem 14 capítulos, análise escolar com 23:23 e biblioteca antes da mata/diocese. O pedido explícito altera somente o fluxo investigativo inicial da escola e sua ligação com a caixa. Não restaura a estrutura antiga nem antecipa o selo, a entidade, 1898 ou as revelações posteriores.

## Diagnóstico do código atual

- `VarginhaCampaignStage.SchoolInteraction` faz Renan entregar uma fotografia ligada a 1996 e exige conversar com ele antes de usar o notebook. A conclusão depende de lhe mostrar o horário. Esse conhecimento e essa função de resposta entram em conflito com o novo pedido.
- A interface mantém tópicos Fotografia, Anotações e Pesquisa, seguidos pela identificação do vulto e do horário. Não oferece as quatro operações investigativas pedidas.
- `CampaignStory` usa flags antigas (`photoOpened`, `buildingSolved`, `codeSolved`, `renanConfirmed`) para orientação, segundo fragmento, inventário e saída. A troca precisa adaptar essas dependências, não apenas os textos.
- `CampaignJournal` conserva resumos da caixa e da fotografia, mas não um catálogo dos jornais completos e das anotações consultáveis.
- A biblioteca já existe como fase interna 6, o capítulo Fragmentos. A escola usa o ID interno 4. Não é necessária uma cena nova para o acervo.

## Implementação proposta

### 1. Recuperação única na casa e catálogo de evidências

Registrar na abertura da caixa a chave, o caderno, os recortes de 1996 e as anotações infantis. Preservar o puzzle das páginas, a investigação da chave e a viagem do Fusca. Os documentos terão IDs estáveis e conteúdo completo acessível pelo caderno mesmo depois da retirada dos itens da hotbar. A casa não explicará as relações que o puzzle escolar deve revelar.

### 2. Escola cotidiana e novos pontos de análise

Renan começa com diálogos escolares normais. Seu notebook não entrega respostas sobrenaturais nem condiciona a investigação à aprovação dele. Reutilizar a mesa, os sprites de notebook e o mural do mapa atual para oferecer mesa de investigação, organização dos documentos e contexto sobre consulta ao acervo. Os papéis pertencem a Edelzio, não são novos colecionáveis espalhados na escola. Preservar fachada, arquitetura, turma, arte e colisores aprovados.

### 3. Puzzle visual de quatro etapas

1. **Cronologia:** reorganizar três recortes embaralhados por datas impressas e legíveis. As datas serão coerentes com 1996 e identificadas como conteúdo ficcional do jogo.
2. **Padrões:** selecionar trechos repetidos nos documentos, com destaque imediato. Haverá informações irrelevantes distinguíveis pelo próprio texto, sem exigir conhecimento externo.
3. **Cruzamento:** ligar anotações infantis aos trechos dos jornais que as sustentam. A interface mostrará as associações e permitirá desfazer ou corrigir cada ligação.
4. **Destino:** relacionar a referência de origem dos recortes ao registro do mural, que identifica explicitamente o acervo da biblioteca. A conclusão será sustentada pelos documentos e pelo contexto examinável, sem senha arbitrária.

O notebook representa ordenar, comparar e filtrar dados sem exigir programação do jogador. Reabertura, textos completos e dicas progressivas dispensam decorar falas. Erros não apagam escolhas corretas, não punem o jogador e indicam o tipo de relação que precisa de revisão. A conclusão atualiza o objetivo para **Investigar o acervo da biblioteca**.

### 4. Progresso e compatibilidade

Adicionar estado próprio para documentos recuperados/examinados, ordem dos recortes, destaques, associações, etapa, dicas e conclusão. Atualizar caderno, objetivos, segundo fragmento e condição da saída da escola. Manter os campos antigos como ponte de compatibilidade para os sistemas posteriores, sem voltar a exigir a confirmação narrativa de Renan.

Migrar saves antigos sem apagar evidências ou repetir capítulos concluídos: quem já passou da escola conserva a conclusão; quem está no meio da investigação recupera seus materiais e pode continuar sem bloqueio. Não completar automaticamente a investigação em partidas novas. Os IDs de cenas e a sequência da campanha continuam estáveis.

### 5. Validação e entrega

Testar coleta única, consulta após retirar itens da hotbar, transporte dos materiais, entrada condicionada no puzzle, ordenação/associações erradas e corrigidas, desfazer, dicas, reabertura, save/reload e chegada à biblioteca. Verificar teclado, mouse e controle. Rodar regressões dos fluxos afetados e capturar cada etapa no Player.

A avaliação com uma pessoa que desconhece a lore será registrada separadamente: os testes automatizados não substituem essa validação. Preparar o roteiro dessa sessão e o percurso sem fornecer respostas antecipadas. Gerar outra candidata após a implementação, preservando a 0.4.9-beta.

## Arquivos e cenas previstos

Principais arquivos: `VarginhaCampaignStage.cs`, `CampaignStory.cs`, `CampaignSequence.cs`, `CampaignHouseSequence.cs`, `CampaignJournal.cs`, `CampaignGuidance.cs`, dicas e testes de casa/escola/transições. Separar catálogo de documentos, regras do puzzle e apresentação escolar em classes próprias para não concentrar toda a mudança no controlador atual.

Cenas afetadas em runtime: casa adulta (ID 2), Industrial (4) e entrada da biblioteca (6). O plano prioriza montagem aditiva em runtime e reutilização das artes existentes. A necessidade de editar um `.unity` será avaliada durante a implementação, sem redesenhar mapas aprovados.

## Critério de conclusão

O fluxo completo deve permitir: casa, caixa sob a cama, materiais de infância, Fusca, escola, análise em quatro etapas e dedução do acervo. Todas as respostas devem estar disponíveis nos documentos examináveis. A entrega precisa incluir código, estado salvo, capturas, testes reais no Unity e build atualizada. Alterar apenas perguntas, respostas ou falas não conclui o redesign.
