using System.Collections.Generic;
using Warzone.Content.Definitions;

namespace Warzone.Combat
{
    public sealed class BattleUnitState
    {
        private readonly List<ActiveStatusEffect> _statusEffects = new List<ActiveStatusEffect>();

        public BattleUnitState(
            BattleEntityId entityId,
            string definitionId,
            FactionId factionId,
            int currentHealth)
        {
            EntityId = entityId;
            DefinitionId = definitionId;
            FactionId = factionId;
            CurrentHealth = currentHealth;
        }

        public BattleEntityId EntityId { get; }
        public string DefinitionId { get; }
        public FactionId FactionId { get; }
        public int CurrentHealth { get; private set; }
        public bool IsAlive => CurrentHealth > 0;
        public IReadOnlyList<ActiveStatusEffect> StatusEffects => _statusEffects;

        public void ApplyDamage(int damage)
        {
            if (damage <= 0 || !IsAlive)
            {
                return;
            }

            CurrentHealth -= damage;
            if (CurrentHealth < 0)
            {
                CurrentHealth = 0;
            }
        }

        public void AddStatusEffect(ActiveStatusEffect statusEffect)
        {
            if (statusEffect == null)
            {
                return;
            }

            _statusEffects.Add(statusEffect);
        }

        public void TickStatusEffects(float deltaTimeSeconds)
        {
            for (int i = _statusEffects.Count - 1; i >= 0; i--)
            {
                ActiveStatusEffect effect = _statusEffects[i];
                effect.RemainingDuration -= deltaTimeSeconds;
                effect.TickTimer -= deltaTimeSeconds;

                if (effect.TickTimer <= 0f)
                {
                    ApplyDamage(effect.Definition.FlatDamagePerTick);
                    effect.TickTimer = effect.Definition.TickIntervalSeconds;
                }

                if (effect.RemainingDuration <= 0f)
                {
                    _statusEffects.RemoveAt(i);
                }
            }
        }
    }
}
