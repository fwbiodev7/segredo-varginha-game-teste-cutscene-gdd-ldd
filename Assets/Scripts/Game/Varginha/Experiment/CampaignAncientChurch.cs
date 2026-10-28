using UnityEngine;

namespace Game.Varginha.Experiment
{
    // Placement follows AncientChurchExteriorV1.json. Original forest pixels remain intact.
    public static class CampaignAncientChurch
    {
        const string Resource="Varginha/Revision20261028/AncientChurchExteriorV1";
        static Sprite _church,_stone;
        static Material _material;
        public static void Apply(CampaignMapPlan plan,CampaignIllustratedMaps.Layout data)
        {
            plan.furniture.RemoveAll(p=>p.name=="Altar em ruínas"||p.name=="Pedra memorial"||p.name=="Sino do santuário");
            // The solid building ends north of the steps; the south doorway stays reachable.
            plan.walls.Add(data.Area(new[]{1172.5f,51.6f,325f,418.6f}));
            foreach(string id in new[]{"fabio","exit"})
            {
                var point=plan.points.Find(p=>p.id==id);
                point.position=data.Position(1334,522);point.label="PORTA DA IGREJA • ENTRAR";
            }
            plan.points.Find(p=>p.id=="sign2").position=data.Position(1205,535);
        }
        public static void Build(Transform map,CampaignIllustratedMaps.Layout data)
        {
            var texture=Resources.Load<Texture2D>(Resource);if(texture==null)return;
            texture.filterMode=FilterMode.Point;
            if(_church==null)_church=Sprite.Create(texture,new Rect(258,100,738,1134),Vector2.one/2,100,0,SpriteMeshType.FullRect);
            if(_material==null)_material=new Material(Resources.Load<Shader>("Varginha/Flash1996/CrispPixelSprite"));
            var background=Resources.Load<Texture2D>("Varginha/IllustratedMaps/Forest");
            if(_stone==null)_stone=Sprite.Create(background,new Rect(1160,1024-155-45,45,45),Vector2.one/2,100,0,SpriteMeshType.FullRect);
            // Existing paving covers only the three removed ruin props, beneath the new building.
            foreach(var rect in new[]{new[]{1267f,99f,88f,106f},new[]{1355f,136f,73f,88f},new[]{1217f,238f,139f,131f}})
                Place(map,"Pavimento_preservado",_stone,data.Area(rect),-990);
            // Thirty percent larger, anchored at the north roofline so it fits
            // the existing map; the doorway and approach move south with it.
            var area=data.Area(new[]{1172.5f,10f,325f,499.39024f});
            var church=Place(map,"Igreja_antiga_inteira",_church,area,0);church.sharedMaterial=_material;
            var ground=new GameObject("Soleira_da_igreja").transform;ground.SetParent(church.transform,false);
            ground.position=data.Position(1334,470.2f);
            VarginhaWorldDepth.Ensure(church,supportingObject:ground);
        }
        static SpriteRenderer Place(Transform parent,string name,Sprite sprite,Rect area,int order)
        {
            var go=new GameObject(name);go.transform.SetParent(parent,false);go.transform.position=area.center;
            go.transform.localScale=new Vector3(area.width/sprite.bounds.size.x,area.height/sprite.bounds.size.y,1);
            var renderer=go.AddComponent<SpriteRenderer>();renderer.sprite=sprite;renderer.sortingOrder=order;return renderer;
        }
    }
}
