using System.Collections.Generic;
using NUnit.Framework;
using Warzone.BattleDomain;
using Warzone.Content;
using Warzone.Content.Definitions;

namespace Warzone.Tests.BattleDomain
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
                new CombatResolver(catalog),
                new MissionRuntime(),
                seed: 7);

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
                    new System.Numerics.Vector2(-2f, 0f),
                    new List<BattleUnitState>
                    {
                        new BattleUnitState(new BattleEntityId(1), "unit.player", FactionId.Player, 10),
                        new BattleUnitState(new BattleEntityId(2), "unit.player", FactionId.Player, 10)
                    }),
                new BattleSquadState(
                    2,
                    FactionId.Enemy,
                    new System.Numerics.Vector2(2f, 0f),
                    new List<BattleUnitState>
                    {
                        new BattleUnitState(new BattleEntityId(101), "unit.enemy", FactionId.Enemy, 6),
                        new BattleUnitState(new BattleEntityId(102), "unit.enemy", FactionId.Enemy, 6)
                    })
            };
        }

        private static ContentCatalog BuildCatalog()
        {
            WeaponDefinition playerWeapon = new WeaponDefinition("weapon.player", 4f, 1f, 3, 16f);
            WeaponDefinition enemyWeapon = new WeaponDefinition("weapon.enemy", 2f, 1f, 1, 12f);

            return new ContentCatalog(
                new Dictionary<string, UnitDefinition>
                {
                    ["unit.player"] = new UnitDefinition("unit.player", "Player", FactionId.Player, 10, 4f, playerWeapon, 8f, 0.65f),
                    ["unit.enemy"] = new UnitDefinition("unit.enemy", "Enemy", FactionId.Enemy, 6, 3f, enemyWeapon, 6f, 0.65f)
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
                new CombatResolver(catalog),
                new MissionRuntime(),
                seed: 7);

            for (int i = 0; i < 30 && session.CurrentOutcome == MissionOutcome.InProgress; i++)
            {
                session.Tick(1f);
            }

            Assert.That(session.CurrentOutcome, Is.EqualTo(MissionOutcome.Victory));
        }
    }
}
