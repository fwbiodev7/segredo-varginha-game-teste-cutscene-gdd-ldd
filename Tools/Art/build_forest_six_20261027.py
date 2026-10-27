from pathlib import Path
import json,hashlib,subprocess,sys,uuid
from PIL import Image
root=Path.cwd();out=root/'output/forest-six-2026-10-27';out.mkdir(parents=True,exist_ok=True)
source=root/'Assets/Resources/Varginha/StoryCharacters/ForestPresenceHybridV1.png';data=json.loads(source.with_suffix('.json').read_text(encoding='utf-8'))
recipe={'cell':[160,160],'columns':3,'padding':0,'pixelsPerUnit':100,'pivot':[80,86],'frames':[],'clips':{}}
for bank,row in enumerate([1,5]):
 ids=[]
 for index,column in enumerate([0,1,3]):
  f=data['frames'][row*4+column];x,y,w,h=f['rect'];local=f['anchorX']+column*224-x
  if bank:local=w-local
  key=('front_side_' if bank==0 else 'back_side_')+str(index);ids.append(key)
  recipe['frames'].append({'id':key,'source':str(source),'crop':[x,y,w,h],'mirrorX':bool(bank),'anchor':[round(local),h],'targetAnchor':[80,144],'durationMs':[167,167,166][index]})
 recipe['clips']['run_front_side' if bank==0 else 'run_back_side']={'frames':ids,'loop':True}
(out/'recipe.json').write_text(json.dumps(recipe,indent=2),encoding='utf-8')
script=Path(r'C:/Users/Usuario/.codex/skills/pixel-art-sprite-sheet-master/scripts/sprite_pipeline.py')
subprocess.run([sys.executable,str(script),'pack',str(out/'recipe.json'),'--out',str(out/'export')],check=True)
canonical=json.loads((out/'export/atlas.json').read_text(encoding='utf-8'));image=out/'export'/canonical['image']
target=root/'Assets/Resources/Varginha/StoryCharacters/ForestPresenceSix20261027.png';assert not target.exists();target.write_bytes(image.read_bytes())
runtime={'source':str(source.relative_to(root)),'sourceSha256':hashlib.sha256(source.read_bytes()).hexdigest(),'width':480,'height':320,'columns':3,'rows':2,'pixelsPerUnit':[100,100],'frames':[{'rect':[c*160,r*160,160,160],'anchorX':c*160+80,'lift':-.16} for r in range(2) for c in range(3)]}
target.with_suffix('.json').write_text(json.dumps(runtime,indent=2),encoding='utf-8')
meta=source.with_suffix('.png.meta').read_text(encoding='utf-8').replace('9c6ca50184c28b96e0ce04adc8cbde55',uuid.uuid4().hex)
target.with_suffix('.png.meta').write_text(meta,encoding='utf-8');target.with_suffix('.json.meta').write_text('fileFormatVersion: 2\nguid: '+uuid.uuid4().hex+'\n',encoding='utf-8')
alpha=set(Image.open(target).getchannel('A').getdata());(out/'contract.json').write_text(json.dumps({'allowSemiTransparency':any(0<a<255 for a in alpha),'lockPivot':True,'requiredClips':['run_front_side','run_back_side'],'maxTextureSize':2048}),encoding='utf-8')
print(json.dumps({'source_sha256':runtime['sourceSha256'],'frames':6,'canvas':[160,160],'ppu':100,'alpha':sorted(alpha),'image':str(target)}))
