"""Read generated art; export exact crops and QA through the user's sprite skill."""
import hashlib
import json
from pathlib import Path
import subprocess
import sys
import zipfile
from PIL import Image

ROOT=Path(__file__).resolve().parents[2]
OUT=ROOT/'output/visual-polish-2026-10-21/art'
SKILL=Path.home()/'.codex/skills/pixel-art-sprite-sheet-master/scripts/sprite_pipeline.py'

def write(path,data):
    path.write_text(json.dumps(data,ensure_ascii=False,indent=2),encoding='utf-8')

def run(*args):
    subprocess.run([sys.executable,str(SKILL),*map(str,args)],check=True,cwd=ROOT)

def main():
    folder=OUT/'skill-exports'
    folder.mkdir(exist_ok=False)
    contract=folder/'contract.json'
    write(contract,{'allowSemiTransparency':True,'lockPivot':True,'maxTextureSize':4096})
    for name,relative,columns,rows,ids,duration,loop in [
        ('child','Assets/Resources/Varginha/CinemaOpening/ChildWatchingTV.png',4,2,[f'watch_{i}' for i in range(8)],125,True),
        ('landmarks','Assets/Resources/Varginha/TrailPuzzle/Landmarks.png',3,2,['tree','fallen_log','bridge','wall','chapel','open_gate'],1000,False)]:
        source=ROOT/relative
        with Image.open(source) as image:w,h=image.size
        cell=[-(-w//columns),-(-h//rows)]
        frames=[]
        for i,frame_id in enumerate(ids):
            x0,y0=int(i%columns*w/columns),int(i//columns*h/rows)
            x1,y1=int((i%columns+1)*w/columns),int((i//columns+1)*h/rows)
            frames.append({'id':frame_id,'source':str(source),'sourceSha256':hashlib.sha256(source.read_bytes()).hexdigest(),
                'crop':[x0,y0,x1-x0,y1-y0],'pivot':[cell[0]/2,cell[1]],'durationMs':duration})
        clips={'watch_tv':{'frames':ids,'loop':loop}} if loop else {i:{'frames':[i],'loop':False} for i in ids}
        recipe=folder/(name+'-recipe.json')
        write(recipe,{'cell':cell,'columns':columns,'padding':0,'pixelsPerUnit':100,'frames':frames,'clips':clips})
        run('pack',recipe,'--out',folder/name)
        atlas=folder/name/'atlas.json'
        run('validate',atlas,'--contract',contract,'--out',folder/(name+'-validation'))
        run('preview',atlas,'--scale','1','--out',folder/(name+'-preview'))
    records=[]
    names=['FuscaCabinCartoon','FuscaDashboardCartoon','ChildRoom1996','ChildWatchingTV','Newspaper1996','MorningTown']
    paths=[ROOT/f'Assets/Resources/Varginha/CinemaOpening/{n}.png' for n in names]
    paths += [ROOT/f'Assets/Resources/Varginha/TrailPuzzle/{n}.png' for n in ['Terrain','Landmarks']]
    with zipfile.ZipFile(folder/'palettes.zip','x',zipfile.ZIP_DEFLATED,compresslevel=6) as archive:
        for path in paths:
            with Image.open(path) as image:
                rgba=image.convert('RGBA');colors=rgba.getcolors(rgba.width*rgba.height)
                visible=sorted(((count,color) for count,color in colors if color[3]),reverse=True)
                record={'file':path.relative_to(ROOT).as_posix(),'size':rgba.size,'sha256':hashlib.sha256(path.read_bytes()).hexdigest(),
                    'rgbaSha256':hashlib.sha256(rgba.tobytes()).hexdigest(),'colors':len(visible),'alpha':rgba.getchannel('A').getextrema()}
                records.append(record)
                archive.writestr(path.stem+'.json',json.dumps(visible,separators=(',',':')))
        archive.testzip()
    write(folder/'source-inventory.json',records)
    print(json.dumps({'success':True,'sources':len(records),'exports':2,'out':str(folder)}))

if __name__=='__main__':main()
