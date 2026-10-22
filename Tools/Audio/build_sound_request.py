"""Export the reviewed sound brief as shareable Markdown, CSV and short message."""
import csv
import json
import shutil
from collections import Counter
from pathlib import Path

ROOT=Path(__file__).resolve().parents[2]
SOURCE=ROOT/'Docs/CampanhaOficial/PEDIDO_EFEITOS_SONOROS_2026-10-22.md'
OUT=ROOT/'output/audio-request-2026-10-22'

def main():
    text=SOURCE.read_text(encoding='utf-8')
    rows=[[cell.strip().replace('`','') for cell in line.strip().strip('|').split('|')]
          for line in text.splitlines() if line.startswith(('| P1 |','| P2 |','| P3 |'))]
    assert len(rows)==28 and all(len(row)==6 for row in rows)
    OUT.mkdir(parents=True,exist_ok=True)
    shutil.copy2(SOURCE,OUT/'Pedido_Efeitos_Sonoros.md')
    with (OUT/'Pedido_Efeitos_Sonoros.csv').open('w',newline='',encoding='utf-8-sig') as stream:
        writer=csv.writer(stream,delimiter=';')
        writer.writerow(['Prioridade','Arquivos sugeridos','Cena e gatilho','Direção sonora e entrega','Situação','Evento atual'])
        writer.writerows(rows)
    message=text.split('## Mensagem pronta para encaminhar à equipe\n\n')[1].split('\n\n##')[0]
    (OUT/'Mensagem_Para_Equipe.txt').write_text(message+'\n\nA lista completa e as especificações estão no Markdown e CSV anexos.\n',encoding='utf-8')
    catalog=json.loads((ROOT/'Assets/Resources/Varginha/Audio/Received20261021/Catalog.json').read_text())
    qa=dict(families=len(rows),priorities=dict(Counter(row[0] for row in rows)),
            receivedIds=len(catalog['entries']),receivedWavs=len({item['path'] for item in catalog['entries']}),
            dubbingPlanned='2026-10-23',status='Production request only; no new audio integrated')
    assert qa['receivedIds']==29 and qa['receivedWavs']==25
    (OUT/'brief-qa.json').write_text(json.dumps(qa,ensure_ascii=False,indent=2)+'\n',encoding='utf-8')
    print(json.dumps(qa,ensure_ascii=False))

if __name__=='__main__':main()
