"""Compose whole runtime sprites for the twelve-student visual review."""
from pathlib import Path
from PIL import Image, ImageDraw, ImageFont

ROOT=Path(__file__).resolve().parents[1]
OUTPUT=ROOT/'Preview/Students/Redesign20261029/Elenco_12.png'
NAMES=[('Fabio','Fábio'),('Marcos','Marcos'),('Matias','Matias'),
       ('AnnaSabia','Anna Sabiá'),('AnaTavares','Ana Tavares'),('Yasmin','Yasmin'),
       ('Pedro','Pedro'),('LuisMartins','Luis Martins'),('LuisMiguelMessias','Luis Miguel Messias'),
       ('Giovana','Giovana'),('Bianca','Bianca'),('Moscardini','Moscardini')]
FONT=Path('C:/Windows/Fonts/consola.ttf')
font=lambda size:ImageFont.truetype(str(FONT),size)
image=Image.new('RGB',(1848,1150),(9,20,27))
draw=ImageDraw.Draw(image)
draw.text((30,22),'ALUNOS ALIADOS',font=font(36),fill=(235,244,244))
draw.text((30,69),'Elenco completo • Frente, perfil e costas',font=font(21),fill=(151,192,190))
for i,(key,label) in enumerate(NAMES):
    x=24+(i%6)*304;y=120+(i//6)*504
    draw.rounded_rectangle((x,y,x+280,y+480),radius=12,fill=(17,34,42),outline=(48,80,87),width=2)
    title_font=font(22 if len(label)<17 else 18)
    title_box=draw.textbbox((0,0),label,font=title_font)
    draw.text((x+(280-title_box[2])/2,y+22),label,font=title_font,fill=(238,245,240))
    atlas=Image.open(ROOT/'Assets/Resources/Varginha/Allies'/f'{key}.png').convert('RGBA')
    assert atlas.size==(256,256),(key,atlas.size)
    for row,scale,dx,dy in [(0,3,44,65),(1,2,8,293),(3,2,144,293)]:
        sprite=atlas.crop((0,row*64,64,row*64+64))
        sprite=sprite.resize((64*scale,64*scale),Image.Resampling.NEAREST)
        image.paste(sprite,(x+dx,y+dy),sprite)
    for text,cx in [('PERFIL',x+72),('COSTAS',x+208)]:
        box=draw.textbbox((0,0),text,font=font(16))
        draw.text((cx-box[2]/2,y+438),text,font=font(16),fill=(147,179,182))
    text='FRENTE';box=draw.textbbox((0,0),text,font=font(16))
    draw.text((x+140-box[2]/2,y+259),text,font=font(16),fill=(147,179,182))
draw.text((30,1113),'O Segredo de Varginha • Redesign dos alunos • 29/10/2026',font=font(18),fill=(122,159,162))
OUTPUT.parent.mkdir(parents=True,exist_ok=True)
image.save(OUTPUT)
print(OUTPUT)
