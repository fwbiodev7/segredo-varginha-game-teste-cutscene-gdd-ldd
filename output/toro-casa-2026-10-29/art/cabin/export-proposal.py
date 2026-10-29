"""Export ImageGen material proposals under strict original geometry/alpha masks.
Only original blue sheet metal and original wood material may change. Neutral
trim, gauges, handles, seats, headliner and the exact alpha channel are reused.
The generated blue material is quantized to the four original Fusca blue colors.
"""
from pathlib import Path
from PIL import Image, ImageDraw
from collections import Counter
from statistics import median
import hashlib,json,shutil

OUT=Path(__file__).resolve().parent
ROOT=Path(__file__).resolve().parents[4]
SRC=ROOT/'Assets/Resources/Varginha/CinemaOpening'
GEN=Path('C:/Users/Usuario/.codex/generated_images/01a1882b-9263-7e40-866e-4a90f45ad7cf')
ROWS=[('FuscaCabinCartoon.png','exec-734cbd99-14e5-441b-a632-319f0c4fa561.png'),
      ('FuscaDashboardCartoon.png','exec-fb569b01-3e59-4023-9d32-384f2f9fbacb.png')]
PALETTE=[(69,179,230),(49,137,204),(24,105,143),(89,210,247)]
def sha(p):return hashlib.sha256(p.read_bytes()).hexdigest()
def pixsha(im):return hashlib.sha256(im.tobytes()).hexdigest()
def write(name,data):(OUT/name).write_text(json.dumps(data,ensure_ascii=False,indent=2)+'\n',encoding='utf-8')
reports=[]
originals=[];finals=[]
for name,generated in ROWS:
    sourcepath=SRC/name; before=sha(sourcepath)
    src=Image.open(sourcepath).convert('RGBA'); gen=Image.open(GEN/generated).convert('RGBA')
    assert src.size==gen.size
    originalpixels=list(src.getdata()); generatedpixels=list(gen.getdata())
    step=4;gw=(src.width+step-1)//step;gh=(src.height+step-1)//step
    pools=[[] for _ in range(gw*gh)];fallback=[[] for _ in range(gw*gh)]
    maskpixels=[]
    for i,(px,gpx) in enumerate(zip(originalpixels,generatedpixels)):
        r,g,b,a=px;x=i%src.width;y=i//src.width
        blue=(b>max(70,r*1.15) and b>g*1.08 and g>r*1.2)
        wood=(r>g*1.25 and g>b*1.08 and r>20)
        if name=='FuscaCabinCartoon.png': wood=wood and 260<=x<=890 and 380<=y<=485
        allowed=a>=128 and (blue or wood)
        maskpixels.append(255 if allowed else 0)
        if allowed:
            gi=(y//step)*gw+x//step
            fallback[gi].append(px[:3] if blue else (49,137,204))
            nr,ng,nb,na=gpx
            if na>=128 and nb>max(60,nr*1.1) and nb>ng*1.04 and ng>nr*1.1:
                pools[gi].append(gpx[:3])
    # Quantize only generated BLUE material, excluding neutral/clear neighbors.
    # This prevents recoloring a window edge into isolated dark palette blocks.
    indices=[]
    for pool,back in zip(pools,fallback):
        selected=pool or back
        if not selected:indices.append(None);continue
        rgb=tuple(median(p[c] for p in selected) for c in range(3))
        indices.append(min(range(4),key=lambda j:sum((v-u)**2 for v,u in zip(PALETTE[j],rgb))))
    clean=indices.copy()
    for gy in range(gh):
        for gx in range(gw):
            i=gy*gw+gx
            if indices[i] is None:continue
            neighbors=[indices[ny*gw+nx] for ny in range(max(0,gy-1),min(gh,gy+2))
                       for nx in range(max(0,gx-1),min(gw,gx+2)) if indices[ny*gw+nx] is not None]
            counts=Counter(neighbors);winner,n=counts.most_common(1)[0]
            if counts[indices[i]]<=2 and n>=4:clean[i]=winner
    result=[];paintcount=0;changed=0;mask=Image.new('L',src.size)
    for i,(px,allowed) in enumerate(zip(originalpixels,maskpixels)):
        x=i%src.width;y=i//src.width;a=px[3]
        if allowed:
            rgb=PALETTE[clean[(y//step)*gw+x//step]]
            new=(*rgb,a);paintcount+=1
        else:new=px
        result.append(new)
        changed+=new!=px
    image=Image.new('RGBA',src.size);image.putdata(result);image.save(OUT/name)
    mask.putdata(maskpixels);mask.save(OUT/(Path(name).stem+'-paint-mask.png'))
    shutil.copy2(GEN/generated,OUT/(Path(name).stem+'-imagegen-source.png'))
    assert src.getchannel('A').tobytes()==image.getchannel('A').tobytes()
    assert all(maskvalue or srcpx==newpx for maskvalue,srcpx,newpx in zip(maskpixels,originalpixels,result))
    assert sha(sourcepath)==before
    row={'image':name,'imageSize':list(src.size),'rectTopLeft':[0,0,*src.size],
         'status':'proposal-only; not integrated','source':str(sourcepath.relative_to(ROOT)),
         'sourceSha256Before':before,'sourceSha256After':sha(sourcepath),
         'generatedSource':Path(name).stem+'-imagegen-source.png','generatedSourceSha256':sha(GEN/generated),
         'outputSha256':sha(OUT/name),'alphaSha256':pixsha(src.getchannel('A')),
         'alphaExactlyPreserved':True,'nonPaintPixelsExactlyPreserved':True,
         'paintRegionPixels':paintcount,'changedPixels':changed,
         'paintPalette':['#%02X%02X%02X'%p for p in PALETTE],
         'operations':'ImageGen material edit; blue paint palette-normalized on4pxgrid using only blue generated samples, isolated palette specks removed by neighborhood majority; composite only under source blue/wood material mask; source alpha and all nonpaint pixels reused without resampling.'}
    if 'Dashboard' in name:
        # Gauge face, ring and neutral boundary remain byte-identical to source.
        row['runtimeGaugeOverlayCentreTopLeft']=[2172*141/780,724*109/260]
        row['runtimeGaugeCentrePixelRGBA']=list(image.getpixel((393,304)))
        row['gaugeFaceHasNoPaintedTicksOrNeedle']=True
    reports.append(row);originals.append(src);finals.append(image)

# Display exact registration geometry used by the existing cinema; preview only.
def checker(size):
    im=Image.new('RGBA',size,(32,37,44,255));d=ImageDraw.Draw(im)
    for y in range(0,size[1],16):
        for x in range(0,size[0],16):
            if (x//16+y//16)%2:d.rectangle((x,y,x+15,y+15),fill=(42,48,56,255))
    return im
preview=Image.new('RGB',(1280,630),(25,30,37));d=ImageDraw.Draw(preview)
d.text((10,8),'ORIGINAL',fill='white');d.text((650,8),'PROPOSTA - chapa azul do Fusca',fill='white')
for col,images in enumerate((originals,finals)):
    cab=checker((640,360));cab.alpha_composite(images[0].resize((640,360),Image.Resampling.NEAREST));preview.paste(cab.convert('RGB'),(640*col,28))
    dash=checker((640,213));dash.alpha_composite(images[1].resize((640,213),Image.Resampling.NEAREST));preview.paste(dash.convert('RGB'),(640*col,406))
preview.save(OUT/'comparison-original-proposal.png')
scene=checker((1280,720))
scene.alpha_composite(finals[0].resize((1280,720),Image.Resampling.NEAREST))
scene.alpha_composite(finals[1].resize((940,313),Image.Resampling.NEAREST),(390,300))
scene.convert('RGB').save(OUT/'proposal-layer-registration.png')
write('proposal-metadata.json',{'status':'proposal-only; no Assets, scripts or scenes edited','generator':'built-in image_gen',
                              'styleReference':'Assets/Resources/Varginha/StoryEffects/FuscaOriginal.png',
                              'styleReferenceSha256':sha(ROOT/'Assets/Resources/Varginha/StoryEffects/FuscaOriginal.png'),
                              'promptFile':'generation-prompts.txt','layers':reports,
                              'verification':{'dimensionsMatch':True,'alphaChannelMatchesByteForByte':True,
                                              'allNonpaintSourcePixelsMatchByteForByte':True,'originalFilesUnchanged':True,
                                              'unityTested':False,'visuallyReviewedAfterExport':False}})
print(json.dumps([{'image':r['image'],'size':r['imageSize'],'paintPixelsChanged':r['changedPixels'],
                   'alphaExactlyPreserved':r['alphaExactlyPreserved'],'sourceUnchanged':r['sourceSha256Before']==r['sourceSha256After']} for r in reports]))
