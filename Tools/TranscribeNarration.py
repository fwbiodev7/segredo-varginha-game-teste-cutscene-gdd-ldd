from pathlib import Path
import sys,os,json
sys.path.insert(0,str(Path('.tools/stt').resolve()))
os.environ['HF_HUB_DISABLE_XET']='1'
from faster_whisper import WhisperModel
import wave,numpy as np
model=WhisperModel('base',device='cpu',compute_type='int8',cpu_threads=4,download_root='.tools/models')
rows=[]
for i in range(1,16):
 with wave.open(f'Assets/Resources/Varginha/Narration/Narration{i:02}.wav') as w: audio=np.frombuffer(w.readframes(w.getnframes()),dtype=np.int16).astype(np.float32)/32768
 segments,info=model.transcribe(audio,language='pt',beam_size=3,vad_filter=True)
 text=' '.join(s.text.strip() for s in segments)
 rows.append({'index':i,'text':text,'duration':info.duration})
 Path('ArtSources/Narration20261029/transcripts.json').write_text(json.dumps(rows,ensure_ascii=False,indent=2),encoding='utf-8')
 print(f'{i}: {text}',flush=True)
