using Warzone.Core.Math;

namespace Warzone.Combat
{
    public sealed class MovementSystem
    {
        private readonly float _arrivalThreshold;

        public MovementSystem(float arrivalThreshold = 0.05f)
        {
            _arrivalThreshold = arrivalThreshold;
        }

        public void Execute(BattleState battleState, float deltaTimeSeconds)
        {
            if (battleState == null)
            {
                return;
            }

            foreach (BattleMemberState memberState in battleState.MembersById.Values)
            {
                MoveMember(battleState, memberState, deltaTimeSeconds);
            }

            foreach (BattleSquadState squadState in battleState.SquadsById.Values)
            {
                UpdateSquadCenter(battleState, squadState);
            }
        }

        private void MoveMember(BattleState battleState, BattleMemberState memberState, float deltaTimeSeconds)
        {
            if (memberState == null || !memberState.CanAct || memberState.CurrentIntent == null)
            {
                return;
            }

            ReleasePreviousOccupancy(battleState, memberState);

            if (memberState.CurrentIntent.IntentType == MemberIntentType.HoldPosition)
            {
                UpdateOccupiedNode(battleState, memberState);
                return;
            }

            if (memberState.CurrentIntent.IntentType != MemberIntentType.MoveToPosition &&
                memberState.CurrentIntent.IntentType != MemberIntentType.SearchPoint &&
                memberState.CurrentIntent.IntentType != MemberIntentType.Extract &&
                memberState.CurrentIntent.IntentType != MemberIntentType.TakeCover &&
                memberState.CurrentIntent.IntentType != MemberIntentType.Retreat)
            {
                return;
            }

            Vec2 delta = memberState.CurrentIntent.TargetPosition - memberState.Position;
            float distance = delta.Magnitude;
            if (distance <= _arrivalThreshold)
            {
                memberState.UpdatePosition(memberState.CurrentIntent.TargetPosition);
                CompleteIntent(memberState);
                UpdateOccupiedNode(battleState, memberState);
                battleState.AddEvent(new BattleEventRecord(BattleEventTypes.MemberReachedPosition, memberState.SquadId, memberState.MemberId));
                return;
            }

            Vec2 direction = Vec2.Normalize(delta);
            float stepDistance = SuppressionRule.ApplyMovementPenalty(memberState.MovementSpeed, memberState.IsSuppressed) * deltaTimeSeconds;
            if (stepDistance >= distance)
            {
                memberState.UpdatePosition(memberState.CurrentIntent.TargetPosition);
                CompleteIntent(memberState);
                memberState.UpdateFacing(direction);
                UpdateOccupiedNode(battleState, memberState);
                battleState.AddEvent(new BattleEventRecord(BattleEventTypes.MemberReachedPosition, memberState.SquadId, memberState.MemberId));
                return;
            }

            memberState.ClearOccupiedTacticalNode();
            memberState.UpdatePosition(memberState.Position + (direction * stepDistance));
            memberState.UpdateFacing(direction);
        }

        private static void CompleteIntent(BattleMemberState memberState)
        {
            if (memberState.CurrentIntent.IntentType == MemberIntentType.TakeCover)
            {
                memberState.SetIntent(new MemberIntent(MemberIntentType.HoldPosition, memberState.CurrentIntent.TargetPosition, true, memberState.CurrentIntent.TacticalNodeId));
                return;
            }

            memberState.CurrentIntent.MarkCompleted();
        }

        private static void ReleasePreviousOccupancy(BattleState battleState, BattleMemberState memberState)
        {
            if (battleState == null || memberState == null || !memberState.OccupiedTacticalNodeId.HasValue)
            {
                return;
            }

            if (memberState.CurrentIntent != null &&
                memberState.CurrentIntent.IsCompleted &&
                memberState.CurrentIntent.TacticalNodeId == memberState.OccupiedTacticalNodeId)
            {
                return;
            }

            TacticalNodeState nodeState;
            if (battleState.TryGetTacticalNode(memberState.OccupiedTacticalNodeId.Value, out nodeState) &&
                nodeState.OccupyingMemberId == memberState.MemberId)
            {
                nodeState.SetOccupyingMember(null);
            }

            memberState.ClearOccupiedTacticalNode();
        }

        private static void UpdateOccupiedNode(BattleState battleState, BattleMemberState memberState)
        {
            if (memberState.CurrentIntent != null &&
                memberState.CurrentIntent.IsCompleted &&
                memberState.CurrentIntent.TacticalNodeId.HasValue)
            {
                memberState.SetOccupiedTacticalNode(memberState.CurrentIntent.TacticalNodeId.Value);
                TacticalNodeState nodeState;
                if (battleState != null && battleState.TryGetTacticalNode(memberState.CurrentIntent.TacticalNodeId.Value, out nodeState))
                {
                    nodeState.SetOccupyingMember(memberState.MemberId);
                    if (nodeState.ReservedByMemberId == memberState.MemberId)
                    {
                        nodeState.ClearReservation();
                    }

                    if (nodeState.BuildingId.HasValue &&
                        battleState.MissionRuntimeState.MarkBuildingEntered(nodeState.BuildingId.Value))
                    {
                        battleState.AddEvent(new BattleEventRecord(BattleEventTypes.BuildingEntered, memberState.SquadId, memberState.MemberId, nodeState.BuildingId.Value.ToString()));
                    }
                }

                return;
            }

            memberState.ClearOccupiedTacticalNode();
        }

        private static void UpdateSquadCenter(BattleState battleState, BattleSquadState squadState)
        {
            Vec2 center = Vec2.Zero;
            int activeCount = 0;

            for (int i = 0; i < squadState.MemberIds.Count; i++)
            {
                BattleMemberState memberState;
                if (!battleState.TryGetMember(squadState.MemberIds[i], out memberState) || !memberState.IsAlive || memberState.IsExtracted)
                {
                    continue;
                }

                center += memberState.Position;
                activeCount++;
            }

            if (activeCount == 0)
            {
                return;
            }

            squadState.UpdatePosition(center / activeCount);
            if (AllMembersAtTargets(battleState, squadState))
            {
                squadState.SetStance(squadState.CurrentOrder is DefendAreaCommand ? SquadStance.Defending : SquadStance.Default);
            }
        }

        private static bool AllMembersAtTargets(BattleState battleState, BattleSquadState squadState)
        {
            for (int i = 0; i < squadState.MemberIds.Count; i++)
            {
                BattleMemberState memberState;
                if (!battleState.TryGetMember(squadState.MemberIds[i], out memberState) || !memberState.CanAct)
                {
                    continue;
                }

                if (memberState.CurrentIntent == null || !memberState.CurrentIntent.IsCompleted)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
