"""Offline geometry audit; Unity Edit/PlayMode remains the authoritative runtime check."""
import json
from pathlib import Path
import numpy as np
from collections import deque

ROOT=Path(__file__).resolve().parents[1]
layouts=json.loads((ROOT/'Preview/Casarao20261009/Layouts.json').read_text(encoding='utf-8'))['layouts']
results=[]
for data in layouts:
    scale=data['bounds'][2]/data['width']
    width=round(data['bounds'][2]/.25);height=round(data['bounds'][3]/.25)
    gy,gx=np.mgrid[0:height,0:width]
    xs=gx*.25/scale;ys=data['height']-gy*.25/scale
    def clear(x,y,radius):
        r=radius/scale
        ok=(x>=r)&(x<=data['width']-r)&(y>=r)&(y<=data['height']-r)
        for wall in data['walls']:
            wx,wy,ww,wh=wall['rect']
            dx=x-np.clip(x,wx,wx+ww);dy=y-np.clip(y,wy,wy+wh)
            ok=ok&((dx*dx+dy*dy)>=r*r)
            if wall.get('art') and wh>ww*1.3:
                # Same full-torso guard used by CampaignMapPlan.IsClear, top-left pixels.
                body_top=y-1.61/scale;body_bottom=y-.03/scale
                overlap=(x+r>wx)&(x-r<wx+ww)&(body_bottom>wy)&(body_top<wy+wh)
                ok=ok&np.logical_not(overlap)
        for prop in data['props']:
            if not prop['base']:continue
            wx,wy,ww,wh=prop['base']
            dx=x-np.clip(x,wx,wx+ww);dy=y-np.clip(y,wy,wy+wh)
            ok=ok&((dx*dx+dy*dy)>=r*r)
        return ok
    def cell(p):
        return (int(np.clip(round((data['height']-p[1])*scale/.25),0,height-1)),
                int(np.clip(round(p[0]*scale/.25),0,width-1)))
    mask=clear(xs,ys,.25)
    reachable=np.zeros_like(mask,dtype=bool)
    start=cell(data['spawn']);queue=deque([start])
    if mask[start]:reachable[start]=True
    while queue:
        row,col=queue.popleft()
        for ry,cx in [(row-1,col),(row+1,col),(row,col-1),(row,col+1)]:
            if 0<=ry<height and 0<=cx<width and mask[ry,cx] and not reachable[ry,cx]:
                reachable[ry,cx]=True;queue.append((ry,cx))
    failures=[]
    if not clear(*data['spawn'],.28):failures.append('spawn blocked')
    for point in data['points']:
        p=point['pixel']
        if not clear(*p,.28):failures.append(point['id']+' blocked')
        if not reachable[cell(p)]:failures.append(point['id']+' unreachable')
    results.append({'phase':data['phase'],'points':len(data['points']),'failures':failures})
output=ROOT/'Preview/Casarao20261009/Navigation.json'
output.write_text(json.dumps(results,indent=2)+'\n',encoding='utf-8')
print(json.dumps(results))
raise SystemExit(1 if any(r['failures'] for r in results) else 0)
