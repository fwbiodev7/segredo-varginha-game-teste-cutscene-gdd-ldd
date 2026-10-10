"""Inspect alpha connectivity and export sprite mesh metadata; never alter source PNGs."""
import json
from pathlib import Path
import numpy as np
from PIL import Image

ROOT = Path(__file__).resolve().parents[1]
ART = ROOT/'Assets/Resources/Varginha'

def components(image):
    mask = np.array(image.convert('RGBA'))[:, :, 3] >= 100
    runs, parents, previous = [], [], []
    def find(i):
        while parents[i] != i:
            parents[i] = parents[parents[i]]; i = parents[i]
        return i
    for y, row in enumerate(mask):
        edges = np.flatnonzero(np.diff(np.r_[False, row, False]))
        current = []
        for x0, x1 in zip(edges[::2], edges[1::2]):
            i = len(runs); runs.append((int(x0), y, int(x1-x0))); parents.append(i)
            current.append(i)
        p = 0
        for i in current:
            x, _, w = runs[i]
            while p < len(previous) and runs[previous[p]][0]+runs[previous[p]][2] < x: p += 1
            for j in previous[p:]:
                q, _, width = runs[j]
                if q > x+w: break
                a, b = find(i), find(j)
                if a != b: parents[a] = b
        previous = current
    groups = {}
    for i, run in enumerate(runs): groups.setdefault(find(i), []).append(run)
    result = []
    for rows in groups.values():
        area = sum(r[2] for r in rows)
        if area < 8: continue
        left = min(r[0] for r in rows); right = max(r[0]+r[2] for r in rows)
        top = min(r[1] for r in rows); bottom = max(r[1] for r in rows)+1
        result.append({'rect':[left, top, right-left, bottom-top], 'area':area, 'runs':rows})
    return result

def bands(runs):
    # Merge identical horizontal runs on successive rows into compact mesh rectangles.
    result, active = [], {}
    for x, y, width in sorted(runs, key=lambda r:(r[1],r[0])):
        key = (x, width)
        index = active.get(key)
        if index is not None and result[index][1]+result[index][3] == y:
            result[index][3] += 1
        else:
            active[key] = len(result); result.append([x,y,width,1])
    return [v for band in result for v in band]

def build():
    story = [p for p in (ART/'StoryCharacters').glob('*.png')
             if not any(x in p.stem for x in ('Effects','RenanProps'))]
    paths = story + list((ART/'TeamArt').glob('*.png')) + list((ART/'Allies').glob('*.png'))
    paths += [ART/'Experiment'/f'{name}.png' for name in ('ChildEdelzio','GameCast')]
    paths += [ART/f'{name}.png' for name in ('CharacterReferencesV1','EdelzioReferenceActionsV1',
        'EdelzioTopDownV3','EdelzioAttackV1','EdelzioInteractionsV1','EdelzioSeatedV2','EdelzioPunchV2','PadreFabioV1')]
    sheets, report = [], []
    for path in sorted(set(paths)):
        if not path.exists(): continue
        image = Image.open(path)
        regions = components(image)
        exported = []
        for r in regions:
            x,y,w,h = r['rect']
            head = [run for run in r['runs'] if run[1] < y+max(1,h*.2)]
            anchor = (min(run[0] for run in head)+max(run[0]+run[2] for run in head))/2
            exported.append({'rect':r['rect'],'area':r['area'],'anchorX':anchor,'bands':bands(r['runs'])})
        sheets.append({'name':path.stem,'path':'Assets/Resources/Varginha/'+path.relative_to(ART).as_posix(),
                       'width':image.width,'height':image.height,'regions':exported})
        report.append({'sheet':path.stem,'size':list(image.size),'regions':len(exported)})
    output = ART/'CharacterFrameGeometry.json'
    output.write_text(json.dumps({'sheets':sheets},separators=(',',':'))+'\n',encoding='utf-8')
    qa = ROOT/'Preview/PersonagensCorrigidos20261009'
    qa.mkdir(exist_ok=True)
    (qa/'Sources.json').write_text(json.dumps(report,indent=2)+'\n',encoding='utf-8')
    print(f'{len(sheets)} character sheets, {sum(len(s["regions"]) for s in sheets)} regions; metadata {output.stat().st_size//1024} KB; original PNGs untouched.')

if __name__ == '__main__': build()
