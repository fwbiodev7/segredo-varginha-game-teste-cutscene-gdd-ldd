using Game.Varginha.Experiment;
using NUnit.Framework;
using UnityEngine;

namespace Game.Tests.EditMode
{
    public class VarginhaExperimentTests
    {
        private ExperimentDefinition _data;
        [SetUp] public void Setup() => _data = ExperimentDefinition.Load();
        [Test] public void WrongMapDoesNotUnlockHistoricalDocuments()
        {
            var state = new ExperimentProgress(); state.Repair(_data);
            Assert.That(state.Submit(_data, 0), Is.False);
            Assert.That(state.CanOpen(1), Is.False);
        }
        [Test] public void CorrectCodeCannotBypassRequiredEvidence()
        {
            var state = new ExperimentProgress { codeInput = "2323" }; state.Repair(_data);
            Assert.That(state.Submit(_data, 2), Is.False);
            Assert.That(state.codeSolved, Is.False);
        }
        [Test] public void CompleteInvestigationUnlocksOnlyItsNextStep()
        {
            var state = new ExperimentProgress(); state.Repair(_data);
            state.mapOrder = (int[])_data.puzzles[0].solution.Clone();
            Assert.That(state.Submit(_data, 0), Is.True);
            Assert.That(state.CanOpen(1), Is.True);
            Assert.That(state.CanOpen(2), Is.False);
            state.documentOrder = (int[])_data.puzzles[1].solution.Clone();
            Assert.That(state.Submit(_data, 1), Is.True);
            state.codeInput = "2323";
            Assert.That(state.Submit(_data, 2), Is.True);
        }
        [Test] public void CorruptSaveRepairsPiecesAndImpossibleFlags()
        {
            var state = new ExperimentProgress { mapOrder = new[] { 1, 1, 1, 1 }, documentOrder = new[] { -1 },
                codeInput = "NaN", codeSolved = true, documentsSolved = true };
            state.Repair(_data);
            Assert.That(ExperimentProgress.IsPermutation(state.mapOrder, 4), Is.True);
            Assert.That(ExperimentProgress.IsPermutation(state.documentOrder, 4), Is.True);
            Assert.That(state.codeInput, Is.Empty);
            Assert.That(state.documentsSolved || state.codeSolved, Is.False);
        }
        [Test] public void ReloadRetainsPartiallyArrangedPiecesAndSeenOpening()
        {
            var state = new ExperimentProgress { openingSeen = true, mapOrder = new[] { 0, 3, 1, 2 },
                documentOrder = new[] { 2, 1, 0, 3 }, codeInput = "23" };
            var loaded = JsonUtility.FromJson<ExperimentProgress>(JsonUtility.ToJson(state)); loaded.Repair(_data);
            Assert.That(loaded.mapOrder, Is.EqualTo(state.mapOrder));
            Assert.That(loaded.documentOrder, Is.EqualTo(state.documentOrder));
            Assert.That(loaded.openingSeen, Is.True);
            Assert.That(loaded.codeInput, Is.EqualTo("23"));
        }
        [Test] public void SkipAndNaturalCompletionHaveIdenticalFinalState()
        {
            var natural = new ExperimentTimeline(_data.shots); natural.Tick(1000);
            var naturalState = new ExperimentProgress(); natural.Finish(naturalState);
            var skipped = new ExperimentTimeline(_data.shots);
            var skippedState = new ExperimentProgress(); skipped.Finish(skippedState);
            Assert.That(JsonUtility.ToJson(naturalState), Is.EqualTo(JsonUtility.ToJson(skippedState)));
            Assert.That(skipped.Time, Is.EqualTo(natural.Time));
        }
        [Test] public void PausingDoesNotAdvanceOrCompleteOpening()
        {
            var timeline = new ExperimentTimeline(_data.shots) { Paused = true };
            timeline.Tick(1000);
            Assert.That(timeline.Time, Is.EqualTo(0));
            Assert.That(timeline.Finished, Is.False);
        }
        [Test] public void InvalidSettingsClampToSupportedValues()
        {
            var settings = new GameSettingsData { master = 2, music = -1, resolution = 99,
                frameRate = 19, display = -1, subtitleSize = 99 };
            settings.Validate();
            Assert.That(settings.master, Is.EqualTo(1));
            Assert.That(settings.music, Is.Zero);
            Assert.That(settings.resolution, Is.EqualTo(3));
            Assert.That(settings.display, Is.Zero);
            Assert.That(settings.subtitleSize, Is.EqualTo(2));
            Assert.That(settings.frameRate, Is.EqualTo(60));
        }
        [Test] public void GeneratedAssetsAndReferenceIdentitiesAreAvailable()
        {
            foreach (var name in new[] { "OpeningStoryboard", "GameCast", "IndustrialFacade" })
                Assert.That(VarginhaExperimentArt.Load(name), Is.Not.Null, name);
            Assert.That(_data.characters[1].name, Is.EqualTo("Renan"));
            Assert.That(_data.characters[3].description, Does.Contain("Mulher negra"));
            Assert.That(_data.shots[_data.shots.Length - 1].mode, Is.EqualTo("school"));
        }
    }
}
