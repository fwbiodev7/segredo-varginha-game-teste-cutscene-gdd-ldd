// Decode the received files locally in Chromium; no upload, playback or external codec install.
import fs from 'node:fs/promises';import path from 'node:path';import {createHash} from 'node:crypto';import {createRequire} from 'node:module';
const require=createRequire('C:/Users/Usuario/.cache/codex-runtimes/codex-primary-runtime/dependencies/node/node_modules/package.json');const {chromium}=require('playwright');
const root=process.cwd(),qa=path.join(root,'output/audio-integration-2026-10-21'),out=path.join(root,'Assets/Resources/Varginha/Audio/Received20261021');
const inventory=JSON.parse(await fs.readFile(path.join(qa,'archive-inventory.json'),'utf8'));await fs.mkdir(out,{recursive:true});
const plans=[
 ['PlayerPunch','mixkit-impact',.5],['EnemyAttack','yodguard-',1.1,.8],['BossRoar','yarzur_ofc-sfx11',2.5,.7],
 ['FootWood','dragon-studio-footsteps-on-wood',.34],['FootWood2','dragon-studio-footsteps-on-wood',.34],['FootWood3','dragon-studio-footsteps-on-wood',.34],
 ['FootNature','dragon-studio-footsteps-on-the-nature',.42],['FootNature2','dragon-studio-footsteps-on-the-nature',.42],['FootNature3','dragon-studio-footsteps-on-the-nature',.42],
 ['SchoolAmbience','freesound_community-loyola',30,1,true],['ForestAmbience','eryliaa-',32,1,true],
 ['TelevisionAmbience','arunangshubanerjee-',16,.55,true],['RadioInterference','freesound_community-068253',1.2,.5],
 ['RenanPulse','wayofx-',1.3,.8],['FabioShield','dragon-studio-angelical',6,.55,true],
 ['CarFail','350chevo-',3,.7],['Engine','magiaz-carro-velho',4,1,true],['Starter','magiaz-carro-velho',1.5,.7],
 ['DoorOpen','abrindo_portawav',1.5,.65],['ChurchBell','alex_jauk-',4,.45],['SiegeAlarm','lazychillzone-',4,.35],
 ['OuzanaGlass','magiaz-vidro',1.2,.65],['ManifestationTear','tutmozis-metal-crack-606770.mp3',1,.7],
 ['ManifestationCollapse','yarzur_ofc-sfx17',1.8,.7],['ChildExplosion','soundreality-',2.2,.65]
];
const browser=await chromium.launch({headless:true,channel:'chrome'}),page=await browser.newPage();await page.setContent('<!doctype html><title>Local audio preparation</title>');
const entries=[],sources=[];
try{
 for(const [id,prefix,maxSeconds,gain=1,loop=false] of plans){
  const source=inventory.entries.find(e=>path.basename(e.name).startsWith(prefix));if(!source)throw new Error('Missing source '+prefix);
  const raw=await fs.readFile(path.join(root,source.file)),hash=createHash('sha256').update(raw).digest('hex');
  const result=await page.evaluate(async({base64,key,id,maxSeconds,loop})=>{
   if(window.audioKey!==key){const context=new OfflineAudioContext(2,1,44100);window.audioBuffer=await context.decodeAudioData(Uint8Array.from(atob(base64),c=>c.charCodeAt(0)).buffer);window.audioKey=key;}
   const b=window.audioBuffer,rate=b.sampleRate,channels=loop?Math.min(2,b.numberOfChannels):1;
   const sourceChannels=Array.from({length:b.numberOfChannels},(_,i)=>b.getChannelData(i));
   let peak=0,energy=0;for(let i=0;i<b.length;i+=8)for(const channel of sourceChannels){peak=Math.max(peak,Math.abs(channel[i]));energy+=channel[i]*channel[i];}
   const hop=Math.max(1,Math.round(rate*.02)),envelope=[];
   for(let i=0;i<b.length;i+=hop){let sum=0,n=0;for(let j=i;j<Math.min(i+hop,b.length);j+=8){const value=sourceChannels[0][j];sum+=value*value;n++;}envelope.push(Math.sqrt(sum/Math.max(1,n)));}
   const threshold=Math.max(.001,Math.max(...envelope)*.07);let first=envelope.findIndex(x=>x>=threshold);first=Math.max(0,first);
   let start=loop?Math.min(id==='Engine'?2:.3,b.duration*.1):Math.max(0,first*.02-.035);
   if(id.startsWith('Foot')){
    const peaks=[];for(let i=1;i<envelope.length-1;i++)if(envelope[i]>threshold&&envelope[i]>=envelope[i-1]&&envelope[i]>=envelope[i+1])peaks.push({i,value:envelope[i]});
    peaks.sort((a,b)=>b.value-a.value);const spaced=[];for(const p of peaks){if(spaced.every(a=>Math.abs(a.i-p.i)*.02>.23))spaced.push(p);if(spaced.length===3)break;}spaced.sort((a,b)=>a.i-b.i);
    const variant=id.endsWith('3')?2:id.endsWith('2')?1:0;start=Math.max(0,(spaced[variant%Math.max(1,spaced.length)]?.i??first)*.02-.045);
   }
   const length=Math.max(1,Math.min(Math.floor(maxSeconds*rate),b.length-Math.floor(start*rate))),offset=Math.floor(start*rate);
   const crossfade=loop?Math.min(Math.round(rate*.18),Math.floor(length/8)):0,count=length-crossfade;
   const data=Array.from({length:channels},()=>new Float32Array(count));let outputPeak=0;
   function sample(channel,index){if(channels===1)return sourceChannels.reduce((sum,c)=>sum+c[index],0)/sourceChannels.length;return sourceChannels[channel][index];}
   for(let c=0;c<channels;c++)for(let i=0;i<count;i++){
    let v=sample(c,offset+i+crossfade);
    if(crossfade&&i>=count-crossfade){const j=i-(count-crossfade),t=j/crossfade;v=v*Math.cos(t*Math.PI/2)+sample(c,offset+j)*Math.sin(t*Math.PI/2);}
    if(!loop){const fade=Math.min(rate*.008,count/8);v*=Math.min(1,i/fade,(count-1-i)/fade);}
    data[c][i]=v;outputPeak=Math.max(outputPeak,Math.abs(v));
   }
   if(outputPeak<.0001)throw new Error('Silent prepared clip '+id);
   const target=loop?.65:.85,normalization=Math.min(4,target/outputPeak),bytes=new Uint8Array(44+count*channels*2),view=new DataView(bytes.buffer);
   function ascii(offset,text){for(let i=0;i<text.length;i++)view.setUint8(offset+i,text.charCodeAt(i));}
   ascii(0,'RIFF');view.setUint32(4,bytes.length-8,true);ascii(8,'WAVE');ascii(12,'fmt ');view.setUint32(16,16,true);view.setUint16(20,1,true);view.setUint16(22,channels,true);view.setUint32(24,rate,true);view.setUint32(28,rate*channels*2,true);view.setUint16(32,channels*2,true);view.setUint16(34,16,true);ascii(36,'data');view.setUint32(40,bytes.length-44,true);
   for(let i=0;i<count;i++)for(let c=0;c<channels;c++)view.setInt16(44+(i*channels+c)*2,Math.round(Math.max(-1,Math.min(1,data[c][i]*normalization))*32767),true);
   let encoded='';for(let i=0;i<bytes.length;i+=32768)encoded+=String.fromCharCode(...bytes.subarray(i,i+32768));
   return{encoded:btoa(encoded),sourceSeconds:b.duration,sourceChannels:b.numberOfChannels,rate,start,seconds:count/rate,channels,normalization,sourcePeak:peak,sourceRms:Math.sqrt(energy/Math.max(1,Math.ceil(b.length/8)*sourceChannels.length)),crossfadeSeconds:crossfade/rate};
  },{base64:raw.toString('base64'),key:hash,id,maxSeconds,loop});
  const bytes=Buffer.from(result.encoded,'base64');delete result.encoded;await fs.writeFile(path.join(out,id+'.wav'),bytes);
  entries.push({id,path:'Varginha/Audio/Received20261021/'+id,gain,loop,source:source.name,sourceSha256:hash,sha256:createHash('sha256').update(bytes).digest('hex'),...result});
  if(!sources.some(x=>x.sha256===hash))sources.push({file:source.name,sha256:hash,seconds:result.sourceSeconds,format:source.format});
 }
 entries.push({...entries.find(e=>e.id==='ManifestationTear'),id:'SealClose',aliasOf:'ManifestationTear'});
 for(const [id,source] of [['BossClaw','EnemyAttack'],['EchoClaw','EnemyAttack'],['ManifestationRise','BossRoar']])
  entries.push({...entries.find(e=>e.id===source),id,aliasOf:source});
 await fs.writeFile(path.join(out,'Catalog.json'),JSON.stringify({entries},null,2));
 await fs.writeFile(path.join(qa,'prepared-audio.json'),JSON.stringify({success:true,sources,entries,method:'Local Chromium decoder; PCM16 WAV, measured onset/footstep transients, gain capped at4, 180ms overlap-crossfade on loops. No original files changed.'},null,2));
 const aliases=entries.filter(e=>e.aliasOf).length;
 console.log(JSON.stringify({success:true,uniqueSources:sources.length,preparedFiles:entries.length-aliases,aliases,sources:sources.map(s=>({file:path.basename(s.file),seconds:Math.round(s.seconds*100)/100}))}));
}finally{await browser.close();}
