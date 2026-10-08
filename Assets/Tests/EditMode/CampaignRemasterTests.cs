using Game.Varginha.Experiment;
using NUnit.Framework;

namespace Game.Tests.EditMode
{
    public class CampaignRemasterTests
    {
        [TestCase(3, CampaignTransitionStyle.CameraPan, "Road")]
        [TestCase(7, CampaignTransitionStyle.Fade, "Forest")]
        [TestCase(9, CampaignTransitionStyle.Fade, "Lab")]
        [TestCase(11, CampaignTransitionStyle.Fade, "School")]
        [TestCase(15, CampaignTransitionStyle.Paranormal, "House")]
        public void NarrativeTransitionsMatchImplementedLocations(int phase, CampaignTransitionStyle style, string ambience)
        {
            var profile = CampaignTransitionProfile.ForScene(CampaignStorySave.Scene(phase));
            Assert.That(profile.style, Is.EqualTo(style));
            Assert.That(profile.ambience, Is.EqualTo(ambience));
        }
        [Test] public void InteriorChangesCrossfadeAndMenuDoesNotPreviewHouseAudio()
        {
            Assert.That(CampaignTransitionProfile.ForScene(CampaignContinuationDefinition.SceneName(12,1)).style, Is.EqualTo(CampaignTransitionStyle.Crossfade));
            Assert.That(CampaignTransitionProfile.ForScene("Menu_MisterioDeVarginha").ambience, Is.Null);
        }
    }
}
