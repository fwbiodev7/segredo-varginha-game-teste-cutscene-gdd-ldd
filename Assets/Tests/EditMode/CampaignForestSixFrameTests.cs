using Game.Varginha.Experiment;
using NUnit.Framework;
using UnityEngine;

namespace Game.Tests.EditMode
{
    public sealed class CampaignForestSixFrameTests
    {
        [TestCase(0,0)][TestCase(0,1)][TestCase(0,2)][TestCase(1,0)][TestCase(1,1)][TestCase(1,2)]
        public void EveryFrameKeepsTheSameNativeCanvasAndGlobalAnchor(int view,int frame)
        {
            var sprite=CampaignStorySprites.Frame(CampaignForestPresence.Atlas,view,frame);
            Assert.That(sprite,Is.Not.Null);Assert.That(sprite.texture.width,Is.EqualTo(480));Assert.That(sprite.texture.height,Is.EqualTo(320));
            Assert.That(sprite.rect.size,Is.EqualTo(new Vector2(160,160)));Assert.That(sprite.pivot.x,Is.EqualTo(80).Within(.001));
            Assert.That(sprite.pivot.y,Is.EqualTo(74).Within(.001));Assert.That(sprite.pixelsPerUnit,Is.EqualTo(100));
            Assert.That(sprite.texture.filterMode,Is.EqualTo(FilterMode.Point));
            Assert.That(CampaignStorySprites.Frame(CampaignForestPresence.Atlas,view,3),Is.Null);
            Assert.That(CampaignStorySprites.Frame(CampaignForestPresence.Atlas,2,frame),Is.Null);
        }
    }
}
