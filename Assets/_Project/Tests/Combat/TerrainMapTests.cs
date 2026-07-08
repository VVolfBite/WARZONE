using NUnit.Framework;
using Warzone.Combat;
using Warzone.Core.Math;

namespace Warzone.Tests.Combat
{
    public sealed class TerrainMapTests
    {
        [Test]
        public void DefaultMap_ContainsForestAndBlocksVision()
        {
            TerrainMap map = TerrainMap.CreateDefault();

            Assert.That(map.GetTerrainAt(new Vec2(-6f, 7f)), Is.EqualTo(TerrainType.Forest));
            Assert.That(map.BlocksLineOfSight(new Vec2(-10f, 7f), new Vec2(-2f, 7f)), Is.True);
        }
    }
}



