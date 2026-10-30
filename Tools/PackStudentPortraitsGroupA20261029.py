"""Save generated whole busts with alpha and nearest-neighbour reduction only."""
from pathlib import Path
import hashlib
import json
import shutil
import uuid
from PIL import Image,ImageDraw,ImageFont

ROOT=Path(__file__).resolve().parents[1]
DELIVERY=ROOT/'ArtSources/StudentPortraits20261029'
GENERATED=Path('C:/Users/Usuario/.codex/generated_images/01a1895f-9579-7540-b0ef-6c20c9ef69e9')
FILES={'Fabio':'exec-4e455a48-97e8-4c7f-a2f7-c1bef53a8a0c.png','Marcos':'exec-f1e2dbd5-2364-4f12-a4d9-52f19c591fb6.png','Matias':'exec-8b1ea7fd-b4d9-41af-8cdb-2af1dde7a7c1.png','AnnaSabia':'exec-c0942cdc-2403-4e4f-956d-12f8c69d50ef.png'}
LABELS={'Fabio':'Fábio','Marcos':'Marcos','Matias':'Matias','AnnaSabia':'Anna Sabiá'}
OUT=ROOT/'Assets/Resources/Varginha/Allies/Portraits'
OUT.mkdir(parents=True,exist_ok=True)
contact=Image.new('RGB',(4*288,336),(15,29,37));draw=ImageDraw.Draw(contact)
font=ImageFont.truetype('C:/Windows/Fonts/consola.ttf',21)
statistics={}
for i,(name,file) in enumerate(FILES.items()):
    folder=DELIVERY/name;folder.mkdir(parents=True,exist_ok=True)
    source=folder/'generated-source.png'
    shutil.copy2(GENERATED/file,source)
    image=Image.open(source).convert('RGBA')
    final=image.resize((256,256),Image.Resampling.NEAREST)
    assert final.getchannel('A').getextrema()[0]==0
    assert final.getchannel('A').getextrema()[1]==255
    path=OUT/(name+'.png');final.save(path)
    final.save(folder/'portrait.png')
    meta=path.with_suffix('.png.meta')
    if not meta.exists():
        meta.write_text('''fileFormatVersion: 2
guid: %s
TextureImporter:
  internalIDToNameTable: []
  externalObjects: {}
  serializedVersion: 13
  mipmaps:
    enableMipMap: 0
    sRGBTexture: 1
  isReadable: 1
  textureFormat: 1
  maxTextureSize: 1024
  textureSettings:
    serializedVersion: 2
    filterMode: 0
    aniso: 1
    mipBias: 0
    wrapU: 1
    wrapV: 1
    wrapW: 1
  nPOTScale: 0
  spriteMode: 0
  alphaUsage: 1
  alphaIsTransparency: 1
  textureType: 0
  textureShape: 1
  platformSettings:
  - serializedVersion: 4
    buildTarget: DefaultTexturePlatform
    maxTextureSize: 1024
    resizeAlgorithm: 0
    textureFormat: -1
    textureCompression: 0
    compressionQuality: 100
    crunchedCompression: 0
    allowsAlphaSplitting: 0
    overridden: 0
  userData:
  assetBundleName:
  assetBundleVariant:
'''%uuid.uuid4().hex,encoding='utf8')
    info={'name':LABELS[name],'assetId':name,'generator':'built-in image_gen','status':'visual-reviewed','source':{'file':source.name,'size':list(image.size),'sha256':hashlib.sha256(source.read_bytes()).hexdigest()},'image':{'path':str(path.relative_to(ROOT)).replace('\\','/'),'size':[256,256],'sha256':hashlib.sha256(path.read_bytes()).hexdigest()},'postprocess':'uniform whole-image nearest-neighbour reduction; original alpha preserved','logicalCanvas':[256,256],'alphaExtrema':list(final.getchannel('A').getextrema()),'bounds':final.getbbox(),'unityRuntimeVerified':False}
    (folder/'manifest.json').write_text(json.dumps(info,ensure_ascii=False,indent=2),encoding='utf8')
    statistics[name]=info['source']['size']
    contact.paste(final,(i*288+16,25),final)
    width=draw.textbbox((0,0),LABELS[name],font=font)[2]
    draw.text((i*288+(288-width)//2,300),LABELS[name],font=font,fill='white')
contact.save(DELIVERY/'Contact_GroupA_4Portraits.png')
print(json.dumps({'portraits':statistics,'size':[256,256],'transparent':True}))
