using System;
using System.Collections.Generic;
using UnityEngine;
using Object=UnityEngine.Object;

namespace Game.Varginha.Experiment
{
    // User artwork and physical floor contacts share one source-image coordinate system.
    // Tall furniture is drawn twice: baked floor art and a shared-texture foreground mesh.
    public static class CampaignIllustratedMaps
    {
        [Serializable]public sealed class Manifest { public Layout[] layouts; }
        [Serializable]public sealed class Layout
        {
            public int phase,width,height;
            public string image,background;
            public float[] bounds,spawn,ambient,laneBorders;
            public bool repeat;
            public Wall[] walls;public Prop[] props;public Point[] points;public Light[] lights;public Patch[] patches;
            // JsonUtility doesn't support nested arrays; seats are derived from desk bounds instead.
            public Rect Bounds=>new(bounds[0],bounds[1],bounds[2],bounds[3]);
            public float Scale=>bounds[2]/width;
            public Vector2 Position(float x,float y)=>new(bounds[0]+x*Scale,Bounds.yMax-y*Scale);
            public Vector2 Position(float[] p)=>Position(p[0],p[1]);
            public Rect Area(float[] p)=>new(Position(p[0],p[1]+p[3]),new Vector2(p[2],p[3])*Scale);
            public Vector2 Objective(string id)=>Position(Array.Find(points,p=>p.id==id).pixel);
        }
        [Serializable]public sealed class Wall {public float[] rect,art;public bool water,skipBodyGuard;}
        [Serializable]public sealed class Prop {public string name,motif;public float[] art, @base,outline;public bool movable;}
        [Serializable]public sealed class Point {public string id;public float[] pixel;}
        [Serializable]public sealed class Light {public float[] pixel,color;public float radius;}
        [Serializable]public sealed class Patch {public float[] sample,rect,ground;public bool cap;}
        private static Manifest _manifest;
        private static readonly Dictionary<string,Sprite> Sprites=new();
        public static void Reload()=>_manifest=null;
        public static Layout Get(int phase)
        {
            if(_manifest==null)
            {
                var source=Resources.Load<TextAsset>("Varginha/IllustratedMaps/Layouts");
                if(source==null)return null;
                _manifest=JsonUtility.FromJson<Manifest>(source.text);
            }
            return Array.Find(_manifest.layouts,l=>l.phase==phase);
        }
        public static bool Apply(CampaignMapPlan plan)
        {
            var data=Get(plan.phase);if(data==null)return false;
            plan.illustrated=true;
            plan.bounds=data.Bounds;plan.spawn=data.Position(data.spawn);
            // Driving progress is measured in world metres, independent of the repeated road art.
            if(plan.phase==3)plan.spawn=new Vector2(data.Position((data.laneBorders[0]+data.laneBorders[1])/2,0).x,0);
            plan.rooms.Clear();plan.walls.Clear();plan.bodyWalls.Clear();plan.furniture.Clear();
            plan.rooms.Add(new CampaignMapPlan.Surface("Cenário ilustrado", "Illustrated",plan.bounds,Color.white));
            if(plan.phase==1)plan.rooms.Add(new CampaignMapPlan.Surface("Circulação da casa","Illustrated",data.Area(new[]{0f,54f,914f,732f}),Color.white));
            float tileHeight=data.height*data.Scale;
            int count=data.repeat?Mathf.CeilToInt(plan.bounds.height/tileHeight):1;
            for(int tile=0;tile<count;tile++)foreach(var wall in data.walls)
            {
                var area=data.Area(wall.rect);if(data.repeat)area.y=plan.bounds.yMin+tile*tileHeight;
                // A feet circle is narrower than the torso. Keep shoulders outside vertical plaster.
                if(!data.repeat&&area.height>area.width*2)
                {float left=Mathf.Max(plan.bounds.xMin,area.xMin-.12f),right=Mathf.Min(plan.bounds.xMax,area.xMax+.12f);area.xMin=left;area.xMax=right;}
                if(data.repeat)area.height=Mathf.Min(tileHeight,plan.bounds.yMax-area.y);
                if(area.height>0)
                {
                    plan.walls.Add(area);
                    if(!wall.water&&!wall.skipBodyGuard&&wall.art?.Length==4&&CampaignWallBody.Guards(plan.phase,area,plan.bounds))plan.bodyWalls.Add(area);
                }
            }
            if(data.repeat)
            {
                plan.walls.Add(new Rect(plan.bounds.xMin,plan.bounds.yMin-.2f,plan.bounds.width,.2f));
                plan.walls.Add(new Rect(plan.bounds.xMin,plan.bounds.yMax,plan.bounds.width,.2f));
            }
            foreach(var prop in data.props)
            {
                var art=data.Area(prop.art);var feet=prop.@base!=null&&prop.@base.Length==4?data.Area(prop.@base):new Rect(art.center,Vector2.zero);
                plan.furniture.Add(new CampaignMapPlan.Furnishing(prop.name,prop.motif,art.center,art.size,feet));
            }
            foreach(var target in data.points)
            {
                var existing=plan.points.Find(p=>p.id==target.id);
                if(existing==null){existing=new CampaignMapPlan.Point(target.id,target.id.ToUpperInvariant(),0,0);plan.points.Add(existing);}
                existing.position=data.Position(target.pixel);
            }
            if(plan.phase==3)
            {
                plan.points.Find(p=>p.id=="start").position=plan.spawn;
                plan.points.Find(p=>p.id=="breakdown").position=new Vector2(plan.spawn.x,55);
                plan.points.Find(p=>p.id=="arrival").position=new Vector2(plan.spawn.x,120);
            }
            return true;
        }
        public static Transform Build(Transform owner,CampaignMapPlan plan,bool furnished)
        {
            var data=Get(plan.phase);var map=new GameObject("Mapa_Campanha").transform;map.SetParent(owner,false);
            var architecture=new GameObject("01_Planta_Paredes_Divisoes").transform;architecture.SetParent(map,false);
            var image=Texture(data.background);
            float tileHeight=data.height*data.Scale;
            int count=data.repeat?Mathf.CeilToInt(plan.bounds.height/tileHeight):1;
            for(int i=0;i<count;i++)
            {
                float height=data.repeat?Mathf.Min(tileHeight,plan.bounds.height-i*tileHeight):plan.bounds.height;
                float sourceHeight=data.repeat?Mathf.Min(image.height,image.height*height/tileHeight):image.height;
                var r=Render(architecture,"Arte_Integrada_"+i,image,new Rect(0,0,image.width,sourceHeight),plan.bounds.width,height);
                r.transform.position=data.repeat?new Vector2(plan.bounds.center.x,plan.bounds.yMin+i*tileHeight+height/2):plan.bounds.center;
                r.sortingOrder=-1000;
            }
            if(data.patches!=null)for(int i=0;i<data.patches.Length;i++)
            {
                var patch=data.patches[i];var area=data.Area(patch.rect);
                var r=Render(architecture,"Portal_aberto_"+i,Texture(data.image),SourceRect(data,patch.sample),area.width,area.height);
                r.transform.position=area.center;r.sortingOrder=patch.cap?-998:-999;
                if(patch.ground?.Length==2)
                {
                    var support=new GameObject("Contato_portal_"+i).transform;support.SetParent(architecture,false);support.position=data.Position(patch.ground);
                    VarginhaWorldDepth.Ensure(r,supportingObject:support,offset:1);
                }
            }
            for(int i=0;i<plan.walls.Count;i++)
            {
                var go=new GameObject("Parede_"+i);go.transform.SetParent(architecture,false);go.transform.position=plan.walls[i].center;
                if(plan.bodyWalls.Contains(plan.walls[i]))go.layer=CampaignWallBody.WallLayer;
                var collider=go.AddComponent<BoxCollider2D>();collider.size=plan.walls[i].size;
                if(!data.repeat&&i<data.walls.Length&&data.walls[i].art?.Length==4)
                {
                    var rect=data.walls[i].art;var area=data.Area(rect);
                    if(plan.phase>=11&&plan.phase!=14&&plan.phase!=15&&plan.phase!=19)
                    {
                        // A long column cannot share one Y-order from top to bottom.
                        // Small strips reuse the same texture and follow their own floor contacts.
                        float strip=rect[3]>rect[2]*1.3f?48:rect[3];
                        for(float row=0;row<rect[3];row+=strip)
                        {
                            var slice=new[]{rect[0],rect[1]+row,rect[2],Mathf.Min(strip,rect[3]-row)};var a=data.Area(slice);
                            var face=Render(go.transform,"Face_da_parede_"+row,Texture(data.image),SourceRect(data,slice),a.width,a.height);face.transform.position=a.center;
                            var contact=new GameObject("Contato_da_parede_"+row).transform;contact.SetParent(go.transform,false);contact.position=new Vector2(a.center.x,a.yMin);
                            VarginhaWorldDepth.Ensure(face,supportingObject:contact);
                        }
                        continue;
                    }
                    var sr=Render(go.transform,"Face_da_parede",Texture(data.image),SourceRect(data,rect),area.width,area.height);
                    sr.transform.position=area.center;
                    VarginhaWorldDepth.Ensure(sr,ground:collider);
                }
            }
            if(furnished)Furnish(map,plan);
            var lighting=map.gameObject.AddComponent<CampaignIllustratedLighting>();lighting.Configure(data);
            if(Application.isPlaying)
            {
                var shadows=Object.FindAnyObjectByType<CampaignCharacterShadows>()??map.gameObject.AddComponent<CampaignCharacterShadows>();
                shadows.Configure(data);
            }
            return map;
        }
        public static void Furnish(Transform map,CampaignMapPlan plan)
        {
            if(map.Find("02_Mobilia_Colisoes")!=null)return;
            var data=Get(plan.phase);var layer=new GameObject("02_Mobilia_Colisoes").transform;layer.SetParent(map,false);
            var texture=Texture(data.image);
            foreach(var prop in plan.furniture)
            {
                var source=Array.Find(data.props,p=>p.name==prop.name);
                var sr=Render(layer,prop.name,texture,SourceRect(data,source.art),prop.size.x,prop.size.y);
                sr.transform.position=prop.position;
                // Car/background must not duplicate after it leaves the workshop.
                if(source.outline?.Length>=6)
                {
                    var outline=new Vector2[source.outline.Length/2];
                    for(int i=0;i<outline.Length;i++)outline[i]=new Vector2(source.outline[i*2],1-source.outline[i*2+1]);
                    Contour(sr,outline);
                }
                if(prop.footprint.width<=0||prop.footprint.height<=0){sr.sortingOrder=2;continue;}
                var collider=sr.gameObject.AddComponent<BoxCollider2D>();
                collider.size=prop.footprint.size/(Vector2)sr.transform.lossyScale;
                collider.offset=(prop.footprint.center-prop.position)/(Vector2)sr.transform.lossyScale;
                if(plan.phase>=11&&plan.phase!=14&&plan.phase!=15&&plan.phase!=19)
                {
                    // A prop's front ground edge decides occlusion, independently of its blocking footprint.
                    var contact=new GameObject("Contato_"+prop.name).transform;contact.SetParent(layer,false);
                    contact.position=new Vector2(prop.footprint.center.x,prop.footprint.yMin+.03f);
                    VarginhaWorldDepth.Ensure(sr,supportingObject:contact);sr.spriteSortPoint=SpriteSortPoint.Pivot;
                }
                else VarginhaWorldDepth.Ensure(sr,ground:collider);
            }
        }
        private static Texture2D Texture(string id)=>Resources.Load<Texture2D>("Varginha/IllustratedMaps/"+id)
            ??throw new InvalidOperationException("Imagem de cenário ausente: "+id);
        private static Rect SourceRect(Layout data,float[] rect)
        {
            var t=Texture(data.image);float x=t.width/(float)data.width,y=t.height/(float)data.height;
            return new Rect(rect[0]*x,t.height-(rect[1]+rect[3])*y,rect[2]*x,rect[3]*y);
        }
        private static SpriteRenderer Render(Transform parent,string name,Texture2D tex,Rect rect,float width,float height)
        {
            // Every crop references the same GPU texture. No Texture2D copies or per-frame slicing.
            string key=tex.name+":"+rect;
            if(!Sprites.TryGetValue(key,out var sprite)||sprite==null)
            {
                sprite=Sprite.Create(tex,rect,Vector2.one/2,100,0,SpriteMeshType.FullRect);sprite.name="Illustrated_"+name;Sprites[key]=sprite;
            }
            var go=new GameObject(name);go.transform.SetParent(parent,false);
            var sr=go.AddComponent<SpriteRenderer>();sr.sprite=sprite;
            // Tightening the cached mesh changes bounds; the source rect fixes world scale.
            go.transform.localScale=new Vector3(width*sprite.pixelsPerUnit/sprite.rect.width,height*sprite.pixelsPerUnit/sprite.rect.height,1);return sr;
        }
        private static void Contour(SpriteRenderer sr,Vector2[] contour)
        {
            // OverrideGeometry affects only the mesh, never the user PNG or its alpha.
            var sprite=sr.sprite;var size=sprite.rect.size;
            var vertices=new Vector2[contour.Length];for(int i=0;i<vertices.Length;i++)vertices[i]=contour[i]*size;
            var triangles=CampaignIllustratedContour.Triangulate(contour);
            // Unity allows this API only inside its player loop, after Awake.
            // The unmodified source image provides the identical editor preview.
            if(Application.isPlaying)sr.gameObject.AddComponent<CampaignIllustratedContour>().Configure(sprite,vertices,triangles);
        }
        public static Vector2[] SchoolSeats(int phase)
        {
            var data=Get(phase);var result=new List<Vector2>();
            foreach(var p in data.props)if(p.name.StartsWith("Carteira original"))
            {
                var chair=Array.Find(data.props,c=>c.name=="Cadeira azul "+p.name.Substring("Carteira original ".Length));
                result.Add(data.Position(chair.art[0]+chair.art[2]/2,chair.art[1]+chair.art[3]+22)+Vector2.up*.58f);
            }
            return result.ToArray();
        }
        public static void ApplyAdultHouse(Transform owner)
        {
            var actor=Object.FindAnyObjectByType<EdelzioTopDownController>();
            foreach(var sr in Object.FindObjectsByType<SpriteRenderer>(FindObjectsInactive.Include))
                if(sr.GetComponentInParent<EdelzioTopDownController>()==null){sr.enabled=false;if(sr.name=="Backpack_Prop")sr.name="Backpack_Original_Anchor";}
            foreach(var collider in Object.FindObjectsByType<Collider2D>(FindObjectsInactive.Include))
                if(collider.GetComponentInParent<EdelzioTopDownController>()==null)collider.enabled=false;
            var plan=CampaignMapPlan.Create(2);var map=Build(owner,plan,true);var data=Get(2);
            // The authored adult player bypasses CreatePlayer, so it also needs foot sorting.
            VarginhaWorldDepth.Ensure(actor.GetComponent<SpriteRenderer>(),ground:actor.GetComponent<CircleCollider2D>());
            // Existing action scripts keep their targets; their visual artwork is in the map.
            Anchor("Notebook_TI",data.Position(65,456));Anchor("Chair_Office",data.Position(80,551));Anchor("Coffee_Cup",data.Position(705,520));
            CampaignSeatingLayers.Attach(actor.GetComponent<SpriteRenderer>(),map.Find("02_Mobilia_Colisoes/Office_Chair")?.GetComponent<SpriteRenderer>(),false,.64f,.85f,.15f);
            // The pickup still uses the established action; the new sprite is a separate object.
            var bag=map.Find("02_Mobilia_Colisoes/Backpack_Prop");
            if(bag!=null){var renderer=bag.GetComponent<SpriteRenderer>();var art=renderer.sprite;bag.gameObject.AddComponent<BackpackPickupAnimation>();renderer.sprite=art;}
            var camera=Camera.main;if(camera!=null)camera.GetComponent<Game.Level.CameraFollow2D>()?.ConfigureMap(actor.transform,plan.bounds,6);
            Physics2D.SyncTransforms();
        }
        private static void Anchor(string name,Vector2 position){var go=GameObject.Find(name);if(go!=null)go.transform.position=position;}
    }
}
