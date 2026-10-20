export async function author(presentation,session,data,images){
presentation.slides.keep([]);presentation.theme.defaultFont='Consolas';
const c={ink:'#071018',paper:'#EEE9D2',cyan:'#73D6DA',amber:'#FFB96C',muted:'#A8B9BB'};
const txt=(s,text,x,y,w,h,size=23,color=c.paper,pixel=false)=>{const t=s.shapes.add({geometry:'textbox',position:{left:x,top:y,width:w,height:h},fill:'none',line:{fill:'none',width:0}});t.text=text;t.text.style={fontSize:size,typeface:pixel?'Press Start 2P':'Consolas',color,wrap:'square',insets:0,verticalAlignment:'top'};return t;};
const im=(s,key,x,y,w,h)=>s.images.add({path:images[key],alt:key,position:{left:x,top:y,width:w,height:h},fit:'contain'});
for(const [i,d]of data.entries()){
 const s=presentation.slides.add({width:1280,height:720,background:{fill:c.ink}});s.speakerNotes.text=d.notes;
 txt(s,d.tag,64,52,1140,25,13,c.cyan,true);
 if(d.kind==='cover'){
  im(s,'cover',0,0,1280,720);s.shapes.add({geometry:'rect',position:{left:0,top:0,width:1280,height:720},fill:'rgba(2,7,11,0.65)',line:{fill:'none',width:0}});txt(s,d.tag,64,52,1140,25,13,c.cyan,true);txt(s,d.title,64,178,610,205,57,c.paper,true);txt(s,d.sub,64,430,610,50,25,c.cyan);txt(s,d.lead,64,498,620,50,22);
 }else{
  txt(s,d.title,64,103,1150,d.kind==='close'?110:75,d.kind==='close'?37:34,c.paper,true);
  if(d.kind==='split'){
   txt(s,d.lead,64,205,430,83,24,c.muted);let y=303;
   for(const[l,t]of d.items){txt(s,l,64,y,430,25,13,c.amber,true);txt(s,t,64,y+28,430,d.items.length===4?50:62,d.items.length===4?21:22);y+=d.items.length===4?82:98;}
   im(s,d.image,540,225,674,370);txt(s,d.caption,540,604,674,35,14,c.muted);
  }else if(d.kind==='photo'){
   im(s,d.image,64,206,780,438);txt(s,d.caption,64,650,780,24,12,c.muted);let y=220;
   for(const[l,t]of d.items){txt(s,l,884,y,330,28,13,c.amber,true);txt(s,t,884,y+33,330,72,22);y+=125;}
  }else if(d.kind==='duo'){
   for(const[f,l,t]of d.figures){const x=l==='MENU'?64:656;im(s,f,x,213,560,310);txt(s,l,x,535,560,30,13,c.amber,true);txt(s,t,x,574,560,54,22);}
  }else if(d.rows){
   if(d.lead)txt(s,d.lead,64,203,1140,50,26,c.muted);
   const top=d.lead?285:220;
   const table=s.tables.add({rows:d.rows.length,columns:3,left:64,top,width:1152,height:d.rows.length*76,columnWidths:d.kind==='quality'?[420,150,582]:d.kind==='priorities'?[150,405,597]:d.kind==='timeline'?[230,220,702]:[190,270,692],values:d.rows});
   table.styleOptions={headerRow:false,bandedRows:false};table.borders.assign({fill:c.ink,width:0});
   for(let r=0;r<d.rows.length;r++)for(let col=0;col<3;col++){const cell=table.getCell(r,col);cell.fill=c.ink;cell.text.style={typeface:'Consolas',fontSize:d.kind==='quality'?22:24,color:col===0?c.cyan:col===1?c.amber:c.paper,insets:5};}
  }else{
   if(d.lead)txt(s,d.lead,64,d.kind==='close'?239:205,1140,65,26,c.muted);
   let y=d.kind==='close'?342:d.kind==='feedback'?225:310;
   for(const[l,t]of d.items){const lw=d.kind==='feedback'?420:220;txt(s,l,64,y,lw,64,d.kind==='feedback'?24:13,c.amber,d.kind!=='feedback');txt(s,t,64+lw+28,y,1120-lw-28,82,24);y+=d.kind==='feedback'?89:98;}
  }
  if(d.bottom)txt(s,d.bottom,64,626,1152,43,19,c.cyan);
 }
 txt(s,'O SEGREDO DE VARGINHA • SPRINT FINAL',64,680,1000,20,12,'#7F999E');txt(s,String(i+1).padStart(2,'0')+' / '+data.length,1140,680,80,20,12,'#7F999E');
}
session.setActiveSlide(presentation.slides.getItem(0));return {slides:presentation.slides.items.length};
}
