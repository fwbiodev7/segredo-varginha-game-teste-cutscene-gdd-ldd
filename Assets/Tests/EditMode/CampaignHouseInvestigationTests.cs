using Game.Varginha.Experiment;
using NUnit.Framework;
using UnityEngine;

namespace Game.Tests.EditMode
{
    public sealed class CampaignHouseInvestigationTests
    {
        [Test] public void PreparationAndPlacementPrecedeDisappearance()
        {
            var h=new CampaignHouseInvestigation();
            Assert.That(h.TakeBag(),Is.False);
            Assert.That(h.PlaceKey(3),Is.False);
            Assert.That(h.PlaceKey(7),Is.True);
            Assert.That(h.KeyOnCounter,Is.True);
            Assert.That(h.PlaceKey(7),Is.False);
            Assert.That(h.TakeBag(),Is.True);
            Assert.That(h.KeyOnCounter,Is.False);
            Assert.That(h.Missing,Is.True);
            Assert.That(h.TakeBag(),Is.False);
        }
        [Test] public void ObservationAloneDoesNotSolveTheInvestigation()
        {
            var h=new CampaignHouseInvestigation();
            Assert.That(h.Observe(0),Is.False);
            h.PlaceKey(7);h.TakeBag();
            Assert.That(h.Deduce(1),Is.False);
            for(int i=0;i<3;i++)Assert.That(h.Observe(i),Is.True);
            Assert.That(h.CanOpenBox,Is.False);
            Assert.That(h.Deduce(0),Is.False);
            Assert.That(h.Deduce(2),Is.False);
            Assert.That(h.Deduce(1),Is.True);
            Assert.That(h.CanOpenBox,Is.True);
        }
        [TestCase(1)] [TestCase(2)] [TestCase(3)]
        public void InvestigationReloadPreservesMilestones(int step)
        {
            var s=new CampaignStory{phase=2,routine=7};s.house.PlaceKey(7);
            if(step>=2)s.house.TakeBag();
            if(step>=3){for(int i=0;i<3;i++)s.house.Observe(i);s.house.Deduce(1);}
            var restored=JsonUtility.FromJson<CampaignStory>(JsonUtility.ToJson(s));restored.Repair();
            Assert.That(restored.house.step,Is.EqualTo(step));
            Assert.That(restored.house.KeyOnCounter,Is.EqualTo(step==1));
            Assert.That(restored.CanLeaveHouse,Is.False);
        }
        [Test] public void LegacyAcquiredEvidenceDoesNotRequireRepeatingPreparation()
        {
            var s=JsonUtility.FromJson<CampaignStory>("{\"version\":3,\"phase\":2,\"boxFound\":true,\"pagesSolved\":true}");s.Repair();
            Assert.That(s.house.Complete,Is.True);Assert.That(s.CanLeaveHouse,Is.True);
            Assert.That(s.routine,Is.Zero);
        }
        [Test] public void CompletedPuzzleAndMigrationPreserveLaterChapters()
        {
            var s=new CampaignStory{phase=4,boxFound=true,pagesSolved=true,renanMet=true,buildingSolved=true,codeSolved=true};s.Repair();
            Assert.That(s.house.Complete&&s.codeSolved,Is.True);
            Assert.That(s.phase,Is.EqualTo(4));
        }
        [Test] public void InvalidFlagsCannotSkipTheInvestigation()
        {
            var s=new CampaignStory{phase=2};s.house.step=99;s.house.clues=0;s.house.deduced=true;s.Repair();
            Assert.That(s.house.CanOpenBox||s.CanLeaveHouse,Is.False);
        }
        [TestCase(620,620)] [TestCase(947,424)] [TestCase(407,325)]
        public void EvidenceApproachesAreReachableOnTheShippedHouse(int x,int y)
        {
            var map=CampaignMapPlan.Create(2);var point=CampaignIllustratedMaps.Get(2).Position(x,y);
            Assert.That(map.IsClear(point,.25f),Is.True,"The player's feet must fit at the evidence approach.");
            Assert.That(map.Route(map.spawn,point,new System.Collections.Generic.List<Vector2>()),Is.True,"The evidence must be accessible through the original doors.");
        }
    }
}
