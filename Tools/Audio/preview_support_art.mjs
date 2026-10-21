import fs from 'node:fs/promises';import path from 'node:path';import{createRequire}from'node:module';import{pathToFileURL}from'node:url';
const require=createRequire('C:/Users/Usuario/.cache/codex-runtimes/codex-primary-runtime/dependencies/node/node_modules/package.json');
const{chromium}=require('playwright');const folder=path.resolve('output/audio-integration-2026-10-21/art/preview-final');
const browser=await chromium.launch({headless:true,channel:'chrome'});const page=await browser.newPage({viewport:{width:1000,height:900}});const errors=[],checks=[];page.on('pageerror',e=>errors.push(e.message));
try{
 await page.goto(pathToFileURL(path.join(folder,'index.html')).href);await page.waitForFunction(()=>document.querySelector('#view').width>0);
 const clips=JSON.parse(await fs.readFile(path.join(folder,'preview-index.json'),'utf8')).clips;
 for(let i=0;i<clips.length;i++){
  await page.locator('#clips').selectOption(String(i));await page.locator('#reset').click();const statuses=new Set();
  for(let n=0;n<50;n++){statuses.add(await page.locator('#status').innerText());await page.waitForTimeout(75);}
  await page.locator('#view').screenshot({path:path.join(folder,`playback-${i}.png`)});
  checks.push({clip:clips[i].clip,distinctFrames:statuses.size,expected:new Set(clips[i].sequence).size,success:statuses.size===new Set(clips[i].sequence).size});
 }
 const report={success:checks.every(c=>c.success)&&!errors.length,checks,errors,scope:'Source-frame preview clock and registration; runtime effects and audio are verified separately in Unity Player.'};
 await fs.writeFile(path.join(folder,'playback-qa.json'),JSON.stringify(report,null,2));console.log(JSON.stringify(report));if(!report.success)process.exitCode=1;
}finally{await browser.close();}
