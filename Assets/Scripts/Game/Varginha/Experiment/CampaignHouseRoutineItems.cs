using UnityEngine;

namespace Game.Varginha.Experiment
{
    /// <summary>Real removable items above clean furniture, restored from the existing routine save bits.</summary>
    public sealed class CampaignHouseRoutineItems : MonoBehaviour
    {
        public SpriteRenderer Coffee {get;private set;}
        public SpriteRenderer Notebook {get;private set;}
        public void Build()
        {
            var data=CampaignIllustratedMaps.Get(2);var map=transform.Find("Mapa_Campanha");if(data==null||map==null)return;
            CampaignHouseFurniture.Apply(map,data);
            var coffee=GameObject.Find("Coffee_Cup");var notebook=GameObject.Find("Notebook_TI");
            Coffee=CampaignHouseFurniture.Item(map,data,"Cafe_interagivel","CoffeeFull",new[]{697f,503,23,25});
            Notebook=CampaignHouseFurniture.Item(map,data,"Notebook_recolhivel","Notebook",new[]{69f,426,41,38});
            // Reuse the original target names so action scripts and scene references continue to work.
            if(coffee!=null)coffee.name="Coffee_Cup_Ancora_Original";Coffee.name="Coffee_Cup";
            if(notebook!=null)notebook.name="Notebook_TI_Ancora_Original";Notebook.name="Notebook_TI";
            VarginhaWorldDepth.Ensure(Coffee,supportingObject:map.Find("02_Mobilia_Colisoes/CoffeeTable_Kitchen"),offset:3);
            VarginhaWorldDepth.Ensure(Notebook,supportingObject:map.Find("02_Mobilia_Colisoes/Desk_Office"),offset:4);
            var shader=Resources.Load<Shader>("Varginha/CinemaOpening/IllustratedMorning");
            if(shader!=null)
            {
                var mapMaterial=new Material(shader){name="Casa_07h_paleta_quintal"};mapMaterial.SetFloat("_ReferenceMap",1);_materials.Add(mapMaterial);
                var furnitureMaterial=new Material(shader){name="Casa_07h_moveis"};furnitureMaterial.SetFloat("_ReferenceMap",0);_materials.Add(furnitureMaterial);
                foreach(var renderer in map.GetComponentsInChildren<SpriteRenderer>())
                {
                    if(renderer.sprite==null||renderer.name.StartsWith("Vidro_diurno"))continue;
                    bool original=renderer.sprite.texture.name==data.image||renderer.sprite.texture.name==data.background||renderer.sprite.texture.name=="AdultInteractiveBase";
                    bool furniture=renderer.sprite.texture.name=="HouseFurnitureBacks"||renderer.sprite.texture.name=="HouseRoutineItems";
                    if(!original&&!furniture)continue;
                    renderer.sharedMaterial=original?mapMaterial:furnitureMaterial;
                }
            }
        }
        public void Restore(int routine)
        {
            if(Coffee!=null)Coffee.gameObject.SetActive((routine&2)==0);
            if(Notebook!=null)Notebook.gameObject.SetActive((routine&4)==0);
        }
        private readonly System.Collections.Generic.List<Material> _materials=new();
        private void OnDestroy(){foreach(var material in _materials)if(material!=null)Destroy(material);}
    }
}
