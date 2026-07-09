using System.Collections.Generic;
using NUnit.Framework;
using Warzone.Combat;
using Warzone.Content;
using Warzone.Content.Definitions;
using Warzone.Core.Math;

namespace Warzone.Tests.Combat
{
    public sealed class BattleSessionTests
    {
        [Test]
        public void Tick_WhenBothSidesFight_EventuallyProducesVictoryOrDefeat()
        {
            ContentCatalog catalog = BuildCatalog();
            BattleSession session = new BattleSession(
                BuildSquads(),
                new CommandProcessor(),
                new CombatResolver(catalog, TerrainMap.CreateDefault()),
                seed: 7,
                terrainMap: TerrainMap.CreateDefault());

            for (int i = 0; i < 20 && session.CurrentOutcome == MissionOutcome.InProgress; i++)
            {
                session.Tick(1f);
            }

            Assert.That(session.CurrentOutcome, Is.Not.EqualTo(MissionOutcome.InProgress));
            BattleResult result = session.BuildResult();
            Assert.That(result.UnitOutcomes.Count, Is.EqualTo(4));
        }

        private static IReadOnlyList<BattleSquadState> BuildSquads()
        {
            return new List<BattleSquadState>
            {
                new BattleSquadState(
                    1,
                    FactionId.Player,
                    new Vec2(-2f, 0f),
                    new List<BattleUnitState>
                    {
                        new BattleUnitState(new BattleEntityId(1), "unit.player", FactionId.Player, 10),
                        new BattleUnitState(new BattleEntityId(2), "unit.player", FactionId.Player, 10)
                    }),
                new BattleSquadState(
                    2,
                    FactionId.Enemy,
                    new Vec2(2f, 0f),
                    new List<BattleUnitState>
                    {
                        new BattleUnitState(new BattleEntityId(101), "unit.enemy", FactionId.Enemy, 6),
                        new BattleUnitState(new BattleEntityId(102), "unit.enemy", FactionId.Enemy, 6)
                    })
            };
        }

        private static ContentCatalog BuildCatalog()
        {
            WeaponDefinition playerWeapon = new WeaponDefinition("weapon.player", 4f, 1f, 3, 16f, DamageType.Piercing);
            WeaponDefinition enemyWeapon = new WeaponDefinition("weapon.enemy", 2f, 1f, 1, 12f, DamageType.Kinetic);

            return new ContentCatalog(
                new Dictionary<string, UnitDefinition>
                {
                    { "unit.player", new UnitDefinition("unit.player", "Player", FactionId.Player, 10, 4f, playerWeapon, 8f, 0.65f, ArmorType.Medium, "effect.support.heal") },
                    { "unit.enemy", new UnitDefinition("unit.enemy", "Enemy", FactionId.Enemy, 6, 3f, enemyWeapon, 6f, 0.65f, ArmorType.Light, "effect.zombie.toxic") }
                },
                new Dictionary<string, MissionDefinition>());
        }

        [Test]
        public void Tick_WhenEnemySideIsEliminated_ProducesVictory()
        {
            ContentCatalog catalog = BuildCatalog();
            BattleSession session = new BattleSession(
                BuildSquads(),
                new CommandProcessor(),
                new CombatResolver(catalog, TerrainMap.CreateDefault()),
                seed: 7,
                terrainMap: TerrainMap.CreateDefault());

            for (int i = 0; i < 30 && session.CurrentOutcome == MissionOutcome.InProgress; i++)
            {
                session.Tick(1f);
            }

            Assert.That(session.CurrentOutcome, Is.EqualTo(MissionOutcome.Victory));
        }

        [Test]
        public void BattleSession_AppliesDefaultStatusEffectsFromDefinitions()
        {
            ContentCatalog catalog = BuildCatalog();
            BattleSession session = new BattleSession(
                BuildSquads(),
                new CommandProcessor(),
                new CombatResolver(catalog, TerrainMap.CreateDefault()),
                seed: 7,
                terrainMap: TerrainMap.CreateDefault());

            Assert.That(session.Squads[0].Units[0].StatusEffects.Count, Is.EqualTo(1));
            Assert.That(session.Squads[0].Units[0].StatusEffects[0].Definition.Id, Is.EqualTo("effect.support.heal"));
            Assert.That(session.Squads[1].Units[0].StatusEffects.Count, Is.EqualTo(1));
            Assert.That(session.Squads[1].Units[0].StatusEffects[0].Definition.Id, Is.EqualTo("effect.zombie.toxic"));
        }
    }
}



