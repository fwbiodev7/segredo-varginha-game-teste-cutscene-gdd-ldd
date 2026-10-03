using System.Collections.Generic;
using UnityEngine;

namespace Game.Varginha.Experiment
{
    // Runtime visual corrections requested after testing; the adult scene and wall colliders stay authored.
    public static class CampaignAdultHouse
    {
        public static readonly Vector2 WashApproach=new(4.6f,-4.15f);
        private static readonly Dictionary<string,Sprite> Sprites=new();
        public static void Apply()
        {
            foreach(var renderer in Object.FindObjectsByType<SpriteRenderer>(FindObjectsInactive.Include,FindObjectsSortMode.None))
            {
                string id=renderer.name;
                if(id.StartsWith("Door_")||id.StartsWith("Doorway_")){renderer.gameObject.SetActive(false);continue;}
                if(id=="Wall_Bedroom_H"){renderer.enabled=false;foreach(var collider in renderer.GetComponents<Collider2D>())collider.enabled=false;continue;}
                if(id.StartsWith("Floor_House")) { Surface(renderer,TextureTile("WoodRequested",1.5f),false);continue; }
                if(id.StartsWith("Wall_")) { var size=(Vector2)renderer.bounds.size;Surface(renderer,TextureTile("WallRequested",1),size.y>size.x);continue; }
                Sprite sprite=null;Vector2? position=null;Vector2 box=renderer.bounds.size;bool solid=false;
                switch(id)
                {
                    case "Sofa_LivingRoom":sprite=Correction(0);position=new(4.6f,2.4f);box=new(2.6f,1.5f);solid=true;break;
                    case "TV_StaticNoise":sprite=CampaignVisualAssets.Prop("TV");box=new(1.35f,1.35f);solid=true;break;
                    case "CoffeeTable_Living":sprite=CampaignVisualAssets.Prop("CoffeeTable");position=new(4.6f,3.85f);box=new(1.5f,.9f);solid=true;break;
                    case "Rug_LivingRoom":sprite=Correction(1);position=new(4.6f,3.2f);box=new(3.8f,2.6f);break;
                    case "KitchenRunner":sprite=Correction(1);position=new(4.65f,-3.65f);box=new(2.1f,1.45f);break;
                    case "BedroomRug":sprite=Correction(1);box=new(3.1f,2.1f);break;
                    case "OfficeRug":case "Rug_Office":case "Tapete_Escritorio":sprite=Correction(1);box=new(3.8f,2.6f);break;
                    case "Tapete_Porta":sprite=Correction(1);box=new(1.2f,.85f);break;
                    case "WallPicture_Office":case "WallPicture_Living":sprite=Correction(2);box=new(1.25f,.95f);break;
                    case "Noticeboard_Office":case "OfficeNoticeboard":case "Noticeboard_Clues":sprite=Correction(3);box=new(1.55f,.8f);break;
                    case "Desk_Office":sprite=CampaignVisualAssets.Prop("Desk");box=new(1.9f,1.5f);solid=true;break;
                    case "Chair_Office":sprite=CampaignVisualAssets.Prop("Chair");box=new(.65f,.9f);solid=true;break;
                    case "Bed_Edelzio":sprite=CampaignVisualAssets.Prop("Bed");box=new(1.9f,2.2f);solid=true;break;
                    case "Nightstand_Bedroom":sprite=CampaignVisualAssets.Prop("Nightstand");solid=true;break;
                    case "Dresser_Bedroom":sprite=CampaignVisualAssets.Prop("Dresser");solid=true;break;
                    case "Bookshelf_Office":sprite=CampaignVisualAssets.Prop("Bookshelf");solid=true;break;
                    case "Kitchen_Cabinet":sprite=CampaignVisualAssets.Prop("Kitchen");box=new(2.65f,1.55f);solid=true;break;
                    case "Fridge_Kitchen":sprite=CampaignVisualAssets.Prop("Fridge");box=new(1.1f,1.7f);solid=true;break;
                    case "Stove_Kitchen":sprite=CampaignVisualAssets.Prop("Stove");solid=true;break;
                    case "CoffeeTable_Kitchen":sprite=CampaignVisualAssets.Prop("CoffeeTable");solid=true;break;
                }
                if(sprite!=null)Fit(renderer,sprite,position??(Vector2)renderer.transform.position,box,solid);
            }
            var wall=GameObject.Find("Wall_North");
            var house=wall!=null?wall.transform.parent:null;
            var decor=house!=null?house.Find("Cenario_Acabamento_V3"):null;
            if(house!=null&&house.Find("Portais_Abertos")==null)
            {
                var portals=new GameObject("Portais_Abertos");portals.transform.SetParent(house,false);
                WallSegment(portals.transform,"Divisão quarto oeste",new Rect(-9,-.3f,3.1f,.6f));
                WallSegment(portals.transform,"Divisão quarto leste",new Rect(-4.1f,-.3f,4.1f,.6f));
                Portal(portals.transform,"Portal_Quarto_Escritorio",new Vector2(-5,0),1.8f,false);
                Portal(portals.transform,"Portal_Escritorio_Sala",new Vector2(0,-3),2f,true);
                Portal(portals.transform,"Portal_Quarto_Sala",new Vector2(0,1),2f,true);
                Portal(portals.transform,"Portal_Sala_Quintal",new Vector2(9,0),2.8f,true);
            }
            if(house!=null)
            {
                var walls=new List<Rect>();
                foreach(var collider in house.GetComponentsInChildren<BoxCollider2D>())
                    if(collider.enabled&&(collider.name.StartsWith("Wall_")||collider.name.StartsWith("Divisão quarto")))
                        walls.Add(new Rect((Vector2)collider.bounds.min,collider.bounds.size));
                CampaignWallConnections.Build(house,walls,TextureTile("WallRequested",.6f),true);
            }
            Support("Notebook_TI","Desk_Office",new Vector2(-5,-3.35f),2);
            Support("Radio_Office","Desk_Office",new Vector2(-5.58f,-3.35f),2);
            Support("Lamp_Desk","Desk_Office",new Vector2(-4.32f,-3.28f),3);
            Support("Coffee_Cup","CoffeeTable_Kitchen",new Vector2(4.6f,-1.86f),2);
            if(decor!=null)
            {
                foreach(var shadow in decor.GetComponentsInChildren<SpriteRenderer>())if(shadow.name.StartsWith("Sombra_Contato_"))shadow.enabled=false;
                var lighting=decor.Find(VarginhaSoftLighting.LayerName);
                if(lighting!=null){lighting.gameObject.SetActive(false);Object.Destroy(lighting.gameObject);}
                VarginhaSoftLighting.Build(house,decor);
                var light=decor.Find(VarginhaSoftLighting.LayerName);
                if(light!=null)light.GetComponent<SpriteRenderer>().color=new Color(1,1,1,.65f);
            }
            Physics2D.SyncTransforms();
        }
        public static Sprite TextureTile(string name,float worldSize)
        {
            string key=name+worldSize;
            if(Sprites.TryGetValue(key,out var cached)&&cached!=null)return cached;
            var texture=Resources.Load<Texture2D>("Varginha/HouseFeedback/"+name);if(texture==null)return null;
            int side=Mathf.Min(texture.width,texture.height)-4;
            var rect=new Rect((texture.width-side)/2,(texture.height-side)/2,side,side);
            var sprite=Sprite.Create(texture,rect,Vector2.one/2,side/Mathf.Max(.1f,worldSize),0,SpriteMeshType.FullRect);
            sprite.name="HouseFeedback_"+key;Sprites[key]=sprite;return sprite;
        }
        public static Sprite Correction(int index)
        {
            string key="FurnitureCorrection"+index;
            if(Sprites.TryGetValue(key,out var cached)&&cached!=null)return cached;
            var texture=Resources.Load<Texture2D>("Varginha/HouseFeedback/FurnitureCorrections");if(texture==null)return null;
            int cw=texture.width/2,ch=texture.height/2,x0=index%2*cw,y0=(1-index/2)*ch;
            var pixels=texture.GetPixels32();int left=x0+cw,right=x0-1,bottom=y0+ch,top=y0-1;
            for(int y=y0;y<y0+ch;y++)for(int x=x0;x<x0+cw;x++)if(pixels[y*texture.width+x].a>=100)
            {left=Mathf.Min(left,x);right=Mathf.Max(right,x);bottom=Mathf.Min(bottom,y);top=Mathf.Max(top,y);}
            if(right<left)return null;
            var sprite=Sprite.Create(texture,new Rect(left,bottom,right-left+1,top-bottom+1),Vector2.one/2,64,0,SpriteMeshType.FullRect);
            sprite.name="HouseFeedback_"+key;Sprites[key]=sprite;return sprite;
        }
        private static void Surface(SpriteRenderer original,Sprite tile,bool vertical)
        {
            if(tile==null)return;
            var child=original.transform.Find("Requested_Texture");if(child!=null)return;
            var area=new Rect((Vector2)original.bounds.min,original.bounds.size);
            if(original.name.StartsWith("Floor_"))
            {
                var go=new GameObject("Requested_Texture");go.transform.SetParent(original.transform,false);
                CampaignArchitectureRenderer.Floor(go.transform,"Piso alinhado",area,tile,original.sortingOrder);
            }
            else
            {
                var renderer=CampaignArchitectureRenderer.Wall(original.transform,"Requested_Texture",area,tile,true,false);
                renderer.sortingLayerID=original.sortingLayerID;renderer.sortingOrder=original.sortingOrder;
            }
            original.enabled=false;
        }
        private static void Fit(SpriteRenderer renderer,Sprite sprite,Vector2 position,Vector2 box,bool solid)
        {
            float scale=Mathf.Min(box.x/sprite.bounds.size.x,box.y/sprite.bounds.size.y);
            Vector2 fitted=sprite.bounds.size*scale;
            renderer.sprite=sprite;renderer.color=Color.white;renderer.transform.position=new Vector3(position.x,position.y,renderer.transform.position.z);
            renderer.transform.localScale=Vector3.one*scale;
            if(solid)
            {
                var collider=renderer.GetComponent<BoxCollider2D>();if(collider==null)collider=renderer.gameObject.AddComponent<BoxCollider2D>();
                collider.isTrigger=false;collider.size=new Vector2(fitted.x*.82f,Mathf.Min(.42f,fitted.y*.35f))/scale;
                collider.offset=new Vector2(0,-fitted.y*.35f/scale);
                VarginhaWorldDepth.Ensure(renderer,ground:collider);
            }
            else if(renderer.name.Contains("Rug")||renderer.name.Contains("Tapete")||renderer.name=="KitchenRunner")
            {
                foreach(var collider in renderer.GetComponents<Collider2D>())collider.enabled=false;
                var depth=renderer.GetComponent<VarginhaWorldDepth>();if(depth!=null)depth.enabled=false;
                var group=renderer.GetComponent<UnityEngine.Rendering.SortingGroup>();if(group!=null)group.sortingOrder=1;
                renderer.sortingOrder=1;
            }
        }
        private static void Support(string name,string supportName,Vector2 position,int order)
        {
            var go=GameObject.Find(name);var support=GameObject.Find(supportName);if(go==null||support==null)return;
            go.transform.position=new Vector3(position.x,position.y,go.transform.position.z);
            VarginhaWorldDepth.Ensure(go.GetComponent<SpriteRenderer>(),supportingObject:support.transform,offset:order);
        }
        private static void WallSegment(Transform parent,string name,Rect rect)
        {
            CampaignArchitectureRenderer.Wall(parent,name,rect,TextureTile("WallRequested",1));
        }
        private static void Portal(Transform parent,string name,Vector2 position,float width,bool vertical)
        {
            var go=new GameObject(name);go.transform.SetParent(parent,false);go.transform.position=position;
            if(vertical)go.transform.rotation=Quaternion.Euler(0,0,90);
            foreach(float side in new[]{-1f,1f})FramePart(go.transform,"Batente",new Vector2(side*(width*.5f+.09f),0),new Vector2(.18f,.9f),true);
            FramePart(go.transform,"Viga superior",new Vector2(0,.53f),new Vector2(width+.36f,.16f),false);
        }
        private static void FramePart(Transform parent,string name,Vector2 position,Vector2 size,bool solid)
        {
            var go=new GameObject(name);go.transform.SetParent(parent,false);go.transform.localPosition=position;
            var sr=go.AddComponent<SpriteRenderer>();sr.sprite=TextureTile("WoodRequested",.3f);sr.drawMode=SpriteDrawMode.Tiled;sr.size=size;
            sr.sortingOrder=VarginhaWorldDepth.OrderAt(parent.position.y)+1;
            if(solid)go.AddComponent<BoxCollider2D>().size=size;
        }
    }
}
