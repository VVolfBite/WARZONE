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
            if (memberState == null || !memberState.IsAlive || memberState.CurrentIntent == null)
            {
                return;
            }

            if (memberState.CurrentIntent.IntentType != MemberIntentType.MoveToPosition)
            {
                return;
            }

            Vec2 delta = memberState.CurrentIntent.TargetPosition - memberState.Position;
            float distance = delta.Magnitude;
            if (distance <= _arrivalThreshold)
            {
                memberState.UpdatePosition(memberState.CurrentIntent.TargetPosition);
                memberState.CurrentIntent.MarkCompleted();
                battleState.AddEvent(new BattleEventRecord(BattleEventTypes.MemberReachedPosition, memberState.SquadId, memberState.MemberId));
                return;
            }

            Vec2 direction = Vec2.Normalize(delta);
            float stepDistance = memberState.MovementSpeed * deltaTimeSeconds;
            if (stepDistance >= distance)
            {
                memberState.UpdatePosition(memberState.CurrentIntent.TargetPosition);
                memberState.CurrentIntent.MarkCompleted();
                memberState.UpdateFacing(direction);
                battleState.AddEvent(new BattleEventRecord(BattleEventTypes.MemberReachedPosition, memberState.SquadId, memberState.MemberId));
                return;
            }

            memberState.UpdatePosition(memberState.Position + (direction * stepDistance));
            memberState.UpdateFacing(direction);
        }

        private static void UpdateSquadCenter(BattleState battleState, BattleSquadState squadState)
        {
            Vec2 center = Vec2.Zero;
            int aliveCount = 0;

            for (int i = 0; i < squadState.MemberIds.Count; i++)
            {
                BattleMemberState memberState;
                if (!battleState.TryGetMember(squadState.MemberIds[i], out memberState) || !memberState.IsAlive)
                {
                    continue;
                }

                center += memberState.Position;
                aliveCount++;
            }

            if (aliveCount == 0)
            {
                return;
            }

            squadState.UpdatePosition(center / aliveCount);
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
                if (!battleState.TryGetMember(squadState.MemberIds[i], out memberState) || !memberState.IsAlive)
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


