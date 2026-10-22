using System.Reflection;
using Game.UI;
using Game.Varginha.Experiment;
using NUnit.Framework;
using UnityEngine;

namespace Game.Tests.EditMode
{
    public sealed class CampaignMessagePresentationTests
    {
        private int _size;
        [SetUp] public void Preserve() { _size = VarginhaGameSettings.Current.subtitleSize; Time.timeScale=1; }
        [TearDown] public void Restore() { VarginhaGameSettings.Current.subtitleSize=_size; Time.timeScale=1; }
        [Test] public void CombiningAccentsAreNeverRevealedSeparately()
        {
            var text=new TypewriterText();text.Set("a\u0301rvore");text.Advance(.1f,10);
            Assert.That(text.VisibleText,Is.EqualTo("a\u0301"));text.RevealImmediately();Assert.That(text.VisibleText,Is.EqualTo("a\u0301rvore"));
        }
        [Test] public void TypewriterRespectsDeltaAndPunctuation()
        {
            var text=new TypewriterText();text.Set("A.B");text.Advance(.2f,10,true);
            Assert.That(text.VisibleText,Is.EqualTo("A."));text.Advance(.1f,10,true);Assert.That(text.VisibleText,Is.EqualTo("A."));
            text.Advance(.2f,10,true);Assert.That(text.IsComplete,Is.True);
        }
        [Test] public void VoiceIdIsStableAndSpeakerSpecific()
        {
            Assert.That(CampaignMessagePresentation.IdFor("Renan","Olá\r\nEdelzio"),Is.EqualTo(CampaignMessagePresentation.IdFor("Renan","Olá\nEdelzio")));
            Assert.That(CampaignMessagePresentation.IdFor("Renan","Olá"),Is.Not.EqualTo(CampaignMessagePresentation.IdFor("Ouzana","Olá")));
        }
        [TestCase(0)][TestCase(1)][TestCase(2)] public void SpeechPagesFitTwoLinesWithoutDroppingWords(int size)
        {
            VarginhaGameSettings.Current.subtitleSize=size;
            string original="Edelzio, encontrei uma fotografia antiga ligada ao caso de 1996. Pode abrir meu notebook na mesa. Observe o portão: há algo estranho nela.";
            var message=new CampaignMessagePresentation();message.Show(null,"Renan",original);
            typeof(CampaignMessagePresentation).GetField("_openedFrame",BindingFlags.Instance|BindingFlags.NonPublic).SetValue(message,-1);
            var style=CampaignMessagePresentation.TextStyle(message.FontSize);float two=style.CalcHeight(new GUIContent("Ag"),message.TextWidth)*2+2;
            string joined="";int safety=0;
            do
            {
                Assert.That(style.CalcHeight(new GUIContent(message.PageText),message.TextWidth),Is.LessThanOrEqualTo(two));
                joined+=(joined.Length==0?"":" ")+message.PageText;
                message.Tick(100,false);
                if(message.Advance())break;
            }while(++safety<20);
            Assert.That(joined,Is.EqualTo(original));Assert.That(safety,Is.LessThan(20));
        }
        [Test] public void FirstAdvanceRevealsAndLaterAdvanceMovesPage()
        {
            var message=new CampaignMessagePresentation();message.Show(null,"Edelzio","A chave estava aqui.");
            typeof(CampaignMessagePresentation).GetField("_openedFrame",BindingFlags.Instance|BindingFlags.NonPublic).SetValue(message,-1);
            Assert.That(message.Advance(),Is.False);Assert.That(message.Revealed,Is.True);Assert.That(message.Advance(),Is.True);
        }
        [Test] public void PausedSpeechAndMandatoryReadingRemainAccessible()
        {
            var message=new CampaignMessagePresentation();message.Show(null,"Renan","Compare as duas cópias.");message.Tick(.1f,false);
            string text=message.VisibleText;message.Tick(9,true);Assert.That(message.VisibleText,Is.EqualTo(text));
            Time.timeScale=0;message.Tick(9,false);Assert.That(message.VisibleText,Is.EqualTo(text));Time.timeScale=1;
            message.Show(null,"REGISTRO DE 1996","Uma pista importante para comparar depois.",CampaignMessageKind.Reading);
            message.Tick(100,false);Assert.That(message.Open,Is.True);Assert.That(message.VisibleText,Is.EqualTo(message.PageText));Assert.That(message.FinishedByVoice,Is.False);
        }
        [Test] public void PickupQueueExpiresWithoutInputAndFreezesOnPause()
        {
            var go=new GameObject("Notice_test");var notice=go.AddComponent<CampaignPickupNotice>();
            try
            {
                notice.Enqueue("Chave coletada.");notice.Enqueue("Caderno coletado.");float time=notice.Remaining;
                notice.Tick(100,true);Assert.That(notice.Remaining,Is.EqualTo(time));
                notice.Tick(100,false);Assert.That(notice.CurrentText,Is.EqualTo("Caderno coletado."));
                notice.Tick(100,false);Assert.That(notice.CurrentText,Is.Null);Assert.That(notice.PendingCount,Is.Zero);
            }
            finally { Object.DestroyImmediate(go); }
        }
    }
}
