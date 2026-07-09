using Warzone.Core.Math;

namespace Warzone.Combat
{
    public sealed class RetreatSystem
    {
        public void Execute(BattleState battleState)
        {
            if (battleState == null)
            {
                return;
            }

            foreach (BattleMemberState memberState in battleState.MembersById.Values)
            {
                if (memberState == null || !memberState.CanAct || !memberState.IsBroken)
                {
                    continue;
                }

                int? tacticalNodeId = null;
                Vec2 retreatTargetPosition = ResolveRetreatTarget(battleState, memberState, out tacticalNodeId);
                if (!memberState.IsRetreating)
                {
                    memberState.BeginRetreat(retreatTargetPosition);
                    battleState.AddEvent(new BattleEventRecord(
                        BattleEventTypes.MemberStartedRetreat,
                        memberState.SquadId,
                        memberState.MemberId,
                        tacticalNodeId.HasValue ? tacticalNodeId.Value.ToString() : "fallback"));
                }
                else
                {
                    memberState.SetRetreatTargetPosition(retreatTargetPosition);
                }

                memberState.SetIntent(new MemberIntent(MemberIntentType.Retreat, retreatTargetPosition, false, tacticalNodeId));
                memberState.SetCurrentTargetEnemy(null);
            }
        }

        private static Vec2 ResolveRetreatTarget(BattleState battleState, BattleMemberState memberState, out int? tacticalNodeId)
        {
            TacticalNodeState safeNode = FindNearestTacticalNodeQuery.Execute(
                battleState,
                memberState.Position,
                TacticalNodeType.RallyPoint,
                TacticalNodeType.ExtractionPoint);

            if (safeNode != null)
            {
                tacticalNodeId = safeNode.NodeId;
                return safeNode.Position;
            }

            BattleEnemyState nearestEnemy = null;
            float nearestDistance = float.MaxValue;
            foreach (BattleEnemyState enemyState in battleState.EnemiesById.Values)
            {
                if (enemyState == null || !enemyState.IsAlive)
                {
                    continue;
                }

                float distance = Vec2.Distance(memberState.Position, enemyState.Position);
                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearestEnemy = enemyState;
                }
            }

            tacticalNodeId = null;
            return nearestEnemy != null
                ? BreakRetreatRule.BuildFallbackRetreatTarget(memberState.Position, nearestEnemy.Position)
                : memberState.Position + new Vec2(-BreakRetreatRule.FallbackRetreatDistance, 0f);
        }
    }
}
