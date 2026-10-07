import fs from 'node:fs';
import path from 'node:path';
import vm from 'node:vm';
const root=path.resolve('dist');
const html=fs.readFileSync(path.join(root,'index.html'),'utf8');
const context={window:{}};
vm.runInNewContext(fs.readFileSync(path.join(root,'config.js'),'utf8'),context);
new vm.Script(fs.readFileSync(path.join(root,'app.js'),'utf8'));
const config=context.window.GAME_SITE;
const failures=[];
const assets=new Set([...html.matchAll(/(?:src|href)="(assets\/[^"?#]+)"/g)].map(value=>value[1]));
for(const shot of config.screenshots){assets.add(shot.src);assets.add(shot.src.replace('.webp','-small.webp'));if(!shot.alt||!['GAMEPLAY','CUTSCENE','ARTE CONCEITUAL','EM DESENVOLVIMENTO'].includes(shot.category))failures.push('Categoria ou descrição da captura inválida.');}
for(const archive of config.archives)for(const key of ['image','second'])if(archive[key])assets.add(archive[key]);
assets.add('assets/social.png');
for(const asset of assets)if(!fs.existsSync(path.join(root,asset)))failures.push('Asset ausente: '+asset);
const ids=[...html.matchAll(/\bid="([^"]+)"/g)].map(value=>value[1]);
if(ids.length!==new Set(ids).size)failures.push('Identificadores duplicados.');
for(const value of html.matchAll(/href="#([^" ]+)"/g))if(!ids.includes(value[1]))failures.push('Âncora ausente: '+value[1]);
for(const source of config.historySources)if(!source.url.startsWith('https://')||!source.publication||!source.summary)failures.push('Fonte histórica incompleta.');
if(config.screenshots.length<8||config.archives.length!==5)failures.push('Conteúdo promocional incompleto.');
const publicCode=['index.html','app.js','config.js'].map(file=>fs.readFileSync(path.join(root,file),'utf8')).join('\n');
for(const expression of [/Edelzio.{0,35}(?:selo|receptáculo|âncora)/i,/acordo.{0,15}1996/i,/CONTROLE\s*→\s*RESÍDUO/i,/ÁRVORE\s*→\s*RIO/i,/23:23/])if(expression.test(publicCode))failures.push('Conteúdo reservado ao jogo encontrado no material público.');
if(/<video[^>]*autoplay|<audio|eval\(|new Function\(/i.test(publicCode))failures.push('Execução ou reprodução indevida.');
if(failures.length){console.error(failures.join('\n'));process.exit(1);}
console.log(JSON.stringify({passed:true,screenshots:config.screenshots.length,archives:config.archives.length,historySources:config.historySources.length,localReferences:assets.size,publicVideos:config.videos.length,publicDownload:!!config.download}));
