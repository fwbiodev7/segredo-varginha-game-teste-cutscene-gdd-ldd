"""Reproduce the three approved mansion layouts from the supplied plant/props sheets.

This is sprite preparation, not generated artwork: original RGB pixels are retained,
annotations are covered with neighbouring floor/step pixels and props use binary alpha.
Run with the bundled Python (Pillow/numpy): --sources DIRECTORY [--apply].
Without --apply, only review PNGs and a three-layout manifest are produced in Preview.
"""
import argparse
import json
from pathlib import Path

import numpy as np
from PIL import Image, ImageDraw

ROOT = Path(__file__).resolve().parents[1]
ART = ROOT / 'Assets/Resources/Varginha/IllustratedMaps'
OUT = ROOT / 'Preview/Casarao20261009'
SHEET = 'CasaraoProps'

# Source crops are in the supplied 1536x1024 prop sheet, top-left pixel coordinates.
CROPS = {
    'bench': (718, 202, 107, 64), 'cart': (174, 170, 149, 122),
    'lamp': (1472, 20, 45, 164), 'flowers': (1180, 222, 62, 85),
    'barrel': (140, 248, 39, 55), 'door': (1416, 191, 99, 119),
    'shelf': (427, 337, 118, 110), 'bottles': (551, 337, 114, 85),
    'drawers': (668, 358, 92, 94), 'wardrobe': (948, 337, 59, 117),
    'sofa': (193, 524, 123, 60), 'armchair': (234, 578, 58, 84),
    'covered': (18, 502, 79, 147), 'oval': (83, 598, 79, 61),
    'rug': (441, 447, 95, 215), 'runner': (548, 426, 68, 237),
    'plant': (960, 510, 60, 110), 'clock': (1016, 571, 42, 89),
    'desk': (251, 405, 93, 82), 'chair': (268, 470, 35, 47),
    'table': (72, 413, 165, 68), 'books': (672, 516, 41, 39),
    'globe': (620, 538, 48, 60), 'portrait': (887, 340, 56, 72),
    'candelabra': (1067, 532, 38, 67),
    'basement_shelf': (195, 756, 92, 152), 'chest': (16, 844, 84, 79),
    'covered_table': (386, 777, 121, 112), 'lab': (582, 807, 124, 87),
    'crate': (137, 830, 51, 60), 'crates': (94, 904, 80, 95),
    'basement_barrel': (304, 856, 60, 82), 'instruments': (1036, 905, 90, 110),
    'ritual': (768, 862, 221, 151), 'basement_candles': (296, 765, 49, 97),
    'cloth': (573, 704, 88, 132), 'papers': (665, 865, 102, 130),
}


def rect(x, y, w, h):
    return [x, y, w, h]


def wall(x, y, w, h, face=True):
    result = {'rect': rect(x, y, w, h), 'exact': True}
    if face:
        result['art'] = result['rect'][:]
    return result


def prop(name, key, x, y, w, h, blocking=True, floor=False, contact=None):
    # Floor footprints occupy the visible contact, rather than the entire tall sprite.
    base = rect(round(x+w*.13, 2), round(y+h*.70, 2), round(w*.74, 2), round(h*.26, 2)) if blocking else []
    return {'name': name, 'motif': 'Original', 'texture': SHEET, 'source': list(CROPS[key]),
            'art': rect(x, y, w, h), 'base': base,
            'ground': contact or [x+w/2, y+h*.96], 'floor': floor,
            'outline': [0, 0, 1, 0, 1, 1, 0, 1]}


def embedded(name, art, base, outline=None):
    return {'name': name, 'motif': 'Original', 'art': art, 'base': base,
            'ground': [base[0]+base[2]/2, base[1]+base[3]],
            'outline': outline or [.15, .05, .85, .05, 1, .55, .85, 1, .15, 1, 0, .55]}


def layout(phase, image, width, height, spawn, walls, props, points, rooms):
    scale = 32/width
    return {'phase': phase, 'image': image, 'background': image, 'width': width, 'height': height,
            'bounds': [-16, -height*scale/2, 32, height*scale], 'spawn': spawn,
            'ambient': [.58, .62, .70], 'walls': walls, 'props': props,
            'points': [{'id': name, 'pixel': p} for name, p in points.items()],
            'rooms': [{'name': name, 'rect': r} for name, r in rooms],
            'lights': [{'pixel': [p['art'][0]+p['art'][2]/2, p['art'][1]+p['art'][3]*.35],
                        'color': [1, .67, .30], 'radius': 2.8}
                       for p in props if any(s in p['name'].lower() for s in ['lampião', 'velas'])]}


def make_layouts():
    exterior_walls = [wall(0,0,17,566,False),wall(493,0,19,566,False),wall(0,0,512,6,False),
        wall(130,6,277,254,False), wall(17,482,219,66),wall(288,482,205,66)]
    exterior_props = [
        embedded('Poço antigo', [41,274,103,85], [52,321,82,35]),
        embedded('Árvore do jardim oeste', [34,395,91,101], [68,463,34,29]),
        embedded('Árvore do jardim leste', [341,395,94,106], [370,467,35,30]),
        prop('Banco do jardim','bench',143,325,88,52),
        prop('Carroça abandonada','cart',358,273,99,81),
        prop('Lampião oeste','lamp',25,502,20,73),prop('Lampião leste','lamp',463,502,20,73),
        prop('Jardineira oeste','flowers',190,266,27,37),prop('Jardineira leste','flowers',307,266,27,37),
        prop('Barril da reforma','barrel',435,350,24,34)]
    exterior = layout(12,'CasaraoExteriorBase',512,652,[261,573],exterior_walls,exterior_props,
        {'entrance':[268,286],'plan':[206,381],'well':[106,374],
         'reveal':[438,237],'service':[445,191],'exit':[262,610]},
        [('Jardim do casarão',[33,260,445,220]),('Rua e portão',[17,550,476,88])])

    interior_walls = [
        wall(0,0,177,45,False),wall(350,0,170,45,False),wall(0,0,520,9,False),
        wall(14,45,165,17),wall(359,45,151,17),wall(14,45,17,460),wall(493,45,17,460),
        wall(177,9,16,38),wall(334,9,16,38),wall(193,9,141,15),
        wall(155,62,17,129),wall(353,62,18,129),
        wall(23,211,160,18),wall(340,211,153,18),
        wall(185,199,16,96),wall(185,334,16,60),wall(324,199,16,96),wall(324,334,16,60),
        wall(23,355,160,18),wall(340,355,153,18),
        wall(185,396,16,54),wall(185,500,16,45),wall(324,396,16,54),wall(324,500,16,45),
        wall(23,493,83,17),wall(106,493,17,48),wall(123,524,62,17),
        wall(340,524,65,17),wall(405,493,17,48),wall(422,493,71,17),
        wall(185,545,16,58),wall(324,545,16,58),wall(0,510,185,142,False),
        wall(340,510,180,142,False),wall(0,628,520,24,False)]
    interior_props = [
        prop('Tapete do hall','runner',230,193,69,226,False,True),
        prop('Tapete da sala','rug',43,258,95,82,False,True),
        prop('Estante da sala','shelf',35,75,58,54),prop('Móvel coberto da sala','covered',110,86,33,61),
        prop('Sofá coberto','sofa',47,241,91,45),prop('Poltrona coberta','armchair',50,292,30,43),
        prop('Mesa redonda da sala','oval',86,299,48,37),
        prop('Planta da sala','plant',130,302,27,49),
        prop('Estante de frascos','bottles',410,68,68,51),prop('Armário do escritório','drawers',418,242,51,52),
        prop('Mesa do escritório','desk',365,281,58,51),prop('Cadeira do escritório','chair',382,337,22,30),
        prop('Retrato do grupo','portrait',378,247,25,33,False),
        prop('Globo do escritório','globe',443,305,26,33),
        prop('Mesa da cozinha','table',52,409,98,41),prop('Armário da cozinha','wardrobe',38,380,29,57),
        prop('Barril da cozinha','barrel',135,448,25,35),
        prop('Arquivo do corredor','shelf',385,381,61,57),prop('Relógio antigo','clock',456,401,24,51),
        prop('Planta do hall','plant',207,422,23,42),
        prop('Velas do hall','candelabra',295,426,22,39,False)]
    interior = layout(112,'CasaraoInteriorBase',520,652,[262,565],interior_walls,interior_props,
        {'garden':[262,588],'plan':[258,392],'reveal':[262,162],
         'service':[262,128],'key':[420,347],'photo':[376,269]},
        [('Hall central',[202,175,122,370]),('Sala de estar',[31,229,154,126]),
         ('Escritório',[340,229,153,126]),('Cozinha',[31,373,154,120]),
         ('Arquivo de serviço',[340,373,153,120]),('Corredor de serviço',[203,24,131,168])])

    cellar_walls = [
        wall(0,0,159,92,False),wall(318,0,163,92,False),wall(0,0,481,9,False),
        wall(11,92,148,17),wall(318,92,143,17),wall(11,92,18,410),wall(445,92,18,410),
        wall(159,9,18,83),wall(300,9,18,83),wall(177,9,123,16),
        wall(159,109,14,105),wall(301,109,14,105),
        wall(29,275,130,19),wall(315,275,130,19),
        wall(158,251,15,103),wall(302,251,15,103),wall(158,395,15,87),wall(302,395,15,87),
        wall(29,485,144,17),wall(317,485,128,17),
        wall(173,485,16,94),wall(302,485,16,94),wall(189,566,113,16),
        wall(0,502,173,150,False),wall(318,502,163,150,False),wall(173,582,145,70,False)]
    cellar_props = [
        prop('Estante dos registros','basement_shelf',37,118,53,87),
        prop('Mesa dos registros','covered_table',72,322,75,69),
        prop('Mesa dos instrumentos','lab',340,122,82,57),
        prop('Mesa de análise','lab',199,300,86,60),
        prop('Símbolo ritual no piso','ritual',327,325,103,71,False,True),
        prop('Estante dos instrumentos','instruments',366,209,60,73),
        prop('Caixotes do arquivo','crates',35,409,58,69),prop('Baú dos registros','chest',88,427,54,51),
        prop('Barril do arquivo','basement_barrel',94,204,34,46),
        prop('Móvel coberto do porão','cloth',114,119,39,58),
        prop('Caixa dos materiais','crate',355,408,40,47),prop('Caixas do fundo','crates',398,406,37,44),
        prop('Papéis da pesquisa','papers',204,407,72,91,False,True),
        prop('Velas do arquivo','basement_candles',35,319,27,54,False),
        prop('Velas da análise','basement_candles',276,185,25,50,False)]
    cellar = layout(13,'CasaraoEsconderijoBase',481,652,[237,155],cellar_walls,cellar_props,
        {'ground':[237,95],'pages':[96,405],'photos':[238,376],
         'materials':[354,196],'puzzle':[235,283],'optional':[387,314],'exit':[239,526]},
        [('Arquivo das experiências',[29,109,130,166]),('Área de análise',[173,109,128,373]),
         ('Sala dos instrumentos',[317,109,128,166]),('Depósito de registros',[29,294,129,191]),
         ('Sala ritual',[317,294,128,191]),('Acesso pela escada',[177,25,123,84])])
    interior['patches']=[{'sample':[238,447,26,50],'rect':[180,450,26,50]},
                         {'sample':[238,447,26,50],'rect':[319,450,26,50]}]
    cellar['patches']=[{'sample':[207,351,20,41],'rect':[155,354,20,41]},
                       {'sample':[207,351,20,41],'rect':[299,354,20,41]}]
    return [exterior, interior, cellar]


def cover_with_strip(image, target, source):
    x,y,w,h = target
    tile = image.crop((source[0],source[1],source[0]+source[2],source[1]+h))
    for offset in range(0,w,tile.width):
        image.paste(tile.crop((0,0,min(tile.width,w-offset),h)),(x+offset,y))


def prepare(sources, apply):
    OUT.mkdir(parents=True, exist_ok=True)
    plant = Image.open(sources/'Planta Pixelada do Casarão Zé Gomes.png').convert('RGB')
    original = Image.open(sources/'Folha de Props do Casarão Zé Gomes.png').convert('RGBA')
    pixels = np.array(original)
    pixels[:,:,3] = np.where(pixels[:,:,3]>=160,255,0).astype(np.uint8)
    atlas = Image.fromarray(pixels)
    # Remove instructional portal labels from the playable texture with like-for-like pixels.
    cover_with_strip(plant,(766,153,39,32),(751,153,13))
    cover_with_strip(plant,(703,370,40,45),(753,370,18))
    cover_with_strip(plant,(826,369,37,48),(754,369,18))
    plant.paste(plant.crop((747,515,790,564)),(763,570))
    cover_with_strip(plant,(1277,103,40,48),(1255,103,18))
    plant.paste(plant.crop((310,658,352,728)),(239,658))
    # A1 overlays the exterior door; the supplied isolated door restores it precisely.
    cover_with_strip(plant,(248,274,37,50),(238,274,10))
    door=atlas.crop((1416,191,1515,310)).resize((61,79),Image.Resampling.NEAREST)
    plant.paste(door,(236,254),door)
    cuts=[(0,80,512,732),(520,80,1040,732),(1055,80,1536,732)]
    layouts=make_layouts()
    assets={SHEET:atlas}
    for data,cut in zip(layouts,cuts):
        base=plant.crop(cut)
        assets[data['image']]=base
        composed=base.convert('RGBA')
        for patch in data.get('patches',[]):
            sx,sy,sw,sh=patch['sample'];x,y,w,h=patch['rect']
            composed.paste(base.crop((sx,sy,sx+sw,sy+sh)).resize((w,h),Image.Resampling.NEAREST),(x,y))
        # Draw low floor decorations first, then furniture by floor contact.
        props=sorted(data['props'],key=lambda p: (-1 if p.get('floor') else p['ground'][1]))
        for p in props:
            if 'source' not in p:
                continue
            sx,sy,sw,sh=p['source'];x,y,w,h=p['art']
            piece=atlas.crop((sx,sy,sx+sw,sy+sh)).resize((w,h),Image.Resampling.NEAREST)
            composed.alpha_composite(piece,(x,y))
        composed.save(OUT/f"{data['phase']}_Mapa.png")
        collision=composed.copy();draw=ImageDraw.Draw(collision,'RGBA')
        for wall_data in data['walls']:
            x,y,w,h=wall_data['rect'];draw.rectangle((x,y,x+w,y+h),fill=(240,45,45,90),outline=(255,65,65,220))
        for p in data['props']:
            if p['base']:
                x,y,w,h=p['base'];draw.rectangle((x,y,x+w,y+h),fill=(255,170,30,85),outline=(255,195,55,230))
        for p in data['points']:
            x,y=p['pixel'];draw.ellipse((x-5,y-5,x+5,y+5),fill=(45,180,255,220));draw.text((x+7,y-5),p['id'],fill='white')
        collision.save(OUT/f"{data['phase']}_Colisoes.png")
    (OUT/'Layouts.json').write_text(json.dumps({'layouts':layouts},ensure_ascii=False,indent=2)+'\n',encoding='utf-8')
    if apply:
        for name,image in assets.items():
            image.save(ART/f'{name}.png')
        path=ART/'Layouts.json'; manifest=json.loads(path.read_text(encoding='utf-8'))
        replacements={x['phase']:x for x in layouts}
        manifest['layouts']=[replacements.get(x['phase'],x) for x in manifest['layouts']]
        path.write_text(json.dumps(manifest,ensure_ascii=False,indent=2)+'\n',encoding='utf-8')
    print(json.dumps({'maps':len(layouts),'props':[len(d['props']) for d in layouts],
                      'atlas':atlas.size,'applied':apply}))


if __name__=='__main__':
    parser=argparse.ArgumentParser();parser.add_argument('--sources',required=True,type=Path)
    parser.add_argument('--apply',action='store_true');args=parser.parse_args()
    prepare(args.sources,args.apply)
