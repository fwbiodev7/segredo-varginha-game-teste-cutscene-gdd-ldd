"""Technical whole-sprite packing and portrait sizing. Drawing is ImageGen output."""
from pathlib import Path
from collections import deque
from PIL import Image, ImageDraw
import hashlib, json, os, re, uuid

ROOT=Path(__file__).resolve().parents[2]
IDS=['Giovana','Pedro','Yasmin','LuisMartins']
WALK_ROOT=ROOT/'ArtSources/StudentsCrisp20261029'
PORTRAIT_ROOT=ROOT/'ArtSources/StudentPortraits20261029'
OUTPUT=ROOT/'Assets/Resources/Varginha/Allies'
PREVIEW=ROOT/'Preview/StudentsCrisp20261029/GroupGiovana'
PREVIEW.mkdir(parents=True,exist_ok=True)
SHA=lambda p:hashlib.sha256(p.read_bytes()).hexdigest()
BOUNDS={'Giovana':[0,333,624,913,1254],'Pedro':[0,313,612,905,1254],
        'Yasmin':[0,337,630,926,1254],'LuisMartins':[0,331,629,920,1254]}

def largest_silhouette(im):
    im=im.copy()
    visible={(x,y)for y in range(im.height)for x in range(im.width)if im.getpixel((x,y))[3]>=128}
    components=[]
    while visible:
        first=visible.pop(); group={first}; queue=deque([first])
        while queue:
            x,y=queue.popleft()
            for dx,dy in [(0,1),(1,0),(0,-1),(-1,0),(-1,-1),(-1,1),(1,-1),(1,1)]:
                point=(x+dx,y+dy)
                if point in visible: visible.remove(point);group.add(point);queue.append(point)
        components.append(group)
    keep=max(components,key=len)
    pixels=im.load()
    for y in range(im.height):
        for x in range(im.width):
            if (x,y)not in keep:pixels[x,y]=(0,0,0,0)
    return im.crop(im.getchannel('A').getbbox())

def write_meta(path):
    dest=path.with_suffix(path.suffix+'.meta')
    if dest.exists():return
    template=(OUTPUT/'Marcos.png.meta').read_text(encoding='utf-8')
    template=re.sub(r'guid: [0-9a-f]+','guid: '+uuid.uuid4().hex,template,count=1)
    template=re.sub(r'textureCompression: \d+','textureCompression: 0',template)
    dest.write_text(template,encoding='utf-8')

summary={}
walk_contact=Image.new('RGBA',(1024,284),(27,39,47,255))
portrait_contact=Image.new('RGBA',(1024,288),(27,39,47,255))
for index,name in enumerate(IDS):
    folder=WALK_ROOT/name; source=folder/'generated-source.png'
    image=Image.open(source).convert('RGBA'); ys=BOUNDS[name]
    xs=[round(image.width*i/4)for i in range(5)]
    crops=[];source_rects=[]
    for row in range(4):
        for col in range(4):
            region=image.crop((xs[col],ys[row],xs[col+1],ys[row+1]))
            bbox=region.getchannel('A').point(lambda a:255 if a>=128 else 0).getbbox()
            x0,y0,x1,y1=bbox
            crops.append(region.crop(bbox))
            source_rects.append([xs[col]+x0,ys[row]+y0,x1-x0,y1-y0])
    tallest=max(im.height for im in crops)
    selected=None
    for step in range(21):
        scale=(50+step*.05)/tallest
        reduced=[largest_silhouette(im.resize((max(1,round(im.width*scale)),max(1,round(im.height*scale))),Image.Resampling.NEAREST))for im in crops]
        if max(im.height for im in reduced)==50:selected=(scale,reduced);break
    if selected is None:raise RuntimeError('Cannot meet common50pixel height: '+name)
    scale,reduced=selected
    atlas=Image.new('RGBA',(256,256))
    for i,im in enumerate(reduced):
        row,col=divmod(i,4)
        atlas.alpha_composite(im,(col*64+(64-im.width)//2,row*64+58-im.height))
    canonical=ROOT/f'Assets/ArtSource/Allies/{name}.png'
    runtime=OUTPUT/f'{name}.png'
    atlas.save(canonical);atlas.save(runtime)
    write_meta(canonical);write_meta(runtime)
    frames=[{'id':f'walk_{d}_{col:02}','rect':[col*64,row*64,64,64],
        'pivot':[.5,.5],'durationMs':100,'action':'walk','direction':d,
        'skin':'default','form':'normal','sourceRect':source_rects[row*4+col],
        'footBaselineUnity':6}for row,d in enumerate('SWEN')for col in range(4)]
    manifest={'schemaVersion':1,'image':os.path.relpath(runtime,folder).replace('\\','/'),
        'imageSha256':SHA(runtime),'nativeSize':[256,256],'sourceImage':'generated-source.png',
        'sourceSha256':SHA(source),'sourceSize':list(image.size),'sourceToFinalUniformScale':scale,
        'pixelsPerUnit':64/1.35,'status':'visual-reviewed','visualApproval':False,'frames':frames,
        'clips':[{'name':'walk_'+d,'frames':[f'walk_{d}_{col:02}'for col in range(4)],'loop':True,'allowStatic':False}for d in 'SWEN'],
        'requiredClips':['walk_'+d for d in 'SWEN'],
        'quality':{'alphaThreshold':128,'maxColors':16384,'maxHeightRatio':1.3},
        'notes':['Complete figures generated with built-in ImageGen against current identity and Bianca/Moscardini style benchmarks.',
            'Whole-frame technical cropping, common uniform nearest-neighbor scaling and connected-alpha padding cleanup only; no anatomy assembly.',
            'Source canonical4x4 and runtime have identical RGBA before Unity import; foot baseline Unity6, maximum height50.',
            'Editor and runtime validation delegated to parent; no Unity used by this art agent.']}
    if name=='Pedro':manifest['notes'].append('Latest priority: brown/ochre Domo-motif beanie and ROUND eyeglasses; front motif absent on back views. Historical no-hat source rejected.')
    (folder/'atlas.json').write_text(json.dumps(manifest,ensure_ascii=False,indent=2),encoding='utf-8')
    atlas.resize((768,768),Image.Resampling.NEAREST).save(folder/'contact-3x.png')
    bounds=[];distinct=[]
    for row,d in enumerate('SWEN'):
        gif=[];hashes=set()
        for col in range(4):
            cel=atlas.crop((col*64,row*64,col*64+64,row*64+64));bbox=cel.getchannel('A').getbbox()
            assert bbox[3]==58 and bbox[0]>=4 and bbox[2]<=60 and 64-bbox[1]<=62,(name,row,col,bbox)
            bounds.append(bbox);hashes.add(hashlib.sha256(cel.tobytes()).hexdigest())
            bg=Image.new('RGBA',(64,64),(27,39,47,255));bg.alpha_composite(cel)
            gif.append(bg.resize((256,256),Image.Resampling.NEAREST).convert('RGB'))
        assert len(hashes)==4,(name,d,'duplicate frames')
        distinct.append(len(hashes))
        gif[0].save(folder/f'walk-{d}.gif',save_all=True,append_images=gif[1:],duration=100,loop=0)
    walk_contact.alpha_composite(atlas,(index*256,28))
    ImageDraw.Draw(walk_contact).text((index*256+8,8),name,fill='white')
    # Portraits preserve complete head/shoulder busts; only the generated ragged bottom padding is cropped.
    pf=PORTRAIT_ROOT/name;ps=pf/'generated-source.png'
    portrait_source=Image.open(ps).convert('RGBA')
    crop=portrait_source.crop((0,0,portrait_source.width,1208))
    bbox=crop.getchannel('A').point(lambda a:255 if a>=128 else 0).getbbox()
    crop=crop.crop(bbox)
    ratio=min(236/crop.width,236/crop.height)
    portrait=largest_silhouette(crop.resize((round(crop.width*ratio),round(crop.height*ratio)),Image.Resampling.NEAREST))
    canvas=Image.new('RGBA',(256,256));canvas.alpha_composite(portrait,((256-portrait.width)//2,256-10-portrait.height))
    target=OUTPUT/'Portraits'/f'{name}.png';canvas.save(target);write_meta(target)
    pd={'id':name,'image':os.path.relpath(target,pf).replace('\\','/'),'imageSha256':SHA(target),
        'sourceImage':'generated-source.png','sourceSha256':SHA(ps),'sourceSize':list(portrait_source.size),
        'finalSize':[256,256],'alpha':True,'technicalCrop':[0,0,portrait_source.width,1208],
        'wholeBustCrop':list(bbox),'uniformNearestNeighborScale':ratio,'status':'visual-reviewed',
        'visualApproval':False,'generation':'built-in ImageGen','prompt':'prompt.txt','identityReference':'full-sprite-reference.png'}
    if name=='Pedro':pd['priorityReference']='hat-reference.jpeg; round eyeglasses explicitly requested'
    (pf/'portrait.json').write_text(json.dumps(pd,ensure_ascii=False,indent=2),encoding='utf-8')
    portrait_contact.alpha_composite(canvas,(index*256,30))
    ImageDraw.Draw(portrait_contact).text((index*256+8,8),name,fill='white')
    summary[name]={'walk':str(runtime.relative_to(ROOT)),'portrait':str(target.relative_to(ROOT)),
        'walkVisibleBoundsPNG':bounds,'distinctPosesPerDirection':distinct,
        'maxHeight':max(b[3]-b[1]for b in bounds),'allFeetUnity6':all(64-b[3]==6 for b in bounds),
        'sourceToRuntimeRGBAEqual':Image.open(canonical).tobytes()==Image.open(runtime).tobytes()}
walk_contact.resize((2048,568),Image.Resampling.NEAREST).save(PREVIEW/'walks-contact-4.png')
portrait_contact.save(PREVIEW/'portraits-contact-4.png')
(PREVIEW/'validation.json').write_text(json.dumps(summary,ensure_ascii=False,indent=2),encoding='utf-8')
print(json.dumps({k:{f:v[f]for f in ['maxHeight','allFeetUnity6','distinctPosesPerDirection','sourceToRuntimeRGBAEqual']}for k,v in summary.items()}))
