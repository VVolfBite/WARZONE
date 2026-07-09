using System.Collections.Generic;
using Warzone.Core.Math;
using Warzone.Content.Definitions;

namespace Warzone.Combat
{
    public sealed class BattleStateFactory
    {
        public BattleState CreateMemberSquadBattle(
            string battleId,
            int squadId,
            FactionId factionId,
            Vec2 rallyPosition,
            int memberCount,
            float formationSpacing,
            int startingMemberId = 1,
            string memberDefinitionId = "sandbox.rifleman",
            string weaponId = "sandbox.rifle",
            int memberHealth = 100,
            float movementSpeed = 4f,
            float detectionRange = 12f,
            float attackRange = 10f,
            string missionDefinitionId = null)
        {
            BattleState battleState = new BattleState(battleId);
            battleState.SetMissionDefinitionId(missionDefinitionId);
            List<BattleEntityId> memberIds = new List<BattleEntityId>(memberCount);
            BattleSquadState squadState = new BattleSquadState(squadId, factionId, rallyPosition, memberIds, formationSpacing);

            for (int i = 0; i < memberCount; i++)
            {
                BattleEntityId memberId = new BattleEntityId(startingMemberId + i);
                Vec2 offset = new Vec2((i % 2) * formationSpacing, -(i / 2) * formationSpacing);
                BattleMemberState memberState = new BattleMemberState(
                    memberId,
                    squadId,
                    factionId,
                    rallyPosition + offset,
                    memberHealth,
                    memberHealth,
                    movementSpeed,
                    weaponId,
                    memberDefinitionId,
                    detectionRange,
                    attackRange);
                battleState.AddMember(memberState);
                memberIds.Add(memberId);
            }

            squadState.SyncMemberIds(memberIds);
            battleState.AddSquad(squadState);
            battleState.AddTacticalNode(new TacticalNodeState(1, TacticalNodeType.RallyPoint, rallyPosition, 1.5f));
            return battleState;
        }

        public TacticalNodeState CreateTacticalNode(
            int nodeId,
            TacticalNodeType nodeType,
            Vec2 position,
            float radius,
            bool isEnabled = true,
            float requiredSearchSeconds = 3f,
            int? extractionOwnerSquadId = null,
            int? buildingId = null)
        {
            return new TacticalNodeState(nodeId, nodeType, position, radius, isEnabled, requiredSearchSeconds, extractionOwnerSquadId, buildingId);
        }

        public BattleEnemyState CreateEnemy(
            int enemyId,
            string definitionId,
            FactionId factionId,
            Vec2 position,
            int health,
            float movementSpeed,
            float detectionRange,
            float attackRange = 0f)
        {
            return new BattleEnemyState(
                new BattleEntityId(enemyId),
                definitionId,
                factionId,
                position,
                health,
                health,
                movementSpeed,
                detectionRange,
                attackRange);
        }

        public TacticalObstacleState CreateObstacle(
            int obstacleId,
            TacticalObstacleType obstacleType,
            Vec2 position,
            float radius,
            bool blocksMovement,
            bool blocksLineOfSight,
            bool blocksFire,
            bool providesCover,
            float damageReductionFactor,
            float accuracyPenaltyAgainstOccupant)
        {
            return new TacticalObstacleState(
                obstacleId,
                obstacleType,
                position,
                radius,
                blocksMovement,
                blocksLineOfSight,
                blocksFire,
                providesCover,
                damageReductionFactor,
                accuracyPenaltyAgainstOccupant);
        }

        public BuildingState CreateBuilding(int buildingId, Vec2 position, float radius, bool isEnterable, IReadOnlyList<int> tacticalNodeIds)
        {
            return new BuildingState(buildingId, position, radius, isEnterable, tacticalNodeIds);
        }
    }
}
