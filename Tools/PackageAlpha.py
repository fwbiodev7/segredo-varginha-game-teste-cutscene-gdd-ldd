"""Package the current successful Windows alpha and record its verified checksums."""
from pathlib import Path
from zipfile import ZipFile, ZIP_DEFLATED
from hashlib import sha256
import json
import shutil
import subprocess

ROOT = Path(__file__).resolve().parents[1]
VERSION = '0.3'
NAME = f'jogo_varginha_alpha_{VERSION}'
FOLDER = ROOT/'Builds'/NAME
DOCS = ROOT/'Docs/Alpha03'

def digest(path):
    with path.open('rb') as stream:
        result = sha256()
        for block in iter(lambda: stream.read(1024*1024), b''): result.update(block)
    return result.hexdigest()

def main():
    report = json.loads((ROOT/'Logs/AlphaBuildReport.json').read_text(encoding='utf-8-sig'))
    exe = FOLDER/(NAME+'.exe')
    assembly = FOLDER/(NAME+'_Data')/'Managed/Game.dll'
    if report['result'] != 'Succeeded' or Path(report['output']).resolve() != exe.resolve():
        raise RuntimeError('The report does not describe a successful current Alpha 0.3 build.')
    if report['errors']:
        diagnostics = json.loads((DOCS/'build-diagnostics.json').read_text(encoding='utf-8-sig'))
        expected = 'Failed to handle /api/exec request: Main thread operation timed out after 5000ms'
        if len(diagnostics['errors']) != report['errors'] or any(error != expected for error in diagnostics['errors']):
            raise RuntimeError('Unreviewed error in the current build report.')
    if not exe.is_file() or not assembly.is_file(): raise RuntimeError('Missing player or game assembly.')
    DOCS.mkdir(parents=True,exist_ok=True)
    (FOLDER/'LEIA_ME.txt').write_text(
        'O SEGREDO DE VARGINHA - ALPHA 0.3\n\nWindows 64 bits.\n'
        f'Extraia todo o ZIP e abra {NAME}.exe.\n'
        'Mantenha a pasta _Data e todas as bibliotecas junto do executavel.\n'
        'Unity Editor nao e necessario.\n\n'
        '14 fases, com areas adicionais da campanha.\n'
        'Inclui revisao do casarao, HUD, puzzles, atmosfera e recortes dos personagens.\n'
        'Use Continuar para retomar o progresso. Iniciar campanha substitui o save local.\n',encoding='utf-8')
    archive = ROOT/'Builds'/(NAME+'_Windows_x64.zip')
    files = sorted(p for p in FOLDER.rglob('*') if p.is_file() and not any(
        'BackUpThisFolder_ButDontShipItWithYourGame' in part for part in p.parts))
    with ZipFile(archive,'w',compression=ZIP_DEFLATED,compresslevel=6) as package:
        for file in files: package.write(file,file.relative_to(FOLDER).as_posix())
    with ZipFile(archive) as package:
        if package.testzip() is not None: raise RuntimeError('ZIP CRC verification failed.')
        assert NAME+'.exe' in package.namelist()
        assert NAME+'_Data/Managed/Game.dll' in package.namelist()
        assert sha256(package.read(NAME+'.exe')).hexdigest() == digest(exe)
    shutil.copy2(ROOT/'Logs/AlphaBuildReport.json',DOCS/'build-results.json')
    manifest = {'version':'0.3.0','channel':'alpha','platform':'Windows x64',
        'base_commit':subprocess.check_output(['git','rev-parse','HEAD'],cwd=ROOT,text=True).strip(),
        'source_state':'Correcoes, versao e registro desta build incluidos no mesmo commit de entrega.',
        'build':report,'package':{'path':archive.relative_to(ROOT).as_posix(),'bytes':archive.stat().st_size,
        'files':len(files),'sha256':digest(archive),'crc_verified':True},
        'exe_sha256':digest(exe),'game_assembly_sha256':digest(assembly)}
    (DOCS/'build-manifest.json').write_text(json.dumps(manifest,ensure_ascii=False,indent=2)+'\n',encoding='utf-8')
    print(json.dumps(manifest['package'],ensure_ascii=False))

if __name__ == '__main__': main()
