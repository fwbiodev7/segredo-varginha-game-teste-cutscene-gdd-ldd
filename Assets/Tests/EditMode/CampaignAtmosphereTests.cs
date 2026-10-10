using Game.Varginha.Experiment;
using NUnit.Framework;
using UnityEngine;

namespace Game.Tests.EditMode
{
    [Category("Atmosphere")]
    public class CampaignAtmosphereTests
    {
        [Test] public void FourteenChaptersProgressWithoutLosingColourAndReadableEdges()
        {
            float previous = -1;
            foreach (int phase in CampaignSequence.Entries)
            {
                var profile = CampaignAtmosphereProfile.For(phase);
                if (phase != 21) Assert.That(profile.tension, Is.GreaterThan(previous));
                previous = profile.tension;
                Assert.That(profile.vignette, Is.InRange(.025f, .24f));
                Assert.That(profile.saturation, Is.GreaterThanOrEqualTo(-13));
                Assert.That(profile.contrast, Is.InRange(0, 11));
            }
            Assert.That(CampaignAtmosphereProfile.For(1).AudioBand, Is.Zero);
            Assert.That(CampaignAtmosphereProfile.For(7).AudioBand, Is.EqualTo(1));
            Assert.That(CampaignAtmosphereProfile.For(20).AudioBand, Is.EqualTo(2));
        }
        [Test] public void ExistingEventsIncreaseTensionAndResolutionRestoresCalm()
        {
            var story = new CampaignStory();
            Assert.That(CampaignAtmosphereProfile.For(12, 1).tension, Is.GreaterThan(CampaignAtmosphereProfile.For(12).tension));
            Assert.That(CampaignAtmosphereProfile.For(13).tension, Is.GreaterThan(CampaignAtmosphereProfile.For(12, 1).tension));
            float baseline = CampaignAtmosphereProfile.For(2).tension; story.boxFound = true;
            Assert.That(CampaignAtmosphereProfile.For(2, 0, story).tension, Is.GreaterThan(baseline));
            story.continuation.manifestationDispelled = true;
            Assert.That(CampaignAtmosphereProfile.For(20, 0, story).tension, Is.LessThan(CampaignAtmosphereProfile.For(20).tension));
            Assert.That(CampaignAtmosphereProfile.For(21, 1, story).tension, Is.LessThan(.1f));
            Assert.That(CampaignAtmosphereProfile.For(1, 0, null, new CampaignMemory { powerFailed = true }).tension, Is.GreaterThan(CampaignAtmosphereProfile.For(1).tension));
        }
        [Test] public void SceneIdsAndEnvironmentsAgreeWithControllers()
        {
            foreach (int phase in CampaignSequence.Entries)
                Assert.That(CampaignAtmosphereProfile.ScenePhase(CampaignStorySave.Scene(phase)), Is.EqualTo(phase));
            Assert.That(CampaignAtmosphereProfile.ScenePhase("Menu_MisterioDeVarginha"), Is.Zero);
            Assert.That(CampaignAtmosphereProfile.Environment(6), Is.EqualTo("House"));
            Assert.That(CampaignAtmosphereProfile.Environment(21, 1), Is.EqualTo("School"));
        }
        [Test] public void CachedStereoBedsHaveHeadroomAndSeamlessEndpoints()
        {
            AudioClip previous = null;
            for (int band = 0; band < 3; band++)
            {
                var clip = CampaignAtmosphereAudio.Bed("Forest", band);
                Assert.That(clip, Is.SameAs(CampaignAtmosphereAudio.Bed("Forest", band)));
                Assert.That(clip, Is.Not.SameAs(previous)); previous = clip;
                Assert.That(clip.channels, Is.EqualTo(2)); Assert.That(clip.length, Is.EqualTo(24).Within(.001f));
                var samples = new float[clip.samples * clip.channels]; Assert.That(clip.GetData(samples, 0), Is.True);
                float peak = 0, energy = 0;
                foreach (float sample in samples) { peak = Mathf.Max(peak, Mathf.Abs(sample)); energy += sample * sample; }
                Assert.That(peak, Is.LessThanOrEqualTo(.18f)); Assert.That(energy / samples.Length, Is.GreaterThan(.00001f));
                Assert.That(Mathf.Abs(samples[0]) + Mathf.Abs(samples[^1]), Is.LessThan(.0001f));
            }
        }
    }
}
