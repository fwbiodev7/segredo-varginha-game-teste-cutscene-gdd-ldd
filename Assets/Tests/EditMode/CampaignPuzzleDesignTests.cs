using System;
using System.Collections.Generic;
using Game.Varginha.Experiment;
using NUnit.Framework;
using UnityEngine;

namespace Game.Tests.EditMode
{
    public class CampaignPuzzleDesignTests
    {
        [Test] public void MapRequiresBothSpatialPlacementAndConnectedDirections()
        {
            var state=new CampaignExpansionState{visited=7,mapOrder=new[]{0,1,2}};
            Assert.That(state.SolveMap(),Is.False);
            state.mapDirections=new[]{1,1,0};Assert.That(state.SolveMap(),Is.True);
            state.mapSolved=false;state.mapOrder=new[]{1,0,2};Assert.That(state.SolveMap(),Is.False);
            state.mapOrder=new[]{0,1,2};state.visited=3;Assert.That(state.SolveMap(),Is.False);
        }
        [Test] public void AlteredArchiveRejectsTheOldFirstChoiceAndUnchangedDetails()
        {
            Assert.That(CampaignPuzzleDesign.Correct(11,0,null),Is.False);
            Assert.That(CampaignPuzzleDesign.Correct(11,1,null),Is.False);
            Assert.That(CampaignPuzzleDesign.Correct(11,3,null),Is.True);
            Assert.That(CampaignPuzzleDesign.Correct(11,7,null),Is.False);
        }
        [TestCase(13)][TestCase(14)][TestCase(15)][TestCase(18)]
        public void DeductionNeedsAllRelationsWithoutRepeatedEvidence(int phase)
        {
            var correct=CampaignPuzzleDesign.Solution(phase);
            Assert.That(CampaignPuzzleDesign.Correct(phase,0,null),Is.False);
            Assert.That(CampaignPuzzleDesign.Correct(phase,0,correct),Is.True);
            int swap=correct[0];correct[0]=correct[1];correct[1]=swap;
            Assert.That(CampaignPuzzleDesign.Correct(phase,0,correct),Is.False);
            Array.Fill(correct,0);Assert.That(CampaignPuzzleDesign.Correct(phase,0,correct),Is.False);
        }
        [Test] public void PartialAnswersSurviveSaveAndOldCompletedChaptersStayComplete()
        {
            var story=new CampaignStory{phase=14};story.puzzles.archiveMarks=1;
            story.puzzles.causes=new[]{2,-1,0,-1};story.puzzles.dates=new[]{-1,2,-1};
            story.puzzles.memory=new[]{0,1};story.puzzles.hypotheses=new[]{1,-1,-1};
            var restored=JsonUtility.FromJson<CampaignStory>(JsonUtility.ToJson(story));restored.Repair();
            Assert.That(restored.puzzles.causes,Is.EqualTo(story.puzzles.causes));
            Assert.That(restored.puzzles.dates,Is.EqualTo(story.puzzles.dates));
            Assert.That(restored.puzzles.memory,Is.EqualTo(story.puzzles.memory));
            Assert.That(restored.puzzles.archiveMarks,Is.EqualTo(story.puzzles.archiveMarks));
            Assert.That(restored.puzzles.hypotheses,Is.EqualTo(story.puzzles.hypotheses));
            var legacy=new CampaignStory{puzzles=null};legacy.expansion.visited=7;legacy.expansion.mapSolved=true;
            foreach(int slot in new[]{0,2,3,4,7,8})legacy.continuation.solved[slot]=true;
            legacy.Repair();Assert.That(legacy.expansion.mapSolved,Is.True);
            Assert.That(legacy.continuation.AgreementComplete,Is.True);
            Assert.That(legacy.puzzles.archiveMarks,Is.EqualTo(3));
            Assert.That(legacy.puzzles.hypotheses,Is.EqualTo(new[]{1,2,0}));
        }
        [Test] public void CompletedLegacyJsonWithoutNewPuzzleFieldsKeepsAllSolvedInvestigations()
        {
            const string json="{\"version\":3,\"phase\":18,\"expansion\":{\"visited\":7,\"mapSolved\":true,\"mapOrder\":[0,1,2],\"forestSigns\":3,\"fabioMet\":true},\"continuation\":{\"clues\":[7,7,7,135,15,0,0,7,15,0,0],\"solved\":[true,false,true,true,true,false,false,true,true,false,false]}}";
            var restored=JsonUtility.FromJson<CampaignStory>(json);restored.Repair();
            Assert.That(restored.expansion.mapSolved,Is.True);
            Assert.That(restored.expansion.fabioMet,Is.True);
            Assert.That(restored.expansion.mapDirections,Is.EqualTo(new[]{1,1,0}));
            foreach(int slot in new[]{0,2,3,4,7,8})
                Assert.That(restored.continuation.solved[slot],Is.True,"Completed legacy slot "+slot);
            Assert.That(restored.continuation.AgreementComplete,Is.True);
            Assert.That(restored.puzzles.archiveMarks,Is.EqualTo(3));
            Assert.That(restored.puzzles.causes,Is.EqualTo(new[]{0,1,2,3}));
            Assert.That(restored.puzzles.dates,Is.EqualTo(new[]{0,1,2}));
            Assert.That(restored.puzzles.memory,Is.EqualTo(new[]{0,1,2,3}));
            Assert.That(restored.puzzles.hypotheses,Is.EqualTo(new[]{1,2,0}));
        }
        [Test] public void UnfinishedLegacyJsonKeepsMapPlacementAndCreatesEditableEmptyAnswers()
        {
            const string json="{\"version\":3,\"phase\":6,\"expansion\":{\"visited\":3,\"mapSolved\":false,\"mapOrder\":[1,2,0]},\"continuation\":{}}";
            var restored=JsonUtility.FromJson<CampaignStory>(json);restored.Repair();
            Assert.That(restored.expansion.visited,Is.EqualTo(3));
            Assert.That(restored.expansion.mapOrder,Is.EqualTo(new[]{1,2,0}));
            Assert.That(restored.expansion.mapDirections,Has.Length.EqualTo(3));
            Assert.That(restored.expansion.SolveMap(),Is.False);
            Assert.That(restored.puzzles.causes,Is.EqualTo(new[]{-1,-1,-1,-1}));
            Assert.That(restored.puzzles.dates,Is.EqualTo(new[]{-1,-1,-1}));
            Assert.That(restored.puzzles.memory,Is.Empty);
            Assert.That(restored.puzzles.hypotheses,Is.EqualTo(new[]{-1,-1,-1}));
            Assert.That(restored.continuation.solved,Has.All.EqualTo(false));
        }
        [Test] public void CorruptSavedAnswersAreRepairableWithoutDiscardingValidEvidenceLinks()
        {
            const string json="{\"version\":3,\"phase\":13,\"expansion\":{\"mapDirections\":[-1,5,2]},\"puzzles\":{\"archiveMarks\":65,\"causes\":[0,0,4,-2],\"dates\":[2,2,-7],\"memory\":[0,1,1],\"hypotheses\":[0,1]}}";
            var restored=JsonUtility.FromJson<CampaignStory>(json);restored.Repair();
            Assert.That(restored.expansion.mapDirections,Is.EqualTo(new[]{0,3,2}));
            Assert.That(restored.puzzles.archiveMarks,Is.EqualTo(1));
            Assert.That(restored.puzzles.causes,Is.EqualTo(new[]{0,-1,-1,-1}));
            Assert.That(restored.puzzles.dates,Is.EqualTo(new[]{2,-1,-1}));
            Assert.That(restored.puzzles.memory,Is.Empty);
            Assert.That(restored.puzzles.hypotheses,Is.EqualTo(new[]{-1,-1,-1}));
            Assert.That(CampaignPuzzleDesign.Correct(13,-1,restored.puzzles.causes),Is.False);
            restored.puzzles.causes=CampaignPuzzleDesign.Solution(13);
            Assert.That(CampaignPuzzleDesign.Correct(13,-1,restored.puzzles.causes),Is.True);
        }
        [TestCase(11)][TestCase(13)][TestCase(14)][TestCase(15)][TestCase(18)]
        public void CompletedInvestigationsGuideToTheirExitEvenWhenOldClueBitsAreMissing(int phase)
        {
            var story=new CampaignStory{phase=phase};story.continuation.solved[phase-11]=true;
            if(phase==18)story.continuation.solved[8]=true;
            Assert.That(CampaignGuidance.Target(story,phase),Is.EqualTo("exit"));
            Assert.That(CampaignGuidance.Next(story,phase),Does.Contain(phase==11?"recuperada":"concluída"));
        }
        [Test] public void GuidanceMovesFromEvidenceToPuzzleToNamedExit()
        {
            var story=new CampaignStory{phase=6};Assert.That(CampaignGuidance.Target(story,6),Is.EqualTo("archive"));
            story.expansion.visited=7;Assert.That(CampaignGuidance.Target(story,6),Is.EqualTo("map"));
            story.expansion.mapSolved=true;Assert.That(CampaignGuidance.Next(story,6),Does.Contain("saia").IgnoreCase);
            Assert.That(CampaignGuidance.Target(story,6),Is.EqualTo("exit"));
            story.continuation.clues[1]=1;story.continuation.serviceRevealed=true;
            Assert.That(CampaignGuidance.Target(story,12,0),Is.EqualTo("entrance"));
            Assert.That(CampaignGuidance.Target(story,12,1),Is.EqualTo("key"));
            story.continuation.serviceKey=true;Assert.That(CampaignGuidance.Target(story,12,1),Is.EqualTo("service"));
        }
        [TestCase(4)][TestCase(5)]
        public void CompletedSchoolGuidanceUsesRenanToReachTheLibrary(int phase)
        {
            var story=new CampaignStory{phase=phase,renanMet=true,photoOpened=true,buildingSolved=true,
                timeOpened=true,schoolTimeChoice=1,codeSolved=true,renanConfirmed=true};
            story.Repair();
            Assert.That(CampaignGuidance.Target(story,phase),Is.EqualTo("renan"));
            Assert.That(CampaignGuidance.Next(story,phase),Does.Contain("Renan"));
            Assert.That(CampaignGuidance.Next(story,phase),Does.Contain("biblioteca").IgnoreCase);
            Assert.That(CampaignGuidance.Next(story,phase),Does.Not.Contain("saída").IgnoreCase);
        }
        [Test] public void MansionRoomsHaveReachableDoorsAndASpecificServiceCorridor()
        {
            var plan=CampaignContinuationDefinition.Plan(12,1);var data=CampaignIllustratedMaps.Get(112);
            var route=new List<Vector2>();
            foreach(var pixel in new[]{new Vector2(325,350),new Vector2(748,329),new Vector2(820,200)})
                Assert.That(plan.Route(plan.spawn,data.Position(pixel.x,pixel.y),route),Is.True,"Mansion route to "+pixel);
            Assert.That(CampaignGuidance.RoomName(plan,data.Position(525,380)),Is.EqualTo("Hall central"));
            Assert.That(CampaignGuidance.RoomName(plan,data.Position(325,350)),Is.EqualTo("Sala de estar"));
            Assert.That(CampaignGuidance.RoomName(plan,data.Position(748,329)),Is.EqualTo("Escritório"));
            Assert.That(CampaignGuidance.RoomName(plan,data.Position(820,200)),Is.EqualTo("Corredor de serviço"));
        }
    }
}
