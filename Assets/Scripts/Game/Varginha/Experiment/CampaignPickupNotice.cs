using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.Varginha.Experiment
{
    /// <summary>Small original pixel panels. Pickups never open a modal or require confirmation.</summary>
    public sealed class CampaignPickupNotice : MonoBehaviour
    {
        private static CampaignPickupNotice _active;
        private readonly Queue<string> _queue = new();
        public string CurrentText { get; private set; }
        public float Remaining { get; private set; }
        public int PendingCount => _queue.Count;
        public static void Show(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return;
            if (_active == null) _active = new GameObject("Avisos_de_coleta").AddComponent<CampaignPickupNotice>();
            _active.Enqueue(VarginhaInputActions.Prompt(text));
        }
        public void Enqueue(string text)
        {
            if (text == CurrentText || string.IsNullOrWhiteSpace(text)) return;
            _queue.Enqueue(text); if (CurrentText == null) Next();
        }
        private void Next()
        {
            CurrentText = _queue.Count > 0 ? _queue.Dequeue() : null;
            Remaining = CurrentText == null ? 0 : Mathf.Clamp(2.5f + CurrentText.Length * .035f, 3, 7);
        }
        public void Tick(float deltaTime, bool paused)
        {
            if (paused || CurrentText == null) return;
            Remaining -= Mathf.Max(0, deltaTime); if (Remaining <= 0) Next();
        }
        private bool Hidden => Time.timeScale <= 0 || CampaignCinematics.IsTransitioning || VarginhaMainMenu.IsOpen || VarginhaCampaignStage.IsModalOpen || VarginhaCampaignPhase1.IsModalOpen || CampaignExpansionController.Active?.IsModalOpen == true || CampaignContinuationController.Active?.Modal == true || VarginhaGameHUD.Instance?.IsDialogueOpen == true || VarginhaGameHUD.Instance?.IsInventoryOpen == true || VarginhaGameHUD.Instance?.IsGameOver == true;
        private void Update() => Tick(Time.unscaledDeltaTime, Hidden);
        public Rect Bounds
        {
            get
            {
                var style = CampaignMessagePresentation.TextStyle(10);
                float height = style.CalcHeight(new GUIContent(CurrentText ?? ""), 310) + 20;
                return new Rect(912, 146, 340, Mathf.Max(44, height));
            }
        }
        private void OnGUI()
        {
            if (CurrentText == null || Hidden) return;
            var before = ExperimentGUI.BeginCanvas(); ExperimentGUI.Init();
            var color = GUI.color; GUI.color = new Color(color.r, color.g, color.b, color.a * Mathf.Clamp01(Remaining / .35f));
            Rect area = Bounds; ExperimentGUI.Panel(area);
            GUI.Label(new Rect(area.x + 15, area.y + 10, 310, area.height - 20), CurrentText, CampaignMessagePresentation.TextStyle(10));
            GUI.color = color; GUI.matrix = before;
        }
        private void OnDestroy() { if (_active == this) _active = null; }
    }
}
