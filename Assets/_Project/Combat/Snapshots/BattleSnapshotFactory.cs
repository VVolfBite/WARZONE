using System.Collections.Generic;
using System.Numerics;

namespace Warzone.Combat
{
    public static class BattleSnapshotFactory
    {
        public static BattleSnapshot Create(BattleState battleState)
        {
            List<BattleSquadSnapshot> squads = new List<BattleSquadSnapshot>();
            List<BattleMemberSnapshot> members = new List<BattleMemberSnapshot>();

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
                        memberState.CurrentIntent != null ? memberState.CurrentIntent.IntentType.ToString() : "None",
                        memberState.CurrentIntent != null ? memberState.CurrentIntent.TargetPosition : (Vector2?)null,
                        memberState.CurrentIntent != null && memberState.CurrentIntent.IsCompleted));
                }
            }

            return new BattleSnapshot(
                battleState != null ? battleState.BattleId : string.Empty,
                battleState != null ? battleState.ElapsedTimeSeconds : 0f,
                squads,
                members);
        }
    }
}
