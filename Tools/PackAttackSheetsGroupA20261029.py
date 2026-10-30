"""Pack already-generated whole attack figures and six existing generated effects.
No artwork generation, drawing, mirroring, anatomy assembly or code changes.
"""
from pathlib import Path
from collections import deque
from statistics import median
import hashlib,json,math,shutil
from PIL import Image,ImageDraw,ImageFont

ROOT=Path(__file__).resolve().parents[1]
DELIVERY=ROOT/'ArtSources/AttackSheets20261029'
GENERATED=Path('C:/Users/Usuario/.codex/generated_images/01a1895f-9579-7540-b0ef-6c20c9ef69e9')
RUNTIME=ROOT/'Assets/Resources/Varginha/StoryCharacters'
BODY={'Fabio':'exec-fb04f535-bfc1-4581-bec7-56562955c41a.png','Marcos':'exec-cdbc80ef-6cab-4df6-9055-e14bd2354b37.png','Matias':'exec-9b537e25-7546-4cdc-bf65-565c79cafd14.png','AnnaSabia':'exec-e825178c-b41f-474b-a027-ff81a6f7e0cb.png'}
FX={'Fabio':'exec-dc872a64-0dd4-4bc0-a974-c279bb14d1e7.png','Marcos':'exec-c53dffc4-2007-43a1-834c-a7f681811a8d.png','Matias':'exec-6156d73d-8792-4467-9d3f-c53c1f4fd01c.png','AnnaSabia':'exec-9026de4e-33b1-441c-ab9c-622da7e6ad98.png'}
LABEL={'Fabio':'Fábio','Marcos':'Marcos','Matias':'Matias','AnnaSabia':'Anna Sabiá'}
ACTION={'Fabio':['halter-arremesso','halter-overhead'],'Marcos':['grito-comico','volei'],'Matias':['jiu-jitsu-combo','jiu-jitsu-projecao'],'AnnaSabia':['pingpong-forehand','pingpong-spin']}
PPU=128/1.55
BASELINE=112
LIFT=.58-(128-BASELINE)/PPU
STAGES=['anticipation','contact','recovery']
sha=lambda p:hashlib.sha256(p.read_bytes()).hexdigest()

def bounds(points):
    return min(x for x,y in points),min(y for x,y in points),max(x for x,y in points)+1,max(y for x,y in points)+1

def components(image):
    width,height=image.size;pixels=image.load()
    remaining={(x,y)for y in range(height)for x in range(width)if pixels[x,y][3]>=128}
    found=[]
    while remaining:
        seed=remaining.pop();queue=deque([seed]);part={seed}
        while queue:
            x,y=queue.popleft()
            for dx,dy in ((-1,-1),(-1,0),(-1,1),(0,-1),(0,1),(1,-1),(1,0),(1,1)):
                point=x+dx,y+dy
                if point in remaining:remaining.remove(point);queue.append(point);part.add(point)
        found.append(part)
    return sorted(found,key=len,reverse=True)

def extract_row(image,row):
    y0=row*image.height//2;y1=(row+1)*image.height//2
    strip=image.crop((0,y0,image.width,y1));parts=components(strip)
    bodies=sorted(parts[:6],key=lambda p:sum(bounds(p)[::2])/2)
    if len(bodies)!=6 or min(len(p)for p in bodies)<2000:raise ValueError(('Expected six whole figures',row))
    centers=[(bounds(p)[0]+bounds(p)[2])/2 for p in bodies]
    groups=[set(p)for p in bodies]
    foot_anchors=[]
    body_bottom=[]
    for body in bodies:
        x0,top,x1,bottom=bounds(body)
        feet=[x for x,y in body if y>=bottom-max(8,(bottom-top)*.13)]
        foot_anchors.append((min(feet)+max(feet))/2)
        body_bottom.append(bottom)
    seeds={id(p)for p in bodies}
    for part in parts:
        if id(part)in seeds:continue
        x0,top,x1,bottom=bounds(part);cx=(x0+x1)/2
        target=min(range(6),key=lambda i:abs(cx-centers[i]))
        # Detached release objects belong to the contact pose, not the adjacent
        # recovery figure; grouping whole separated figures never cuts anatomy.
        if centers[1]<cx<centers[2]-15:target=1
        elif centers[3]+15<cx<centers[4]:target=4
        if len(part)>1:groups[target].update(part)
    result=[]
    baseline=max(body_bottom)
    source_pixels=strip.load()
    for col,group in enumerate(groups):
        box=bounds(group);x0,top,x1,bottom=box
        frame=Image.new('RGBA',(x1-x0,bottom-top))
        for x,y in group:
            r,g,b,a=source_pixels[x,y];frame.putpixel((x-x0,y-top),(r,g,b,255))
        result.append({'image':frame,'sourceRect':[x0,top+y0,x1-x0,bottom-top],'anchor':foot_anchors[col]-x0,'ground':baseline-top,'bodyHeight':body_bottom[col]-bounds(bodies[col])[1]})
    return result

def nearest(image,scale):
    return image.resize((max(1,round(image.width*scale)),max(1,round(image.height*scale))),Image.Resampling.NEAREST)

summaries={}
contact=Image.new('RGB',(12*128,4*170),(18,32,40));draw=ImageDraw.Draw(contact)
effects_contact=Image.new('RGB',(6*128,4*170),(18,32,40));fxdraw=ImageDraw.Draw(effects_contact)
for group_index,name in enumerate(BODY):
    folder=DELIVERY/name;folder.mkdir(parents=True,exist_ok=True)
    src=folder/'generated-attacks.png';shutil.copy2(GENERATED/BODY[name],src)
    canonical=ROOT/'Assets/Resources/Varginha/Allies/Portraits'/f'{name}.png'
    shutil.copy2(canonical,folder/'canonical-portrait.png')
    image=Image.open(src).convert('RGBA')
    frames=extract_row(image,0)+extract_row(image,1)
    scale=min(88/frames[0]['bodyHeight'],112/max(f['image'].width for f in frames),104/max(f['ground']for f in frames))
    atlas=Image.new('RGBA',(768,256));manifest={'width':768,'height':256,'columns':6,'rows':2,'pixelsPerUnit':[PPU,PPU],'frames':[]}
    unique=[];geometry=[];preview_frames=[]
    for i,item in enumerate(frames):
        row,col=divmod(i,6);small=nearest(item['image'],scale)
        # Preserve original airborne heights relative to each source-row ground.
        y=BASELINE-round(item['ground']*scale);x=(128-small.width)//2
        if x<4 or y<4 or y+small.height>124:raise ValueError((name,i,'padding',x,y,small.size))
        atlas.alpha_composite(small,(col*128+x,row*128+y))
        frame=atlas.crop((col*128,row*128,col*128+128,row*128+128))
        anchor=x+item['anchor']*scale
        record={'rect':[col*128,row*128,128,128],'anchorX':col*128+anchor,'lift':LIFT,'action':row,'direction':'east'if col<3 else'west','stage':STAGES[col%3],'sha256':hashlib.sha256(frame.tobytes()).hexdigest()}
        manifest['frames'].append(record);unique.append(frame.tobytes());preview_frames.append(frame)
        box=frame.getbbox();geometry.append(list(box))
        contact.paste(frame,(i*128,group_index*170+28),frame)
    runtime=RUNTIME/f'Hero{name}.png';data=runtime.with_suffix('.json')
    for path in (runtime,data):
        previous=folder/('before-'+path.name)
        if path.exists()and not previous.exists():shutil.copy2(path,previous)
    atlas.save(runtime);atlas.save(folder/'atlas.png');data.write_text(json.dumps(manifest,ensure_ascii=False,indent=2),encoding='utf8')
    (folder/'runtime.json').write_text(json.dumps(manifest,ensure_ascii=False,indent=2),encoding='utf8')
    # Six already-generated detached effect motifs: 3 columns x 2 variants.
    fxsource=folder/'generated-effects.png';shutil.copy2(GENERATED/FX[name],fxsource)
    effect_image=Image.open(fxsource).convert('RGBA');effect_atlas=Image.new('RGBA',(384,256));effects=[]
    for row in range(2):
        for col in range(3):
            rect=(col*effect_image.width//3,row*effect_image.height//2,(col+1)*effect_image.width//3,(row+1)*effect_image.height//2)
            whole=effect_image.crop(rect);whole.putalpha(whole.getchannel('A').point(lambda a:255 if a>=128 else 0));box=whole.getbbox()
            if not box:raise ValueError((name,'empty effect',row,col))
            whole=whole.crop(box);ratio=min(108/whole.width,108/whole.height);small=nearest(whole,ratio)
            x=(128-small.width)//2;y=(128-small.height)//2
            effect_atlas.alpha_composite(small,(col*128+x,row*128+y))
            cell=effect_atlas.crop((col*128,row*128,col*128+128,row*128+128))
            effects_contact.paste(cell,((row*3+col)*128,group_index*170+28),cell)
            effects.append({'rect':[col*128,row*128,128,128],'action':row,'stage':['projectile','impact','dissipate'][col],'sourceRect':[rect[0]+box[0],rect[1]+box[1],whole.width,whole.height],'sha256':hashlib.sha256(cell.tobytes()).hexdigest()})
    fxruntime=RUNTIME/f'Hero{name}Effects.png';previous=folder/('before-'+fxruntime.name)
    if fxruntime.exists()and not previous.exists():shutil.copy2(fxruntime,previous)
    effect_atlas.save(fxruntime);effect_atlas.save(folder/'effects.png')
    provenance={'student':LABEL[name],'assetId':name,'generator':'built-in image_gen','sourceSize':list(image.size),'sourceSha256':sha(src),'sourceEffectsSha256':sha(fxsource),'portraitSha256':sha(canonical),'runtimeSha256':sha(runtime),'runtimeEffectSha256':sha(fxruntime),'runtimeSize':[768,256],'effectsSize':[384,256],'scale':scale,'sourceFrames':[{k:v for k,v in f.items()if k!='image'}for f in frames],'effects':effects,'geometry':geometry,'distinctAttackFrames':len(set(unique)),'status':'visual-reviewed','runtimeVerified':False,'notes':['No additional generation after user budget instruction; all sources already complete.','Whole figures with their separate attached/released props grouped technically; no anatomical assembly, mirroring or recolouring.','Alpha mask threshold128; RGB art preserved, uniform whole-sprite nearest scaling.','Ground pivot is16px above cell bottom. lift=.58-16/PPU, anchor follows actor foot centre.','Runtime .meta GUIDs preserved.']}
    (folder/'provenance.json').write_text(json.dumps(provenance,ensure_ascii=False,indent=2),encoding='utf8')
    # Short authored 3-phase clips for both directions and variants.
    for row in range(2):
        for direction,start in [('east',0),('west',3)]:
            anim=[]
            for index in range(row*6+start,row*6+start+3):
                frame=Image.new('RGBA',(128,128),(26,40,48,255));frame.alpha_composite(preview_frames[index]);anim.append(frame.convert('RGB').resize((256,256),Image.Resampling.NEAREST))
            anim[0].save(folder/f'attack-{row}-{direction}.gif',save_all=True,append_images=anim[1:],duration=[150,110,180],loop=0)
    draw.text((4,group_index*170+6),LABEL[name]+' | '+', '.join(ACTION[name]),fill='white')
    fxdraw.text((4,group_index*170+6),LABEL[name]+' | projétil / impacto / final',fill='white')
    summaries[name]={'runtime':str(runtime.relative_to(ROOT)),'json':str(data.relative_to(ROOT)),'effects':str(fxruntime.relative_to(ROOT)),'frames':12,'distinctFrames':len(set(unique)),'effectsFrames':6,'PPU':PPU,'scale':scale,'alpha':[0,255],'geometryErrors':[]}
contact.save(DELIVERY/'Contact_GroupA_Attacks.png');effects_contact.save(DELIVERY/'Contact_GroupA_Effects.png')
(DELIVERY/'QA_GroupA.json').write_text(json.dumps(summaries,ensure_ascii=False,indent=2),encoding='utf8')
print(json.dumps(summaries,ensure_ascii=False))
