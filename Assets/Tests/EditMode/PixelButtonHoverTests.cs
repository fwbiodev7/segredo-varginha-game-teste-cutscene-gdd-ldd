using Game.UI;
using NUnit.Framework;

namespace Game.Tests.EditMode
{
    public class PixelButtonHoverTests
    {
        [Test] public void FocusAndExitTakeTwoHundredMillisecondsAndDisabledClearsImmediately()
        {
            var motion=new PixelButtonHoverState();
            motion.Advance(true,.1f);Assert.That(motion.Amount,Is.EqualTo(.5f).Within(.001f));
            motion.Advance(true,.1f);Assert.That(motion.Amount,Is.EqualTo(1));
            motion.Advance(false,.1f);Assert.That(motion.Amount,Is.EqualTo(.5f).Within(.001f));
            motion.Advance(false,.1f);Assert.That(motion.Amount,Is.Zero);
            motion.Advance(true,1);motion.Advance(true,0,false);Assert.That(motion.Amount,Is.Zero);
        }
    }
}
