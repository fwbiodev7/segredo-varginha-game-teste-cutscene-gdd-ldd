using Game.Varginha;
using NUnit.Framework;
using UnityEngine;

namespace Game.Tests.EditMode
{
    public class MenuUfoFlightTests
    {
        [Test] public void LongHoverAndPreparationLeadToFastDepartureAndContinuousReturn()
        {
            Assert.That(Vector2.Distance(MenuUfoFlight.Sample(0).Offset,MenuUfoFlight.Sample(1).Offset),Is.GreaterThan(.005f));
            var before=MenuUfoFlight.Sample(17.9f);Assert.That(before.Presence,Is.EqualTo(1));
            var slow=MenuUfoFlight.Sample(MenuUfoFlight.DepartureAt+.05f);var fast=MenuUfoFlight.Sample(MenuUfoFlight.DepartureAt+.48f);
            Assert.That(fast.Fast,Is.True);Assert.That(fast.Offset.x-slow.Offset.x,Is.GreaterThan(.4f));
            Assert.That(slow.Trail,Is.GreaterThan(0));
            Assert.That(MenuUfoFlight.Sample(MenuUfoFlight.DepartureAt+MenuUfoFlight.DepartureDuration+.01f).Presence,Is.Zero);
            Assert.That(MenuUfoFlight.Sample(30).Presence,Is.Zero);
            Assert.That(MenuUfoFlight.Sample(41).Presence,Is.EqualTo(1));
            Assert.That(Vector2.Distance(MenuUfoFlight.Sample(51.99f).Offset,MenuUfoFlight.Sample(52.01f).Offset),Is.LessThan(.001f),"No position jump when the cycle wraps");
        }
    }
}
