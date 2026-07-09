using Warzone.Core.Math;

namespace Warzone.Combat
{
    public sealed class EnemyAwarenessSystem
    {
        public void Execute(BattleState battleState)
        {
            if (battleState == null)
            {
                return;
            }

            foreach (BattleEnemyState enemyState in battleState.EnemiesById.Values)
            {
                if (enemyState == null || !enemyState.IsAlive)
                {
                    continue;
                }

                BattleMemberState nearestMember = null;
                float nearestDistance = float.MaxValue;

                foreach (BattleMemberState memberState in battleState.MembersById.Values)
                {
                    if (memberState == null || !memberState.CanAct || memberState.FactionId == enemyState.FactionId)
                    {
                        continue;
                    }

                    float distance = Vec2.Distance(enemyState.Position, memberState.Position);
                    if (distance > enemyState.DetectionRange || distance >= nearestDistance)
                    {
                        continue;
                    }

                    LineOfSightResult lineOfSight = LineOfSightRule.Evaluate(battleState, enemyState.Position, memberState.Position);
                    if (!lineOfSight.HasLineOfSight)
                    {
                        continue;
                    }

                    nearestDistance = distance;
                    nearestMember = memberState;
                }

                enemyState.SetCurrentTargetMember(nearestMember != null ? (BattleEntityId?)nearestMember.MemberId : null);
            }
        }
    }
}
