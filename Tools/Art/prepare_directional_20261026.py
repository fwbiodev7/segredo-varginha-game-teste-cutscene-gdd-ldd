#!/usr/bin/env python3
"""Normalize authored 8x8 directional sheets without changing the sources."""
from __future__ import annotations
import hashlib, json, pathlib, uuid
import numpy as np
import cv2
from PIL import Image

ROOT = pathlib.Path(__file__).resolve().parents[2]
SOURCES = ROOT / "output/directional-2026-10-26/sources.json"
DEST = ROOT / "Assets/Resources/Varginha/StoryCharacters/Directional20261026"
# 256px cells keep a generous transparent gutter around every directional pose
# while staying at Unity's 2048px maximum for a single 8x8 atlas.
CELL, ALPHA = 256, 250
SOURCE_GUTTER = 12
TARGET = {"Edelzio":1.50,"EdelzioNoBag":1.50,"EdelzioChild":1.08,"Renan":1.50,"PadreFabio":1.50,"Marcos":1.12,"Yasmin":1.12,"Pedro":1.12,"Matias":1.12,"AnaTavares":1.12,"AnnaSabia":1.12,"LuisMartins":1.12,"LuisMiguelMessias":1.12,"Fabio":1.12,"ManifestationBoss":4.60}

def write_meta(path, importer, guid):
    # Re-rendering pixels must not change Unity asset identity. Keep the existing
    # GUID so scenes, prefabs, and any direct references remain valid.
    meta_path = pathlib.Path(str(path)+".meta")
    if meta_path.is_file():
        import re
        match = re.search(r"(?m)^guid: ([0-9a-f]{32})$", meta_path.read_text(encoding="utf-8"))
        if match:
            guid = match.group(1)
    text = f"fileFormatVersion: 2\nguid: {guid}\n{importer}:\n"
    if importer == "TextureImporter":
        text += "  serializedVersion: 13\n  mipmaps:\n    enableMipMap: 0\n  isReadable: 0\n  textureSettings:\n    filterMode: 0\n    aniso: 1\n    mipBias: 0\n    wrapU: 1\n    wrapV: 1\n    wrapW: 1\n  nPOTScale: 0\n  maxTextureSize: 2048\n  textureType: 0\n  textureShape: 1\n  alphaIsTransparency: 1\n  textureCompression: 0\n"
    else:
        text += "  externalObjects: {}\n  userData: \n  assetBundleName: \n  assetBundleVariant: \n"
    pathlib.Path(str(path)+".meta").write_text(text, encoding="utf-8")

def main():
    DEST.mkdir(parents=True, exist_ok=True)
    jobs = json.loads(SOURCES.read_text(encoding="utf-8"))["assets"]
    results=[]
    for job in jobs:
        src=pathlib.Path(job["path"])
        if not src.is_file(): raise FileNotFoundError(src)
        key=job["id"]
        raw=np.asarray(Image.open(src).convert("RGBA")); h,w=raw.shape[:2]
        sheet=Image.new("RGBA",(CELL*8,CELL*8),(0,0,0,0)); frames=[]; heights=[]
        for row in range(8):
            y0,y1=round(row*h/8),round((row+1)*h/8)
            for col in range(8):
                x0,x1=round(col*w/8),round((col+1)*w/8)
                # Some generated poses touch or cross their nominal source
                # grid (notably Edelzio's right-facing profile). Read a small
                # overlap, then select the component anchored inside this
                # source cell so the missing nose/hair/shoes are recovered
                # without importing a neighboring pose.
                sx0,sx1=max(0,x0-SOURCE_GUTTER),min(w,x1+SOURCE_GUTTER)
                sy0,sy1=max(0,y0-SOURCE_GUTTER),min(h,y1+SOURCE_GUTTER)
                tile=raw[sy0:sy1,sx0:sx1].copy()
                # Marcos' generated walk rows accidentally inherit a blue ball
                # fragment at the top of one cell. He walks empty-handed; remove
                # only saturated blue from the upper body, keeping blue shoes.
                if key=="Marcos" and col<4:
                    upper=np.indices(tile.shape[:2])[0] < tile.shape[0]*.34
                    blue=(tile[:,:,2]>90)&(tile[:,:,2]>tile[:,:,0]*1.35)&(tile[:,:,2]>tile[:,:,1]*1.18)
                    tile[:,:,3][upper&blue]=0
                # The generated Edelzio walk sheets contain detached near-black
                # contour pixels around hair, hands and shoes. They read as
                # clipping/texture debris in motion. Keep the intended dark
                # outlines beside colored pixels, but discard dark spill more
                # than one pixel beyond the actual figure. Facial details and
                # dark clothing remain because they touch the colored silhouette.
                if key in {"Edelzio", "EdelzioNoBag"}:
                    opaque=tile[:,:,3]>=ALPHA
                    # Many of the trousers and shoes are intentionally very
                    # dark blue/gray, so only pure black spill is treated as
                    # contour noise.
                    colored=opaque & (tile[:,:,:3].max(axis=2)>4)
                    outline=cv2.dilate(colored.astype("uint8"),np.ones((3,3),np.uint8),iterations=1)>0
                    dark=tile[:,:,:3].max(axis=2)<=4
                    tile[:,:,3][dark & ~outline]=0
                active=(tile[:,:,3]>=ALPHA).astype("uint8")
                # Image generation occasionally spills a neighboring frame into
                # the gutter. Pick the component with most opaque pixels inside
                # the nominal cell, then
                # retain only parts touching it after a 1px dilation (e.g. a
                # disconnected hand-held prop). This removes clipped ghosts at
                # the cell edges that otherwise show up behind feet in Unity.
                count,labels,stats,centroids=cv2.connectedComponentsWithStats(active,8)
                keep=np.zeros(active.shape,dtype=bool)
                if count>1:
                    cx0,cx1=x0-sx0,x1-sx0
                    cy0,cy1=y0-sy0,y1-sy0
                    central=labels[cy0:cy1,cx0:cx1]
                    core_counts=np.bincount(central.ravel(),minlength=count)
                    core_counts[0]=0
                    main=int(np.argmax(core_counts))
                    if core_counts[main]>0:
                        core=(labels==main).astype("uint8")
                        near=cv2.dilate(core,np.ones((3,3),np.uint8),iterations=1)>0
                        keep|=labels==main
                        for label in range(1,count):
                            if label==main: continue
                            area=int(stats[label,cv2.CC_STAT_AREA])
                            if area>=12 and np.any(near & (labels==label)):
                                keep|=labels==label
                tile[:,:,3]=np.where(keep,255,0)
                sprite=Image.fromarray(tile,"RGBA"); bounds=sprite.getchannel("A").getbbox()
                if bounds is None: raise ValueError(f"empty frame {job['id']} {row},{col}")
                sprite=sprite.crop(bounds)
                if sprite.width>CELL-4 or sprite.height>CELL-4: raise ValueError(f"oversize {job['id']} {row},{col}")
                px=(CELL-sprite.width)//2; py=CELL-sprite.height-8
                sheet.alpha_composite(sprite,(col*CELL+px,row*CELL+py))
                frames.append({"rect":[col*CELL+px,row*CELL+py,sprite.width,sprite.height],"anchorX":col*CELL+px+sprite.width/2,"lift":0.0})
                if col<4: heights.append(sprite.height)
        ppu=float(np.median(heights))/TARGET[job["id"]]
        if not 20<ppu<220: raise ValueError(f"unexpected PPU {job['id']} {ppu}")
        png=DEST/f"{key}.png"; js=DEST/f"{key}.json"
        sheet.save(png)
        manifest={"source":f"{key} directional sheet, hard alpha threshold {ALPHA}","width":sheet.width,"height":sheet.height,"columns":8,"rows":8,"pixelsPerUnit":[ppu]*8,"frames":frames,"directions":["S","SW","W","NW","N","NE","E","SE"],"poses":["walk_0","walk_1","walk_2","walk_3","windup","strike","followthrough","recovery"]}
        js.write_text(json.dumps(manifest,ensure_ascii=False,indent=2),encoding="utf-8")
        write_meta(png,"TextureImporter",uuid.uuid4().hex); write_meta(js,"TextScriptImporter",uuid.uuid4().hex)
        results.append({"id":key,"source":str(src),"sha256":hashlib.sha256(src.read_bytes()).hexdigest(),"sourceSize":[w,h],"ppu":round(ppu,3),"targetHeight":TARGET[key],"frameCount":len(frames)})
        print(f"{key}: 64 frames, {w}x{h}, target {TARGET[key]:.2f}u, PPU {ppu:.2f}")
    (ROOT/"output/directional-2026-10-26/asset-register.json").write_text(json.dumps(results,ensure_ascii=False,indent=2),encoding="utf-8")

if __name__=="__main__": main()
