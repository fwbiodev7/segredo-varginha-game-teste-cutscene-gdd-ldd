using Game.Varginha.Experiment;
using NUnit.Framework;
using UnityEngine;

namespace Game.Tests.EditMode
{
    public class CampaignStoryTests
    {
        [Test] public void CorrectAnswersCannotBypassInvestigatedEvidence()
        {
            var state = new CampaignStory { pages = new[] { 0, 1, 2 }, symbols = new[] { 0, 1, 0, 1 }, circle = 2, triangle = 3 };
            Assert.That(state.SubmitPages(), Is.False);
            Assert.That(state.SubmitBuilding(1), Is.False);
            Assert.That(state.SubmitCode(), Is.False);
            Assert.That(state.MapFragments, Is.Zero);
        }
        [Test] public void InvestigationSurvivesReloadAndRetainsBothMapFragments()
        {
            var state = new CampaignStory { phase = 5, routine = 15, boxFound = true, pages = new[] { 0, 1, 2 }, renanMet = true, lesson = true, archive = true };
            Assert.That(state.SubmitPages(), Is.True);
            Assert.That(state.SubmitBuilding(0), Is.False);
            Assert.That(state.SubmitBuilding(1), Is.True);
            state.symbolsFound = state.legendFound = true; state.symbols = new[] { 0, 1, 0, 1 }; state.circle = 2; state.triangle = 3;
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
            Assert.That(state.phase, Is.EqualTo(10));
            Assert.That(state.MapFragments, Is.Zero);
            Assert.That(state.codeSolved || state.renanConfirmed, Is.False);
            Assert.That(state.positionPhase, Is.Zero);
            Assert.That(state.driveDistance, Is.Zero);
            Assert.That(state.pages, Is.EqualTo(new[] { 2, 0, 1 }));
        }
    }
}
