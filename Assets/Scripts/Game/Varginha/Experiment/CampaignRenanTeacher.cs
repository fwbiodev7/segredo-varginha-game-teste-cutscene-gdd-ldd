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
            var desk=GameObject.Find("Mapa_Campanha/02_Mobilia_Colisoes/Mesa do professor");
            Vector2[] pixels={new(685,70),new(714,70),new(742,70)};
            Vector2[] sizes={new(.4f,.5f),new(.49f,.27f),new(.38f,.3f)};
            for(int i=0;i<3;i++)
            {
                var go=new GameObject(new[]{"Mochila_cinza_do_Renan","Notebook_do_Renan","Material_de_aula_do_Renan"}[i]);go.transform.SetParent(owner,false);
                var sr=go.AddComponent<SpriteRenderer>();sr.sprite=CampaignStorySprites.Frame("RenanProps",0,i);
                float fit=Mathf.Min(sizes[i].x/sr.sprite.bounds.size.x,sizes[i].y/sr.sprite.bounds.size.y);
                go.transform.localScale=Vector3.one*fit;
                go.transform.position=(Vector3)layout.Position(pixels[i].x,pixels[i].y)-sr.sprite.bounds.center*fit;
                VarginhaWorldDepth.Ensure(sr,supportingObject:desk?.transform,offset:3);
            }
            var board=new GameObject("Anotações_no_quadro_do_Renan");board.transform.SetParent(owner,false);board.transform.position=layout.Position(990,38);
            var text=board.AddComponent<TextMesh>();text.text="1996\n2026";text.font=Resources.Load<Font>("Fonts/PressStart2P-Regular");text.fontSize=24;text.characterSize=.06f;text.color=new Color(.16f,.23f,.27f);
            text.anchor=TextAnchor.UpperLeft;text.alignment=TextAlignment.Left;
            var mesh=board.GetComponent<MeshRenderer>();mesh.sharedMaterial=text.font.material;mesh.sortingOrder=22000;
        }
    }
}
