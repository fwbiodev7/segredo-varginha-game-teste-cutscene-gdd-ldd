using Game.Varginha;
using NUnit.Framework;

namespace Game.Tests.EditMode
{
    public class MenuCaptionRotationTests
    {
        [Test]
        public void EachMenuEntrySelectsADifferentCaptionAndAllFourCanAppear()
        {
            var random = new System.Random(1996);
            var seen = new System.Collections.Generic.HashSet<int>();
            int previous = -1;
            for(int i = 0; i < 128; i++)
            {
                int next = MenuCaptionRotation.NextIndex(previous, random);
                Assert.That(next, Is.InRange(0, MenuCaptionRotation.Count - 1));
                Assert.That(next, Is.Not.EqualTo(previous));
                Assert.That(MenuCaptionRotation.Text(next), Is.Not.Empty);
                seen.Add(next); previous = next;
            }
            Assert.That(seen.Count, Is.EqualTo(4));
        }

        [TestCase(-1)]
        [TestCase(400)]
        public void MissingOrOutdatedPreferenceStillSelectsAValidCaption(int previous)
        {
            Assert.That(MenuCaptionRotation.NextIndex(previous, new System.Random(96)),
                Is.InRange(0, MenuCaptionRotation.Count - 1));
        }
    }
}
