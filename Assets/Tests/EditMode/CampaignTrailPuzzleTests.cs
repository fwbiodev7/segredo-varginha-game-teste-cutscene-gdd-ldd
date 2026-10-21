using Game.Varginha.Experiment;
using NUnit.Framework;
using UnityEngine;

namespace Game.Tests.EditMode
{
    public class CampaignTrailPuzzleTests
    {
        [Test] public void ExactlyOneOfTheEightRoutesMatchesAllPhysicalEvidence()
        {
            int solutions=0;
            for(int route=0;route<8;route++)
            {
                var s=new CampaignExpansionState{visited=7,trailChoices=new[]{route&1,(route>>1)&1,(route>>2)&1}};
                if(s.SolveMap())solutions++;
            }
            Assert.That(solutions,Is.EqualTo(1));
        }
        [Test] public void LegacyPermutationAndDirectionsCannotSolveTheReplacementPuzzle()
        {
            var s=new CampaignExpansionState{visited=7,mapOrder=new[]{0,1,2},mapDirections=new[]{1,1,0}};
            Assert.That(s.SolveMap(),Is.False);Assert.That(s.mapSolved,Is.False);
            s.trailChoices=new[]{1,0,1};Assert.That(s.SolveMap(),Is.True);
        }
        [Test] public void MissingEvidenceDoesNotEraseACompletedTraceOrUnlockTheForest()
        {
            var s=new CampaignExpansionState{visited=3,trailChoices=new[]{1,0,1}};
            Assert.That(s.SolveMap(),Is.False);CollectionAssert.AreEqual(new[]{1,0,1},s.trailChoices);
            Assert.That(s.trailFeedback,Is.EqualTo(4));s.visited=7;Assert.That(s.SolveMap(),Is.True);
        }
        [Test] public void WrongCrossingRetainsTheEarlierValidPassage()
        {
            var s=new CampaignExpansionState{visited=7,trailChoices=new[]{1,1,1}};
            Assert.That(s.SolveMap(),Is.False);CollectionAssert.AreEqual(new[]{1,-1,-1},s.trailChoices);
            Assert.That(s.trailFeedback,Is.EqualTo(2));Assert.That(CampaignTrailPuzzle.Feedback(s),Does.Contain("ponte"));
        }
        [Test] public void TraceUndoRestartAndPartialSaveRemainPredictable()
        {
            var s=new CampaignExpansionState{visited=7};Assert.That(CampaignTrailPuzzle.Commit(s,-1),Is.False);
            CampaignTrailPuzzle.Commit(s,1);CampaignTrailPuzzle.Commit(s,0);CampaignTrailPuzzle.Undo(s);
            CollectionAssert.AreEqual(new[]{1,-1,-1},s.trailChoices);
            var restored=JsonUtility.FromJson<CampaignExpansionState>(JsonUtility.ToJson(s));restored.Repair();
            CollectionAssert.AreEqual(s.trailChoices,restored.trailChoices);Assert.That(restored.mapSolved,Is.False);
            CampaignTrailPuzzle.Reset(restored);Assert.That(CampaignTrailPuzzle.Step(restored),Is.Zero);
        }
        [Test] public void CompletedOldSaveRetainsForestAndSubsequentProgress()
        {
            var s=JsonUtility.FromJson<CampaignExpansionState>("{\"visited\":7,\"mapSolved\":true,\"forestSigns\":3,\"fabioMet\":true}");s.Repair();
            CollectionAssert.AreEqual(new[]{1,0,1},s.trailChoices);Assert.That(s.fabioMet,Is.True);
            CampaignTrailPuzzle.Reset(s);Assert.That(CampaignTrailPuzzle.Step(s),Is.EqualTo(3));
        }
        [Test] public void MalformedOrGappedTraceCannotGrantProgress()
        {
            var s=new CampaignExpansionState{visited=7,trailChoices=new[]{1,-9,1},trailHints=99};s.Repair();
            CollectionAssert.AreEqual(new[]{1,-1,-1},s.trailChoices);Assert.That(s.trailHints,Is.EqualTo(3));
            Assert.That(s.SolveMap(),Is.False);Assert.That(s.mapSolved,Is.False);
        }
        [Test] public void ProgressiveHintsBeginWithObservationRatherThanTheAnswer()
        {
            var s=new CampaignExpansionState{visited=7};
            Assert.That(CampaignTrailPuzzle.Hint(s,0),Does.Not.Contain("trilha B"));
            Assert.That(CampaignTrailPuzzle.Hint(s,1),Does.Contain("tronco"));
            Assert.That(CampaignTrailPuzzle.Hint(s,2),Does.Contain("trilha B"));
        }
    }
}
