"""Pack the ImageGen white raised-body redraw onto a native pixel grid.
No vehicle geometry is drawn or stretched here. The generated redraw is trimmed,
uniformly fit to52native pixels high (width123rounded), quantized and padded.
"""
from pathlib import Path
from PIL import Image, ImageDraw
from collections import Counter
import hashlib, json, shutil

OUT = Path(__file__).resolve().parent
ROOT = Path(__file__).resolve().parents[4]
BLUE = OUT.parent / 'FiatToroSide.png'
REF = ROOT / 'Assets/Resources/Varginha/StoryEffects/FuscaOriginal.png'
RAW = Path('C:/Users/Usuario/.codex/generated_images/01a1882b-9263-7e40-866e-4a90f45ad7cf/exec-dcb85ed1-26b5-4b00-a839-dae830294994.png')

def sha(p): return hashlib.sha256(p.read_bytes()).hexdigest()
def pixelsha(im): return hashlib.sha256(f'{im.width}x{im.height}:RGBA:'.encode()+im.tobytes()).hexdigest()
def write(name,data): (OUT/name).write_text(json.dumps(data,ensure_ascii=False,indent=2)+'\n',encoding='utf-8')
bluehash,refhash=sha(BLUE),sha(REF)
ref=Image.open(REF).convert('RGBA').crop((1,12,100,65))
blue=Image.open(BLUE).convert('RGBA')
palette=[px[:3] for px,n in Counter(ref.getdata()).most_common() if px[3]]
palette.extend([(246,247,247),(212,223,228)])
assert len(set(palette))==17
raw=Image.open(RAW).convert('RGBA')
rawbbox=raw.getchannel('A').point(lambda p:255 if p>=128 else 0).getbbox()
crop=raw.crop(rawbbox)
height=52
width=round(crop.width*height/crop.height)
assert width==123
sampled=crop.resize((width,height),Image.Resampling.NEAREST)
normalized=[]
for r,g,b,a in sampled.getdata():
    if a<128: normalized.append((0,0,0,0))
    else:
        color=min(palette,key=lambda p:sum((v-u)**2 for v,u in zip(p,(r,g,b))))
        normalized.append((*color,255))
sprite=Image.new('RGBA',(width,height)); sprite.putdata(normalized)
canvas=Image.new('RGBA',(128,72)); canvas.paste(sprite,(2,9))
canvas.save(OUT/'FiatToroSide.png')
shutil.copy2(RAW,OUT/'toro-white-tall-imagegen-source.png')
bbox=list(canvas.getchannel('A').getbbox())
baseline=bbox[3]-1
pivot=[64,baseline+1]
counts=Counter(px for px in canvas.getdata() if px[3])
bluebbox=blue.getchannel('A').getbbox()
increase=(bbox[3]-bbox[1])/(bluebbox[3]-bluebbox[1])-1
assert bbox==[2,9,125,61]
assert set(canvas.getchannel('A').getdata())=={0,255}
assert set(px[:3] for px in counts)<=set(palette)
assert sha(BLUE)==bluehash and sha(REF)==refhash
metadata={
 'schema':'pixel-art-sprite-sheet/v1','image':'FiatToroSide.png','imageSize':[128,72],'pixelsPerUnit':100,
 'frames':[{'id':'toro_side_right','rect':[0,0,128,72],'sourceSize':[128,72],
            'trimOffset':[0,0],'pivot':pivot,'durationMs':120}],
 'clips':{'toro_side_right':{'frames':['toro_side_right'],'loop':False}},
 'variant':'white-tall','measuredBoundsTopLeftExclusive':bbox,
 'wheelContactYTopLeft':baseline,'heightIncreaseAgainstBluePercent':round(increase*100,2),
 'provenance':{'generator':'built-in image_gen edits','source':'toro-white-tall-imagegen-source.png',
               'sourceSha256':sha(RAW),'rawAlpha128Bounds':list(rawbbox),'rawDimensions':list(raw.size),
               'originalBlue':'../FiatToroSide.png','originalBlueSha256':bluehash,
               'styleReference':'Assets/Resources/Varginha/StoryEffects/FuscaOriginal.png',
               'styleReferenceSha256':refhash,'styleReferenceUnityRect':[1,35,99,53],
               'artChanges':'ImageGen recolored body white and redrew raised suspension/clearance; wheels kept round.',
               'export':'Alpha128 trim; uniform nearest fit to52pxhigh with widthrounded123; palette17colors; padding(2,9). No body geometry drawn or nonuniform stretch in export.',
               'promptFile':'generation-prompts.txt','humanApproval':'Requested white and taller; final in-game acceptance pending'},
 'expectedUnityImport':{'filterMode':'Point','mipMaps':False,'compression':'None','wrap':'Clamp',
                        'spriteMeshType':'FullRect','pixelsPerUnit':100}}
write('FiatToroSide.json',metadata)
write('unity-slices.json',{'image':'FiatToroSide.png','spriteMode':'Single','rectUnityBottomLeft':[0,0,128,72],
                         'pivotTopLeftPixels':pivot,'pivotNormalizedUnity':[pivot[0]/128,1-pivot[1]/72],
                         'pixelsPerUnit':100,'filterMode':'Point','mipMaps':False,'textureCompression':'None',
                         'wrapMode':'Clamp','meshType':'FullRect','integrationStatus':'principal integrates/tests'})
write('palette.json',{'inheritedFromFusca':['#%02X%02X%02X'%p for p in palette[:-2]],
                      'whitePaintAdditions':['#F6F7F7','#D4DFE4'],
                      'counts':{'#%02X%02X%02X'%px[:3]:n for px,n in counts.most_common()}})
write('contract.json',{'allowedColors':['#%02X%02X%02X'%p for p in palette],
                       'allowSemiTransparency':False,'maxColors':17,'requiredClips':['toro_side_right'],
                       'lockPivot':True,'maxTextureSize':128})
write('reference-and-validation.json',{'referenceSha256Before':refhash,'referenceSha256After':sha(REF),
                                     'blueSha256Before':bluehash,'blueSha256After':sha(BLUE),
                                     'whiteFileSha256':sha(OUT/'FiatToroSide.png'),'whitePixelsSha256':pixelsha(canvas),
                                     'dimensions':[128,72],'bounds':bbox,'visibleHeight':52,'blueVisibleHeight':43,
                                     'heightIncreasePercent':round(increase*100,2),'alphaValues':[0,255],
                                     'colors':len(counts),'visiblePixels':sum(counts.values()),
                                     'noCanvasEdgeContact':True,'nativeResampling':'Nearest; uniform fit with integer size rounding',
                                     'pillowVersion':__import__('PIL').__version__})
canvas.resize((1024,576),Image.Resampling.NEAREST).save(OUT/'FiatToroSide-preview-8x.png')

# Three assets at the same native scale and aligned tire contact baseline.
preview=Image.new('RGBA',(402,100),(34,39,46,255)); d=ImageDraw.Draw(preview)
for y in range(16,90,8):
    for x in range(0,402,8):
        d.rectangle((x,y,x+7,y+7),fill=(46,52,60,255) if (x//8+y//8)%2 else (40,46,53,255))
d.text((8,3),'Fusca original',fill='white');d.text((120,3),'Toro azul',fill='white');d.text((260,3),'Toro branca +20.9%',fill='white')
ground=83;d.line((0,ground,401,ground),fill=(93,108,123,255))
preview.alpha_composite(ref,(8,ground-ref.height))
preview.alpha_composite(blue,(120,ground-56))
preview.alpha_composite(canvas,(260,ground-pivot[1]))
preview.convert('RGB').save(OUT/'comparison-native.png')
preview.convert('RGB').resize((1608,400),Image.Resampling.NEAREST).save(OUT/'comparison-nearest-4x.png')
print(json.dumps({'size':canvas.size,'bbox':bbox,'pivot':pivot,'heightIncreasePercent':round(increase*100,2),
                  'colors':len(counts),'alpha':[0,255],'blueAndReferenceUnchanged':True}))
