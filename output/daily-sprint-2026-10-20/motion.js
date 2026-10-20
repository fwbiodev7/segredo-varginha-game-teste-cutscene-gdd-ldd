/* Conteúdo e controles continuam disponíveis durante a navegação rápida. */
let index=0;
const pages=[...document.querySelectorAll('.slide')];
const reducedMotion=matchMedia('(prefers-reduced-motion: reduce)');
const fx=document.getElementById('transition-fx');
const progress=document.getElementById('progress');
let enabled=!reducedMotion.matches,preference=null;
try{preference=localStorage.getItem('varginha-deck-motion');if(preference!==null)enabled=preference==='on'}catch{}
let activeAnimations=[],transitionId=0,transitionTimer=0;
const effectHistory=[];
const effectNames=['signal','pixels','portal','blinds','diagonal','diamond','doors','crt','film'];
function fit(){document.documentElement.style.setProperty('--scale',Math.min(innerWidth/1280,innerHeight/720))}
function animate(el,keyframes,options){
 const a=el.animate(keyframes,{fill:'both',...options});activeAnimations.push(a);return a;
}
function settle(){
 clearTimeout(transitionTimer);transitionId++;
 activeAnimations.forEach(a=>a.cancel());activeAnimations=[];
 pages.forEach(s=>s.classList.remove('leaving'));
 fx.classList.remove('active');fx.removeAttribute('data-effect');
 document.body.classList.remove('transitioning');
}
function updateMotion(){
 document.body.classList.toggle('motion-enabled',enabled);
 const button=document.getElementById('motion');
 button.textContent='Efeitos: '+(enabled?'ligados':'desligados');
 button.setAttribute('aria-pressed',String(enabled));
}
function toggleMotion(){settle();enabled=!enabled;preference=enabled?'on':'off';try{localStorage.setItem('varginha-deck-motion',preference)}catch{}updateMotion()}
function animateContent(slide){
 if(!enabled)return;
 const groups=[
  [slide.querySelector('.tag'),70],
  [slide.querySelector('h1, :scope > h2'),110],
  [slide.querySelector('.lead, .covertext h2'),175],
  ...[...slide.querySelectorAll('.items > div, table tr, .duo > div')].map((el,i)=>[el,220+i*70]),
  ...[...slide.querySelectorAll('.split figure, .widephoto figure')].map(el=>[el,220]),
  [slide.querySelector('.covertext > p'),280],
  [slide.querySelector('.bottom'),360]
 ];
 for(const[el,delay]of groups){if(!el)continue;
  animate(el,[{opacity:0,transform:'translateY(16px)'},{opacity:1,transform:'translateY(0)'}],{duration:490,delay,easing:'cubic-bezier(.16,1,.3,1)'});
 }
 const figure=slide.querySelector('.split figure, .widephoto figure');
 if(figure)animate(figure,[{clipPath:'inset(0 100% 0 0)'},{clipPath:'inset(0 0% 0 0)'}],{duration:650,delay:160,easing:'cubic-bezier(.2,.8,.2,1)'});
}
function show(n,options={}){
 const next=Math.max(0,Math.min(pages.length-1,n));
 const previous=index;
 const shouldAnimate=options.animate!==false&&enabled&&next!==previous;
 settle();index=next;
 pages.forEach((s,i)=>{s.classList.toggle('on',i===index);s.setAttribute('aria-hidden',i===index?'false':'true');s.inert=i!==index});
 document.getElementById('count').textContent=(index+1)+' / '+pages.length;
 document.getElementById('note').textContent=notes[index];
 document.getElementById('prev').disabled=index===0;
 document.getElementById('next').disabled=index===pages.length-1;
 progress.style.transform='scaleX('+((index+1)/pages.length)+')';
 history.replaceState(null,'','#'+(index+1));
 if(!shouldAnimate)return;
 const old=pages[previous],incoming=pages[index],direction=index>previous?1:-1;
 old.classList.add('leaving');fx.classList.add('active');document.body.classList.add('transitioning');
 const mode=index%effectNames.length,id=transitionId;
 const effect=effectNames[mode];
 fx.dataset.effect=effect;effectHistory.push(effect);
 if(effectHistory.length>24)effectHistory.shift();
 const fade=animate(old,[{opacity:1,transform:'scale(1)'},{opacity:0,transform:'scale(.98)'}],{duration:510,easing:'ease-in'});
 if(mode===0){
  animate(incoming,[{clipPath:'inset(0 '+(direction>0?'100%':'0')+' 0 '+(direction<0?'100%':'0')+')',transform:'translateX('+(direction*38)+'px)'},{clipPath:'inset(0 0 0 0)',transform:'translateX(0)'}],{duration:650,easing:'cubic-bezier(.2,.85,.2,1)'});
  [...fx.querySelectorAll('.signal-band')].forEach((el,i)=>animate(el,[{opacity:0,transform:'translateX('+(-direction*100)+'%)'},{opacity:.9,transform:'translateX(0)',offset:.35},{opacity:0,transform:'translateX('+(direction*100)+'%)'}],{duration:570,delay:i*45,easing:'steps(8,end)'}));
 }else if(mode===1){
  animate(incoming,[{opacity:0,transform:'translateX('+(direction*18)+'px)'},{opacity:1,transform:'translateX(0)'}],{duration:500,delay:140,easing:'cubic-bezier(.2,.85,.2,1)'});
  [...fx.querySelectorAll('.shutter i')].forEach((el,i)=>{
   const column=i%16,row=Math.floor(i/16),distance=direction>0?column:15-column;
   const delay=distance*15+((row*7+column*11)%5)*13;
   animate(el,[{opacity:0,transform:'scaleY(.08)'},{opacity:1,transform:'scaleY(1)',offset:.3},{opacity:1,transform:'scaleY(1)',offset:.56},{opacity:0,transform:'scaleY(.08)'}],{duration:390,delay,easing:'steps(4,end)'});
  });
 }else if(mode===2){
  animate(incoming,[{clipPath:'circle(0% at 62% 48%)',transform:'scale(1.045)'},{clipPath:'circle(125% at 62% 48%)',transform:'scale(1)'}],{duration:760,easing:'cubic-bezier(.22,.8,.22,1)'});
  animate(fx.querySelector('.aura'),[{opacity:0,transform:'scale(.3)'},{opacity:.8,offset:.4},{opacity:0,transform:'scale(1.3)'}],{duration:780,easing:'ease-out'});
 }else if(mode===3){
  animate(incoming,[{opacity:0,transform:'translateY(18px)'},{opacity:1,transform:'translateY(0)'}],{duration:620,delay:80,easing:'cubic-bezier(.2,.8,.2,1)'});
  [...fx.querySelectorAll('.shutter i')].forEach((el,i)=>{
   const column=i%16;el.style.transformOrigin=direction>0?'left':'right';
   animate(el,[{opacity:0,transform:'scaleX(0)'},{opacity:1,transform:'scaleX(1)',offset:.24},{opacity:1,transform:'scaleX(1)',offset:.5},{opacity:0,transform:'scaleX(0)'}],{duration:500,delay:(direction>0?column:15-column)*17,easing:'cubic-bezier(.65,0,.35,1)'});
  });
 }else if(mode===4){
  const start=direction>0?'polygon(0% 0%, 0% 0%, 0% 100%, 0% 100%)':'polygon(100% 0%, 100% 0%, 100% 100%, 100% 100%)';
  const mid=direction>0?'polygon(0% 0%, 72% 0%, 35% 100%, 0% 100%)':'polygon(28% 0%, 100% 0%, 100% 100%, 65% 100%)';
  animate(incoming,[{clipPath:start,transform:'translateX('+(direction*28)+'px)'},{clipPath:mid,offset:.48},{clipPath:'polygon(0% 0%, 100% 0%, 100% 100%, 0% 100%)',transform:'translateX(0)'}],{duration:770,easing:'cubic-bezier(.25,.8,.25,1)'});
  animate(fx.querySelector('.aura'),[{opacity:0,transform:'translateX('+(-direction*70)+'%) rotate(-20deg)'},{opacity:.6,offset:.4},{opacity:0,transform:'translateX('+(direction*70)+'%) rotate(-20deg)'}],{duration:770});
 }else if(mode===5){
  animate(incoming,[{clipPath:'polygon(50% 50%, 50% 50%, 50% 50%, 50% 50%)',transform:'scale(1.035)'},{clipPath:'polygon(50% -70%, 170% 50%, 50% 170%, -70% 50%)',transform:'scale(1)'}],{duration:820,easing:'cubic-bezier(.22,.8,.22,1)'});
  animate(fx.querySelector('.aura'),[{opacity:0,transform:'scale(.15) rotate(45deg)'},{opacity:.65,offset:.4},{opacity:0,transform:'scale(1.2) rotate(45deg)'}],{duration:820});
 }else if(mode===6){
  animate(incoming,[{opacity:0,transform:'perspective(1400px) scale(.94)'},{opacity:1,transform:'perspective(1400px) scale(1)'}],{duration:740,delay:90,easing:'cubic-bezier(.2,.8,.2,1)'});
  [...fx.querySelectorAll('.door')].forEach((el,i)=>animate(el,[{opacity:0,transform:'perspective(1100px) rotateY(0deg)'},{opacity:1,transform:'perspective(1100px) rotateY(0deg)',offset:.13},{opacity:0,transform:'perspective(1100px) rotateY('+(i===0?-95:95)+'deg)'}],{duration:760,easing:'cubic-bezier(.35,.1,.2,1)'}));
 }else if(mode===7){
  animate(incoming,[{opacity:0,transform:'scale(1,.012)'},{opacity:.75,transform:'scale(1,.012)',offset:.18},{opacity:1,transform:'scale(1,1)'}],{duration:640,delay:130,easing:'cubic-bezier(.2,.8,.2,1)'});
  animate(fx.querySelector('.crt-line'),[{opacity:0,transform:'scaleX(.1)'},{opacity:.6,transform:'scaleX(1)',offset:.22},{opacity:0,transform:'scaleX(.1)'}],{duration:540,easing:'ease-out'});
 }else{
  animate(incoming,[{opacity:.15,transform:'translateY('+(direction*720)+'px)'},{opacity:1,transform:'translateY(0)'}],{duration:680,easing:'cubic-bezier(.25,.8,.25,1)'});
  animate(old,[{transform:'translateY(0)',opacity:1},{transform:'translateY('+(-direction*720)+'px)',opacity:0}],{duration:680,easing:'cubic-bezier(.25,.8,.25,1)'});
  [...fx.querySelectorAll('.film-edge')].forEach(el=>animate(el,[{opacity:0,backgroundPositionY:'0px'},{opacity:.7,offset:.25},{opacity:0,backgroundPositionY:(-direction*180)+'px'}],{duration:740,easing:'steps(12,end)'}));
 }
 animate(fx.querySelector('.sweep'),[{opacity:0,transform:'translateY(0)'},{opacity:.55,offset:.2},{opacity:0,transform:'translateY(720px)'}],{duration:700,easing:'ease-out'});
 animateContent(incoming);
 fade.finished.then(()=>{if(id===transitionId)old.classList.remove('leaving')}).catch(()=>{});
 transitionTimer=setTimeout(()=>{if(id===transitionId)settle()},1250);
}
function full(){if(document.fullscreenElement)document.exitFullscreen();else document.documentElement.requestFullscreen().catch(()=>{})}
function toggleNotes(){document.getElementById('note').hidden=!document.getElementById('note').hidden}
document.querySelectorAll('h1,.slide > h2').forEach(el=>el.dataset.echo=[...el.childNodes].map(n=>n.nodeName==='BR'?'\n':n.textContent).join(''));
document.getElementById('prev').onclick=()=>show(index-1);
document.getElementById('next').onclick=()=>show(index+1);
document.getElementById('full').onclick=full;
document.getElementById('notes').onclick=toggleNotes;
document.getElementById('motion').onclick=toggleMotion;
addEventListener('keydown',e=>{
 if(e.ctrlKey||e.metaKey||e.altKey||e.target.matches('input,textarea,[contenteditable="true"]'))return;
 if(['ArrowRight',' ','PageDown'].includes(e.key)){e.preventDefault();show(index+1)}
 if(['ArrowLeft','PageUp'].includes(e.key)){e.preventDefault();show(index-1)}
 if(e.key==='Home')show(0);
 if(e.key==='End')show(pages.length-1);
 if(e.key.toLowerCase()==='f')full();
 if(e.key.toLowerCase()==='n')toggleNotes();
 if(e.key.toLowerCase()==='m')toggleMotion();
 if(e.key==='Escape')document.getElementById('note').hidden=true;
});
reducedMotion.addEventListener('change',()=>{if(preference===null){settle();enabled=!reducedMotion.matches;updateMotion()}});
addEventListener('resize',fit);
addEventListener('beforeprint',settle);
document.addEventListener('visibilitychange',()=>{if(document.hidden)settle()});
fit();updateMotion();show((Number(location.hash.slice(1))||1)-1,{animate:false});
document.fonts.ready.then(()=>{if(enabled)animateContent(pages[index])});
window.deck={show,slides:pages,settle,effectNames:[...effectNames],get index(){return index},get effects(){return [...effectHistory]},get motionEnabled(){return enabled}};
