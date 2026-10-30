"""Extract whole supplied sprites; no drawing, recolouring or anatomical assembly.

The JPEG reference is preserved verbatim. Background removal only reaches pixels
connected to each crop's outer boundary. All scaling uses nearest neighbour and
one ratio for the complete character sheet.
"""
from pathlib import Path
from collections import deque
import hashlib
import json
import math
import shutil
import subprocess
import sys

from PIL import Image, ImageDraw, ImageFilter

ROOT = Path(__file__).resolve().parents[1]
OUT = ROOT / 'ArtSources' / 'Students20261029'
SOURCE = Path('C:/Users/Usuario/Downloads/WhatsApp Image 2026-10-29 at 1.56.38 PM.jpeg')
HELPER = Path('C:/Users/Usuario/.codex/skills/pixel-art-sprite-sheet-master/scripts/sprite_pipeline.py')
STUDENTS = [
    ('Fabio', 'Fábio', 94, 178), ('Marcos', 'Marcos', 180, 262),
    ('Matias', 'Matias', 265, 350), ('AnnaSabia', 'AnnaSofia', 440, 524),
    ('AnaTavares', 'AnaTavares', 527, 613), ('Yasmin', 'Yasmin', 616, 702),
    ('Pedro', 'Pedro', 705, 792), ('LuisMartins', 'LuisMartins', 796, 890),
    ('LuisMiguelMessias', 'LuisMiguelMessias', 896, 993),
]
GROUPS = [('S', [240,315,387,460,535]), ('W',[892,966,1039,1111,1184]),
          ('E',[1216,1289,1361,1434,1505]), ('N',[578,648,720,789,858])]


def sha(path):
    return hashlib.sha256(path.read_bytes()).hexdigest()


def remove_edge_background(crop):
    image = crop.convert('RGBA')
    w,h = image.size
    pixels = image.load()
    visited = set()
    queue = deque()

    def background(x,y):
        r,g,b,_ = pixels[x,y]
        # The sheet's blue background is distinct from brown hair, black outlines,
        # green sleeves, dark green trousers and white shoes. JPEG edge colours
        # require a small tolerance; no matching interior colour is erased globally.
        return b >= g - 3 and g >= r + 2 and b >= r + 6 and 22 <= max(r,g,b) < 145

    # Seal JPEG pinholes in the outer ink before flood removal. This is a mask
    # operation only: RGB pixels always come from the unmodified reference.
    ink=Image.new('L',(w,h));ip=ink.load()
    for y in range(h):
        for x in range(w): ip[x,y]=0 if background(x,y) else 255
    barrier=ink.filter(ImageFilter.MaxFilter(3)).filter(ImageFilter.MinFilter(3)).load()
    for x in range(w):
        for y in (0,h-1):
            if not barrier[x,y]: queue.append((x,y)); visited.add((x,y))
    for y in range(h):
        for x in (0,w-1):
            if not barrier[x,y]: queue.append((x,y)); visited.add((x,y))
    while queue:
        x,y = queue.popleft()
        pixels[x,y] = (0,0,0,0)
        for nx,ny in ((x-1,y),(x+1,y),(x,y-1),(x,y+1)):
            if 0 <= nx < w and 0 <= ny < h and (nx,ny) not in visited and not barrier[nx,ny]:
                visited.add((nx,ny)); queue.append((nx,ny))

    # Remove disconnected JPEG residue outside the complete main figure; never
    # remove a colour from a figure. Components touching a crop edge are reviewed.
    components = []
    done = set()
    for y in range(h):
        for x in range(w):
            if pixels[x,y][3] == 0 or (x,y) in done: continue
            part=[]; q=deque([(x,y)]); done.add((x,y))
            while q:
                ax,ay=q.popleft(); part.append((ax,ay))
                for nx,ny in ((ax-1,ay),(ax+1,ay),(ax,ay-1),(ax,ay+1)):
                    if 0<=nx<w and 0<=ny<h and (nx,ny) not in done and pixels[nx,ny][3]:
                        done.add((nx,ny));q.append((nx,ny))
            components.append(part)
    largest=max(components,key=len)
    keep=set(largest)
    for part in components:
        if part is largest: continue
        # Keep isolated shoe/silhouette pixels adjacent to the figure.
        if (len(part)>4 and not any(x in (0,w-1) or y in (0,h-1) for x,y in part)) or any((x+dx,y+dy) in keep for x,y in part for dx in range(-1,2) for dy in range(-1,2)):
            keep.update(part)
    for y in range(h):
        for x in range(w):
            if (x,y) not in keep: pixels[x,y]=(0,0,0,0)
    bounds=image.getbbox()
    if not bounds: raise ValueError('Empty reference sprite')
    if bounds[0]==0 or bounds[1]==0 or bounds[2]==w:
        raise ValueError(f'Silhouette touches source crop: {bounds} / {image.size}')
    return image.crop(bounds), bounds


def main():
    OUT.mkdir(parents=True, exist_ok=True)
    reference=OUT/'Reference_AlliedStudents.jpeg'
    if not reference.exists(): shutil.copy2(SOURCE, reference)
    source=Image.open(reference).convert('RGB')
    contact=Image.new('RGB',(9*136,4*144),(22,32,36))
    draw=ImageDraw.Draw(contact)
    overview=[]
    for n,(name,display,y0,y1) in enumerate(STUDENTS):
        frames=[];provenance=[]
        for direction,xs in GROUPS:
            for col in range(4):
                rect=(xs[col],y0,xs[col+1],y1)
                try: frame,bounds=remove_edge_background(source.crop(rect))
                except ValueError as exc: raise ValueError(f'{name} {direction}{col}: {exc}') from exc
                frames.append(frame)
                provenance.append({'direction':direction,'frame':col,'sourceRect':[rect[0]+bounds[0],rect[1]+bounds[1],frame.width,frame.height]})
        tallest=max(f.height for f in frames); widest=max(f.width for f in frames)
        height=44 if name=='AnnaSabia' else 56 if name=='AnaTavares' else 50
        ratio=min(height/tallest,56/widest)
        # A square, padded source keeps whole original JPEG-sized sprites intact.
        raw=Image.new('RGBA',(512,512))
        atlas=Image.new('RGBA',(256,256))
        for i,frame in enumerate(frames):
            raw.alpha_composite(frame,((i%4)*128+(128-frame.width)//2,(i//4)*128+120-frame.height))
            # Match the existing Unity exporter including its bottom-left sample
            # origin and post-sampling foot alignment.
            sw,sh=max(1,round(frame.width*ratio)),max(1,round(frame.height*ratio))
            small=Image.new('RGBA',(sw,sh))
            for sy in range(sh):
                fy=frame.height-1-min(frame.height-1,math.floor((sh-1-sy+.5)/ratio))
                for sx in range(sw):
                    fx=min(frame.width-1,math.floor((sx+.5)/ratio))
                    small.putpixel((sx,sy),frame.getpixel((fx,fy)))
            sb=small.getbbox()
            atlas.alpha_composite(small,((i%4)*64+(64-small.width)//2,(i//4)*64+58-sb[3]))
        for folder,img in [('Assets/ArtSource/Allies',raw),('Assets/Resources/Varginha/Allies',atlas)]:
            path=ROOT/folder/(name+'.png')
            before=OUT/'Before'/Path(folder).name/(name+'.png')
            if path.exists() and not before.exists():
                before.parent.mkdir(parents=True,exist_ok=True);shutil.copy2(path,before)
            img.save(path)
        per=OUT/name;per.mkdir(exist_ok=True)
        raw.save(per/'extracted-source.png');atlas.save(per/'atlas.png')
        manifest={'schemaVersion':1,'image':'atlas.png','imageSha256':sha(per/'atlas.png'),'nativeSize':[256,256],
                  'sourceSize':list(source.size),'pixelsPerUnit':64/1.35,'status':'candidate',
                  'displayName':display,'runtimeId':name,'reference':{'path':'../Reference_AlliedStudents.jpeg','sha256':sha(reference)},
                  'method':'whole-frame extraction; edge-connected background removal; one uniform nearest-neighbour ratio per sheet',
                  'scale':ratio,'coordinateOrigin':'top-left','provenance':provenance,
                  'quality':{'alphaThreshold':127,'maxColors':65536,'maxHeightRatio':1.3},'frames':[],'clips':[]}
        for row,(direction,_) in enumerate(GROUPS):
            ids=[]
            for col in range(4):
                id=f'walk_{direction}_{col:02}';ids.append(id)
                manifest['frames'].append({'id':id,'rect':[col*64,row*64,64,64],'pivot':[.5,.5],
                                          'footAnchor':[32,58],'durationMs':118,'action':'walk','direction':direction,'skin':'industrial','form':'normal'})
            manifest['clips'].append({'name':f'walk_{direction}','frames':ids,'loop':True,'allowStatic':False})
            view=atlas.crop((0,row*64,64,row*64+64)).resize((128,128),Image.Resampling.NEAREST)
            contact.paste(view,(n*136,row*144+16),view)
        manifest['requiredClips']=[c['name'] for c in manifest['clips']]
        manifest['timingNote']='118ms preview only, derived from the runtime minimum stride rate 8.5; actual animation is distance driven.'
        (per/'atlas.json').write_text(json.dumps(manifest,ensure_ascii=False,indent=2),encoding='utf8')
        draw.text((n*136+2,0),display,fill='white')
        overview.append({'name':name,'displayName':display,'frames':16,'ratio':ratio,'sourceTallest':tallest,'sourceWidest':widest})
    contact.save(OUT/'Contact_9Students.png')
    (OUT/'extraction-summary.json').write_text(json.dumps(overview,ensure_ascii=False,indent=2),encoding='utf8')
    print(json.dumps(overview,ensure_ascii=False))


if __name__=='__main__': main()
