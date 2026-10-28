using System.Linq;
using Game.Varginha.Experiment;
using NUnit.Framework;
using UnityEngine;

namespace Game.Tests.EditMode
{
    public sealed class CampaignSchoolInvestigationTests
    {
        public static void Advance(CampaignSchoolInvestigation s,int stage)
        {
            s.RecoverDocuments();for(int i=0;i<6;i++)s.Examine(i);
            if(stage>0){s.Swap(0,1);s.Swap(1,2);Assert.That(s.Submit(out _),Is.True);}
            if(stage>1){s.ChooseComparison(CampaignSchoolDocuments.UniqueEvent);Assert.That(s.Submit(out _),Is.True);}
            if(stage>2){for(int i=0;i<3;i++)s.Link(i,i);Assert.That(s.Submit(out _),Is.True);}
            if(stage>3){s.Examine(6);s.ChooseDestination(1);Assert.That(s.Submit(out _),Is.True);}
        }
        [Test]public void FreshGameCannotSolveWithoutRecoveringAndReadingItsDocuments()
        {
            var story=new CampaignStory();story.Repair();var s=story.school;
            Assert.That(s.documentsRecovered||s.completed,Is.False);Assert.That(s.Submit(out _),Is.False);
            Assert.That(s.Examine(0),Is.False);s.RecoverDocuments();s.Swap(0,1);s.Swap(1,2);
            Assert.That(s.Submit(out _),Is.False);Assert.That(story.SubmitPhotoClue(1)||story.SubmitCode(),Is.False);
            Assert.That(story.MapFragments,Is.Zero);
        }
        [Test]public void BoxRecoveryIsIdempotentAndFullDocumentsRemainAvailableInLaterChapters()
        {
            var story=new CampaignStory{boxFound=true,phase=21};var s=story.school;
            Assert.That(s.RecoverDocuments(),Is.True);s.Examine(4);Assert.That(s.RecoverDocuments(),Is.False);
            Assert.That(s.examined,Is.EqualTo(16));
            var journal=CampaignJournal.Entries(story);
            for(int id=0;id<6;id++)Assert.That(journal,Does.Contain(CampaignSchoolDocuments.Text(id)));
        }
        [TestCase(0)][TestCase(1)][TestCase(2)][TestCase(3)][TestCase(4)]
        public void EveryStageSurvivesSerializationAndRepeatedRepair(int stage)
        {
            var story=new CampaignStory{phase=4,boxFound=true};Advance(story.school,stage);story.SyncSchoolCompletion();story.school.hintLevel=2;
            var restored=JsonUtility.FromJson<CampaignStory>(JsonUtility.ToJson(story));restored.Repair();
            Assert.That(restored.school.stage,Is.EqualTo(stage));Assert.That(restored.school.hintLevel,Is.EqualTo(2));
            Assert.That(restored.school.completed,Is.EqualTo(stage==4));Assert.That(restored.codeSolved,Is.EqualTo(stage==4));
            string first=JsonUtility.ToJson(restored);restored.Repair();Assert.That(JsonUtility.ToJson(restored),Is.EqualTo(first));
        }
        [Test]public void WrongSelectionsAndUndoDoNotEraseTheOtherCorrectLinks()
        {
            var s=new CampaignSchoolInvestigation();Advance(s,2);
            s.Link(0,0);s.Link(1,2);s.Link(2,2);Assert.That(s.Submit(out _),Is.False);
            s.Link(1,-1);Assert.That(s.links,Is.EqualTo(new[]{0,-1,2}));s.Link(1,1);
            Assert.That(s.Submit(out _),Is.True);
            s.Examine(6);s.ChooseDestination(0);Assert.That(s.Submit(out _),Is.False);s.ChooseDestination(1);Assert.That(s.Submit(out _),Is.True);
        }
        [Test]public void ComparisonAcceptsOnlyOneUniqueEventAndKeepsTheLastSelection()
        {
            var s=new CampaignSchoolInvestigation();Advance(s,1);
            Assert.That(s.Submit(out _),Is.False);
            Assert.That(s.ChooseComparison(0),Is.True);Assert.That(s.Submit(out _),Is.False);
            Assert.That(s.ChooseComparison(1),Is.True);Assert.That(s.comparisonChoice,Is.EqualTo(1));
            Assert.That(s.ChooseComparison(2),Is.True);Assert.That(s.Submit(out _),Is.True);
            Assert.That(s.ChooseComparison(0),Is.False,"A completed stage cannot be changed.");
        }
        [Test]public void LaterStagesDoNotRequireNewReadingsAndPreserveLegacyProgress()
        {
            var s=new CampaignSchoolInvestigation();s.RecoverDocuments();for(int i=0;i<3;i++)s.Examine(i);
            s.order=new[]{0,1,2};Assert.That(s.Submit(out _),Is.True);
            s.ChooseComparison(2);Assert.That(s.Submit(out _),Is.True);
            for(int i=0;i<3;i++)s.Link(i,i);Assert.That(s.Submit(out _),Is.True,"Reading the displayed notes is sufficient.");
            Assert.That(s.examined&56,Is.Zero);Assert.That(s.noticeRead,Is.False);
            s.ChooseDestination(1);Assert.That(s.Submit(out _),Is.True,"The mural is optional context.");
            s.comparisonMigrated=false;s.comparisonChoice=-1;s.Repair();
            Assert.That(s.completed,Is.True);Assert.That(s.comparisonChoice,Is.EqualTo(2));
        }
        [TestCase(1)][TestCase(2)][TestCase(3)]
        public void LegacyCompletedSchoolMigratesWithoutReplayingOrChangingLaterEvidence(int version)
        {
            var story=new CampaignStory{version=version,phase=11,positionPhase=11,codeSolved=true,boxFound=true};
            story.continuation.clues[0]=3;story.Repair();
            Assert.That(story.school.completed&&story.codeSolved&&story.renanConfirmed,Is.True);
            Assert.That(story.continuation.clues[0],Is.EqualTo(3));Assert.That(story.positionPhase,Is.EqualTo(11));
        }
        [Test]public void LegacyPartialSchoolGainsItsOwnedDocumentsButMustCompleteTheNewAnalysis()
        {
            var story=new CampaignStory{version=3,phase=4,buildingSolved=true,photoOpened=true,renanMet=true};story.Repair();
            Assert.That(story.school.documentsRecovered,Is.True);Assert.That(story.school.completed||story.codeSolved,Is.False);
            Assert.That(story.school.stage,Is.Zero);Assert.That(story.renanMet,Is.True);
        }
        [Test]public void CorruptStateCannotSkipFourStagesAndInvalidIdsDoNotMutateEvidence()
        {
            var s=new CampaignSchoolInvestigation{documentsRecovered=true,completed=true,stage=90,order=new[]{0,0,8},links=null};s.Repair();
            Assert.That(s.completed,Is.False);Assert.That(s.stage,Is.Zero);
            string before=JsonUtility.ToJson(s);Assert.That(s.Examine(-1)||s.Examine(7)||s.ToggleHighlight(0)||s.Link(0,0),Is.False);
            Assert.That(JsonUtility.ToJson(s),Is.EqualTo(before));
        }
        [Test]public void SchoolDocumentsContainAllAnswersWithoutPrematureLore()
        {
            string documents=string.Join("\n",Enumerable.Range(0,7).Select(CampaignSchoolDocuments.Text));
            foreach(string token in new[]{"14/01/1996","20/01/1996","26/01/1996","Rua das Acácias","clarão azul","poste","Três batidas","árvore","C-96","Biblioteca da Industrial"})Assert.That(documents,Does.Contain(token));
            foreach(string token in new[]{"1898","selo vivo","diocese","entidade","23:23"})Assert.That(documents,Does.Not.Contain(token));
        }
        [Test]public void InconsistentFinalStageCanBeConfirmedAgainInsteadOfBecomingStuck()
        {
            var s=new CampaignSchoolInvestigation();Advance(s,4);s.completed=false;s.Repair();
            Assert.That(s.stage,Is.EqualTo(3));Assert.That(s.Submit(out _),Is.True);Assert.That(s.completed,Is.True);
        }
        [Test]public void SchoolInvestigationTargetsAreReachableOnTheApprovedMap()
        {
            var plan=CampaignMapPlan.Create(4);var path=new System.Collections.Generic.List<Vector2>();
            foreach(var point in plan.points)Assert.That(plan.Route(plan.spawn,point.position,path),Is.True,point.id);
        }
        static void AssertShuffled(CampaignSchoolInvestigation s)
        {
            Assert.That(s.noteOrder.OrderBy(x=>x),Is.EqualTo(new[]{0,1,2}));
            Assert.That(s.passageOrder.OrderBy(x=>x),Is.EqualTo(new[]{0,1,2}));
            Assert.That(s.noteOrder,Is.Not.EqualTo(new[]{0,1,2}));Assert.That(s.passageOrder,Is.Not.EqualTo(new[]{0,1,2}));
            for(int row=0;row<3;row++)Assert.That(s.noteOrder[row],Is.Not.EqualTo(s.passageOrder[row]),"Position must never reveal a correct pair.");
        }
        [Test]public void BothColumnsShuffleWithoutAlignedAnswersAcrossDifferentSeeds()
        {
            var layouts=new System.Collections.Generic.HashSet<string>();
            for(int seed=0;seed<64;seed++)
            {
                var s=new CampaignSchoolInvestigation();Assert.That(s.EnsureLinkLayout(new System.Random(seed)),Is.True);AssertShuffled(s);
                layouts.Add(string.Join(",",s.noteOrder)+"/"+string.Join(",",s.passageOrder));
                string before=JsonUtility.ToJson(s);Assert.That(s.EnsureLinkLayout(new System.Random(seed+1)),Is.False);
                Assert.That(JsonUtility.ToJson(s),Is.EqualTo(before),"Reopening must not reshuffle a valid layout.");
            }
            Assert.That(layouts.Count,Is.GreaterThan(1));
        }
        [Test]public void SerializedLayoutAndPartialAssociationsRemainStable()
        {
            var s=new CampaignSchoolInvestigation();Advance(s,2);s.Link(0,0);s.Link(1,2);
            string before=JsonUtility.ToJson(s);var restored=JsonUtility.FromJson<CampaignSchoolInvestigation>(before);restored.Repair();
            AssertShuffled(restored);Assert.That(JsonUtility.ToJson(restored),Is.EqualTo(before));
            Assert.That(restored.links,Is.EqualTo(new[]{0,2,-1}));
        }
        [Test]public void MissingLayoutInOlderV4SavePreservesSemanticAssociationsAndCompletion()
        {
            foreach(int stage in new[]{2,4})
            {
                var s=new CampaignSchoolInvestigation();Advance(s,stage);if(stage==2){s.Link(0,0);s.Link(1,2);}
                s.noteOrder=s.passageOrder=null;var restored=JsonUtility.FromJson<CampaignSchoolInvestigation>(JsonUtility.ToJson(s));restored.Repair();
                AssertShuffled(restored);Assert.That(restored.links,Is.EqualTo(s.links));Assert.That(restored.stage,Is.EqualTo(stage));
                Assert.That(restored.completed,Is.EqualTo(stage==4));string first=JsonUtility.ToJson(restored);restored.Repair();Assert.That(JsonUtility.ToJson(restored),Is.EqualTo(first));
            }
        }
        [Test]public void InvalidDisplayOrdersAreRepairedWithoutClearingCorrectLinks()
        {
            var s=new CampaignSchoolInvestigation();Advance(s,2);s.Link(0,0);
            s.noteOrder=new[]{1,1,7};s.passageOrder=new[]{0,1,2};s.Repair();AssertShuffled(s);
            Assert.That(s.links,Is.EqualTo(new[]{0,-1,-1}));Assert.That(s.stage,Is.EqualTo(2));
        }
        [Test]public void MatchingRowsIsWrongButMatchingDocumentContentStillSolvesThePuzzle()
        {
            var s=new CampaignSchoolInvestigation();Advance(s,2);
            for(int row=0;row<3;row++)s.Link(s.noteOrder[row],s.passageOrder[row]);Assert.That(s.Submit(out _),Is.False);
            for(int note=0;note<3;note++)s.Link(note,note);Assert.That(s.Submit(out _),Is.True);
        }
    }
}
