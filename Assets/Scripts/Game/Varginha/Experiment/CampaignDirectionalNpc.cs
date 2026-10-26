using UnityEngine;

namespace Game.Varginha.Experiment
{
    /// <summary>Faces a stationary dialogue character toward the player using its authored eight-way idle bank.</summary>
    [DisallowMultipleComponent]
    public sealed class CampaignDirectionalNpc : MonoBehaviour
    {
        SpriteRenderer _renderer;
        Transform _target;
        string _sheet;
        int _direction=-1;

        public void Configure(string sheet,Transform target)
        {
            _renderer=GetComponent<SpriteRenderer>();_sheet=sheet;_target=target;
            Present(Vector2.down);
        }

        void LateUpdate()
        {
            if(_renderer==null||_target==null||string.IsNullOrEmpty(_sheet))return;
            Vector2 facing=(Vector2)_target.position-(Vector2)transform.position;
            if(facing.sqrMagnitude<.01f)return;
            Present(facing);
        }

        void Present(Vector2 facing)
        {
            int next=CampaignStorySprites.EightDirection(facing);
            if(next==_direction)return;
            var sprite=CampaignStorySprites.Frame(_sheet,next,0);
            if(sprite!=null){_renderer.sprite=sprite;_direction=next;}
        }
    }
}
