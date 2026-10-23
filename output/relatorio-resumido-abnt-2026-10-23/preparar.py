from pathlib import Path

OUT=Path(__file__).resolve().parent
OLD=OUT.parent/'relatorio-modelo-abtn-2026-10-23'
original=(OLD/'gerar_relatorio.py').read_text(encoding='utf-8')
prefix=original.split('figure_titles=')[0].replace("NAME = 'relatorio_desenvolvimento_varginha_modelo_abtn'", "NAME = 'relatorio_desenvolvimento_varginha_resumido'")
functions=original[original.index('book_id=0;fig_no=0'):original.index('def campaign():')]
lists='''
for heading,label,bookmark in [('LISTA DE ILUSTRAÇÕES','Figura 1 - Escola Industrial no acervo da remasterização','Figura1'),('LISTA DE TABELAS','Tabela 1 - Principais marcos do desenvolvimento','Tabela1')]:
    unnumbered(heading)
    p=para(label,'ListEntry');p.paragraph_format.line_spacing=1.5
    p.paragraph_format.tab_stops.add_tab_stop(Cm(15.9),WD_ALIGN_PARAGRAPH.RIGHT,1)
    p.add_run('\\t');field(p,f'PAGEREF {bookmark} \\\\h','0')
unnumbered('SUMÁRIO')
field(para('', 'ListEntry'),'TOC \\\\o "1-2" \\\\h \\\\z \\\\u','Atualizar sumário')
s=section();p=s.header.paragraphs[0];p.alignment=WD_ALIGN_PARAGRAPH.RIGHT
p.paragraph_format.first_line_indent=Cm(0);field(p,'PAGE','0')
for r in p.runs:r.font.name='Times New Roman';r.font.size=Pt(10)
'''
body='''
content=(OUT/'conteudo.md').read_text(encoding='utf-8')
for chunk in re.split(r'\\n\\s*\\n',content.strip()):
    if chunk.startswith('# '):
        p=para(chunk[2:],'Heading 1')
        if chunk.startswith('# 1 '):p.paragraph_format.page_break_before=False
    elif chunk.startswith('## '):para(chunk[3:],'Heading 2')
    elif chunk=='@page':page()
    elif chunk=='@timeline':
        table('Tabela',1,'Principais marcos do desenvolvimento',['Período','Marco documentado'],[
          ('Setembro','Protótipo, testes iniciais, entrada, sprites, combate e interações.'),
          ('1 a 3 de outubro','Expansão experimental, mapas, investigação e Livro do Tombo.'),
          ('4 a 7 de outubro','Etapas finais, chefe e condensação em 14 capítulos.'),
          ('8 a 21 de outubro','Remasterização, versões alpha e beta, abertura e sons.'),
          ('22 e 23 de outubro','Rotina, mesa escolar, falas, build 0.4.8 e nova revisão.')
        ],[3.4,12.6],'elaboração a partir do histórico e dos relatórios internos (Projeto O Segredo de Varginha, 2026a, 2026c).')
    elif chunk=='@fig':
        caption('Figura',1,'Escola Industrial no acervo da remasterização')
        p=para();p.alignment=WD_ALIGN_PARAGRAPH.CENTER;p.paragraph_format.first_line_indent=Cm(0)
        p.paragraph_format.line_spacing=1;p.paragraph_format.keep_with_next=True
        p.add_run().add_picture(str(ROOT/'Docs/Remaster20261008/Prints/02_Escola_Industrial.png'),width=Cm(11))
        source('acervo interno Remaster20261008, de outubro de 2026.')
    else:para(chunk.replace('\\n',' '))
unnumbered('5 REFERÊNCIAS BIBLIOGRÁFICAS',include=True)
refs=[
'CHACON, Scott; STRAUB, Ben. Pro Git. 2. ed. [S. l.]: Apress, 2014. Disponível em: https://git-scm.com/book/en/v2/Getting-Started-About-Version-Control. Acesso em: 23 out. 2026.',
'HUNICKE, Robin; LEBLANC, Marc; ZUBEK, Robert. MDA: a formal approach to game design and game research. In: AAAI WORKSHOP ON CHALLENGES IN GAME AI, 2004. Anais [...]. [S. l.]: AAAI, 2004. Disponível em: https://www.cs.northwestern.edu/~hunicke/MDA.pdf. Acesso em: 23 out. 2026.',
'MODELO ABTN. [S. l.: s. n.], [s. d.]. Arquivo PDF fornecido pelo solicitante como referência de apresentação.',
'PROJETO O SEGREDO DE VARGINHA. Histórico e código do projeto. Varginha, 2026a. Repositório local: 49 commits alcançáveis, de 8dd459a a a2946be, e alterações locais. Consulta em: 23 out. 2026.',
'PROJETO O SEGREDO DE VARGINHA. Documentos de design e campanha. Varginha, 2026b. Documentos internos. Localização: Docs/CampanhaOficial/Referencias/GDD_Usuario.md; O_Segredo_de_Varginha_LDD.md; Docs/CampanhaOficial/CAMPANHA_14_FASES.md. Consulta em: 23 out. 2026.',
'PROJETO O SEGREDO DE VARGINHA. Relatórios e evidências de desenvolvimento. Varginha, 2026c. Acervo interno em Docs e output, incluindo dialogue-casual-2026-10-23 e revisao-mestre-2026-10-23. Consulta em: 23 out. 2026.'
]
for ref in refs:para(ref,'Reference')
for st in doc.styles:
    if st.type==1:
        st.font.color.rgb=RGBColor(0,0,0)
        for b in st.element.xpath('./w:pPr/w:pBdr'):b.getparent().remove(b)
doc.core_properties.title='O Segredo de Varginha Relatório do desenvolvimento do projeto'
doc.core_properties.author='Turma do 3º ano de Desenvolvimento de Sistemas'
doc.core_properties.subject='Síntese documental até 23 de outubro de 2026'
upd=OxmlElement('w:updateFields');upd.set(qn('w:val'),'true');doc.settings.element.append(upd)
dest=OUT/(NAME+'.docx');doc.save(dest)
print(json.dumps({'docx':str(dest),'words_body':len(content.split()),'sections':5,'references':len(refs)},ensure_ascii=False))
'''
(OUT/'gerar_relatorio.py').write_text(prefix+lists+functions+body,encoding='utf-8')
for name in ['exportar_word.ps1','renderizar_conferir.py']:
    s=(OLD/name).read_text(encoding='utf-8').replace('relatorio_desenvolvimento_varginha_modelo_abtn','relatorio_desenvolvimento_varginha_resumido')
    (OUT/name).write_text(s,encoding='utf-8-sig' if name.endswith('.ps1') else 'utf-8')
