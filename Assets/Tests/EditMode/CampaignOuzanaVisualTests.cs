using Game.Varginha.Experiment;
using NUnit.Framework;
using UnityEngine;

namespace Game.Tests.EditMode
{
    public sealed class CampaignOuzanaVisualTests
    {
        [TestCase(0)][TestCase(1)][TestCase(2)][TestCase(3)]
        [TestCase(4)][TestCase(5)][TestCase(6)][TestCase(7)]
        public void EightFacingsKeepNativeFramesAndApprovedThreePoseCycles(int direction)
        {
            float angle=direction*45*Mathf.Deg2Rad;
            Vector2 face=new(-Mathf.Sin(angle),-Mathf.Cos(angle));
            Assert.That(CampaignStorySprites.EightDirection(face),Is.EqualTo(direction));
            for(int frame=0;frame<6;frame++)
            {
                foreach(string sheet in new[]{"OuzanaWalkV2","OuzanaAttackV2"})
                {
                    var pose=CampaignStorySprites.Frame(sheet,direction,frame);
                    Assert.That(pose,Is.Not.Null,sheet+direction+frame);
                    Assert.That(pose.texture.filterMode,Is.EqualTo(FilterMode.Point));
                    Assert.That(pose.texture.isReadable,Is.False);
                    Assert.That(pose.texture.mipmapCount,Is.EqualTo(1));
                    Assert.That(pose.bounds.min.y,Is.EqualTo(-.58f).Within(.002f));
                    Assert.That(pose.bounds.size.y,Is.InRange(1.25f,1.85f));
                    Assert.That(CampaignStorySprites.Frame(sheet,direction,frame),Is.SameAs(pose));
                }
            }
            // All six authored frames above stay available, while the approved
            // compact cycles select three full poses without doubling their speed.
            int[] walk={0,2,4},attack={0,3,5};
            for(int cycle=0;cycle<2;cycle++)for(int pose=0;pose<3;pose++)
                Assert.That(CampaignOuzanaVisual.Walk(face,cycle*.66f+pose*.22f+.001f),Is.SameAs(CampaignStorySprites.Frame("OuzanaWalkV2",direction,walk[pose])));
            for(int pose=0;pose<3;pose++)
                Assert.That(CampaignOuzanaVisual.Attack(face,pose),Is.SameAs(CampaignStorySprites.Frame("OuzanaAttackV2",direction,attack[pose])));
            Assert.That(CampaignOuzanaVisual.Walk(face,-.1f),Is.SameAs(CampaignStorySprites.Frame("OuzanaWalkV2",direction,0)));
            Assert.That(CampaignOuzanaVisual.Idle(face),Is.SameAs(CampaignOuzanaVisual.Attack(face,5)));
        }
    }
}
