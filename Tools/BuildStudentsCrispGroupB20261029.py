"""Pack complete generated walk figures for Ana Tavares and Luis Miguel Messias."""
from pathlib import Path
from collections import deque
from PIL import Image
import hashlib,json
ROOT=Path(__file__).resolve().parents[1]
SPEC={'AnaTavares':(56,[0,312,609,907,1240]),'LuisMiguelMessias':(50,[0,320,628,936,1246])}
sha=lambda p:hashlib.sha256(p.read_bytes()).hexdigest()
for name,(target_height,ys)in SPEC.items():
    folder=ROOT/'ArtSources/StudentsCrisp20261029'/name
    source=Image.open(folder/'generated-walk.png').convert('RGBA')
    sprites=[]
    for row in range(4):
        for col in range(4):
            x0=col*source.width//4;x1=(col+1)*source.width//4
            area=source.crop((x0,ys[row],x1,ys[row+1]))
            solid={(x,y)for y in range(area.height)for x in range(area.width)if area.getpixel((x,y))[3]>=128}
            components=[]
            while solid:
                seed=solid.pop();q=deque([seed]);body={seed}
                while q:
                    x,y=q.popleft()
                    for dx,dy in ((-1,-1),(-1,0),(-1,1),(0,-1),(0,1),(1,-1),(1,0),(1,1)):
                        p=(x+dx,y+dy)
                        if p in solid:solid.remove(p);q.append(p);body.add(p)
                components.append(body)
            body=max(components,key=len)
            bx0=min(x for x,y in body);by0=min(y for x,y in body)
            bx1=max(x for x,y in body)+1;by1=max(y for x,y in body)+1
            sprite=Image.new('RGBA',(bx1-bx0,by1-by0))
            for x,y in body:sprite.putpixel((x-bx0,y-by0),area.getpixel((x,y)))
            sprites.append((sprite,[x0+bx0,ys[row]+by0,bx1-bx0,by1-by0],len(components)-1))
    scale=min(target_height/max(s[0].height for s in sprites),56/max(s[0].width for s in sprites))
    atlas=Image.new('RGBA',(256,256));frames=[]
    for index,(sprite,rect,islands)in enumerate(sprites):
        row,col=divmod(index,4)
        small=sprite.resize((round(sprite.width*scale),round(sprite.height*scale)),Image.Resampling.NEAREST)
        small=small.crop(small.getchannel('A').getbbox())
        atlas.alpha_composite(small,(col*64+(64-small.width)//2,row*64+58-small.height))
        frames.append({'id':f'walk_{"SWEN"[row]}_{col:02}','rect':[col*64,row*64,64,64],
            'sourceRect':rect,'sourceImage':'generated-walk.png','pivot':[.5,.5],
            'action':'walk','direction':'SWEN'[row],'durationMs':100,'form':'normal','skin':'uniform',
            'paddingIslandsRemoved':islands,'footBaselineFromTop':58})
    runtime=ROOT/'Assets/Resources/Varginha/Allies'/(name+'.png')
    atlas.save(runtime)
    atlas.resize((512,512),Image.Resampling.NEAREST).save(ROOT/'Assets/ArtSource/Allies'/(name+'.png'))
    # Existing .meta GUIDs remain untouched.
    manifest={'schemaVersion':1,'image':'../../../Assets/Resources/Varginha/Allies/'+name+'.png',
        'imageSha256':sha(runtime),'nativeSize':[256,256],'sourceSize':list(source.size),
        'sourceImage':'generated-walk.png','sourceSha256':sha(folder/'generated-walk.png'),
        'pixelsPerUnit':64/1.35,'status':'visual-reviewed','visualApproval':False,'uniformNearestScale':scale,
        'frames':frames,'clips':[{'name':f'walk_{d}','frames':[f'walk_{d}_{i:02}'for i in range(4)],'loop':True,'allowStatic':False}for d in 'SWEN'],
        'requiredClips':[f'walk_{d}'for d in 'SWEN'],
        'quality':{'alphaThreshold':128,'maxColors':4096,'maxHeightRatio':1.3},
        'notes':['Built-in ImageGen whole-figure drawings in crisp Bianca/Moscardini style.',
            'Ana Tavares photo is primary identity reference: long highlighted curls, open forehead, school yellow sleeves retained.'if name=='AnaTavares'else'Original student design and short beard retained.',
            'Whole figures only; no anatomical assembly, mirroring, deformation or procedural recoloring.',
            'Canonical source is integer2x copy for existing importer; final foot baseline58=Unity6.',
            'ImageGen adaptation faithful to identity, not exact pixel copy. Native runtime verification pending parent integration.']}
    (folder/'atlas.json').write_text(json.dumps(manifest,ensure_ascii=False,indent=2),encoding='utf8')
    preview=ROOT/'Preview/StudentsCrisp20261029'/name;preview.mkdir(parents=True,exist_ok=True)
    bg=Image.new('RGBA',(256,256),(233,231,216,255));bg.alpha_composite(atlas)
    bg.resize((768,768),Image.Resampling.NEAREST).save(preview/'contact-3x.png')
    print(json.dumps({'name':name,'frames':16,'sourceSize':source.size,'maxHeight':target_height,'footBaseline':58,
        'sourceTopMin':min(s[1][1] for s in sprites[:4]),'atlasSha256':sha(runtime)}))
