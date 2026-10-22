using System.Globalization;
using UnityEngine;

namespace Game.UI
{
    /// <summary>Revela texto gradualmente para diálogos e instruções, independente do Time.timeScale.</summary>
    public sealed class TypewriterText
    {
        public string FullText { get; private set; } = string.Empty;
        public string VisibleText => IsComplete ? FullText : FullText.Substring(0, _elements[_visibleCharacters]);
        public bool IsComplete => _visibleCharacters >= _elements.Length;

        private int _visibleCharacters;
        private float _progress;
        private int[] _elements = System.Array.Empty<int>();
        private int _lastFrame = -1;
        public bool HasAdvanced { get; private set; }

        public void Set(string text)
        {
            FullText = text ?? string.Empty;
            _elements = StringInfo.ParseCombiningCharacters(FullText);
            _visibleCharacters = 0;
            _progress = 0f;
            HasAdvanced = false;
            _lastFrame = -1;
        }

        public void Tick(float charactersPerSecond)
        {
            // IMGUI invokes Layout/Repaint/input events in the same frame.
            if (_lastFrame == Time.frameCount) return;
            _lastFrame = Time.frameCount;
            Advance(Time.unscaledDeltaTime, charactersPerSecond);
        }

        public void Advance(float deltaTime, float charactersPerSecond, bool punctuationPauses = false)
        {
            HasAdvanced = false;
            if (IsComplete || deltaTime <= 0 || charactersPerSecond <= 0) return;
            int prev = _visibleCharacters;
            _progress += deltaTime;
            while (!IsComplete)
            {
                float delay = 1f / charactersPerSecond;
                if (punctuationPauses && _visibleCharacters > 0)
                {
                    char previous = FullText[_elements[_visibleCharacters - 1]];
                    if (".!?…".IndexOf(previous) >= 0) delay += .14f;
                    else if (",;:".IndexOf(previous) >= 0) delay += .07f;
                }
                if (_progress < delay) break;
                _progress -= delay;
                _visibleCharacters++;
            }
            HasAdvanced = _visibleCharacters > prev;
        }

        public void RevealImmediately()
        {
            _visibleCharacters = _elements.Length;
        }
    }
}
