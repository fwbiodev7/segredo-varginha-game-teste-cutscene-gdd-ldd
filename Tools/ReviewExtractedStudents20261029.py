"""Geometry/provenance review for the supplied reference extraction."""
from pathlib import Path
import json
from PIL import Image, ImageDraw

ROOT=Path(__file__).resolve().parents[1]
DELIVERY=ROOT/'ArtSources/Students20261029'
NAMES=['Fabio','Marcos','Matias','AnnaSabia','AnaTavares','Yasmin','Pedro','LuisMartins','LuisMiguelMessias']
contact=Image.new('RGB',(16*96,9*112),(28,38,42))
draw=ImageDraw.Draw(contact)
errors=[]
statistics={}
for row,name in enumerate(NAMES):
    image=Image.open(DELIVERY/name/'atlas.png')
    runtime=Image.open(ROOT/'Assets/Resources/Varginha/Allies'/f'{name}.png')
    if runtime.tobytes()!=image.tobytes(): errors.append((name,'runtime pixel mismatch'))
    draw.text((2,row*112),name,fill='white')
    unique=[]
    for i in range(16):
        frame=image.crop((i%4*64,i//4*64,i%4*64+64,i//4*64+64))
        bounds=frame.getbbox()
        if bounds is None or bounds[3]!=58 or bounds[0]<4 or bounds[2]>60 or bounds[1]<2:
            errors.append((name,i,bounds))
        zoom=frame.resize((96,96),Image.Resampling.NEAREST)
        contact.paste(zoom,(i*96,row*112+16),zoom)
        unique.append(frame.tobytes())
    statistics[name]={'frames':16,'distinctFrames':len(set(unique)),'runtimePixelsEqual':runtime.tobytes()==image.tobytes()}
contact.save(DELIVERY/'Contact_All144Frames.png')
report={'errors':errors,'statistics':statistics,'notes':['Comparison uses decoded RGBA pixels; PNG encoders can produce different bytes.','Baselines measured in top-left coordinates: bottom exclusive=58, equivalent to Unity yMin=6.']}
(DELIVERY/'geometry-review.json').write_text(json.dumps(report,ensure_ascii=False,indent=2),encoding='utf8')
print(json.dumps(report))
raise SystemExit(1 if errors else 0)
