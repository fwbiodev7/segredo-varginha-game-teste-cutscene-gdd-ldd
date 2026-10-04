using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.Varginha.Experiment
{
    // Shared sprite projections and contact ellipses; no real-time shadow maps or texture copies.
    public sealed class CampaignCharacterShadows : MonoBehaviour
    {
        private CampaignIllustratedMaps.Layout _layout;
        private readonly List<CampaignCharacterShadow> _actors = new();
        private float _discover, _refresh;
        public void Configure(CampaignIllustratedMaps.Layout layout) => _layout = layout;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Install()
        {
            SceneManager.sceneLoaded -= SceneLoaded;
            SceneManager.sceneLoaded += SceneLoaded;
            SceneLoaded(SceneManager.GetActiveScene(), LoadSceneMode.Single);
        }
        private static void SceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.name.StartsWith("Menu") || FindAnyObjectByType<CampaignCharacterShadows>() != null) return;
            new GameObject("Sombras_dos_personagens").AddComponent<CampaignCharacterShadows>();
        }
        private void LateUpdate()
        {
            if (Time.unscaledTime >= _discover)
            {
                _discover = Time.unscaledTime + 2;
                foreach (var sr in FindObjectsByType<SpriteRenderer>(FindObjectsSortMode.None))
                {
                    bool actor = sr.GetComponent<EdelzioTopDownController>() != null
                        || sr.GetComponent<VarginhaStudentAnimation>() != null
                        || sr.GetComponent<VarginhaCombatEnemy>() != null
                        || sr.GetComponent<EntityManifestationAI>() != null
                        || sr.name.StartsWith("Refem_") || sr.name.StartsWith("Renan_")
                        || sr.name == "Padre Fábio" || sr.name == "Ouzana"
                        || sr.name == "Manifestação não combatível";
                    if (!actor || sr.GetComponent<CampaignCharacterShadow>() != null) continue;
                    var shadow = sr.gameObject.AddComponent<CampaignCharacterShadow>();
                    shadow.Configure(sr, transform); _actors.Add(shadow);
                }
            }
            if (Time.unscaledTime < _refresh) return;
            _refresh = Time.unscaledTime + .1f;
            foreach (var actor in _actors) if (actor != null)
                actor.SetLight(_layout, actor.Feet);
        }
        public static bool OpenAir(CampaignIllustratedMaps.Layout data, Vector2 feet)
        {
            if (data == null) return true;
            float x = (feet.x-data.Bounds.xMin)/data.Scale;
            float y = (data.Bounds.yMax-feet.y)/data.Scale;
            return data.phase == 3 || data.phase == 7
                || data.phase == 1 && x >= 914
                || data.phase == 2 && x >= 960
                || (data.phase == 4 || data.phase == 5) && (y >= 660 || x < 240 || x > 1150)
                || data.phase == 6 && (y >= 485 || x >= 510 && x <= 1025)
                || data.phase == 10 && y >= 667;
        }
        public static Vector2 AwayFromNearestLight(CampaignIllustratedMaps.Layout data, Vector2 feet, out float proximity)
        {
            proximity = 0; Vector2 away = new(.3f, -.7f); float closest = float.PositiveInfinity;
            if (data?.lights == null) return away.normalized;
            foreach (var lamp in data.lights)
            {
                Vector2 point = data.Position(lamp.pixel);
                if (data.repeat)
                {
                    float height = data.height*data.Scale;
                    point.y += Mathf.Round((feet.y-point.y)/height)*height;
                }
                float distance = (feet-point).sqrMagnitude;
                if (distance >= closest) continue;
                closest = distance; away = feet-point;
                proximity = Mathf.Clamp01(1-Mathf.Sqrt(distance)/lamp.radius);
            }
            return away.sqrMagnitude > .001f ? away.normalized : Vector2.down;
        }
    }

    [DefaultExecutionOrder(12000), DisallowMultipleComponent]
    public sealed class CampaignCharacterShadow : MonoBehaviour
    {
        private SpriteRenderer _actor, _projection, _contact;
        private CircleCollider2D _feet;
        private MaterialPropertyBlock _properties;
        private static Material _projectMaterial, _contactMaterial;
        private static Sprite _white;
        private Vector2 _direction = Vector2.down;
        private float _length = .7f, _opacity = .23f;
        private bool _outdoor;
        public Vector2 Feet => _feet != null ? (Vector2)_feet.transform.TransformPoint(_feet.offset)
            : new Vector2(_actor.bounds.center.x, _actor.bounds.min.y+.05f);
        public Vector2 Direction => _direction;
        public bool IsOpenAir => _outdoor;
        public void Configure(SpriteRenderer actor, Transform owner)
        {
            _actor = actor; _feet = actor.GetComponent<CircleCollider2D>();
            _properties = new MaterialPropertyBlock();
            if (_projectMaterial == null) _projectMaterial = new Material(Resources.Load<Shader>("Varginha/IllustratedMaps/CharacterShadow")) { hideFlags=HideFlags.DontSave };
            if (_contactMaterial == null) _contactMaterial = new Material(Resources.Load<Shader>("Varginha/IllustratedMaps/ContactShadow")) { hideFlags=HideFlags.DontSave };
            if (_white == null) _white = Sprite.Create(Texture2D.whiteTexture, new Rect(0,0,Texture2D.whiteTexture.width,Texture2D.whiteTexture.height), Vector2.one/2, Texture2D.whiteTexture.width);
            _projection = Create(owner, "Sombra_"+actor.name, _projectMaterial);
            _contact = Create(owner, "Contato_"+actor.name, _contactMaterial); _contact.sprite = _white;
        }
        private static SpriteRenderer Create(Transform owner, string name, Material material)
        {
            var go = new GameObject(name); go.transform.SetParent(owner, false);
            var sr = go.AddComponent<SpriteRenderer>(); sr.sharedMaterial = material; sr.sortingOrder = -950; return sr;
        }
        public void SetLight(CampaignIllustratedMaps.Layout layout, Vector2 feet)
        {
            // Legacy maps use the original floor ordering instead of the illustrated ground layer.
            _projection.sortingOrder = _contact.sortingOrder = layout == null ? 4 : -950;
            _outdoor = CampaignCharacterShadows.OpenAir(layout, feet);
            _direction = CampaignCharacterShadows.AwayFromNearestLight(layout, feet, out float proximity);
            _length = Mathf.Lerp(.5f, 1.05f, proximity);
            _opacity = Mathf.Lerp(.16f, .3f, proximity);
        }
        private void LateUpdate()
        {
            if (_actor == null || _contact == null) return;
            bool visible = _actor.enabled && _actor.gameObject.activeInHierarchy && _actor.sprite != null && _actor.color.a > .01f;
            _contact.enabled = visible; _projection.enabled = visible && !_outdoor;
            if (!visible) return;
            Vector2 feet = Feet;
            float width = Mathf.Clamp(_actor.bounds.size.x*.72f, .36f, .82f);
            _contact.transform.position = new Vector3(feet.x, feet.y-.025f, 0);
            _contact.transform.localScale = new Vector3(width, width*.36f, 1);
            _contact.color = new Color(.025f,.035f,.045f, (_outdoor?.29f:.22f)*_actor.color.a);
            _projection.sprite = _actor.sprite; _projection.flipX = _actor.flipX; _projection.flipY = _actor.flipY;
            _projection.transform.SetPositionAndRotation(_actor.transform.position, _actor.transform.rotation);
            _projection.transform.localScale = _actor.transform.lossyScale;
            _projection.color = new Color(.025f,.035f,.045f, _opacity*_actor.color.a);
            _properties.SetVector("_Ground", new Vector4(feet.x,feet.y,0,0));
            _properties.SetVector("_Projection", new Vector4(_direction.x,_direction.y,_length,Mathf.Max(.1f,_actor.bounds.size.y)));
            _projection.SetPropertyBlock(_properties);
        }
        private void OnDisable() { if (_projection != null) _projection.enabled=false; if (_contact != null) _contact.enabled=false; }
        private void OnDestroy() { if (_projection != null) Destroy(_projection.gameObject); if (_contact != null) Destroy(_contact.gameObject); }
    }
}
