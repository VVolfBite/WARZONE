using System.Collections.Generic;
using NUnit.Framework;
using Warzone.Combat;
using Warzone.Content;
using Warzone.Content.Definitions;

namespace Warzone.Tests.Combat
{
    public sealed class AiSystemTests
    {
        [Test]
        public void Tick_WhenEnemyHasTarget_QueuesAttackOrMove()
        {
            ContentCatalog catalog = BuildCatalog();
            BattleSession session = new BattleSession(
                BuildSquads(),
                new CommandProcessor(),
                new CombatResolver(catalog, TerrainMap.CreateDefault()),
                seed: 11,
                terrainMap: TerrainMap.CreateDefault());

            AiSystem aiSystem = new AiSystem();
            aiSystem.Tick(session, 1f);

            BattleSquadState enemy = session.Squads[1];
            Assert.That(enemy.CommandState, Is.EqualTo(SquadCommandState.Moving).Or.EqualTo(SquadCommandState.Attacking));
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
                        new BattleUnitState(new BattleEntityId(1), "unit.player", FactionId.Player, 10)
                    }),
                new BattleSquadState(
                    2,
                    FactionId.Enemy,
                    new System.Numerics.Vector2(2f, 0f),
                    new List<BattleUnitState>
                    {
                        new BattleUnitState(new BattleEntityId(101), "unit.enemy", FactionId.Enemy, 6)
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
                    ["unit.player"] = new UnitDefinition("unit.player", "Player", FactionId.Player, 10, 4f, playerWeapon, 8f, 0.65f, ArmorType.Medium),
                    ["unit.enemy"] = new UnitDefinition("unit.enemy", "Enemy", FactionId.Enemy, 6, 3f, enemyWeapon, 6f, 0.65f, ArmorType.Light)
                },
                new Dictionary<string, MissionDefinition>());
        }
    }
}
