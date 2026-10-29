"""Normalize an unapproved ImageGen candidate, preserving the original game reference.

No vehicle geometry is drawn here. The generated candidate is sampled onto the
requested native grid with nearest-neighbor, restricted to the Fusca palette,
and binary alpha. Reference and native images are enlarged only by integer NN.
"""
from PIL import Image, ImageDraw
from pathlib import Path
from collections import Counter
import hashlib
import json
import shutil

ROOT = Path(__file__).resolve().parents[3]
OUT = Path(__file__).resolve().parent
REFERENCE = ROOT / 'Assets/Resources/Varginha/StoryEffects/FuscaOriginal.png'
RAW = Path('C:/Users/Usuario/.codex/generated_images/01a1882b-9263-7e40-866e-4a90f45ad7cf/exec-fb276c94-abe5-4213-a806-b62728217acd.png')

def sha(path):
    return hashlib.sha256(path.read_bytes()).hexdigest()

def pixel_sha(im):
    return hashlib.sha256(f'{im.width}x{im.height}:RGBA:'.encode() + im.tobytes()).hexdigest()

def write(name, data):
    (OUT / name).write_text(json.dumps(data, ensure_ascii=False, indent=2) + '\n', encoding='utf-8')

reference_hash = sha(REFERENCE)
reference = Image.open(REFERENCE).convert('RGBA')
ref = reference.crop((1, 12, 100, 65))
palette = [rgb[:3] for rgb, count in Counter(ref.getdata()).most_common() if rgb[3]]
assert len(palette) == 15
ref.save(OUT / 'reference-fusca-side.png')
shutil.copy2(RAW, OUT / 'toro-imagegen-source.png')
raw = Image.open(RAW).convert('RGBA')
sampled = raw.resize((128, 64), Image.Resampling.NEAREST)
pixels = []
for r, g, b, a in sampled.getdata():
    if a < 128:
        pixels.append((0, 0, 0, 0))
    else:
        best = min(palette, key=lambda p: sum((v-u)**2 for v, u in zip(p, (r, g, b))))
        pixels.append((*best, 255))
toro = Image.new('RGBA', (128, 64))
toro.putdata(pixels)
toro.save(OUT / 'FiatToroSide.png')
bbox = toro.getchannel('A').getbbox()
assert bbox is not None
baseline = bbox[3] - 1
pivot = [64, baseline + 1]
colors = Counter(px for px in toro.getdata() if px[3])
assert set(px[:3] for px in colors).issubset(set(palette))
assert set(toro.getchannel('A').getdata()) == {0, 255}
assert sha(REFERENCE) == reference_hash

meta = {
    'schema': 'pixel-art-sprite-sheet/v1', 'image': 'FiatToroSide.png',
    'imageSize': [128, 64], 'pixelsPerUnit': 100,
    'frames': [{'id': 'toro_side_right', 'rect': [0, 0, 128, 64],
                'sourceSize': [128, 64], 'trimOffset': [0, 0],
                'pivot': pivot, 'durationMs': 120}],
    'clips': {'toro_side_right': {'frames': ['toro_side_right'], 'loop': False}},
    'provenance': {
        'generator': 'built-in image_gen', 'approval': 'candidate; requires visual acceptance in game',
        'sourceImage': 'toro-imagegen-source.png', 'sourceSha256': sha(RAW),
        'referenceImage': 'Assets/Resources/Varginha/StoryEffects/FuscaOriginal.png',
        'referenceSha256': reference_hash, 'referenceUnityRect': [1,35,99,53],
        'referencePillowRect': [1,12,99,53], 'referenceCropPixelSha256': pixel_sha(ref),
        'normalization': 'Nearest sample generated candidate1774x887 onto128x64; alpha threshold128; nearest reference RGB palette; no geometry redrawn.',
        'referencePixelsModified': False,
        'promptFile': 'generation-prompt.txt'
    },
    'measuredBounds': list(bbox), 'wheelContactYTopLeft': baseline,
    'expectedUnityImport': {'filterMode': 'Point', 'mipMaps': False, 'compression': 'None',
                            'wrap': 'Clamp', 'spriteMeshType': 'FullRect', 'pixelsPerUnit': 100}
}
write('FiatToroSide.json', meta)
write('palette.json', {'source':'FuscaOriginal first side frame','colors':['#%02X%02X%02X'%p for p in palette],
                       'toroCounts':{'#%02X%02X%02X'%px[:3]:n for px,n in colors.most_common()}})
write('contract.json', {'allowedColors':['#%02X%02X%02X'%p for p in palette],
                        'allowSemiTransparency':False,'maxColors':15,
                        'requiredClips':['toro_side_right'],'lockPivot':True,'maxTextureSize':128})
write('unity-slices.json', {'image':'FiatToroSide.png','spriteMode':'Single','rectUnityBottomLeft':[0,0,128,64],
                           'pivotTopLeftPixels':pivot,'pivotNormalizedUnity':[pivot[0]/128,1-pivot[1]/64],
                           'pixelsPerUnit':100,'filterMode':'Point','mipMaps':False,'textureCompression':'None',
                           'wrapMode':'Clamp','meshType':'FullRect','integrationStatus':'not imported or run by art agent'})
write('reference-and-validation.json', {
    'referenceFileSha256Before':reference_hash,'referenceFileSha256After':sha(REFERENCE),
    'referenceFullImagePixelsSha256':pixel_sha(reference),'referenceCropPixelsSha256':pixel_sha(ref),
    'toroFileSha256':sha(OUT/'FiatToroSide.png'),'toroPixelsSha256':pixel_sha(toro),
    'sourceGeneratedDimensions':list(raw.size),'canonicalDimensions':list(toro.size),
    'alphaValues':sorted(set(toro.getchannel('A').getdata())), 'opaqueColors':len(colors),
    'bboxTopLeftExclusive':list(bbox),'allVisiblePixelsWithinReferencePalette':True,
    'noCanvasEdgeContact':all((bbox[0]>0,bbox[1]>0,bbox[2]<128,bbox[3]<64)),
    'wheelContactYTopLeft':baseline,'expectedFilter':'Point/nearest-neighbor',
    'pythonVersion':__import__('sys').version,'pillowVersion':__import__('PIL').__version__
})

# Equal-scale comparison with tire contact points aligned; no resizing of the reference.
canvas = Image.new('RGBA', (260, 88), (34,39,46,255))
draw = ImageDraw.Draw(canvas)
for y in range(14, 78, 8):
    for x in range(0, 260, 8):
        draw.rectangle((x,y,x+7,y+7),fill=(46,52,60,255) if (x//8+y//8)%2 else (40,46,53,255))
draw.text((8, 2), 'Fusca original 99x53', fill='white')
draw.text((126, 2), 'Toro nova 128x64', fill='white')
ground_y = 73
draw.line((0,ground_y,259,ground_y),fill=(93,108,123,255))
canvas.alpha_composite(ref,(8,ground_y-ref.height))
canvas.alpha_composite(toro,(125,ground_y-(baseline+1)))
canvas.convert('RGB').save(OUT/'comparison-native.png')
canvas.convert('RGB').resize((1560,528),Image.Resampling.NEAREST).save(OUT/'comparison-nearest-6x.png')
toro.resize((1024,512),Image.Resampling.NEAREST).save(OUT/'FiatToroSide-preview-8x.png')
print(json.dumps({'image':'FiatToroSide.png','size':toro.size,'bbox':bbox,'pivot':pivot,'colors':len(colors),
                  'alphaValues':[0,255],'referenceUnchanged':sha(REFERENCE)==reference_hash},ensure_ascii=False))
