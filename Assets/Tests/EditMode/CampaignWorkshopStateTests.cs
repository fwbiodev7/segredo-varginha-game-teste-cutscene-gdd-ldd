using Game.Varginha.Experiment;
using NUnit.Framework;
using UnityEngine;

namespace Game.Tests.EditMode
{
    public class CampaignWorkshopStateTests
    {
        [Test] public void VehicleAndReagentAtlasesShareCachedGpuOnlyPointTextures()
        {
            var first=CampaignWorkshopVehicle.FrameSprite(CampaignWorkshopVehicle.Facing.North);
            for(int i=0;i<4;i++)
            {
                var sprite=CampaignWorkshopVehicle.FrameSprite((CampaignWorkshopVehicle.Facing)i);
                Assert.That(sprite.name,Does.StartWith("Fusca_Original_Top"));Assert.That(sprite.texture.isReadable,Is.False);
                Assert.That(sprite.texture.filterMode,Is.EqualTo(FilterMode.Point));
                Assert.That(sprite,Is.SameAs(CampaignWorkshopVehicle.FrameSprite((CampaignWorkshopVehicle.Facing)i)));
                Assert.That(Mathf.Max(sprite.bounds.size.x,sprite.bounds.size.y),Is.EqualTo(3.9f).Within(.01f));
            }
            Assert.That(first.texture.width,Is.LessThanOrEqualTo(1024));Assert.That(first.texture.height,Is.LessThanOrEqualTo(1024));
            var mark=CampaignReagentMarks.FrameSprite(0);
            for(int i=0;i<3;i++)
            {
                var sprite=CampaignReagentMarks.FrameSprite(i);Assert.That(sprite.texture,Is.SameAs(mark.texture));
                Assert.That(sprite.texture.isReadable,Is.False);Assert.That(sprite.texture.filterMode,Is.EqualTo(FilterMode.Point));
                Assert.That(sprite,Is.SameAs(CampaignReagentMarks.FrameSprite(i)));
            }
            Assert.That(mark.texture.width,Is.LessThanOrEqualTo(512));Assert.That(mark.texture.height,Is.LessThanOrEqualTo(512));
        }
        [Test] public void EarlierInvestigationsMigrateToAParkedVehicle()
        {
            var state=new CampaignExpansionState { evidencePresented=true,labClues=7,sampleOrder=new[]{0,1,2} };
            state.SolveAnchor(1996,1,23);state.SolveSamples();state.Spray(0);
            var restored=JsonUtility.FromJson<CampaignExpansionState>(JsonUtility.ToJson(state));restored.Repair();
            Assert.That(restored.workshopParked,Is.True);Assert.That(restored.sprayed,Is.EqualTo(7));Assert.That(restored.reagentCharges,Is.EqualTo(5));
            Assert.That(restored.workshopDeparted,Is.False);
        }
        [Test] public void InvalidParkingCoordinatesCannotCorruptTheNextArrival()
        {
            var state=new CampaignExpansionState { workshopHasCarPosition=true,workshopCarX=float.NaN,workshopCarY=float.PositiveInfinity,workshopHeading=99,workshopDeparted=true };
            state.Repair();Assert.That(state.workshopHasCarPosition,Is.False);Assert.That(state.workshopCarX,Is.Zero);Assert.That(state.workshopCarY,Is.Zero);
            Assert.That(state.workshopHeading,Is.InRange(0,3));Assert.That(state.workshopDeparted,Is.False);
        }
    }
}
