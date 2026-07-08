using System;
using Warzone.Content;
using Warzone.Content.Definitions;

namespace Warzone.Combat
{
    public sealed class AbilitySystem
    {
        private readonly ContentCatalog _contentCatalog;

        public AbilitySystem(ContentCatalog contentCatalog)
        {
            _contentCatalog = contentCatalog;
        }

        public bool TryUseAbility(BattleSession battleSession, BattleSquadState sourceSquad, string abilityId)
        {
            if (battleSession == null || sourceSquad == null || string.IsNullOrEmpty(abilityId))
            {
                return false;
            }

            if (sourceSquad.AbilityCooldownRemaining > 0f)
            {
                return false;
            }

            AbilityDefinition ability;
            if (!_contentCatalog.Abilities.TryGetValue(abilityId, out ability))
            {
                return false;
            }

            if (ability.HealAmount > 0)
            {
                if (!TryHealFriendlySquad(battleSession, sourceSquad, ability))
                {
                    return false;
                }
            }
            else if (!string.IsNullOrEmpty(ability.AppliedStatusEffectId))
            {
                StatusEffectDefinition definition;
                if (!battleSession.TryGetStatusEffectDefinition(ability.AppliedStatusEffectId, out definition))
                {
                    return false;
                }

                for (int i = 0; i < sourceSquad.Units.Count; i++)
                {
                    battleSession.ApplyStatusEffect(sourceSquad.Units[i], definition);
                }
            }

            sourceSquad.ResetAbilityCooldown(ability.CooldownSeconds);
            return true;
        }

        private bool TryHealFriendlySquad(BattleSession battleSession, BattleSquadState sourceSquad, AbilityDefinition ability)
        {
            BattleSquadState target = FindNearestDamagedFriendlySquad(battleSession, sourceSquad, ability.Range);
            if (target == null)
            {
                return false;
            }

            for (int i = 0; i < target.Units.Count; i++)
            {
                target.Units[i].ApplyHealing(ability.HealAmount);
            }

            StatusEffectDefinition effectDefinition;
            if (!string.IsNullOrEmpty(ability.AppliedStatusEffectId) &&
                battleSession.TryGetStatusEffectDefinition(ability.AppliedStatusEffectId, out effectDefinition))
            {
                for (int i = 0; i < target.Units.Count; i++)
                {
                    battleSession.ApplyStatusEffect(target.Units[i], effectDefinition);
                }
            }

            return true;
        }

        private static BattleSquadState FindNearestDamagedFriendlySquad(BattleSession battleSession, BattleSquadState sourceSquad, float range)
        {
            BattleSquadState nearest = null;
            float nearestDistance = float.MaxValue;

            for (int i = 0; i < battleSession.Squads.Count; i++)
            {
                BattleSquadState candidate = battleSession.Squads[i];
                if (candidate.FactionId != sourceSquad.FactionId || !candidate.HasLivingUnits)
                {
                    continue;
                }

                bool hasDamage = false;
                for (int j = 0; j < candidate.Units.Count; j++)
                {
                    if (candidate.Units[j].CurrentHealth < battleSession.GetMaxHealth(candidate.Units[j]))
                    {
                        hasDamage = true;
                        break;
                    }
                }

                if (!hasDamage)
                {
                    continue;
                }

                float distance = CombatResolver.GetDistance(sourceSquad, candidate);
                if (distance > range || distance >= nearestDistance)
                {
                    continue;
                }

                nearest = candidate;
                nearestDistance = distance;
            }

            return nearest;
        }
    }
}


