using System.Numerics;

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

            Vector2 delta = memberState.CurrentIntent.TargetPosition - memberState.Position;
            float distance = delta.Length();
            if (distance <= _arrivalThreshold)
            {
                memberState.UpdatePosition(memberState.CurrentIntent.TargetPosition);
                memberState.CurrentIntent.MarkCompleted();
                battleState.EventBuffer.Add(new BattleEventRecord("MemberReachedTarget", memberState.SquadId, memberState.MemberId));
                return;
            }

            Vector2 direction = Vector2.Normalize(delta);
            float stepDistance = memberState.MovementSpeed * deltaTimeSeconds;
            if (stepDistance >= distance)
            {
                memberState.UpdatePosition(memberState.CurrentIntent.TargetPosition);
                memberState.CurrentIntent.MarkCompleted();
                memberState.UpdateFacing(direction);
                battleState.EventBuffer.Add(new BattleEventRecord("MemberReachedTarget", memberState.SquadId, memberState.MemberId));
                return;
            }

            memberState.UpdatePosition(memberState.Position + (direction * stepDistance));
            memberState.UpdateFacing(direction);
        }

        private static void UpdateSquadCenter(BattleState battleState, BattleSquadState squadState)
        {
            Vector2 center = Vector2.Zero;
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
