"""Pack whole generated figures; preserve identity, GUIDs and importer contract."""
from pathlib import Path
import hashlib,json,math,shutil
from PIL import Image,ImageDraw,ImageFont

ROOT=Path(__file__).resolve().parents[1]
DELIVERY=ROOT/'ArtSources/StudentsCrisp20261029'
GENERATED=Path('C:/Users/Usuario/.codex/generated_images/01a1895f-9579-7540-b0ef-6c20c9ef69e9')
FILES={'Fabio':'exec-1bb697b2-6350-476a-918d-0ca8c326e582.png','Marcos':'exec-cc4d17cb-655d-4e7a-9a60-ad6ebac68559.png','Matias':'exec-ea8873dc-7556-41ff-8b3f-43a764230250.png','AnnaSabia':'exec-7e030c9b-3ab5-4fd3-9370-6c76d9510af6.png'}
LABELS={'Fabio':'Fábio','Marcos':'Marcos','Matias':'Matias','AnnaSabia':'Anna Sabiá'}
DIRECTIONS=['S','W','E','N']
contact=Image.new('RGB',(16*96,4*116),(21,34,40));draw=ImageDraw.Draw(contact)
stats={}
def boundaries(image,axis):
    alpha=image.getchannel('A').load();size=image.size[axis];other=image.size[1-axis]
    values=[sum((alpha[t,q] if axis==0 else alpha[q,t])>=128 for q in range(other)) for t in range(size)]
    spans=[];start=None
    for t,value in enumerate(values+[0]):
        if value>5 and start is None:start=t
        if value<=5 and start is not None:
            if t-start>10:
                if spans and start-spans[-1][1]<=8:spans[-1][1]=t
                else:spans.append([start,t])
            start=None
    if len(spans)!=4:raise ValueError(('Expected four separated figure bands',axis,spans))
    return [0]+[(spans[i-1][1]+spans[i][0])//2 for i in range(1,4)]+[size]

for index,(name,file) in enumerate(FILES.items()):
    folder=DELIVERY/name;folder.mkdir(parents=True,exist_ok=True)
    original=folder/'generated-source.png';shutil.copy2(GENERATED/file,original)
    image=Image.open(original).convert('RGBA')
    # A binary alpha edge avoids interpolation halos. Do not recolour RGB or
    # remove dark hair/outline pixels by their colour.
    image.putalpha(image.getchannel('A').point(lambda value:255 if value>=128 else 0))
    frames=[];source_rects=[];xs=boundaries(image,0);ys=boundaries(image,1)
    for row in range(4):
        for col in range(4):
            cell=(xs[col],ys[row],xs[col+1],ys[row+1])
            frame=image.crop(cell);box=frame.getbbox()
            if box is None:raise ValueError((name,row,col,'empty'))
            if box[0]<3 or box[1]<3 or box[2]>frame.width-3 or box[3]>frame.height-3:raise ValueError((name,row,col,'source clipped',box))
            frame=frame.crop(box)
            if frame.width>=384 or frame.height>=368:raise ValueError((name,'source too large'))
            frames.append(frame);source_rects.append([cell[0]+box[0],cell[1]+box[1],frame.width,frame.height])
    tallest=max(f.height for f in frames);widest=max(f.width for f in frames)
    height=44 if name=='AnnaSabia' else 50
    ratio=min(height/tallest,56/widest)
    source=Image.new('RGBA',(1536,1536));atlas=Image.new('RGBA',(256,256))
    for i,frame in enumerate(frames):
        source.alpha_composite(frame,((i%4)*384+(384-frame.width)//2,(i//4)*384+368-frame.height))
        width=max(1,round(frame.width*ratio));h=max(1,round(frame.height*ratio))
        small=Image.new('RGBA',(width,h))
        for y in range(h):
            sy=frame.height-1-min(frame.height-1,math.floor((h-1-y+.5)/ratio))
            for x in range(width):
                sx=min(frame.width-1,math.floor((x+.5)/ratio));small.putpixel((x,y),frame.getpixel((sx,sy)))
        box=small.getbbox();atlas.alpha_composite(small,((i%4)*64+(64-width)//2,(i//4)*64+58-box[3]))
    for category,art in [('ArtSource',source),('Resources/Varginha',atlas)]:
        path=ROOT/'Assets'/category/'Allies'/f'{name}.png'
        before=folder/f'before-{category.split("/")[0]}.png'
        if not before.exists():shutil.copy2(path,before)
        art.save(path)
    source.save(folder/'normalized-source.png');atlas.save(folder/'atlas.png')
    manifest={'schemaVersion':1,'image':'atlas.png','imageSha256':hashlib.sha256((folder/'atlas.png').read_bytes()).hexdigest(),'nativeSize':[256,256],'sourceSize':list(image.size),'normalizedSourceSize':[1536,1536],'sourceSha256':hashlib.sha256(original.read_bytes()).hexdigest(),'pixelsPerUnit':64/1.35,'status':'visual-reviewed','displayName':LABELS[name],'sourceRects':source_rects,'scale':ratio,'frames':[],'clips':[],'quality':{'alphaThreshold':127,'maxColors':65536,'maxHeightRatio':1.3}}
    for row,direction in enumerate(DIRECTIONS):
        ids=[]
        for col in range(4):
            id=f'walk_{direction}_{col:02}';ids.append(id)
            manifest['frames'].append({'id':id,'rect':[col*64,row*64,64,64],'pivot':[.5,.5],'footAnchor':[32,58],'durationMs':118,'action':'walk','direction':direction,'skin':'industrial','form':'normal'})
        manifest['clips'].append({'name':f'walk_{direction}','frames':ids,'loop':True,'allowStatic':False})
    manifest['requiredClips']=[c['name'] for c in manifest['clips']]
    (folder/'atlas.json').write_text(json.dumps(manifest,ensure_ascii=False,indent=2),encoding='utf8')
    geometries=[];distinct=[]
    for i in range(16):
        frame=atlas.crop((i%4*64,i//4*64,i%4*64+64,i//4*64+64));box=frame.getbbox()
        if box is None or box[3]!=58 or box[0]<4 or box[2]>60 or box[1]<2:geometries.append((i,box))
        distinct.append(frame.tobytes())
        zoom=frame.resize((96,96),Image.Resampling.NEAREST);contact.paste(zoom,(i*96,index*116+20),zoom)
    draw.text((2,index*116),LABELS[name],fill='white')
    stats[name]={'geometryErrors':geometries,'distinctFrames':len(set(distinct)),'visibleColors':len({p[:3] for p in atlas.getdata() if p[3]>0}),'sourceTallest':tallest,'sourceWidest':widest,'ratio':ratio}
    if geometries:raise ValueError((name,geometries))
contact.save(DELIVERY/'Contact_GroupA_64Frames.png')
(DELIVERY/'geometry-group-a.json').write_text(json.dumps(stats,ensure_ascii=False,indent=2),encoding='utf8')
print(json.dumps(stats))
