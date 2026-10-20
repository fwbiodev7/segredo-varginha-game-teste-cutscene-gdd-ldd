"""Pack new generated candidates; never modify the game's reference artwork."""
from pathlib import Path
import hashlib, json, shutil, sys
from PIL import Image, ImageOps

ROOT = Path(__file__).resolve().parents[2]
OUT = ROOT / 'output/key-investigation-2026-10-20/art'
DEST = ROOT / 'Assets/Resources/Varginha/KeyInvestigation'
SOURCE = Path(sys.argv[1]) if len(sys.argv)>1 else OUT/'generated-source.png'
IDS = ['key_rest','key_place','paper_trace','box_closed','box_open','page_house','page_tree','page_figure']

def sha(p): return hashlib.sha256(p.read_bytes()).hexdigest()

def main():
    OUT.mkdir(parents=True, exist_ok=True); DEST.mkdir(parents=True, exist_ok=True)
    if SOURCE.resolve()!=(OUT/'generated-source.png').resolve():shutil.copy2(SOURCE, OUT / 'generated-source.png')
    source = Image.open(SOURCE).convert('RGBA')
    # Rectangles reviewed on the candidate. Separate tight crops avoid the model's
    # non-integral sheet grid. Pad, then reduce by exactly four with nearest pixels.
    rects = [(15,70,435,415),(505,65,865,420),(948,45,1270,420),(1335,50,1765,420),
             (10,445,465,884),(475,465,890,880),(895,465,1335,880),(1340,465,1767,880)]
    atlas = Image.new('RGBA',(512,256))
    frames=[]
    for i,(name,rect) in enumerate(zip(IDS,rects)):
        raw=source.crop(rect); assert raw.width<=512 and raw.height<=512
        canvas=Image.new('RGBA',(512,512))
        canvas.paste(raw,((512-raw.width)//2,(512-raw.height)//2))
        frame=canvas.resize((128,128),Image.Resampling.NEAREST)
        alpha=frame.getchannel('A').point(lambda a:255 if a>=128 else 0)
        # New, unapproved candidate only: establish a compact palette, binary alpha.
        frame=frame.convert('RGB').quantize(colors=32,method=Image.Quantize.MEDIANCUT,dither=Image.Dither.NONE).convert('RGBA')
        frame.putalpha(alpha)
        frame.save(OUT / (name+'.png'))
        x,y=i%4*128,i//4*128;atlas.paste(frame,(x,y))
        frames.append(dict(id=name,rect=[x,y,128,128],pivot=[64,64],durationMs=150 if i==1 else 1000))
    atlas.save(DEST/'HouseEvidence.png')
    metadata=dict(image='HouseEvidence.png',size=[512,256],cell=[128,128],ppu=128,
                  origin='top-left',alpha='binary',filter='Point',frames=frames,
                  clips={'key_place':{'frames':['key_place','key_rest'],'loop':False},
                         'box_open':{'frames':['box_closed','box_open'],'loop':False}},
                  sourceSha256=sha(SOURCE),approved=False)
    (DEST/'HouseEvidence.json').write_text(json.dumps(metadata,indent=2),encoding='utf8')
    preview=Image.new('RGBA',(512,256),(30,37,43,255));preview.alpha_composite(atlas)
    preview.resize((1024,512),Image.Resampling.NEAREST).save(OUT/'contact-sheet-2x.png')
    for label,seq in [('key_place',[1,0]),('box_open',[3,4])]:
        images=[Image.open(OUT/(IDS[i]+'.png')).resize((512,512),Image.Resampling.NEAREST) for i in seq]
        images[0].save(OUT/(label+'.apng'),save_all=True,append_images=images[1:],duration=[450,1300],loop=0,disposal=1)
    validation=dict(frames=len(frames),canvas=[512,256],binaryAlpha=set(atlas.getchannel('A').get_flattened_data())<={0,255},
                    allNonEmpty=all(atlas.crop((f['rect'][0],f['rect'][1],f['rect'][0]+128,f['rect'][1]+128)).getbbox() for f in frames),
                    sourceSha256=sha(SOURCE),atlasSha256=sha(DEST/'HouseEvidence.png'),
                    references={str(p.relative_to(ROOT)):sha(p) for p in [ROOT/'Assets/Resources/Varginha/IllustratedMaps/Adult2026.png',ROOT/'Assets/Resources/Varginha/StoryEffects/FuscaOriginal.png']})
    assert validation['binaryAlpha'] and validation['allNonEmpty']
    (OUT/'validation.json').write_text(json.dumps(validation,indent=2),encoding='utf8')
    print(json.dumps(validation))

if __name__=='__main__': main()
