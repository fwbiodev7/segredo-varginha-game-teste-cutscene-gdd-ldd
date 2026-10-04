using Game.Level;
using Game.Managers;
using Game.Player;
using UnityEngine;

namespace Game.Varginha.Experiment
{
    public static class CampaignMapConstruction
    {
        public static VarginhaIndustrialSchoolFacade PreserveSchoolFacade(Transform school)
        {
            return VarginhaIndustrialSchoolFacade.EnsureRebuiltCampus(school);
        }
        public static Transform Build(Transform owner, CampaignMapPlan plan, bool furnished = true)
        {
            var existing = owner.Find("Mapa_Campanha"); if (existing != null) return existing;
            if(CampaignIllustratedMaps.Get(plan.phase)!=null)return CampaignIllustratedMaps.Build(owner,plan,furnished);
            var map = new GameObject("Mapa_Campanha").transform; map.SetParent(owner);
            var architecture = new GameObject("01_Planta_Paredes_Divisoes").transform; architecture.SetParent(map);
            int order = -1000;
            foreach (var room in plan.rooms)
            {
                CampaignArchitectureRenderer.Floor(architecture,room.name,room.rect,
                    CampaignArchitectureRenderer.FloorMaterial(room.motif)??VarginhaPixelArtSprites.Create(room.motif,room.color),order++);
            }
            for (int i = 0; i < plan.walls.Count; i++)
            {
                var wall = plan.walls[i];
                bool river = plan.title=="A MATA" && Mathf.Approximately(wall.width,1.5f);
                bool forestBoundary=plan.phase==7&&(wall.xMin<=plan.bounds.xMin+.01f||wall.xMax>=plan.bounds.xMax-.01f||wall.yMin<=plan.bounds.yMin+.01f||wall.yMax>=plan.bounds.yMax-.01f);
                bool outsideBoundary=wall.yMax<=plan.bounds.yMin;
                var material=plan.phase==1?CampaignAdultHouse.TextureTile("WallRequested",1):CampaignVisualAssets.Wall(plan.phase);
                if(plan.phase==9)material=CampaignOuzanaArt.Wall(wall.center.x<=-1)??material;
                CampaignArchitectureRenderer.Wall(architecture,"Parede_"+i,wall,material,
                    !river&&!forestBoundary&&!outsideBoundary&&!(plan.phase==3&&wall.width>=4));
            }
            if(plan.phase==9)CampaignWallConnections.Build(architecture,plan.walls,CampaignOuzanaArt.Wall(false),true,p=>CampaignOuzanaArt.Wall(p.x<=-1));
            else if(plan.phase!=3&&plan.phase!=7)CampaignWallConnections.Build(architecture,plan.walls,
                plan.phase==1?CampaignAdultHouse.TextureTile("WallRequested",1):CampaignVisualAssets.Wall(plan.phase),plan.phase==1||plan.phase==6);
            if (furnished) Furnish(map, plan);
            return map;
        }
        public static void Furnish(Transform map, CampaignMapPlan plan)
        {
            if(CampaignIllustratedMaps.Get(plan.phase)!=null){CampaignIllustratedMaps.Furnish(map,plan);return;}
            if (map.Find("02_Mobilia_Colisoes") != null) return;
            var layer = new GameObject("02_Mobilia_Colisoes").transform; layer.SetParent(map);
            foreach (var prop in plan.furniture)
            {
                Sprite sprite;
                string motif=prop.name.StartsWith("Árvore")&&prop.motif=="Plant"?"Tree":prop.motif;
                sprite=motif=="Fusca"?null:CampaignInteriorArt.Prop(motif)??CampaignOuzanaArt.Prop(motif)??CampaignVisualAssets.Prop(motif);
                if(sprite!=null) { }
                else if (prop.motif == "Fusca")
                {
                    var source = VarginhaExperimentArt.Load("FuscaTopView");
                    int w=source.width/2,h=source.height/2;
                    sprite = Sprite.Create(source, new Rect(0,h,w,h), Vector2.one / 2, h / prop.size.y,0,SpriteMeshType.FullRect);
                }
                else if (prop.name.StartsWith("Árvore"))
                {
                    var source = Resources.Load<Texture2D>("Varginha/TravelPixel/TreeReference");
                    sprite = Sprite.Create(source,new Rect(0,0,source.width,source.height),Vector2.one/2,source.height/prop.size.y,0,SpriteMeshType.FullRect);
                }
                else sprite = VarginhaFurnitureArt.Create(prop.motif, prop.size) ?? VarginhaSceneryArt.Create(prop.motif, prop.size);
                var renderer = Render(layer, prop.name, sprite, prop.position, prop.size, 5);
                if(plan.phase==1||motif.StartsWith("Ouzana_")||CampaignInteriorArt.Contains(motif))
                {
                    renderer.drawMode=SpriteDrawMode.Simple;
                    float fit=Mathf.Min(prop.size.x/sprite.bounds.size.x,prop.size.y/sprite.bounds.size.y);
                    renderer.transform.localScale=Vector3.one*fit;
                }
                if(motif.StartsWith("Ouzana_"))renderer.sharedMaterial=CampaignOuzanaArt.Material;
                else if(CampaignInteriorArt.Contains(motif))renderer.sharedMaterial=CampaignInteriorArt.Material;
                if(prop.footprint.width<=0||prop.footprint.height<=0)
                {renderer.sortingOrder=prop.motif=="Rug"||prop.motif=="Church_Runner"?1:4;continue;}
                var collider = renderer.gameObject.AddComponent<BoxCollider2D>();
                var scale=(Vector2)renderer.transform.lossyScale;
                collider.size = prop.footprint.size/scale; collider.offset = (prop.footprint.center - prop.position)/scale;
                VarginhaWorldDepth.Ensure(renderer, ground: collider);
                if(!sprite.name.StartsWith("WorldArt"))VarginhaContactShadow.Ensure(renderer,false);
            }
            CampaignAtmosphereLighting.Build(map,plan,layer);
        }
        private static SpriteRenderer Render(Transform parent, string name, Sprite sprite, Vector2 position, Vector2 size, int order)
        {
            var go = new GameObject(name); go.transform.SetParent(parent); go.transform.position = position;
            var renderer = go.AddComponent<SpriteRenderer>(); renderer.sprite = sprite; renderer.sortingOrder = order;
            renderer.drawMode = SpriteDrawMode.Sliced; renderer.size = size;
            return renderer;
        }
        public static EdelzioTopDownController CreatePlayer(Transform owner, CampaignMapPlan plan, bool childhood = false)
        {
            var player = new GameObject("Edelzio_Protagonist"); player.tag = "Player"; player.transform.SetParent(owner); player.transform.position = plan.spawn + (childhood ? Vector2.zero : new Vector2(0,.58f));
            var renderer = player.AddComponent<SpriteRenderer>(); renderer.sortingOrder = 5;
            var body = player.AddComponent<Rigidbody2D>(); body.gravityScale = 0; body.constraints = RigidbodyConstraints2D.FreezeRotation;
            player.AddComponent<CircleCollider2D>(); player.AddComponent<HealthSystem>();
            var controller = player.AddComponent<EdelzioTopDownController>();
            player.AddComponent<VarginhaPlayerSpriteAnimation>(); player.AddComponent<VarginhaPlayerActionAnimation>();
            CampaignPresentation.FootCollision(controller, childhood);
            if (!childhood) player.AddComponent<CampaignTeamEdelzio>();
            VarginhaWorldDepth.Ensure(renderer, ground: player.GetComponent<Collider2D>());
            if (GameManager.Instance == null) new GameObject("GameManager").AddComponent<GameManager>();
            var cameraObject = new GameObject("Main Camera"); cameraObject.transform.SetParent(owner); cameraObject.tag = "MainCamera";
            var camera = cameraObject.AddComponent<Camera>(); camera.orthographic = true; camera.orthographicSize = 6;
            camera.backgroundColor = new Color(.025f,.035f,.05f); cameraObject.AddComponent<AudioListener>();
            cameraObject.AddComponent<CameraFollow2D>().ConfigureMap(player.transform, plan.bounds, 6);
            VarginhaPixelPresentation.Configure(camera);
            return controller;
        }
    }
}
