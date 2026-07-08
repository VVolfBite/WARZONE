using System.Collections.Generic;
using NUnit.Framework;
using Warzone.Combat;
using Warzone.Content;
using Warzone.Content.Definitions;
using Warzone.Core.Math;

namespace Warzone.Tests.Combat
{
    public sealed class AbilitySystemTests
    {
        [Test]
        public void TryUseAbility_WhenFriendlyUnitIsDamaged_HealsTargetAndStartsCooldown()
        {
            AbilityDefinition medkit = new AbilityDefinition("ability.medkit", "Medkit", 8f, 5f, 4);
            WeaponDefinition rifle = new WeaponDefinition("weapon.player", 4f, 1f, 3, 16f, DamageType.Piercing);

            ContentCatalog catalog = new ContentCatalog(
                new Dictionary<string, UnitDefinition>
                {
                    ["unit.support"] = new UnitDefinition("unit.support", "Support", FactionId.Player, 12, 4f, rifle, 8f, 0.65f, ArmorType.Light, activeAbilityId: "ability.medkit"),
                    ["unit.friend"] = new UnitDefinition("unit.friend", "Friend", FactionId.Player, 12, 4f, rifle, 8f, 0.65f, ArmorType.Light)
                },
                new Dictionary<string, MissionDefinition>(),
                new Dictionary<string, AbilityDefinition>
                {
                    [medkit.Id] = medkit
                });

            BattleSquadState support = new BattleSquadState(
                1,
                FactionId.Player,
                new Vec2(0f, 0f),
                new List<BattleUnitState>
                {
                    new BattleUnitState(new BattleEntityId(1), "unit.support", FactionId.Player, 12)
                });
            BattleUnitState damagedUnit = new BattleUnitState(new BattleEntityId(2), "unit.friend", FactionId.Player, 12);
            damagedUnit.ApplyDamage(5);
            BattleSquadState ally = new BattleSquadState(
                2,
                FactionId.Player,
                new Vec2(2f, 0f),
                new List<BattleUnitState>
                {
                    damagedUnit
                });

            BattleSession session = new BattleSession(
                new[] { support, ally },
                new CommandProcessor(),
                new CombatResolver(catalog, TerrainMap.CreateDefault()),
                seed: 5,
                terrainMap: TerrainMap.CreateDefault());

            AbilitySystem abilitySystem = new AbilitySystem(catalog);
            bool result = abilitySystem.TryUseAbility(session, support, "ability.medkit");

            Assert.That(result, Is.True);
            Assert.That(damagedUnit.CurrentHealth, Is.EqualTo(11));
            Assert.That(support.AbilityCooldownRemaining, Is.EqualTo(8f).Within(0.001f));
        }
    }
}



