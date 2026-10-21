"""Guard against false approvals and accidental multi-megabyte console failures."""
import json
from pathlib import Path
import subprocess
import sys
import tempfile
import unittest

SCRIPT=Path(__file__).with_name('summarize_unity.py')

class SummaryTests(unittest.TestCase):
    def run_xml(self,result,cases):
        with tempfile.TemporaryDirectory() as folder:
            source=Path(folder)/'run.xml';report=Path(folder)/'run.json'
            source.write_text(f'<test-run result="{result}">{cases}</test-run>',encoding='utf-8')
            run=subprocess.run([sys.executable,str(SCRIPT),str(source),'--out',str(report)],capture_output=True,text=True,encoding='utf-8')
            return run,json.loads(run.stdout),json.loads(report.read_text(encoding='utf-8'))
    def test_completed_run_passes(self):
        run,brief,_=self.run_xml('Passed','<test-case result="Passed"/>')
        self.assertEqual(run.returncode,0);self.assertTrue(brief['success'])
    def test_suite_failure_cannot_hide_behind_passed_cases(self):
        run,brief,_=self.run_xml('Failed','<test-case result="Passed"/>')
        self.assertEqual(run.returncode,1);self.assertFalse(brief['success'])
    def test_empty_run_never_passes(self):
        run,_,_=self.run_xml('Passed','');self.assertEqual(run.returncode,1)
    def test_all_skipped_never_passes(self):
        run,_,_=self.run_xml('Passed','<test-case result="Skipped"/>');self.assertEqual(run.returncode,1)
    def test_unknown_completion_never_passes(self):
        run,_,_=self.run_xml('Unknown','<test-case result="Passed"/>');self.assertEqual(run.returncode,1)
    def test_long_failure_is_bounded_but_preserved(self):
        message='PIXEL '*10000
        run,brief,full=self.run_xml('Failed',f'<test-case fullname="Sprite" result="Failed"><failure><message>{message}</message></failure></test-case>')
        self.assertEqual(run.returncode,1);self.assertLess(len(run.stdout),1500)
        self.assertEqual(full['failures'][0]['message'],message)
        self.assertEqual(len(brief['failures'][0]['message']),500)

if __name__=='__main__':unittest.main()
