"""Summarize NUnit XML without treating missing/empty/skipped runs as success."""
import argparse, json
from pathlib import Path
import xml.etree.ElementTree as ET

def main():
    p=argparse.ArgumentParser();p.add_argument('xml',type=Path);p.add_argument('--out',type=Path)
    a=p.parse_args();root=ET.parse(a.xml).getroot();cases=list(root.iter('test-case'))
    result={'source':str(a.xml),'runResult':root.get('result','Unknown'),'tests':{'total':len(cases),'passed':0,'failed':0,'skipped':0},'failures':[]}
    for case in cases:
        state=case.get('result','').lower()
        key='passed' if state in ('passed','success') else 'failed' if state in ('failed','failure','error') else 'skipped'
        result['tests'][key]+=1
        if key=='failed':result['failures'].append({'name':case.get('fullname',case.get('name')),'message':case.findtext('failure/message'),'stack':case.findtext('failure/stack-trace')})
    result['success']=root.get('result','').lower() in ('passed','success') and bool(cases) and result['tests']['failed']==0 and result['tests']['passed']>0
    if a.out:a.out.parent.mkdir(parents=True,exist_ok=True);a.out.write_text(json.dumps(result,indent=2,ensure_ascii=False),encoding='utf-8')
    console={k:v for k,v in result.items() if k!='failures'}
    console['failures']=[{'name':f['name'],'message':(f['message'] or '')[:500]} for f in result['failures']]
    if a.out:console['fullReport']=str(a.out)
    print(json.dumps(console,ensure_ascii=False))
    raise SystemExit(0 if result['success'] else 1)

if __name__=='__main__':main()
