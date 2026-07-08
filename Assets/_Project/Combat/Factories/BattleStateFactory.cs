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
            float attackRange = 10f)
        {
            BattleState battleState = new BattleState(battleId);
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
            battleState.AddTacticalNode(new TacticalNodeState(1, rallyPosition, 1.5f));
            return battleState;
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
    }
}
