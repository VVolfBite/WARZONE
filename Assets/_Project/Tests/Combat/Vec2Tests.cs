using NUnit.Framework;
using Warzone.Core.Math;

namespace Warzone.Tests.Combat
{
    public sealed class Vec2Tests
    {
        [Test]
        public void Vec2_Distance_Works()
        {
            Assert.That(Vec2.Distance(new Vec2(0f, 0f), new Vec2(3f, 4f)), Is.EqualTo(5f).Within(0.001f));
        }

        [Test]
        public void Vec2_MoveTowards_ReachesTargetWithoutOvershoot()
        {
            Vec2 result = Vec2.MoveTowards(new Vec2(0f, 0f), new Vec2(1f, 0f), 2f);
            Assert.That(result, Is.EqualTo(new Vec2(1f, 0f)));
        }

        [Test]
        public void NormalizeSafe_HandlesZeroVector()
        {
            Assert.That(Vec2.NormalizeSafe(Vec2.Zero), Is.EqualTo(Vec2.Zero));
        }
    }
}
