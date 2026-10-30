"""Technical whole-frame extraction from the preserved ImageGen source; no redraw."""
from pathlib import Path
from PIL import Image
import hashlib, json, re, uuid
from collections import deque

ROOT = Path(__file__).resolve().parents[5]
SOURCE = Path(__file__).parent
OUTPUT = ROOT / 'Assets/Resources/Varginha/Allies/Giovana.png'
CANONICAL = ROOT / 'Assets/ArtSource/Allies/Giovana.png'
PREVIEW = ROOT / 'Preview/AlliesRedesign20261029/Giovana'
sha = lambda p: hashlib.sha256(p.read_bytes()).hexdigest()
im = Image.open(SOURCE / 'generated-source.png').convert('RGBA')
# Visually checked row boundaries. The generated source is not an exact regular grid.
xs, ys = [0, 313, 626, 939, 1254], [0, 313, 626, 939, 1254]
rects, sprites = [], []
for row in range(4):
    for col in range(4):
        area = im.crop((xs[col], ys[row], xs[col+1], ys[row+1]))
        bbox = area.getchannel('A').point(lambda a: 255 if a > 128 else 0).getbbox()
        x0,y0,x1,y1 = bbox
        # Keep a two-source-pixel transparent perimeter, preserving the generated alpha.
        x0,y0 = max(0,x0-2),max(0,y0-2)
        x1,y1 = min(area.width,x1+2),min(area.height,y1+2)
        rects.append([xs[col]+x0,ys[row]+y0,x1-x0,y1-y0])
        sprites.append(area.crop((x0,y0,x1,y1)))
scale = 50.5 / max(s.height for s in sprites)
atlas = Image.new('RGBA',(256,256))
frames = []
for i, sprite in enumerate(sprites):
    row,col = divmod(i,4)
    reduced = sprite.resize((round(sprite.width*scale),round(sprite.height*scale)),Image.Resampling.NEAREST)
    # Transparent padding from the generation contains disconnected color islands.
    # Keep the complete connected character silhouette; never key a material color.
    visible={(x,y) for y in range(reduced.height) for x in range(reduced.width)
        if reduced.getpixel((x,y))[3]>128}
    components=[]
    while visible:
        first=visible.pop(); queue=deque([first]); component={first}
        while queue:
            x,y=queue.popleft()
            for dx,dy in ((0,1),(1,0),(0,-1),(-1,0),(-1,1),(1,-1),(1,1),(-1,-1)):
                point=(x+dx,y+dy)
                if point in visible:
                    visible.remove(point); component.add(point); queue.append(point)
        components.append(component)
    body=max(components,key=len)
    for y in range(reduced.height):
        for x in range(reduced.width):
            if (x,y) not in body: reduced.putpixel((x,y),(0,0,0,0))
    # Reproduce the importer's solid-bounds centering and Unity foot baseline y=6.
    reduced = reduced.crop(reduced.getchannel('A').getbbox())
    atlas.alpha_composite(reduced,(col*64+(64-reduced.width)//2,row*64+58-reduced.height))
    frames.append({'id':f'walk_{"SWEN"[row]}_{col:02}', 'rect':[col*64,row*64,64,64],
        'sourceRect':rects[i], 'pivot':[.5,.5], 'footBaselineFromTop':58,
        'durationMs':100, 'action':'walk', 'direction':'SWEN'[row], 'skin':'marcos-tone','form':'normal'})
atlas.save(OUTPUT)
atlas.save(CANONICAL)
manifest = {'schemaVersion':1,'image':'../../../../Resources/Varginha/Allies/Giovana.png',
    'imageSha256':sha(OUTPUT),'nativeSize':[256,256], 'sourceSize':list(im.size),
    'sourceImage':'generated-source.png','sourceSha256':sha(SOURCE/'generated-source.png'),
    'pixelsPerUnit':64/1.35,'status':'visual-reviewed','visualApproval':False,
    'referenceHashes':{p.name:sha(p) for p in SOURCE.glob('*.jpeg')},
    'sourceRectsOrigin':'top-left','sourceToFinalUniformScale':scale,
    'canonicalRebuildSource':'../../Giovana.png', 'canonicalSourceSha256':sha(CANONICAL),
    'frames':frames, 'clips':[{'name':f'walk_{d}','frames':[f'walk_{d}_{i:02}'for i in range(4)],
        'loop':True,'allowStatic':False} for d in 'SWEN'],
    'requiredClips':[f'walk_{d}'for d in 'SWEN'],
    'quality':{'alphaThreshold':128,'maxColors':4096,'maxHeightRatio':1.3},
    'notes':['ImageGen redraw based on exact supplied identity, with Marcos used as skin-tone reference.',
        'No limbs or facial features transplanted; each complete frame uniformly reduced by nearest-neighbor.',
        'Disconnected transparent-padding color islands removed via connected alpha silhouette; no material color keyed.',
        '10 FPS matches existing ally attack presentation; runtime not exercised by art subagent.']}
# Path.relative_to does not compute paths through parents.
manifest['image']='../../../../Resources/Varginha/Allies/Giovana.png'
(SOURCE/'atlas.json').write_text(json.dumps(manifest,ensure_ascii=False,indent=2),encoding='utf-8')
meta = OUTPUT.with_suffix('.png.meta')
if not meta.exists():
    template = (OUTPUT.parent/'Marcos.png.meta').read_text(encoding='utf-8')
    meta.write_text(re.sub(r'guid: [0-9a-f]+',f'guid: {uuid.uuid4().hex}',template,count=1),encoding='utf-8')
canonical_meta = CANONICAL.with_suffix('.png.meta')
if not canonical_meta.exists():
    canonical_meta.write_text(re.sub(r'guid: [0-9a-f]+',f'guid: {uuid.uuid4().hex}',meta.read_text(encoding='utf-8'),count=1),encoding='utf-8')
atlas.resize((768,768),Image.Resampling.NEAREST).save(PREVIEW/'atlas-contact-3x.png')
for row,d in enumerate('SWEN'):
    sequence=[]
    for col in range(4):
        cel=atlas.crop((col*64,row*64,(col+1)*64,(row+1)*64))
        bg=Image.new('RGBA',(64,64),(28,40,48,255)); bg.alpha_composite(cel)
        sequence.append(bg.resize((256,256),Image.Resampling.NEAREST).convert('RGB'))
    sequence[0].save(PREVIEW/f'walk-{d}.gif',save_all=True,append_images=sequence[1:],duration=100,loop=0)
print(json.dumps({'atlas':str(OUTPUT),'size':atlas.size,'frames':len(frames),'sourceRects':rects}))
