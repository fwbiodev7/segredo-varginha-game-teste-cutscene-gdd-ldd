"""Refine vector foreground meshes without resampling or repainting the source artwork."""
import copy
import json
import re
from pathlib import Path
from PIL import Image, ImageDraw

PATH = Path('Assets/Resources/Varginha/IllustratedMaps/Layouts.json')
document = json.loads(PATH.read_text(encoding='utf-8'))

def flat(points):
    return [n for point in points for n in point]

def shape(prop, points):
    prop['outline'] = flat(points)

def rectangle(x, y, w, h):
    return flat([(x,y),(x+w,y),(x+w,y+h),(x,y+h)])

# Detailed silhouettes are expressed in the existing crop, with a top-left origin.
CHAIR = [(0.13,0.02),(.85,.02),(.98,.13),(.98,.97),(.82,.97),(.82,.69),(.20,.69),(.20,.97),(.04,.97),(.04,.13)]
PLANT = [(.49,.01),(.60,.09),(.73,.02),(.72,.20),(.91,.14),(.85,.31),(.99,.35),(.86,.47),(.95,.55),(.76,.60),(.72,.80),(.66,.96),(.31,.97),(.23,.83),(.20,.67),(.07,.62),(.16,.48),(.01,.41),(.16,.34),(.06,.22),(.28,.24),(.30,.07),(.42,.15)]
TREE = [(.49,.02),(.66,.06),(.70,.14),(.82,.11),(.86,.23),(.96,.25),(.94,.40),(.99,.47),(.88,.61),(.79,.68),(.65,.69),(.64,.83),(.74,.92),(.60,.97),(.48,.90),(.33,.96),(.28,.89),(.39,.80),(.36,.70),(.22,.71),(.11,.61),(.01,.50),(.06,.36),(.03,.30),(.18,.21),(.27,.12),(.36,.14),(.39,.05)]
BED = [(.09,.01),(.16,.01),(.16,.07),(.81,.07),(.81,.01),(.91,.01),(.93,.95),(.84,.98),(.84,.89),(.16,.89),(.16,.98),(.06,.98),(.06,.08)]
TABLE = [(.07,.05),(.91,.05),(.98,.15),(.98,.59),(.92,.65),(.92,.97),(.85,.97),(.85,.67),(.15,.67),(.15,.97),(.07,.97),(.07,.66),(.02,.59),(.02,.16)]

special = {
 'IndustrialLibrary': {
  'Estante norte oeste':[(.03,.03),(.97,.03),(.99,.96),(.04,.96)],
  'Estante norte central':[(.02,.03),(.98,.03),(.98,.97),(.02,.97)],
  'Estante norte leste':[(.02,.02),(.98,.02),(.98,.28),(.78,.28),(.78,.97),(.03,.97)],
  'Estante sudoeste':[(.02,.02),(.98,.02),(.98,.96),(.12,.96),(.12,.82),(.02,.82)],
  'Estante sudeste':[(.02,.02),(.98,.02),(.98,.82),(.85,.82),(.85,.96),(.02,.96)],
  'Painel de avisos':[(.02,.02),(.98,.02),(.98,.96),(.02,.96)],
  'Armario do globo':[(.24,.01),(.43,.03),(.49,.17),(.48,.29),(.60,.31),(.70,.25),(.84,.21),(.91,.23),(.95,.43),(.98,.43),(.98,.98),(.02,.98),(.02,.26),(.12,.26),(.12,.10)],
  'Mesa redonda':[(.24,.11),(.52,.07),(.76,.15),(.91,.30),(.97,.46),(.91,.65),(.77,.78),(.73,.94),(.66,.94),(.66,.81),(.32,.81),(.32,.97),(.25,.97),(.24,.78),(.08,.63),(.03,.42),(.10,.26)],
  'Mesa dos recortes':[(.05,.05),(.90,.05),(.92,.78),(.88,.81),(.88,.98),(.81,.98),(.81,.82),(.18,.82),(.18,.98),(.09,.98),(.09,.81),(.04,.78)],
 },
 'Child1996': {
  'Sofá':[(.09,.04),(.88,.04),(.88,.18),(.96,.18),(.99,.91),(.91,.95),(.91,.85),(.08,.85),(.08,.97),(.01,.92),(.03,.18),(.09,.18)],
  'TV CRT':[(.21,.01),(.43,.22),(.47,.22),(.71,.01),(.73,.04),(.53,.23),(.83,.25),(.89,.32),(.89,.64),(.98,.65),(.98,.99),(.02,.99),(.02,.67),(.10,.65),(.10,.34),(.16,.27),(.38,.25),(.19,.04)],
  'Box do banheiro':[(.05,.02),(.90,.02),(.90,.97),(.06,.97)],
  'Geladeira':[(.04,.01),(.91,.01),(.96,.06),(.96,.98),(.03,.98),(.03,.08)],
  'Fogão':[(.07,.02),(.90,.02),(.96,.09),(.95,.94),(.87,.94),(.87,.98),(.13,.98),(.13,.94),(.03,.94),(.03,.11)],
  'Lavatório':[(.09,.02),(.80,.02),(.96,.09),(.99,.32),(.97,.43),(.96,.96),(.88,.96),(.88,.92),(.13,.92),(.13,.98),(.05,.98),(.05,.43),(.01,.38),(.05,.18)],
  'Mesa do jornal':[(.04,.05),(.95,.05),(.98,.21),(.98,.64),(.92,.69),(.92,.96),(.85,.96),(.85,.72),(.13,.72),(.13,.97),(.06,.97),(.06,.68),(.02,.61)],
 },
 'Adult2026': {
  'Bookshelf_Bedroom':[(.03,.02),(.93,.02),(.93,.99),(.03,.99)],
  'Bookshelf_Office':[(.01,.08),(.94,.08),(.94,.98),(.01,.98)],
  'Nightstand_Bedroom':[(.06,.02),(.93,.02),(.96,.89),(.91,.89),(.91,.97),(.83,.97),(.83,.90),(.18,.90),(.18,.97),(.09,.97),(.09,.91),(.04,.89)],
  'Dresser_Bedroom':[(.06,.02),(.95,.02),(.98,.42),(.95,.43),(.95,.97),(.07,.97),(.07,.44),(.03,.43)],
  'Sofa_LivingRoom':[(.16,.04),(.84,.04),(.88,.12),(.94,.12),(.97,.29),(.97,.90),(.90,.94),(.90,.86),(.12,.86),(.12,.94),(.06,.92),(.06,.29),(.10,.12),(.16,.12)],
  'Fridge_Kitchen':[(.17,.01),(.82,.01),(.95,.06),(.96,.98),(.09,.98),(.09,.08)],
  'TV_StaticNoise':[(.03,.05),(.84,.05),(.84,.52),(.55,.52),(.55,.59),(.74,.64),(.85,.64),(.96,.67),(.96,.98),(.02,.98),(.02,.64),(.37,.64),(.53,.59),(.53,.52),(.03,.52)],
 },
 'Ouzana': {
  'Herbário doméstico':[(.02,.03),(.94,.03),(.94,.95),(.90,.95),(.90,.98),(.84,.98),(.84,.95),(.10,.95),(.10,.98),(.03,.98)],
  'Sofá verde':[(.16,.02),(.88,.02),(.91,.12),(.96,.15),(.98,.32),(.97,.93),(.91,.95),(.91,.86),(.09,.86),(.09,.97),(.03,.97),(.02,.38),(.06,.15),(.13,.15)],
  'Geladeira de amostras':[(.11,.01),(.83,.01),(.95,.10),(.95,.92),(.85,.98),(.14,.98),(.06,.93),(.06,.10)],
  'Espécime luminoso':[(.28,.01),(.79,.01),(.96,.06),(.96,.18),(.89,.22),(.89,.73),(.96,.75),(.97,.89),(.91,.97),(.08,.97),(.04,.89),(.05,.74),(.11,.72),(.11,.22),(.05,.19),(.05,.08)],
  'Terrário doméstico':[(.02,.02),(.94,.02),(.95,.94),(.89,.94),(.89,.97),(.81,.97),(.81,.94),(.12,.94),(.12,.98),(.04,.98),(.04,.94),(.02,.94)],
  'Arquivo científico':[(.12,.01),(.89,.01),(.91,.20),(.94,.20),(.94,.95),(.90,.95),(.90,.99),(.85,.99),(.85,.95),(.17,.95),(.17,.99),(.12,.99)],
  'Carrinho de cultivo':[(.09,.18),(.91,.18),(.96,.29),(.96,.77),(.92,.78),(.95,.90),(.88,.98),(.83,.98),(.76,.91),(.80,.83),(.17,.83),(.21,.91),(.14,.98),(.08,.98),(.02,.93),(.02,.86),(.06,.82),(.06,.29)],
 },
 'Church': {
  'Estante de registros':[(.02,.01),(.98,.01),(.98,.98),(.02,.98)],
  'Arquivo de 1898':[(.02,.01),(.98,.01),(.98,.98),(.02,.98)],
  'Coluna oeste':[(.06,.01),(.97,.01),(.97,.11),(.85,.15),(.85,.77),(.91,.81),(.95,.81),(.95,.98),(.05,.98),(.05,.83),(.11,.79),(.18,.78),(.18,.15),(.08,.11)],
  'Coluna leste':[(.01,.01),(.92,.01),(.92,.11),(.80,.15),(.80,.77),(.87,.81),(.91,.81),(.91,.98),(.01,.98),(.01,.83),(.07,.79),(.14,.78),(.14,.15),(.04,.11)],
  'Tapete central':[(.02,.01),(.98,.01),(.98,.98),(.02,.98)],
 },
 'Continuation112': {
  'Estante da sala':[(.06,.08),(.96,.08),(.96,.96),(.06,.96)],
  'Sofá oeste':[(.17,.03),(.50,.01),(.62,.05),(.73,.21),(.73,.81),(.66,.89),(.39,.89),(.24,.87),(.11,.74),(.11,.25)],
  'Sofá norte':[(.21,.01),(.88,.01),(.94,.11),(.98,.28),(.98,.59),(.91,.90),(.80,.96),(.13,.96),(.07,.87),(.07,.42),(.10,.21)],
  'Mesa da sala':[(.41,.07),(.74,.15),(.82,.26),(.88,.50),(.83,.66),(.73,.78),(.57,.87),(.35,.83),(.24,.73),(.14,.53),(.14,.38),(.25,.23)],
  'Mesa do hall':[(.42,.05),(.65,.09),(.81,.23),(.89,.42),(.84,.66),(.74,.78),(.54,.89),(.32,.83),(.13,.67),(.07,.42),(.16,.20)],
  'Mesa do escritório':[(.10,.25),(.89,.25),(.89,.77),(.85,.80),(.85,.92),(.78,.92),(.78,.81),(.17,.81),(.17,.92),(.11,.92)],
  'Armário do escritório':[(.16,.01),(.94,.01),(.94,.38),(.16,.38)],
  'Estante do escritório':[(.07,.03),(.78,.03),(.78,.64),(.07,.64)],
 },
 'Continuation13': {
  'Estante de arquivo':[(.02,.09),(.93,.09),(.93,.97),(.02,.97)],
  'Bancada oeste':[(.04,.36),(.89,.36),(.89,.80),(.77,.80),(.77,.95),(.70,.95),(.70,.81),(.15,.81),(.15,.96),(.07,.96)],
  'Mesa central':[(.06,.20),(.94,.20),(.94,.70),(.90,.70),(.90,.96),(.85,.96),(.85,.71),(.12,.71),(.12,.96),(.07,.96)],
  'Bancada de instrumentos':[(.07,.13),(.85,.13),(.94,.45),(.94,.73),(.86,.75),(.86,.89),(.79,.89),(.79,.75),(.17,.75),(.17,.94),(.09,.94),(.09,.75),(.05,.73)],
  'Equipamento coberto':[(.48,.03),(.68,.07),(.72,.23),(.85,.36),(.95,.42),(.95,.96),(.54,.96),(.50,.84)],
 },
 'Continuation16': {
  'Painel elétrico':[(.20,.09),(.94,.09),(.94,.95),(.20,.95)],
  'Bancada':[(.18,.04),(.80,.04),(.85,.39),(.85,.70),(.79,.71),(.79,.79),(.74,.79),(.74,.70),(.24,.70),(.24,.85),(.19,.85),(.19,.70)],
  'Válvula da área alagada':[(.44,.02),(.65,.02),(.77,.14),(.77,.24),(.70,.29),(.69,.41),(.37,.41),(.37,.27),(.28,.22),(.28,.11)],
  'Porta norte':[(.14,.07),(.67,.07),(.80,.23),(.80,.72),(.69,.79),(.14,.79)],
 },
 'Continuation18': {
  'Cilindro da ruptura':[(.38,.04),(.61,.04),(.66,.18),(.66,.26),(.64,.31),(.64,.78),(.69,.82),(.69,.95),(.31,.95),(.28,.87),(.34,.78),(.34,.32),(.28,.27),(.28,.16)],
 },
 'Workshop': {
  'Armário de ferramentas':[(.17,.01),(.90,.01),(.90,.75),(.72,.75),(.72,.98),(.08,.98),(.08,.60),(.17,.60)],
  'Peças e ferramentas':[(.01,.01),(.98,.01),(.98,.97),(.01,.97)],
  'Reserva do reagente':[(.08,.07),(.27,.02),(.90,.02),(.92,.80),(.92,.95),(.86,.95),(.86,.86),(.15,.86),(.15,.95),(.08,.95)],
  'Caixas leste':[(.26,.02),(.67,.02),(.73,.33),(.75,.35),(.75,.66),(.48,.66),(.48,.93),(.12,.93),(.12,.44),(.26,.44)],
  'Lona e pneus':[(.06,.10),(.39,.10),(.61,.17),(.65,.38),(.68,.42),(.67,.96),(.09,.96),(.02,.87),(.02,.65)],
 },
}

seen_changes = set()
for layout in document['layouts']:
    image = layout['image']
    for prop in layout['props']:
        original = copy.deepcopy(prop)
        name, motif = prop['name'], prop['motif']
        if name in special.get(image, {}): shape(prop, special[image][name])
        elif motif in ('Tree',): shape(prop,TREE)
        elif motif == 'PottedPlant': shape(prop,PLANT)
        elif motif in ('Chair',): shape(prop,CHAIR)
        elif 'Bed' in motif: shape(prop,BED)
        elif motif in ('CoffeeTable','Child90_CoffeeTable','Ouzana_CoffeeTable'): shape(prop,TABLE)
        # Existing detailed computer desks/font/organ silhouettes are retained.
        if prop != original: seen_changes.add((image,name))

    if image=='School':
        for prop in layout['props']:
            if prop['name'].startswith('Cadeira azul'):
                shape(prop,[(.20,.01),(.77,.01),(.85,.08),(.90,.13),(.97,.13),(.97,.94),(.85,.94),(.85,.47),(.16,.47),(.16,.94),(.04,.94),(.04,.13),(.11,.13),(.18,.04)])

    if image == 'IndustrialLibrary':
        # Feet, rather than the entire ladder image, meet the floor.
        for wall in layout['walls']:
            if wall['rect'][0] in (317,683):
                x,y,w,h=wall['rect']; wall['rect']=[x,y+h-16,w,16];wall['skipBodyGuard']=True
        for prop in layout['props']:
            if prop['name']=='Escada da estante':
                prop['base']=[400,231,32,12]
                prop['pieces']=[{'outline':rectangle(.03,.01,.10,.98)},{'outline':rectangle(.82,.01,.10,.98)}]
                prop['pieces'] += [{'outline':rectangle(.10,y,.75,.035)} for y in (.15,.29,.43,.57,.71,.85)]
            if prop['name']=='Planta suspensa':
                prop['base']=[];prop['ground']=[336,223]
            if prop['name']=='Mesa central de fragmentos':
                shape(prop,[(.04,.18),(.38,.18),(.39,.28),(.42,.30),(.42,.42),(.45,.46),(.50,.46),(.53,.41),(.53,.29),(.57,.26),(.58,.18),(.95,.18),(.95,.77),(.92,.77),(.92,.98),(.89,.98),(.89,.80),(.12,.80),(.12,.98),(.08,.98),(.08,.77),(.04,.77)])
            if prop['name']=='Mesa de leitura oeste':
                shape(prop,[(.04,.22),(.55,.22),(.55,.72),(.88,.72),(.92,.52),(.97,.52),(.97,.77),(.92,.77),(.92,.96),(.85,.96),(.85,.80),(.12,.80),(.12,.98),(.07,.98),(.07,.77),(.02,.77)])
        # Independent vegetation meshes, sorted by their supporting furniture.
        for name,art,support in [('Planta sobre a mesa central',[476,312,62,72],'Mesa central de fragmentos'),('Planta sobre a mesa de leitura',[210,122,62,79],'Mesa de leitura oeste'),('Planta sobre a mesa redonda',[838,214,43,41],'Mesa redonda')]:
            if not any(p['name']==name for p in layout['props']):
                layout['props'].append({'name':name,'motif':'PottedPlant','art':art,'base':[],'outline':flat(PLANT),'support':support})

    # Trace foliage into source-pixel-aligned vector strips. The original PNG is never edited.
    # Green/yellow leaves are separated from wooden floor and shelf pixels; pots keep a manual mesh.
    if image in ('IndustrialLibrary','Ouzana'):
        source=Image.open(PATH.parent/(image+'.png')).convert('RGB')
        for prop in layout['props']:
            if not (prop['motif']=='PottedPlant' or prop['name'].startswith('Samambaia')) or prop['name']=='Planta suspensa':continue
            x,y,w,h=prop['art']; crop=source.crop((x,y,x+w,y+h)); pixels=crop.load();pieces=[]
            for row in range(h):
                run=None
                for column in range(w+1):
                    leafy=False
                    if column<w:
                        r,g,b=pixels[column,row];leafy=g>=r*.91 and g>b*1.35 and g>25 and r<g*1.12
                    if leafy and run is None:run=column
                    if not leafy and run is not None:
                        pieces.append({'outline':rectangle(run/w,row/h,(column-run)/w,1/h)});run=None
            base=prop.get('base',[])
            if len(base)==4:
                left=(base[0]-x)/w;right=(base[0]+base[2]-x)/w;top=max(0,(base[1]-y-4)/h);bottom=min(1,(base[1]+base[3]-y)/h)
            else:left,right,top,bottom=.30,.68,.69,.96
            width=right-left;hei=bottom-top
            pieces.append({'outline':flat([(left,top),(right,top),(right,top+hei*.28),(right-width*.12,bottom),(left+width*.12,bottom),(left,top+hei*.28)])})
            prop['pieces']=pieces

    # A blocking footprint must not reach beyond the visible source crop.
    for prop in layout['props']:
        base=prop.get('base',[])
        if len(base)!=4:continue
        x,y,w,h=prop['art'];bx,by,bw,bh=base
        left=max(x,bx);top=max(y,by);right=min(x+w,bx+bw);bottom=min(y+h,by+bh)
        prop['base']=[left,top,max(0,right-left),max(0,bottom-top)]

    if image=='Church':
        layout['stainedGlass']=[]
        colors={'Azul':[.55,.72,1.0],'Âmbar':[1.0,.78,.44],'Violeta':[.84,.53,1.0]}
        patches=[
            ('Azul',[(120,389),(162,411),(231,512),(172,499)]),
            ('Âmbar',[(112,423),(140,441),(204,542),(171,529)]),
            ('Violeta',[(199,481),(224,510),(279,596),(236,578)]),
            ('Azul',[(675,471),(738,500),(881,808),(824,785)]),
            ('Âmbar',[(762,458),(808,496),(963,827),(914,809)]),
            ('Violeta',[(704,538),(757,562),(914,887),(858,865)]),
        ]
        for index,(color,path) in enumerate(patches):
            layout['stainedGlass'].append({'name':'Vitral_'+str(index)+'_'+color,'outline':flat(path),'color':colors[color],'intensity':1,'falloff':.20})
            if index<3:
                layout['stainedGlass'].append({'name':'Vitral_leste_'+str(index)+'_'+color,'outline':flat([(layout['width']-x,y) for x,y in reversed(path)]),'color':colors[color],'intensity':1,'falloff':.20})

text=json.dumps(document,ensure_ascii=False,indent=2)
def compact_piece(match):
    values=[line.strip().rstrip(',') for line in match.group(2).splitlines()]
    return match.group(1)+'{"outline": ['+', '.join(values)+']}'+match.group(3)
text=re.sub(r'(?m)^(\s*)\{\n\s+"outline": \[\n((?:\s+[\d.eE+\-]+,?\n)+)\s+\]\n\s+\}(,?)',compact_piece,text)
PATH.write_text(text+'\n',encoding='utf-8')
print('Refined unique existing contours:',len(seen_changes))
print('Total furnishing entries:',sum(len(l['props']) for l in document['layouts']))
