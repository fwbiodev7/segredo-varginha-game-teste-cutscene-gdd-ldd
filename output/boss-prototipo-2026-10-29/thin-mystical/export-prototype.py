from pathlib import Path
from PIL import Image,ImageDraw
from collections import Counter
import json,hashlib,shutil
OUT=Path(__file__).resolve().parent;ROOT=OUT.parents[2]
ATLAS=ROOT/'Assets/Resources/Varginha/StoryCharacters/Directional20261026/ManifestationBoss.png'
MANIFEST=ATLAS.with_suffix('.json')
RAW=Path('C:/Users/Usuario/.codex/generated_images/01a1882b-9263-7e40-866e-4a90f45ad7cf/exec-7bc88fcb-bd39-48d3-b89c-d338302b3ca5.png')
COLORS=json.loads((OUT.parent/'palette.json').read_text(encoding='utf-8'))['colors']
PALETTE=[tuple(bytes.fromhex(c[1:])) for c in COLORS]
def sha(p):return hashlib.sha256(p.read_bytes()).hexdigest()
def write(n,d):(OUT/n).write_text(json.dumps(d,ensure_ascii=False,indent=2)+'\n',encoding='utf-8')
sourcehash,manifesthash=sha(ATLAS),sha(MANIFEST)
data=json.loads(MANIFEST.read_text(encoding='utf-8'));rect=data['frames'][0]['rect']
atlas=Image.open(ATLAS).convert('RGBA');reference=atlas.crop((rect[0],rect[1],rect[0]+rect[2],rect[1]+rect[3]))
raw=Image.open(RAW).convert('RGBA');rawbbox=raw.getchannel('A').point(lambda a:255 if a>=128 else 0).getbbox()
trim=raw.crop(rawbbox);height=110;width=round(trim.width*height/trim.height)
sample=trim.resize((width,height),Image.Resampling.NEAREST);pixels=[]
for r,g,b,a in sample.getdata():
    pixels.append((0,0,0,0) if a<128 else (*min(PALETTE,key=lambda p:sum((v-u)**2 for v,u in zip(p,(r,g,b)))),255))
sprite=Image.new('RGBA',(width,height));sprite.putdata(pixels)
canvas=Image.new('RGBA',(128,128));canvas.paste(sprite,((128-width)//2,9))
canvas.save(OUT/'ManifestationBoss-prototype.png')
canvas.resize((512,512),Image.Resampling.NEAREST).save(OUT/'prototype-nearest-4x.png')
shutil.copy2(RAW,OUT/'imagegen-source.png')
bbox=list(canvas.getchannel('A').getbbox());pivot=[64,bbox[3]];counts=Counter(p for p in canvas.getdata() if p[3])
assert bbox[3]-bbox[1]==110 and set(canvas.getchannel('A').getdata())=={0,255}
assert sha(ATLAS)==sourcehash and sha(MANIFEST)==manifesthash
metadata={'schema':'pixel-art-sprite-sheet/v1','image':'ManifestationBoss-prototype.png','imageSize':[128,128],
 'pixelsPerUnit':data['pixelsPerUnit'][0],
 'frames':[{'id':'thin_mystical_front','rect':[0,0,128,128],'sourceSize':[128,128],'trimOffset':[0,0],'pivot':pivot,'durationMs':120}],
 'clips':{'thin_mystical_front':{'frames':['thin_mystical_front'],'loop':False}},
 'displayName':'Ser místico do submundo — alto e magro',
 'status':'PROTOTYPE ONLY; PENDING HUMAN APPROVAL; NOT INTEGRATED',
 'identity':{'proportions':'tall slender hunched figure; narrow torso and hips; long thin arms and legs; no exaggerated musculature',
             'head':'elongated organic cranium with three low rounded ridges',
             'skin':'dark brown/ochre matte organic folds and sparse ribs',
             'eyes':'large bright saturated red, controlled socket glow',
             'pose':'single frontal resting/threat pose, arms hanging near body',
             'role':'final underworld manifestation, narrative unchanged'},
 'boundsTopLeftExclusive':bbox,'visualFootPivotTopLeft':pivot,
 'scale':{'visibleHeight':110,'currentBossVisibleHeight':110,'sourcePPU':data['pixelsPerUnit'][0],
          'worldHeightIfLaterImportedAtSourcePPU':110/data['pixelsPerUnit'][0],
          'note':'tall stature means height, not muscle/width; no runtime import performed'},
 'sourceReference':{'atlas':str(ATLAS.relative_to(ROOT)),'sha256':sourcehash,'manifestSha256':manifesthash,'rectTopLeft':rect},
 'generation':{'tool':'built-in image_gen edit','input':'../prototype-nearest-4x.png','source':'imagegen-source.png',
               'sourceSha256':sha(RAW),'prompt':'generation-prompt.txt','rawDimensions':list(raw.size),'rawAlpha128Bounds':list(rawbbox),
               'export':'uniform nearest fit to110pxheight;24color nearest palette;alpha128threshold;paddingonly'},
 'limitations':'one static prototype; no animation, hurtbox, attacks, scripts, dialogue, runtime or original assets modified'}
write('prototype-metadata.json',metadata)
write('contract.json',{'allowedColors':COLORS,'maxColors':24,'allowSemiTransparency':False,'requiredClips':['thin_mystical_front'],'lockPivot':True,'maxTextureSize':128})
write('validation-evidence.json',{'dimensions':[128,128],'bounds':bbox,'visibleSize':[width,height],'visiblePixels':sum(counts.values()),
                                 'colors':len(counts),'alphaValues':[0,255],'sourceAtlasUnchanged':sha(ATLAS)==sourcehash,
                                 'sourceManifestUnchanged':sha(MANIFEST)==manifesthash,'pngSha256':sha(OUT/'ManifestationBoss-prototype.png'),
                                 'unityTested':False,'approval':'pending'})
preview=Image.new('RGB',(320,160),(29,34,41));d=ImageDraw.Draw(preview)
for y in range(16,144,8):
    for x in range(0,320,8):d.rectangle((x,y,x+7,y+7),fill=(39,45,52) if (x//8+y//8)%2 else (34,39,46))
d.text((8,3),'ATUAL',fill='white');d.text((168,3),'ALTO E MAGRO - PROTOTIPO',fill='white')
baseline=132;d.line((0,baseline,319,baseline),fill=(89,101,113))
preview.paste(reference,(16,baseline-reference.height),reference)
preview.paste(canvas,(176,baseline-pivot[1]),canvas)
preview.save(OUT/'comparison-native.png');preview.resize((1280,640),Image.Resampling.NEAREST).save(OUT/'comparison-nearest-4x.png')
print(json.dumps({'canvas':[128,128],'visibleSize':[width,height],'bbox':bbox,'pivot':pivot,'colors':len(counts),'alpha':[0,255],'sourcePreserved':True}))
