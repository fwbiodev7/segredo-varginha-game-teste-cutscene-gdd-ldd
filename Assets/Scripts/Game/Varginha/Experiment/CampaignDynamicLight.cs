using System.Collections.Generic;
using UnityEngine;

namespace Game.Varginha.Experiment
{
    // Moving emitters share the same lighting field as the painted map's lamps.
    // The flashlight uses its existing scenery-clipped beam rather than a second cone.
    [DisallowMultipleComponent]
    public sealed class CampaignDynamicLight : MonoBehaviour
    {
        private static readonly List<CampaignDynamicLight> Sources = new();
        private readonly RaycastHit2D[] _hits = new RaycastHit2D[32];
        public EdelzioFlashlight flashlight;
        public Color color = new(1f, .88f, .61f);
        public float reach = 3.5f, intensity = .65f, halfAngle = 18f;
        public static IReadOnlyList<CampaignDynamicLight> Active => Sources;
        public Vector2 Origin => flashlight != null ? flashlight.BeamOrigin : transform.position;
        public Vector2 Direction => flashlight != null ? flashlight.BeamDirection : (Vector2)transform.up;
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetSources() => Sources.Clear();
        private void OnEnable() { if (!Sources.Contains(this)) Sources.Add(this); }
        private void OnDisable() => Sources.Remove(this);
        private void OnDestroy() => Sources.Remove(this);

        public float Weight(Vector2 point, CampaignIllustratedMaps.Layout layout)
        {
            if (!isActiveAndEnabled || flashlight != null && !flashlight.IlluminatesPoint(point)) return 0;
            Vector2 delta = point - Origin;
            float range = flashlight != null ? flashlight.Reach : reach;
            float angle = flashlight != null ? flashlight.HalfAngleRadians * Mathf.Rad2Deg : halfAngle;
            if (range <= 0 || delta.sqrMagnitude >= range * range) return 0;
            float edge = Mathf.InverseLerp(Mathf.Cos(angle * Mathf.Deg2Rad),
                Mathf.Cos(angle * .65f * Mathf.Deg2Rad), Vector2.Dot(Direction.normalized, delta.normalized));
            if (edge <= 0) return 0;
            if (flashlight == null && (CampaignLightField.Blocked(layout, Origin, point) || BlockedByScenery(point))) return 0;
            return intensity * edge * Mathf.Pow(1f - delta.magnitude / range, 1.4f);
        }

        private bool BlockedByScenery(Vector2 point)
        {
            Vector2 delta = point - Origin;
            int count = Physics2D.Raycast(Origin, delta.normalized,
                new ContactFilter2D { useTriggers = false }, _hits, delta.magnitude);
            for (int i = 0; i < count; i++)
            {
                var collider = _hits[i].collider;
                if (collider == null || collider.transform.IsChildOf(transform.parent != null ? transform.parent : transform)
                    || CampaignIllustratedLighting.IsActor(collider.GetComponent<SpriteRenderer>())
                    || collider.GetComponentInParent<VarginhaCombatTarget>() != null) continue;
                if (_hits[i].distance > .025f && _hits[i].distance < delta.magnitude - .06f) return true;
            }
            return false;
        }
    }

    public static class CampaignLightField
    {
        public struct Sample
        {
            public Color tint;
            public Vector2 direction;
            public float strength;
        }
        public static Sample Evaluate(CampaignIllustratedMaps.Layout layout, Vector2 feet)
        {
            Color ambient = layout?.ambient?.Length >= 3
                ? new Color(layout.ambient[0], layout.ambient[1], layout.ambient[2]) : Color.white;
            var result = new Sample { tint = ambient, direction = new Vector2(-.35f, .65f).normalized };
            if (layout?.lights != null) foreach (var lamp in layout.lights)
            {
                if (lamp.radius <= 0 || lamp.color == null || lamp.color.Length < 3) continue;
                var point = layout.Position(lamp.pixel);
                if (layout.repeat) point.y += Mathf.Round((feet.y - point.y) / (layout.height * layout.Scale)) * layout.height * layout.Scale;
                float weight = Mathf.Max(0, 1 - (point - feet).sqrMagnitude / (lamp.radius * lamp.radius));
                weight = weight * weight * .42f;
                if (weight <= 0 || Blocked(layout, point, feet)) continue;
                Add(ref result, new Color(lamp.color[0], lamp.color[1], lamp.color[2]), weight, point - feet);
            }
            foreach (var source in CampaignDynamicLight.Active)
                if (source != null) Add(ref result, source.color, source.Weight(feet, layout), source.Origin - feet);
            result.tint = new Color(Mathf.Min(1.18f, result.tint.r), Mathf.Min(1.18f, result.tint.g), Mathf.Min(1.18f, result.tint.b), 1);
            return result;
        }
        private static void Add(ref Sample sample, Color color, float weight, Vector2 direction)
        {
            sample.tint += color * weight;
            if (weight <= sample.strength) return;
            sample.strength = weight;
            if (direction.sqrMagnitude > .001f) sample.direction = direction.normalized;
        }
        // Use the map's authored wall footprints: furniture and character colliders
        // must not incorrectly separate two characters in the same room.
        public static bool Blocked(CampaignIllustratedMaps.Layout layout, Vector2 start, Vector2 end)
        {
            if (layout?.walls == null || layout.repeat) return false;
            foreach (var wall in layout.walls)
            {
                if (wall.water || wall.rect == null) continue;
                Rect rect = layout.Area(wall.rect);
                if (rect.Contains(start) || rect.Contains(end)) continue;
                Vector2 delta = end - start;
                float near = 0, far = 1;
                for (int axis = 0; axis < 2; axis++)
                {
                    float min = axis == 0 ? rect.xMin : rect.yMin, max = axis == 0 ? rect.xMax : rect.yMax;
                    if (Mathf.Abs(delta[axis]) < .00001f)
                    { if (start[axis] < min || start[axis] > max) { far = -1; break; } continue; }
                    float a = (min - start[axis]) / delta[axis], b = (max - start[axis]) / delta[axis];
                    near = Mathf.Max(near, Mathf.Min(a, b)); far = Mathf.Min(far, Mathf.Max(a, b));
                }
                if (near <= far && near > .001f && near < .999f) return true;
            }
            return false;
        }
    }
}
