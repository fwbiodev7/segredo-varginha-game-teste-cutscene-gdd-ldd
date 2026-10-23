using Game.Varginha.Experiment;
using NUnit.Framework;
using UnityEngine;

namespace Game.Tests.EditMode
{
    public sealed class CampaignBookPuzzleTests
    {
        [Test] public void AGuessedCombinationCannotBypassDocumentAndEvidence()
        {
            var s=new CampaignExpansionState();Assert.That(s.SolveAnchor(1996,1,23),Is.False);
            Assert.That(CampaignBookPuzzle.Link(s,0,0),Is.False);Assert.That(CampaignBookPuzzle.Submit(s),Is.False);
            s.anchorClues=7;Assert.That(CampaignBookPuzzle.Submit(s),Is.False);
            s.bookOrder=new[]{0,1,2};Assert.That(CampaignBookPuzzle.Submit(s),Is.False);
            for(int i=0;i<3;i++)CampaignBookPuzzle.Link(s,i,i);
            Assert.That(CampaignBookPuzzle.Submit(s),Is.True);Assert.That(s.anchorFound,Is.True);
        }
        [Test] public void WrongAssociationsSurviveCheckingAndCanBeCorrectedIndividually()
        {
            var s=new CampaignExpansionState{anchorClues=7,bookOrder=new[]{0,1,2},bookLinks=new[]{0,2,1}};
            Assert.That(CampaignBookPuzzle.Submit(s),Is.False);CollectionAssert.AreEqual(new[]{0,2,1},s.bookLinks);
            CampaignBookPuzzle.Link(s,1,1);CampaignBookPuzzle.Link(s,2,2);Assert.That(CampaignBookPuzzle.Submit(s),Is.True);
        }
        [Test] public void ReferencesMoveWithFragmentsAndPartialWorkSurvivesReload()
        {
            var s=new CampaignExpansionState{anchorClues=1};CampaignBookPuzzle.Link(s,1,0);
            CampaignBookPuzzle.Swap(s,0,1);Assert.That(s.bookLinks[0],Is.EqualTo(0));
            var restored=JsonUtility.FromJson<CampaignExpansionState>(JsonUtility.ToJson(s));restored.Repair();
            CollectionAssert.AreEqual(s.bookOrder,restored.bookOrder);CollectionAssert.AreEqual(s.bookLinks,restored.bookLinks);
            Assert.That(restored.anchorFound,Is.False);
        }
        [Test] public void OldSolvedRecordsKeepTheCompartmentAndLaterRewards()
        {
            var s=new CampaignExpansionState{anchorFound=true,anchorClues=7,evidencePresented=true,reagentUnlocked=true};s.Repair();
            Assert.That(CampaignBookPuzzle.Ready(s),Is.True);Assert.That(s.reagentUnlocked,Is.True);
            Assert.That(CampaignBookPuzzle.Swap(s,0,1),Is.False);Assert.That(CampaignBookPuzzle.Link(s,0,2),Is.False);
        }
        [Test] public void MalformedAndUnacquiredReferencesCannotGrantTheReward()
        {
            var s=new CampaignExpansionState{anchorClues=1,bookOrder=new[]{9,9,9},bookLinks=new[]{0,1,99},bookHints=99};s.Repair();
            Assert.That(s.bookHints,Is.EqualTo(3));CollectionAssert.AreEqual(new[]{0,-1,-1},s.bookLinks);
            Assert.That(CampaignBookPuzzle.Submit(s),Is.False);Assert.That(s.anchorFound,Is.False);
        }
    }
}
