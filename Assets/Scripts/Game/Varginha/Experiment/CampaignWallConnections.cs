using System.Collections.Generic;
using UnityEngine;

namespace Game.Varginha.Experiment
{
    public static class CampaignWallConnections
    {
        // Finish perpendicular wall intersections with one continuous corner block.
        // Visual joins never add collision inside an authored opening.
        public static void Build(Transform parent,IReadOnlyList<Rect> walls,Sprite source,bool wood,System.Func<Vector2,Sprite> sourceAt=null)
        {
            if(source==null||parent.Find("Juncoes_Paredes")!=null)return;
            var root=new GameObject("Juncoes_Paredes").transform;root.SetParent(parent,false);
            Rect cut=source.rect;
            if(wood)
            {
                cut.height=Mathf.Floor(cut.height*.32f);
                cut.x+=Mathf.Floor((cut.width-cut.height)/2);cut.width=cut.height;
            }
            var corner=Sprite.Create(source.texture,cut,Vector2.one/2,source.pixelsPerUnit,0,SpriteMeshType.FullRect);
            corner.name="Wall_ConnectedCorner";
            var corners=new Dictionary<Sprite,Sprite>{{source,corner}};
            var positions=new HashSet<Vector2>();
            foreach(var horizontal in walls)
            {
                if(horizontal.width<=horizontal.height)continue;
                foreach(var vertical in walls)
                {
                    if(vertical.height<=vertical.width)continue;
                    if(horizontal.xMin>vertical.xMax+.02f||horizontal.xMax<vertical.xMin-.02f
                        ||horizontal.yMin>vertical.yMax+.02f||horizontal.yMax<vertical.yMin-.02f)continue;
                    var center=new Vector2(vertical.center.x,horizontal.center.y);
                    if(!positions.Add(center))continue;
                    var material=sourceAt?.Invoke(center)??source;
                    if(!corners.TryGetValue(material,out corner))
                    {
                        var rect=material.rect;
                        if(wood){rect.height=Mathf.Floor(rect.height*.32f);rect.x+=Mathf.Floor((rect.width-rect.height)/2);rect.width=rect.height;}
                        corner=Sprite.Create(material.texture,rect,Vector2.one/2,material.pixelsPerUnit,0,SpriteMeshType.FullRect);
                        corner.name="Wall_ConnectedCorner";corners[material]=corner;
                    }
                    var go=new GameObject("Canto conectado");go.transform.SetParent(root,false);go.transform.position=center;
                    var sr=go.AddComponent<SpriteRenderer>();sr.sprite=corner;sr.sortingOrder=4;
                    sr.transform.localScale=new Vector3(vertical.width/corner.bounds.size.x,horizontal.height/corner.bounds.size.y,1);
                }
            }
        }
    }
}
