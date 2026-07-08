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

                AiDecision decision = EvaluateDecision(blackboard);
                ExecuteDecision(battleSession, squad, blackboard, decision);
            }
        }

        private static AiDecision EvaluateDecision(AiBlackboard blackboard)
        {
            if (!blackboard.HasTarget)
            {
                return AiDecision.Idle;
            }

            if (blackboard.SelfDefinitionId == "unit.rpg")
            {
                if (blackboard.DistanceToTarget < 8f)
                {
                    return AiDecision.Retreat;
                }

                return blackboard.DistanceToTarget <= 16f ? AiDecision.AttackTarget : AiDecision.MoveToTarget;
            }

            if (blackboard.SelfDefinitionId == "unit.technical")
            {
                return blackboard.DistanceToTarget <= 18f ? AiDecision.Strafe : AiDecision.MoveToTarget;
            }

            if (blackboard.SelfDefinitionId == "unit.warlord")
            {
                if (blackboard.HealthRatio <= 0.35f)
                {
                    return AiDecision.UseAbility;
                }

                return blackboard.DistanceToTarget <= 20f ? AiDecision.AttackTarget : AiDecision.MoveToTarget;
            }

            return blackboard.DistanceToTarget <= 12f ? AiDecision.AttackTarget : AiDecision.MoveToTarget;
        }

        private static void ExecuteDecision(BattleSession battleSession, BattleSquadState squad, AiBlackboard blackboard, AiDecision decision)
        {
            switch (decision)
            {
                case AiDecision.AttackTarget:
                    battleSession.ExecuteCommand(new Command(CommandType.Attack, squad.SquadId, blackboard.TargetSquadId));
                    break;
                case AiDecision.MoveToTarget:
                    battleSession.ExecuteCommand(new Command(CommandType.Move, squad.SquadId, destination: blackboard.TargetPosition));
                    break;
                case AiDecision.Retreat:
                    Vector2 retreatDirection = Vector2.Normalize(blackboard.SelfPosition - blackboard.TargetPosition);
                    battleSession.ExecuteCommand(new Command(CommandType.Move, squad.SquadId, destination: blackboard.SelfPosition + (retreatDirection * 6f)));
                    break;
                case AiDecision.Strafe:
                    Vector2 offset = blackboard.TargetPosition - blackboard.SelfPosition;
                    Vector2 strafeDirection = Vector2.Normalize(new Vector2(-offset.Y, offset.X));
                    battleSession.ExecuteCommand(new Command(CommandType.Move, squad.SquadId, destination: blackboard.TargetPosition + (strafeDirection * 5f)));
                    break;
                case AiDecision.UseAbility:
                    UnitDefinition definition = battleSession.GetPrimaryDefinition(squad);
                    if (definition != null && !string.IsNullOrEmpty(definition.ActiveAbilityId))
                    {
                        battleSession.ExecuteCommand(new Command(CommandType.UseAbility, squad.SquadId, abilityId: definition.ActiveAbilityId));
                    }
                    else
                    {
                        battleSession.ExecuteCommand(new Command(CommandType.Attack, squad.SquadId, blackboard.TargetSquadId));
                    }
                    break;
            }
        }

        private AiBlackboard BuildBlackboard(BattleSession battleSession, BattleSquadState sourceSquad)
        {
            AiBlackboard blackboard = new AiBlackboard
            {
                SelfSquadId = sourceSquad.SquadId,
                SelfPosition = sourceSquad.Position,
                SelfDefinitionId = sourceSquad.Units.Count > 0 ? sourceSquad.Units[0].DefinitionId : string.Empty,
                HealthRatio = BuildHealthRatio(battleSession, sourceSquad)
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

        private static float BuildHealthRatio(BattleSession battleSession, BattleSquadState sourceSquad)
        {
            if (sourceSquad.Units.Count == 0)
            {
                return 1f;
            }

            BattleUnitState unit = sourceSquad.Units[0];
            int maxHealth = battleSession.GetMaxHealth(unit);
            if (maxHealth <= 0)
            {
                return 1f;
            }

            return (float)unit.CurrentHealth / maxHealth;
        }
    }
}


