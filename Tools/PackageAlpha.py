"""Package the successful Windows Alpha 0.3 or Beta 0.4 and verify its checksums."""
from pathlib import Path
from zipfile import ZipFile, ZIP_DEFLATED
from hashlib import sha256
import json
import shutil
import subprocess
import argparse

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
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--beta', action='store_true', help='Package Beta 0.4 instead of Alpha 0.3.')
    beta = parser.parse_args().beta
    name = 'jogo_varginha_beta_0.4' if beta else NAME
    folder = ROOT/'Builds'/name
    docs = ROOT/'Docs/Beta04' if beta else DOCS
    report_path = ROOT/'Logs'/('BetaBuildReport.json' if beta else 'AlphaBuildReport.json')
    label = 'BETA 0.4' if beta else 'ALPHA 0.3'
    report = json.loads(report_path.read_text(encoding='utf-8-sig'))
    exe = folder/(name+'.exe')
    assembly = folder/(name+'_Data')/'Managed/Game.dll'
    if report['result'] != 'Succeeded' or Path(report['output']).resolve() != exe.resolve():
        raise RuntimeError('The report does not describe a successful current '+label+' build.')
    if report['errors']:
        if beta: raise RuntimeError('Build contains errors; resolve them before packaging Beta 0.4.')
        diagnostics = json.loads((docs/'build-diagnostics.json').read_text(encoding='utf-8-sig'))
        expected = 'Failed to handle /api/exec request: Main thread operation timed out after 5000ms'
        if len(diagnostics['errors']) != report['errors'] or any(error != expected for error in diagnostics['errors']):
            raise RuntimeError('Unreviewed error in the current build report.')
    if not exe.is_file() or not assembly.is_file(): raise RuntimeError('Missing player or game assembly.')
    docs.mkdir(parents=True,exist_ok=True)
    (folder/'LEIA_ME.txt').write_text(
        'O SEGREDO DE VARGINHA - '+label+'\n\nWindows 64 bits.\n'
        f'Extraia todo o ZIP e abra {name}.exe.\n'
        'Mantenha a pasta _Data e todas as bibliotecas junto do executavel.\n'
        'Unity Editor nao e necessario.\n\n'
        '14 fases, com areas adicionais da campanha.\n'
        + ('Inclui laboratorio moderno, menu cinematografico, fusca azul, hover nos botoes e selo de retorno da criatura.\n' if beta
           else 'Inclui revisao do casarao, HUD, puzzles, atmosfera e recortes dos personagens.\n') +
        'Use Continuar para retomar o progresso. Iniciar campanha substitui o save local.\n',encoding='utf-8')
    archive = ROOT/'Builds'/(name+'_Windows_x64.zip')
    files = sorted(p for p in folder.rglob('*') if p.is_file() and not any(
        'BackUpThisFolder_ButDontShipItWithYourGame' in part for part in p.parts))
    with ZipFile(archive,'w',compression=ZIP_DEFLATED,compresslevel=6) as package:
        for file in files: package.write(file,file.relative_to(folder).as_posix())
    with ZipFile(archive) as package:
        if package.testzip() is not None: raise RuntimeError('ZIP CRC verification failed.')
        assert name+'.exe' in package.namelist()
        assert name+'_Data/Managed/Game.dll' in package.namelist()
        assert sha256(package.read(name+'.exe')).hexdigest() == digest(exe)
    shutil.copy2(report_path,docs/'build-results.json')
    manifest = {'version':'0.4.0-beta' if beta else '0.3.0','channel':'beta' if beta else 'alpha','platform':'Windows x64',
        'base_commit':subprocess.check_output(['git','rev-parse','HEAD'],cwd=ROOT,text=True).strip(),
        'source_state':'Correcoes, versao e registro desta build incluidos no mesmo commit de entrega.',
        'build':report,'package':{'path':archive.relative_to(ROOT).as_posix(),'bytes':archive.stat().st_size,
        'files':len(files),'sha256':digest(archive),'crc_verified':True},
        'exe_sha256':digest(exe),'game_assembly_sha256':digest(assembly)}
    (docs/'build-manifest.json').write_text(json.dumps(manifest,ensure_ascii=False,indent=2)+'\n',encoding='utf-8')
    print(json.dumps(manifest['package'],ensure_ascii=False))

if __name__ == '__main__': main()
