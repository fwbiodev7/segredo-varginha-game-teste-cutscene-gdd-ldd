"""Mechanical assembly of image_gen outputs; original art remains untouched."""
from pathlib import Path
from PIL import Image, ImageDraw, ImageChops
import hashlib, json, shutil

ROOT = Path(__file__).resolve().parents[2]
OUT = ROOT / 'output/house-interactions-2026-10-22/art'
ASSETS = ROOT / 'Assets/Resources/Varginha/HouseFurniture'
def digest(path): return hashlib.sha256(path.read_bytes()).hexdigest()
def write(path, data): path.write_text(json.dumps(data, ensure_ascii=False, indent=2)+'\n', encoding='utf-8')

def main():
    ASSETS.mkdir(parents=True, exist_ok=True)
    original_path = ROOT / 'Assets/Resources/Varginha/IllustratedMaps/AdultEmptyDivisoes.png'
    original = Image.open(original_path).convert('RGBA')
    edited = Image.open(OUT/'clean-source.png').convert('RGBA')
    assert original.size == edited.size == (1888, 833)
    # Include the full old faucet, coffee mug, desk accessories and contact shadows.
    # These are source-image coordinates, rather than the smaller collider/art crops.
    regions = {
        'dresser': [24, 280, 126, 394], 'desk': [24, 428, 146, 511],
        'bookshelves': [24, 638, 238, 754], 'coffee_table': [647, 493, 765, 589],
        'stove_and_sink': [485, 659, 655, 755], 'fridge': [844, 623, 926, 756],
        'desk_left_leg': [24, 510, 58, 534], 'desk_right_leg': [113, 510, 146, 534],
    }
    mask = Image.new('L', original.size)
    draw = ImageDraw.Draw(mask)
    for x0,y0,x1,y1 in regions.values(): draw.rectangle((x0,y0,x1-1,y1-1), fill=255)
    base = Image.composite(edited, original, mask)
    # Exact pixel preservation outside the approved removal mask.
    difference = ImageChops.difference(base, original).convert('RGB')
    assert ImageChops.multiply(difference, Image.merge('RGB', (ImageChops.invert(mask),)*3)).getbbox() is None
    base.save(ASSETS/'AdultInteractiveBase.png')
    mask.save(OUT/'removal-mask.png')
    write(OUT/'clean-base-provenance.json', {
        'tool': 'built-in image_gen; masked compositing only', 'source': str(original_path.relative_to(ROOT)),
        'sourceSha256': digest(original_path), 'generatedSha256': digest(OUT/'clean-source.png'),
        'regionsXYXY': regions, 'outsideMaskPixelsIdentical': True,
        'resultSha256': digest(ASSETS/'AdultInteractiveBase.png'),
    })
    source = Image.open(OUT/'items-source.png').convert('RGBA')
    w,h = source.size
    sprites = [('CoffeeFull',(0,0,w//2,h//2),(30,29)),
               ('Notebook',(w//2,0,w,h//2),(41,38)),
               ('DeskLamp',(0,h//2,w//2,h),(27,38)),
               ('CoffeeEmpty',(w//2,h//2,w,h),(22,24))]
    folder = OUT/'items'; folder.mkdir(exist_ok=True)
    for name, region, target in sprites:
        item = source.crop(region)
        item.putalpha(item.getchannel('A').point(lambda a: 255 if a >= 128 else 0))
        item = item.crop(item.getchannel('A').getbbox())
        item.thumbnail(target, Image.Resampling.NEAREST)
        canvas = Image.new('RGBA', (64,64))
        canvas.paste(item, ((64-item.width)//2,64-item.height))
        canvas.save(folder/(name+'.png'))
    recipe = {'cell':[64,64], 'columns':2, 'padding':1, 'pixelsPerUnit':100,
              'pivot':[32,64], 'durationMs':100,
              'frames':[{'id':n, 'source':n+'.png'} for n,_,_ in sprites],
              'clips':{n:{'frames':[n], 'loop':False} for n,_,_ in sprites}}
    write(folder/'recipe.json', recipe)
    write(folder/'provenance.json', {'tool':'built-in image_gen', 'sourceSha256':digest(OUT/'items-source.png'),
          'normalization':'Alpha bbox crop; nearest native scale; alpha threshold 128; no background removal by color key',
          'reference':str(original_path.relative_to(ROOT)), 'sourceSize':list(source.size)})

if __name__ == '__main__': main()
