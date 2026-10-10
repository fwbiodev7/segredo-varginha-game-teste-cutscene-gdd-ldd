using Game.Varginha.Experiment;
using NUnit.Framework;

namespace Game.Tests.EditMode
{
    [Category("HudPuzzles")] public class CampaignHudTests
    {
        [SetUp] public void Reset()=>CampaignHud.Reset();
        [Test] public void IdentificationHoldsTwelveSecondsAndFadesBeforeFifteen()
        {
            var hud=new CampaignHudState();hud.Tick(12,true,"Investigar");
            Assert.That(hud.IdentificationAlpha,Is.EqualTo(1));
            hud.Tick(1,true,"Investigar");Assert.That(hud.IdentificationAlpha,Is.EqualTo(.5f).Within(.001f));
            hud.Tick(1,true,"Investigar");Assert.That(hud.IdentificationAlpha,Is.Zero);
            Assert.That(hud.TutorialAlpha,Is.Zero);
        }
        [Test] public void MenusAndObjectiveChangesNeverRestartIdentification()
        {
            var hud=new CampaignHudState();hud.Tick(14,true,"Investigar");
            hud.Tick(100,false,"Outro objetivo","Escritório");
            Assert.That(hud.Elapsed,Is.EqualTo(14));Assert.That(hud.Goal,Is.EqualTo("Investigar"));
            hud.Tick(0,true,"Outro objetivo","Escritório");
            Assert.That(hud.IdentificationAlpha,Is.Zero);Assert.That(hud.GoalAlpha,Is.EqualTo(1));
            Assert.That(hud.RoomAlpha,Is.EqualTo(1));
            hud.Tick(5,true,"Outro objetivo","Escritório");
            Assert.That(hud.GoalAlpha,Is.Zero);Assert.That(hud.RoomAlpha,Is.Zero);
            hud.Tick(0,true,"Outro objetivo","Hall");
            Assert.That(hud.RoomAlpha,Is.EqualTo(1));Assert.That(hud.GoalAlpha,Is.Zero);
        }
        [Test] public void AreasShareTheirChapterTimerAndNewChaptersStartFresh()
        {
            var hud=CampaignHud.For(12);hud.Tick(14,true,"Investigar");
            Assert.That(CampaignHud.For(13),Is.SameAs(hud));
            Assert.That(CampaignHud.For(14).IdentificationAlpha,Is.EqualTo(1));
            CampaignHud.Reset();Assert.That(CampaignHud.For(12).Elapsed,Is.Zero);
        }
        [Test] public void NewMechanicShowsControlsOnceWithoutRestartingTheChapterCard()
        {
            var hud=new CampaignHudState();hud.Tick(14,true,"Investigar");
            hud.BeginTutorial("combate");Assert.That(hud.TutorialAlpha,Is.EqualTo(1));
            Assert.That(hud.IdentificationAlpha,Is.Zero);
            hud.Tick(8,true,"Investigar");hud.BeginTutorial("combate");Assert.That(hud.TutorialAlpha,Is.Zero);
            hud.Tick(100,false,"Investigar");Assert.That(hud.Elapsed,Is.EqualTo(22));
        }
        [Test] public void InvalidFrameTimesDoNotCorruptFade()
        {
            var hud=new CampaignHudState();
            foreach(float delta in new[]{-1,float.NaN,float.PositiveInfinity})hud.Tick(delta,true,"Investigar");
            Assert.That(hud.Elapsed,Is.Zero);Assert.That(hud.IdentificationAlpha,Is.EqualTo(1));
        }
        [Test] public void EveryShippedChapterHasThreeProgressiveHints()
        {
            var story=new CampaignStory();Assert.That(CampaignSequence.Entries.Length,Is.EqualTo(14));
            foreach(int phase in CampaignSequence.Entries)
            {
                string first=CampaignHints.Get(story,phase,0),second=CampaignHints.Get(story,phase,1),solution=CampaignHints.Get(story,phase,2);
                Assert.That(first,Is.Not.Empty,"Phase "+phase);Assert.That(second,Is.Not.EqualTo(first));
                Assert.That(solution,Is.Not.EqualTo(second));
            }
        }
        [TestCase(13)] [TestCase(14)] [TestCase(15)] [TestCase(18)]
        public void IncompleteAnswersExplainTheMissingActionWithoutProvidingTheSolution(int phase)
        {
            string incomplete=CampaignPuzzleDesign.Feedback(phase,-1,null);
            Assert.That(incomplete,Is.Not.EqualTo(CampaignPuzzleDesign.Feedback(phase)));
            Assert.That(CampaignPuzzleDesign.Correct(phase,-1,null),Is.False);
            Assert.That(CampaignPuzzleDesign.Correct(phase,-1,CampaignPuzzleDesign.Solution(phase)),Is.True);
        }
    }
}
