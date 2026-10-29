"""Package only a successful candidate, with checksums and the external-playtest guide."""
import argparse
import hashlib
import json
from pathlib import Path
import shutil
import zipfile

ROOT=Path(__file__).resolve().parents[2]
QA=ROOT/'output/qa-2026-10-21'

def sha(path):
    h=hashlib.sha256()
    with path.open('rb') as stream:
        for block in iter(lambda:stream.read(1024*1024),b''):h.update(block)
    return h.hexdigest()

def main():
    parser=argparse.ArgumentParser();parser.add_argument('--qa',type=Path,default=QA)
    parser.add_argument('--archive-suffix',default='');args=parser.parse_args()
    if any(c not in 'abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789_-' for c in args.archive_suffix):
        raise ValueError('Archive suffix must be a simple filename suffix.')
    qa=args.qa.resolve()
    if not qa.is_relative_to(ROOT/'output'):raise ValueError('QA evidence must stay in the project output folder.')
    report=json.loads((qa/'windows-build.json').read_text(encoding='utf-8'))
    if report['result']!='Succeeded' or report['errors']!=0:raise ValueError('No successful Windows build.')
    exe=Path(report['output']).resolve();folder=exe.parent
    if not folder.is_relative_to(ROOT/'Builds') or not exe.is_file():raise ValueError('Unexpected/missing build output.')
    archive=folder.parent/(folder.name+'_Windows_x64'+args.archive_suffix+'.zip')
    if archive.exists():raise FileExistsError(archive)
    guides={'0.5.3-beta':'ENTREGA_REVISAO_2026-10-29.md','0.5.4-beta':'ENTREGA_TORO_CASA_2026-10-29.md'}
    guide=ROOT/'Docs/CampanhaOficial'/guides.get(report['version'],'PLAYTEST_2026-10-21.md')
    shutil.copyfile(guide,folder/'LEIA-ME_PLAYTEST.md')
    if report['version'] in guides and exe.name!='Oizlede - Segredo de Varginha.exe':
        raise ValueError('Unexpected executable name for the Oizlede revision.')
    if not (folder/(exe.stem+'_Data')).is_dir():raise ValueError('Missing matching Unity data folder.')
    if report['version']=='0.4.4-beta':
        shutil.copyfile(ROOT/'Docs/CampanhaOficial/CREDITOS_AUDIO_RECEBIDO_2026-10-21.md',folder/'CREDITOS_AUDIO.md')
    elif report['version'] in guides:
        shutil.copyfile(ROOT/'Docs/CampanhaOficial/CREDITOS_AUDIO_REVISAO_2026-10-29.md',folder/'CREDITOS_AUDIO.md')
    def ship(p):
        return not any(part.endswith(('_BackUpThisFolder_ButDontShipItWithYourGame','_BurstDebugInformation_DoNotShip')) for part in p.relative_to(folder).parts)
    files=[p for p in sorted(folder.rglob('*')) if p.is_file() and p.name!='MANIFEST_SHA256.json' and ship(p)]
    if any(p.name in ('GamePlayModeTests.dll','GameEditModeTests.dll') for p in files):raise ValueError('Test assemblies must not enter the distribution package.')
    manifest={'version':report['version'],'unity':report['unity'],'excluded':'Unity backup/debug folders explicitly labelled DoNotShip',
              'files':[{'path':p.relative_to(folder).as_posix(),'bytes':p.stat().st_size,'sha256':sha(p)} for p in files]}
    (folder/'MANIFEST_SHA256.json').write_text(json.dumps(manifest,indent=2),encoding='utf-8')
    with zipfile.ZipFile(archive,'x',zipfile.ZIP_DEFLATED,compresslevel=6) as package:
        for p in files+[folder/'MANIFEST_SHA256.json']:package.write(p,Path(folder.name)/p.relative_to(folder))
    with zipfile.ZipFile(archive) as package:
        bad=package.testzip()
        if bad:raise ValueError('Corrupt package entry: '+bad)
    result={'success':True,'archive':str(archive),'bytes':archive.stat().st_size,'sha256':sha(archive),'files':len(files)+1,'build':str(exe),'version':report['version']}
    (qa/'windows-package.json').write_text(json.dumps(result,indent=2),encoding='utf-8')
    print(json.dumps(result))

if __name__=='__main__':main()
