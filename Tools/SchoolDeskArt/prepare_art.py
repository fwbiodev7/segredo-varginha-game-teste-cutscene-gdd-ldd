"""Preserve the generated source; assemble exact crops with the pixel-art skill.

No painted source is overwritten. This script only writes metadata and copies
the chosen RGBA file; the skill's pack command performs the atlas assembly.
"""
import hashlib
import json
import shutil
from pathlib import Path
from PIL import Image

ROOT = Path(__file__).resolve().parents[2]
OUT = ROOT / 'output/school-desk-2026-10-22/art'
SOURCE = Path('C:/Users/Usuario/.codex/generated_images/01a15918-63bb-7b31-b60b-3eef3c6eca04/exec-d948e538-0911-4004-9f21-9825e9e3e4b0.png')


def prepare():
    OUT.mkdir(parents=True, exist_ok=True)
    shutil.copy2(SOURCE, OUT / 'generated-source.png')
    crops = [[74, 52, 602, 631], [807, 182, 790, 496], [1730, 404, 402, 287]]
    frames = []
    for name, crop in zip(['backpack', 'notebook_rear', 'whiteboard_markers'], crops):
        frames.append(dict(id=name, source='generated-source.png', crop=crop,
                           anchor=[crop[2] / 2, crop[3]], targetAnchor=[400, 670]))
    recipe = dict(cell=[800, 672], columns=3, padding=0, pixelsPerUnit=100,
                  pivot=[400, 670], durationMs=1000, frames=frames,
                  clips={f['id']: dict(frames=[f['id']], loop=False) for f in frames})
    (OUT / 'recipe.json').write_text(json.dumps(recipe, indent=2), encoding='utf-8')
    provenance = dict(source=str(SOURCE), sourceSha256=hashlib.sha256(SOURCE.read_bytes()).hexdigest(),
                      method='Built-in image_gen, new transparent atlas; exact crop/registration only',
                      authorization='User: yes aprovado bro, 22/10/2026; not independent artistic approval',
                      crops=crops, sourceSize=list(Image.open(SOURCE).size),
                      identity='Charcoal gray backpack, rear-facing dark notebook, blue/red/black whiteboard markers',
                      originalAssetsPreserved=['RenanProps.png', 'RenanProps.json', 'School.png'])
    (OUT / 'provenance.json').write_text(json.dumps(provenance, indent=2), encoding='utf-8')


def integrate():
    atlas = json.loads((OUT / 'export/atlas.json').read_text())
    image = Image.open(OUT / 'export' / atlas['image'])
    runtime = dict(width=image.width, height=image.height, rows=1, columns=3,
                   coordinateOrigin='top-left', pixelsPerUnit=[100], frames=[])
    for frame in atlas['frames']:
        x, y, w, h = frame['rect']
        # Keep generated alpha and every visible detail inside the selected crop.
        box = image.crop((x, y, x + w, y + h)).getchannel('A').getbbox()
        l, t, r, b = box
        runtime['frames'].append(dict(rect=[x + l, y + t, r - l, b - t],
                                      anchorX=x + (l + r) / 2, lift=0))
    target = ROOT / 'Assets/Resources/Varginha/StoryCharacters'
    shutil.copy2(OUT / 'export' / atlas['image'], target / 'RenanDeskProps.png')
    (target / 'RenanDeskProps.json').write_text(json.dumps(runtime, indent=2), encoding='utf-8')
    print(json.dumps(dict(asset='RenanDeskProps', size=image.size, frames=len(runtime['frames']))))


if __name__ == '__main__':
    import sys
    integrate() if '--integrate' in sys.argv else prepare()
