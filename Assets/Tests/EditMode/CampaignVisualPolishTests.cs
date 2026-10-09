using System.Collections.Generic;
using System.Linq;
using Game.Varginha.Experiment;
using NUnit.Framework;
using UnityEngine;

namespace Game.Tests.EditMode
{
    public class CampaignVisualPolishTests
    {
        public static int[] Phases = {1,2,3,4,5,6,7,8,9,10,11,12,112,13,14,15,16,18,19,20,21,121};
        [TestCaseSource(nameof(Phases))]
        public void EveryForegroundMeshHasValidCutsSupportsAndOriginalPixelSampling(int phase)
        {
            var layout=CampaignIllustratedMaps.Get(phase);
            var texture=Resources.Load<Texture2D>("Varginha/IllustratedMaps/"+layout.image);
            Assert.That(texture.filterMode,Is.EqualTo(FilterMode.Point));
            Assert.That(texture.mipmapCount,Is.EqualTo(1));
            foreach(var prop in layout.props)
            {
                Assert.That(prop.art[0]+prop.art[2],Is.LessThanOrEqualTo(layout.width+1),prop.name);
                Assert.That(prop.art[1]+prop.art[3],Is.LessThanOrEqualTo(layout.height+1),prop.name);
                if(prop.@base?.Length==4)
                {
                    Assert.That(prop.@base[0],Is.GreaterThanOrEqualTo(prop.art[0]),prop.name+" collider left");
                    Assert.That(prop.@base[1],Is.GreaterThanOrEqualTo(prop.art[1]),prop.name+" collider top");
                    Assert.That(prop.@base[0]+prop.@base[2],Is.LessThanOrEqualTo(prop.art[0]+prop.art[2]),prop.name+" collider right");
                    Assert.That(prop.@base[1]+prop.@base[3],Is.LessThanOrEqualTo(prop.art[1]+prop.art[3]),prop.name+" collider bottom");
                }
                var paths=prop.pieces?.Length>0?prop.pieces.Select(p=>p.outline):new[]{prop.outline};
                foreach(var path in paths)
                {
                    Assert.That(path.Length%2,Is.Zero,prop.name);
                    foreach(float coordinate in path)Assert.That(coordinate,Is.InRange(0f,1f),prop.name);
                    var points=new Vector2[path.Length/2];
                    for(int i=0;i<points.Length;i++)points[i]=new Vector2(path[i*2],1-path[i*2+1]);
                    Assert.That(CampaignIllustratedContour.Triangulate(points).Length,Is.EqualTo((points.Length-2)*3),prop.name);
                }
                if(!string.IsNullOrEmpty(prop.support))Assert.That(layout.props.Any(p=>p.name==prop.support),Is.True,prop.name+" support");
            }
        }
        [Test]
        public void FragmentosHasClearCrossingBelowPillarsAndLadderStopsAtItsFeet()
        {
            var plan=CampaignMapPlan.Create(6);var data=CampaignIllustratedMaps.Get(6);
            foreach(float x in new[]{294f,315f,333f,355f,390f,415f,448f})
                Assert.That(plan.IsClear(data.Position(x,270),.25f),Is.True,"Walkable floor below column/ladder "+x);
            Assert.That(plan.IsClear(data.Position(415,237),.23f),Is.False,"Solid ladder feet");
            Assert.That(plan.IsClear(data.Position(333,216),.23f),Is.False,"Solid column floor contact");
            foreach(var wall in plan.bodyWalls)Assert.That(wall.height,Is.GreaterThan(wall.width*1.3f),"Horizontal walls do not create invisible head barriers");
        }
        [Test]
        public void TabletopPlantsHaveIndependentMeshesAndNoFloorBlockingShape()
        {
            var data=CampaignIllustratedMaps.Get(6);
            foreach(var plant in data.props.Where(p=>!string.IsNullOrEmpty(p.support)))
            { Assert.That(plant.@base.Length,Is.Zero);Assert.That(plant.motif,Is.EqualTo("PottedPlant")); }
            Assert.That(data.props.Count(p=>!string.IsNullOrEmpty(p.support)),Is.EqualTo(3));
            Assert.That(data.props.Single(p=>p.name=="Escada da estante").pieces.Length,Is.EqualTo(8));
        }
        [TestCase(8)][TestCase(14)]
        public void ReusedChurchHasAllThreeStainedGlassColoursWithSmoothEdges(int phase)
        {
            var data=CampaignIllustratedMaps.Get(phase);
            Assert.That(data.stainedGlass.Length,Is.EqualTo(9));
            Assert.That(data.stainedGlass.Any(l=>l.color[0]>l.color[2]),Is.True,"Amber");
            Assert.That(data.stainedGlass.Any(l=>l.color[2]>l.color[0]&&l.color[1]>l.color[0]),Is.True,"Blue");
            Assert.That(data.stainedGlass.Any(l=>l.color[0]>l.color[1]&&l.color[2]>l.color[0]),Is.True,"Violet");
            foreach(var light in data.stainedGlass)Assert.That(light.falloff,Is.InRange(.1f,.4f));
        }
    }
}
