from pathlib import Path
import hashlib
import json
import shutil
import struct
import subprocess
import zipfile
from datetime import datetime, timezone

ROOT=Path(__file__).resolve().parents[2]
QA=Path.home()/'AppData/Local/Temp/varginha-master-qa-20261023-120709'
EVIDENCE=ROOT/'output/completion-2026-10-27'
BUILD=ROOT/'Builds/Varginha_0.4.9-beta_2026-10-27'
def sha(path):
    h=hashlib.sha256()
    with path.open('rb') as stream:
        for block in iter(lambda:stream.read(1024*1024),b''):h.update(block)
    return h.hexdigest()

report=json.loads((EVIDENCE/'windows-build.json').read_text(encoding='utf-8-sig'))
assert report['result']=='Succeeded' and report['errors']==0 and report['version']=='0.4.9-beta'
assert Path(report['output']).resolve()==(BUILD/'SegredoDeVarginha.exe').resolve()
assert len(report['scenes'])==19
exe=BUILD/'SegredoDeVarginha.exe'
with exe.open('rb') as stream:
    assert stream.read(2)==b'MZ'
    stream.seek(0x3c);pe=struct.unpack('<I',stream.read(4))[0]
    stream.seek(pe);assert stream.read(4)==b'PE\0\0';assert struct.unpack('<H',stream.read(2))[0]==0x8664
assert (BUILD/'SegredoDeVarginha_Data/Managed/Game.dll').stat().st_size>0
assert (BUILD/'UnityPlayer.dll').stat().st_size>0

sources=list((ROOT/'Assets/Scripts').rglob('*.cs'))+list((ROOT/'Assets/Resources').rglob('*.shader'))
sources += [ROOT/scene for scene in report['scenes']]
sources += [ROOT/'Assets/Resources/Varginha/IllustratedMaps/Layouts.json']
sources += list((ROOT/'Assets/Resources/Varginha/StoryCharacters/Directional20261026').glob('*.json'))
checked=[]
for source in sorted(set(sources)):
    relative=source.relative_to(ROOT);actual=QA/relative
    expected=sha(source)
    assert actual.exists() and sha(actual)==expected, f'QA source mismatch: {relative}'
    checked.append({'path':relative.as_posix(),'sha256':expected})

tests=[]
for name in ['final-editmode','final-player-regression','chapter-exits-final','final-return-and-hud']:
    result=json.loads((EVIDENCE/(name+'-summary.json')).read_text(encoding='utf-8-sig'))
    assert result['success'] and result['tests']['total']>0 and result['tests']['passed']==result['tests']['total']
    tests.append({'name':name,'counts':result['tests'],'xml_sha256':sha(EVIDENCE/(name+'.xml'))})
manifest={'version':report['version'],'created_utc':datetime.now(timezone.utc).isoformat(),
          'git_base':subprocess.check_output(['git','rev-parse','HEAD'],cwd=ROOT,text=True).strip(),
          'includes_local_completion_changes':True,'source_files_verified_against_qa':checked,
          'qa_urp_from_versioned_baseline':sha(QA/'Assets/UniversalRenderPipelineGlobalSettings.asset'),
          'tests':tests,'scenes':report['scenes']}
(BUILD/'source-manifest.json').write_text(json.dumps(manifest,ensure_ascii=False,indent=2),encoding='utf-8')
shutil.copy2(EVIDENCE/'windows-build.json',BUILD/'windows-build.json')
(BUILD/'LEIA-ME.txt').write_text('''O SEGREDO DE VARGINHA - 0.4.9-beta - Windows x64 - 27/10/2026

Extraia TODO o ZIP e abra SegredoDeVarginha.exe.
Mantenha a pasta SegredoDeVarginha_Data, MonoBleedingEdge e as bibliotecas junto do executável.
Não é necessário instalar o Unity.

Use Continuar para retomar o progresso. Nova História substitui o save atual.
Os comandos podem ser consultados em Configurações > Controles.

Esta candidata reúne as correções de 26/10 e o fechamento de 27/10:
piso do notebook, sprites completos, papéis da fase 2, saídas sem avanço duplo,
correção de sobreposição do HUD na chegada à batalha e recursos da campanha.
São 14 capítulos e 19 cenas, incluindo menu e áreas internas.

Os testes automatizados e as capturas não substituem playtest externo ou teste em outro PC.
Arquivos de fontes, evidências e logs não são necessários para jogar.
Os manifestos acompanham a build para identificar a versão e a origem dos arquivos.
''',encoding='utf-8')

files=[p for p in BUILD.rglob('*') if p.is_file() and not any('DoNotShip' in part or 'BackUpThisFolder' in part for part in p.parts)]
archive=BUILD.with_name(BUILD.name+'_Windows_x64.zip')
assert not archive.exists(), f'Existing package preserved: {archive}'
with zipfile.ZipFile(archive,'x',compression=zipfile.ZIP_DEFLATED,compresslevel=6) as package:
    for file in sorted(files):package.write(file,Path(BUILD.name)/file.relative_to(BUILD))
with zipfile.ZipFile(archive) as package:
    assert package.testzip() is None
    assert len(package.infolist())==len(files)
    for critical in ['SegredoDeVarginha.exe','UnityPlayer.dll','SegredoDeVarginha_Data/Managed/Game.dll','source-manifest.json']:
        assert hashlib.sha256(package.read(BUILD.name+'/'+critical)).hexdigest()==sha(BUILD/critical)
result={'success':True,'version':report['version'],'archive':str(archive),'bytes':archive.stat().st_size,
        'sha256':sha(archive),'files':len(files),'scenes':len(report['scenes']),
        'source_files_verified':len(checked),'zip_crc_verified':True,'critical_hashes_verified':True}
(EVIDENCE/'windows-package.json').write_text(json.dumps(result,indent=2),encoding='utf-8')
print(json.dumps(result))
