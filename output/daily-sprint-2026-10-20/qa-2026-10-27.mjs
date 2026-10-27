import fs from 'node:fs/promises';
import path from 'node:path';
import {createRequire} from 'node:module';
const require=createRequire('C:/Users/Usuario/.cache/codex-runtimes/codex-primary-runtime/dependencies/node/node_modules/package.json');
const {chromium}=require('playwright');
const root=process.cwd(),out=path.join(root,'output/daily-sprint-2026-10-20');
const qa=path.join(out,'.build/2026-10-27');await fs.mkdir(qa,{recursive:true});
const browser=await chromium.launch({headless:true,channel:'chrome'});
let page=await browser.newPage({viewport:{width:1280,height:720},deviceScaleFactor:1});
const errors=[];page.on('pageerror',e=>errors.push(e.message));
// Render the self-contained artifact in memory, without navigating to file://.
await page.setContent(await fs.readFile(path.join(out,'.build/before-2026-10-27/A_Sprint_Final_Varginha.html'),'utf8'));
await page.evaluate(()=>document.fonts.ready);
const reference=await page.locator('.slide').count();
for(let i=0;i<reference;i++){
 await page.evaluate(i=>window.deck.show(i,{animate:false}),i);
 await page.screenshot({path:path.join(qa,`reference-${String(i+1).padStart(2,'0')}.png`),animations:'disabled'});
}
await page.close();page=await browser.newPage({viewport:{width:1280,height:720},deviceScaleFactor:1});
page.on('pageerror',e=>errors.push(e.message));
await page.setContent(await fs.readFile(path.join(out,'A_Sprint_Final_Varginha.html'),'utf8'));
await page.evaluate(()=>document.fonts.ready);
const source=JSON.parse(await fs.readFile(path.join(out,'conteudo-2026-10-27.json'),'utf8'));
const text=await page.locator('#stage').textContent();
const report={date:'2026-10-27',slides:await page.locator('.slide').count(),referenceSlides:reference,errors,overflow:[],clipping:[]};
report.content={date:(await page.title()).includes('27/10/2026'),count:report.slides===10&&reference===10,
 version:text.includes('0.4.9-beta'),noPlaceholder:!text.includes('A CONFERIR'),
 notes:await page.evaluate(notes=>window.eval('notes').every((n,i)=>n===notes[i]),source.map(s=>s.notes)),
 historicalPictures:text.includes('Capturas de 26/10'),futurePresentation:text.includes('30/10'),
 font:await page.evaluate(()=>document.fonts.check('24px Pixel')),
 images:await page.locator('img').evaluateAll(a=>a.every(i=>i.complete&&i.naturalWidth>0))};
for(let i=0;i<report.slides;i++){
 await page.evaluate(i=>window.deck.show(i,{animate:false}),i);
 const overflow=await page.locator('.slide.on').evaluate(s=>[...s.querySelectorAll('h1,h2,h3,p,table,figure')].filter(e=>{const r=e.getBoundingClientRect();return r.bottom>690||r.right>1240||r.left<0}).map(e=>({tag:e.tagName,text:e.textContent.slice(0,80),bounds:e.getBoundingClientRect().toJSON()})));
 const clipping=await page.locator('.slide.on').evaluate(s=>[...s.querySelectorAll('h1,h2,h3,p,td,th,figcaption')].filter(e=>e.scrollWidth>e.clientWidth+1||e.scrollHeight>e.clientHeight+1).map(e=>({tag:e.tagName,text:e.textContent.slice(0,80)})));
 if(overflow.length)report.overflow.push({slide:i+1,elements:overflow});
 if(clipping.length)report.clipping.push({slide:i+1,elements:clipping});
 await page.screenshot({path:path.join(qa,`slide-${String(i+1).padStart(2,'0')}.png`),animations:'disabled'});
}
await page.evaluate(()=>window.deck.show(0,{animate:false}));await page.keyboard.press('ArrowRight');
report.keyboard=await page.locator('#count').textContent()==='2 / 10';
await page.locator('#notes').click();report.notesOpen=await page.locator('#note').isVisible();await page.keyboard.press('Escape');report.notesClose=!(await page.locator('#note').isVisible());
await page.waitForFunction(()=>!document.body.classList.contains('transitioning'));
await page.evaluate(()=>window.deck.show(0,{animate:false}));
report.transitions=[];
for(let i=0;i<9;i++){
 await page.keyboard.press('ArrowRight');await page.waitForTimeout(180);
 const effect=await page.evaluate(()=>document.getElementById('transition-fx').dataset.effect);
 await page.waitForFunction(()=>!document.body.classList.contains('transitioning'));
 report.transitions.push({effect,settled:await page.evaluate(()=>document.querySelectorAll('.slide.on').length===1&&document.querySelectorAll('.leaving').length===0)});
}
report.effects=report.transitions.every(t=>t.settled)&&new Set(report.transitions.map(t=>t.effect)).size===9;
await page.keyboard.press('End');report.end=await page.locator('#next').isDisabled();
await page.keyboard.press('m');await page.keyboard.press('Home');report.home=await page.locator('#prev').isDisabled();
report.motionOff=await page.evaluate(()=>!window.deck.motionEnabled&&!document.body.classList.contains('transitioning'));
await page.emulateMedia({media:'print'});
report.printSlides=await page.locator('.slide').evaluateAll(a=>a.filter(s=>getComputedStyle(s).display!=='none').length);
await fs.mkdir(path.join(root,'output/pdf'),{recursive:true});
const pdf=path.join(root,'output/pdf/Daily_Varginha_27-10-2026.pdf');
await page.pdf({path:pdf,preferCSSPageSize:true,printBackground:true});
const {execFileSync}=require('node:child_process');
report.pdfPages=Number(execFileSync('C:/Users/Usuario/.cache/codex-runtimes/codex-primary-runtime/dependencies/python/python.exe',['-c','import sys; from pypdf import PdfReader; print(len(PdfReader(sys.argv[1]).pages))',pdf],{encoding:'utf8'}).trim());
const pdfCheck=`import sys,json,re,unicodedata
from pypdf import PdfReader
def norm(s): return re.sub(r'[^a-z0-9]','',unicodedata.normalize('NFKD',s).encode('ascii','ignore').decode().lower())
r=PdfReader(sys.argv[1]); expected=json.load(open(sys.argv[2],encoding='utf-8'))
checks=[]
for i,p in enumerate(r.pages):
 t=p.extract_text() or ''; own=i<len(expected) and norm(expected[i]['title']) in norm(t)
 markers=re.findall(r'\\b[0-9]{2}\\s*/\\s*10\\s*$',t)
 checks.append({'page':i+1,'own_title':own,'one_footer':len(markers)==1,'nonempty':len(t)>100})
print(json.dumps(checks))`;
report.pdfContent=JSON.parse(execFileSync('C:/Users/Usuario/.cache/codex-runtimes/codex-primary-runtime/dependencies/python/python.exe',['-c',pdfCheck,pdf,path.join(out,'conteudo-2026-10-27.json')],{encoding:'utf8'}));
const reduced=await browser.newPage({viewport:{width:1280,height:720},reducedMotion:'reduce'});
await reduced.setContent(await fs.readFile(path.join(out,'A_Sprint_Final_Varginha.html'),'utf8'));
await reduced.keyboard.press('ArrowRight');report.reducedMotion=await reduced.evaluate(()=>!window.deck.motionEnabled&&!document.body.classList.contains('transitioning'));
await reduced.close();await browser.close();
report.failures=[];
for(const[k,v]of Object.entries(report.content))if(v!==true)report.failures.push('content_'+k);
for(const k of ['keyboard','notesOpen','notesClose','effects','end','home','motionOff','reducedMotion'])if(report[k]!==true)report.failures.push(k);
if(errors.length)report.failures.push('javascript');if(report.overflow.length)report.failures.push('overflow');if(report.clipping.length)report.failures.push('clipping');if(report.printSlides!==10)report.failures.push('print');
if(report.pdfPages!==10)report.failures.push('pdf_page_count');
if(!report.pdfContent.every(p=>p.own_title&&p.one_footer&&p.nonempty))report.failures.push('pdf_page_content');
report.success=report.failures.length===0;report.pdf=pdf;
await fs.writeFile(path.join(qa,'qa.json'),JSON.stringify(report,null,2));console.log(JSON.stringify(report));process.exitCode=report.success?0:1;
