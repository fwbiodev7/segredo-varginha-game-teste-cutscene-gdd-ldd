"""Export one broad organic underworld manifestation proposal; no runtime changes."""
from pathlib import Path
from PIL import Image,ImageDraw
from collections import Counter
import json,hashlib,shutil
OUT=Path(__file__).resolve().parent;ROOT=OUT.parents[2]
ATLAS=ROOT/'Assets/Resources/Varginha/StoryCharacters/Directional20261026/ManifestationBoss.png'
MANIFEST=ATLAS.with_suffix('.json')
PREVIOUS=OUT.parent/'ManifestationBoss-prototype.png'
RAW=Path('C:/Users/Usuario/.codex/generated_images/01a1882b-9263-7e40-866e-4a90f45ad7cf/exec-f55746c0-8d92-45a0-bcf3-b5723786bb03.png')
COLORS=['#0D0A09','#211813','#38261D','#503829','#6B4C35','#866241','#A27E50','#BF9C66','#D3B584',
        '#292321','#3E322A','#5D4735','#775740','#96724D','#AE895A','#494440','#6B6053','#8F7D68',
        '#46060B','#7B0912','#B90D18','#E51621','#FF181F','#FF794F']
PALETTE=[tuple(bytes.fromhex(c[1:])) for c in COLORS]
def sha(p):return hashlib.sha256(p.read_bytes()).hexdigest()
def write(name,data):(OUT/name).write_text(json.dumps(data,ensure_ascii=False,indent=2)+'\n',encoding='utf-8')
sourcehash,manifesthash,previoushash=sha(ATLAS),sha(MANIFEST),sha(PREVIOUS)
data=json.loads(MANIFEST.read_text(encoding='utf-8'));rect=data['frames'][0]['rect']
atlas=Image.open(ATLAS).convert('RGBA');reference=atlas.crop((rect[0],rect[1],rect[0]+rect[2],rect[1]+rect[3]))
reference.save(OUT/'reference-current-front.png')
raw=Image.open(RAW).convert('RGBA');rawbbox=raw.getchannel('A').point(lambda a:255 if a>=128 else 0).getbbox()
trim=raw.crop(rawbbox);height=110;width=round(trim.width*height/trim.height)
assert width==133
sample=trim.resize((width,height),Image.Resampling.NEAREST);pixels=[]
for r,g,b,a in sample.getdata():
    if a<128:pixels.append((0,0,0,0))
    else:pixels.append((*min(PALETTE,key=lambda p:sum((v-u)**2 for v,u in zip(p,(r,g,b)))),255))
sprite=Image.new('RGBA',(width,height));sprite.putdata(pixels)
canvas=Image.new('RGBA',(160,144));canvas.paste(sprite,((160-width)//2,17))
canvas.save(OUT/'ManifestationBoss-prototype.png');shutil.copy2(RAW,OUT/'imagegen-source.png')
canvas.resize((640,576),Image.Resampling.NEAREST).save(OUT/'prototype-nearest-4x.png')
bbox=list(canvas.getchannel('A').getbbox());pivot=[80,bbox[3]];counts=Counter(p for p in canvas.getdata() if p[3])
assert bbox==[13,17,146,127] and bbox[3]-bbox[1]==reference.height
assert set(canvas.getchannel('A').getdata())=={0,255}
assert sha(ATLAS)==sourcehash and sha(MANIFEST)==manifesthash and sha(PREVIOUS)==previoushash
metadata={'schema':'pixel-art-sprite-sheet/v1','image':'ManifestationBoss-prototype.png','imageSize':[160,144],
          'pixelsPerUnit':data['pixelsPerUnit'][0],
          'frames':[{'id':'mystical_front_threat','rect':[0,0,160,144],'sourceSize':[160,144],
                     'trimOffset':[0,0],'pivot':pivot,'durationMs':120}],
          'clips':{'mystical_front_threat':{'frames':['mystical_front_threat'],'loop':False}},
          'displayName':'Ser místico — manifestação do submundo',
          'status':'PROTOTYPE ONLY — PENDING HUMAN APPROVAL; no integration',
          'identity':{'role':'final underworld manifestation; narrative unchanged',
                      'presence':'large, broad and heavy, same110px visible height as canonical boss',
                      'anatomy':'organic brown/umber skin, embedded root-like folds, broad shoulders, heavy long arms, bent supporting legs',
                      'head':'broad cranium integrated into shoulders with low rounded organic ridges',
                      'eyes':'strong vivid red, crimson socket rims, tiny contained highlights',
                      'tone':'mysterious, ancient, sober and inscrutable; no religious iconography',
                      'exclusions':'no horns, crown, gore, clothing, cyan cracks or modern equipment'},
          'boundsTopLeftExclusive':bbox,'visualFootPivotTopLeft':pivot,
          'pivotNote':'visual proposal contact only, not an approved runtime anchor',
          'scaleComparison':{'currentVisibleSize':[129,110],'proposalVisibleSize':[133,110],
                             'heightPercentVsCurrent':100,'widthPercentVsCurrent':round(133/129*100,2)},
          'sourceReference':{'atlas':str(ATLAS.relative_to(ROOT)),'sha256':sourcehash,'manifestSha256':manifesthash,
                             'frameRowColumn':[0,0],'rectTopLeft':rect},
          'trace':{'previousNarrowProposal':'../ManifestationBoss-prototype.png','previousSha256':previoushash,
                   'note':'previous proposal retained for trace; current human direction supersedes it'},
          'generation':{'tool':'built-in image_gen','source':'imagegen-source.png','sourceSha256':sha(RAW),
                        'prompt':'generation-prompt.txt','rawDimensions':list(raw.size),'rawAlpha128Bounds':list(rawbbox),
                        'export':'uniform nearest fit to110pxheight, widthrounded133;24-color palette,alpha128threshold,paddingonly'},
          'limitations':'one static front pose; no animation, no runtime, pending concept approval'}
write('prototype-metadata.json',metadata)
write('palette.json',{'colors':COLORS,'counts':{'#%02X%02X%02X'%p[:3]:n for p,n in counts.most_common()}})
write('contract.json',{'allowedColors':COLORS,'maxColors':24,'allowSemiTransparency':False,
                      'requiredClips':['mystical_front_threat'],'lockPivot':True,'maxTextureSize':160})
write('validation-evidence.json',{'dimensions':[160,144],'bounds':bbox,'visiblePixels':sum(counts.values()),
                                 'colors':len(counts),'alphaValues':[0,255],'sourceUnchanged':True,
                                 'previousProposalUnchanged':True,'pngSha256':sha(OUT/'ManifestationBoss-prototype.png'),
                                 'sourceAtlasSha256':sourcehash,'sourceManifestSha256':manifesthash,
                                 'previousProposalSha256':previoushash,'unityTested':False,'approval':'pending'})
preview=Image.new('RGB',(384,176),(29,34,41));d=ImageDraw.Draw(preview)
for y in range(16,160,8):
    for x in range(0,384,8):d.rectangle((x,y,x+7,y+7),fill=(39,45,52) if (x//8+y//8)%2 else (34,39,46))
d.text((8,3),'ATUAL',fill='white');d.text((200,3),'SER MISTICO - PROTOTIPO',fill='white')
baseline=148;d.line((0,baseline,383,baseline),fill=(89,101,113))
preview.paste(reference,(31,baseline-reference.height),reference)
preview.paste(canvas,(208,baseline-pivot[1]),canvas)
preview.save(OUT/'comparison-native.png')
preview.resize((1536,704),Image.Resampling.NEAREST).save(OUT/'comparison-nearest-4x.png')
print(json.dumps({'canvas':[160,144],'visible':[133,110],'bbox':bbox,'pivot':pivot,'colors':len(counts),
                  'alpha':[0,255],'canonicalAndPreviousPreserved':True}))
