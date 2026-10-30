"""Technical extraction of complete attack figures; no generated drawing or recoloring."""
from pathlib import Path
from collections import deque
import hashlib,json,re,uuid
from PIL import Image,ImageDraw
ROOT=Path(__file__).resolve().parents[2]
BASE=Path(__file__).resolve().parent
OUT=ROOT/'Assets/Resources/Varginha/StoryCharacters'
PREVIEW=ROOT/'Preview/AttackSheets20261029/GroupB'
PREVIEW.mkdir(parents=True,exist_ok=True)
NAMES=['AnaTavares','LuisMiguelMessias','Bianca','Moscardini']
PPU=82.90155440414507
sha=lambda p:hashlib.sha256(p.read_bytes()).hexdigest()
raw_sha=lambda im:hashlib.sha256(im.tobytes()).hexdigest()

def actors(path,count,rows):
    im=Image.open(path).convert('RGBA'); data=im.load()
    visible={(x,y)for y in range(im.height)for x in range(im.width)if data[x,y][3]>=128}
    groups=[]
    while visible:
        seed=visible.pop();q=deque([seed]);body={seed}
        while q:
            x,y=q.popleft()
            for dx,dy in ((-1,-1),(-1,0),(-1,1),(0,-1),(0,1),(1,-1),(1,0),(1,1)):
                p=(x+dx,y+dy)
                if p in visible:visible.remove(p);body.add(p);q.append(p)
        groups.append(body)
    figures=[]
    for body in sorted(groups,key=len,reverse=True)[:count]:
        x0=min(x for x,y in body);y0=min(y for x,y in body)
        x1=max(x for x,y in body)+1;y1=max(y for x,y in body)+1
        fig=Image.new('RGBA',(x1-x0,y1-y0)); dst=fig.load()
        for x,y in body:dst[x-x0,y-y0]=data[x,y]
        feet=[x for x,y in body if y>=y1-max(4,(y1-y0)//12)]
        figures.append({'image':fig,'rect':[x0,y0,x1-x0,y1-y0],
            'footX':sum(feet)/len(feet)-x0,'factor':1.0,'source':path.name})
    figures.sort(key=lambda f:f['rect'][1]+f['rect'][3]/2)
    ordered=[]
    for row in range(rows):
        strip=figures[row*(count//rows):(row+1)*(count//rows)]
        ordered.extend(sorted(strip,key=lambda f:f['rect'][0]+f['rect'][2]/2))
    return ordered

def ensure_meta(asset):
    meta=asset.with_suffix(asset.suffix+'.meta')
    if meta.exists():return
    if asset.suffix=='.json':
        text='fileFormatVersion: 2\nguid: '+uuid.uuid4().hex+'\nTextScriptImporter:\n  externalObjects: {}\n  userData: \n  assetBundleName: \n  assetBundleVariant: \n'
    else:
        text=(OUT/'HeroPedroEffects.png.meta').read_text(encoding='utf8')
        text=re.sub(r'guid: [0-9a-f]+','guid: '+uuid.uuid4().hex,text,count=1)
        text=re.sub(r'textureCompression: \d+','textureCompression: 0',text)
    meta.write_text(text,encoding='utf8')

summary=[]
for name in NAMES:
    folder=BASE/name
    figures=actors(folder/'actors-source.png',12,2)
    if name=='AnaTavares':
        replacements=actors(folder/'west-contact-repairs.png',2,1)
        # Both complete repaired figures use a larger source resolution. Normalize
        # the whole replacement batch by its first standing figure, never limbs.
        factor=figures[0]['image'].height/replacements[0]['image'].height
        for index,replacement in zip((4,10),replacements):
            replacement['factor']=factor;figures[index]=replacement
    ratio=min(108/max(f['image'].height*f['factor']for f in figures),
        116/max(f['image'].width*f['factor']for f in figures))
    atlas=Image.new('RGBA',(768,256)); frames=[]; provenance=[]
    for index,figure in enumerate(figures):
        row,col=divmod(index,6);scale=ratio*figure['factor'];src=figure['image']
        small=src.resize((round(src.width*scale),round(src.height*scale)),Image.Resampling.NEAREST)
        box=small.getchannel('A').getbbox();small=small.crop(box)
        footx=figure['footX']*scale-box[0]
        left=max(4,min(124-small.width,round(64-footx)));top=120-small.height
        atlas.alpha_composite(small,(col*128+left,row*128+top))
        crop=atlas.crop((col*128,row*128,(col+1)*128,(row+1)*128))
        frames.append({'rect':[col*128,row*128,128,128],
            'anchorX':col*128+left+footx,'lift':.58-8/PPU,'action':row,
            'direction':'east'if col<3 else'west','stage':['anticipation','contact','recovery'][col%3],
            'sha256':raw_sha(crop)})
        provenance.append({'frame':index,'source':figure['source'],'sourceRect':figure['rect'],
            'uniformScale':scale,'destination':[col*128+left,row*128+top,small.width,small.height]})
    output=OUT/('Hero'+name+'.png');atlas.save(output);ensure_meta(output)
    manifest={'width':768,'height':256,'columns':6,'rows':2,'pixelsPerUnit':[PPU,PPU],
        'frames':frames,'source':'ArtSources/AttackSheets20261029/'+name,
        'status':'visual-reviewed','visualApproval':False}
    json_file=output.with_suffix('.json');json_file.write_text(json.dumps(manifest,indent=2),encoding='utf8');ensure_meta(json_file)
    (folder/'provenance.json').write_text(json.dumps({'actorSourceHash':sha(folder/'actors-source.png'),
        'identityHash':sha(folder/'canonical-identity.png'),'frames':provenance,
        'outputHash':sha(output),'notes':['Complete figures only; no anatomy assembly or mirrored direction.',
        'Shared scale per sheet; Ana two full-frame west contact repairs separately generated, remaining10frames unchanged.',
        'Alpha-connected figure extraction preserves material colors; source transparent padding islands omitted.',
        'Foot anchor computed from lower complete-figure silhouette; JSON lift matches ground foot baseline8.',
        'Runtime Unity verification belongs to parent integration.']},indent=2),encoding='utf8')
    bg=Image.new('RGBA',atlas.size,(224,229,224,255));bg.alpha_composite(atlas)
    bg.resize((1536,512),Image.Resampling.NEAREST).save(PREVIEW/(name+'-actors-2x.png'))
    # Effects contain intentionally disconnected pages/Zz/sparks; preserve all of
    # each original complete cell and its alpha, never select largest component.
    fx_source=folder/'effects-source.png'
    if fx_source.exists():
        fx=Image.open(fx_source).convert('RGBA');effects=Image.new('RGBA',(384,256))
        for i in range(6):
            row,col=divmod(i,3)
            area=fx.crop((col*fx.width//3,row*fx.height//2,(col+1)*fx.width//3,(row+1)*fx.height//2))
            effects.alpha_composite(area.resize((128,128),Image.Resampling.NEAREST),(col*128,row*128))
        fx_output=OUT/('Hero'+name+'Effects.png');effects.save(fx_output);ensure_meta(fx_output)
        preview=Image.new('RGBA',effects.size,(35,43,54,255));preview.alpha_composite(effects)
        preview.resize((768,512),Image.Resampling.NEAREST).save(PREVIEW/(name+'-effects-2x.png'))
    hashes=[f['sha256']for f in frames]
    assert len(set(hashes))==12,(name,'duplicate frames')
    for i in range(12):
        row,col=divmod(i,6);box=atlas.crop((col*128,row*128,(col+1)*128,(row+1)*128)).getchannel('A').getbbox()
        assert box and box[0]>=2 and box[1]>=2 and box[2]<=126 and box[3]<=122,(name,i,box)
    summary.append({'name':name,'frames':12,'unique':len(set(hashes)),'size':[768,256],
        'effects':'generated'if fx_source.exists()else'existing retained','actorHash':sha(output),
        'rowCounts':[6,6],'footBaseline':120,'bounds':'PASS','metaGuid':'preserved if present'})
(BASE/'validation-group-b.json').write_text(json.dumps(summary,indent=2),encoding='utf8')
print(json.dumps(summary))
