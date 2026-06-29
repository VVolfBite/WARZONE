using NUnit.Framework;
using Warzone.Combat;

namespace Warzone.Tests.Combat
{
    public sealed class TerrainMapTests
    {
        [Test]
        public void DefaultMap_ContainsForestAndBlocksVision()
        {
            TerrainMap map = TerrainMap.CreateDefault();

            Assert.That(map.GetTerrainAt(new System.Numerics.Vector2(-6f, 7f)), Is.EqualTo(TerrainType.Forest));
            Assert.That(map.BlocksLineOfSight(new System.Numerics.Vector2(-10f, 7f), new System.Numerics.Vector2(-2f, 7f)), Is.True);
        }
    }
}
