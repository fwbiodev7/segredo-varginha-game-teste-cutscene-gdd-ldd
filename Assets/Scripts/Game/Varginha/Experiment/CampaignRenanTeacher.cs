using UnityEngine;

namespace Game.Varginha.Experiment
{
    public sealed class CampaignRenanTeacher : MonoBehaviour
    {
        private SpriteRenderer _renderer;
        private void Awake()=>_renderer=GetComponent<SpriteRenderer>();
        private void LateUpdate()
        {
            // Writing, pointing to the board, then explaining to the seated class.
            float time=Time.time%12;
            int row=time<7?3:0;
            int frame=time<7?1+Mathf.FloorToInt(Time.time*2)%2:Mathf.FloorToInt(Time.time)%4;
            var sprite=CampaignStorySprites.Frame("RenanTeaching",row,frame);
            if(sprite!=null)_renderer.sprite=sprite;
        }
        public static void DeskItems(Transform owner,CampaignIllustratedMaps.Layout layout)
        {
            if(owner.Find("Objetos_da_mesa_do_Renan")!=null)return;
            var desk=owner.Find("Mapa_Campanha/02_Mobilia_Colisoes/Mesa do professor")
                ??GameObject.Find("Mapa_Campanha/02_Mobilia_Colisoes/Mesa do professor")?.transform;
            var items=new GameObject("Objetos_da_mesa_do_Renan").transform;items.SetParent(owner,false);
            // Bottom contacts rest on the actual desktop, not at arbitrary sprite centres.
            Vector2[] pixels={new(684,77),new(717,77),new(745,77)};
            Vector2[] sizes={new(.39f,.46f),new(.65f,.42f),new(.30f,.14f)};
            for(int i=0;i<3;i++)
            {
                var go=new GameObject(new[]{"Mochila_cinza_do_Renan","Notebook_do_Renan","Pinceis_de_quadro_do_Renan"}[i]);go.transform.SetParent(items,false);
                var sr=go.AddComponent<SpriteRenderer>();sr.sprite=CampaignStorySprites.Frame("RenanDeskProps",0,i);
                if(sr.sprite==null)continue;
                float fit=Mathf.Min(sizes[i].x/sr.sprite.bounds.size.x,sizes[i].y/sr.sprite.bounds.size.y);
                go.transform.localScale=Vector3.one*fit;
                var contact=new Vector3(sr.sprite.bounds.center.x,sr.sprite.bounds.min.y,0);
                go.transform.position=(Vector3)layout.Position(pixels[i].x,pixels[i].y)-contact*fit;
                VarginhaWorldDepth.Ensure(sr,supportingObject:desk,offset:3+i);
            }
            var board=new GameObject("Anotações_no_quadro_do_Renan");board.transform.SetParent(owner,false);board.transform.position=layout.Position(990,38);
            var text=board.AddComponent<TextMesh>();text.text="1996\n2026";text.font=Resources.Load<Font>("Fonts/PressStart2P-Regular");text.fontSize=24;text.characterSize=.06f;text.color=new Color(.16f,.23f,.27f);
            text.anchor=TextAnchor.UpperLeft;text.alignment=TextAlignment.Left;
            var mesh=board.GetComponent<MeshRenderer>();mesh.sharedMaterial=text.font.material;mesh.sortingOrder=22000;
        }
    }
}
