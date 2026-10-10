using Game.Varginha;
using NUnit.Framework;

namespace Game.Tests.EditMode
{
    public class MenuSuspenseTests
    {
        [Test]
        public void IdleLookWaitsForItsWindowAndActivityCancelsIt()
        {
            var clock=new MenuSuspenseClock(964,0);
            for(int i=0;i<12;i++)
            {
                float start=i*100;
                clock.Activity(start);
                Assert.That(clock.NextIdle-start,Is.InRange(40f,90f));
                float eventTime=clock.NextIdle;
                Assert.That(clock.Tick(eventTime-.01f,true),Is.False);
                Assert.That(clock.Tick(eventTime,true),Is.True);
                Assert.That(clock.Tick(eventTime+.01f,true),Is.False,"One event, not one per frame");
                Assert.That(clock.Gaze(eventTime),Is.Zero);
                Assert.That(clock.Gaze(eventTime+2.5f),Is.EqualTo(1).Within(.001));
                Assert.That(clock.Gaze(eventTime+7.1f),Is.Zero);
                clock.Activity(eventTime+3);
                Assert.That(clock.Gaze(eventTime+3),Is.Zero,"Input cancels the stare");
                Assert.That(clock.NextIdle-(eventTime+3),Is.InRange(40f,90f));
            }
        }
        [Test]
        public void ReducedEffectsSuppressStaresAndInterference()
        {
            var clock=new MenuSuspenseClock(96,0);float due=clock.NextIdle;
            Assert.That(clock.Tick(due,false),Is.False);
            Assert.That(clock.Gaze(due+3),Is.Zero);
            Assert.That(clock.Glitch(due),Is.False);
            Assert.That(clock.TitleGlitch(due),Is.False);
            clock.Activity(due);
            Assert.That(clock.Tick(due+39,true),Is.False);
        }
    }
}
