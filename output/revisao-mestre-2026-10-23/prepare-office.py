from pathlib import Path
from PIL import Image,ImageDraw,ImageFilter
import json,hashlib,re,shutil

ROOT=Path(__file__).resolve().parents[2]
OUT=Path(__file__).resolve().parent/'office'
OUT.mkdir(parents=True,exist_ok=True)
source=ROOT/'Assets/Resources/Varginha/HouseFurniture/AdultInteractiveBase.png'
original=Image.open(source).convert('RGBA')
layouts=ROOT/'Assets/Resources/Varginha/IllustratedMaps/Layouts.json'
data=json.loads(layouts.read_text(encoding='utf-8'))
chair=next(p for l in data['layouts'] if l['phase']==2 for p in l['props'] if p['name']=='Office_Chair')
# Preserve the chair pixels at their original native resolution. Only registration changes.
mask=Image.new('L',(54,59));ImageDraw.Draw(mask).polygon([(round(x*54),round(y*59)) for x,y in zip(chair['outline'][::2],chair['outline'][1::2])],fill=255)
ImageDraw.Draw(mask).rectangle((15,44,40,52),fill=255) # Preserve the low crossbar omitted by the foreground notch.
fixed=original.copy();old=original.crop((59,497,113,556))
generated=Path('C:/Users/Usuario/.codex/generated_images/01a16833-5617-7d30-bc5d-7298bd7f3401/exec-e0971760-35be-45b1-9680-8d0aa7b86426.png')
shutil.copy2(generated,OUT/'generated-clean-floor.png')
clean=Image.open(generated).convert('RGBA').resize((105,100),Image.Resampling.NEAREST)
# The floor contains longer feet than the separately rendered chair crop.
# Remove that full baked silhouette; the original prop renders the chair once.
wood=clean.crop((27,25,85,100))
fixed.paste(wood,(57,495))
dest=ROOT/'Assets/Resources/Varginha/HouseFurniture/AdultInteractiveSeatAligned.png'
fixed.save(dest)
original.crop((25,420,205,585)).resize((720,660),Image.Resampling.NEAREST).save(OUT/'before-floor.png')
fixed.crop((25,420,205,585)).resize((720,660),Image.Resampling.NEAREST).save(OUT/'after-floor.png')
with layouts.open('r',encoding='utf-8',newline='') as stream:text=stream.read()
start=text.index('"name": "Office_Chair"');end=text.index('"movable": false',start)+len('"movable": false')
block=text[start:end]
block=re.sub(r'("art":\s*\[\s*59,\s*)497',r'\g<1>481',block,count=1)
block=re.sub(r'("base":\s*\[\s*65,\s*)531',r'\g<1>515',block,count=1)
eol='\r\n' if '\r\n' in text else '\n'
if '"source":' not in block:
    block=block.replace('"motif": "OriginalChair",','"motif": "OriginalChair",'+eol+'          "source": [59, 497, 54, 59],',1)
with layouts.open('w',encoding='utf-8',newline='') as stream:stream.write(text[:start]+block+text[end:])
report={'source_sha256':hashlib.sha256(source.read_bytes()).hexdigest(),'target_sha256':hashlib.sha256(dest.read_bytes()).hexdigest(),
        'chair_source':[59,497,54,59],'chair_target':[59,481,54,59],'footprint_source':[65,531,43,22],'footprint_target':[65,515,43,22],
        'clean_floor_source':str(generated),'filter':'generated clean floor limited to [57,495,58,75]; chair rendered once from its unchanged original prop crop at integer offset',
        'original_asset_preserved':True,'changed_pixels':sum(a!=b for a,b in zip(original.getdata(),fixed.getdata()))}
(OUT/'provenance.json').write_text(json.dumps(report,indent=2),encoding='utf-8')
print(json.dumps(report))
