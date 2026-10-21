// Observe the actual sprite-skill preview clock; do not print embedded image data.
import fs from 'node:fs/promises';import path from 'node:path';import {pathToFileURL} from 'node:url';import {createRequire} from 'node:module';
const require=createRequire('C:/Users/Usuario/.cache/codex-runtimes/codex-primary-runtime/dependencies/node/node_modules/package.json');
const {chromium}=require('playwright');const root=path.resolve('output/visual-polish-2026-10-21');
const browser=await chromium.launch({headless:true,channel:'chrome'});const page=await browser.newPage({viewport:{width:1050,height:850}});
const errors=[];page.on('pageerror',e=>errors.push(e.message));
try{
 await page.goto(pathToFileURL(path.join(root,'art/skill-exports/child-preview/index.html')).href);
 await page.waitForFunction(()=>document.querySelector('#view').width>0);
 const statuses=new Set();for(let i=0;i<20;i++){statuses.add(await page.locator('#status').innerText());await page.waitForTimeout(80);}
 await page.locator('#view').screenshot({path:path.join(root,'art/child-playback-browser.png')});
 const result={success:statuses.size===8&&!errors.length,distinctFramesObserved:statuses.size,statuses:[...statuses],errors,
  scope:'8-frame source preview playback, not a measured game FPS benchmark; runtime captures are separate.'};
 await fs.writeFile(path.join(root,'art/playback.json'),JSON.stringify(result,null,2));console.log(JSON.stringify(result));if(!result.success)process.exitCode=1;
}finally{await browser.close();}
