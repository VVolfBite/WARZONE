using System.Collections.Generic;
using UnityEngine;
using Warzone.Combat;
using Warzone.Content;
using Warzone.Content.Definitions;

namespace Warzone.Adapters
{
    public static class SandboxBattleFactory
    {
        public static ContentCatalog BuildSandboxContent()
        {
            AbilityDefinition fieldMedkit = new AbilityDefinition("ability.support.field_medkit", "Field Medkit", 8f, 6f, 4, "effect.support.heal");
            AbilityDefinition suppressiveBurst = new AbilityDefinition("ability.rifleman.suppressive_burst", "Suppressive Burst", 10f, 10f, 0, "effect.suppression");
            AbilityDefinition warlordRage = new AbilityDefinition("ability.warlord.rage", "Warlord Rage", 16f, 12f, 0, "effect.warlord.rage");

            WeaponDefinition playerRifle = new WeaponDefinition("weapon.assault_rifle", 14f, 0.65f, 4, 28f, DamageType.Kinetic);
            WeaponDefinition playerSniper = new WeaponDefinition("weapon.sniper_rifle", 24f, 1.6f, 9, 36f, DamageType.Piercing);
            WeaponDefinition playerGrenade = new WeaponDefinition("weapon.grenade_launcher", 16f, 2.1f, 8, 18f, DamageType.Explosive);
            WeaponDefinition playerMedic = new WeaponDefinition("weapon.smg", 9f, 0.45f, 3, 24f, DamageType.Kinetic);
            WeaponDefinition enemyMilitiaRifle = new WeaponDefinition("weapon.ak_rifle", 12f, 0.9f, 2, 22f, DamageType.Kinetic);
            WeaponDefinition enemyRpg = new WeaponDefinition("weapon.rpg7", 18f, 2.8f, 12, 14f, DamageType.Explosive);
            WeaponDefinition enemyVehicleMg = new WeaponDefinition("weapon.vehicle_mg", 16f, 0.35f, 4, 30f, DamageType.Kinetic);
            WeaponDefinition enemyHeavyMg = new WeaponDefinition("weapon.heavy_mg", 20f, 0.55f, 6, 34f, DamageType.Kinetic);
            Dictionary<string, UnitDefinition> units = new Dictionary<string, UnitDefinition>
            {
                ["unit.rifleman"] = new UnitDefinition("unit.rifleman", "Rifleman", FactionId.Player, 22, 5.5f, playerRifle, 18f, 0.5f, ArmorType.Light, activeAbilityId: suppressiveBurst.Id),
                ["unit.sniper"] = new UnitDefinition("unit.sniper", "Sniper", FactionId.Player, 12, 4.5f, playerSniper, 25f, 0.45f, ArmorType.Light),
                ["unit.grenadier"] = new UnitDefinition("unit.grenadier", "Grenadier", FactionId.Player, 28, 4f, playerGrenade, 15f, 0.55f, ArmorType.Medium),
                ["unit.medic"] = new UnitDefinition("unit.medic", "Medic", FactionId.Player, 16, 5.5f, playerMedic, 15f, 0.5f, ArmorType.Light, "effect.support.heal", fieldMedkit.Id),
                ["unit.militia"] = new UnitDefinition("unit.militia", "Militia", FactionId.Enemy, 10, 3f, enemyMilitiaRifle, 14f, 0.5f, ArmorType.Light),
                ["unit.rpg"] = new UnitDefinition("unit.rpg", "RPG Trooper", FactionId.Enemy, 14, 2.5f, enemyRpg, 16f, 0.5f, ArmorType.Light),
                ["unit.technical"] = new UnitDefinition("unit.technical", "Technical", FactionId.Enemy, 35, 8f, enemyVehicleMg, 20f, 1.2f, ArmorType.Medium),
                ["unit.warlord"] = new UnitDefinition("unit.warlord", "Warlord", FactionId.Enemy, 150, 3f, enemyHeavyMg, 30f, 0.8f, ArmorType.Heavy, "effect.warlord.rage", warlordRage.Id)
            };
            Dictionary<string, AbilityDefinition> abilities = new Dictionary<string, AbilityDefinition>
            {
                [fieldMedkit.Id] = fieldMedkit,
                [suppressiveBurst.Id] = suppressiveBurst,
                [warlordRage.Id] = warlordRage
            };

            Dictionary<string, MissionDefinition> missions = new Dictionary<string, MissionDefinition>
            {
                ["mission.sandbox"] = new MissionDefinition("mission.sandbox", "Industrial Cleanup", 1, 4)
            };

            return new ContentCatalog(units, missions, abilities);
        }

        public static BattleSession CreateSandboxBattle(ContentCatalog catalog, int seed)
        {
            List<BattleSquadState> squads = new List<BattleSquadState>
            {
                CreateSingleUnitSquad(1, FactionId.Player, "unit.rifleman", -14f, -4f, 22),
                CreateSingleUnitSquad(2, FactionId.Player, "unit.sniper", -16f, 2.5f, 12),
                CreateSingleUnitSquad(3, FactionId.Player, "unit.grenadier", -12f, 4.5f, 28),
                CreateSingleUnitSquad(4, FactionId.Player, "unit.medic", -13f, 0.5f, 16)
            };

            return new BattleSession(
                squads,
                new CommandProcessor(),
                new CombatResolver(catalog, TerrainMap.CreateDefault()),
                seed,
                TerrainMap.CreateDefault());
        }

        public static IReadOnlyList<SandboxWaveSpawnPlan> BuildWaves()
        {
            return new List<SandboxWaveSpawnPlan>
            {
                new SandboxWaveSpawnPlan(1, 1.5f, new[]
                {
                    CreateSingleUnitSquad(101, FactionId.Enemy, "unit.militia", 16f, -5f, 10),
                    CreateSingleUnitSquad(102, FactionId.Enemy, "unit.militia", 18f, -2f, 10),
                    CreateSingleUnitSquad(103, FactionId.Enemy, "unit.militia", 18f, 2f, 10),
                    CreateSingleUnitSquad(104, FactionId.Enemy, "unit.militia", 20f, 5f, 10)
                }),
                new SandboxWaveSpawnPlan(2, 4.5f, new[]
                {
                    CreateSingleUnitSquad(105, FactionId.Enemy, "unit.militia", 18f, -6f, 10),
                    CreateSingleUnitSquad(106, FactionId.Enemy, "unit.militia", 19f, -1f, 10),
                    CreateSingleUnitSquad(107, FactionId.Enemy, "unit.rpg", 22f, 3f, 14),
                    CreateSingleUnitSquad(108, FactionId.Enemy, "unit.technical", 24f, 0f, 35)
                }),
                new SandboxWaveSpawnPlan(3, 5.0f, new[]
                {
                    CreateSingleUnitSquad(109, FactionId.Enemy, "unit.militia", 20f, -7f, 10),
                    CreateSingleUnitSquad(110, FactionId.Enemy, "unit.militia", 23f, -3f, 10),
                    CreateSingleUnitSquad(111, FactionId.Enemy, "unit.rpg", 24f, 1f, 14),
                    CreateSingleUnitSquad(112, FactionId.Enemy, "unit.technical", 26f, 6f, 35)
                }),
                new SandboxWaveSpawnPlan(4, 8f, new[]
                {
                    CreateSingleUnitSquad(113, FactionId.Enemy, "unit.militia", 22f, -8f, 10),
                    CreateSingleUnitSquad(114, FactionId.Enemy, "unit.militia", 25f, -4f, 10),
                    CreateSingleUnitSquad(115, FactionId.Enemy, "unit.rpg", 26f, 0f, 14),
                    CreateSingleUnitSquad(116, FactionId.Enemy, "unit.technical", 28f, 4f, 35),
                    CreateSingleUnitSquad(117, FactionId.Enemy, "unit.warlord", 30f, 0f, 150)
                })
            };
        }

        private static BattleSquadState CreateSingleUnitSquad(int squadId, FactionId factionId, string definitionId, float x, float y, int health)
        {
            return new BattleSquadState(
                squadId,
                factionId,
                new System.Numerics.Vector2(x, y),
                new List<BattleUnitState>
                {
                    new BattleUnitState(new BattleEntityId(squadId), definitionId, factionId, health)
                });
        }
    }
}
