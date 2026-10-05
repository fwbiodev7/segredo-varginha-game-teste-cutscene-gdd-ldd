using UnityEngine;

namespace Game.Varginha
{
    /// <summary>The real Industrial school frontage; cut away its tall face while indoors.</summary>
    public sealed class VarginhaIndustrialSchoolFacade : MonoBehaviour
    {
        public const string ResourcePath = "Varginha/SchoolIndustrialFacadeV1";
        public const string RootName = "Fachada_Escola_Industrial_V1";
        private static readonly Sprite[] Sprites = new Sprite[2];
        [SerializeField] private SpriteRenderer[] faces, wallEdges;
        [SerializeField] private SpriteRenderer[] coveredInterior;
        [SerializeField] private float interiorY = -5.15f;
        private EdelzioTopDownController _player;

        public static VarginhaIndustrialSchoolFacade EnsureRebuiltCampus(Transform school)
        {
            var texture=Resources.Load<Texture2D>(ResourcePath);
            if(texture==null)return null;
            var root=school.Find(RootName);
            if(root==null){root=new GameObject(RootName).transform;root.SetParent(school,false);}
            var facade=root.GetComponent<VarginhaIndustrialSchoolFacade>();if(facade==null)facade=root.gameObject.AddComponent<VarginhaIndustrialSchoolFacade>();
            var renderer=root.GetComponent<SpriteRenderer>();if(renderer==null)renderer=root.gameObject.AddComponent<SpriteRenderer>();
            // Keep the complete authored frontage and its gate, at one uniform scale.
            renderer.sprite=Sprite.Create(texture,new Rect(0,0,texture.width,texture.height),new Vector2(300f/texture.width,24f/texture.height),32,0,SpriteMeshType.FullRect);
            renderer.sprite.name="Industrial_Fachada_Integral_Original";renderer.sortingOrder=VarginhaWorldDepth.OrderAt(-6.05f);
            root.position=new Vector3(.75f,-6.05f,0);root.localScale=Vector3.one;
            VarginhaWorldDepth.Ensure(renderer,supportingObject:root);
            var furnishings=school.Find("Mapa_Campanha/02_Mobilia_Colisoes");
            var covered=new System.Collections.Generic.List<SpriteRenderer>();
            if(furnishings!=null)foreach(Transform prop in furnishings)
            {if(prop.position.y>-5.7f&&prop.position.y< -3.6f)covered.Add(prop.GetComponent<SpriteRenderer>());}
            facade.coveredInterior=covered.ToArray();
            facade.faces=new[]{renderer};facade.wallEdges=null;facade.interiorY=-5.15f;facade.ShowInterior(false);
            return facade;
        }

        public static void Ensure(Transform school)
        {
            var exterior = school.Find(VarginhaSchoolExterior.RootName);
            var left = school.Find("CenarioV2_Parede_Sul");
            var right = exterior != null ? exterior.Find("Parede_Sul_Direita") : null;
            if (exterior == null || left == null || right == null) return;
            var texture = Resources.Load<Texture2D>(ResourcePath);
            if (texture == null) return;
            for (int i = 0; i < 2; i++)
            {
                if (Sprites[i] != null && Sprites[i].texture != null) continue;
                string name = i == 0 ? "Industrial_Cursos_Portao" : "Industrial_Identificacao";
                foreach (var imported in Resources.LoadAll<Sprite>(ResourcePath))
                    if (imported.name == name) Sprites[i] = imported;
                if (Sprites[i] != null) continue;
                var rect = i == 0 ? new Rect(8, 24, 270, 125) : new Rect(325, 24, 210, 125);
                Sprites[i] = Sprite.Create(texture, rect, new Vector2(.5f, 0), 32, 0, SpriteMeshType.FullRect);
                Sprites[i].name = name;
            }
            var root = exterior.Find(RootName);
            if (root == null) { root = new GameObject(RootName).transform; root.SetParent(exterior, false); }
            var facade = root.GetComponent<VarginhaIndustrialSchoolFacade>();
            if (facade == null) facade = root.gameObject.AddComponent<VarginhaIndustrialSchoolFacade>();
            facade.wallEdges = new[] { left.GetComponent<SpriteRenderer>(), right.GetComponent<SpriteRenderer>() };
            facade.faces = new SpriteRenderer[2];
            for (int i = 0; i < 2; i++)
            {
                string name = i == 0 ? "Fachada_Industrial_Esquerda" : "Fachada_Industrial_Direita";
                var panel = root.Find(name);
                if (panel == null) { panel = new GameObject(name).transform; panel.SetParent(root, false); }
                var sr = panel.GetComponent<SpriteRenderer>();
                if (sr == null) sr = panel.gameObject.AddComponent<SpriteRenderer>();
                sr.sprite = Sprites[i]; sr.color = Color.white; sr.sortingOrder = 4;
                float width = i == 0 ? 7.5f : 7.1f;
                panel.position = new Vector3(i == 0 ? -4.25f : 5.55f, -6.05f, 0);
                panel.localScale = new Vector3(width / sr.sprite.bounds.size.x, 3.4f / sr.sprite.bounds.size.y, 1);
                facade.faces[i] = sr;
                if (Experiment.VarginhaCampaignStage.Active == null)
                    VarginhaWorldDepth.Ensure(sr, ground: (i == 0 ? left : right).GetComponent<Collider2D>());
            }
            foreach (string name in new[] { "Placa_Escola", "Placa_Escola_Fundo" })
            {
                var previous = exterior.Find(name);
                if (previous != null) previous.gameObject.SetActive(false);
            }
            facade.ShowInterior(false);
        }

        public void ShowInterior(bool inside)
        {
            if (faces != null) foreach (var face in faces) if (face != null) face.enabled = !inside;
            if (wallEdges != null) foreach (var edge in wallEdges) if (edge != null) edge.enabled = inside;
            if (coveredInterior != null) foreach(var furnishing in coveredInterior)if(furnishing!=null)furnishing.enabled=inside;
        }

        private void LateUpdate()
        {
            if (_player == null) _player = FindAnyObjectByType<EdelzioTopDownController>();
            if (_player == null) return;
            Vector2 position = _player.transform.position;
            ShowInterior(position.y > interiorY && position.x > -7.7f && position.x < 8.7f);
        }
    }
}
