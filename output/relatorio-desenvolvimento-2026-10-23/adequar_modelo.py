from pathlib import Path
import re, shutil, json

OLD=Path(__file__).resolve().parent
OUT=OLD.parent/'relatorio-modelo-abtn-2026-10-23'
OUT.mkdir(exist_ok=True)
text=(OLD/'conteudo.md').read_text(encoding='utf-8')
sections={}
for part in re.split(r'(?m)^# ',text):
    if not part.strip():continue
    title,body=part.split('\n',1)
    sections[int(title.split()[0])]=(title,body.strip())

def sub_body(number,sub):
    body=sections[number][1]
    parts=re.split(r'(?m)^## ',body)
    for part in parts[1:]:
        title,content=part.split('\n',1)
        if title.startswith(f'{number}.{sub} '):return content.strip()
    raise ValueError((number,sub))

def nested(number,parent):
    title,body=sections[number]
    result=[]
    for part in re.split(r'(?m)^## ',body):
        if not part.strip():continue
        if re.match(r'^\d+\.\d+ ',part):
            head,content=part.split('\n',1)
            old,subhead=head.split(' ',1)
            result.append(f'### {parent}.{old.split(".")[1]} {subhead}\n\n{content.strip()}')
        else:result.append(part.strip())
    return '\n\n'.join(result)

blocks=['# 1 INTRODUÇÃO\n\n'+sections[1][1],
        '# 2 REFERENCIAL TEÓRICO',
        '## 2.1 Design de jogos e experiência do jogador\n\n'+sub_body(2,1),
        '## 2.2 Versionamento e validação de software\n\n'+sub_body(2,2),
        '# 3 PROCEDIMENTOS METODOLÓGICOS',
        '## 3.1 Pesquisa documental e critérios de análise\n\n'+sub_body(2,3),
        'O recorte encerra-se em 23 de outubro de 2026. As informações anteriores ao primeiro commit são descritas apenas pelas intenções preservadas nos documentos. Custos efetivos, horas individuais, requisitos mínimos medidos e resultados de testes externos não estavam demonstrados e não receberam valores estimados. A apresentação segue a estrutura institucional do arquivo Modelo ABTN.pdf, com fonte Times New Roman, corpo 12, entrelinhas de 1,5 e títulos principais centralizados em tamanho 16. As referências identificam as fontes consultadas (Universidade Federal do Espírito Santo, 2026).']
titles={3:'Concepção narrativa e evolução do escopo',4:'Trajetória do desenvolvimento',5:'Organização da campanha e dos níveis',6:'Arquitetura e tecnologias',7:'Mecânicas e sistemas de jogo',8:'Produção visual cinematográfica e sonora',9:'Interface controles e acessibilidade',10:'Qualidade testes e resultados técnicos',11:'Entregas documentação e gestão'}
for old in range(3,12):
    parent=f'3.{old-1}'
    blocks.append(f'## {parent} {titles[old]}\n\n'+nested(old,parent))
blocks.extend([
    '## 3.11 Resultados obtidos e estágio atual\n\n'+sub_body(12,1),
    '# 4 RESULTADOS ESPERADOS',
    'O resultado esperado é consolidar uma versão estável e compreensível da campanha já implementada, preservando sua identidade visual, a investigação e a continuidade narrativa. Os resultados apresentados na seção anterior sustentam essa expectativa, mas também mostram os limites que precisam orientar a próxima entrega.',
    '## 4.1 Continuidade do projeto\n\n'+sub_body(12,2),
    '## 4.2 Síntese conclusiva\n\n'+sections[13][1]
])
new='\n\n'.join(blocks)+'\n'
new=new.replace('O Quadro 3 separa os resultados existentes das validações ainda pendentes.','O Quadro 3 separa os resultados existentes das validações ainda pendentes.')
(OUT/'conteudo.md').write_text(new,encoding='utf-8')

src=(OLD/'gerar_relatorio.py').read_text(encoding='utf-8')
src=src.replace("NAME = 'relatorio_desenvolvimento_o_segredo_de_varginha_23-10-2026'", "NAME = 'relatorio_desenvolvimento_varginha_modelo_abtn'")
src=src.replace("st.paragraph_format.space_before=Pt(18 if name=='Heading 2' else 0)","st.paragraph_format.space_before=Pt(24 if name=='Heading 1' else 18 if name in ['Heading 2','Heading 3'] else 0)")
src=src.replace("doc.styles['Heading 1'].paragraph_format.page_break_before=True", "doc.styles['Heading 1'].paragraph_format.page_break_before=True\ndoc.styles['Heading 1'].paragraph_format.alignment=WD_ALIGN_PARAGRAPH.CENTER\ndoc.styles['Heading 1'].paragraph_format.space_after=Pt(24)")
cover='''front('ESCOLA ESTADUAL DEPUTADO DOMINGOS DE FIGUEIREDO',before=71,bold=True,size=14)
front('TURMA DO 3º ANO DE DESENVOLVIMENTO DE SISTEMAS',before=144,bold=True,size=14)
p=front('O SEGREDO DE VARGINHA',before=161,bold=True,size=18);p.style='Title';p.alignment=WD_ALIGN_PARAGRAPH.CENTER;p.paragraph_format.line_spacing=1.0
p=front('Relatório do desenvolvimento do projeto',size=14)
for r in p.runs:r.italic=True
front('VARGINHA, 2026',before=228,bold=True,size=14)

face=section(1)
face.bottom_margin=Cm(2)
front('TURMA DO 3º ANO DE DESENVOLVIMENTO DE SISTEMAS',bold=True,size=14)
p=front('O SEGREDO DE VARGINHA',before=175,bold=True,size=18);p.style='Title';p.alignment=WD_ALIGN_PARAGRAPH.CENTER;p.paragraph_format.line_spacing=1.0
p=front('Relatório do desenvolvimento do projeto',size=14)
for r in p.runs:r.italic=True
p=para('Relatório do desenvolvimento do projeto apresentado ao 3º ano do curso técnico em Desenvolvimento de Sistemas da Escola Estadual Deputado Domingos de Figueiredo.','Front')
p.paragraph_format.left_indent=Cm(6.8);p.paragraph_format.space_before=Pt(93);p.alignment=WD_ALIGN_PARAGRAPH.LEFT
p=para('Professores: Renan e Alex.','Front');p.paragraph_format.left_indent=Cm(6.8);p.alignment=WD_ALIGN_PARAGRAPH.LEFT
front('VARGINHA, 2026',before=264,bold=True,size=14)

'''
a=src.index("front('ESCOLA ESTADUAL")
b=src.index('figure_titles=')
src=src[:a]+cover+src[b:]
a=src.index('for title,kind,items in ')
b=src.index("unnumbered('SUMÁRIO')")
lists='''unnumbered('LISTA DE ILUSTRAÇÕES')
for kind,items in [('Figura',figure_titles),('Quadro',quadro_titles)]:
    for n,t in enumerate(items,1):
        p=para(f'{kind} {n} – {t}','ListEntry')
        p.paragraph_format.line_spacing=1.5;p.paragraph_format.space_after=Pt(6)
        p.paragraph_format.tab_stops.add_tab_stop(Cm(15.9),WD_ALIGN_PARAGRAPH.RIGHT,1)
        p.add_run('\\t');field(p,f'PAGEREF {kind}{n} \\\\h','0')
unnumbered('LISTA DE TABELAS')
for n,t in enumerate(table_titles,1):
    p=para(f'Tabela {n} – {t}','ListEntry')
    p.paragraph_format.line_spacing=1.5;p.paragraph_format.space_after=Pt(6)
    p.paragraph_format.tab_stops.add_tab_stop(Cm(15.9),WD_ALIGN_PARAGRAPH.RIGHT,1)
    p.add_run('\\t');field(p,f'PAGEREF Tabela{n} \\\\h','0')

'''
src=src[:a]+lists+src[b:]
src=src.replace('s=section()','s=section()\ns.bottom_margin=Cm(2)')
# Correct the principal title style without imposing the former report structure.
a=src.index("    if chunk.startswith('# '):")
b=src.index("    elif chunk.startswith('@fig|'):",a)
render='''    if chunk.startswith('# '):
        p=para(chunk[2:],'Heading 1')
        p.alignment=WD_ALIGN_PARAGRAPH.CENTER
        if chunk.startswith('# 1 '):p.paragraph_format.page_break_before=False
    elif chunk.startswith('### '):
        p=para(chunk[4:],'Heading 3')
    elif chunk.startswith('## '):
        p=para(chunk[3:],'Heading 2')
'''
src=src[:a]+render+src[b:]
src=src.replace("unnumbered('REFERÊNCIAS',include=True)","unnumbered('5 REFERÊNCIAS BIBLIOGRÁFICAS',include=True)")
src=src.replace('Diretórios discriminados no Apêndice B.','Diretórios: output/key-investigation-2026-10-20; output/qa-2026-10-21; output/cinematic-opening-2026-10-21; output/visual-polish-2026-10-21; output/audio-integration-2026-10-21.')
# The detailed development is retained in section 3. Remove supplementary pages absent from the model.
a=src.index("unnumbered('APÊNDICE A")
b=src.index('# Keep title paragraphs',a)
src=src[:a]+src[b:]
src=src.replace("'primary_sections':13","'primary_sections':5")
src=src.replace("p.paragraph_format.keep_with_next=False", "p.paragraph_format.keep_with_next=(kind=='Quadro')")
# Keep the two compact synthesis matrices with their own headings and context.
src=src.replace("p=para(chunk[3:],'Heading 2')", "p=para(chunk[3:],'Heading 2')\n        if chunk.startswith('## 3.11 '):p.paragraph_format.page_break_before=True")
src=src.replace("p=para(chunk[4:],'Heading 3')", "p=para(chunk[4:],'Heading 3')\n        if chunk.startswith('### 3.10.4 '):p.paragraph_format.page_break_before=True")
(OUT/'gerar_relatorio.py').write_text(src,encoding='utf-8')
for file in ['exportar_word.ps1','renderizar_conferir.py']:
    s=(OLD/file).read_text(encoding='utf-8').replace('relatorio_desenvolvimento_o_segredo_de_varginha_23-10-2026','relatorio_desenvolvimento_varginha_modelo_abtn')
    (OUT/file).write_text(s,encoding='utf-8')
(OUT/'fidelidade-modelo.json').write_text(json.dumps({'reference':'C:/Users/Usuario/Documents/Modelo ABTN.pdf','structure':['Capa','Folha de rosto','Lista de ilustrações','Lista de tabelas','Sumário','1 INTRODUÇÃO','2 REFERENCIAL TEÓRICO','3 PROCEDIMENTOS METODOLÓGICOS','4 RESULTADOS ESPERADOS','5 REFERÊNCIAS BIBLIOGRÁFICAS'],'retained':'Conteúdo de desenvolvimento integral organizado em subseções; dados e evidências mantidos','intentional_adaptations':['Campos genéricos preenchidos com os dados fornecidos','Placeholder de logomarca omitido por ausência de logomarca institucional fornecida','Instruções vermelhas e exemplos do modelo não reproduzidos como conteúdo','Referências reais e atuais preservadas; resultados obtidos distinguidos das expectativas','Sem resumos em idiomas e apêndices adicionais, ausentes no modelo']},ensure_ascii=False,indent=2),encoding='utf-8')
print(OUT)
