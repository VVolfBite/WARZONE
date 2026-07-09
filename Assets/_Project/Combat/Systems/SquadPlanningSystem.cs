using Warzone.Core.Math;

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

                DefendAreaCommand defendCommand = squadState.CurrentOrder as DefendAreaCommand;
                if (defendCommand != null)
                {
                    squadState.SetDesiredPosition(defendCommand.AreaCenter);
                    squadState.SetStance(SquadStance.Defending);
                    continue;
                }

                SearchPointCommand searchCommand = squadState.CurrentOrder as SearchPointCommand;
                if (searchCommand != null)
                {
                    TacticalNodeState searchNode;
                    if (battleState.TryGetTacticalNode(searchCommand.NodeId, out searchNode))
                    {
                        squadState.SetDesiredPosition(searchNode.Position);
                    }

                    squadState.SetStance(SquadStance.Defending);
                    continue;
                }

                ExtractSquadCommand extractCommand = squadState.CurrentOrder as ExtractSquadCommand;
                if (extractCommand != null)
                {
                    TacticalNodeState extractionNode;
                    if (battleState.TryGetTacticalNode(extractCommand.ExtractionNodeId, out extractionNode))
                    {
                        squadState.SetDesiredPosition(extractionNode.Position);
                    }

                    squadState.SetStance(SquadStance.Moving);
                    continue;
                }

                EnterBuildingCommand enterBuildingCommand = squadState.CurrentOrder as EnterBuildingCommand;
                if (enterBuildingCommand != null)
                {
                    BuildingState buildingState;
                    if (battleState.TryGetBuilding(enterBuildingCommand.BuildingId, out buildingState))
                    {
                        squadState.SetDesiredPosition(buildingState.Position);
                    }

                    squadState.SetStance(SquadStance.Moving);
                    continue;
                }

                DefendBuildingCommand defendBuildingCommand = squadState.CurrentOrder as DefendBuildingCommand;
                if (defendBuildingCommand != null)
                {
                    BuildingState buildingState;
                    if (battleState.TryGetBuilding(defendBuildingCommand.BuildingId, out buildingState))
                    {
                        squadState.SetDesiredPosition(buildingState.Position);
                    }

                    squadState.SetStance(SquadStance.Defending);
                    continue;
                }

                SearchBuildingCommand searchBuildingCommand = squadState.CurrentOrder as SearchBuildingCommand;
                if (searchBuildingCommand != null)
                {
                    BuildingState buildingState;
                    if (battleState.TryGetBuilding(searchBuildingCommand.BuildingId, out buildingState))
                    {
                        squadState.SetDesiredPosition(buildingState.Position);
                    }

                    squadState.SetStance(SquadStance.Defending);
                    continue;
                }

                if (squadState.MemberIds.Count == 0)
                {
                    continue;
                }

                Vec2 center = CalculateCurrentCenter(battleState, squadState);
                squadState.UpdatePosition(center);
                squadState.SetDesiredPosition(center);
            }
        }

        private static Vec2 CalculateCurrentCenter(BattleState battleState, BattleSquadState squadState)
        {
            Vec2 center = Vec2.Zero;
            int activeCount = 0;

            for (int i = 0; i < squadState.MemberIds.Count; i++)
            {
                BattleMemberState memberState;
                if (!battleState.TryGetMember(squadState.MemberIds[i], out memberState) || !memberState.CanAct)
                {
                    continue;
                }

                center += memberState.Position;
                activeCount++;
            }

            if (activeCount == 0)
            {
                return squadState.Position;
            }

            return center / activeCount;
        }
    }
}
