using UnityEngine;

namespace Game.Varginha.Experiment
{
    // Shares the existing atlas, lighting and ground-sorting systems.
    public sealed class CampaignOuzanaBiologist : MonoBehaviour
    {
        private SpriteRenderer _renderer;
        private EdelzioTopDownController _player;
        private Vector2 _facing=Vector2.down;
        private Vector3 _lastPosition;
        private float _walkClock;
        private void Awake()
        {
            _renderer=GetComponent<SpriteRenderer>();_renderer.sharedMaterial=CampaignOuzanaVisual.Material;
            _lastPosition=transform.position;
        }
        private void Start() => _player = FindAnyObjectByType<EdelzioTopDownController>();
        private void LateUpdate()
        {
            if(Time.timeScale<=0)return;
            Vector2 travel=transform.position-_lastPosition;_lastPosition=transform.position;
            bool moving=travel.sqrMagnitude>.000001f;
            if(moving){_facing=travel.normalized;_walkClock+=Time.deltaTime;}
            else if (_player != null)
            {
                Vector2 toward = _player.transform.position - transform.position;
                if (toward.sqrMagnitude < 9 && toward.sqrMagnitude > .04f)
                    _facing=toward.normalized;
            }
            var sprite=moving?CampaignOuzanaVisual.Walk(_facing,_walkClock):CampaignOuzanaVisual.Idle(_facing);
            if(sprite!=null)_renderer.sprite=sprite;
        }
    }
}
