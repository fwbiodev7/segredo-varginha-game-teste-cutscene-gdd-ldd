using System.Collections.Generic;
using UnityEngine;

namespace Game.Varginha.Experiment
{
    // Reuses the existing materials. Floor UVs follow one world grid; every wall
    // contains its complete plaster/baseboard profile, regardless of thickness.
    public static class CampaignArchitectureRenderer
    {
        private static readonly Dictionary<(Sprite,RectInt),Sprite> Crops=new();
        private static readonly Dictionary<Sprite,Sprite> Trims=new();
        public static Sprite FloorMaterial(string motif)
            =>CampaignOuzanaArt.Floor(motif)??(motif=="Floor_House"?CampaignAdultHouse.TextureTile("WoodRequested",1.5f):CampaignVisualAssets.Floor(motif));

        public static void Floor(Transform parent,string name,Rect area,Sprite material,int order)
        {
            if(material==null)return;
            var root=new GameObject(name).transform;root.SetParent(parent,false);
            float tw=material.bounds.size.x,th=material.bounds.size.y;
            int x0=Mathf.FloorToInt(area.xMin/tw),y0=Mathf.FloorToInt(area.yMin/th);
            int x1=Mathf.CeilToInt(area.xMax/tw),y1=Mathf.CeilToInt(area.yMax/th);
            for(int y=y0;y<y1;y++)for(int x=x0;x<x1;x++)
            {
                var tile=new Rect(x*tw,y*th,tw,th);
                float left=Mathf.Max(tile.xMin,area.xMin),right=Mathf.Min(tile.xMax,area.xMax);
                float bottom=Mathf.Max(tile.yMin,area.yMin),top=Mathf.Min(tile.yMax,area.yMax);
                if(right-left<.001f||top-bottom<.001f)continue;
                var pixels=new RectInt(Mathf.Clamp(Mathf.RoundToInt((left-tile.xMin)/tw*material.rect.width),0,(int)material.rect.width-1),
                    Mathf.Clamp(Mathf.RoundToInt((bottom-tile.yMin)/th*material.rect.height),0,(int)material.rect.height-1),
                    Mathf.Max(1,Mathf.RoundToInt((right-left)/tw*material.rect.width)),
                    Mathf.Max(1,Mathf.RoundToInt((top-bottom)/th*material.rect.height)));
                pixels.width=Mathf.Min(pixels.width,(int)material.rect.width-pixels.x);
                pixels.height=Mathf.Min(pixels.height,(int)material.rect.height-pixels.y);
                if(!Crops.TryGetValue((material,pixels),out var crop)||crop==null)
                {
                    crop=Sprite.Create(material.texture,new Rect(material.rect.x+pixels.x,material.rect.y+pixels.y,pixels.width,pixels.height),Vector2.one/2,material.pixelsPerUnit,0,SpriteMeshType.FullRect);
                    crop.name="Piso_Continuo";Crops[(material,pixels)]=crop;
                }
                var go=new GameObject("Piso");
                go.transform.position=new Vector3((left+right)/2,(bottom+top)/2);
                go.transform.localScale=new Vector3((right-left)/crop.bounds.size.x,(top-bottom)/crop.bounds.size.y,1);
                go.transform.SetParent(root,true);
                var renderer=go.AddComponent<SpriteRenderer>();renderer.sprite=crop;renderer.sortingOrder=order;
            }
        }

        public static SpriteRenderer Wall(Transform parent,string name,Rect footprint,Sprite material,bool visible=true,bool collision=true)
        {
            var go=new GameObject(name);go.transform.position=footprint.center;
            bool vertical=footprint.height>footprint.width;
            float cross=vertical?footprint.width:footprint.height;
            go.transform.rotation=Quaternion.Euler(0,0,vertical?90:0);
            go.transform.localScale=new Vector3(1,cross/material.bounds.size.y,1);
            go.transform.SetParent(parent,true);
            var renderer=go.AddComponent<SpriteRenderer>();renderer.sprite=material;renderer.sortingOrder=3;
            renderer.drawMode=SpriteDrawMode.Tiled;
            renderer.size=new Vector2(vertical?footprint.height:footprint.width,material.bounds.size.y);
            renderer.enabled=visible;
            if(collision)
            {
                var collider=go.AddComponent<BoxCollider2D>();collider.size=renderer.size;
                VarginhaWorldDepth.Ensure(renderer,background:true,ground:collider);
            }
            if(visible)
            {
                var trim=Trim(material);const float edge=.0625f;
                if(vertical)
                {
                    Strip(go.transform,"Moldura externa",new Rect(footprint.xMin,footprint.yMin,edge,footprint.height),trim,true);
                    Strip(go.transform,"Rodapé lateral",new Rect(footprint.xMax-edge,footprint.yMin,edge,footprint.height),trim,true);
                }
                else
                {
                    Strip(go.transform,"Viga contínua",new Rect(footprint.xMin,footprint.yMax-edge,footprint.width,edge),trim,false);
                    Strip(go.transform,"Rodapé contínuo",new Rect(footprint.xMin,footprint.yMin,footprint.width,edge),trim,false);
                }
            }
            return renderer;
        }
        private static Sprite Trim(Sprite source)
        {
            if(Trims.TryGetValue(source,out var cached)&&cached!=null)return cached;
            var rect=source.rect;rect.height=Mathf.Max(3,Mathf.Round(rect.height*.055f));
            var sprite=Sprite.Create(source.texture,rect,Vector2.one/2,source.pixelsPerUnit,0,SpriteMeshType.FullRect);
            sprite.name="Moldura_Material_Existente";Trims[source]=sprite;return sprite;
        }
        private static void Strip(Transform parent,string name,Rect area,Sprite trim,bool vertical)
        {
            var go=new GameObject(name);go.transform.position=area.center;
            go.transform.rotation=Quaternion.Euler(0,0,vertical?90:0);
            go.transform.localScale=new Vector3(1,(vertical?area.width:area.height)/trim.bounds.size.y,1);
            go.transform.SetParent(parent,true);
            var renderer=go.AddComponent<SpriteRenderer>();renderer.sprite=trim;renderer.drawMode=SpriteDrawMode.Tiled;
            renderer.size=new Vector2(vertical?area.height:area.width,trim.bounds.size.y);
            renderer.sortingOrder=4;renderer.color=new Color(.78f,.75f,.71f,1);
        }
    }
}
