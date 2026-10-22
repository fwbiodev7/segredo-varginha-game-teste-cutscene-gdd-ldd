using System;
using System.Collections.Generic;
using System.Text;
using Game.UI;
using UnityEngine;

namespace Game.Varginha.Experiment
{
    public enum CampaignMessageKind { Speech, Reading }

    /// <summary>One clock per Update, shared by campaign and legacy HUD. Text and progression stay separate.</summary>
    public sealed class CampaignMessagePresentation
    {
        private readonly TypewriterText _reveal = new();
        private readonly List<string> _pages = new();
        private CampaignDialogueVoice _voice;
        private float _voiceElapsed, _voiceDuration;
        private int _openedFrame;
        private int _settingSize;
        public CampaignMessageKind Kind { get; private set; }
        public string Speaker { get; private set; }
        public string FullText { get; private set; }
        public string LineId { get; private set; }
        public int PageIndex { get; private set; }
        public int PageCount => _pages.Count;
        public string PageText => _pages.Count == 0 ? "" : _pages[PageIndex];
        public string VisibleText => Kind == CampaignMessageKind.Reading ? PageText : _reveal.VisibleText;
        public string RenderText => VisibleText + (Revealed ? "" : "<color=#00000000>" + PageText.Substring(VisibleText.Length) + "</color>");
        public bool Revealed => Kind == CampaignMessageKind.Reading || _reveal.IsComplete;
        public bool HasVoice => _voiceDuration > 0;
        public bool FinishedByVoice { get; private set; }
        public bool Open { get; private set; }
        public int FontSize { get; private set; }
        public float TextWidth => 780;
        public bool CanAcceptInput => Time.frameCount > _openedFrame;

        // Stable across code movement, scene changes, and UI size. A text edit intentionally changes its recording ID.
        public static string IdFor(string speaker, string text)
        {
            ulong hash = 14695981039346656037UL;
            foreach (byte value in Encoding.UTF8.GetBytes((speaker ?? "") + "\n" + (text ?? "").Replace("\r\n", "\n")))
                hash = unchecked((hash ^ value) * 1099511628211UL);
            return "fala_" + hash.ToString("x16");
        }

        public static GUIStyle TextStyle(int size)
        {
            var style = new GUIStyle { fontSize = size, wordWrap = true, richText = false };
            PixelUIFont.Apply(style); style.normal.textColor = ExperimentGUI.Paper;
            return style;
        }

        public void Show(MonoBehaviour owner, string speaker, string text, CampaignMessageKind kind = CampaignMessageKind.Speech, AudioClip voiceOverride = null)
        {
            Close(); Kind = kind; Speaker = speaker ?? ""; FullText = VarginhaInputActions.Prompt(text ?? "");
            LineId = IdFor(Speaker, text); Open = true; FinishedByVoice = false; _openedFrame = Time.frameCount;
            int size = VarginhaGameSettings.Current.subtitleSize;
            _settingSize = size;
            FontSize = kind == CampaignMessageKind.Reading ? 14 : size == 0 ? 12 : size == 2 ? 17 : 14;
            Paginate(FullText, kind == CampaignMessageKind.Speech ? 2 : 7);
            _reveal.Set(PageText);
            if (kind != CampaignMessageKind.Speech || owner == null) return;
            var clip = voiceOverride != null ? voiceOverride : Resources.Load<AudioClip>("Varginha/Audio/Voices/" + LineId);
            if (clip == null) return;
            _voice = owner.GetComponent<CampaignDialogueVoice>() ?? owner.gameObject.AddComponent<CampaignDialogueVoice>();
            _voice.Play(clip); _voiceDuration = clip.length;
        }

        private void Paginate(string text, int lines)
        {
            _pages.Clear(); PageIndex = 0;
            var style = TextStyle(FontSize);
            float height = style.CalcHeight(new GUIContent("Ag"), TextWidth) * lines + 2;
            string page = "";
            // Keep paragraph breaks and punctuation; only page boundaries replace a separator with an advance.
            foreach (string word in System.Text.RegularExpressions.Regex.Split(text, @"(?<=\s)"))
            {
                string candidate = page + word;
                if (page.Length > 0 && style.CalcHeight(new GUIContent(candidate.TrimEnd()), TextWidth) > height)
                { _pages.Add(page.TrimEnd()); page = word.TrimStart(); }
                else page = candidate;
            }
            if (page.Length > 0 || _pages.Count == 0) _pages.Add(page.TrimEnd());
        }

        public void Tick(float deltaTime, bool paused)
        {
            if (!Open) return;
            if (Kind == CampaignMessageKind.Speech && _settingSize != VarginhaGameSettings.Current.subtitleSize)
            {
                int consumed = VisibleText.Length; for (int i = 0; i < PageIndex; i++) consumed += _pages[i].Length;
                _settingSize = VarginhaGameSettings.Current.subtitleSize;
                FontSize = _settingSize == 0 ? 12 : _settingSize == 2 ? 17 : 14;
                Paginate(FullText,2);
                while (PageIndex + 1 < PageCount && consumed >= PageText.Length) { consumed -= PageText.Length; PageIndex++; }
                _reveal.Set(PageText); _reveal.Advance(consumed / 38f,38);
            }
            paused |= Time.timeScale <= 0;
            _voice?.Pause(paused);
            if (paused) return;
            if (HasVoice)
            {
                _voiceElapsed += Mathf.Max(0, deltaTime);
                float segmentEnd = VoiceBoundary(PageIndex + 1);
                if (_voiceElapsed >= segmentEnd)
                {
                    _reveal.RevealImmediately();
                    if (PageIndex + 1 < PageCount) NextPage();
                    else if (_voiceElapsed >= _voiceDuration + .45f) FinishedByVoice = true;
                }
            }
            float rate = HasVoice ? Mathf.Max(24, FullText.Length / Mathf.Max(.1f, _voiceDuration) * 1.15f) : 38;
            _reveal.Advance(deltaTime, rate, !HasVoice);
        }

        private float VoiceBoundary(int count)
        {
            int chars = 0; foreach (string page in _pages) chars += page.Length;
            int before = 0; for (int i = 0; i < count; i++) before += _pages[i].Length;
            return _voiceDuration * before / Mathf.Max(1, chars);
        }

        // Returns true only after the final fully revealed page. The owner retains its mission callbacks.
        public bool Advance()
        {
            if (!Open || !CanAcceptInput) return false;
            if (!Revealed) { _reveal.RevealImmediately(); return false; }
            if (PageIndex + 1 >= PageCount) return true;
            NextPage();
            if (HasVoice)
            {
                _voiceElapsed = VoiceBoundary(PageIndex);
                _voice.Seek(_voiceElapsed);
            }
            return false;
        }
        private void NextPage() { PageIndex++; _reveal.Set(PageText); }
        public void SeekCaption(float elapsed, float duration)
        {
            float total = 0; foreach (string page in _pages) total += page.Length;
            float rate = duration > 0 ? Mathf.Max(38, total / Mathf.Max(.2f, duration - .6f)) : 38;
            float characters = Mathf.Max(0, elapsed) * rate;
            PageIndex = 0;
            while (PageIndex + 1 < PageCount && characters > _pages[PageIndex].Length + rate * .5f)
            { characters -= _pages[PageIndex].Length + rate * .5f; PageIndex++; }
            _reveal.Set(PageText); _reveal.Advance(characters / rate, rate);
        }
        public void Close()
        {
            _voice?.Stop(); _voice = null; _voiceDuration = _voiceElapsed = 0;
            Open = false; FinishedByVoice = false;
        }
    }

    internal static class CampaignMessageBox
    {
        public static Rect Bounds(CampaignMessagePresentation message)
        {
            var style = CampaignMessagePresentation.TextStyle(message.FontSize);
            float body = style.CalcHeight(new GUIContent(message.PageText), message.TextWidth);
            float height = Mathf.Clamp(body + 52, 76, message.Kind == CampaignMessageKind.Speech ? 106 : 240);
            return new Rect(230, 686 - height, 820, height);
        }
        public static bool Draw(CampaignMessagePresentation message, bool car = false)
        {
            if (!message.Open) return false;
            bool speech = message.Kind == CampaignMessageKind.Speech;
            Rect area = Bounds(message);
            if (!speech || VarginhaGameSettings.Current.subtitles || !message.HasVoice)
            {
                if (speech)
                {
                    ExperimentGUI.Box(area, new Color(.025f, .045f, .065f, .84f));
                    ExperimentGUI.Box(new Rect(area.x, area.y, 2, area.height), new Color(.43f, .77f, .67f, .7f));
                }
                else ExperimentGUI.Panel(area);
                var name = CampaignMessagePresentation.TextStyle(10); name.normal.textColor = ExperimentGUI.Muted;
                GUI.Label(new Rect(area.x + 20, area.y + 7, 650, 16), message.Speaker, name);
                // Reserve the whole page so glyph reveal never changes wrapping or panel height.
                var style = CampaignMessagePresentation.TextStyle(message.FontSize);
                style.richText = true;
                GUI.Label(new Rect(area.x + 20, area.y + 26, message.TextWidth, area.height - 50), message.RenderText, style);
            }
            string action = !message.Revealed ? "MOSTRAR TEXTO" : message.PageIndex + 1 < message.PageCount ? "PRÓXIMO" : "CONTINUAR";
            var button = new Rect(area.xMax - 230, area.yMax - 25, 218, 22);
            if (message.PageCount > 1)
                ExperimentGUI.Label(new Rect(area.x + 20, area.yMax - 24, 120, 20), (message.PageIndex + 1) + "/" + message.PageCount, small: true);
            return car ? VarginhaGamepadUI.CarButton(button, action) : ExperimentGUI.Button(button, action);
        }
    }
}
