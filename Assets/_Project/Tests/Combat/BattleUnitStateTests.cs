using NUnit.Framework;
using Warzone.Combat;
using Warzone.Content.Definitions;

namespace Warzone.Tests.Combat
{
    public sealed class BattleUnitStateTests
    {
        [Test]
        public void StatusEffects_StackMoveAndRangeMultipliers()
        {
            BattleUnitState unit = new BattleUnitState(new BattleEntityId(1), "unit.test", FactionId.Player, 10);
            unit.AddStatusEffect(new ActiveStatusEffect(new StatusEffectDefinition("slow", "Slow", 5f, 1f, 0, 0, 0.8f, 1f)));
            unit.AddStatusEffect(new ActiveStatusEffect(new StatusEffectDefinition("aura", "Aura", 5f, 1f, 0, 0, 1f, 0.9f)));

            Assert.That(unit.GetMoveSpeedMultiplier(), Is.EqualTo(0.8f).Within(0.0001f));
            Assert.That(unit.GetRangeMultiplier(), Is.EqualTo(0.9f).Within(0.0001f));
        }
    }
}
