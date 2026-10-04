using System.Collections.Generic;
using Game.Varginha.Experiment;
using NUnit.Framework;
using UnityEngine;

namespace Game.Tests.EditMode
{
    public sealed class CampaignIllustratedMapTests
    {
        [Test]public void FlashCanBeReplayedAfterUnityDiscardsNativeSprites()
        {
            var frame=CampaignFlashArt.Explosion[4];Object.DestroyImmediate(frame);
            Assert.That(CampaignFlashArt.Explosion[4],Is.Not.Null);
            Assert.That(CampaignFlashArt.Explosion[4].texture.name,Is.EqualTo("AlienExplosion"));
        }
        [Test]public void NewAnimationSheetsHaveAllFramesWithoutReadableCpuTextureCopies()
        {
            string[] names={"WalkGray","Life","LifeGray","Seated","SeatedGray","Punch","Driving","RenanTeaching","RenanProps","SeatedDeskNorth","OuzanaBiologist"};
            int[] columns={4,12,12,6,6,8,4,4,3,6,4},rows={4,4,4,8,8,4,4,4,1,1,4};
            for(int i=0;i<names.Length;i++)for(int r=0;r<rows[i];r++)for(int c=0;c<columns[i];c++)
            {
                var frame=CampaignStorySprites.Frame(names[i],r,c);
                Assert.That(frame,Is.Not.Null,names[i]+r+":"+c);
                Assert.That(frame.texture.isReadable,Is.False);Assert.That(frame.texture.filterMode,Is.EqualTo(FilterMode.Point));
                Assert.That(frame.rect.xMin,Is.GreaterThanOrEqualTo(0));Assert.That(frame.rect.yMin,Is.GreaterThanOrEqualTo(0));
                Assert.That(frame.rect.xMax,Is.LessThanOrEqualTo(frame.texture.width));Assert.That(frame.rect.yMax,Is.LessThanOrEqualTo(frame.texture.height));
            }
        }
        [TestCase(0)][TestCase(1)][TestCase(3)][TestCase(7)]
        public void CorrectChurchCombinationSurvivesReloadWithoutHiddenInspectionGates(int inspected)
        {
            var state = new CampaignExpansionState { anchorClues = inspected };
            Assert.That(state.SolveAnchor(1996,0,23),Is.False);
            Assert.That(state.SolveAnchor(1996,1,1),Is.False);
            Assert.That(state.SolveAnchor(1898,1,23),Is.False);
            Assert.That(state.anchorClues,Is.EqualTo(inspected));
            Assert.That(state.SolveAnchor(1996,1,23),Is.True);
            var restored = JsonUtility.FromJson<CampaignExpansionState>(JsonUtility.ToJson(state));
            restored.Repair();
            Assert.That(restored.Complete(8),Is.True);
            Assert.That(restored.anchorClues,Is.EqualTo(7));
        }
        [Test]public void ChurchScaleUsesSeparateCachedFramesAndOtherMapsKeepNormalSize()
        {
            foreach(string name in new[]{"Life","LifeGray","WalkGray","OuzanaBiologist"})
                for(int row=0;row<4;row++)
                {
                    var sprite=CampaignStorySprites.Frame(name,row,0);
                    Assert.That(sprite.bounds.size.y,Is.EqualTo(CampaignTeamEdelzio.StandingHeight).Within(.04f),name+row);
                    Assert.That(sprite.bounds.min.y,Is.EqualTo(-.58f).Within(.002f),name+row);
                    var church=CampaignStorySprites.Frame(name,row,0,churchScale:true);
                    Assert.That(church.bounds.size.y,Is.EqualTo(name=="OuzanaBiologist"?CampaignTeamEdelzio.StandingHeight:CampaignTeamEdelzio.ChurchStandingHeight).Within(.04f),name+row);
                    Assert.That(church.bounds.min.y,Is.EqualTo(-.58f).Within(.002f));
                    Assert.That(CampaignStorySprites.Frame(name,row,0),Is.SameAs(sprite),"The reduced cache must not replace the normal frame.");
                }
            for(int row=0;row<4;row++)
                Assert.That(CampaignTeamEdelzio.Frame("EdelzioWalk",row,1).bounds.size.y,
                    Is.EqualTo(CampaignTeamEdelzio.StandingHeight).Within(.002f));
        }
        [Test]public void LegacySolvedChurchRecordsRecoverTheirConfirmedCombination()
        {
            var state=new CampaignExpansionState {anchorFound=true,anchorClues=7};
            state.Repair();
            Assert.That(state.Complete(8),Is.True);
            Assert.That(state.anchorYear,Is.EqualTo(1996));
            Assert.That(state.anchorSymbol,Is.EqualTo(1));
            Assert.That(state.anchorRecord,Is.EqualTo(23));
        }
        [Test]public void ShadowsTurnAwayFromTheNearestLampAndOpenAirKeepsContactOnly()
        {
            var layout=new CampaignIllustratedMaps.Layout {width=100,height=100,bounds=new[]{0f,0f,10f,10f},phase=2,
                lights=new[]{new CampaignIllustratedMaps.Light {pixel=new[]{50f,50f},radius=5,color=new[]{1f,1f,1f}},
                new CampaignIllustratedMaps.Light {pixel=new[]{0f,0f},radius=10,color=new[]{1f,1f,1f}}}};
            Assert.That(CampaignCharacterShadows.AwayFromNearestLight(layout,new Vector2(6,5),out _),Is.EqualTo(Vector2.right));
            Assert.That(CampaignCharacterShadows.AwayFromNearestLight(layout,new Vector2(4,5),out _),Is.EqualTo(Vector2.left));
            var house=CampaignIllustratedMaps.Get(2);
            Assert.That(CampaignCharacterShadows.OpenAir(house,house.Position(200,300)),Is.False);
            Assert.That(CampaignCharacterShadows.OpenAir(house,house.Position(1300,300)),Is.True);
        }
        [Test]public void FuscaPosesKeepAStableBodyPivotWithOptimizedTransparentTexture()
        {
            var texture=Resources.Load<Texture2D>("Varginha/IllustratedMaps/FuscaActors");
            Assert.That(texture.height,Is.EqualTo(1024));Assert.That(texture.isReadable,Is.False);Assert.That(texture.filterMode,Is.EqualTo(FilterMode.Point));
            for(int frame=0;frame<4;frame++)
            {
                var sprite=CampaignFuscaLighting.Frame(texture,frame);
                try{Assert.That(sprite.bounds.center,Is.EqualTo(Vector3.zero));Assert.That(sprite.bounds.size.y,Is.EqualTo(2).Within(.001f));}
                finally{Object.DestroyImmediate(sprite);}
            }
        }
        [TestCase(4)][TestCase(5)]public void ClassroomFeetRemainClearWhenForegroundDeskCropsChange(int phase)
        {
            var plan=CampaignMapPlan.Create(phase);
            foreach(var seat in CampaignIllustratedMaps.SchoolSeats(plan.phase))
                Assert.That(plan.IsClear(seat-Vector2.up*.58f,.22f),Is.True,"Seats follow the chair floor position, not the monitor crop.");
        }
        [Test]public void FurnitureContoursExcludeConcaveFloorGapsAndTriangulateEveryMap()
        {
            for(int phase=1;phase<=10;phase++)foreach(var prop in CampaignIllustratedMaps.Get(phase).props)
            {
                Assert.That(prop.outline.Length,Is.GreaterThanOrEqualTo(6),prop.name);
                var points=new Vector2[prop.outline.Length/2];
                for(int i=0;i<points.Length;i++)points[i]=new Vector2(prop.outline[i*2],1-prop.outline[i*2+1]);
                Assert.That(CampaignIllustratedContour.Triangulate(points).Length,Is.EqualTo((points.Length-2)*3),prop.name);
            }
            var concave=new[]{new Vector2(0,0),new Vector2(1,0),new Vector2(1,1),new Vector2(.7f,1),new Vector2(.7f,.4f),new Vector2(.3f,.4f),new Vector2(.3f,1),new Vector2(0,1)};
            var triangles=CampaignIllustratedContour.Triangulate(concave);float area=0;
            for(int i=0;i<triangles.Length;i+=3)
            {
                var a=concave[triangles[i]];var b=concave[triangles[i+1]];var c=concave[triangles[i+2]];
                area+=Mathf.Abs((b.x-a.x)*(c.y-a.y)-(b.y-a.y)*(c.x-a.x))/2;
            }
            Assert.That(area,Is.EqualTo(.76f).Within(.0001f),"The notch stays transparent rather than filled by a triangle fan.");
        }
        [Test]public void DrivingLaneFollowsTheSuppliedAsphaltAndExcludesTheSidewalk()
        {
            var data=CampaignIllustratedMaps.Get(3);var plan=CampaignMapPlan.Create(3);
            Assert.That(plan.spawn.x,Is.EqualTo(data.Position(837,0).x).Within(.001f));
            Assert.That(plan.IsClear(data.Position(650,470),.25f),Is.False,"Left sidewalk");
            Assert.That(plan.IsClear(data.Position(1000,470),.25f),Is.False,"Right sidewalk");
            Assert.That(plan.IsClear(data.Position(837,470),.55f),Is.True,"Car in the asphalt lane");
        }
        [TestCase(1)][TestCase(2)][TestCase(3)][TestCase(4)][TestCase(5)][TestCase(6)][TestCase(7)][TestCase(8)][TestCase(9)][TestCase(10)]
        public void SuppliedArtworkHasWalkableObjectivesAndCollidersWithinFurniture(int phase)
        {
            var data=CampaignIllustratedMaps.Get(phase);var plan=CampaignMapPlan.Create(phase);
            Assert.That(plan.IsClear(plan.spawn,.25f),Is.True,"Spawn");
            var route=new List<Vector2>();
            foreach(var p in plan.points)
            {
                Assert.That(plan.IsClear(p.position,.25f),Is.True,p.id);
                Assert.That(plan.Route(plan.spawn,p.position,route),Is.True,p.id);
            }
            foreach(var furniture in plan.furniture)
            {
                var art=new Rect(furniture.position-furniture.size/2,furniture.size);
                if(furniture.footprint.width>0)
                {
                    Assert.That(furniture.footprint.xMin,Is.GreaterThanOrEqualTo(art.xMin-.001f),furniture.name);
                    Assert.That(furniture.footprint.xMax,Is.LessThanOrEqualTo(art.xMax+.001f),furniture.name);
                    Assert.That(furniture.footprint.yMin,Is.GreaterThanOrEqualTo(art.yMin-.001f),furniture.name);
                    Assert.That(furniture.footprint.yMax,Is.LessThanOrEqualTo(art.yMax+.001f),furniture.name);
                }
            }
            var source=Resources.Load<Texture2D>("Varginha/IllustratedMaps/"+data.image);
            Assert.That(source,Is.Not.Null);Assert.That(source.filterMode,Is.EqualTo(FilterMode.Point));
            Assert.That(source.mipmapCount,Is.EqualTo(1));Assert.That(source.isReadable,Is.False,"No retained CPU copy of full map");
        }
        [TestCase(1)][TestCase(2)][TestCase(4)][TestCase(6)][TestCase(7)][TestCase(8)][TestCase(9)][TestCase(10)]
        public void ForegroundUsesSourceTextureAndSharesRealFloorContact(int phase)
        {
            var root=new GameObject("IllustrationLayers");
            try
            {
                var plan=CampaignMapPlan.Create(phase);var map=CampaignMapConstruction.Build(root.transform,plan);Physics2D.SyncTransforms();
                var texture=Resources.Load<Texture2D>("Varginha/IllustratedMaps/"+CampaignIllustratedMaps.Get(phase).image);
                foreach(var item in plan.furniture)
                {
                    var go=map.Find("02_Mobilia_Colisoes/"+item.name);var renderer=go.GetComponent<SpriteRenderer>();
                    Assert.That(renderer.sprite.texture,Is.SameAs(texture),item.name+" shared GPU texture");
                    if(item.footprint.width<=0)continue;
                    var collider=go.GetComponent<BoxCollider2D>();
                    Assert.That(collider.bounds.center.x,Is.EqualTo(item.footprint.center.x).Within(.001f));
                    Assert.That(collider.bounds.center.y,Is.EqualTo(item.footprint.center.y).Within(.001f));
                    Assert.That(renderer.GetComponent<Game.Varginha.VarginhaWorldDepth>(),Is.Not.Null,item.name+" depth");
                }
            }
            finally{Object.DestroyImmediate(root);}
        }
    }
}
