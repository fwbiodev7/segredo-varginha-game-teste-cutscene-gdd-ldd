using UnityEngine;

namespace Game.Varginha.Experiment
{
    public static class CampaignAtmosphereLighting
    {
        public static void Build(Transform map,CampaignMapPlan plan,Transform furnishings)
        {
            var decor=new GameObject("03_Atmosfera_Luz_Ambiente").transform;decor.SetParent(map,false);
            foreach(var prop in plan.furniture)
            {
                if(prop.motif=="Torch"||prop.motif=="Lamp"||prop.motif=="SpecimenTank")
                {
                    bool torch=prop.motif=="Torch",specimen=prop.motif=="SpecimenTank";
                    var position=prop.position+Vector2.down*(torch?.5f:prop.size.y*.3f);
                    var size=specimen?new Vector2(2.2f,2):new Vector2(3.3f,2.5f);
                    var color=specimen?new Color(.4f,1,.63f):torch?new Color(1,.5f,.16f):new Color(1,.77f,.42f);
                    Source(decor,"Luz_"+prop.name,"Glow",position,size,color);
                }
                if(prop.motif=="GothicWindow")
                    Source(decor,"Reflexo_Vitral_"+prop.name,"GlassLight",prop.position+new Vector2(.4f,-2),new Vector2(2,3.5f),new Color(.74f,.57f,1));
                if(plan.phase==5&&prop.motif=="OriginalProjector")
                    Source(decor,"Luz_Projetor_Investigação","Glow",prop.position+Vector2.down*1.5f,new Vector2(3.5f,3),new Color(.43f,.56f,1));
            }
            // Reuse the original game's baked floor-light compositor and footprint shadows.
            VarginhaSoftLighting.Build(map,decor);
            if(plan.phase==8)
                foreach(var renderer in map.GetComponentsInChildren<SpriteRenderer>())
                    if(renderer.transform.IsChildOf(furnishings)&&!renderer.name.StartsWith("Tocha"))renderer.color=new Color(.86f,.82f,.95f);
        }
        private static void Source(Transform parent,string name,string motif,Vector2 position,Vector2 size,Color color)
        {
            var go=new GameObject(name);go.transform.SetParent(parent,false);go.transform.position=position;
            var renderer=go.AddComponent<SpriteRenderer>();renderer.sprite=VarginhaSceneryArt.Create(motif,size);renderer.color=color;renderer.sortingOrder=1;
        }
    }
}
