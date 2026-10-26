"""Build recipes/manifests for the generated Ouzana sheets; preserve source pixels.

Image assembly, mirrored directional poses and previews use the sprite skill's
pack/preview commands. This script measures alpha and writes metadata only.
"""
from pathlib import Path
from PIL import Image
import hashlib, json, shutil, subprocess, sys, uuid
import numpy as np

ROOT=Path(__file__).resolve().parents[2]
OUT=ROOT/'output/ouzana-2026-10-26'
SOURCES=OUT/'source'
SOURCES.mkdir(parents=True,exist_ok=True)
GEN=Path('C:/Users/Usuario/.codex/generated_images/01a177ca-2f99-77b0-b3c1-4d64ba42385a')
PIPE=Path('C:/Users/Usuario/.codex/skills/pixel-art-sprite-sheet-master/scripts/sprite_pipeline.py')
DIRECTIONS=['S','SW','W','NW','N','NE','E','SE']
SHEETS={'Walk':'exec-208860d1-18d6-4576-847b-9b6d02e9d19c.png','Attack':'exec-caf54fb3-6eae-4a07-a8d0-4b0aab9e8175.png'}

for action,filename in SHEETS.items():
    src=SOURCES/(action.lower()+'-generated.png')
    if not src.exists():shutil.copy2(GEN/filename,src)
    image=Image.open(src).convert('RGBA')
    assert image.size==(1086,1448),image.size
    alpha=np.array(image)[:,:,3]>=220
    def runs(values):
        edges=np.diff(np.r_[False,values,False].astype(int))
        return list(zip(np.where(edges==1)[0].tolist(),np.where(edges==-1)[0].tolist()))
    bands=runs(alpha.sum(axis=1)>30)
    assert len(bands)==8,bands
    boxes=[]
    for y0,y1 in bands:
        columns=runs(alpha[y0:y1].sum(axis=0)>10)
        assert len(columns)==6,columns
        row_boxes=[]
        for x0,x1 in columns:
            # Crop measured art, not an assumed equal image-generation grid.
            x0=max(0,x0-3);x1=min(image.width,x1+3);ymin=max(0,y0-3);ymax=min(image.height,y1+3)
            row_boxes.append([x0,ymin,x1-x0,ymax-ymin])
        boxes.append(row_boxes)
    frames=[];clips={}
    for row,direction in enumerate(DIRECTIONS):
        ids=[]
        for col in range(6):
            # NE is mirrored for NW to keep the release aimed away/up-left.
            # Symmetric glasses/coat have no text or side-specific badge.
            source_row=5 if action=='Attack' and direction=='NW' else row
            crop=boxes[source_row][col]
            cell=image.crop((crop[0],crop[1],crop[0]+crop[2],crop[1]+crop[3]))
            bbox=cell.getchannel('A').point(lambda a:255 if a>=220 else 0).getbbox()
            assert bbox is not None,(action,direction,col)
            mirror=action=='Attack' and direction=='NW'
            anchor_x=round((bbox[0]+bbox[2])/2)
            if mirror:anchor_x=crop[2]-anchor_x
            id=f'{action.lower()}_{direction.lower()}_{col}'
            f={'id':id,'source':str(src),'crop':crop,'anchor':[anchor_x,bbox[3]],'targetAnchor':[112,202],
               'durationMs':110 if action=='Walk' else [100,110,110,90,100,130][col]}
            if mirror:f['mirrorX']=True
            frames.append(f);ids.append(id)
        clips[f'{action.lower()}_{direction.lower()}']={'frames':ids,'loop':action=='Walk'}
        if action=='Walk':clips[f'idle_{direction.lower()}']={'frames':[ids[2]],'loop':True}
    recipe={'cell':[224,224],'columns':6,'padding':1,'pixelsPerUnit':108,'pivot':[112,202],
            'durationMs':110,'frames':frames,'clips':clips}
    recipe_path=OUT/(action.lower()+'-recipe.json');recipe_path.write_text(json.dumps(recipe,indent=2),encoding='utf-8')
    export=OUT/(action.lower()+'-export-v2')
    if not (export/'atlas.json').exists():
        subprocess.run([sys.executable,str(PIPE),'pack',str(recipe_path),'--out',str(export)],check=True)
    atlas=json.loads((export/'atlas.json').read_text(encoding='utf-8'))
    packed=Image.open(export/'sheet.png').convert('RGBA')
    runtime_frames=[]
    for i,f in enumerate(atlas['frames']):
        x,y,w,h=f['rect'];body=packed.crop((x,y,x+w,y+h))
        bx0,by0,bx1,by1=body.getchannel('A').point(lambda a:255 if a>=220 else 0).getbbox()
        runtime_frames.append({'rect':[x+bx0,y+by0,bx1-bx0,by1-by0],
                               'anchorX':x+112,'lift':0,'action':action.lower(),'direction':DIRECTIONS[i//6],'index':i%6})
    name='Ouzana'+action+'V2';asset=ROOT/'Assets/Resources/Varginha/StoryCharacters'/name
    shutil.copy2(export/'sheet.png',asset.with_suffix('.png'))
    manifest={'source':'reference-20261026-dark-brown-glasses','width':atlas['imageSize'][0],'height':atlas['imageSize'][1],
              'columns':6,'rows':8,'directions':DIRECTIONS,'coordinateOrigin':'top-left','pixelsPerUnit':[108]*8,
              'frames':runtime_frames}
    asset.with_suffix('.json').write_text(json.dumps(manifest,indent=2),encoding='utf-8')
    for ext in ['.png','.json']:
        meta=Path(str(asset.with_suffix(ext))+'.meta')
        if not meta.exists():
            source_meta=ROOT/'Assets/Resources/Varginha/StoryCharacters'/('OuzanaBiologist'+ext+'.meta')
            if ext=='.png':source_meta=ROOT/'Assets/Resources/Varginha/SchoolGlobe/Globe.png.meta'
            import re
            text=re.sub(r'(?m)^guid: .*$', 'guid: '+uuid.uuid4().hex,source_meta.read_text(encoding='utf-8'))
            meta.write_text(text,encoding='utf-8')
    contract={'allowSemiTransparency':True,'lockPivot':True,'maxTextureSize':2048,
              'requiredClips':list(clips),'areaRatioWarning':1.8}
    contract_path=OUT/(action.lower()+'-contract.json');contract_path.write_text(json.dumps(contract,indent=2),encoding='utf-8')
    if not (OUT/(action.lower()+'-validation')).exists():
        subprocess.run([sys.executable,str(PIPE),'validate',str(export/'atlas.json'),'--contract',str(contract_path),'--out',str(OUT/(action.lower()+'-validation'))],check=True)
    if not (OUT/(action.lower()+'-preview')).exists():
        subprocess.run([sys.executable,str(PIPE),'preview',str(export/'atlas.json'),'--scale','2','--out',str(OUT/(action.lower()+'-preview'))],check=True)
    print(json.dumps({'action':action,'frames':len(frames),'clips':len(clips),'source_sha256':hashlib.sha256(src.read_bytes()).hexdigest(),'asset':str(asset)}))
