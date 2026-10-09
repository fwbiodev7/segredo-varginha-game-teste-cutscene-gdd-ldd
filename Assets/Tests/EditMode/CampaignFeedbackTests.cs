using System.Linq;
using Game.Varginha.Experiment;
using NUnit.Framework;
using UnityEngine;

namespace Game.Tests.EditMode
{
    public class CampaignFeedbackTests
    {
        [TestCaseSource(typeof(CampaignVisualPolishTests),nameof(CampaignVisualPolishTests.Phases))]
        public void AuthoredContactShapesMatchNavigationAndStayWithinThePhysicalBase(int phase)
        {
            var data=CampaignIllustratedMaps.Get(phase);var plan=new CampaignMapPlan{phase=phase};CampaignIllustratedMaps.Apply(plan);
            for(int i=0;i<data.walls.Length && !data.repeat;i++)
                if(data.walls[i].exact)Assert.That(plan.walls[i],Is.EqualTo(data.Area(data.walls[i].rect)),"Exact wall "+phase+":"+i);
            foreach(var source in data.props.Where(p=>p.collision?.Length>=6))
            {
                var prop=plan.furniture.Single(p=>p.name==source.name);
                Assert.That(prop.collision.Length,Is.EqualTo(source.collision.Length/2));
                float area=0;
                for(int i=0;i<source.collision.Length;i+=2)
                {
                    float x=source.collision[i],y=source.collision[i+1];
                    Assert.That(x,Is.InRange(source.@base[0],source.@base[0]+source.@base[2]),source.name);
                    Assert.That(y,Is.InRange(source.@base[1],source.@base[1]+source.@base[3]),source.name);
                    int j=(i+2)%source.collision.Length;area+=x*source.collision[j+1]-source.collision[j]*y;
                }
                Assert.That(Mathf.Abs(area),Is.GreaterThan(1),source.name+" nondegenerate");
            }
        }
        [Test] public void RoundedPotCornersDoNotBlockEmptyFloorAndItsCentreRemainsSolid()
        {
            var plan=CampaignMapPlan.Create(6);var data=CampaignIllustratedMaps.Get(6);
            Assert.That(plan.IsClear(data.Position(69.2f,487.2f),.001f),Is.True,"Empty corner outside the actual pot");
            Assert.That(plan.IsClear(data.Position(80,497),.01f),Is.False,"Solid pot centre");
        }
        [Test] public void RadioStaticIsShortBoundedAndHasQuietEndpoints()
        {
            var clip=CampaignSoundscape.Clip("RadioInterference");var samples=new float[clip.samples];clip.GetData(samples,0);
            Assert.That(clip.length,Is.EqualTo(.65f).Within(.001f));
            Assert.That(samples.Max(v=>Mathf.Abs(v)),Is.InRange(.01f,.25f));
            Assert.That(Mathf.Abs(samples[0]),Is.LessThan(.001f));Assert.That(Mathf.Abs(samples[^1]),Is.LessThan(.001f));
        }
    }
}
