"""Pack complete ImageGen frames into the existing student atlas contract.

No recoloring, anatomical assembly, mirroring, or generated artwork is performed here.
"""
from pathlib import Path
from collections import deque
import hashlib, json, re, uuid, argparse
from PIL import Image

ROOT = Path(__file__).resolve().parents[1]
SETS = {
    'Bianca': [('final-skin-match-source.png', ['S','W','E','N'], [0,313,627,940,1254])],
    'Moscardini': [('SW-generated-source.png', ['S','W'], [0,512,1024]),
                   ('EN-generated-source.png', ['E','N'], [0,512,1024])]
}
sha = lambda p: hashlib.sha256(p.read_bytes()).hexdigest()

def complete_sprite(area):
    # The generated alpha is real, but its transparent padding contains faint
    # islands. Keep the entire connected silhouette by alpha, never by RGB color.
    solid = {(x,y) for y in range(area.height) for x in range(area.width)
             if area.getpixel((x,y))[3] >= 128}
    components=[]
    while solid:
        seed=solid.pop(); q=deque([seed]); component={seed}
        while q:
            x,y=q.popleft()
            for dx,dy in ((-1,-1),(-1,0),(-1,1),(0,-1),(0,1),(1,-1),(1,0),(1,1)):
                p=(x+dx,y+dy)
                if p in solid:
                    solid.remove(p); q.append(p); component.add(p)
        components.append(component)
    body=max(components,key=len)
    x0=min(x for x,y in body); y0=min(y for x,y in body)
    x1=max(x for x,y in body)+1; y1=max(y for x,y in body)+1
    result=Image.new('RGBA',(x1-x0,y1-y0))
    for x,y in body: result.putpixel((x-x0,y-y0),area.getpixel((x,y)))
    return result,(x0,y0,x1,y1),len(components)-1

parser=argparse.ArgumentParser(description=__doc__)
parser.add_argument('--only', choices=list(SETS))
args=parser.parse_args()
for student,batches in SETS.items():
    if args.only and args.only!=student: continue
    folder=ROOT/'Assets/ArtSource/Allies/Redesign20261029'/student
    preview=ROOT/'Preview/AlliesRedesign20261029'/student
    folder.mkdir(parents=True,exist_ok=True); preview.mkdir(parents=True,exist_ok=True)
    # Every input lives in the repository. Regeneration requires neither Downloads
    # nor the original ImageGen cache and never calls the image service.
    assert (folder/'design-reference.jpeg').is_file(), 'Preserved design reference missing'
    if student=='Bianca': assert (folder/'marcos-skin-reference.jpeg').is_file()
    atlas=Image.new('RGBA',(256,256)); frames=[]; sources=[]
    for source_name,directions,ys in batches:
        target=folder/source_name
        source=Image.open(target).convert('RGBA'); sprites=[]
        for row,direction in enumerate(directions):
            for col in range(4):
                x0=col*source.width//4; x1=(col+1)*source.width//4
                sprite,box,islands=complete_sprite(source.crop((x0,ys[row],x1,ys[row+1])))
                sprites.append((direction,col,sprite,[x0+box[0],ys[row]+box[1],box[2]-box[0],box[3]-box[1]],islands))
        # Each generation used a different source canvas; one uniform ratio per
        # batch adapts source resolution while preserving gait height differences.
        ratio=min(50/max(s[2].height for s in sprites),56/max(s[2].width for s in sprites))
        sources.append({'image':target.name,'sha256':sha(target),'size':list(source.size),'uniformScale':ratio})
        for direction,col,sprite,rect,islands in sprites:
            row='SWEN'.index(direction)
            small=sprite.resize((round(sprite.width*ratio),round(sprite.height*ratio)),Image.Resampling.NEAREST)
            box=small.getchannel('A').getbbox()
            # Bounding transparency can shift by one sample during downscaling.
            small=small.crop(box)
            atlas.alpha_composite(small,(col*64+(64-small.width)//2,row*64+58-small.height))
            frames.append({'id':f'walk_{direction}_{col:02}','rect':[col*64,row*64,64,64],
                'pivot':[.5,.5],'footBaselineFromTop':58,'durationMs':100,'action':'walk',
                'direction':direction,'skin':'marcos-tone' if student=='Bianca' else 'reference',
                'form':'normal','sourceImage':target.name,'sourceRect':rect,'paddingIslandsRemoved':islands})
    frames.sort(key=lambda f:('SWEN'.index(f['direction']),f['id']))
    output=ROOT/'Assets/Resources/Varginha/Allies'/f'{student}.png'; atlas.save(output)
    # Canonical importer-compatible source: integer 2x copy of final pixels.
    canonical=ROOT/'Assets/ArtSource/Allies'/f'{student}.png'
    atlas.resize((512,512),Image.Resampling.NEAREST).save(canonical)
    for asset in (output,canonical):
        meta=asset.with_suffix('.png.meta')
        if not meta.exists():
            template=(output.parent/'Marcos.png.meta').read_text(encoding='utf-8')
            meta.write_text(re.sub(r'guid: [0-9a-f]+',f'guid: {uuid.uuid4().hex}',template,count=1),encoding='utf-8')
    manifest={'schemaVersion':1,'image':'../../../../Resources/Varginha/Allies/'+student+'.png',
        'imageSha256':sha(output),'nativeSize':[256,256],'pixelsPerUnit':64/1.35,
        'status':'visual-reviewed','visualApproval':False,'sources':sources,
        'referenceHashes':{p.name:sha(p) for p in folder.glob('*.jpeg')},
        'rectOrigin':'top-left','frames':frames,
        'clips':[{'name':f'walk_{d}','frames':[f'walk_{d}_{i:02}' for i in range(4)],'loop':True,'allowStatic':False} for d in 'SWEN'],
        'requiredClips':[f'walk_{d}' for d in 'SWEN'],
        'quality':{'alphaThreshold':128,'maxColors':4096,'maxHeightRatio':1.3},
        'notes':['Built-in ImageGen drawing based on supplied complete character designs.',
            'Bianca final skin-only ImageGen edit uses actual runtime Marcos atlas as visual skin-tone reference; generative color match, not a measured exact shared palette.' if student=='Bianca' else 'Moscardini preserves supplied hairstyle, school uniform and trousers.',
            'Only complete frames extracted; no anatomical assembly, mirrors, rotations or palette recoloring.',
            'Alpha threshold128 removes faint disconnected transparent-padding islands; no material color keyed.',
            'Uniform whole-frame nearest-neighbor scale per generated source batch, final maximum height50px, feet58.',
            'Canonical 512px source is an integer2x copy to preserve existing student importer workflow.',
            'Four different gait drawings per direction; runtime Unity verification belongs to integration task.']}
    (folder/'atlas.json').write_text(json.dumps(manifest,ensure_ascii=False,indent=2),encoding='utf-8')
    atlas.resize((768,768),Image.Resampling.NEAREST).save(preview/'atlas-contact-3x.png')
    sheet=Image.new('RGB',(256*3,256*3),(229,229,216)); sheet.paste(atlas.resize((768,768),Image.Resampling.NEAREST),mask=atlas.getchannel('A').resize((768,768),Image.Resampling.NEAREST)); sheet.save(preview/'atlas-light-3x.png')
    print(json.dumps({'student':student,'output':str(output),'frames':len(frames),'size':[256,256],'maxHeight':50,'baseline':58}))
