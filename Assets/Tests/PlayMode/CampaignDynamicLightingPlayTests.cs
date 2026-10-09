using System.Collections;
using System.IO;
using Game.Varginha;
using Game.Varginha.Experiment;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace Game.Tests.PlayMode
{
    public class CampaignDynamicLightingPlayTests
    {
        private string _save;
        private GameObject _source, _late;
        private bool _has, _active, _equipped;
        [SetUp] public void Preserve()
        {
            _save = File.Exists(CampaignStorySave.Path) ? File.ReadAllText(CampaignStorySave.Path) : null;
            _has = EdelzioTopDownController.PersistentHasFlashlight; _active = EdelzioTopDownController.PersistentFlashlightActive; _equipped = EdelzioTopDownController.PersistentFlashlightInHotbar;
        }
        [TearDown] public void Restore()
        {
            Time.timeScale = 1;
            EdelzioTopDownController.PersistentHasFlashlight = _has; EdelzioTopDownController.PersistentFlashlightActive = _active; EdelzioTopDownController.PersistentFlashlightInHotbar = _equipped;
            if (_source != null) Object.DestroyImmediate(_source);
            if (_late != null) Object.DestroyImmediate(_late);
            if (_save != null) File.WriteAllText(CampaignStorySave.Path, _save); else if (File.Exists(CampaignStorySave.Path)) File.Delete(CampaignStorySave.Path);
        }
        [UnityTest] public IEnumerator ActualCharacterReceivesMovingLightAndItsShadowUsesTheSameDirection()
        {
            CampaignStorySave.Write(new CampaignStory { phase = 2 });
            yield return SceneManager.LoadSceneAsync(CampaignStorySave.Scene(2)); yield return new WaitForSeconds(.5f);
            var player = Object.FindAnyObjectByType<EdelzioTopDownController>(); player.SetInputLocked(true);
            var body = player.GetComponent<SpriteRenderer>(); var original = body.sprite;
            var feetCollider = player.GetComponent<CircleCollider2D>(); var offset = feetCollider.offset;
            Vector2 feet = CampaignIllustratedMaps.Get(2).Position(270, 345);
            player.GetComponent<Rigidbody2D>().position = feet - offset;
            yield return new WaitForFixedUpdate(); yield return null;
            var block = new MaterialPropertyBlock(); body.GetPropertyBlock(block); Color before = block.GetColor("_SceneTint");
            var camera = Camera.main; Assert.That(camera, Is.Not.Null);
            Capture(camera, "Casa_Ambiente.png");
            _source = new GameObject("Luz dinamica QA"); _source.transform.position = feet + Vector2.down * 1.2f;
            _source.AddComponent<CampaignDynamicLight>().intensity = .8f;
            yield return null; yield return null;
            body.GetPropertyBlock(block); Color after = block.GetColor("_SceneTint");
            Assert.That(after.r - before.r, Is.GreaterThan(.15f));
            var key = block.GetVector("_KeyLight"); var shadow = player.GetComponent<CampaignCharacterShadow>();
            Assert.That(shadow, Is.Not.Null); Assert.That(Vector2.Dot(shadow.Direction, -new Vector2(key.x, key.y)), Is.GreaterThan(.99f));
            Capture(camera, "Casa_Luz_Dinamica.png");
            _source.transform.position = feet + Vector2.up * 1.2f; _source.transform.rotation = Quaternion.Euler(0, 0, 180);
            yield return null; yield return null; body.GetPropertyBlock(block);
            Assert.That(block.GetVector("_KeyLight").y, Is.GreaterThan(.9f));
            Assert.That(body.sprite.texture, Is.EqualTo(original.texture)); Assert.That(feetCollider.offset, Is.EqualTo(offset));
            _source.SetActive(false); yield return null; yield return null; body.GetPropertyBlock(block);
            Assert.That(block.GetColor("_SceneTint").r, Is.EqualTo(before.r).Within(.015f));
            yield return SceneManager.LoadSceneAsync("Menu_MisterioDeVarginha");
        }
        [UnityTest] public IEnumerator CharactersCreatedAfterSixSecondsStillReceiveMapLighting()
        {
            CampaignStorySave.Write(new CampaignStory { phase = 2 });
            yield return SceneManager.LoadSceneAsync(CampaignStorySave.Scene(2)); yield return new WaitForSeconds(6.2f);
            var player = Object.FindAnyObjectByType<EdelzioTopDownController>();
            _late = new GameObject("Renan_Apoio"); var sr = _late.AddComponent<SpriteRenderer>();
            sr.sprite = player.GetComponent<SpriteRenderer>().sprite; _late.transform.position = player.transform.position + Vector3.right;
            yield return new WaitForSeconds(2.2f);
            Assert.That(sr.sharedMaterial.shader.name, Is.EqualTo("Varginha/IllustratedActorLighting"));
            var block = new MaterialPropertyBlock(); sr.GetPropertyBlock(block); Assert.That(block.GetColor("_SceneTint").a, Is.EqualTo(1));
            Assert.That(Object.FindObjectsByType<CampaignCharacterShadow>().Length, Is.LessThan(32));
            yield return SceneManager.LoadSceneAsync("Menu_MisterioDeVarginha");
        }
        [UnityTest] public IEnumerator EquippedFlashlightLightsACharacterButStopsAtScenery()
        {
            CampaignStorySave.Write(new CampaignStory { phase = 2 });
            yield return SceneManager.LoadSceneAsync(CampaignStorySave.Scene(2)); yield return new WaitForSeconds(.5f);
            var player = Object.FindAnyObjectByType<EdelzioTopDownController>(); player.SetInputLocked(true);
            var feet = player.GetComponent<CircleCollider2D>(); player.GetComponent<Rigidbody2D>().position = CampaignIllustratedMaps.Get(2).Position(270, 350) - feet.offset;
            player.HasFlashlight = true; player.IsFlashlightEquippedInHotbar = true; player.FlashlightActive = false;
            var beam = player.GetComponent<EdelzioFlashlight>(); if (beam == null) beam = player.gameObject.AddComponent<EdelzioFlashlight>();
            const System.Reflection.BindingFlags flags = System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance;
            typeof(EdelzioFlashlight).GetField("followCursor", flags).SetValue(beam, false);
            typeof(EdelzioTopDownController).GetField("_lastFacing", flags).SetValue(player, Vector2.right);
            yield return new WaitForFixedUpdate(); yield return null;
            _late = new GameObject("Renan_Apoio"); var receiver = _late.AddComponent<SpriteRenderer>();
            receiver.sprite = player.GetComponent<SpriteRenderer>().sprite; _late.transform.position = player.transform.position + Vector3.right * 1.2f;
            yield return new WaitForSeconds(2.1f);
            var block = new MaterialPropertyBlock(); receiver.GetPropertyBlock(block); Color before = block.GetColor("_SceneTint");
            player.FlashlightActive = true; yield return null; yield return null;
            Assert.That(beam.IlluminatesPoint(_late.transform.position), Is.True);
            receiver.GetPropertyBlock(block); Assert.That(block.GetColor("_SceneTint").r - before.r, Is.GreaterThan(.08f));
            _source = new GameObject("Parede QA"); _source.transform.position = Vector2.Lerp(beam.BeamOrigin, _late.transform.position, .5f);
            _source.AddComponent<BoxCollider2D>().size = new Vector2(.12f, 1.4f); Physics2D.SyncTransforms();
            yield return null; yield return null;
            Assert.That(beam.IlluminatesPoint(_late.transform.position), Is.False);
            receiver.GetPropertyBlock(block); Assert.That(block.GetColor("_SceneTint").r, Is.EqualTo(before.r).Within(.015f));
            yield return SceneManager.LoadSceneAsync("Menu_MisterioDeVarginha");
        }
        [UnityTest] public IEnumerator FuscaHeadlightsFollowTheCarAndRegisterOnlyWhileSwitchedOn()
        {
            yield return SceneManager.LoadSceneAsync("Menu_MisterioDeVarginha"); yield return null;
            int before = CampaignDynamicLight.Active.Count;
            _source = new GameObject("Fusca QA"); CampaignFuscaLighting.Add(_source.transform);
            Assert.That(CampaignDynamicLight.Active.Count, Is.EqualTo(before + 2));
            var lamps = _source.GetComponentsInChildren<CampaignDynamicLight>();
            Assert.That(lamps[0].Weight(new Vector2(0, 2), null) + lamps[1].Weight(new Vector2(0, 2), null), Is.GreaterThan(.2f));
            _source.transform.rotation = Quaternion.Euler(0, 0, 90);
            Assert.That(lamps[0].Weight(new Vector2(0, 2), null), Is.Zero);
            Assert.That(lamps[0].Weight(new Vector2(-2, 0), null) + lamps[1].Weight(new Vector2(-2, 0), null), Is.GreaterThan(.2f));
            CampaignFuscaLighting.SetEnabled(_source.transform, false);
            Assert.That(CampaignDynamicLight.Active.Count, Is.EqualTo(before));
            CampaignFuscaLighting.SetEnabled(_source.transform, true); CampaignFuscaLighting.SetEnabled(_source.transform, true);
            Assert.That(CampaignDynamicLight.Active.Count, Is.EqualTo(before + 2));
        }
        private static void Capture(Camera camera, string name)
        {
            var previous = camera.targetTexture; var active = RenderTexture.active;
            var target = RenderTexture.GetTemporary(1280, 960, 24); var image = new Texture2D(1280, 960, TextureFormat.RGB24, false);
            try
            {
                camera.targetTexture = target; VarginhaPixelPresentation.RenderInto(camera,target); RenderTexture.active = target;
                image.ReadPixels(new Rect(0, 0, 1280, 960), 0, 0); image.Apply();
                Directory.CreateDirectory("Docs/QAIluminacao20261007"); File.WriteAllBytes("Docs/QAIluminacao20261007/" + name, image.EncodeToPNG());
            }
            finally { camera.targetTexture = previous; RenderTexture.active = active; RenderTexture.ReleaseTemporary(target); Object.Destroy(image); }
        }
    }
}
