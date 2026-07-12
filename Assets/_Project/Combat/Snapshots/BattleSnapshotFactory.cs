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
            List<EnvironmentalZoneSnapshot> zones = new List<EnvironmentalZoneSnapshot>();

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
                        memberState.RetreatTargetPosition,
                        memberState.NightVisionLevel,
                        memberState.SmokeVisionLevel,
                        memberState.HasLightSource,
                        memberState.DetectionRange,
                        memberState.EffectiveDetectionRange));
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
                        enemyState.AttackCooldownRemaining,
                        enemyState.NightVisionLevel,
                        enemyState.HasLightSource,
                        enemyState.DetectionRange,
                        enemyState.EffectiveDetectionRange));
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
                        nodeState.OccupyingMemberId,
                        nodeState.ReservedByMemberId,
                        nodeState.BuildingId,
                        nodeState.IsInsideBuilding,
                        nodeState.AllowsFireThrough,
                        nodeState.AllowsVisionThrough,
                        nodeState.IsEntryPoint,
                        nodeState.IsSearchPoint));
                }

                foreach (TacticalObstacleState obstacleState in battleState.ObstaclesById.Values)
                {
                    obstacles.Add(new TacticalObstacleSnapshot(
                        obstacleState.ObstacleId,
                        obstacleState.ObstacleType,
                        obstacleState.Position,
                        obstacleState.Radius,
                        obstacleState.BlocksMovement,
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
                        buildingState.IsDamaged,
                        new List<int>(buildingState.TacticalNodeIds),
                        new List<int>(buildingState.EntranceNodeIds),
                        new List<int>(buildingState.WindowNodeIds),
                        new List<int>(buildingState.InteriorNodeIds),
                        new List<int>(buildingState.SearchNodeIds)));
                }

                foreach (EnvironmentalZoneState zoneState in battleState.EnvironmentState.ZonesById.Values)
                {
                    zones.Add(new EnvironmentalZoneSnapshot(
                        zoneState.ZoneId,
                        zoneState.ZoneType,
                        zoneState.Position,
                        zoneState.Radius,
                        zoneState.Intensity,
                        zoneState.DurationRemaining,
                        zoneState.IsActive,
                        zoneState.VisionPenalty,
                        zoneState.DamagePerSecond,
                        zoneState.PressurePerSecond));
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
                battleState != null ? new BattleEnvironmentSnapshot(
                    battleState.EnvironmentState.IsNight,
                    battleState.EnvironmentState.GlobalVisibilityMultiplier,
                    battleState.EnvironmentState.AmbientLightLevel,
                    zones) : new BattleEnvironmentSnapshot(false, 1f, 1f, zones),
                battleState != null ? battleState.CurrentMissionStatus : new BattleMissionStatusSnapshot(0, 0, 0, 0, 0, 0, 0, 0, false, false, false, false, false, false, BattleCompletionType.Partial, 0),
                battleState != null ? battleState.CurrentBattleResult : null,
                battleState != null ? new List<BattleEventRecord>(battleState.RecentEvents) : new List<BattleEventRecord>());
        }
    }
}
