using System.Collections.Generic;
using Warzone.Core.Math;

namespace Warzone.Combat
{
    public static class BattleSnapshotFactory
    {
        public static BattleSnapshot Create(BattleState battleState)
        {
            List<BattleSquadSnapshot> squads = new List<BattleSquadSnapshot>();
            List<BattleMemberSnapshot> members = new List<BattleMemberSnapshot>();
            List<BattleEnemySnapshot> enemies = new List<BattleEnemySnapshot>();

            if (battleState != null)
            {
                foreach (BattleSquadState squadState in battleState.SquadsById.Values)
                {
                    int aliveMemberCount = 0;
                    for (int i = 0; i < squadState.MemberIds.Count; i++)
                    {
                        BattleMemberState memberState;
                        if (battleState.TryGetMember(squadState.MemberIds[i], out memberState) && memberState.IsAlive)
                        {
                            aliveMemberCount++;
                        }
                    }

                    squads.Add(new BattleSquadSnapshot(
                        squadState.SquadId,
                        squadState.FactionId,
                        squadState.Position,
                        squadState.DesiredPosition,
                        squadState.CurrentOrder != null ? squadState.CurrentOrder.Name : "None",
                        squadState.Stance,
                        squadState.MemberIds.Count,
                        aliveMemberCount,
                        squadState.FormationSpacing));
                }

                foreach (BattleMemberState memberState in battleState.MembersById.Values)
                {
                    members.Add(new BattleMemberSnapshot(
                        memberState.MemberId,
                        memberState.SquadId,
                        memberState.Position,
                        memberState.Facing,
                        memberState.Health,
                        memberState.MaxHealth,
                        memberState.IsAlive,
                        memberState.WeaponId,
                        memberState.CurrentTargetEnemyId,
                        memberState.AttackCooldownRemaining,
                        memberState.CurrentIntent != null ? memberState.CurrentIntent.IntentType.ToString() : "None",
                        memberState.CurrentIntent != null ? memberState.CurrentIntent.TargetPosition : (Vec2?)null,
                        memberState.CurrentIntent != null && memberState.CurrentIntent.IsCompleted));
                }

                foreach (BattleEnemyState enemyState in battleState.EnemiesById.Values)
                {
                    enemies.Add(new BattleEnemySnapshot(
                        enemyState.EnemyId,
                        enemyState.DefinitionId,
                        enemyState.FactionId,
                        enemyState.Position,
                        enemyState.Health,
                        enemyState.MaxHealth,
                        enemyState.IsAlive));
                }
            }

            return new BattleSnapshot(
                battleState != null ? battleState.BattleId : string.Empty,
                battleState != null ? battleState.ElapsedTimeSeconds : 0f,
                squads,
                members,
                enemies,
                battleState != null ? new List<BattleEventRecord>(battleState.RecentEvents) : new List<BattleEventRecord>());
        }
    }
}
