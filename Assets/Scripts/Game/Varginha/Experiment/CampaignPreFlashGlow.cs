using UnityEngine;

namespace Game.Varginha.Experiment
{
    // A quiet signal before the existing encounter, sharing its authored explosion art.
    public sealed class CampaignPreFlashGlow : MonoBehaviour
    {
        private static Sprite _coreArt;
        private SpriteRenderer _core,_halo;
        private void Awake()
        {
            transform.position=new Vector3(17.2f,0);
            _halo=Create("Halo",VarginhaSceneryArt.Create("Glow",new Vector2(.85f,.85f)),22000);
            if(_coreArt==null||_coreArt.texture==null)
            {
                var source=CampaignFlashArt.Explosion[0];
                // Measured opaque seed inside frame zero; reuse its texture without its padding.
                _coreArt=Sprite.Create(source.texture,new Rect(source.rect.x+169,source.rect.y+100,64,66),Vector2.one/2,source.pixelsPerUnit,0,SpriteMeshType.FullRect);
                _coreArt.name="Semente_do_clarao";
            }
            _core=Create("Nucleo",_coreArt,22001);
            _core.sharedMaterial=CampaignFlashArt.Material;
            _core.transform.localScale=Vector3.one*(.13f/_core.sprite.bounds.size.y);
            gameObject.SetActive(false);
        }
        private SpriteRenderer Create(string label,Sprite sprite,int order)
        {
            var go=new GameObject(label);go.transform.SetParent(transform,false);
            var renderer=go.AddComponent<SpriteRenderer>();renderer.sprite=sprite;renderer.sortingOrder=order;
            return renderer;
        }
        private void Update()
        {
            float pulse=VarginhaGameSettings.Current.reducedMotion ? .5f : .5f+.5f*Mathf.Sin(Time.time*2.2f);
            _core.color=new Color(.78f,.94f,1,.7f+pulse*.15f);
            _halo.color=new Color(.48f,.76f,1,.38f+pulse*.12f);
        }
    }
}
