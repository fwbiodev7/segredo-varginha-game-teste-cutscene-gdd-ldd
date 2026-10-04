using System.Collections.Generic;
using UnityEngine;

namespace Game.Varginha.Experiment
{
    // One shared material; property blocks are updated at 10 Hz, with no frame allocations.
    [DefaultExecutionOrder(11000)]
    public sealed class CampaignIllustratedLighting:MonoBehaviour
    {
        private CampaignIllustratedMaps.Layout _data;
        private readonly List<SpriteRenderer> _actors=new();
        private MaterialPropertyBlock _block;
        private static Material _material;
        private float _discover,_refresh,_discoverUntil;
        private static readonly int Tint=Shader.PropertyToID("_SceneTint"),Key=Shader.PropertyToID("_KeyLight"),Shine=Shader.PropertyToID("_Shine");
        public void Configure(CampaignIllustratedMaps.Layout data)
        {
            _block ??=new MaterialPropertyBlock();_data=data;_discoverUntil=Time.unscaledTime+6;
            // Preview construction must not assign runtime materials to the authored editor scene.
            if(Application.isPlaying)Discover();
        }
        private void Start()=>Discover();
        private void Discover()
        {
            if(_material==null)_material=new Material(Resources.Load<Shader>("Varginha/IllustratedMaps/ActorLighting")){name="Iluminação leve dos personagens",hideFlags=HideFlags.DontSave};
            foreach(var renderer in FindObjectsByType<SpriteRenderer>(FindObjectsSortMode.None))
            {
                bool actor=renderer.GetComponentInParent<EdelzioTopDownController>()!=null||renderer.GetComponent<VarginhaStudentAnimation>()!=null||renderer.GetComponent<CampaignSchoolLife>()!=null||renderer.GetComponent<VarginhaCombatEnemy>()!=null||renderer.GetComponent<EntityManifestationAI>()!=null||renderer.name.StartsWith("Refem_")||renderer.name.StartsWith("Renan_")||renderer.name=="Ouzana"||renderer.name=="Padre Fábio"||renderer.name.StartsWith("Fusca_TopView")||(_data.phase==10&&renderer.name=="Fusca");
                if(!actor||_actors.Contains(renderer))continue;
                if(renderer.sprite!=null)renderer.sprite.texture.filterMode=FilterMode.Point;
                renderer.sharedMaterial=_material;_actors.Add(renderer);
            }
            _discover=Time.unscaledTime+2;
        }
        private void LateUpdate()
        {
            if(_data==null)return;if(Time.unscaledTime<=_discoverUntil&&Time.unscaledTime>=_discover)Discover();
            if(Time.unscaledTime<_refresh)return;_refresh=Time.unscaledTime+.1f;
            var ambient=new Vector3(_data.ambient[0],_data.ambient[1],_data.ambient[2]);
            for(int i=_actors.Count-1;i>=0;i--)
            {
                var renderer=_actors[i];if(renderer==null){_actors.RemoveAt(i);continue;}
                if(!renderer.enabled||!renderer.gameObject.activeInHierarchy)continue;
                var feet=renderer.transform.position;
                var ground=renderer.GetComponent<CircleCollider2D>();
                if(ground!=null)feet=ground.transform.TransformPoint(ground.offset);
                Vector3 color=ambient;float best=0;Vector2 key=new(-.35f,.65f);
                foreach(var lamp in _data.lights)
                {
                    var p=_data.Position(lamp.pixel);if(_data.repeat)p.y=_data.Bounds.yMin+_data.height*_data.Scale-lamp.pixel[1]*_data.Scale+Mathf.Floor((feet.y-_data.Bounds.yMin)/(_data.height*_data.Scale))*(_data.height*_data.Scale);
                    var delta=p-(Vector2)feet;float weight=Mathf.Max(0,1-delta.sqrMagnitude/(lamp.radius*lamp.radius));weight*=weight;
                    color+=new Vector3(lamp.color[0],lamp.color[1],lamp.color[2])*(weight*.42f);
                    if(weight>best){best=weight;key=delta.normalized;}
                }
                renderer.GetPropertyBlock(_block);_block.SetColor(Tint,new Color(Mathf.Min(1.18f,color.x),Mathf.Min(1.18f,color.y),Mathf.Min(1.18f,color.z),1));
                _block.SetVector(Key,new Vector4(key.x,key.y,.45f+best*.25f,0));_block.SetFloat(Shine,renderer.name.Contains("Fusca")?.2f:.07f);renderer.SetPropertyBlock(_block);
            }
        }
    }
}
