using System.Collections.Generic;
using UnityEngine;

namespace Game.Varginha.Experiment
{
    // One shared material; moving sources update each frame, without per-actor materials.
    [DefaultExecutionOrder(11000)]
    public sealed class CampaignIllustratedLighting:MonoBehaviour
    {
        private CampaignIllustratedMaps.Layout _data;
        private readonly List<SpriteRenderer> _actors=new();
        private MaterialPropertyBlock _block;
        private static Material _material;
        private float _discover;
        private static readonly int Tint=Shader.PropertyToID("_SceneTint"),Key=Shader.PropertyToID("_KeyLight"),Shine=Shader.PropertyToID("_Shine"),Body=Shader.PropertyToID("_ActorBounds");
        public void Configure(CampaignIllustratedMaps.Layout data)
        {
            _block ??=new MaterialPropertyBlock();_data=data;
            // Preview construction must not assign runtime materials to the authored editor scene.
            if(Application.isPlaying)Discover();
        }
        private void Start()=>Discover();
        public static bool IsActor(SpriteRenderer renderer)
        {
            if(renderer==null||renderer.name.StartsWith("Sombra_")||renderer.name.StartsWith("Contato_")||renderer.name.Contains("Shadow"))return false;
            return renderer.GetComponentInParent<EdelzioTopDownController>()!=null
                ||renderer.GetComponent<VarginhaStudentAnimation>()!=null||renderer.GetComponent<CampaignSchoolLife>()!=null
                ||renderer.GetComponent<VarginhaCombatEnemy>()!=null||renderer.GetComponent<EntityManifestationAI>()!=null
                ||renderer.GetComponent<CampaignManifestationCombat>()!=null||renderer.name.StartsWith("Refem_")
                ||renderer.name.StartsWith("Renan_")||renderer.name=="Ouzana"||renderer.name=="Padre Fábio"
                ||renderer.name.EndsWith("_Apoio")||renderer.name=="Entidade ferida"||renderer.name=="Manifestação não combatível"
                ||renderer.name.StartsWith("Fusca_TopView")||renderer.name=="Fusca";
        }
        private void Discover()
        {
            if(_material==null)_material=new Material(Resources.Load<Shader>("Varginha/IllustratedMaps/ActorLighting")){name="Iluminação leve dos personagens",hideFlags=HideFlags.DontSave};
            foreach(var renderer in FindObjectsByType<SpriteRenderer>())
            {
                if(!IsActor(renderer)||_actors.Contains(renderer))continue;
                if(renderer.sprite!=null)renderer.sprite.texture.filterMode=FilterMode.Point;
                renderer.sharedMaterial=CampaignStainedGlassLighting.Enabled(_data)?CampaignStainedGlassLighting.ActorMaterial:_material;_actors.Add(renderer);
            }
            _discover=Time.unscaledTime+2;
        }
        private void LateUpdate()
        {
            if(_data==null)return;if(Time.unscaledTime>=_discover)Discover();
            _block??=new MaterialPropertyBlock();
            for(int i=_actors.Count-1;i>=0;i--)
            {
                var renderer=_actors[i];if(renderer==null){_actors.RemoveAt(i);continue;}
                if(!renderer.enabled||!renderer.gameObject.activeInHierarchy)continue;
                var feet=renderer.transform.position;
                var ground=renderer.GetComponent<CircleCollider2D>();
                if(ground!=null)feet=ground.transform.TransformPoint(ground.offset);
                var sample=CampaignLightField.Evaluate(_data,feet);
                renderer.GetPropertyBlock(_block);_block.SetColor(Tint,sample.tint);
                _block.SetVector(Key,new Vector4(sample.direction.x,sample.direction.y,.45f+sample.strength*.6f,sample.strength));
                var bounds=renderer.bounds;
                _block.SetVector(Body,new Vector4(bounds.center.x,bounds.center.y,Mathf.Max(.01f,bounds.size.x),Mathf.Max(.01f,bounds.size.y)));
                _block.SetFloat(Shine,renderer.name.Contains("Fusca")?.2f:.09f);renderer.SetPropertyBlock(_block);
            }
        }
    }
}
