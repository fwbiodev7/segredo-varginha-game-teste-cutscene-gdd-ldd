using Game.Varginha.Experiment;
using NUnit.Framework;
using UnityEngine;

namespace Game.Tests.EditMode
{
    public sealed class CampaignHouseInvestigationTests
    {
        [TestCase(0,0)] [TestCase(0,1)] [TestCase(0,3)] [TestCase(0,7)]
        [TestCase(1,7)] [TestCase(2,7)] [TestCase(3,7)] [TestCase(4,7)] [TestCase(5,7)]
        public void EveryOpeningMilestoneHasProgressiveHintsWithoutChangingProgress(int step,int routine)
        {
            var h=new CampaignHouseInvestigation{step=step,clues=step>=3?7:0,deduced=step>=3};
            string before=JsonUtility.ToJson(h);
            string first=h.Hint(routine,0),second=h.Hint(routine,1),solution=h.Hint(routine,2);
            Assert.That(first,Is.Not.Empty);Assert.That(second,Is.Not.EqualTo(first));
            Assert.That(solution,Is.Not.EqualTo(first).And.Not.EqualTo(second));
            if(step==0&&routine==0)Assert.That(second,Does.Contain("cozinha"),"The approved adult map places the wash basin beside the stove.");
            Assert.That(JsonUtility.ToJson(h),Is.EqualTo(before),"Hints cannot solve or advance the mission.");
        }
        [Test] public void PreparationAndPlacementPrecedeDisappearance()
        {
            var h=new CampaignHouseInvestigation();
            Assert.That(h.TakeBag(),Is.False);
            Assert.That(h.PlaceKey(3),Is.False);
            Assert.That(h.TakeInitialKey(3),Is.False);
            Assert.That(h.TakeInitialKey(7),Is.True);
            Assert.That(h.HasInitialKey,Is.True);Assert.That(h.Missing,Is.False);
            Assert.That(h.PlaceKey(7),Is.False,"The first visit to the Fusca precedes leaving the key.");
            Assert.That(h.TakeBag(),Is.False);
            Assert.That(h.RememberBag(),Is.True);Assert.That(h.RememberBag(),Is.False,"The first car visit emits its reminder once.");
            Assert.That(h.PlaceKey(7),Is.True);
            Assert.That(h.KeyOnCounter,Is.True);
            Assert.That(h.PlaceKey(7),Is.False);
            Assert.That(h.HasInitialKey,Is.False,"The key is physically on the table now.");
            Assert.That(h.TakeBag(),Is.True);
            Assert.That(h.KeyOnCounter,Is.False);
            Assert.That(h.Missing,Is.True);
            Assert.That(h.TakeBag(),Is.False);
        }
        [Test] public void ObservationAloneDoesNotSolveTheInvestigation()
        {
            var h=new CampaignHouseInvestigation();
            Assert.That(h.Observe(0),Is.False);
            h.TakeInitialKey(7);h.RememberBag();h.PlaceKey(7);h.TakeBag();
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
            var s=new CampaignStory{phase=2,routine=7};s.house.TakeInitialKey(7);s.house.RememberBag();s.house.PlaceKey(7);
            if(step>=2)s.house.TakeBag();
            if(step>=3){for(int i=0;i<3;i++)s.house.Observe(i);s.house.Deduce(1);}
            var restored=JsonUtility.FromJson<CampaignStory>(JsonUtility.ToJson(s));restored.Repair();
            Assert.That(restored.house.step,Is.EqualTo(step));
            Assert.That(restored.house.KeyOnCounter,Is.EqualTo(step==1));
            Assert.That(restored.CanLeaveHouse,Is.False);
        }
        [Test] public void DoorReminderAndBagSearchHaveExclusiveObjectivesAndSurviveReload()
        {
            var h=new CampaignHouseInvestigation();h.TakeInitialKey(7);
            Assert.That(h.Target(7),Is.EqualTo("car"));
            Assert.That(h.Observe(0),Is.False);h.RememberBag();
            var restored=JsonUtility.FromJson<CampaignHouseInvestigation>(JsonUtility.ToJson(h));restored.Repair(false,false);
            Assert.That(restored.Target(7),Is.EqualTo("counter"));Assert.That(restored.PlaceKey(7),Is.True);
            Assert.That(restored.Target(7),Is.EqualTo("bag"));Assert.That(restored.TakeBag(),Is.True);
            Assert.That(restored.Observe(0),Is.True);Assert.That(restored.Missing,Is.True);
        }
        [TestCase(false)] [TestCase(true)]
        public void NewCarDepartureStatesSurviveSaveRepairWithoutUnlockingTheBox(bool visitedCar)
        {
            var story=new CampaignStory{phase=2,routine=7};story.house.TakeInitialKey(7);
            if(visitedCar)story.house.RememberBag();
            var restored=JsonUtility.FromJson<CampaignStory>(JsonUtility.ToJson(story));restored.Repair();
            Assert.That(restored.house.HasInitialKey,Is.True);
            Assert.That(restored.house.State,Is.EqualTo(visitedCar?CampaignHouseState.ReturningForBag:CampaignHouseState.KeyInPocket));
            Assert.That(restored.house.Missing||restored.house.CanOpenBox||restored.CanLeaveHouse,Is.False);
            Assert.That(restored.house.Target(7),Is.EqualTo(visitedCar?"counter":"car"));
        }
        [Test] public void LegacyBagAlreadyCollectedDoesNotGetStuckBehindReminder()
        {
            var s=new CampaignStory{phase=2,routine=15};s.house.step=1;s.Repair();
            Assert.That(s.house.Missing,Is.True);Assert.That(s.house.bagReminder,Is.True);
        }
        [Test] public void LegacyAcquiredEvidenceDoesNotRequireRepeatingPreparation()
        {
            var s=JsonUtility.FromJson<CampaignStory>("{\"version\":3,\"phase\":2,\"boxFound\":true,\"pagesSolved\":true}");s.Repair();
            Assert.That(s.house.Complete,Is.True);Assert.That(s.CanLeaveHouse,Is.True);
            Assert.That(s.routine,Is.Zero);
        }
        [Test] public void CompletedPuzzleAndMigrationPreserveLaterChapters()
        {
            var s=new CampaignStory{version=3,phase=4,boxFound=true,pagesSolved=true,renanMet=true,buildingSolved=true,codeSolved=true};s.Repair();
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
