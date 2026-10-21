// Exercise the skill's actual browser preview clock, including one-shot final holds.
import fs from 'node:fs/promises';
import path from 'node:path';
import {pathToFileURL} from 'node:url';
import {createRequire} from 'node:module';
import {createHash} from 'node:crypto';
const require=createRequire('C:/Users/Usuario/.cache/codex-runtimes/codex-primary-runtime/dependencies/node/node_modules/package.json');
const {chromium}=require('playwright');
const folder=path.resolve(process.argv[2]||'output/qa-2026-10-21/art/preview');
const browser=await chromium.launch({headless:true,channel:'chrome'});
const page=await browser.newPage({viewport:{width:1100,height:850}});
const errors=[];page.on('pageerror',e=>errors.push(e.message));
const checks=[];
try {
    await page.goto(pathToFileURL(path.join(folder,'index.html')).href);
    await page.waitForFunction(()=>document.querySelector('#view').width>0);
    for(const [index,first,last,delay] of [[0,'key_place','key_rest',550],[1,'box_closed','box_open',700]]) {
        await page.locator('#clips').selectOption(String(index));
        await page.locator('#reset').click();
        const before=await page.locator('#status').innerText();
        await page.locator('#view').screenshot({path:path.join(folder,`playback-${index}-start.png`)});
        await page.waitForTimeout(delay);
        const after=await page.locator('#status').innerText();
        const ending=await page.locator('#view').screenshot({path:path.join(folder,`playback-${index}-end.png`)});
        await page.waitForTimeout(1800);
        const held=await page.locator('#view').screenshot();
        const hash=b=>createHash('sha256').update(b).digest('hex');
        checks.push({clip:index,before,after,firstFrame:before.includes(first),lastFrame:after.includes(last),finalHold:hash(held)===hash(ending)});
    }
    const report={success:errors.length===0&&checks.every(c=>c.firstFrame&&c.lastFrame&&c.finalHold),checks,errors,
        scope:'Browser preview timing and pixel-art registration; cinematic integration is covered separately by Unity PlayMode/Player tests.'};
    await fs.writeFile(path.join(folder,'playback.json'),JSON.stringify(report,null,2));
    console.log(JSON.stringify(report));
    if(!report.success)process.exitCode=1;
} finally {await browser.close();}
