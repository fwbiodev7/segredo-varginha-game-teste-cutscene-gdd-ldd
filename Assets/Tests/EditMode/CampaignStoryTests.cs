using Game.Varginha.Experiment;
using NUnit.Framework;
using UnityEngine;

namespace Game.Tests.EditMode
{
    [Category("HudPuzzles")] public class CampaignStoryTests
    {
        [Test] public void CorrectAnswersCannotBypassInvestigatedEvidence()
        {
            var state = new CampaignStory { pages = new[] { 0, 1, 2 }, symbols = new[] { 0, 1, 0, 1 }, circle = 2, triangle = 3 };
            Assert.That(state.SubmitPages(), Is.False);
            Assert.That(state.SubmitPhotoClue(1), Is.False);
            Assert.That(state.SubmitCode(), Is.False);
            Assert.That(state.MapFragments, Is.Zero);
        }
        [Test] public void InvestigationSurvivesReloadAndRetainsBothMapFragments()
        {
            var state = new CampaignStory { phase = 5, routine = 15, boxFound = true, pages = new[] { 0, 1, 2 }, renanMet = true, photoOpened = true };
            Assert.That(state.SubmitPages(), Is.True);
            Assert.That(state.SubmitPhotoClue(0), Is.False);
            Assert.That(state.SubmitPhotoClue(1), Is.True);
            state.timeOpened = true; state.schoolTimeChoice = 1;
            Assert.That(state.SubmitCode(), Is.True);
            state.renanConfirmed = true;
            var restored = JsonUtility.FromJson<CampaignStory>(JsonUtility.ToJson(state)); restored.Repair();
            Assert.That(restored.phase, Is.EqualTo(5));
            Assert.That(restored.MapFragments, Is.EqualTo(2));
            Assert.That(restored.renanConfirmed, Is.True);
        }
        [Test] public void CorruptProgressCannotUnlockImpossibleClues()
        {
            var state = new CampaignStory { phase = 900, pagesSolved = true, buildingSolved = true, codeSolved = true, renanConfirmed = true, pages = new[] { 2, 2, 2 }, driveDistance = float.NaN, x = float.PositiveInfinity, positionPhase = 2 };
            state.Repair();
            Assert.That(state.phase, Is.EqualTo(21));
            Assert.That(state.MapFragments, Is.Zero);
            Assert.That(state.codeSolved || state.renanConfirmed, Is.False);
            Assert.That(state.positionPhase, Is.Zero);
            Assert.That(state.driveDistance, Is.Zero);
            Assert.That(state.pages, Is.EqualTo(new[] { 2, 0, 1 }));
        }
        [Test] public void SchoolUsesOnlyPhotoThenOneTimePuzzleAndKeepsLegacyCompletions()
        {
            var state=new CampaignStory {renanMet=true};
            Assert.That(state.SubmitPhotoClue(1),Is.False,"The notebook must actually be opened.");
            state.photoOpened=true;Assert.That(state.SubmitPhotoClue(1),Is.True);
            Assert.That(state.MapFragments,Is.Zero,"The new fragment is revealed by the time puzzle.");
            state.schoolTimeChoice=1;Assert.That(state.SubmitCode(),Is.False);
            state.timeOpened=true;Assert.That(state.SubmitCode(),Is.True);
            var legacy=new CampaignStory {phase=6,renanMet=true,buildingSolved=true,codeSolved=true,renanConfirmed=true};
            legacy.Repair();Assert.That(legacy.codeSolved&&legacy.renanConfirmed,Is.True);
            Assert.That(legacy.MapFragments,Is.EqualTo(1));
            foreach(var point in CampaignMapPlan.Create(4).points)
                Assert.That(point.id,Is.EqualTo("renan").Or.EqualTo("notebook"),"Only existing teacher targets are required.");
        }
    }
}
