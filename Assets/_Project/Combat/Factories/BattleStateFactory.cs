using System.Collections.Generic;
using System.Numerics;
using Warzone.Content.Definitions;

namespace Warzone.Combat
{
    public sealed class BattleStateFactory
    {
        public BattleState CreateMemberSquadBattle(
            string battleId,
            int squadId,
            FactionId factionId,
            Vector2 rallyPosition,
            int memberCount,
            float formationSpacing,
            int startingMemberId = 1)
        {
            BattleState battleState = new BattleState(battleId);
            List<BattleEntityId> memberIds = new List<BattleEntityId>(memberCount);
            BattleSquadState squadState = new BattleSquadState(squadId, factionId, rallyPosition, memberIds, formationSpacing);

            for (int i = 0; i < memberCount; i++)
            {
                BattleEntityId memberId = new BattleEntityId(startingMemberId + i);
                Vector2 offset = new Vector2((i % 2) * formationSpacing, -(i / 2) * formationSpacing);
                BattleMemberState memberState = new BattleMemberState(
                    memberId,
                    squadId,
                    factionId,
                    rallyPosition + offset,
                    100,
                    100,
                    4f,
                    weaponId: "sandbox.rifle",
                    definitionId: "sandbox.rifleman");
                battleState.AddMember(memberState);
                memberIds.Add(memberId);
            }

            squadState.SyncMemberIds(memberIds);
            battleState.AddSquad(squadState);
            battleState.AddTacticalNode(new TacticalNodeState(1, rallyPosition, 1.5f));
            return battleState;
        }
    }
}
