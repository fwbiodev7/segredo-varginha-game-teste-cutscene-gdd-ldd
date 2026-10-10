"""Assemble the approved newly generated empty bases + separate alpha furniture.

Only technical cropping/packing/scaling occurs here. Architecture is the unpatched
ImageGen output. Source pixels use top-left coordinates in the 1254-square drawings.
"""
import json
from pathlib import Path
from PIL import Image, ImageDraw, ImageFont
import numpy as np

ROOT=Path(__file__).resolve().parents[1]
SOURCES=ROOT/'Docs/Casarao20261009/Refeito/Fontes'
OUTPUT=ROOT/'Preview/CasaraoRefeito20261009'
ART=ROOT/'Assets/Resources/Varginha/IllustratedMaps'
WORLD_WIDTH=18.0
SIZE=1254
NAMES=['tree','tall_tree','shrub','well','bench','cart','lamp','flowers','barrel','crate','chest','crates',
       'shelf','bottles','drawers','wardrobe','sofa','armchair','oval','rug','runner','plant','clock','desk',
       'chair','table','books','globe','portrait','candelabra','lab','covered_table','ritual','cloth','papers','instruments']
CROPS=[(10,12,205,238),(236,8,134,242),(405,65,188,182),(608,46,202,202),(816,112,205,129),(1025,63,215,183),
       (60,254,82,215),(221,293,180,163),(449,287,126,166),(641,295,127,158),(827,314,190,142),(1045,266,181,183),
       (24,478,180,189),(212,478,193,190),(416,466,191,188),(632,469,145,204),(787,525,263,149),(1052,477,189,197),
       (24,713,179,137),(211,688,263,174),(489,659,98,210),(633,668,143,202),(840,654,101,215),(1000,691,241,169),
       (46,860,106,175),(184,889,287,147),(498,860,104,174),(673,869,136,166),(850,868,149,167),(1059,847,144,187),
       (17,1031,228,206),(251,1050,181,185),(437,1031,257,215),(697,1039,176,201),(881,1054,197,185),(1093,1036,133,198)]


def atlas():
    source=Image.open(SOURCES/'Moveis.png').convert('RGBA')
    packed=Image.new('RGBA',(1536,1536))
    frames={}
    folder=OUTPUT/'Moveis';folder.mkdir(parents=True,exist_ok=True)
    for index,(name,(x,y,w,h)) in enumerate(zip(NAMES,CROPS)):
        cut=source.crop((x,y,x+w,y+h))
        rgba=np.array(cut)
        # Remove only low-confidence alpha fringe; retain original RGB and strong alpha.
        rgba[:,:,3][rgba[:,:,3]<200]=0
        cut=Image.fromarray(rgba)
        box=cut.getbbox();cut=cut.crop(box)
        factor=min(1,224/cut.width,224/cut.height)
        if factor<1:
            cut=cut.resize((round(cut.width*factor),round(cut.height*factor)),Image.Resampling.NEAREST)
        cut.save(folder/(name+'.png'))
        px=(index%6)*256+(256-cut.width)//2;py=(index//6)*256+(256-cut.height)//2
        packed.alpha_composite(cut,(px,py))
        frames[name]={'source':[px,py,cut.width,cut.height], 'support':[cut.width/2,cut.height*.96]}
    packed.save(ART/'CasaraoProps.png');packed.save(OUTPUT/'04_Moveis_Atlas.png')
    (OUTPUT/'Moveis_Manifesto.json').write_text(json.dumps({'texture':'CasaraoProps','size':list(packed.size),
       'grid':[6,6],'cell':[256,256],'origin':'top-left','frames':frames},indent=2)+'\n',encoding='utf-8')
    return packed,frames


def wall(x,y,w,h,face=True):
    result={'rect':[x,y,w,h],'exact':True}
    if face:result['art']=[x,y,w,h]
    else:result['hidden']=True
    return result


def prop(frames,name,key,x,y,width,blocking=True,floor=False):
    source=frames[key]['source'];height=round(width*source[3]/source[2])
    feet=[round(x+width*.14,2),round(y+height*.73,2),round(width*.72,2),round(height*.24,2)] if blocking else []
    return {'name':name,'motif':'Original','texture':'CasaraoProps','source':source,
            'art':[x,y,width,height],'base':feet,'ground':[x+width/2,y+height*.97],
            'floor':floor,'outline':[0,0,1,0,1,1,0,1]}


def building_walls(cellar=False):
    result=[wall(0,0,1254,42,False),wall(0,42,479,78,False),wall(779,42,475,78,False),
        wall(0,120,77,971,False),wall(1177,120,77,971,False),wall(0,1091,479,163,False),
        wall(779,1091,475,163,False),wall(479,1208,300,46,False),
        wall(77,120,41,971),wall(1136,120,41,971),wall(118,120,361,116),wall(779,120,357,116),
        wall(479,42,41,194),wall(738,42,41,194),wall(520,42,218,36),
        wall(118,425,361,110 if not cellar else 35),wall(779,425,357,110 if not cellar else 35),
        wall(479,425,41,128 if not cellar else 35),wall(738,425,41,128 if not cellar else 35),
        wall(118,733,361,109 if not cellar else 35),wall(779,733,357,109 if not cellar else 35),
        wall(479,733,41,129 if not cellar else 35),wall(738,733,41,129 if not cellar else 35),
        wall(118,1037,361,54),wall(779,1037,357,54),
        wall(479,1037,41,171),wall(738,1037,41,171),wall(520,1168,218,40),
        wall(551,1037,25,131),wall(678,1037,25,131)]
    return result


def make_layout(phase,image,walls,props,spawn,points,rooms):
    return {'phase':phase,'image':image,'background':image,'width':SIZE,'height':SIZE,
        'bounds':[-WORLD_WIDTH/2,-WORLD_WIDTH/2,WORLD_WIDTH,WORLD_WIDTH],
        'spawn':spawn,'ambient':[.58,.62,.70],'walls':walls,'props':props,
        'points':[{'id':key,'pixel':value} for key,value in points.items()],
        'rooms':[{'name':name,'rect':area} for name,area in rooms],
        'lights':[{'pixel':[p['art'][0]+p['art'][2]/2,p['art'][1]+p['art'][3]*.3],
                   'color':[1,.67,.30],'radius':1.55} for p in props if 'Lampião' in p['name'] or 'Velas' in p['name']]}


def layouts(frames):
    p=lambda *args,**kwargs:prop(frames,*args,**kwargs)
    external=make_layout(12,'CasaraoExteriorBase',
      [wall(0,0,1254,59,False),wall(0,59,31,922,False),wall(1226,59,28,922,False),
       wall(31,59,49,794),wall(1180,59,46,794),wall(80,59,1100,86),
       wall(272,75,715,376,False),wall(966,259,91,150,False),
       wall(512,444,44,57),wall(698,444,45,57),wall(29,853,519,128),wall(710,853,515,128)],
      [p('Poço antigo','well',151,578,168),p('Banco do jardim','bench',350,635,158),
       p('Carroça abandonada','cart',915,671,196),p('Árvore oeste','tree',90,330,201),
       p('Árvore leste','tall_tree',1045,326,116),p('Arbusto oeste','shrub',392,732,119),
       p('Arbusto leste','shrub',765,732,112),p('Jardineira oeste','flowers',474,543,68),
       p('Jardineira leste','flowers',712,543,68),p('Lampião oeste','lamp',484,810,47),
       p('Lampião leste','lamp',728,810,47),p('Barril da reforma','barrel',818,665,64)],
      [625,1005],{'entrance':[625,556],'plan':[430,765],'well':[235,790],
                  'reveal':[997,490],'service':[1020,442],'exit':[625,1121]},
      [('Jardim do casarão',[80,451,1100,402]),('Rua e portão',[80,981,1100,235])])

    ground=make_layout(112,'CasaraoInteriorBase',building_walls(),
      [p('Tapete da sala','rug',200,248,272,False,True),p('Sofá coberto','sofa',264,240,193),
       p('Poltrona da sala','armchair',145,245,65),p('Mesa redonda','oval',290,317,100),
       p('Estante da sala','shelf',935,240,99),p('Planta da sala','plant',421,347,46),
       p('Estante do arquivo','shelf',790,240,123),p('Armário do arquivo','wardrobe',1054,240,74),
       p('Globo do arquivo','globe',961,327,60),
       p('Mesa da cozinha','table',258,600,209),p('Barril da cozinha','barrel',419,850,56),
       p('Prateleira de frascos','bottles',135,540,107),p('Cadeira da cozinha','chair',303,540,45),
       p('Segunda cadeira da cozinha','chair',382,540,45),
       p('Mesa do escritório','desk',827,579,163),p('Gaveteiro do escritório','drawers',1047,540,70),
       p('Retrato do grupo','portrait',880,460,64,False),p('Cadeira do escritório','chair',885,540,48),
       p('Estante do depósito','shelf',140,850,109),p('Baú do depósito','chest',300,940,104),
       p('Móvel coberto do depósito','cloth',336,850,50),
       p('Relógio do corredor','clock',1060,850,51),p('Armário de serviço','drawers',803,850,103),
       p('Mesa de serviço','oval',950,930,105),
       p('Tapete norte do hall','runner',591,270,66,False,True),
       p('Tapete central do hall','runner',591,576,66,False,True),
       p('Tapete sul do hall','runner',591,875,66,False,True),
       p('Planta do hall','plant',527,916,40),p('Velas do hall','candelabra',677,919,43,False)],
      [625,1101],{'garden':[625,1123],'plan':[625,793],'reveal':[625,274],
                   'service':[625,204],'key':[1082,655],'photo':[998,704]},
      [('Hall central',[520,236,218,801]),('Sala de estar',[118,236,361,189]),
       ('Arquivo de serviço',[779,236,357,189]),('Cozinha',[118,535,361,198]),
       ('Escritório',[779,535,357,198]),('Depósito',[118,842,361,195]),
       ('Sala de serviço',[779,842,357,195]),('Corredor de serviço',[576,78,102,158])])

    cellar=make_layout(13,'CasaraoEsconderijoBase',building_walls(True),
      [p('Estante dos registros','shelf',132,216,146),p('Móvel coberto dos registros','cloth',347,241,88),
       p('Barril dos registros','barrel',348,344,55),p('Velas dos registros','candelabra',427,351,43,False),
       p('Estante dos instrumentos','instruments',1024,210,110),p('Mesa dos instrumentos','lab',808,240,167),
       p('Estante de frascos','bottles',1012,470,105),p('Caixa dos materiais','crate',912,475,79),
       p('Mesa dos registros','covered_table',145,475,172),p('Caixotes do arquivo','crates',350,475,90),
       p('Mesa de análise','lab',552,550,149),p('Papéis da pesquisa','papers',335,620,104,False,True),
       p('Símbolo ritual no piso','ritual',817,800,278,False,True),
       p('Caixotes do depósito','crates',145,782,132),p('Baú dos registros','chest',309,922,119),
       p('Barril do depósito','barrel',395,820,48),p('Velas da análise','candelabra',651,559,41,False)],
      [625,282],{'ground':[625,186],'pages':[232,670],'photos':[395,713],
                 'materials':[945,624],'puzzle':[625,708],'optional':[1063,622],'exit':[625,1101]},
      [('Arquivo das experiências',[118,236,361,189]),('Sala dos instrumentos',[779,236,357,189]),
       ('Depósito de registros',[118,460,361,273]),('Sala dos materiais',[779,460,357,273]),
       ('Depósito inferior',[118,768,361,269]),('Sala ritual',[779,768,357,269]),
       ('Área de análise',[520,236,218,801]),('Acesso pela escada',[576,78,102,158])])
    # Candles rest on the laboratory desktop, so share its floor sorting contact.
    desk=next(item for item in cellar['props'] if item['name']=='Mesa de análise')
    candles=next(item for item in cellar['props'] if item['name']=='Velas da análise')
    candles['ground']=[candles['ground'][0],desk['ground'][1]+1]
    candles['support']=desk['name']
    return [external,ground,cellar]


def render(data,base,sheet):
    composed=base.convert('RGBA')
    for p in sorted(data['props'],key=lambda item:(not item['floor'],item['ground'][1])):
        sx,sy,sw,sh=p['source'];x,y,w,h=p['art']
        piece=sheet.crop((sx,sy,sx+sw,sy+sh)).resize((w,h),Image.Resampling.NEAREST)
        composed.alpha_composite(piece,(x,y))
    return composed


def collisions(data,composed,furnished=True):
    mask=Image.new('RGBA',(SIZE,SIZE));paint=ImageDraw.Draw(mask)
    for room in data['rooms']:
        x,y,w,h=room['rect'];paint.rectangle((x,y,x+w,y+h),fill=(35,160,75,56))
    for item in data['walls']:
        if item.get('hidden'):continue
        x,y,w,h=item['rect'];paint.rectangle((x,y,x+w,y+h),fill=(235,36,42,130),outline=(255,55,65,245),width=3)
    if furnished:
        for p in data['props']:
            if not p['base']:continue
            x,y,w,h=p['base'];paint.rectangle((x,y,x+w,y+h),fill=(235,36,42,110),outline=(255,70,70,245),width=3)
    portals={'entrance','garden','service','ground','exit'}
    for point in data['points']:
        x,y=point['pixel'];color=(45,167,255,230) if point['id'] in portals else (255,203,40,235)
        paint.rectangle((x-13,y-13,x+13,y+13),outline=color,width=4)
    image=Image.alpha_composite(composed.convert('RGBA'),mask)
    panel=Image.new('RGBA',(SIZE,SIZE+70),(7,16,23,255));panel.paste(image,(0,0))
    draw=ImageDraw.Draw(panel);font=ImageFont.truetype('C:/Windows/Fonts/arial.ttf',21)
    draw.text((20,SIZE+12),'VERMELHO: colisão   VERDE: piso   AZUL: passagem   AMARELO: investigação',font=font,fill='white')
    return panel


def main():
    OUTPUT.mkdir(parents=True,exist_ok=True)
    sheet,frames=atlas();maps=layouts(frames)
    for index,(data,name) in enumerate(zip(maps,['Exterior','Interior','Esconderijo']),1):
        base=Image.open(SOURCES/(name+'.png')).convert('RGB')
        base.save(OUTPUT/f'{index:02}_{name}_Planta.png')
        # Logical production resolution; keep source independently at full generated resolution.
        base.resize((512,512),Image.Resampling.NEAREST).save(ART/(data['image']+'.png'))
        composed=render(data,base,sheet)
        composed.save(OUTPUT/f'{index:02}_{name}_Mobiliado.png')
        collisions(data,base,False).save(OUTPUT/f'{index:02}_{name}_Colisoes_Planta.png')
        collisions(data,composed).save(OUTPUT/f'{index:02}_{name}_Colisoes.png')
    (OUTPUT/'Layouts.json').write_text(json.dumps({'layouts':maps},ensure_ascii=False,indent=2)+'\n',encoding='utf-8')
    target=ART/'Layouts.json';document=json.loads(target.read_text(encoding='utf-8'))
    replacements={m['phase']:m for m in maps}
    document['layouts']=[replacements.get(item['phase'],item) for item in document['layouts']]
    target.write_text(json.dumps(document,ensure_ascii=False,indent=2)+'\n',encoding='utf-8')
    print(json.dumps({'maps':len(maps),'worldWidth':WORLD_WIDTH,'props':[len(m['props']) for m in maps],
                       'atlas':list(sheet.size),'output':str(OUTPUT)}))


if __name__=='__main__':main()
