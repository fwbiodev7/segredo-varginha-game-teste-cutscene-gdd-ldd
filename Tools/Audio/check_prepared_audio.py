"""Check prepared PCM recordings; this measures waveforms, not perceived mix quality."""
import array, hashlib, json, math, wave
from pathlib import Path

root=Path(__file__).resolve().parents[2]
folder=root/'Assets/Resources/Varginha/Audio/Received20261021'
entries=json.loads((folder/'Catalog.json').read_text())['entries']
rows=[]
for entry in entries:
    if entry.get('aliasOf'):continue
    path=folder/(entry['id']+'.wav')
    with wave.open(str(path),'rb') as clip:
        rate,channels,frames,width=clip.getframerate(),clip.getnchannels(),clip.getnframes(),clip.getsampwidth()
        samples=array.array('h',clip.readframes(frames))
    peak=max(abs(v) for v in samples)/32768
    rms=math.sqrt(sum((v/32768)**2 for v in samples)/len(samples))
    seam=max(abs(samples[c]-samples[-channels+c]) for c in range(channels))/32768
    valid=rate==44100 and width==2 and rms>1e-6 and peak<=.851 and abs(frames/rate-entry['seconds'])<.001
    valid=valid and hashlib.sha256(path.read_bytes()).hexdigest()==entry['sha256']
    if not entry['loop']:valid=valid and all(v==0 for v in samples[:channels]+samples[-channels:])
    rows.append(dict(id=entry['id'],channels=channels,seconds=frames/rate,peak=peak,rms=rms,loop=entry['loop'],seamJump=seam,success=valid))
result=dict(success=len(rows)==25 and all(r['success'] for r in rows),files=len(rows),clips=rows,scope='PCM integrity, non-silence, peak headroom and quiet effect endpoints. Loops use an overlap crossfade; seamJump is measured, not a perceptual judgement.')
out=root/'output/audio-integration-2026-10-21/waveform-qa.json'
out.write_text(json.dumps(result,ensure_ascii=False,indent=2),encoding='utf-8')
print(json.dumps(dict(success=result['success'],files=len(rows),highestPeak=max(r['peak'] for r in rows),failures=[r['id'] for r in rows if not r['success']],report=str(out))))
raise SystemExit(0 if result['success'] else 1)
