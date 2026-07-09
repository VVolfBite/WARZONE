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
            List<TacticalNodeSnapshot> tacticalNodes = new List<TacticalNodeSnapshot>();

            if (battleState != null)
            {
                foreach (BattleSquadState squadState in battleState.SquadsById.Values)
                {
                    int aliveMemberCount = 0;
                    for (int i = 0; i < squadState.MemberIds.Count; i++)
                    {
                        BattleMemberState memberState;
                        if (battleState.TryGetMember(squadState.MemberIds[i], out memberState) && memberState.IsAlive && !memberState.IsExtracted)
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
                        memberState.IsExtracted,
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
                        enemyState.IsAlive,
                        enemyState.CurrentTargetMemberId,
                        enemyState.AttackCooldownRemaining));
                }

                foreach (TacticalNodeState nodeState in battleState.TacticalNodesById.Values)
                {
                    tacticalNodes.Add(new TacticalNodeSnapshot(
                        nodeState.NodeId,
                        nodeState.NodeType,
                        nodeState.Position,
                        nodeState.Radius,
                        nodeState.IsEnabled,
                        nodeState.IsSearched,
                        nodeState.SearchProgress,
                        nodeState.RequiredSearchSeconds,
                        nodeState.OccupyingMemberId));
                }
            }

            return new BattleSnapshot(
                battleState != null ? battleState.BattleId : string.Empty,
                battleState != null ? battleState.ElapsedTimeSeconds : 0f,
                squads,
                members,
                enemies,
                tacticalNodes,
                CreateMissionStatus(battleState),
                battleState != null ? new List<BattleEventRecord>(battleState.RecentEvents) : new List<BattleEventRecord>());
        }

        private static BattleMissionStatusSnapshot CreateMissionStatus(BattleState battleState)
        {
            if (battleState == null)
            {
                return new BattleMissionStatusSnapshot(0, 0, 0, 0, 0, 0, false);
            }

            int aliveEnemyCount = 0;
            int searchedPointCount = 0;
            int totalSearchPointCount = 0;
            int extractedMemberCount = 0;
            int totalAliveMemberCount = 0;

            foreach (BattleEnemyState enemyState in battleState.EnemiesById.Values)
            {
                if (enemyState.IsAlive)
                {
                    aliveEnemyCount++;
                }
            }

            foreach (BattleMemberState memberState in battleState.MembersById.Values)
            {
                if (memberState.IsAlive)
                {
                    totalAliveMemberCount++;
                }

                if (memberState.IsExtracted)
                {
                    extractedMemberCount++;
                }
            }

            foreach (TacticalNodeState nodeState in battleState.TacticalNodesById.Values)
            {
                if (nodeState.NodeType != TacticalNodeType.SearchPoint)
                {
                    continue;
                }

                totalSearchPointCount++;
                if (nodeState.IsSearched)
                {
                    searchedPointCount++;
                }
            }

            bool allSearchesDone = totalSearchPointCount == 0 || searchedPointCount >= totalSearchPointCount;
            bool allEnemiesEliminated = aliveEnemyCount == 0;
            bool allAliveMembersExtracted = totalAliveMemberCount == 0 || extractedMemberCount >= totalAliveMemberCount;
            return new BattleMissionStatusSnapshot(
                aliveEnemyCount,
                battleState.EnemiesById.Count,
                searchedPointCount,
                totalSearchPointCount,
                extractedMemberCount,
                totalAliveMemberCount,
                allSearchesDone && allEnemiesEliminated && allAliveMembersExtracted);
        }
    }
}
