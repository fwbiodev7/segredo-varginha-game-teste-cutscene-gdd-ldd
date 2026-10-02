using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.Varginha.Experiment
{
    /// <summary>Adds reference-based NPC presentation without replacing any shipped map.</summary>
    public sealed class VarginhaExperimentalCharacters : MonoBehaviour
    {
        private EdelzioTopDownController _player;
        private Transform _renan;
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Register()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.name != VarginhaTravelCinematic.SchoolScene && scene.name != VarginhaTravelCinematic.ChurchScene) return;
            var root = new GameObject("Referencias_Experimentais");
            root.AddComponent<VarginhaExperimentalCharacters>();
        }
        private IEnumerator Start()
        {
            yield return null; // Existing runtime factories finish assembling their own maps first.
            _player = FindAnyObjectByType<EdelzioTopDownController>();
            if (SceneManager.GetActiveScene().name == VarginhaTravelCinematic.ChurchScene)
            {
                var padre = GameObject.Find("Padre_Fabio");
                if (padre != null)
                {
                    var sprite = VarginhaExperimentArt.Body(2);
                    var renderer = padre.GetComponent<SpriteRenderer>();
                    if (sprite != null && renderer != null) renderer.sprite = sprite;
                }
                yield break;
            }
            _renan = new GameObject("Renan_Industrial_Referencia").transform;
            _renan.SetParent(transform);
            _renan.position = new Vector3(2.8f, -7.8f);
            var rendererRenan = _renan.gameObject.AddComponent<SpriteRenderer>();
            rendererRenan.sprite = VarginhaExperimentArt.Body(1);
            rendererRenan.sortingOrder = 6;
            var collider = _renan.gameObject.AddComponent<CircleCollider2D>();
            collider.radius = .3f; collider.offset = new Vector2(0, .18f);
        }
        private void Update()
        {
            if (_renan == null || _player == null || _player.IsInputLocked || VarginhaTravelCinematic.IsTravelling
                || VarginhaGameHUD.Instance?.IsDialogueOpen == true) return;
            if (Vector2.Distance(_player.transform.position, _renan.position) < 2.2f
                && VarginhaInputBindings.WasPressedThisFrame(VarginhaInputAction.Interact))
                VarginhaGameHUD.Instance?.ShowDialogue("Renan",
                    "Estou aqui na Industrial, Edelzio. Os arquivos mudaram, mas os documentos físicos permitem conferir a verdade. No laboratório podemos testar o mapa, a cronologia e o código da fotografia.");
        }
    }
}
