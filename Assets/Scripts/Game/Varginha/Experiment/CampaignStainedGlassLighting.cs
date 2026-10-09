using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Game.Varginha.Experiment
{
    public static class CampaignStainedGlassLighting
    {
        private static Material _actor, _scenery;
        public static bool Enabled(CampaignIllustratedMaps.Layout data) => data?.stainedGlass?.Length > 0 && GraphicsSettings.currentRenderPipeline is UniversalRenderPipelineAsset;
        public static Material ActorMaterial => Material(ref _actor, 1);
        private static Material Material(ref Material material, float response)
        {
            if (material == null)
            {
                material = new Material(Resources.Load<Shader>("Varginha/IllustratedMaps/StainedGlassLighting")) { name = "Vitral pixel art " + response, hideFlags = HideFlags.DontSave };
                material.SetFloat("_GlassResponse", response);
            }
            return material;
        }
        public static void Build(Transform map, CampaignIllustratedMaps.Layout data)
        {
            if (!Enabled(data)) return;
            var root = new GameObject("03_Iluminacao_2D_Vitrais").transform; root.SetParent(map, false);
            var global = Source(root, "Ambiente_neutro", Color.white, Light2D.LightType.Global);
            global.intensity = 1;
            int order = 1;
            foreach (var glass in data.stainedGlass)
            {
                var light = Source(root, glass.name, new Color(glass.color[0], glass.color[1], glass.color[2]), Light2D.LightType.Freeform);
                light.intensity = glass.intensity;
                light.overlapOperation = Light2D.OverlapOperation.AlphaBlend;
                light.lightOrder = order++;
                light.shapeLightFalloffSize = glass.falloff;
                light.falloffIntensity = .5f;
                var path = new Vector3[glass.outline.Length / 2];
                for (int i = 0; i < path.Length; i++) path[i] = data.Position(glass.outline[i * 2], glass.outline[i * 2 + 1]);
                light.SetShapePath(path);
            }
            // The painted floor already contains the glass projection; a restrained response preserves it.
            foreach (var renderer in map.GetComponentsInChildren<SpriteRenderer>()) renderer.sharedMaterial = Material(ref _scenery, .18f);
        }
        private static Light2D Source(Transform parent, string name, Color color, Light2D.LightType type)
        {
            var go = new GameObject(name); go.transform.SetParent(parent, false);
            var light = go.AddComponent<Light2D>(); light.lightType = type; light.color = color;
            light.blendStyleIndex = 0; light.shadowsEnabled = false;
            return light;
        }
    }
}
