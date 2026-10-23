from pathlib import Path
from PIL import Image
import hashlib,json,shutil

OUT=Path(__file__).resolve().parent
ROOT=OUT.parents[2]
generated=Path('C:/Users/Usuario/.codex/generated_images/01a16833-5617-7d30-bc5d-7298bd7f3401/exec-c742b6b2-05c8-4bfb-85ef-72e26dca1831.png')
source=ROOT/'Assets/Resources/Varginha/IllustratedMaps/IndustrialLibrary.png'
OUT.mkdir(exist_ok=True,parents=True)
shutil.copy2(generated,OUT/'generated-source.png')
base=Image.open(source).convert('RGBA')
globe=Image.open(generated).convert('RGBA').resize((40,56),Image.Resampling.NEAREST)
globe.save(OUT/'globe-native.png')
# Original scene pixels are copied exactly; only the old support under the new
# sphere is cleared using a neighbouring strip of the same authored wooden surface.
patch=base.crop((874,126,918,190))
for y in range(160,187):
    for x in range(882,913):patch.putpixel((x-874,y-126),base.getpixel((877+(x-882)%4,y)))
patch.alpha_composite(globe,(2,5))
patch.save(OUT/'globe-replacement-plate.png')
preview=base.copy();preview.alpha_composite(patch,(874,126))
preview.crop((840,90,985,245)).resize((580,620),Image.Resampling.NEAREST).save(OUT/'preview-context.png')
recipe={'cell':[44,64],'columns':1,'padding':0,'pixelsPerUnit':50,'pivot':[22,64],
        'frames':[{'id':'school_globe_static','source':'globe-replacement-plate.png','durationMs':1000}],
        'clips':{'school_globe_static':{'frames':['school_globe_static'],'loop':True}}}
(OUT/'recipe.json').write_text(json.dumps(recipe,indent=2),encoding='utf-8')
report={'source':str(source),'source_sha256':hashlib.sha256(source.read_bytes()).hexdigest(),
        'generated_source_sha256':hashlib.sha256(generated.read_bytes()).hexdigest(),
        'native_globe':[40,56],'plate':[44,64],'source_rectangle':[874,126,44,64],
        'PPU':50,'pivot_top_origin':[22,64],
        'changed_pixels':sum(a!=b for a,b in zip(base.getdata(),preview.getdata())),
        'outside_plate_changed':0,'artwork':'New isolated globe generated; exact original background retained outside the replacement plate; neighbouring authored wood reused only below the sphere.'}
(OUT/'provenance.json').write_text(json.dumps(report,indent=2),encoding='utf-8')
print(json.dumps(report))
