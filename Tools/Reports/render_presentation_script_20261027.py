from pathlib import Path
from xml.sax.saxutils import escape
import re
from reportlab.platypus import SimpleDocTemplate, Paragraph, PageBreak
from reportlab.lib.styles import ParagraphStyle
from reportlab.lib.enums import TA_LEFT
from reportlab.lib.colors import HexColor
from reportlab.lib.pagesizes import A4
from reportlab.pdfbase import pdfmetrics
from reportlab.pdfbase.ttfonts import TTFont
from pypdf import PdfReader

ROOT = Path(__file__).resolve().parents[2]
source = ROOT / 'Docs/Apresentacao/ROTEIRO_COMPLETO_30-10-2026.md'
target = ROOT / 'output/pdf/Roteiro_Apresentacao_Varginha_30-10-2026.pdf'
target.parent.mkdir(parents=True, exist_ok=True)
pdfmetrics.registerFont(TTFont('ScriptBody', 'C:/Windows/Fonts/calibri.ttf'))
pdfmetrics.registerFont(TTFont('ScriptBold', 'C:/Windows/Fonts/calibrib.ttf'))
pdfmetrics.registerFontFamily('ScriptBody', normal='ScriptBody', bold='ScriptBold')
body = ParagraphStyle('Body', fontName='ScriptBody', fontSize=11.5, leading=16,
                      textColor=HexColor('#25333a'), spaceAfter=9, alignment=TA_LEFT)
title = ParagraphStyle('Title', parent=body, fontName='ScriptBold', fontSize=25, leading=30, spaceAfter=17)
heading = ParagraphStyle('Heading', parent=body, fontName='ScriptBold', fontSize=16, leading=20,
                         textColor=HexColor('#153c45'), spaceBefore=17, spaceAfter=9, keepWithNext=True)
cue = ParagraphStyle('Cue', parent=body, fontSize=10, leading=14, textColor=HexColor('#53666e'), spaceAfter=12)
bullet = ParagraphStyle('Bullet', parent=body, leftIndent=12, firstLineIndent=-9)

def markup(text):
    return re.sub(r'\*\*(.+?)\*\*', r'<b>\1</b>', escape(text.strip())).replace('—', '-').replace('–', '-')

story = []
for block in source.read_text(encoding='utf-8').split('\n\n'):
    block=block.strip()
    if not block:
        continue
    if block.startswith('# '):
        story.append(Paragraph(markup(block[2:]), title))
        story.append(Paragraph('ROTEIRO DE FALA  |  APRESENTAÇÃO EM 30/10/2026', cue))
    elif block.startswith('## '):
        if block.startswith(('## 3.', '## 5.', '## 7.', '## 9.', '## Preparação')):
            story.append(PageBreak())
        story.append(Paragraph(markup(block[3:]), heading))
    elif block.startswith('- '):
        for line in block.splitlines():
            story.append(Paragraph('&#8226; ' + markup(line[2:]), bullet))
    else:
        story.append(Paragraph(markup(block), cue if block.startswith('Tempo:') else body))

def page_number(canvas, doc):
    canvas.setFont('ScriptBody', 9)
    canvas.setFillColor(HexColor('#53666e'))
    canvas.drawString(54, 28, 'O Segredo de Varginha - roteiro preparado em 27/10/2026')
    canvas.drawRightString(A4[0]-54, 28, str(doc.page))

SimpleDocTemplate(str(target), pagesize=A4, rightMargin=54, leftMargin=54, topMargin=45, bottomMargin=48,
                  title='O Segredo de Varginha - Roteiro da apresentação de 30/10/2026',
                  author='Projeto O Segredo de Varginha').build(story, onFirstPage=page_number, onLaterPages=page_number)
reader=PdfReader(str(target))
text='\n'.join(p.extract_text() or '' for p in reader.pages)
assert all(x in text for x in ['A proposta do jogo', 'Perguntas prováveis', '14 capítulos', '30/10/2026'])
assert all(len(p.extract_text() or '')>100 for p in reader.pages)
print({'output':str(target),'pages':len(reader.pages),'words':len(text.split())})
