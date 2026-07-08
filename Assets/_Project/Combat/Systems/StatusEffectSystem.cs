using System.Collections.Generic;
using Warzone.Content.Definitions;

namespace Warzone.Combat
{
    public sealed class StatusEffectSystem
    {
        public void Tick(BattleSession battleSession, float deltaTimeSeconds)
        {
            ApplyAuraEffects(battleSession);

            for (int i = 0; i < battleSession.Squads.Count; i++)
            {
                BattleSquadState squad = battleSession.Squads[i];
                for (int j = 0; j < squad.Units.Count; j++)
                {
                    BattleUnitState unit = squad.Units[j];
                    unit.TickStatusEffects(deltaTimeSeconds);
                }
            }

            battleSession.SyncStatusEffects();
        }

        public void ApplyEffect(BattleUnitState unit, StatusEffectDefinition definition)
        {
            if (unit == null || definition == null)
            {
                return;
            }

            if (unit.HasStatusEffect(definition.Id))
            {
                unit.RefreshStatusEffect(definition);
                return;
            }

            unit.AddStatusEffect(new ActiveStatusEffect(definition));
        }

        public void ApplyHealingAura(BattleUnitState unit, int healingAmount)
        {
            if (unit == null)
            {
                return;
            }

            unit.ApplyHealing(healingAmount);
        }

        private void ApplyAuraEffects(BattleSession battleSession)
        {
            const float SupportHealRadius = 4.5f;
            const float ToxicAuraRadius = 3.5f;

            for (int i = 0; i < battleSession.Squads.Count; i++)
            {
                BattleSquadState sourceSquad = battleSession.Squads[i];
                if (!sourceSquad.HasLivingUnits)
                {
                    continue;
                }

                UnitDefinition sourceDefinition = battleSession.GetPrimaryDefinition(sourceSquad);
                if (sourceDefinition == null || string.IsNullOrEmpty(sourceDefinition.DefaultStatusEffectId))
                {
                    continue;
                }

                if (sourceDefinition.DefaultStatusEffectId == "effect.support.heal")
                {
                    ApplySupportAura(battleSession, sourceSquad, SupportHealRadius);
                    continue;
                }

                if (sourceDefinition.DefaultStatusEffectId == "effect.zombie.toxic")
                {
                    ApplyToxicAura(battleSession, sourceSquad, ToxicAuraRadius, sourceDefinition.DefaultStatusEffectId);
                }
            }
        }

        private void ApplySupportAura(BattleSession battleSession, BattleSquadState sourceSquad, float radius)
        {
            for (int i = 0; i < battleSession.Squads.Count; i++)
            {
                BattleSquadState targetSquad = battleSession.Squads[i];
                if (targetSquad.FactionId != sourceSquad.FactionId || !targetSquad.HasLivingUnits)
                {
                    continue;
                }

                if (CombatResolver.GetDistance(sourceSquad, targetSquad) > radius)
                {
                    continue;
                }

                for (int j = 0; j < targetSquad.Units.Count; j++)
                {
                    ApplyHealingAura(targetSquad.Units[j], 1);
                }
            }
        }

        private void ApplyToxicAura(BattleSession battleSession, BattleSquadState sourceSquad, float radius, string effectId)
        {
            StatusEffectDefinition definition;
            if (!battleSession.TryGetStatusEffectDefinition(effectId, out definition))
            {
                return;
            }

            for (int i = 0; i < battleSession.Squads.Count; i++)
            {
                BattleSquadState targetSquad = battleSession.Squads[i];
                if (targetSquad.FactionId == sourceSquad.FactionId || !targetSquad.HasLivingUnits)
                {
                    continue;
                }

                if (CombatResolver.GetDistance(sourceSquad, targetSquad) > radius)
                {
                    continue;
                }

                for (int j = 0; j < targetSquad.Units.Count; j++)
                {
                    ApplyEffect(targetSquad.Units[j], definition);
                }
            }
        }
    }
}


