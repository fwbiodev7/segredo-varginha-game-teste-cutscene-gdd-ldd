"""Technical whole-portrait extraction; never redraws, recolors or assembles anatomy."""
from pathlib import Path
from collections import deque
import hashlib,json,re,uuid
from PIL import Image,ImageDraw
ROOT=Path(__file__).resolve().parents[1]
NAMES=['Bianca','Moscardini','AnaTavares','LuisMiguelMessias']
sha=lambda p:hashlib.sha256(p.read_bytes()).hexdigest()
preview=ROOT/'Preview/StudentPortraits20261029/GroupB'
preview.mkdir(parents=True,exist_ok=True)
out=ROOT/'Assets/Resources/Varginha/Allies/Portraits'
out.mkdir(parents=True,exist_ok=True)
contact=Image.new('RGBA',(512,556),(231,229,217,255));draw=ImageDraw.Draw(contact)
for index,name in enumerate(NAMES):
    source=ROOT/'ArtSources/StudentPortraits20261029'/name
    image=Image.open(source/'generated-portrait.png').convert('RGBA')
    # Alpha padding can contain tiny detached generation islands. Find the complete
    # connected bust, not RGB material colors or isolated anatomical parts.
    solid={(x,y)for y in range(image.height)for x in range(image.width)if image.getpixel((x,y))[3]>=128}
    components=[]
    while solid:
        seed=solid.pop();queue=deque([seed]);body={seed}
        while queue:
            x,y=queue.popleft()
            for dx,dy in ((-1,-1),(-1,0),(-1,1),(0,-1),(0,1),(1,-1),(1,0),(1,1)):
                p=(x+dx,y+dy)
                if p in solid:solid.remove(p);body.add(p);queue.append(p)
        components.append(body)
    body=max(components,key=len)
    x0=min(x for x,y in body);x1=max(x for x,y in body)+1
    y0=min(y for x,y in body);y1=max(y for x,y in body)+1
    crop=Image.new('RGBA',(x1-x0,y1-y0))
    for x,y in body:crop.putpixel((x-x0,y-y0),image.getpixel((x,y)))
    scale=240/max(crop.size)
    reduced=crop.resize((round(crop.width*scale),round(crop.height*scale)),Image.Resampling.NEAREST)
    final=Image.new('RGBA',(256,256))
    final.alpha_composite(reduced,((256-reduced.width)//2,(256-reduced.height)//2))
    file=out/(name+'.png');final.save(file)
    meta=file.with_suffix('.png.meta')
    if not meta.exists():
        template=(ROOT/'Assets/Resources/Varginha/Allies/Marcos.png.meta').read_text(encoding='utf-8')
        template=re.sub(r'guid: [0-9a-f]+','guid: '+uuid.uuid4().hex,template,count=1)
        template=re.sub(r'textureCompression: \d+','textureCompression: 0',template)
        template=re.sub(r'spriteMode: \d+','spriteMode: 0',template)
        meta.write_text(template,encoding='utf-8')
    manifest={'schemaVersion':1,'character':name,'mode':'built-in image_gen','status':'visual-reviewed','visualApproval':False,
        'image':str(file.relative_to(ROOT)).replace('\\','/'),'imageSha256':sha(file),'nativeSize':[256,256],
        'sourceImage':'generated-portrait.png','sourceSha256':sha(source/'generated-portrait.png'),'sourceSize':list(image.size),
        'sourceRect':[x0,y0,x1-x0,y1-y0],'sourceRectOrigin':'top-left','uniformNearestScale':scale,
        'completeBust':True,'removedPaddingComponents':len(components)-1,'alphaThreshold':128,
        'notes':['Complete bust extracted and uniformly reduced; no anatomical assembly or palette recoloring.',
            'Alpha-connected silhouette removes only detached transparent-margin generation noise.',
            'Current supplied photo is Ana Tavares identity authority, school uniform retained.'if name=='AnaTavares'else'Current supplied design identity retained.',
            'Generative adaptation; exact pixel equality with source designs is not claimed. Unity runtime pending parent integration.']}
    (source/'portrait.json').write_text(json.dumps(manifest,ensure_ascii=False,indent=2),encoding='utf-8')
    x=(index%2)*256;y=(index//2)*278
    draw.text((x+6,y+4),name,fill=(25,35,37,255));contact.alpha_composite(final,(x,y+22))
    print(json.dumps({'name':name,'size':final.size,'mode':final.mode,'alpha':final.getchannel('A').getextrema(),'sha256':sha(file)}))
contact.save(preview/'contact-4.png');contact.resize((1024,1112),Image.Resampling.NEAREST).save(preview/'contact-4-2x.png')
