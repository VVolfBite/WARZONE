using System.Collections.Generic;
using Warzone.Content.Definitions;

namespace Warzone.Combat
{
    public sealed class AiSystem
    {
        private float _tickTimer;

        public void Tick(BattleSession battleSession, float deltaTimeSeconds)
        {
            _tickTimer += deltaTimeSeconds;
            if (_tickTimer < 0.5f)
            {
                return;
            }

            _tickTimer = 0f;

            for (int i = 0; i < battleSession.Squads.Count; i++)
            {
                BattleSquadState squad = battleSession.Squads[i];
                if (squad.FactionId != FactionId.Enemy || !squad.HasLivingUnits)
                {
                    continue;
                }

                BattleSquadState target = FindNearestPlayerSquad(battleSession, squad);
                if (target == null)
                {
                    continue;
                }

                float distance = CombatResolver.GetDistance(squad, target);
                if (distance <= 3f)
                {
                    battleSession.ExecuteCommand(new Command(CommandType.Attack, squad.SquadId, target.SquadId));
                }
                else
                {
                    battleSession.ExecuteCommand(new Command(CommandType.Move, squad.SquadId, destination: target.Position));
                }
            }
        }

        private static BattleSquadState FindNearestPlayerSquad(BattleSession battleSession, BattleSquadState sourceSquad)
        {
            BattleSquadState nearest = null;
            float nearestDistance = float.MaxValue;

            for (int i = 0; i < battleSession.Squads.Count; i++)
            {
                BattleSquadState squad = battleSession.Squads[i];
                if (squad.FactionId != FactionId.Player || !squad.HasLivingUnits)
                {
                    continue;
                }

                float distance = CombatResolver.GetDistance(sourceSquad, squad);
                if (distance < nearestDistance)
                {
                    nearest = squad;
                    nearestDistance = distance;
                }
            }

            return nearest;
        }
    }
}
