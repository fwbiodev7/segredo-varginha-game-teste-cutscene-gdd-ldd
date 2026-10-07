using Game.Varginha.Experiment;
using NUnit.Framework;
using UnityEngine;

namespace Game.Tests.EditMode
{
    public class CampaignDynamicLightingTests
    {
        private GameObject _source;
        [TearDown] public void Cleanup() { if (_source != null) Object.DestroyImmediate(_source); }
        private static CampaignIllustratedMaps.Layout Layout() => new()
        { width = 100, height = 100, bounds = new[] { 0f, 0f, 10f, 10f }, ambient = new[] { .4f, .4f, .5f }, walls = new CampaignIllustratedMaps.Wall[0] };
        [Test] public void MovingLightChangesTintAndDirectionAndDisableRestoresAmbient()
        {
            _source = new GameObject("Luz de teste"); var light = _source.AddComponent<CampaignDynamicLight>();
            // Runtime emitters intentionally do not execute in editor previews.
            typeof(CampaignDynamicLight).GetMethod("OnEnable", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).Invoke(light, null);
            light.reach = 5; light.intensity = .6f; _source.transform.position = new Vector3(5, 2);
            var first = CampaignLightField.Evaluate(Layout(), new Vector2(5, 3));
            Assert.That(first.tint.r, Is.GreaterThan(.7f)); Assert.That(first.direction.y, Is.LessThan(-.9f));
            _source.transform.position = new Vector3(5, 4); _source.transform.rotation = Quaternion.Euler(0, 0, 180);
            var moved = CampaignLightField.Evaluate(Layout(), new Vector2(5, 3));
            Assert.That(moved.direction.y, Is.GreaterThan(.9f));
            light.enabled = false;
            typeof(CampaignDynamicLight).GetMethod("OnDisable", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).Invoke(light, null);
            Assert.That(CampaignLightField.Evaluate(Layout(), new Vector2(5, 3)).tint.r, Is.EqualTo(.4f).Within(.001f));
        }
        [Test] public void ConeRejectsSidesRangeAndAuthoredWalls()
        {
            _source = new GameObject("Luz de teste"); var light = _source.AddComponent<CampaignDynamicLight>();
            _source.transform.position = new Vector3(5, 2); var layout = Layout();
            Assert.That(light.Weight(new Vector2(5, 3), layout), Is.GreaterThan(0));
            Assert.That(light.Weight(new Vector2(6, 2), layout), Is.Zero);
            Assert.That(light.Weight(new Vector2(5, 8), layout), Is.Zero);
            layout.walls = new[] { new CampaignIllustratedMaps.Wall { rect = new[] { 40f, 70f, 20f, 5f } } };
            Assert.That(light.Weight(new Vector2(5, 3.5f), layout), Is.Zero);
        }
        [Test] public void PaintedLampDoesNotLightTheNeighbouringRoom()
        {
            var layout = Layout(); layout.lights = new[] { new CampaignIllustratedMaps.Light { pixel = new[] { 30f, 50f }, radius = 5, color = new[] { 1f, .8f, .5f } } };
            layout.walls = new[] { new CampaignIllustratedMaps.Wall { rect = new[] { 50f, 0f, 4f, 100f } } };
            Assert.That(CampaignLightField.Evaluate(layout, new Vector2(4, 5)).tint.r, Is.GreaterThan(.7f));
            Assert.That(CampaignLightField.Evaluate(layout, new Vector2(6, 5)).tint.r, Is.EqualTo(.4f).Within(.001f));
        }
        [TestCase("Renan_Apoio", true)] [TestCase("Entidade ferida", true)] [TestCase("Sombra_Renan_Apoio", false)] [TestCase("Contato_Edelzio", false)]
        public void CharacterDiscoveryIncludesSupportAndExcludesShadowVisuals(string name, bool actor)
        {
            _source = new GameObject(name); var sr = _source.AddComponent<SpriteRenderer>();
            Assert.That(CampaignIllustratedLighting.IsActor(sr), Is.EqualTo(actor));
        }
    }
}
