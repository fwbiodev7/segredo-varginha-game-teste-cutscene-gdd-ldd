import fs from 'node:fs/promises';
import path from 'node:path';
import {pathToFileURL} from 'node:url';
import {createRequire} from 'node:module';
const require=createRequire('C:/Users/Usuario/.cache/codex-runtimes/codex-primary-runtime/dependencies/node/node_modules/package.json');
const {chromium}=require('playwright');const sharp=require('sharp');
const out=path.join(process.cwd(),'output/daily-sprint-2026-10-20');
const browser=await chromium.launch({headless:true,channel:'chrome'});
const page=await browser.newPage({viewport:{width:1280,height:720},deviceScaleFactor:1});
await page.goto(pathToFileURL('C:/Users/Usuario/Downloads/A Sprint Final.html').href);
await page.waitForTimeout(500);
const refs=await page.locator('.slide').count();
await fs.mkdir(path.join(out,'.build/reference'),{recursive:true});
for(let i=0;i<refs;i++){
 await page.evaluate(i=>{document.querySelectorAll('.slide').forEach((s,n)=>s.classList.toggle('on',n===i));document.getAnimations().forEach(a=>{try{a.finish()}catch{a.pause()}});},i);
 await page.screenshot({path:path.join(out,`.build/reference/ref-${i+1}.png`)});
}
const errors=[];page.on('pageerror',e=>errors.push(e.message));
await page.goto(pathToFileURL(path.join(out,'A_Sprint_Final_Varginha.html')).href);
await page.evaluate(()=>document.fonts.ready);
const report={slides:await page.locator('.slide').count(),referenceSlides:refs,font:await page.evaluate(()=>document.fonts.check('24px Pixel')),images:await page.locator('img').evaluateAll(a=>a.every(i=>i.complete&&i.naturalWidth>0)),errors,overflow:[]};
const thumbs=[];
for(let i=0;i<report.slides;i++){
 await page.evaluate(i=>window.deck.show(i,{animate:false}),i);
 const overflow=await page.locator('.slide.on').evaluate(s=>[...s.querySelectorAll('h1,h2,h3,p,table,figure')].filter(e=>{const r=e.getBoundingClientRect();return r.bottom>690||r.right>1240||r.left<0}).map(e=>({tag:e.tagName,text:e.textContent.slice(0,80),bounds:e.getBoundingClientRect().toJSON()})));
 if(overflow.length)report.overflow.push({slide:i+1,elements:overflow});
 const p=path.join(out,`.build/slide-${String(i+1).padStart(2,'0')}.png`);await page.screenshot({path:p});
 thumbs.push(await sharp(p).resize(384,216).png().toBuffer());
}
await page.evaluate(()=>window.deck.show(0,{animate:false}));await page.keyboard.press('ArrowRight');report.keyboard=await page.locator('#count').textContent()===('2 / '+report.slides);
await page.locator('#notes').click();report.notes=await page.locator('#note').isVisible();await page.keyboard.press('Escape');report.escape=!(await page.locator('#note').isVisible());
await sharp({create:{width:1152,height:Math.ceil(report.slides/3)*216,channels:3,background:'#020608'}}).composite(thumbs.map((input,i)=>({input,left:(i%3)*384,top:Math.floor(i/3)*216}))).png().toFile(path.join(out,'.build/overview.png'));
// Exercita a navegação real com cada efeito, inclusive interrupções rápidas.
report.transitions=[];
await page.evaluate(()=>window.deck.show(0,{animate:false}));
const variants=await page.evaluate(()=>window.deck.effectNames.length);
for(let i=1;i<=variants;i++){
 await page.keyboard.press('ArrowRight');await page.waitForTimeout(200);
 report.transitions.push(await page.evaluate(()=>({effect:document.getElementById('transition-fx').dataset.effect,active:document.body.classList.contains('transitioning'),animations:document.getAnimations().length,index:window.deck.index})));
 await page.screenshot({path:path.join(out,`.build/transition-${i}.png`),animations:'allow'});
 await page.waitForFunction(()=>!document.body.classList.contains('transitioning'));
 const stable=await page.evaluate(()=>({on:document.querySelectorAll('.slide.on').length,leaving:document.querySelectorAll('.leaving').length,clip:getComputedStyle(document.querySelector('.slide.on')).clipPath,transform:getComputedStyle(document.querySelector('.slide.on')).transform}));
 report.transitions.at(-1).settled=stable.on===1&&stable.leaving===0&&stable.clip==='none'&&stable.transform==='none';
}
report.allTransitions=new Set(report.transitions.map(t=>t.effect)).size===variants&&report.transitions.every(t=>t.active&&t.settled);
await page.evaluate(()=>window.deck.show(3,{animate:false}));
for(let i=0;i<7;i++)await page.keyboard.press('ArrowRight');
for(let i=0;i<5;i++)await page.keyboard.press('ArrowLeft');
await page.waitForFunction(()=>!document.body.classList.contains('transitioning'));
report.rapidNavigation=await page.evaluate(()=>({index:window.deck.index,active:document.querySelectorAll('.slide.on').length,leaving:document.querySelectorAll('.leaving').length,hidden:document.querySelectorAll('.slide[aria-hidden="true"]').length}));
await page.keyboard.press('End');await page.waitForFunction(()=>!document.body.classList.contains('transitioning'));
report.lastSlide=await page.locator('.slide.on > h2').innerText();report.endBoundary=await page.locator('#next').isDisabled();
await page.keyboard.press('m');await page.keyboard.press('Home');
report.effectsOff=await page.evaluate(()=>!window.deck.motionEnabled&&!document.body.classList.contains('transitioning')&&document.querySelectorAll('.leaving').length===0);
report.homeBoundary=await page.locator('#prev').isDisabled();
await page.keyboard.press('m');
await page.emulateMedia({media:'print'});
report.printSlides=await page.locator('.slide').evaluateAll(a=>a.filter(s=>getComputedStyle(s).display!=='none').length);
await page.emulateMedia({media:'screen'});
const reduced=await browser.newPage({viewport:{width:1280,height:720},reducedMotion:'reduce'});
await reduced.goto(pathToFileURL(path.join(out,'A_Sprint_Final_Varginha.html')).href);
await reduced.keyboard.press('ArrowRight');
report.reducedMotion=await reduced.evaluate(()=>!window.deck.motionEnabled&&!document.body.classList.contains('transitioning')&&document.getAnimations().length===0);
await reduced.close();
report.failures=[];
for(const key of ['font','images','keyboard','notes','escape','endBoundary','effectsOff','homeBoundary','reducedMotion','allTransitions'])if(report[key]!==true)report.failures.push(key);
if(report.overflow.length)report.failures.push('overflow');
if(errors.length)report.failures.push('javascript_errors');
if(report.rapidNavigation.index!==Math.max(0,Math.min(10,report.slides-1)-5)||report.rapidNavigation.active!==1||report.rapidNavigation.leaving!==0||report.rapidNavigation.hidden!==report.slides-1)report.failures.push('rapid_navigation');
if(report.printSlides!==report.slides)report.failures.push('print_slides');
report.success=report.failures.length===0;
await fs.writeFile(path.join(out,'.build/qa.json'),JSON.stringify(report,null,2));
console.log(JSON.stringify(report));await browser.close();process.exitCode=report.success?0:1;
