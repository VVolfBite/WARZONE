using NUnit.Framework;
using Warzone.Combat;
using Warzone.Content.Definitions;

namespace Warzone.Tests.Combat
{
    public sealed class StatusEffectSystemTests
    {
        [Test]
        public void Tick_WhenEffectExpires_RemovesEffectAndAppliesDamage()
        {
            BattleUnitState unit = new BattleUnitState(new BattleEntityId(1), "unit.test", FactionId.Player, 10);
            StatusEffectDefinition definition = new StatusEffectDefinition("effect.test", "Test", 1f, 0.5f, 2, 0, 1f, 1f);

            unit.AddStatusEffect(new ActiveStatusEffect(definition));
            unit.TickStatusEffects(0.5f);
            unit.TickStatusEffects(0.5f);

            Assert.That(unit.CurrentHealth, Is.EqualTo(8));
            Assert.That(unit.StatusEffects.Count, Is.EqualTo(0));
        }

        [Test]
        public void ApplyHealingAura_IncreasesHealth()
        {
            BattleUnitState unit = new BattleUnitState(new BattleEntityId(1), "unit.test", FactionId.Player, 10);
            StatusEffectSystem system = new StatusEffectSystem();

            system.ApplyHealingAura(unit, 3);

            Assert.That(unit.CurrentHealth, Is.EqualTo(13));
        }
    }
}
