using System.Collections.Generic;
using UnityEngine;

namespace Game.Varginha.Experiment
{
    public static class CampaignWallConnections
    {
        // Finish perpendicular wall intersections with one continuous corner block.
        // Visual joins never add collision inside an authored opening.
        public static void Build(Transform parent,IReadOnlyList<Rect> walls,Sprite source,bool wood)
        {
            if(source==null||parent.Find("Juncoes_Paredes")!=null)return;
            var root=new GameObject("Juncoes_Paredes").transform;root.SetParent(parent,false);
            Rect cut=source.rect;
            if(wood)cut.height=Mathf.Floor(cut.height*.38f);
            var corner=Sprite.Create(source.texture,cut,Vector2.one/2,source.pixelsPerUnit,0,SpriteMeshType.FullRect);
            corner.name="Wall_ConnectedCorner";
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
                    var go=new GameObject("Canto conectado");go.transform.SetParent(root,false);go.transform.position=center;
                    var sr=go.AddComponent<SpriteRenderer>();sr.sprite=corner;sr.sortingOrder=4;
                    sr.transform.localScale=new Vector3(vertical.width/corner.bounds.size.x,horizontal.height/corner.bounds.size.y,1);
                }
            }
        }
    }
}
