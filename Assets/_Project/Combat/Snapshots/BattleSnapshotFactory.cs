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
            List<TacticalObstacleSnapshot> obstacles = new List<TacticalObstacleSnapshot>();
            List<BuildingSnapshot> buildings = new List<BuildingSnapshot>();

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
                        memberState.OccupiedTacticalNodeId,
                        memberState.WeaponId,
                        memberState.CurrentTargetEnemyId,
                        memberState.AttackCooldownRemaining,
                        memberState.CurrentIntent != null ? memberState.CurrentIntent.IntentType.ToString() : "None",
                        memberState.CurrentIntent != null ? memberState.CurrentIntent.TargetPosition : (Vec2?)null,
                        memberState.CurrentIntent != null && memberState.CurrentIntent.IsCompleted,
                        memberState.Pressure,
                        memberState.MaxPressure,
                        memberState.IsSuppressed,
                        memberState.IsBroken,
                        memberState.IsRetreating,
                        memberState.RetreatTargetPosition));
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
                        enemyState.OccupiedTacticalNodeId,
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

                foreach (TacticalObstacleState obstacleState in battleState.ObstaclesById.Values)
                {
                    obstacles.Add(new TacticalObstacleSnapshot(
                        obstacleState.ObstacleId,
                        obstacleState.ObstacleType,
                        obstacleState.Position,
                        obstacleState.Radius,
                        obstacleState.BlocksLineOfSight,
                        obstacleState.BlocksFire,
                        obstacleState.ProvidesCover,
                        obstacleState.DamageReductionFactor,
                        obstacleState.IsDestroyed));
                }

                foreach (BuildingState buildingState in battleState.BuildingsById.Values)
                {
                    buildings.Add(new BuildingSnapshot(
                        buildingState.BuildingId,
                        buildingState.Position,
                        buildingState.Radius,
                        buildingState.IsEnterable,
                        new List<int>(buildingState.TacticalNodeIds)));
                }
            }

            return new BattleSnapshot(
                battleState != null ? battleState.BattleId : string.Empty,
                battleState != null ? battleState.ElapsedTimeSeconds : 0f,
                squads,
                members,
                enemies,
                tacticalNodes,
                obstacles,
                buildings,
                battleState != null ? battleState.CurrentMissionStatus : new BattleMissionStatusSnapshot(0, 0, 0, 0, 0, 0, false, false, false, false, false, BattleCompletionType.Partial, 0),
                battleState != null ? battleState.CurrentBattleResult : null,
                battleState != null ? new List<BattleEventRecord>(battleState.RecentEvents) : new List<BattleEventRecord>());
        }
    }
}
