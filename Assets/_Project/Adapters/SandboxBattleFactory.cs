using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Warzone.Combat;
using Warzone.Content.Authoring;
using Warzone.Content;
using Warzone.Content.Definitions;

namespace Warzone.Adapters
{
    public static class SandboxBattleFactory
    {
        public static ContentCatalog BuildSandboxContent()
        {
            ContentCatalogAsset catalogAsset = Resources.Load<ContentCatalogAsset>("ContentCatalog");
            if (catalogAsset != null)
            {
                ContentCatalog catalog = ContentCatalogBuilder.Build(catalogAsset);
                if (catalog.Units.Count > 0)
                {
                    return catalog;
                }
            }

            WeaponDefinition playerRifle = new WeaponDefinition("weapon.player.rifle", 12f, 0.75f, 4, 26f);
            WeaponDefinition playerSupport = new WeaponDefinition("weapon.player.carbine", 9f, 0.5f, 3, 22f);
            WeaponDefinition enemyClaws = new WeaponDefinition("weapon.enemy.claws", 2.0f, 1.2f, 1, 14f);
            WeaponDefinition enemyRifle = new WeaponDefinition("weapon.enemy.rifle", 10f, 0.9f, 2, 20f);

            Dictionary<string, UnitDefinition> units = new Dictionary<string, UnitDefinition>
            {
                ["unit.player.infantry"] = new UnitDefinition("unit.player.infantry", "Infantry", FactionId.Player, 22, 5.75f, playerRifle, 15f, 0.72f),
                ["unit.player.support"] = new UnitDefinition("unit.player.support", "Support", FactionId.Player, 26, 5.1f, playerSupport, 13f, 0.76f),
                ["unit.enemy.zombie"] = new UnitDefinition("unit.enemy.zombie", "Zombie", FactionId.Enemy, 10, 2.8f, enemyClaws, 8f, 0.78f),
                ["unit.enemy.rifleman"] = new UnitDefinition("unit.enemy.rifleman", "Rifleman", FactionId.Enemy, 14, 3.9f, enemyRifle, 11f, 0.7f)
            };

            Dictionary<string, MissionDefinition> missions = new Dictionary<string, MissionDefinition>
            {
                ["mission.sandbox"] = new MissionDefinition("mission.sandbox", "Sandbox", 1, 3)
            };

            return new ContentCatalog(units, missions);
        }

        public static BattleSession CreateSandboxBattle(ContentCatalog catalog, int seed)
        {
            List<BattleSquadState> squads = new List<BattleSquadState>
            {
                CreateSingleUnitSquad(1, FactionId.Player, "unit.player.infantry", -14f, -3.5f, 22),
                CreateSingleUnitSquad(2, FactionId.Player, "unit.player.infantry", -12f, 0f, 22),
                CreateSingleUnitSquad(3, FactionId.Player, "unit.player.infantry", -14f, 3.5f, 22),
                CreateSingleUnitSquad(4, FactionId.Player, "unit.player.infantry", -16f, 0f, 22)
            };

            return new BattleSession(
                squads,
                new CommandProcessor(),
                new CombatResolver(catalog),
                seed);
        }

        public static IReadOnlyList<SandboxWaveSpawnPlan> BuildWaves()
        {
            return new List<SandboxWaveSpawnPlan>
            {
                new SandboxWaveSpawnPlan(1, 1.5f, new[]
                {
                    CreateSingleUnitSquad(101, FactionId.Enemy, "unit.enemy.zombie", 16f, -4f, 10),
                    CreateSingleUnitSquad(102, FactionId.Enemy, "unit.enemy.zombie", 18f, 1f, 10),
                    CreateSingleUnitSquad(103, FactionId.Enemy, "unit.enemy.rifleman", 19f, -1f, 14)
                }),
                new SandboxWaveSpawnPlan(2, 2.5f, new[]
                {
                    CreateSingleUnitSquad(104, FactionId.Enemy, "unit.enemy.zombie", 17f, -2f, 10),
                    CreateSingleUnitSquad(105, FactionId.Enemy, "unit.enemy.rifleman", 19f, 2f, 14),
                    CreateSingleUnitSquad(106, FactionId.Enemy, "unit.enemy.zombie", 18f, 4f, 10)
                }),
                new SandboxWaveSpawnPlan(3, 3.0f, new[]
                {
                    CreateSingleUnitSquad(107, FactionId.Enemy, "unit.enemy.rifleman", 20f, 0f, 14),
                    CreateSingleUnitSquad(108, FactionId.Enemy, "unit.enemy.zombie", 21f, 3.5f, 12),
                    CreateSingleUnitSquad(109, FactionId.Enemy, "unit.enemy.rifleman", 22f, -3.5f, 14)
                })
            };
        }

        private static BattleSquadState CreateSingleUnitSquad(int squadId, FactionId factionId, string definitionId, float x, float y, int health)
        {
            return new BattleSquadState(
                squadId,
                factionId,
                new Vector2(x, y),
                new List<BattleUnitState>
                {
                    new BattleUnitState(new BattleEntityId(squadId), definitionId, factionId, health)
                });
        }
    }
}
