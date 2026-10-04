using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Varginha.Experiment
{
    public sealed class CampaignIllustratedContour:MonoBehaviour
    {
        // Ear clipping preserves the empty space below tables and between furniture legs.
        // A triangle fan would paint those concave gaps with opaque floor pixels.
        public static ushort[] Triangulate(Vector2[] points)
        {
            var remaining=new List<int>();float area=0;
            for(int i=0;i<points.Length;i++)area+=Cross(points[i],points[(i+1)%points.Length]);
            for(int i=0;i<points.Length;i++)remaining.Add(area>0?i:points.Length-1-i);
            var result=new List<ushort>();
            while(remaining.Count>3)
            {
                bool clipped=false;
                for(int i=0;i<remaining.Count;i++)
                {
                    int a=remaining[(i+remaining.Count-1)%remaining.Count],b=remaining[i],c=remaining[(i+1)%remaining.Count];
                    if(Cross(points[b]-points[a],points[c]-points[b])<=.000001f)continue;
                    bool occupied=false;
                    foreach(int p in remaining)
                        if(p!=a&&p!=b&&p!=c&&Inside(points[p],points[a],points[b],points[c])){occupied=true;break;}
                    if(occupied)continue;
                    result.Add((ushort)a);result.Add((ushort)b);result.Add((ushort)c);remaining.RemoveAt(i);clipped=true;break;
                }
                if(!clipped)throw new InvalidOperationException("Furniture contour must be a simple nondegenerate polygon.");
            }
            foreach(int p in remaining)result.Add((ushort)p);
            return result.ToArray();
        }
        private static float Cross(Vector2 a,Vector2 b)=>a.x*b.y-a.y*b.x;
        private static bool Inside(Vector2 p,Vector2 a,Vector2 b,Vector2 c)=>
            Cross(b-a,p-a)>=-.000001f&&Cross(c-b,p-b)>=-.000001f&&Cross(a-c,p-c)>=-.000001f;
        private Sprite _sprite;
        private Vector2[] _vertices;
        private ushort[] _triangles;
        public void Configure(Sprite sprite,Vector2[] vertices,ushort[] triangles)
        { _sprite=sprite;_vertices=vertices;_triangles=triangles; }
        private void Start()
        {
            if(GetComponent<SpriteRenderer>().sprite==_sprite)_sprite.OverrideGeometry(_vertices,_triangles);
            Destroy(this);
        }
    }
}
