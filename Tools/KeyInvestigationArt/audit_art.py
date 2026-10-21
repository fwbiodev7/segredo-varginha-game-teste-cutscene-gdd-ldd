"""Read-only QA of the shipped evidence art using pixel-art-sprite-sheet-master."""
from pathlib import Path
import argparse, hashlib, json, os, subprocess, sys
from PIL import Image

ROOT=Path(__file__).resolve().parents[2]

def digest(path): return hashlib.sha256(path.read_bytes()).hexdigest()

def main():
    parser=argparse.ArgumentParser()
    parser.add_argument('--skill',type=Path,default=Path.home()/'.codex/skills/pixel-art-sprite-sheet-master')
    parser.add_argument('--out',type=Path,required=True)
    args=parser.parse_args(); out=args.out.resolve(); out.mkdir(parents=True,exist_ok=False)
    helper=args.skill/'scripts/sprite_pipeline.py'
    if not helper.is_file(): raise FileNotFoundError(helper)
    source=ROOT/'output/key-investigation-2026-10-20/art'
    shipped=ROOT/'Assets/Resources/Varginha/KeyInvestigation/HouseEvidence.png'
    meta=json.loads((shipped.with_suffix('.json')).read_text(encoding='utf-8'))
    frames=[]
    for f in meta['frames']:
        image=source/(f['id']+'.png')
        frames.append(dict(id=f['id'],source=os.path.relpath(image,out),sourceSha256=digest(image),
                           durationMs=450 if f['id']=='key_place' else 600 if f['id']=='box_closed' else 1000))
    clips={'key_deposit':{'frames':['key_place','key_rest'],'loop':False},
           'box_open':{'frames':['box_closed','box_open'],'loop':False}}
    for name in ('paper_trace','page_house','page_tree','page_figure'):
        clips[name]={'frames':[name],'loop':False}
    recipe=dict(cell=[128,128],columns=4,padding=0,pixelsPerUnit=128,pivot=[64,64],frames=frames,clips=clips)
    (out/'recipe.json').write_text(json.dumps(recipe,indent=2),encoding='utf-8')
    contract=dict(allowSemiTransparency=False,maxColors=256,requiredClips=list(clips),lockPivot=True,maxTextureSize=2048)
    (out/'contract.json').write_text(json.dumps(contract,indent=2),encoding='utf-8')
    def run(*argv): subprocess.run([sys.executable,str(helper),*map(str,argv)],check=True)
    run('inspect',shipped,'--out',out/'inventory')
    run('pack',out/'recipe.json','--out',out/'export')
    atlas=out/'export/atlas.json'
    run('validate',atlas,'--contract',out/'contract.json','--out',out/'validation')
    run('validate',atlas,'--contract',out/'contract.json','--unity-meta',shipped.with_suffix('.png.meta'),'--out',out/'unity-meta-triage')
    run('preview',atlas,'--scale','3','--out',out/'preview')
    exported=json.loads(atlas.read_text(encoding='utf-8'))
    packed=atlas.parent/exported['image']
    equal=Image.open(packed).convert('RGBA').tobytes()==Image.open(shipped).convert('RGBA').tobytes()
    result=dict(success=equal,shippedSha256=digest(shipped),packedSha256=digest(packed),rgbaPixelsUnchanged=equal,
                frames=len(frames),helperSha256=digest(helper),
                timing='Preview holds; runtime cinematic motion remains in CampaignHouseSequence.cs')
    (out/'preservation.json').write_text(json.dumps(result,indent=2),encoding='utf-8')
    print(json.dumps(result))
    if not equal: raise RuntimeError('The canonical export changed shipped pixels')

if __name__=='__main__': main()
