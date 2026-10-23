"""Daily project report from dated repository evidence, not file modification times."""
from pathlib import Path
from datetime import datetime, date
from html import escape
import json, hashlib, subprocess
from reportlab.pdfgen import canvas
from reportlab.platypus import (BaseDocTemplate, PageTemplate, Frame, Paragraph, Spacer,
    Table, TableStyle, PageBreak, Image, KeepTogether)
from reportlab.lib import colors
from reportlab.lib.styles import ParagraphStyle
from reportlab.lib.enums import TA_LEFT
from reportlab.lib.pagesizes import A4
from reportlab.lib.utils import ImageReader
from reportlab.pdfbase import pdfmetrics
from reportlab.pdfbase.ttfonts import TTFont

ROOT=Path(__file__).resolve().parents[2]
OUT=ROOT/'output/pdf'
PDF=OUT/'Diario_Desenvolvimento_Varginha_09_a_22_10_2026.pdf'
EVIDENCE=OUT/'diario-09-a-22-10-2026'
REPO='https://github.com/fwbiodev7/segredo-varginha-game-teste-cutscene-gdd-ldd'
CUTOFF='eb2f8eac84c779345e9d9428496aaec066821518'
W,H=A4
M=45
CW=W-2*M
NAVY=colors.HexColor('#13272D')
TEAL=colors.HexColor('#276A63')
GOLD=colors.HexColor('#A77227')
INK=colors.HexColor('#26383E')
MUTED=colors.HexColor('#62777B')
PALE=colors.HexColor('#EDF3F1')
LINE=colors.HexColor('#D7E2DE')

SOURCES=[
 ('S01','Histórico Git do intervalo',None),
 ('S02','Revisão visual/física e reanálise da alpha 0.1','Docs/QAAlpha01/RELATORIO.md'),
 ('S03','Validação e entrega alpha 0.1 em 09/10','Docs/QAAlpha01/VALIDACAO.md'),
 ('S04','Entrega alpha 0.3','Docs/Alpha03/README.md'),
 ('S05','Entrega beta 0.4','Docs/Beta04/README.md'),
 ('S06','Investigação da chave e daily de 20/10','output/key-investigation-2026-10-20/RELATORIO.md'),
 ('S07','Fechamento técnico 0.4.1 de 21/10','output/qa-2026-10-21/RELATORIO.md'),
 ('S08','Roteiro de cutscenes baseado em GDD e LDD','Docs/CampanhaOficial/ROTEIRO_CUTSCENES_GDD_LDD_2026-10-21.md'),
 ('S09','Fusca e novo puzzle, entrega 0.4.2','output/cinematic-opening-2026-10-21/RELATORIO.md'),
 ('S10','Abertura de 1996, manhã e arte, entrega 0.4.3','output/visual-polish-2026-10-21/RELATORIO.md'),
 ('S11','Áudio e apoios, entrega 0.4.4','output/audio-integration-2026-10-21/RELATORIO.md'),
 ('S12','Auditoria dos três apoios','Docs/CampanhaOficial/AUDITORIA_APOIOS_2026-10-21.md'),
 ('S13','Atualização de 22/10 e arquivos/cenas afetados','Docs/CampanhaOficial/ATUALIZACAO_2026-10-22.md'),
 ('S14','Relatório final e evidências de 22/10','output/house-interactions-2026-10-22/RELATORIO.md'),
 ('S15','Resumo técnico final de 22/10','output/house-interactions-2026-10-22/summary.json'),
 ('S16','Roteiros de dublagem: contagens e elenco','output/dublagem-2026-10-22/manifesto.json'),
 ('S17','Pendências ao encerrar 22/10','Docs/CampanhaOficial/PENDENCIAS_2026-10-22.md'),
 ('S18','Arte, fontes, recortes e camadas de 22/10','output/house-interactions-2026-10-22/technical-report.md'),
 ('S19','Empacotamento Windows 0.4.5','output/house-interactions-2026-10-22/package.json'),
]

def link(url,label):return f'<link href="{escape(url,quote=True)}" color="#276A63">{escape(label)}</link>'
def srcrefs(*ids):return 'Fontes: '+', '.join(f'<link href="#src-{i}" color="#276A63">[{i}]</link>' for i in ids)+'.'

pdfmetrics.registerFont(TTFont('Body','C:/Windows/Fonts/arial.ttf'))
pdfmetrics.registerFont(TTFont('Bold','C:/Windows/Fonts/arialbd.ttf'))
pdfmetrics.registerFont(TTFont('Italic','C:/Windows/Fonts/ariali.ttf'))
pdfmetrics.registerFontFamily('Body',normal='Body',bold='Bold',italic='Italic',boldItalic='Bold')
styles={
 'body':ParagraphStyle('body',fontName='Body',fontSize=10.1,leading=14.5,textColor=INK,spaceAfter=7),
 'small':ParagraphStyle('small',fontName='Body',fontSize=8.4,leading=11.7,textColor=MUTED,spaceAfter=6),
 'tiny':ParagraphStyle('tiny',fontName='Body',fontSize=7.5,leading=10.3,textColor=MUTED,spaceAfter=4),
 'label':ParagraphStyle('label',fontName='Bold',fontSize=8.6,leading=12,textColor=TEAL,spaceAfter=8),
 'title':ParagraphStyle('title',fontName='Bold',fontSize=25,leading=28,textColor=NAVY,spaceAfter=13),
 'h1':ParagraphStyle('h1',fontName='Bold',fontSize=20,leading=24,textColor=NAVY,spaceAfter=10),
 'h2':ParagraphStyle('h2',fontName='Bold',fontSize=11.5,leading=15,textColor=TEAL,spaceBefore=9,spaceAfter=6,keepWithNext=True),
 'cell':ParagraphStyle('cell',fontName='Body',fontSize=8.6,leading=12,textColor=INK),
 'headcell':ParagraphStyle('headcell',fontName='Bold',fontSize=8.6,leading=12,textColor=colors.white),
 'caption':ParagraphStyle('caption',fontName='Italic',fontSize=8.1,leading=11,textColor=MUTED,spaceBefore=5,spaceAfter=8),
}
story=[]
def P(text,kind='body'):return Paragraph(text,styles[kind])
def p(text,kind='body'):story.append(P(text,kind))
def h(text):p(text,'h2')
def bullet(title,text):p('<b>'+title+'</b> '+text)
def ref(*ids):p(srcrefs(*ids),'small')
def section(kicker,title,lead):
 story.append(PageBreak());p(kicker,'label');p(title,'h1');p(lead)
def table(headers,rows,widths):
 data=[[P(escape(str(x)),'headcell') for x in headers]]+[[P(escape(str(x)),'cell') for x in row] for row in rows]
 t=Table(data,colWidths=widths,repeatRows=1,hAlign='LEFT')
 t.setStyle(TableStyle([('BACKGROUND',(0,0),(-1,0),TEAL),('VALIGN',(0,0),(-1,-1),'TOP'),
  ('LEFTPADDING',(0,0),(-1,-1),8),('RIGHTPADDING',(0,0),(-1,-1),8),
  ('TOPPADDING',(0,0),(-1,-1),7),('BOTTOMPADDING',(0,0),(-1,-1),7),
  ('ROWBACKGROUNDS',(0,1),(-1,-1),[PALE,colors.white]),('LINEBELOW',(0,0),(-1,0),.5,TEAL),
  ('LINEBELOW',(0,1),(-1,-1),.35,LINE)]))
 story.append(t);story.append(Spacer(1,9))
def figure(path,caption,width=CW,maxheight=220):
 path=ROOT/path;assert path.exists(),path
 iw,ih=ImageReader(str(path)).getSize();fit=min(width/iw,maxheight/ih)
 block=[Image(str(path),width=iw*fit,height=ih*fit,hAlign='LEFT'),P(caption,'caption')]
 story.append(KeepTogether(block))

def collect_history():
 args=['git','log','--all','--since=2026-10-09 00:00:00 -0300','--until=2026-10-22 23:59:59 -0300','--format=%H|%aI|%cI|%s','--reverse']
 rows=[]
 for line in subprocess.check_output(args,cwd=ROOT).decode('utf-8').splitlines():
  sha,a,c,title=line.split('|',3)
  files=subprocess.check_output(['git','diff-tree','--no-commit-id','--name-only','-r',sha],cwd=ROOT).decode('utf-8').splitlines()
  rows.append(dict(commit=sha,authorDate=a,commitDate=c,title=title,changedFiles=files))
 assert len(rows)==9
 return rows

def build_content(commits):
 # 1. Cover
 p('O SEGREDO DE VARGINHA  /  DIÁRIO DO PROJETO','label')
 story.append(Spacer(1,30))
 p('Relatório diário<br/>de desenvolvimento','title')
 p('09 a 22 de outubro de 2026','h1')
 p('Da revisão dos mapas e da alpha à beta 0.4.5, com a investigação da chave, cutscenes, puzzles, áudio e preparação para dublagem.')
 story.append(Spacer(1,10))
 figure('output/house-interactions-2026-10-22/captures/driving-pose-0.png',
  'Estado de 22/10: Edelzio no Fusca, com painel, coluna, volante e instrumentos em camadas corrigidas.',maxheight=260)
 table(['PERÍODO','HISTÓRICO','ESTADO FINAL'],[['14 dias de calendário','9 commits em 5 datas','0.4.5-beta publicada']], [CW/3]*3)
 p('Fechamento documental: 22/10/2026. Horários dos commits em America/Sao_Paulo (UTC-3). Referência de código: '+link(REPO+'/commit/'+CUTOFF,'eb2f8ea')+'.','small')
 p('Documento baseado em histórico Git, relatórios e evidências de validação. Não atribui horas trabalhadas, aprovação humana ou atividades a dias sem registro datado.','small')

 # 2. Overview and method
 section('LEITURA DO DIÁRIO','Panorama e critérios do registro',
  'O período reúne a consolidação visual e física da campanha, a primeira missão adulta revisada e a produção de uma candidata Windows com documentação de entrega.')
 h('Como as datas foram atribuídas')
 p('Foram consultados os nove commits alcançáveis nas referências locais entre 09/10 e 22/10 e os relatórios associados. Data de commit indica o registro da entrega; não mede sua duração nem prova que todo o trabalho ocorreu naquele dia. Atividades anteriores incorporadas à entrega são tratadas como base herdada.')
 p('Os dias de 11 a 19/10 aparecem individualmente no calendário. Não foi encontrado commit ou relatório específico que permita descrever uma entrega diária nessas datas. Isso não comprova ausência de trabalho local ou fora deste repositório.')
 h('Evolução principal')
 table(['DATA','MARCO','RESULTADO'],[
  ['09/10','Revisão e alpha 0.3','Layouts, colisões, sprites, HUD e atmosfera consolidados.'],
  ['10/10','Beta 0.4','Laboratório moderno, menu cinematográfico e selo de retorno.'],
  ['20/10','Investigação da chave','Preparação, desaparecimento, pistas, dedução e ligação com a escola.'],
  ['21/10','Betas 0.4.1 a 0.4.4','Fechamento técnico, Fusca animado, novo mapa, prólogo, manhã, áudio e apoios.'],
  ['22/10','Beta 0.4.5','Itens reais, interface menor, recortes limpos, cabine corrigida e roteiros de voz.'],
 ],[49,113,CW-162])
 h('O que a validação significa')
 p('Testes EditMode verificam regras e integridade; PlayMode no Editor verifica execução; Player Windows cobre execução fora do Editor. Compilar, iniciar no menu e concluir uma campanha manual são verificações diferentes. As tabelas mantêm essas distinções e não somam retestes como casos únicos.')
 ref('S01','S04','S05','S06','S07','S11','S14')

 # 3. Oct9 first registration
 section('09/10/2026  /  SEXTA-FEIRA  /  PARTE 1','Mapas, profundidade e colisões',
  'O primeiro registro do dia consolidou a revisão visual/física da campanha. O título do commit cita uma ferramenta, mas o conteúdo e os relatórios abrangem layouts, renderização, testes e documentação.')
 bullet('22 layouts revisados.', 'Foram refinados 114 recortes distintos e adicionadas três plantas, passando de 315 para 318 entradas no conjunto documentado.')
 bullet('Fragmentos.', 'As colunas internas passaram a bloquear pelo contato no chão; a proteção do tronco deixou de ser aplicada indiscriminadamente às paredes. A escada separou degraus e longarinas para conservar os vãos, e a planta suspensa deixou de bloquear a circulação.')
 bullet('Objetos apoiados.', 'Plantas sobre mesas receberam objetos próprios e profundidade ligada ao móvel. Cadeiras, mesas, camas, estantes, equipamentos, árvores e eletrodomésticos tiveram silhuetas refinadas sem repintar a imagem original.')
 bullet('Bases de colisão.', 'Foram ajustadas bases que ultrapassavam os recortes: árvore do jardim, estante do casarão, caixas do arquivo e cilindro da ruptura.')
 bullet('Igreja e pixel art.', 'A revisão configurou URP 2D com nove projeções de vitrais em azul, âmbar e violeta, bordas graduais e resposta de cor por pixel. Arena e vistas do Fusca receberam filtro Point e compressão desativada. Capturas de pausa/transição foram adaptadas ao URP.')
 h('Verificação e limite encontrado naquele momento')
 p('A reanálise registrou 131/131 EditMode selecionados e 3/3 testes visuais PlayMode. Uma seleção isolada executou zero testes e não foi aceita como aprovação. Uma regressão histórica da biblioteca ainda exigia diagnóstico de rota/colisão. A alpha 0.1 foi gerada posteriormente no próprio dia: Succeeded, zero erros e dois avisos; o smoke automatizado confirmou apenas a inicialização.')
 p('A migração/revisão URP deste registro não deve ser confundida com o remaster anterior de 02 a 08/10, cujo relatório se refere a uma base BuiltIn. O diário não transfere aqueles resultados antigos para 09/10.','small')
 ref('S02','S03');p('Commit: '+link(REPO+'/commit/'+commits[0]['commit'],'efa844f - 14:19')+'.','small')

 # 4. Oct9 alpha3
 section('09/10/2026  /  SEXTA-FEIRA  /  PARTE 2','Consolidação da alpha 0.3',
  'A entrega noturna reuniu mapas, divisão de cômodos, mobiliário, HUD, atmosfera e correções de personagens em uma build Windows x64. A campanha manteve 14 capítulos/fases na ordem vigente, com suas áreas adicionais.')
 bullet('Casarão e interiores.', 'Revisão dos três mapas do casarão, divisões de cômodos, contatos dos móveis e rotas de circulação. Ferramentas de montagem e conferência ficaram versionadas.')
 bullet('HUD e pistas.', 'Identificação temporária, instruções contextuais adaptadas ao dispositivo e dicas progressivas. O objetivo foi orientar sem trocar as soluções já aprovadas.')
 bullet('Atmosfera.', 'Progressão de iluminação, áudio e efeitos ao longo da campanha; integração das transições e continuidade ambiental da base existente.')
 bullet('Personagens.', 'As 55 folhas preservaram dimensões nativas e filtro Point. A geometria removeu fragmentos de frames vizinhos, com ajustes de recorte e alinhamento sem escalar a arte para esconder problemas.')
 h('Entrega e evidências')
 p('Três testes PlayMode de personagens passaram, verificando 928 quadros e poses. O executável permaneceu ativo no menu por 12 segundos, sem exceção de inicialização, e o ZIP passou na verificação CRC. Isso não representa uma partida completa das 14 fases.')
 p('A build consta como Succeeded. O relatório conta um erro de timeout da chamada de automação, documentado nos diagnósticos, e um aviso do pacote opcional Pipeline. O diário preserva essa ocorrência, em vez de apresentar o relatório como zero erros.')
 h('Arquivos e sistemas representativos')
 p('CharacterFrameGeometry, importador de personagens, CampaignIllustratedMaps, sistemas de atmosfera/HUD, mapas do casarão e ferramentas de preparo/empacotamento. Manifestos, diagnósticos e README da alpha 0.3 acompanharam o commit.')
 ref('S04');p('Commit: '+link(REPO+'/commit/'+commits[1]['commit'],'6edf652 - 22:17')+'.','small')

 # 5. Oct10
 section('10/10/2026  /  SÁBADO','Beta 0.4: laboratório, menu e retorno',
  'A beta 0.4.0 integrou a revisão aprovada do laboratório e a apresentação cinematográfica do menu e do selo final, conservando puzzles, diálogos e progressão.')
 bullet('Laboratório moderno.', 'ET na incubadora, bancada e objetos com recortes, proporções e colisões ajustados. A verificação cobriu física, puzzle e recarregamento.')
 bullet('Menu cinematográfico.', 'Névoa, céu corrigido, Fusca azul, OVNI com transparência e partida rápida. Um hover suave compartilhado foi aplicado aos botões, incluindo interfaces existentes.')
 bullet('Selo de retorno.', '24 quadros de abertura, sustentação a 12 fps e fechamento; centro transparente, runas ciano e detalhes vermelhos. O efeito representa a saída da criatura do nosso mundo, acompanha a travessia e fica atrás dela.')
 bullet('Continuidade.', 'Pausa, movimento reduzido e retomada do save foram preservados. A entrega manteve as 14 fases e áreas adicionais.')
 figure('Docs/Beta04/PortalRetorno.png','Registro de 10/10: selo de retorno integrado à travessia da criatura.',maxheight=195)
 p('Validação registrada: 11 testes PlayMode de menu, três de selo, um de laboratório; quatro EditMode direcionados e uma regressão adicional de calibração/travessia. Build sem erros, com um aviso. Smoke de 20 segundos no menu e CRC do ZIP aprovados; sem certificação de desempenho ou campanha humana completa.','small')
 ref('S05');p('Commit: '+link(REPO+'/commit/'+commits[2]['commit'],'df2b091 - 14:25')+'.','small')

 # 6. No records
 section('11 A 19/10/2026  /  REGISTRO DIÁRIO','Intervalo sem entrega datada identificada',
  'O histórico local consultado não contém commits nessas nove datas. Também não foi localizado um relatório diário que permita atribuir uma implementação específica a cada uma delas.')
 weekdays=['domingo','segunda-feira','terça-feira','quarta-feira','quinta-feira','sexta-feira','sábado','domingo','segunda-feira']
 table(['DATA','DIA','REGISTRO VERIFICÁVEL'],[[f'{d:02d}/10',weekdays[d-11],'Sem commit ou relatório diário específico identificado.'] for d in range(11,20)],[49,100,CW-149])
 h('Como interpretar este intervalo')
 p('As retrospectivas citam a janela 09 a 19/10 como período da sprint, mas uma janela de apresentação não prova uma entrega em cada dia. Por isso, laboratório e beta 0.4 ficam em 10/10, e a integração da chave fica em 20/10, conforme seus registros.')
 p('O estado versionado de referência ao final de 19/10 era df2b091. Pode ter existido trabalho não commitado, discussão ou planejamento fora do repositório; não há base documental para inventar essas atividades, duração ou responsáveis.')
 ref('S01')

 # 7. Oct20 gameplay
 section('20/10/2026  /  TERÇA-FEIRA  /  PARTE 1','A chave desaparecida entra na missão adulta',
  'Foi integrada a sequência de preparação para a escola, colocação e desaparecimento da chave, investigação, caixa de 1996 e conexão com a viagem para a Industrial.')
 bullet('Tutorial contextual.', 'Higiene, café, notebook e mochila orientam o início da fase adulta. As instruções consultam os controles configurados, em vez de depender somente de teclas fixas.')
 bullet('Colocação e desaparecimento.', 'Nesta entrega, a chave ficava no balcão. Houve gesto de colocação, estado visível e desaparecimento fora da observação do jogador, com pistas sonoras posicionais. Em 22/10, o local foi alterado para a mesa do café.')
 bullet('Investigação jogável.', 'Balcão, trinco e papel produzem observações. O jogador precisa relacionar as evidências antes de chegar à caixa; uma hipótese incorreta não apaga o progresso.')
 bullet('Caixa e páginas.', 'A caixa reúne a chave, o caderno e vestígios de 1996. As páginas são reconstruídas pelas ilustrações, muro e galhos; a ordem não aparece entregue nas instruções iniciais.')
 bullet('Objetivos e saves.', 'Dicas progressivas, registros do caderno e objetivos acompanham a investigação. Saves intermediários e progresso antigo foram preservados, com IDs estáveis.')
 figure('output/key-investigation-2026-10-20/counter-gameplay.png','Registro histórico de 20/10: chave ainda no balcão. O posicionamento mostrado foi substituído em 22/10.',maxheight=190)
 ref('S06')

 # 8. Oct20 assets tools tests
 section('20/10/2026  /  TERÇA-FEIRA  /  PARTE 2','Arte, viagem, daily e economia de saída',
  'A missão ganhou oito sprites próprios e uma ligação cinematográfica com a escola. A documentação e a apresentação passaram a acompanhar o que estava implementado e o que ainda precisava de validação.')
 bullet('Oito sprites.', 'Chave em repouso e colocação, papel, caixa fechada/aberta e três páginas. Atlas 512x256, células 128x128, PPU 128, alpha binário e filtro Point.')
 bullet('Viagem do Fusca.', 'Embarque, partida, rádio, painel, pane, silêncio, reinício e chegada. O trecho é cinematográfico e não acrescenta um capítulo obrigatório de direção.')
 bullet('Daily.', 'A versão registrada em 20/10 tinha 16 slides, prints reais, notas e nove transições. O encerramento ficou reservado a dúvidas, opiniões e feedbacks reais; os possíveis feedbacks presumidos foram retirados. Versões posteriores simplificaram a apresentação para dez slides.')
 bullet('Logs compactos.', 'Tools/AgentOutput passou a preservar stdout/stderr em disco e entregar resumo orientado à pergunta, falhas e caminhos das evidências. Seus 12 testes passaram. A redução medida é de bytes de saída; não houve modelo local configurado nem garantia de economia de 99% de tokens.')
 h('Validação e pendências daquela entrega')
 table(['VERIFICAÇÃO','RESULTADO'],[['Investigação, saves e rotas','11/11'],['História existente','4/4'],['Sequência e migração','16/16'],['Percurso alternativo no Editor','Concluído pelo CampaignOpeningAudit'],['Arte e apresentação','8 frames; 16 slides conferidos']],[205,CW-205])
 p('Total EditMode: 31/31. O runner PlayMode padrão apresentou erro de infraestrutura em PlayModeRunTask; o percurso passou por uma auditoria alternativa com asserts, não por aquele runner. Não houve nova build Player nesse dia. Ficaram pendentes a regressão completa, uma candidata Windows, dublagem e playtest externo.')
 ref('S06');p('Commit: '+link(REPO+'/commit/'+commits[3]['commit'],'44a80af - 15:15')+'.','small')

 # 9. Oct21 4.1
 section('21/10/2026  /  QUARTA-FEIRA  /  ETAPA 1','Fechamento técnico e candidata 0.4.1',
  'A primeira entrega do dia fechou as pendências técnicas da missão integrada em 20/10 antes da ampliação cinematográfica posterior.')
 bullet('Dicas e apresentação.', 'A dica de higiene passou a apontar à pia junto ao fogão. As etapas receberam três níveis distintos de dica. Poses de soco reaproveitaram o alinhamento corporal; sombras dos aliados passaram a se ocultar imediatamente ao mudar a apresentação.')
 bullet('Combate e pausa.', 'A morte de Edelzio passou a cancelar ataques pendentes mesmo com tempo pausado; a pausa com o personagem vivo continua preservando a ação.')
 bullet('Falha exclusiva do Player.', 'A criação de PolygonCollider2D disparava leitura automática de textura não legível. O sprite passou a ser desvinculado durante a configuração do polígono autoral e restaurado em seguida, conservando arte e geometria.')
 bullet('Confiabilidade dos testes.', 'Fixtures foram isoladas em cenas próprias e contratos antigos passaram a considerar layouts, portraits e ataques A/B vigentes. Testes de física e limites visuais foram alinhados às condições reais; os resultados intermediários foram preservados.')
 table(['BATERIA','RESULTADO / ALCANCE'],[['EditMode final','348/348, sem skips'],['PlayMode no Editor','270/270 pelo último resultado de cada caso'],['Player Windows','4 casos únicos: percurso/colisões e cobertura das 18 áreas'],['Ferramentas','30 helpers de pixel art, 12 do wrapper e 6 do resumo NUnit aprovados']],[164,CW-164])
 p('Os 270 resultados não são apresentados como uma única execução limpa de toda a suíte. A candidata 0.4.1 foi empacotada com CRC/SHA-256. A inspeção do executável confirmou menu e casa adulta; a automação parou quando detectou interação do usuário. Os saves originais foram restaurados após o fechamento da build.')
 ref('S07');p('Commit: '+link(REPO+'/commit/'+commits[4]['commit'],'117d2eb - 08:24')+'.','small')

 # 10. plan/puzzle/fusca4.2
 section('21/10/2026  /  QUARTA-FEIRA  /  ETAPA 2','Roteiro, Fusca animado e novo Fragmentos',
  'Foi documentado um plano de cutscenes apoiado no GDD e no LDD, e depois produzida a abertura adulta/viagem autorizada. A implementação não transforma todas as cenas futuras do plano em conteúdo entregue.')
 bullet('Continuidade narrativa.', 'O roteiro destacou divergências documentais de horário, ordem de encontros, cronologia e finais. Conservou a campanha vigente e evitou uma segunda descoberta idêntica da caixa. A frase infantil não deveria antecipar a revelação do Fusca.')
 bullet('Animação em camadas.', 'Gesto e trajetória da chave, caixa e embarque; quatro frames originais de Edelzio dirigindo, paisagem em movimento, rádio/painel, pane, ignição quente e reinício espontâneo. O close usa antebraço exposto e três poses; não inventa manga comprida.')
 bullet('Cabine e desembarque.', 'A foto real do Fusca fornecida pelo usuário guiou banco, estrutura azul, painel marrom e rádio. A escala do motorista foi ampliada. A saída do carro usa o mapa real da escola e uma rota compatível com suas colisões até o ponto de gameplay.')
 bullet('Glitches.', 'Focos distribuídos, faixas, fragmentos deslocados e sinais verdes/violetas reforçaram a interferência do ET. Intervalos de leitura e movimento reduzido foram mantidos; não foi criada uma entidade ou explicação nova.')
 bullet('Puzzle de Fragmentos substituído.', 'Ordenar marcos/girar setas deu lugar a três bifurcações com pistas de terreno e ponte, prévia, traçar, desfazer/recomeçar, primeira contradição explicada e três dicas. Progresso parcial e prefixo correto são salvos; há uma solução entre oito rotas. A dificuldade mediana ainda depende de playtest humano.')
 p('Validação da 0.4.2: 367/367 EditMode e 5/5 casos direcionados no Player Windows; build sem erros, um aviso, ZIP íntegro. A daily foi simplificada para dez slides. As páginas da caixa na casa e o puzzle de Fragmentos na biblioteca são sistemas distintos.','small')
 ref('S08','S09');p('Commit: '+link(REPO+'/commit/'+commits[5]['commit'],'6dfd878 - 13:14')+'.','small')

 # 11. visual 4.3
 section('21/10/2026  /  QUARTA-FEIRA  /  ETAPA 3','Prólogo de 1996, manhã às 7h e mapa ilustrado',
  'A beta 0.4.3 poliu a abertura, o horário da fase adulta, a identidade da cabine e a arte do mapa de Fragmentos, preservando a progressão existente.')
 bullet('Jornal e TV.', 'Montagem de cerca de 16 segundos com jornal/noticiário, transmissão recuando para a TV e Edelzio criança assistindo. Oito poses a 125 ms formam o ciclo de atenção/respiração; apresentador e boca existentes foram preservados.')
 bullet('Gatilhos da infância.', 'Não há frase do rádio, criatura completa ou clarão antecipado na montagem. O jogador ganha controle antes da perda de sinal e dos eventos de exploração. Abertura já vista pode ser pulada; pausa e retomada foram verificadas.')
 bullet('Manhã adulta.', 'As janelas da casa e o cenário visto pelo Fusca passaram à manhã. A referência de cidade enviada foi interpretada em pixel art. HUD às 07:00, ambiente mais neutro e sombras aclaradas; a infância permanece noturna.')
 bullet('Cabine cartunesca.', 'Banco/porta e painel ganharam acabamento fosco compatível com o jogo. Foram preservados os quatro frames do motorista e a identidade do personagem.')
 bullet('Mapa em pixel art.', 'O esquema geométrico de Fragmentos foi substituído por terreno, árvore, tronco, ponte, muro, portão e capela. A mecânica de três bifurcações permaneceu; a única ponte utilizável ficou visível.')
 figure('output/visual-polish-2026-10-21/captures/trail-puzzle.png','Registro de 21/10: o novo puzzle usa um mapa ilustrado em pixel art, com pistas do terreno.',maxheight=185)
 p('378/378 EditMode e 8/8 casos direcionados no Player Windows. Prévias observadas: oito poses da criança, 72 quadros amostrados do prólogo e 114 da viagem. Build sem erros, um aviso; pacote local íntegro. Essas prévias não medem FPS.','small')
 ref('S10');p('Commit: '+link(REPO+'/commit/'+commits[6]['commit'],'572a296 - 14:31')+'.','small')

 # 12. audio
 section('21/10/2026  /  QUARTA-FEIRA  /  ETAPA 4','Áudio recebido e novas artes dos apoios',
  'A beta 0.4.4 integrou os sons recebidos, após a atribuição corrigida pelo usuário, e revisou a comunicação visual e temporal dos três apoios finais.')
 bullet('Inventário de sons.', '21 arquivos recebidos, 20 fontes únicas, 25 WAV preparados e quatro aliases. Foram integrados passos, ambientes, combate e eventos cinematográficos; todas as fontes receberam uso documentado e créditos.')
 table(['SOM RECEBIDO','USO DEFINIDO'],[['Glitch variation','Ataque do Renan'],['Vidro quebrando','Frasco da Ouzana no impacto'],['Explosão','Evento da infância'],['Som angelical, distinto do sino','Escudo do Padre Fábio']],[177,CW-177])
 bullet('Renan cyber.', 'O efeito passou a representar o alcance radial existente, com pacotes de dados e interferência nos inimigos cujo impacto foi aceito. O desenho antigo sugeria um arco que não correspondia ao acerto.')
 bullet('Ouzana.', 'Frasco e nuvem verde preservados, com oito frames novos e área visual mais coerente com o diâmetro real. O vidro soa uma vez no impacto, não a cada tick.')
 bullet('Padre Fábio.', 'Cúpula com contorno dourado e centro transparente. Corrigida a perda de tempo de proteção durante mensagens modais; pausas combinadas não reativam o áudio antes da hora.')
 bullet('Legibilidade e orçamento.', 'Aviso dos apoios afastado da ação. Foram criados 24 desenhos, oito por apoio. Cada invocação usa um objeto de efeito, sem ParticleSystem; o pulso cyber possui teto de 256 quads e até seis alvos. A auditoria mediu 208 quads nesse cenário.')
 p('Dano, recarga e identidade dos personagens foram preservados. A arena de captura usava chefe imobilizado e vida ampliada para comparação visual; não foi um teste humano de balanceamento.','small')
 p('389/389 EditMode, 10/10 casos da rodada ampla e 4/4 no reteste de áudio/escudo. As rodadas se sobrepõem: não são 14 casos únicos. WAV 25/25; build sem erros, um aviso; v0.4.4-beta publicada com ZIP e daily.','small')
 ref('S11','S12');p('Commit: '+link(REPO+'/commit/'+commits[7]['commit'],'786143d - 15:46')+'.','small')

 # 13. Oct22 items
 section('22/10/2026  /  QUINTA-FEIRA  /  PARTE 1','Rotina adulta com objetos que mudam de estado',
  'A revisão corrigiu situações em que a ação era registrada, mas a xícara cheia e o notebook continuavam desenhados no cenário.')
 bullet('Café.', 'Concluir a animação consome o café e retira a xícara cheia. Interromper a ação não consome o item nem conclui a tarefa. O estado permanece após salvar e reabrir.')
 bullet('Notebook.', 'A preparação termina por ação efetiva, recolhe o objeto e retira-o da mesa. Foi corrigida a busca da cadeira: o notebook removível passou a viver na hierarquia do mapa, enquanto a cadeira autoral continuava na casa original.')
 bullet('Higiene e escala.', 'Lavar o rosto confirma a tarefa somente ao terminar. As poses de gameplay foram normalizadas visualmente pela cabeça e apoio dos pés; a raiz do personagem e suas colisões não são redimensionadas durante a ação.')
 bullet('Chave mais visível.', 'A chave saiu do balcão e foi para a mesa do café, maior. Colocação, abordagem física, marcador, pista, caderno e instruções foram atualizados. Os IDs antigos foram mantidos para preservar os saves.')
 bullet('Continuidade.', 'O desaparecimento, as três pistas, a dedução, a caixa e as páginas continuam no fluxo aprovado. O texto deixou de depender de um copo que deveria permanecer cheio e intacto.')
 figure('output/house-interactions-2026-10-22/captures/key-coffee-table-larger.png','Estado final de 22/10: chave maior sobre a mesa do café, com objetivo e feedback atualizados.',maxheight=210)
 ref('S13','S14','S15')

 # 14. layers furniture
 section('22/10/2026  /  QUINTA-FEIRA  /  PARTE 2','Correções de recorte, cenário e cabine',
  'O feedback visual expôs sobreposições reais. A revisão passou de cobrir o desenho antigo para separar a base limpa, os objetos novos e as camadas de cinema.')
 bullet('Móveis de costas.', 'Geladeira, fogão, pia/armário, estantes do escritório e cômoda com planta/luminária receberam vistas traseiras. Mesas vazias e itens separados completam a composição; os sete nós preservam os limites de colisão autorais.')
 bullet('Remoção da base antiga.', 'Foram limpas as regiões dos objetos substituídos, incluindo a torneira inteira e os pés que excediam os crops. Uma máscara limita a alteração; os pixels fora dela são conferidos como idênticos à fonte.')
 bullet('Transparência real.', 'Café, notebook e luminária usam alpha próprio. A xícara deixou de carregar madeira da mesa junto ao contorno. Os novos atlas e a coluna somam 12 frames/objetos estáticos, com filtro Point e normalização nearest-neighbor.')
 bullet('Quintal.', 'Grama e plantas foram aproximadas dos verdes oliva das árvores, conservando textura, caminhos e física. Exclusões retangulares do primeiro tratamento foram retiradas para evitar manchas.')
 bullet('Volante e painel.', 'Os quatro frames originais do motorista foram registrados pelo cubo do volante. A coluna passou a ser pequena, fosca e simples após o primeiro suporte ter sido rejeitado por destoar do estilo. Ponteiro e marcações são desenhados atrás do volante.')
 bullet('Chegada à escola.', 'A imagem da escola não cobria as bordas e revelava a casa por trás. Um fundo opaco passou a cobrir toda a área cinematográfica; o teste verifica as margens e a devolução do controle.')
 figure('output/house-interactions-2026-10-22/wheel-four-frames.png','Conferência final dos quatro frames: instrumentos atrás do volante e ligação discreta ao painel.',maxheight=185)
 ref('S14','S18')

 # 15. ui scripts release
 section('22/10/2026  /  QUINTA-FEIRA  /  PARTE 3','Interface, dublagem e publicação 0.4.5',
  'A etapa final reduziu a ocupação da tela e preparou a produção das vozes sem declarar gravações ainda não recebidas como integradas.')
 bullet('Coleta e ações.', 'Avisos menores com o fundo pixel art anterior, fila de mensagens e desaparecimento automático. Não exigem Continuar nem abrem uma janela modal para a simples confirmação do item.')
 bullet('Falas e leituras.', 'Legendas compactas na parte inferior, com nome e revelação progressiva. Confirmar revela primeiro e avança depois. Documentos, pistas e escolhas mantêm painéis adequados à leitura. Teclado, controle, pausa e tamanhos configuráveis foram conferidos.')
 bullet('Canal de voz.', 'IDs estáveis por personagem/texto, volume independente, pausa e sincronização preparada. Nenhuma dublagem sintética foi adicionada; o teste usa clip silencioso para validar o canal.')
 table(['MATERIAL','CONTEÚDO'],[['Roteiro completo','153 entradas, 16 papéis; PDF de 18 páginas e Word'],['Edelzio isolado','78 entradas de adulto e criança; PDF de 10 páginas e Word'],['Tipos de entrada','47 existentes (E), 98 propostas (P), 8 esforços vocais (V)'],['Separação do elenco','ZIP por personagem e CSV; sem professores ausentes ou reservas futuras']],[156,CW-156])
 p('As 98 propostas não foram inseridas silenciosamente no jogo: precisam de revisão antes da gravação/integração. PDFs e Word foram renderizados e conferidos visualmente; não são apenas arquivos gerados sem inspeção.')
 bullet('Daily.', 'Dez slides simples, prints atuais, nove transições, notas e encerramento exclusivo de dúvidas, opiniões e feedbacks. Verificação sem overflow e com movimento reduzido.')
 bullet('Publicação.', 'Commit eb2f8ea enviado ao GitHub. A v0.4.5-beta inclui ZIP Windows com 198 arquivos, guia de playtest, créditos e manifesto. Integridade CRC e SHA-256 dos oito anexos da Release foram conferidos. As versões anteriores foram preservadas.')
 p('Build: Succeeded, zero erros e um aviso preexistente do pacote opcional Unity.Pipeline. O URP 2D continua ativo. Três saves, preferências e configurações originais foram restaurados após os testes.','small')
 ref('S13','S14','S16','S19');p('Commit: '+link(REPO+'/commit/'+commits[8]['commit'],'eb2f8ea - 09:15')+'.','small')

 # 16. tests consolidated
 section('CONSOLIDAÇÃO TÉCNICA','Resultados de validação por entrega',
  'As contagens abaixo são retratos de versões e escopos diferentes. Não devem ser somadas como uma bateria única nem usadas como medida de ausência de bugs.')
 table(['DATA / ENTREGA','EDITMODE','EXECUÇÃO / LIMITE'],[
  ['09/10 - revisão/alpha 0.1','131/131 selecionados','3/3 visuais PlayMode; inicialização da build. Regressão histórica da biblioteca ainda em análise.'],
  ['09/10 - alpha 0.3','Não há suíte completa atribuída aqui','3/3 de personagens, 928 quadros/poses; menu por 12 s.'],
  ['10/10 - beta 0.4','4 direcionados + 1 regressão','15 PlayMode (11 menu, 3 selo, 1 laboratório); menu por 20 s.'],
  ['20/10 - investigação','31/31','Auditoria alternativa do percurso no Editor; runner padrão com falha de infraestrutura.'],
  ['21/10 - 0.4.1','348/348','270/270 PlayMode pelo último resultado por caso; 4 casos únicos nativos.'],
  ['21/10 - 0.4.2','367/367','5/5 direcionados no Player Windows.'],
  ['21/10 - 0.4.3','378/378','8/8 direcionados no Player Windows.'],
  ['21/10 - 0.4.4','389/389','10/10 na rodada ampla; reteste 4/4 de áudio/apoios, com sobreposição.'],
  ['22/10 - 0.4.5','398/398','16/16 casos únicos no Player Windows, tomando a última execução de cada caso.'],
 ],[100,90,CW-190])
 h('Problemas corrigidos durante as rodadas')
 p('Foram preservados registros de falhas iniciais: runner vazio/incompleto, contratos legados, leitura de textura não legível no Player, captura GPU com -nographics, prazos de screenshot, geometria de sprite, referência da cadeira e asserção de escala do teste. Os resultados finais citados correspondem às correções e retestes; nenhuma tentativa incompleta foi contada como aprovação.')
 p('Os avisos locais de GPU/fence e o aviso do pacote opcional Pipeline não foram convertidos em certificação de desempenho. A candidata final foi compilada e o pacote verificado; os testes nativos vieram do Player de testes, sem alegar um playtest humano separado do executável normal.','small')
 ref('S03','S04','S05','S06','S07','S09','S10','S11','S15')

 # 17. systems scope / pending
 section('FECHAMENTO DO PERÍODO','Estado entregue e pendências reais',
  'Em 22/10, o escopo autorizado da revisão estava integrado, testado e publicado na 0.4.5-beta. A campanha preservou sua estrutura; revisões posteriores não foram apresentadas como missões ou finais novos.')
 table(['ÁREA','O QUE FICOU ENTREGUE'],[
  ['Mapas e arte','Layouts/refinamento físico, laboratório, cenário adulto, paleta, móveis e recortes sem duplicação.'],
  ['Primeira missão adulta','Tutorial contextual, preparação, itens com estado real, chave, desaparecimento, pistas, caixa e páginas.'],
  ['Puzzles','Investigação por evidências e Fragmentos com três bifurcações, prévia, desfazer e dicas progressivas.'],
  ['Cutscenes','Prólogo de 1996, abertura adulta, Fusca e chegada à escola, com pausa e retomada.'],
  ['Áudio e combate','Sons recebidos documentados; Renan cyber, Ouzana e escudo transparente, sem mudar dano/recargas.'],
  ['Comunicação','Daily, relatórios, fontes de arte, roteiros de dublagem, guia de playtest e pacotes Windows.'],
 ],[108,CW-108])
 h('Próximas ações ainda dependentes de material ou avaliação')
 bullet('Dublagem humana.', 'Receber, selecionar e editar takes; revisar as 98 falas propostas e integrar os arquivos aprovados pelos IDs. As 47 existentes e oito esforços ficam separados no roteiro.')
 bullet('Playtest externo.', 'Pessoa que não conhece as soluções deve avaliar tutorial, investigação, mapa e ritmo. Registrar dificuldade, dicas e bugs reproduzíveis; nenhum feedback externo foi presumido.')
 bullet('Mixagem e outro computador.', 'Conferir fones/caixas, equilíbrio dos apoios contra chefe móvel e desempenho em outro PC. Os limites técnicos de VFX não demonstram FPS mínimo.')
 bullet('Produção futura.', 'As cutscenes e variantes de finais além do recorte implementado continuam no plano GDD/LDD. As divergências editoriais documentadas precisam ser fechadas antes de ampliar esse conteúdo.')
 p('Download da candidata e anexos: '+link(REPO+'/releases/tag/v0.4.5-beta','Release v0.4.5-beta')+'.')
 ref('S08','S13','S17','S19')

 # 18. commit ledger
 section('RASTREABILIDADE','Índice dos nove commits do intervalo',
  'As datas e horários abaixo são os registros Git em UTC-3. O título é preservado; o diário descreve o escopo a partir dos arquivos e relatórios, não apenas do título.')
 rows=[]
 for c in commits:
  d=datetime.fromisoformat(c['commitDate']);rows.append([d.strftime('%d/%m %H:%M'),c['commit'][:7],c['title']])
 table(['REGISTRO','COMMIT','TÍTULO'],rows,[81,61,CW-142])
 p('Cada hash abaixo abre o commit correspondente: '+', '.join(link(REPO+'/commit/'+c['commit'],c['commit'][:7]) for c in commits)+'.','small')
 h('Escopo preservado')
 p('Os relatórios do período distinguem os IDs históricos de fases da ordem jogável de 14 capítulos e suas áreas adicionais. As revisões de 20 a 22/10 afetaram sobretudo a infância, casa adulta, viagem/escola, Fragmentos e sistemas compartilhados de áudio/apoios. Arquivos .unity de produção não precisaram ser regravados nessas últimas etapas.')
 p('Fontes de arte e personagens aprovados foram preservados. Alterações preexistentes do usuário em URP e cenas de recuperação ficaram fora dos commits recentes. A geração deste diário acrescenta apenas documentação e não modifica o jogo.')
 ref('S01','S06','S07','S13')

 # 19. source register, allow automatic continuation
 section('FONTES E EVIDÊNCIAS','Documentos consultados',
  'Os identificadores entre colchetes ao longo do diário remetem aos registros abaixo. Os links apontam para a versão do repositório no fechamento eb2f8ea, evitando confundir um documento histórico com uma edição futura.')
 for i,(sid,title,path) in enumerate(SOURCES):
  if i==10:
   section('FONTES E EVIDÊNCIAS  /  CONTINUAÇÃO','Documentos consultados',
    'Registros de áudio, apoios, rotina adulta, dublagem, arte e empacotamento final.')
  if path:
   file=ROOT/path;assert file.exists(),path
   titlehtml=link(REPO+'/blob/'+CUTOFF+'/'+path,title)
   story.append(KeepTogether([P(f'<a name="src-{sid}"/><b>[{sid}]</b> '+titlehtml),P(escape(path),'tiny')]))
  else:
   p(f'<a name="src-{sid}"/><b>[{sid}]</b> Histórico Git: nove registros entre 09 e 22/10/2026. Datas completas, hashes e arquivos por commit no manifesto documental que acompanha a fonte deste PDF.','body')
 p('Complementos verificados: Docs/QAAlpha01/REANALISE_2026-10-09.md; diagnósticos da alpha 0.3; resultados JSON do menu, selo, laboratório e hover da beta 0.4; relatórios XML/JSON e QA dos slides citados nos relatórios de cada entrega.','small')
 p('Nota sobre cronologia: o relatório de sprint apresentado em 09/10 cobre 02 a 08/10. Ele foi usado somente como contexto da base anterior, sem reclassificar suas atividades como trabalho diário deste intervalo.','small')

class DiaryDoc(BaseDocTemplate):
 def afterFlowable(self,f):
  if isinstance(f,Paragraph) and f.style.name=='h1':
   title=f.getPlainText();key='page-'+str(self.page)+'-'+str(len(self._outline))
   self.canv.bookmarkPage(key);self.canv.addOutlineEntry(title,key,0,False);self._outline.append((title,self.page))
 def beforeDocument(self):self._outline=[]

def page_design(c,doc):
 c.saveState();c.setFillColor(NAVY);c.rect(0,H-16,W,16,fill=1,stroke=0)
 c.setStrokeColor(LINE);c.setLineWidth(.6);c.line(M,H-43,W-M,H-43);c.line(M,39,W-M,39)
 c.setFont('Bold',8);c.setFillColor(TEAL);c.drawString(M,H-34,'O SEGREDO DE VARGINHA')
 c.setFont('Body',8);c.setFillColor(MUTED);c.drawRightString(W-M,H-34,'DIÁRIO  |  09-22 OUT 2026')
 c.setFont('Body',8);c.drawString(M,25,'Desenvolvimento, validação e entregas registradas')
 c.setFont('Bold',8);c.drawRightString(W-M,25,f'{doc.page:02d}')
 c.restoreState()

def main():
 OUT.mkdir(exist_ok=True,parents=True);EVIDENCE.mkdir(exist_ok=True,parents=True)
 commits=collect_history();build_content(commits)
 doc=DiaryDoc(str(PDF),pagesize=A4,leftMargin=M,rightMargin=M,topMargin=60,bottomMargin=53,
   title='Diário de desenvolvimento - O Segredo de Varginha - 09 a 22/10/2026',author='Equipe do projeto O Segredo de Varginha',
   subject='Histórico diário com commits, implementações, testes e pendências',allowSplitting=1)
 doc.addPageTemplates(PageTemplate(id='normal',frames=[Frame(M,53,CW,H-113,leftPadding=0,rightPadding=0,topPadding=0,bottomPadding=0)],onPage=page_design))
 doc.build(story)
 manifest={'period':['2026-10-09','2026-10-22'],'timezone':'America/Sao_Paulo','cutoffCommit':CUTOFF,'commits':commits,
  'sources':[{'id':sid,'title':title,'path':path,'sha256':hashlib.sha256((ROOT/path).read_bytes()).hexdigest() if path else None} for sid,title,path in SOURCES],
  'sections':doc._outline,'pdf':str(PDF.relative_to(ROOT)),'pdfSha256':hashlib.sha256(PDF.read_bytes()).hexdigest()}
 (EVIDENCE/'sources-and-history.json').write_text(json.dumps(manifest,ensure_ascii=False,indent=2)+'\n',encoding='utf-8')
 print(json.dumps({'pdf':str(PDF),'pages':doc.page,'sections':len(doc._outline),'commits':len(commits)},ensure_ascii=False))

if __name__=='__main__':main()
