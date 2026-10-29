from pathlib import Path
import importlib.util, json, re, shutil
from PIL import Image, ImageOps, ImageDraw
import pdfplumber

OUT=Path(__file__).resolve().parent
stem='relatorio_desenvolvimento_o_segredo_de_varginha_23-10-2026'
script=Path('C:/Users/Usuario/.codex/plugins/cache/openai-primary-runtime/documents/26.1022.11436/skills/documents/render_docx.py')
spec=importlib.util.spec_from_file_location('canonical_docx_renderer',script)
module=importlib.util.module_from_spec(spec);spec.loader.exec_module(module)
def native_word_backend(doc_path,user_profile,convert_tmp_dir,stem,verbose):
    pdf=Path(doc_path).with_suffix('.pdf')
    if not pdf.is_file():raise FileNotFoundError('Exportação nativa pelo Word necessária antes da renderização.')
    dest=Path(convert_tmp_dir)/(stem+'.pdf');shutil.copy2(pdf,dest)
    return str(dest),'Backend adaptado para Microsoft Word instalado; PDF exportado do mesmo DOCX com campos atualizados. LibreOffice indisponível.'
module.convert_to_pdf=native_word_backend
pages=module.rasterize(str(OUT/(stem+'.docx')),str(OUT/'render'),100,False,False)
report={'backend':'Native Microsoft Word PDF; canonical render_docx.py rasterize with task-local conversion adapter; Poppler','page_count':len(pages),'pages':[]}
with pdfplumber.open(OUT/(stem+'.pdf')) as pdf:
    all_text=[]
    for n,p in enumerate(pdf.pages,1):
        text=p.extract_text() or '';all_text.append(text)
        words=p.extract_words()
        outside=[w['text'] for w in words if w['x0']<80 or w['x1']>543 or w['top']<44 or w['bottom']>790]
        report['pages'].append({'page':n,'words':len(words),'first':text.splitlines()[:2],'last':text.splitlines()[-2:],'outside_expected_print_area':outside})
    report['pdf_A4']=all(abs(p.width-595.28)<1 and abs(p.height-841.89)<1 for p in pdf.pages)
    report['unresolved_fields']=re.findall(r'Erro!|Error!|Atualizar sumário|Bookmark not defined|Indicador não definido','\n'.join(all_text))
    (OUT/'texto-extraido.txt').write_text('\n\n'.join(f'=== PÁGINA {n} ===\n{t}' for n,t in enumerate(all_text,1)),encoding='utf-8')

for first in range(0,len(pages),4):
    canvas=Image.new('RGB',(1200,1750),'#d7d7d7');draw=ImageDraw.Draw(canvas)
    for j,path in enumerate(pages[first:first+4]):
        im=Image.open(path).convert('RGB');im.thumbnail((580,825))
        x=10+(j%2)*600;y=28+(j//2)*875
        canvas.paste(im,(x,y));draw.text((x,y-19),f'Página {first+j+1}',fill='black')
    canvas.save(OUT/'render'/f'contato-{first+1:02d}.png')
(OUT/'qa-estrutura.json').write_text(json.dumps(report,ensure_ascii=False,indent=2),encoding='utf-8')
print(json.dumps({'pages':len(pages),'A4':report['pdf_A4'],'unresolved_fields':report['unresolved_fields'],'sparse_pages':[(p['page'],p['words'],p['first']) for p in report['pages'] if p['words']<70],'overflow':[(p['page'],p['outside_expected_print_area']) for p in report['pages'] if p['outside_expected_print_area']]},ensure_ascii=False))
