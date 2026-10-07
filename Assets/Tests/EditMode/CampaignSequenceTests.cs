using System.Linq;
using Game.Varginha.Experiment;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace Game.Tests.EditMode
{
    public class CampaignSequenceTests
    {
        private static string DetectPipeline() => GraphicsSettings.currentRenderPipeline == null ? "Built-in" : GraphicsSettings.currentRenderPipeline.GetType().FullName;

        [Test] public void FifteenChaptersUseShippedScenesAndNeverEnterRemovedTasks()
        {
            Debug.Log("CAMPAIGN_RENDER_PIPELINE="+DetectPipeline());
            Assert.That(CampaignSequence.Entries.Length,Is.EqualTo(15));
            Assert.That(CampaignSequence.Entries.Select(CampaignSequence.Chapter),Is.EqualTo(Enumerable.Range(1,15)));
            var enabled=EditorBuildSettings.scenes.Where(s=>s.enabled).Select(s=>System.IO.Path.GetFileNameWithoutExtension(s.path)).ToArray();
            foreach(int id in CampaignSequence.Entries.Concat(new[]{10,13}))Assert.That(enabled,Does.Contain(CampaignStorySave.Scene(id)));
            foreach(int id in new[]{3,5,16,19})Assert.That(enabled,Does.Not.Contain(CampaignStorySave.Scene(id)));
            int current=1,guard=0;
            while(current!=21&&guard++<22){Assert.That(new[]{3,5,16,19}.Contains(current),Is.False);current=CampaignSequence.Next(current);}
            Assert.That(current,Is.EqualTo(21));Assert.That(guard,Is.EqualTo(16),"15 chapters plus workshop and cellar areas.");
        }

        [TestCase(3,4)] [TestCase(5,4)] [TestCase(16,17)] [TestCase(19,18)]
        public void LegacyRemovedScenesMigrateOnceAndDiscardOnlyTheirPosition(int before,int after)
        {
            var s=new CampaignStory{version=1,phase=before,positionPhase=before,x=4,y=2};
            s.continuation.clues[before>=11?before-11:0]=7;
            s.Repair();Assert.That(s.phase,Is.EqualTo(after));Assert.That(s.positionPhase,Is.Zero);
            Assert.That(s.version,Is.EqualTo(2));string first=JsonUtility.ToJson(s);s.Repair();Assert.That(JsonUtility.ToJson(s),Is.EqualTo(first));
        }

        [Test] public void MigrationPreservesEvidencePuzzlesEquipmentVictoryAndCrossing()
        {
            var s=new CampaignStory{version=1,phase=21,positionPhase=21,x=2,y=1,boxFound=true,pagesSolved=true,codeSolved=true};
            var e=s.expansion;e.visited=7;e.mapSolved=true;e.forestSigns=3;e.fabioMet=true;e.anchorFound=true;e.evidencePresented=true;e.reagentUnlocked=true;e.sprayed=7;e.stabilized=true;e.reagentCharges=2;e.workshopDeparted=true;
            var c=s.continuation;c.clues=Enumerable.Repeat(255,11).ToArray();c.solved=Enumerable.Repeat(true,11).ToArray();c.battleStudents=new[]{8,3,5};c.battleSupport=2;c.manifestationDispelled=true;c.chambersPrepared=7;c.chamberValues=new[]{0,2,1};c.finalStep=2;
            s.Repair();var restored=JsonUtility.FromJson<CampaignStory>(JsonUtility.ToJson(s));restored.Repair();
            Assert.That(restored.MapFragments,Is.EqualTo(3));Assert.That(restored.positionPhase,Is.EqualTo(21));
            Assert.That(restored.expansion.anchorFound&&restored.expansion.reagentUnlocked&&restored.expansion.workshopDeparted,Is.True);
            Assert.That(restored.expansion.reagentCharges,Is.EqualTo(2));Assert.That(restored.continuation.clues,Is.EqualTo(c.clues));
            Assert.That(restored.continuation.solved,Is.EqualTo(c.solved));Assert.That(restored.continuation.battleStudents,Is.EqualTo(new[]{8,3,5}));Assert.That(restored.continuation.battleSupport,Is.EqualTo(2));
            Assert.That(restored.continuation.CanReturn,Is.True);Assert.That(restored.continuation.finalStep,Is.EqualTo(2));
            Assert.That(CampaignJournal.Entries(restored).Any(text=>text.Contains("PÁGINA OCULTADA")),Is.True);
        }

        [Test] public void HouseAndWorkshopRequireEvidenceWithoutRoutineOrRepeatedPuzzles()
        {
            var s=new CampaignStory{boxFound=true,pages=new[]{0,1,2}};Assert.That(s.SubmitPages(),Is.True);s.Repair();Assert.That(s.CanLeaveHouse,Is.True);Assert.That(s.routine,Is.Zero);
            var e=s.expansion;Assert.That(e.SolveSamples(),Is.False);e.SolveAnchor(1996,1,23);e.evidencePresented=true;
            Assert.That(e.SolveSamples(),Is.True);Assert.That(e.Spray(1),Is.True);Assert.That(e.sprayed,Is.EqualTo(7));Assert.That(e.reagentCharges,Is.EqualTo(5));Assert.That(e.Complete(10),Is.True);Assert.That(e.testDriven,Is.False);
        }

        [Test] public void BattleCalibrationAndCrossingRemainIndependentAndPersist()
        {
            var s=new CampaignContinuationState{chambersPrepared=7,finalRegulators=new[]{1,0,1}};
            Assert.That(s.CalibrateReturn(),Is.False);s.manifestationDispelled=true;s.finalRegulators=new[]{0,0,0};Assert.That(s.CalibrateReturn(),Is.False);
            s.finalRegulators=new[]{1,0,1};Assert.That(s.CalibrateReturn(),Is.True);Assert.That(s.finalStep,Is.Zero);
            var restored=JsonUtility.FromJson<CampaignContinuationState>(JsonUtility.ToJson(s));restored.Repair();Assert.That(restored.CanReturn,Is.True);Assert.That(restored.finalStep,Is.Zero);
            restored.TurnFinalRegulator(0);Assert.That(restored.CanReturn,Is.False);Assert.That(restored.finalCalibrated,Is.False);
            restored.Repair();Assert.That(restored.finalCalibrated,Is.False,"Repair cannot resurrect an invalidated calibration.");
            s.SetFinalSeal(false);Assert.That(s.CanReturn,Is.False);Assert.That(s.CalibrateReturn(),Is.False);
        }

        [Test] public void MergedAgreementKeepsLegacyEvidenceWithoutSkippingTheUnfinishedProcedure()
        {
            var story=new CampaignStory{version=1,phase=19};story.continuation.solved[7]=true;story.continuation.clues[7]=7;
            story.Repair();Assert.That(story.phase,Is.EqualTo(18));Assert.That(story.continuation.solved[7],Is.True);
            Assert.That(story.continuation.AgreementComplete,Is.False);Assert.That(story.continuation.clues[7],Is.EqualTo(7));
            story.continuation.solved[8]=true;Assert.That(story.continuation.AgreementComplete,Is.True);
        }

        [TestCase(11,0)] [TestCase(12,1)] [TestCase(17,2)] [TestCase(20,0)] [TestCase(21,1)]
        public void ResumeAreaIsLimitedToScenesThatExist(int phase,int maximum)
        {
            var story=new CampaignStory{phase=phase,positionPhase=phase};story.continuation.area=2;story.Repair();
            Assert.That(story.continuation.area,Is.EqualTo(maximum));
            if(maximum<2)Assert.That(story.positionPhase,Is.Zero);
        }

        [Test] public void GraduatedHintsFollowCurrentProgressWithoutSolvingPuzzles()
        {
            var story=new CampaignStory{phase=2};string before=JsonUtility.ToJson(story);
            foreach(int id in CampaignSequence.Entries)for(int level=0;level<3;level++)Assert.That(CampaignHints.Get(story,id,level),Is.Not.Null.And.Not.Empty);
            Assert.That(JsonUtility.ToJson(story),Is.EqualTo(before));
            Assert.That(CampaignHints.Get(story,10,2),Does.Contain("vaga"));story.expansion.workshopParked=true;
            Assert.That(CampaignHints.Get(story,10,2),Does.Contain("Uma aplicação"));
        }

        [Test] public void LoadingLegacySavePersistsMigrationAndDoesNotRewriteItOnNextLoad()
        {
            string previous=System.IO.File.Exists(CampaignStorySave.Path)?System.IO.File.ReadAllText(CampaignStorySave.Path):null;
            try
            {
                CampaignStorySave.Write(new CampaignStory{version=1,phase=3,positionPhase=3,boxFound=true,pagesSolved=true});
                var migrated=CampaignStorySave.Load();Assert.That(migrated.phase,Is.EqualTo(4));Assert.That(migrated.pagesSolved,Is.True);
                var disk=JsonUtility.FromJson<CampaignStory>(System.IO.File.ReadAllText(CampaignStorySave.Path));Assert.That(disk.version,Is.EqualTo(2));Assert.That(disk.positionPhase,Is.Zero);
                System.IO.File.SetLastWriteTimeUtc(CampaignStorySave.Path,new System.DateTime(2020,1,1,0,0,0,System.DateTimeKind.Utc));
                var stamp=System.IO.File.GetLastWriteTimeUtc(CampaignStorySave.Path);
                Assert.That(CampaignStorySave.Load().phase,Is.EqualTo(4));Assert.That(System.IO.File.GetLastWriteTimeUtc(CampaignStorySave.Path),Is.EqualTo(stamp));
            }
            finally
            {
                if(previous!=null)System.IO.File.WriteAllText(CampaignStorySave.Path,previous);
                else if(System.IO.File.Exists(CampaignStorySave.Path))System.IO.File.Delete(CampaignStorySave.Path);
            }
        }
    }
}
