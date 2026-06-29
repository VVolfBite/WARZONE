using System.Collections.Generic;

namespace Warzone.Combat
{
    public sealed class StatusEffectSystem
    {
        public void Tick(BattleSession battleSession, float deltaTimeSeconds)
        {
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

            unit.AddStatusEffect(new ActiveStatusEffect(definition));
        }
    }
}
