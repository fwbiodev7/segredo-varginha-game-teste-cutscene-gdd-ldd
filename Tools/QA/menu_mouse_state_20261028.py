"""Preserve the existing save before a native mouse review, then restore and hash it."""
from pathlib import Path
import argparse
import hashlib
import json
import re
import shutil

ROOT = Path(__file__).resolve().parents[2]
EVIDENCE = ROOT / 'output/menu-continuation-2026-10-28'
BACKUP = EVIDENCE / 'mouse-save-backup'
settings = (ROOT / 'ProjectSettings/ProjectSettings.asset').read_text(encoding='utf-8-sig')
company = re.search(r'^  companyName: (.+)$', settings, re.M).group(1)
product = re.search(r'^  productName: (.+)$', settings, re.M).group(1)
SAVE = Path.home() / 'AppData/LocalLow' / company / product

def sha(path):
    return hashlib.sha256(path.read_bytes()).hexdigest()

parser = argparse.ArgumentParser()
parser.add_argument('action', choices=['prepare', 'restore'])
args = parser.parse_args()
manifest_path = BACKUP / 'manifest.json'
if args.action == 'prepare':
    assert not manifest_path.exists(), 'Existing review backup preserved; restore it before another review.'
    fixture = EVIDENCE / 'school-mouse-start.json'
    story = json.loads(fixture.read_text(encoding='utf-8-sig'))
    layout = next(x for x in json.loads((ROOT / 'Assets/Resources/Varginha/IllustratedMaps/Layouts.json').read_text(encoding='utf-8-sig'))['layouts'] if x['phase'] == 4)
    point = next(x for x in layout['points'] if x['id'] == 'notebook')['pixel']
    bounds = layout['bounds']; scale = bounds[2] / layout['width']
    story.update(positionPhase=4, x=bounds[0] + point[0] * scale, y=bounds[1] + bounds[3] - point[1] * scale - .22)
    BACKUP.mkdir(parents=True, exist_ok=True)
    originals = {}
    for name in ['CampaignStory2026.json', 'CampaignMemory1996.json', 'ExperimentGDDLDDSave.json']:
        source = SAVE / name
        originals[name] = sha(source) if source.exists() else None
        if source.exists():
            shutil.copy2(source, BACKUP / name)
    manifest = {'save_directory': str(SAVE), 'originals': originals, 'restored': False}
    manifest_path.write_text(json.dumps(manifest, indent=2), encoding='utf-8')
    SAVE.mkdir(parents=True, exist_ok=True)
    (SAVE / 'CampaignStory2026.json').write_text(json.dumps(story, ensure_ascii=False, indent=2), encoding='utf-8')
    print(json.dumps({'prepared': True, 'phase': 4, 'backup': str(BACKUP)}))
else:
    manifest = json.loads(manifest_path.read_text(encoding='utf-8'))
    assert Path(manifest['save_directory']) == SAVE
    for name, original in manifest['originals'].items():
        destination = SAVE / name
        if original is None:
            if destination.exists(): destination.unlink()
        else:
            assert sha(BACKUP / name) == original, f'Backup hash mismatch: {name}'
            shutil.copy2(BACKUP / name, destination)
            assert sha(destination) == original, f'Restore mismatch: {name}'
    manifest['restored'] = True
    manifest_path.write_text(json.dumps(manifest, indent=2), encoding='utf-8')
    print(json.dumps({'restored': True, 'verified_files': len(manifest['originals'])}))
