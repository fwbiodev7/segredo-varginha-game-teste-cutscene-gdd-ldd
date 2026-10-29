"""Pack one generated proposal, without touching runtime assets or authored atlas."""
from pathlib import Path
from PIL import Image,ImageDraw
from collections import Counter
import hashlib,json,shutil

OUT=Path(__file__).resolve().parent
ROOT=OUT.parents[1]
ATLAS=ROOT/'Assets/Resources/Varginha/StoryCharacters/Directional20261026/ManifestationBoss.png'
MANIFEST=ATLAS.with_suffix('.json')
RAW=Path('C:/Users/Usuario/.codex/generated_images/01a1882b-9263-7e40-866e-4a90f45ad7cf/exec-ed9fb435-233d-400c-8ca3-53be08de5162.png')
COLORS=['#080606','#1A1110','#261712','#2F1E18','#3E271C','#4D3022','#5B3927',
        '#6B442E','#7A5136','#8C6240','#9D7248','#AD8250','#C0935A','#D4AA6C',
        '#3A0508','#600710','#870A12','#B90D18','#DE131E','#F52228','#FF151F',
        '#FF4338','#FF6352','#FFB787']
PALETTE=[tuple(bytes.fromhex(s[1:])) for s in COLORS]
def sha(p):return hashlib.sha256(p.read_bytes()).hexdigest()
def write(name,data):(OUT/name).write_text(json.dumps(data,ensure_ascii=False,indent=2)+'\n',encoding='utf-8')
before,manifestbefore=sha(ATLAS),sha(MANIFEST)
data=json.loads(MANIFEST.read_text(encoding='utf-8'))
atlas=Image.open(ATLAS).convert('RGBA');r=data['frames'][0]['rect']
reference=atlas.crop((r[0],r[1],r[0]+r[2],r[1]+r[3]))
reference.save(OUT/'reference-current-front.png')
reference.resize((reference.width*4,reference.height*4),Image.Resampling.NEAREST).save(OUT/'reference-current-front-4x.png')
raw=Image.open(RAW).convert('RGBA');bboxraw=raw.getchannel('A').point(lambda a:255 if a>=128 else 0).getbbox()
trim=raw.crop(bboxraw);height=110;width=round(trim.width*height/trim.height)
sample=trim.resize((width,height),Image.Resampling.NEAREST)
pixels=[]
for rr,gg,bb,aa in sample.getdata():
    if aa<128:pixels.append((0,0,0,0))
    else:pixels.append((*min(PALETTE,key=lambda p:sum((v-u)**2 for v,u in zip(p,(rr,gg,bb)))),255))
sprite=Image.new('RGBA',(width,height));sprite.putdata(pixels)
canvas=Image.new('RGBA',(128,128));offset=[(128-width)//2,9];canvas.paste(sprite,offset)
canvas.save(OUT/'ManifestationBoss-prototype.png')
canvas.resize((512,512),Image.Resampling.NEAREST).save(OUT/'prototype-nearest-4x.png')
shutil.copy2(RAW,OUT/'imagegen-source.png')
bbox=list(canvas.getchannel('A').getbbox());pivot=[64,bbox[3]]
counts=Counter(px for px in canvas.getdata() if px[3])
reds=[(x,y) for y in range(128) for x in range(128)
      if (lambda p:p[3] and p[0]>=180 and p[0]>p[1]*2.3 and p[0]>p[2]*2)(canvas.getpixel((x,y)))]
assert height==reference.height==110
assert sha(ATLAS)==before and sha(MANIFEST)==manifestbefore
assert set(canvas.getchannel('A').getdata())=={0,255}
assert set(px[:3] for px in counts)<=set(PALETTE)
metadata={'schema':'pixel-art-sprite-sheet/v1','image':'ManifestationBoss-prototype.png','imageSize':[128,128],
          'pixelsPerUnit':data['pixelsPerUnit'][0],
          'frames':[{'id':'prototype_front_threat','rect':[0,0,128,128],'sourceSize':[128,128],
                     'trimOffset':[0,0],'pivot':pivot,'durationMs':120}],
          'clips':{'prototype_front_threat':{'frames':['prototype_front_threat'],'loop':False}},
          'status':'PROTOTYPE ONLY — PENDING HUMAN VISUAL APPROVAL; not integrated',
          'identity':{'entity':'final manifestation; separate from injured creature narrative',
                      'skin':'dark brown/ochre; organic folds; narrow ribs and torso',
                      'head':'elongated; three rounded organic cranial protrusions',
                      'eyes':'large vivid red almond shapes, crimson rim, tiny controlled warm highlights',
                      'pose':'frontal threat, hunched torso, long thin arms, bent knees',
                      'removed':'cyan cracks, rocky armor, bulky bodybuilder silhouette',
                      'preservedScope':'no dialogue, attacks, hurtboxes, canonical atlas or gameplay changes'},
          'boundsTopLeftExclusive':bbox,'visualFootPivotTopLeft':pivot,
          'pivotNote':'visual prototype contact only; runtime anchor must be reviewed after approval',
          'sourceReference':{'atlas':str(ATLAS.relative_to(ROOT)),'sha256':before,
                             'manifestSha256':manifestbefore,'frameRowColumn':[0,0],'rectTopLeft':r,
                             'nativeVisibleHeight':110,'sourcePPU':data['pixelsPerUnit'][0]},
          'generation':{'tool':'built-in image_gen','source':'imagegen-source.png','sha256':sha(RAW),
                        'prompt':'generation-prompt.txt','rawDimensions':list(raw.size),'rawAlpha128Bounds':list(bboxraw),
                        'export':'uniform nearest fit to110px high (width rounded),24color nearest palette,alpha128threshold,padding only'},
          'expectedImportIfApproved':{'filter':'Point','mipmaps':False,'compression':'None'},
          'limitations':'one still pose, no directional/attack animation, no Unity/runtime validation, pending approval'}
write('prototype-metadata.json',metadata)
write('palette.json',{'colors':COLORS,'counts':{'#%02X%02X%02X'%p[:3]:n for p,n in counts.most_common()}})
write('contract.json',{'allowedColors':COLORS,'allowSemiTransparency':False,'maxColors':24,
                      'requiredClips':['prototype_front_threat'],'lockPivot':True,'maxTextureSize':128})
write('validation-evidence.json',{'dimensions':[128,128],'bounds':bbox,'visibleHeight':bbox[3]-bbox[1],
                                 'visiblePixels':sum(counts.values()),'colors':len(counts),'alphaValues':[0,255],
                                 'redEyePixels':len(reds),'redEyeBounds':[min(x for x,y in reds),min(y for x,y in reds),
                                                                      max(x for x,y in reds)+1,max(y for x,y in reds)+1],
                                 'pngSha256':sha(OUT/'ManifestationBoss-prototype.png'),
                                 'sourceAtlasBefore':before,'sourceAtlasAfter':sha(ATLAS),
                                 'sourceManifestBefore':manifestbefore,'sourceManifestAfter':sha(MANIFEST),
                                 'unityTested':False,'humanApproval':'pending'})

comparison=Image.new('RGB',(320,160),(29,34,41));d=ImageDraw.Draw(comparison)
for y in range(16,144,8):
    for x in range(0,320,8):
        d.rectangle((x,y,x+7,y+7),fill=(39,45,52) if (x//8+y//8)%2 else (34,39,46))
d.text((8,3),'ATUAL - frontal',fill='white');d.text((168,3),'PROTOTIPO - frontal',fill='white')
baseline=132;d.line((0,baseline,319,baseline),fill=(89,101,113))
comparison.paste(reference,(16,baseline-reference.height),reference)
comparison.paste(canvas,(176,baseline-pivot[1]),canvas)
comparison.save(OUT/'comparison-native.png')
comparison.resize((1280,640),Image.Resampling.NEAREST).save(OUT/'comparison-nearest-4x.png')
print(json.dumps({'canvas':[128,128],'bbox':bbox,'height':110,'colors':len(counts),'alpha':[0,255],
                  'redEyePixels':len(reds),'referenceUnchanged':sha(ATLAS)==before}))
