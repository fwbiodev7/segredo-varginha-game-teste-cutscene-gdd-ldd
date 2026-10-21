"""Report latest per-case results across a complete run and explicit fixture reruns."""
import json
from pathlib import Path
import xml.etree.ElementTree as ET

FOLDER=Path('output/qa-2026-10-21')
RUNS=['playmode-all.xml','playmode-regression-fixed.xml','playmode-component-final.xml','playmode-light-final.xml']
ALIASES={
    'AdultHouseHygieneRespondsToKeyboardAndUnlocksTheBox':'AdultHouseHygieneRespondsToKeyboardAndPreservesInvestigationGate',
    'CoveredOfficeSofaBlocksRealMovementFromEveryAccessibleSide':'CoveredLivingRoomSofaBlocksRealMovementFromEveryAccessibleSide',
    'AnnaRicochetsAcrossTwoNewTargetsWithoutRepeatingDamage':'AnnaFirstRacketShotHitsOnlyItsAim',
    'KatanaPiercesOnlyTargetsBehindItsAim':'FirstKatanaCutHitsOnlyItsAim',
    'MarcosSpikesOnceAndSplashRespectsRange':'MarcosFirstServeHitsOnceWithoutSplash',
}

def main():
    latest={};runs=[];expected=None
    for name in RUNS:
        root=ET.parse(FOLDER/name).getroot();cases=list(root.iter('test-case'))
        if not cases or not root.get('end-time') or len(cases)!=int(root.get('total','-1')):
            raise ValueError('Missing/incomplete NUnit execution: '+name)
        keys=set()
        for case in cases:
            key=case.get('fullname')
            for old,new in ALIASES.items():
                if key.endswith('.'+old):key=key[:-len(old)]+new
            keys.add(key);latest[key]={'name':key,'result':case.get('result'),'source':name}
        if expected is None:expected=keys
        elif not keys.issubset(expected):raise ValueError('Unexpected extra tests; review aliases before merging.')
        runs.append({'source':name,'result':root.get('result'),'total':len(cases),'passed':int(root.get('passed','0')),'failed':int(root.get('failed','0'))})
    counts={'total':len(latest),'passed':0,'failed':0,'skipped':0}
    for item in latest.values():
        state=item['result'].lower();counts['passed' if state=='passed' else 'failed' if state.startswith(('failed','error')) else 'skipped']+=1
    result={'success':bool(latest) and set(latest)==expected and counts['failed']==0 and counts['skipped']==0,
            'mode':'Latest result per case, from full suite plus affected-fixture reruns; not a single green full-suite execution.',
            'tests':counts,'runs':runs,'renamedContracts':ALIASES,'cases':sorted(latest.values(),key=lambda x:x['name'])}
    (FOLDER/'playmode-coverage.json').write_text(json.dumps(result,indent=2,ensure_ascii=False),encoding='utf-8')
    print(json.dumps({k:v for k,v in result.items() if k not in ('cases','renamedContracts')},ensure_ascii=False))
    raise SystemExit(0 if result['success'] else 1)

if __name__=='__main__':main()
