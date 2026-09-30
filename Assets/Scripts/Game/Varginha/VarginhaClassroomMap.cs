using UnityEngine;

namespace Game.Varginha
{
    /// <summary>Reference IT classroom with broad battle aisles and a continuous route to the parking lot.</summary>
    public static class VarginhaClassroomMap
    {
        public const string Marker = "Sala_Informatica_V4";
        public static readonly Vector3[] StudentPositions =
        {
            new(-4.25f,1.5f), new(.75f,1.5f), new(5.25f,1.5f),
            new(-4.25f,-.9f), new(.75f,-.9f), new(5.25f,-.9f),
            new(-4.25f,-3.9f), new(.75f,-3.9f), new(5.25f,-3.9f)
        };
        public static readonly Vector3[] EnemyPositions =
        { new(-4.25f,2.9f), new(5.25f,2.9f), new(.75f,3.8f), new(5.25f,-2.1f) };

        public static void Ensure(Transform school)
        {
            if (school == null) return;
            bool fresh = school.Find(Marker) == null;
            if (fresh)
            {
                // Hide the shipped multi-room interior when opening an older serialized scene.
                foreach (Transform child in school)
                    if (child.name.StartsWith("CenarioV2_") && !child.name.StartsWith("CenarioV2_Parede_")
                        && child.name != "CenarioV2_Vaga_Fusca") child.gameObject.SetActive(false);
                Node(school, Marker);
                Node(school, "CenarioVisual_Escola_V2");
                var oldDecor = school.Find("Cenario_Acabamento_V3");
                if (oldDecor != null)
                    foreach (Transform child in oldDecor) child.gameObject.SetActive(false);
            }
            var decor = Node(school, "Cenario_Acabamento_V3");
            // Same tiled surface renderer and source pixel density as Phase 1.
            var floor = Part(school, "SalaV4_Piso", new(.5f,0),
                VarginhaClassroomArt.Surface(false), 0);
            floor.drawMode = SpriteDrawMode.Tiled;
            floor.size = new Vector2(16,11);
            Wall(school,"CenarioV2_Parede_Norte",new(.5f,5.7f),new(16.7f,.7f));
            Wall(school,"CenarioV2_Parede_Sul",new(-4.25f,-5.7f),new(7.5f,.7f));
            Wall(school,"CenarioV2_Parede_Oeste",new(-7.7f,0),new(.7f,11.7f));
            Wall(school,"CenarioV2_Parede_Leste",new(8.7f,0),new(.7f,11.7f));
            float[] columns = { -5.7f,-2.8f,3.8f,6.7f };
            float[] rows = { 3f,.5f,-2f };
            for (int r=0;r<rows.Length;r++)
            for (int c=0;c<columns.Length;c++)
            {
                int index = r*4+c;
                Vector2 position = new(columns[c],rows[r]);
                var desk = Art(school,"SalaV4_Mesa_"+index,c == 1 ? 1 : 0,position,new(1.65f,1.25f),3);
                Collision(desk.gameObject,new(1.6f,.55f),new(0,.12f));
                var chair = Art(school,"SalaV4_Cadeira_"+index,10,position+Vector2.down*.8f,new(.68f,.85f),3);
                Collision(chair.gameObject,new(.44f,.38f),new(0,-.05f));
                if (chair.GetComponent<InteractableProp>() == null)
                    chair.gameObject.AddComponent<InteractableProp>().Configure(PropType.ClassroomSeat,"Cadeira da sala","Sentar na carteira.",true);
                Part(decor,"SalaV4_Sombra_Mesa_"+index,position+Vector2.down*.5f,
                    VarginhaSceneryArt.Create("Shadow",new Vector2(1.8f,.52f)),1);
            }
            // Tall windows and cream curtains evoke the user's classroom photo.
            for (int i=0;i<3;i++)
            {
                float x = -5.4f+i*3.1f;
                Art(school,"SalaV4_Janela_"+i,4,new(x,5.22f),new(2.6f,.85f),4);
                var ray = Part(decor,"Luar_SalaV4_"+i,new(x,3.55f),
                    VarginhaSceneryArt.Create("WindowLight",new Vector2(2.7f,2.9f)),1);
                ray.color = new Color(.65f,.78f,1f,.16f);
            }
            Art(school,"SalaV4_Quadro_Branco",7,new(6.6f,5.18f),new(2.3f,.85f),4);
            Art(school,"SalaV4_Mesa_Professor",3,new(.75f,4.75f),new(2.1f,.95f),3);
            Art(school,"SalaV4_Armario",8,new(-6.5f,-4.65f),new(.85f,1.3f),3);
            Art(school,"SalaV4_Mural",11,new(7.95f,-4.1f),new(.6f,1f),3);
            Art(school,"SalaV4_Projetor",9,new(3.3f,5.2f),new(.85f,.42f),4);
            // Decorative wainscoting stays outside the walkable floor footprint.
            for (int i=0;i<7;i++)
                Art(school,"SalaV4_Lambril_"+i,5,new(-6.4f+i*2.2f,5.55f),new(2.2f,.75f),2);
            if(decor.GetComponentsInChildren<VarginhaSoftLighting>().Length==0)
                VarginhaSoftLighting.Build(school,decor);
            if (fresh) VarginhaSchoolNavigation.Invalidate();
        }

        private static Transform Node(Transform parent,string name)
        {
            var found=parent.Find(name);
            if(found!=null) return found;
            var go=new GameObject(name);
            go.transform.SetParent(parent,false);
            return go.transform;
        }

        private static SpriteRenderer Part(Transform parent,string name,Vector2 position,Sprite sprite,int order)
        {
            var item=Node(parent,name);
            item.gameObject.SetActive(true);
            item.localPosition=new Vector3(Mathf.Round(position.x*32)/32,Mathf.Round(position.y*32)/32);
            item.localScale=Vector3.one;
            var renderer=item.GetComponent<SpriteRenderer>();
            if(renderer==null) renderer=item.gameObject.AddComponent<SpriteRenderer>();
            if(sprite!=null) renderer.sprite=sprite;
            renderer.sortingOrder=order;
            return renderer;
        }

        private static SpriteRenderer Art(Transform parent,string name,int cell,Vector2 position,Vector2 size,int order)
            => Part(parent,name,position,VarginhaClassroomArt.Create(cell,size),order);

        private static void Wall(Transform parent,string name,Vector2 position,Vector2 size)
        {
            var sr=Part(parent,name,position,VarginhaClassroomArt.Surface(true),3);
            sr.drawMode=SpriteDrawMode.Tiled;
            sr.size=size;
            Collision(sr.gameObject,size,Vector2.zero);
        }

        private static void Collision(GameObject go,Vector2 size,Vector2 offset)
        {
            var collider=go.GetComponent<BoxCollider2D>();
            if(collider==null) collider=go.AddComponent<BoxCollider2D>();
            collider.size=size; collider.offset=offset;
        }
    }
}
