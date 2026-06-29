using System.Collections.Generic;
using System.Numerics;
using Warzone.Content.Definitions;

namespace Warzone.Combat
{
    public sealed class AiSystem
    {
        private float _tickTimer;
        private readonly AiProfile _defaultProfile = new AiProfile(0.5f, 11f, 22f);

        public void Tick(BattleSession battleSession, float deltaTimeSeconds)
        {
            _tickTimer += deltaTimeSeconds;
            if (_tickTimer < _defaultProfile.UpdateIntervalSeconds)
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

                AiBlackboard blackboard = BuildBlackboard(battleSession, squad);
                if (!blackboard.HasTarget)
                {
                    continue;
                }

                if (blackboard.DistanceToTarget <= 3.5f)
                {
                    battleSession.ExecuteCommand(new Command(CommandType.Attack, squad.SquadId, blackboard.TargetSquadId));
                }
                else
                {
                    battleSession.ExecuteCommand(new Command(CommandType.Move, squad.SquadId, destination: blackboard.TargetPosition));
                }
            }
        }

        private AiBlackboard BuildBlackboard(BattleSession battleSession, BattleSquadState sourceSquad)
        {
            AiBlackboard blackboard = new AiBlackboard
            {
                SelfSquadId = sourceSquad.SquadId,
                SelfPosition = sourceSquad.Position
            };

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

            if (nearest != null)
            {
                blackboard.TargetSquadId = nearest.SquadId;
                blackboard.TargetPosition = nearest.Position;
                blackboard.DistanceToTarget = nearestDistance;
                blackboard.CurrentLeashDistance = Vector2.Distance(sourceSquad.Position, nearest.Position);
            }

            return blackboard;
        }
    }
}
