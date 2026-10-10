"""Mount generated architectural crops without repainting original furniture.

Sources stay immutable. Only explicitly selected architectural strips are composited;
all gameplay coordinates and furniture metadata remain untouched.
"""
import json
import shutil
from pathlib import Path
from PIL import Image, ImageDraw, ImageFont
import numpy as np

ROOT=Path(__file__).resolve().parents[1]
ART=ROOT/'Assets/Resources/Varginha/IllustratedMaps'
DOC=ROOT/'Docs/Divisoes20261009'
OUT=ROOT/'Preview/Divisoes20261009'
GEN=Path('C:/Users/Usuario/.codex/generated_images/01a12299-a925-7bc1-9bfe-42d6a3fcfcec')
SPECS={
 1:('Child1996','exec-73d4a4d4-5366-438d-9e4c-95567dfc2e92.png',[
   ([356,77,38,167],[356,77,38,147]),([356,244,38,99],[356,224,38,119]),
   ([356,343,38,215],[356,343,38,207]),([356,558,38,89],[356,550,38,117]),
   ([356,647,38,142],[356,667,38,118]),
   ([0,404,354,26],[0,404,354,26]),
   ([671,510,209,35],[668,513,212,30]),
   ([671,543,33,48],[668,543,24,40]),([671,591,33,56],[668,583,24,64]),
   ([671,647,33,113],[668,647,24,113])]),
 2:('AdultEmpty','exec-817d0546-f6e7-4240-a9e0-48039eaadf9d.png',[
   ([448,60,41,220],[448,60,41,215]),([448,280,41,105],[448,275,41,110]),
   ([448,385,41,94],[448,385,41,85]),([448,479,41,109],[448,470,41,106]),
   ([448,588,41,206],[448,576,41,197]),
   ([0,389,142,40],[0,389,142,40]),([322,389,129,40],[322,389,129,40])]),
 6:('IndustrialLibrary','exec-5423c81a-9bac-40d9-adbb-ea73590280f5.png',[
   ([314,144,37,80],[317,144,36,80]),([685,144,36,80],[683,144,35,80]),
   ([304,580,43,143],[304,580,43,143]),([681,580,41,143],[681,580,41,143]),
   ([343,684,90,40],[343,684,90,40]),([591,684,95,40],[591,684,95,40])]),
 9:('Ouzana','exec-25840db6-5a43-43c2-815a-db39724e190e.png',[
   ([659,0,64,425],[659,0,64,442]),([659,642,64,331],[659,642,64,331]),
   ([25,369,245,54],[25,369,245,54]),([468,369,191,54],[468,369,191,54])]),
 10:('WorkshopEmpty','exec-a648f5be-e28e-4138-83b1-31cdb502f728.png',[
   ([496,46,63,336],[496,46,63,336]),([1006,46,61,336],[1006,46,61,336]),
   ([496,451,66,142],[496,451,66,142]),([1006,451,69,142],[1006,451,69,142])]),
 16:('Continuation16','exec-457c40d6-686f-48c4-86c0-27ce140c19e1.png',[
   (r,r) for r in [[423,5,26,90],[629,5,27,90],[580,317,36,167],
     [311,395,187,27],[314,521,163,18],[595,521,126,18],[478,475,19,102],
     [576,475,19,102],[497,575,79,13],[446,225,29,172]]]),
 18:('Continuation18','exec-469e7dd6-ca4e-4bca-8360-b43e0dbe560c.png',[
   (r,r) for r in [[112,15,20,120],[620,30,13,130],[231,176,240,22],
     [131,156,100,26],[471,159,148,24],[159,121,130,24],[489,122,94,26]]])
}

def box(rect,sx,sy):
    x,y,w,h=rect
    return tuple(round(v) for v in (x*sx,y*sy,(x+w)*sx,(y+h)*sy))

def montage(data,name,filename,crops):
    original=Image.open(DOC/(name+'_Antes.png')).convert('RGB')
    target=Image.open(DOC/(name+'_Alvo.png'))
    generated=Image.open(GEN/filename).convert('RGB')
    shutil.copy2(GEN/filename,DOC/(name+'_Gerado.png'))
    if data['phase']==18:
        # ImageGen added white letterboxing; remove only the border before registration.
        a=np.asarray(generated);rows=np.flatnonzero((a.min(axis=2)<240).mean(axis=1)>.8)
        generated=generated.crop((0,int(rows[0]),generated.width,int(rows[-1])+1))
    generated=generated.resize(target.size,Image.Resampling.NEAREST)
    sx=original.width/data['width'];sy=original.height/data['height']
    edited=original.copy();mask=Image.new('L',original.size);draw=ImageDraw.Draw(mask)
    for sample,destination in crops:
        a=box(sample,sx,sy);b=box(destination,sx,sy)
        piece=generated.crop(a).resize((b[2]-b[0],b[3]-b[1]),Image.Resampling.NEAREST)
        edited.paste(piece,b[:2]);draw.rectangle((b[0],b[1],b[2]-1,b[3]-1),fill=255)
    # Protect full original furniture rectangles, including attached decorations.
    for prop in data['props']:
        b=box(prop['art'],sx,sy)
        draw.rectangle((b[0],b[1],b[2]-1,b[3]-1),fill=0)
    final=Image.composite(edited,original,mask)
    final.save(ART/(name+'Divisoes.png'))
    final.save(OUT/(name+'_Depois.png'));original.save(OUT/(name+'_Antes.png'))
    mask.save(DOC/(name+'_Mascara.png'))
    return final

def collision_preview(data,base,name):
    sx=base.width/data['width'];sy=base.height/data['height']
    overlay=Image.new('RGBA',base.size);draw=ImageDraw.Draw(overlay)
    for wall in data['walls']:
        b=box(wall['rect'],sx,sy)
        draw.rectangle(b,fill=(245,45,45,65),outline=(255,60,65,205),width=2)
    for prop in data['props']:
        if len(prop.get('base',[]))!=4:continue
        b=box(prop['base'],sx,sy)
        draw.rectangle(b,fill=(235,45,45,60),outline=(255,70,70,205),width=2)
    for p in data['points']:
        x,y=p['pixel'];x*=sx;y*=sy
        draw.ellipse((x-7,y-7,x+7,y+7),fill=(45,170,255,225))
    Image.alpha_composite(base.convert('RGBA'),overlay).save(OUT/(name+'_Colisoes.png'))

def preview_furniture(data,base):
    # The empty adult/workshop backgrounds deliberately omit the runtime vehicle.
    # Preview that unchanged original sprite using the same aspect-preserving fit.
    result=base.convert('RGBA')
    for prop in data['props']:
        if prop['name']!='Fusca':continue
        x,y,w,h=prop['art'];sx=base.width/data['width'];sy=base.height/data['height']
        source=ROOT/'Assets/Resources/Varginha/StoryEffects/FuscaOriginal'
        if w<h:piece=Image.open(str(source)+'TopNorth.png').convert('RGBA')
        else:piece=Image.open(str(source)+'.png').convert('RGBA').crop((1,12,100,65))
        ratio=min(w*sx/piece.width,h*sy/piece.height)
        piece=piece.resize((round(piece.width*ratio),round(piece.height*ratio)),Image.Resampling.NEAREST)
        result.alpha_composite(piece,(round((x+w/2)*sx-piece.width/2),round((y+h/2)*sy-piece.height/2)))
    return result

def gallery(rows):
    css='body{background:#0b141b;color:#eadcbd;font:16px system-ui;max-width:1300px;margin:30px auto;padding:20px}img{width:100%;image-rendering:pixelated}article{padding:20px 0;border-top:1px solid #53605b}.pair{display:grid;grid-template-columns:1fr 1fr;gap:16px}a{color:#81cbbc}p{line-height:1.5}'
    html=['<!doctype html><meta charset="utf-8"><title>Divisões dos interiores</title><style>'+css+'</style>',
      '<h1>Divisões dos interiores</h1><p>Arquitetura refinada com os móveis e as coordenadas de jogo originais preservados. Sem execução de testes.</p>']
    for title,name,phases in rows:
        html.append(f'<article><h2>{title}</h2><p>Fases: {phases}</p><div class="pair"><div>Antes<img src="{name}_Antes.png"></div><div>Depois<img src="{name}_Depois.png"></div></div><p><a href="{name}_Colisoes.png">Mapa de colisões</a></p></article>')
    (OUT/'index.html').write_text('\n'.join(html),encoding='utf-8')

def main():
    OUT.mkdir(parents=True,exist_ok=True)
    baseline=json.loads((DOC/'Antes.json').read_text(encoding='utf-8'))
    target=ART/'Layouts.json';manifest=json.loads(target.read_text(encoding='utf-8'))
    rows=[];titles=['Casa de 1996','Casa de 2026','Biblioteca','Casa e laboratório da Ouzana','Oficina','Instalação subterrânea','Câmara dos mecanismos']
    for title,(phase,(name,file,crops)) in zip(titles,SPECS.items()):
        data=next(l for l in baseline['layouts'] if l['phase']==phase)
        final=montage(data,name,file,crops)
        shown=preview_furniture(data,final);shown.save(OUT/(name+'_Depois.png'))
        before=Image.open(DOC/(name+'_Antes.png')).convert('RGB')
        preview_furniture(data,before).save(OUT/(name+'_Antes.png'))
        collision_preview(data,shown,name)
        phases=[]
        for layout in manifest['layouts']:
            if layout['background'] not in [name,name+'Divisoes']:continue
            layout['background']=name+'Divisoes';layout['architecture']=name+'Divisoes'
            # Legacy copied floor/cap overlays were already baked into the baseline.
            # Depth-aware overlays are kept, because their contact is independent.
            layout['patches']=[p for p in layout.get('patches',[]) if p.get('ground')]
            phases.append(layout['phase'])
        rows.append((title,name,', '.join(map(str,phases))))
    target.write_text(json.dumps(manifest,ensure_ascii=False,indent=2)+'\n',encoding='utf-8')
    gallery(rows)
    print(json.dumps({'architectures':len(rows),'phases':[r[2] for r in rows],'gallery':str(OUT/'index.html')}))

if __name__=='__main__':main()
