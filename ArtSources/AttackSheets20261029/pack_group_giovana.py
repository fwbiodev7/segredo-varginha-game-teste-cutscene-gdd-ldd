"""Pack complete authored figures/effects, without drawing or assembling anatomy."""
from pathlib import Path
from PIL import Image,ImageDraw
from collections import deque
import json,hashlib,os,re,uuid

ROOT=Path(__file__).resolve().parents[2]
BASE=Path(__file__).parent
OUT=ROOT/'Assets/Resources/Varginha/StoryCharacters'
IDS=['Pedro','Yasmin','LuisMartins','Giovana']
SHA=lambda p:hashlib.sha256(p.read_bytes()).hexdigest()
PPU=104/1.2

def meta(path,template):
    dest=path.with_suffix(path.suffix+'.meta')
    if dest.exists():return
    if path.suffix=='.json':
        text='fileFormatVersion: 2\nguid: '+uuid.uuid4().hex+'\nTextScriptImporter:\n  externalObjects: {}\n  userData: \n  assetBundleName: \n  assetBundleVariant: \n'
    else:
        text=re.sub(r'guid: [0-9a-f]+','guid: '+uuid.uuid4().hex,template.read_text(encoding='utf-8'),count=1)
    dest.write_text(text,encoding='utf-8')

def clean_padding(im):
    # Retain the complete figure and substantial detached solid instrument parts.
    im=im.copy();visible={(x,y)for y in range(im.height)for x in range(im.width)if im.getpixel((x,y))[3]>=128}
    keep=set()
    while visible:
        p=visible.pop();component={p};q=deque([p])
        while q:
            x,y=q.popleft()
            for dx,dy in [(0,1),(1,0),(0,-1),(-1,0),(-1,-1),(-1,1),(1,-1),(1,1)]:
                p=(x+dx,y+dy)
                if p in visible:visible.remove(p);component.add(p);q.append(p)
        if len(component)>=4:keep.update(component)
    for y in range(im.height):
        for x in range(im.width):
            if (x,y)not in keep:im.putpixel((x,y),(0,0,0,0))
    return im

contact=Image.new('RGBA',(1536,552),(27,39,47,255));summary={}
for k,name in enumerate(IDS):
    folder=BASE/name;src=folder/'generated-source.png'
    source=Image.open(src).convert('RGBA');crops=[];source_rects=[];head_centres=[]
    for row in range(2):
        for col in range(6):
            x0=col*source.width//6;y0=row*source.height//2
            area=source.crop((x0,y0,(col+1)*source.width//6,(row+1)*source.height//2))
            box=area.getchannel('A').point(lambda a:255 if a>=128 else 0).getbbox()
            im=area.crop(box);crops.append(im)
            source_rects.append([x0+box[0],y0+box[1],box[2]-box[0],box[3]-box[1]])
    holds=[]
    if name=='Pedro':
        # Generated contact cells4/10 face east. Reuse complete valid west poses, never mirror anatomy.
        for target,replacement in [(4,5),(10,11)]:
            crops[target]=crops[replacement].copy();source_rects[target]=source_rects[replacement][:]
            holds.append({'target':target,'source':replacement,'reason':'Generated contact faced east; valid west recovery pose held for contact.'})
    for im in crops:
        band=im.getchannel('A').crop((0,0,im.width,max(1,round(im.height*.25)))).point(lambda a:255 if a>=128 else 0).getbbox()
        head_centres.append((band[0]+band[2])/2)
    scale=min(104/max(im.height for im in crops),min(59/max(h,im.width-h)for h,im in zip(head_centres,crops)))
    atlas=Image.new('RGBA',(768,256));frames=[];audit_frames=[]
    for i,(im,h)in enumerate(zip(crops,head_centres)):
        row,col=divmod(i,6)
        reduced=clean_padding(im.resize((max(1,round(im.width*scale)),max(1,round(im.height*scale))),Image.Resampling.NEAREST))
        solid=reduced.getchannel('A').getbbox();left=64-round(h*scale)
        top=116-solid[3]
        assert left>=0 and left+reduced.width<=128 and top>=0,(name,i,left,top,reduced.size)
        atlas.alpha_composite(reduced,(col*128+left,row*128+top))
        cell=atlas.crop((col*128,row*128,col*128+128,row*128+128))
        stage=['anticipation','contact','recovery'][col%3];direction='east'if col<3 else'west'
        f={'rect':[col*128,row*128,128,128],'anchorX':col*128+64,'lift':-12/PPU,
           'action':row,'direction':direction,'stage':stage,'sha256':hashlib.sha256(cell.tobytes()).hexdigest(),
           'groundSupportBaselineUnity':12,'sourceRect':source_rects[i]}
        if name=='Pedro'and i in [4,10]:f['intentionalHold']='Valid west recovery pose reused at contact; dedicated west strum remains missing.'
        frames.append(f)
        audit_frames.append({'id':f'attack_{row}_{direction}_{stage}','rect':f['rect'],'pivot':[.5,(.58*PPU+12)/128],
            'durationMs':[160,130,200][col%3],'action':f'attack_{row}','direction':direction,'skin':'default','form':'normal'})
    png=OUT/f'Hero{name}.png';atlas.save(png)
    data={'width':768,'height':256,'columns':6,'rows':2,'pixelsPerUnit':[PPU,PPU],'frames':frames,
          'source':'ArtSources/AttackSheets20261029/'+name+'/generated-source.png','sourceSha256':SHA(src),
          'canonicalPortraitSha256':SHA(folder/'canonical-portrait.png'),'technicalUniformScale':scale,
          'origin':'top-left','intentionalHolds':holds,'runtimeVerified':False,
          'notes':['Complete authored figure+instrument per frame. Effects separate. Baseline12 and lift=-12/PPU give world ground offset-.58 in CampaignStorySprites.']}
    if name=='Yasmin':data['notes'].append('Curly long hair and blue-green eyes from definitive portrait; real piano/keyboard in both variants. No microphone.')
    js=OUT/f'Hero{name}.json';js.write_text(json.dumps(data,ensure_ascii=False,indent=2),encoding='utf-8')
    meta(png,OUT/'HeroPedro.png.meta');meta(js,OUT/'HeroPedro.png.meta')
    clips=[]
    for row in range(2):
        for d in ['east','west']:
            clips.append({'name':f'attack_{row}_{d}','frames':[f'attack_{row}_{d}_{s}'for s in ['anticipation','contact','recovery']],
                'loop':False,'allowStatic':name=='Pedro'and d=='west'})
    am={'schemaVersion':1,'image':os.path.relpath(png,folder).replace('\\','/'),'imageSha256':SHA(png),'nativeSize':[768,256],
        'pixelsPerUnit':PPU,'status':'visual-reviewed','visualApproval':False,'frames':audit_frames,'clips':clips,
        'requiredClips':[c['name']for c in clips],'quality':{'alphaThreshold':128,'maxColors':32768,'maxHeightRatio':1.5}}
    (folder/'audit-manifest.json').write_text(json.dumps(am,ensure_ascii=False,indent=2),encoding='utf-8')
    atlas.resize((1536,512),Image.Resampling.NEAREST).save(folder/'attack-contact-2x.png')
    for row in range(2):
        for d,start in [('east',0),('west',3)]:
            gif=[]
            for col in range(start,start+3):
                bg=Image.new('RGBA',(128,128),(27,39,47,255));bg.alpha_composite(atlas.crop((col*128,row*128,col*128+128,row*128+128)))
                gif.append(bg.resize((256,256),Image.Resampling.NEAREST).convert('RGB'))
            gif[0].save(folder/f'attack-{row}-{d}.gif',save_all=True,append_images=gif[1:],duration=[160,130,200],loop=0)
    cx=k%2*768;cy=k//2*276
    contact.alpha_composite(atlas,(cx,cy+20));ImageDraw.Draw(contact).text((cx+8,cy+4),name,fill='white')
    summary[name]={'png':str(png.relative_to(ROOT)),'json':str(js.relative_to(ROOT)),'frames':12,'intentionalHolds':holds,
        'effects':'existing256x256 reused; runtime2-column adaptation needed'if name!='Giovana'else'new384x256'}

# Only Giovana receives a new effects sheet. Other generated effects are deliberately not installed.
ef=BASE/'Giovana/generated-effects-source.png';eim=Image.open(ef).convert('RGBA');effects=Image.new('RGBA',(384,256));erects=[]
ys=[0,560,1024]
for row in range(2):
    for col in range(3):
        cell=eim.crop((col*512,ys[row],(col+1)*512,ys[row+1]))
        # Remove only near-transparent generation padding; retain real translucent barrier and glints.
        cell.putalpha(cell.getchannel('A').point(lambda a:0 if a<=8 else a))
        bbox=cell.getchannel('A').getbbox();cell=cell.crop(bbox)
        ratio=112/max(cell.size);cell=cell.resize((round(cell.width*ratio),round(cell.height*ratio)),Image.Resampling.NEAREST)
        effects.alpha_composite(cell,(col*128+(128-cell.width)//2,row*128+(128-cell.height)//2))
        erects.append({'variant':row,'stage':['projectile','impact','final'][col],'rect':[col*128,row*128,128,128],
            'sourceRect':[col*512+bbox[0],ys[row]+bbox[1],bbox[2]-bbox[0],bbox[3]-bbox[1]],'uniformScale':ratio})
epng=OUT/'HeroGiovanaEffects.png';effects.save(epng);meta(epng,OUT/'HeroPedroEffects.png.meta')
(BASE/'Giovana/effects.json').write_text(json.dumps({'width':384,'height':256,'columns':3,'rows':2,'cell':[128,128],
    'imageSha256':SHA(epng),'sourceSha256':SHA(ef),'frames':erects,'alphaPaddingThreshold':8,'runtimeVerified':False},indent=2),encoding='utf-8')
effects.resize((768,512),Image.Resampling.NEAREST).save(BASE/'Giovana/effects-contact-2x.png')
contact.save(BASE/'GROUP_GIOVANA_ATTACK_CONTACT.png')
(BASE/'GROUP_GIOVANA_SUMMARY.json').write_text(json.dumps(summary,ensure_ascii=False,indent=2),encoding='utf-8')
print(json.dumps({n:{'frames':d['frames'],'holds':len(d['intentionalHolds'])}for n,d in summary.items()}))
