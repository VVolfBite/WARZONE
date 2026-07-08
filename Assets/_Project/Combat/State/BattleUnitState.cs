using System.Collections.Generic;
using Warzone.Content.Definitions;

namespace Warzone.Combat
{
    public sealed class BattleUnitState
    {
        private readonly List<ActiveStatusEffect> _statusEffects = new List<ActiveStatusEffect>();
        private readonly int _maxHealth;

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
            _maxHealth = currentHealth;
        }

        public BattleEntityId EntityId { get; private set; }
        public string DefinitionId { get; private set; }
        public FactionId FactionId { get; private set; }
        public int CurrentHealth { get; private set; }

        public bool IsAlive
        {
            get { return CurrentHealth > 0; }
        }

        public IReadOnlyList<ActiveStatusEffect> StatusEffects
        {
            get { return _statusEffects; }
        }

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

        public void ApplyHealing(int healing)
        {
            if (healing <= 0 || !IsAlive)
            {
                return;
            }

            CurrentHealth += healing;
            if (CurrentHealth > _maxHealth)
            {
                CurrentHealth = _maxHealth;
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

        public bool HasStatusEffect(string effectId)
        {
            if (string.IsNullOrEmpty(effectId))
            {
                return false;
            }

            for (int i = 0; i < _statusEffects.Count; i++)
            {
                if (_statusEffects[i].Definition.Id == effectId)
                {
                    return true;
                }
            }

            return false;
        }

        public void RefreshStatusEffect(StatusEffectDefinition definition)
        {
            if (definition == null)
            {
                return;
            }

            for (int i = 0; i < _statusEffects.Count; i++)
            {
                ActiveStatusEffect effect = _statusEffects[i];
                if (effect.Definition.Id != definition.Id)
                {
                    continue;
                }

                effect.RemainingDuration = definition.DurationSeconds;
                effect.TickTimer = definition.TickIntervalSeconds;
                return;
            }
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
                    ApplyHealing(effect.Definition.FlatHealingPerTick);
                    effect.TickTimer = effect.Definition.TickIntervalSeconds;
                }

                if (effect.RemainingDuration <= 0f)
                {
                    _statusEffects.RemoveAt(i);
                }
            }
        }

        public float GetMoveSpeedMultiplier()
        {
            float multiplier = 1f;
            for (int i = 0; i < _statusEffects.Count; i++)
            {
                multiplier *= _statusEffects[i].Definition.MoveSpeedMultiplier;
            }

            return multiplier;
        }

        public float GetRangeMultiplier()
        {
            float multiplier = 1f;
            for (int i = 0; i < _statusEffects.Count; i++)
            {
                multiplier *= _statusEffects[i].Definition.RangeMultiplier;
            }

            return multiplier;
        }
    }
}

