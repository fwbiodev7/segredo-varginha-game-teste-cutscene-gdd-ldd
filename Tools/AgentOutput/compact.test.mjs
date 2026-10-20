import test from 'node:test';
import assert from 'node:assert/strict';
import fs from 'node:fs/promises';
import os from 'node:os';
import path from 'node:path';
import {createServer} from 'node:http';
import {extract,run,localNote} from './compact.mjs';
test('extracts completed tests and preserves failures',()=>{
 assert.equal(extract(JSON.stringify({summary:{total:3,passed:3,failed:0,skipped:0}}),0).verification,'passed');
 const r=extract(JSON.stringify({summary:{total:2,passed:1,failed:1},results:[{FullName:'Puzzle.Save',Status:'Failed',Message:'Expected 4 got 0'}]}),0);
 assert.equal(r.verification,'failed');assert.equal(r.failures[0].test,'Puzzle.Save');
});
test('empty and running test suites never pass',()=>{
 assert.equal(extract('{"summary":{"total":0,"passed":0,"failed":0}}',0).verification,'no_tests');
 assert.equal(extract('{"summary":{"total":3,"passed":0,"failed":0,"skipped":3}}',0).verification,'no_tests');
 assert.equal(extract('{"status":"running","summary":{"total":3,"passed":3,"failed":0,"skipped":0}}',0).verification,'incomplete');
});
test('unknown process output does not prove test success',()=>assert.equal(extract('process started\nfinished',0).verification,'not_inferred'));
test('zero exit code does not hide structured failure',()=>assert.equal(extract('{"success":false,"errors":["navigation broken"]}',0).verification,'failed'));
test('success field cannot hide errors in a presentation report',()=>assert.equal(extract('{"slides":13,"success":true,"errors":["broken page"]}',0).verification,'failed'));
test('recognizes TAP counts and does not claim zero tests passed',()=>{
 assert.equal(extract('TAP version 13\n# tests 3\n# pass 3\n# fail 0\n# skipped 0',0).verification,'passed');
 assert.equal(extract('# tests 0\n# pass 0\n# fail 0',0).verification,'no_tests');
});
test('parses NUnit header and keeps nonzero exit failure',()=>{
 assert.equal(extract('<test-run result="Passed" total="10" passed="10" failed="0" skipped="0">',0).tests.total,10);
 assert.equal(extract('{"summary":{"total":1,"passed":1,"failed":0,"skipped":0}}',7).verification,'failed');
});
test('streams full logs and returns only useful failed output',async()=>{
 const dir=await fs.mkdtemp(path.join(os.tmpdir(),'varginha-compact-'));
 const r=await run(process.execPath,['-e','console.log("routine output\\n".repeat(10000));console.error("Error: failed save test");process.exit(7)'],{logDir:dir});
 assert.equal(r.exitCode,7);assert.equal(r.summary.verification,'failed');assert.ok(r.summary.failures.join(' ').includes('failed save test'));assert.ok((await fs.stat(r.summary.logs.stdout)).size>100000);assert.ok(JSON.stringify(r.summary).length<2000);assert.equal(r.summary.raw_sha256.stdout.length,64);
});
test('timeout and missing executable are explicit',async()=>{
 const dir=await fs.mkdtemp(path.join(os.tmpdir(),'varginha-compact-'));
 const r=await run(process.execPath,['-e','setInterval(()=>{},1000)'],{logDir:dir,timeoutMs:200});
 assert.equal(r.exitCode,124);assert.equal(r.summary.verification,'incomplete');
 const missing=await run('definitely-missing-varginha-command-5832',[],{logDir:dir});assert.equal(missing.exitCode,127);assert.equal(missing.summary.execution,'could_not_start');
});
test('large logs stay on disk and report bounded inspection',async()=>{
 const dir=await fs.mkdtemp(path.join(os.tmpdir(),'varginha-compact-'));
 const r=await run(process.execPath,['-e','console.log("routine\\n".repeat(400000));console.error("Error: final diagnostic");process.exit(2)'],{logDir:dir});
 assert.ok(r.summary.input_bytes>2*1024*1024);assert.ok(r.summary.inspection.includes('bounded_window'));assert.ok(r.summary.failures.join(' ').includes('final diagnostic'));assert.ok(r.raw.length<220000);
});
test('local model adapter accepts a bounded note and blocks remote endpoints',async()=>{
 const server=createServer((req,res)=>{req.resume();req.on('end',()=>{res.setHeader('Content-Type','application/json');res.end(JSON.stringify({choices:[{message:{content:'{"note":"One failed test: Save.Resume"}'}}]}))})});
 await new Promise(resolve=>server.listen(0,'127.0.0.1',resolve));
 try{const note=await localNote('FAIL Save.Resume','Which test failed?','http://127.0.0.1:'+server.address().port+'/v1/chat/completions','test-adapter');assert.equal(note,'One failed test: Save.Resume');await assert.rejects(localNote('','', 'https://example.com/v1/chat/completions','test'),/loopback/)}finally{await new Promise(resolve=>server.close(resolve))}
});
test('unavailable local model leaves the command exit code and failure intact',async()=>{
 const server=createServer((req,res)=>{req.resume();req.on('end',()=>{res.writeHead(503);res.end()})});
 await new Promise(resolve=>server.listen(0,'127.0.0.1',resolve));
 const dir=await fs.mkdtemp(path.join(os.tmpdir(),'varginha-compact-'));
 try{const r=await run(process.execPath,['-e','console.error("Error: broken save");process.exit(7)'],{logDir:dir});await assert.rejects(localNote(r.raw,'Why?','http://127.0.0.1:'+server.address().port+'/v1/chat/completions','test'),/503/);assert.equal(r.exitCode,7);assert.equal(r.summary.verification,'failed')}finally{await new Promise(resolve=>server.close(resolve))}
});
