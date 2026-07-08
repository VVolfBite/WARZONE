using System.Numerics;

namespace Warzone.Combat
{
    public sealed class SquadPlanningSystem
    {
        public void Execute(BattleState battleState)
        {
            if (battleState == null)
            {
                return;
            }

            foreach (BattleSquadState squadState in battleState.SquadsById.Values)
            {
                MoveSquadCommand moveCommand = squadState.CurrentOrder as MoveSquadCommand;
                if (moveCommand != null)
                {
                    squadState.SetDesiredPosition(moveCommand.Destination);
                    squadState.SetStance(SquadStance.Moving);
                    continue;
                }

                DefendAreaCommand defendAreaCommand = squadState.CurrentOrder as DefendAreaCommand;
                if (defendAreaCommand != null)
                {
                    squadState.SetDesiredPosition(defendAreaCommand.AreaCenter);
                    squadState.SetStance(SquadStance.Defending);
                    continue;
                }

                if (squadState.MemberIds.Count == 0)
                {
                    continue;
                }

                Vector2 center = CalculateCurrentCenter(battleState, squadState);
                squadState.UpdatePosition(center);
                squadState.SetDesiredPosition(center);
            }
        }

        private static Vector2 CalculateCurrentCenter(BattleState battleState, BattleSquadState squadState)
        {
            Vector2 center = Vector2.Zero;
            int aliveCount = 0;

            for (int i = 0; i < squadState.MemberIds.Count; i++)
            {
                BattleMemberState memberState;
                if (!battleState.TryGetMember(squadState.MemberIds[i], out memberState) || !memberState.IsAlive)
                {
                    continue;
                }

                center += memberState.Position;
                aliveCount++;
            }

            if (aliveCount == 0)
            {
                return squadState.Position;
            }

            return center / aliveCount;
        }
    }
}
