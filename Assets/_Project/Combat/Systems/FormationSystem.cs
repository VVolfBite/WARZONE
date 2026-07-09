using System.Collections.Generic;
using Warzone.Core.Math;

namespace Warzone.Combat
{
    public sealed class FormationSystem
    {
        public void Execute(BattleState battleState)
        {
            if (battleState == null)
            {
                return;
            }

            ResetTacticalReservations(battleState);
            foreach (BattleSquadState squadState in battleState.SquadsById.Values)
            {
                AssignIntents(battleState, squadState);
            }
        }

        private static void ResetTacticalReservations(BattleState battleState)
        {
            foreach (TacticalNodeState nodeState in battleState.TacticalNodesById.Values)
            {
                if (nodeState == null)
                {
                    continue;
                }

                nodeState.ClearReservation();
                if (!nodeState.OccupyingMemberId.HasValue)
                {
                    continue;
                }

                BattleMemberState memberState;
                if (!battleState.TryGetMember(nodeState.OccupyingMemberId.Value, out memberState) || !memberState.CanAct)
                {
                    nodeState.SetOccupyingMember(null);
                }
            }
        }

        private static void AssignIntents(BattleState battleState, BattleSquadState squadState)
        {
            List<BattleMemberState> activeMembers = new List<BattleMemberState>();
            for (int i = 0; i < squadState.MemberIds.Count; i++)
            {
                BattleMemberState memberState;
                if (!battleState.TryGetMember(squadState.MemberIds[i], out memberState) || !memberState.CanReceiveOrders)
                {
                    continue;
                }

                activeMembers.Add(memberState);
            }

            if (activeMembers.Count == 0)
            {
                return;
            }

            DefendBuildingCommand defendBuildingCommand = squadState.CurrentOrder as DefendBuildingCommand;
            if (defendBuildingCommand != null)
            {
                AssignDefendBuildingIntents(battleState, squadState, activeMembers, defendBuildingCommand);
                return;
            }

            EnterBuildingCommand enterBuildingCommand = squadState.CurrentOrder as EnterBuildingCommand;
            if (enterBuildingCommand != null)
            {
                AssignEnterBuildingIntents(battleState, squadState, activeMembers, enterBuildingCommand);
                return;
            }

            SearchBuildingCommand searchBuildingCommand = squadState.CurrentOrder as SearchBuildingCommand;
            if (searchBuildingCommand != null)
            {
                AssignSearchBuildingIntents(battleState, squadState, activeMembers, searchBuildingCommand);
                return;
            }

            DefendAreaCommand defendCommand = squadState.CurrentOrder as DefendAreaCommand;
            if (defendCommand != null)
            {
                AssignDefendIntents(battleState, squadState, activeMembers, defendCommand);
                return;
            }

            SearchPointCommand searchCommand = squadState.CurrentOrder as SearchPointCommand;
            if (searchCommand != null)
            {
                AssignSearchIntents(battleState, squadState, activeMembers, searchCommand);
                return;
            }

            ExtractSquadCommand extractCommand = squadState.CurrentOrder as ExtractSquadCommand;
            if (extractCommand != null)
            {
                AssignExtractIntents(battleState, activeMembers, extractCommand);
                return;
            }

            AssignMoveFormation(squadState, activeMembers);
        }

        private static void AssignDefendIntents(BattleState battleState, BattleSquadState squadState, List<BattleMemberState> activeMembers, DefendAreaCommand command)
        {
            for (int i = 0; i < activeMembers.Count; i++)
            {
                BattleMemberState memberState = activeMembers[i];
                TacticalNodeState coverNode = FindAvailableCoverNodeQuery.Execute(battleState, command.AreaCenter, command.Radius);
                if (coverNode != null)
                {
                    ReserveNodeForMember(battleState, squadState, memberState, coverNode, MemberIntentType.TakeCover);
                    continue;
                }

                Vec2 fallback = BuildFormationTarget(squadState, activeMembers.Count, i, command.AreaCenter);
                ApplyIntent(memberState, MemberIntentType.MoveToPosition, fallback, null);
            }
        }

        private static void AssignSearchIntents(BattleState battleState, BattleSquadState squadState, List<BattleMemberState> activeMembers, SearchPointCommand command)
        {
            TacticalNodeState searchNode;
            if (!battleState.TryGetTacticalNode(command.NodeId, out searchNode))
            {
                return;
            }

            for (int i = 0; i < activeMembers.Count; i++)
            {
                BattleMemberState memberState = activeMembers[i];
                if (!searchNode.IsSearched && i == 0)
                {
                    ReserveNodeForMember(battleState, squadState, memberState, searchNode, MemberIntentType.SearchPoint);
                    continue;
                }

                TacticalNodeState coverNode = FindAvailableCoverNodeQuery.Execute(battleState, searchNode.Position, 6f);
                if (coverNode != null)
                {
                    ReserveNodeForMember(battleState, squadState, memberState, coverNode, MemberIntentType.TakeCover);
                    continue;
                }

                Vec2 fallback = BuildFormationTarget(squadState, activeMembers.Count, i, searchNode.Position);
                ApplyIntent(memberState, MemberIntentType.MoveToPosition, fallback, null);
            }
        }

        private static void AssignExtractIntents(BattleState battleState, List<BattleMemberState> activeMembers, ExtractSquadCommand command)
        {
            TacticalNodeState extractionNode;
            if (!battleState.TryGetTacticalNode(command.ExtractionNodeId, out extractionNode))
            {
                return;
            }

            for (int i = 0; i < activeMembers.Count; i++)
            {
                ApplyIntent(activeMembers[i], MemberIntentType.Extract, extractionNode.Position, extractionNode.NodeId);
            }
        }

        private static void AssignEnterBuildingIntents(BattleState battleState, BattleSquadState squadState, List<BattleMemberState> activeMembers, EnterBuildingCommand command)
        {
            BuildingState buildingState = FindBuildingByIdQuery.Execute(battleState, command.BuildingId);
            if (buildingState == null)
            {
                return;
            }

            for (int i = 0; i < activeMembers.Count; i++)
            {
                BattleMemberState memberState = activeMembers[i];
                TacticalNodeState nodeState = FindAvailableInteriorNodeQuery.Execute(battleState, buildingState.BuildingId) ??
                                              FindBuildingEntranceQuery.Execute(battleState, buildingState.BuildingId, memberState.Position) ??
                                              FindAvailableWindowNodeQuery.Execute(battleState, buildingState.BuildingId);
                if (nodeState != null)
                {
                    ReserveNodeForMember(battleState, squadState, memberState, nodeState, MemberIntentType.MoveToPosition);
                    continue;
                }

                Vec2 fallback = BuildFormationTarget(squadState, activeMembers.Count, i, buildingState.Position);
                ApplyIntent(memberState, MemberIntentType.MoveToPosition, fallback, null);
            }
        }

        private static void AssignDefendBuildingIntents(BattleState battleState, BattleSquadState squadState, List<BattleMemberState> activeMembers, DefendBuildingCommand command)
        {
            BuildingState buildingState = FindBuildingByIdQuery.Execute(battleState, command.BuildingId);
            if (buildingState == null)
            {
                AssignDefendIntents(battleState, squadState, activeMembers, new DefendAreaCommand(squadState.SquadId, squadState.DesiredPosition, 8f));
                return;
            }

            for (int i = 0; i < activeMembers.Count; i++)
            {
                BattleMemberState memberState = activeMembers[i];
                TacticalNodeState nodeState = FindAvailableWindowNodeQuery.Execute(battleState, buildingState.BuildingId);
                if (nodeState == null)
                {
                    nodeState = FindBuildingEntranceQuery.Execute(battleState, buildingState.BuildingId, memberState.Position);
                }

                if (nodeState == null)
                {
                    nodeState = FindAvailableInteriorNodeQuery.Execute(battleState, buildingState.BuildingId);
                }

                if (nodeState != null)
                {
                    ReserveNodeForMember(battleState, squadState, memberState, nodeState, nodeState.NodeType == TacticalNodeType.Window ? MemberIntentType.TakeCover : MemberIntentType.MoveToPosition);
                    continue;
                }

                TacticalNodeState fallbackCoverNode = FindAvailableCoverNodeQuery.Execute(battleState, buildingState.Position, buildingState.Radius + 4f);
                if (fallbackCoverNode != null)
                {
                    ReserveNodeForMember(battleState, squadState, memberState, fallbackCoverNode, MemberIntentType.TakeCover);
                    continue;
                }

                Vec2 fallback = BuildFormationTarget(squadState, activeMembers.Count, i, buildingState.Position);
                ApplyIntent(memberState, MemberIntentType.MoveToPosition, fallback, null);
            }
        }

        private static void AssignSearchBuildingIntents(BattleState battleState, BattleSquadState squadState, List<BattleMemberState> activeMembers, SearchBuildingCommand command)
        {
            BuildingState buildingState = FindBuildingByIdQuery.Execute(battleState, command.BuildingId);
            if (buildingState == null)
            {
                return;
            }

            TacticalNodeState searchNode = FindBuildingSearchPointQuery.Execute(battleState, buildingState.BuildingId, true);
            for (int i = 0; i < activeMembers.Count; i++)
            {
                BattleMemberState memberState = activeMembers[i];
                if (searchNode != null && !searchNode.IsSearched && i == 0)
                {
                    ReserveNodeForMember(battleState, squadState, memberState, searchNode, MemberIntentType.SearchPoint);
                    continue;
                }

                TacticalNodeState nodeState = FindAvailableWindowNodeQuery.Execute(battleState, buildingState.BuildingId) ??
                                              FindAvailableInteriorNodeQuery.Execute(battleState, buildingState.BuildingId);
                if (nodeState != null)
                {
                    ReserveNodeForMember(battleState, squadState, memberState, nodeState, nodeState.NodeType == TacticalNodeType.Window ? MemberIntentType.TakeCover : MemberIntentType.MoveToPosition);
                    continue;
                }

                Vec2 fallback = BuildFormationTarget(squadState, activeMembers.Count, i, buildingState.Position);
                ApplyIntent(memberState, MemberIntentType.MoveToPosition, fallback, null);
            }
        }

        private static void ReserveNodeForMember(BattleState battleState, BattleSquadState squadState, BattleMemberState memberState, TacticalNodeState nodeState, MemberIntentType intentType)
        {
            nodeState.ReserveFor(memberState.MemberId);
            ApplyIntent(memberState, intentType, nodeState.Position, nodeState.NodeId);
            battleState.AddEvent(new BattleEventRecord(BattleEventTypes.TacticalNodeReserved, squadState.SquadId, memberState.MemberId, nodeState.NodeId.ToString()));

            if (intentType == MemberIntentType.TakeCover || nodeState.NodeType == TacticalNodeType.Window || nodeState.NodeType == TacticalNodeType.Doorway)
            {
                battleState.AddEvent(new BattleEventRecord(BattleEventTypes.DefensivePositionAssigned, squadState.SquadId, memberState.MemberId, nodeState.NodeId.ToString()));
            }

            if (nodeState.NodeType == TacticalNodeType.Window)
            {
                battleState.AddEvent(new BattleEventRecord(BattleEventTypes.WindowPositionAssigned, squadState.SquadId, memberState.MemberId, nodeState.NodeId.ToString()));
            }
        }

        private static void AssignMoveFormation(BattleSquadState squadState, List<BattleMemberState> activeMembers)
        {
            for (int i = 0; i < activeMembers.Count; i++)
            {
                Vec2 target = BuildFormationTarget(squadState, activeMembers.Count, i, squadState.DesiredPosition);
                ApplyIntent(activeMembers[i], MemberIntentType.MoveToPosition, target, null);
            }
        }

        private static Vec2 BuildFormationTarget(BattleSquadState squadState, int memberCount, int memberIndex, Vec2 center)
        {
            Vec2 direction = squadState.DesiredPosition - squadState.Position;
            direction = direction.LengthSquared <= 0.0001f ? new Vec2(0f, 1f) : Vec2.Normalize(direction);
            Vec2 right = new Vec2(-direction.Y, direction.X);
            float halfWidth = (memberCount - 1) * squadState.FormationSpacing * 0.5f;
            return center + (right * ((memberIndex * squadState.FormationSpacing) - halfWidth));
        }

        private static void ApplyIntent(BattleMemberState memberState, MemberIntentType intentType, Vec2 targetPosition, int? tacticalNodeId)
        {
            if (memberState.CurrentIntent == null)
            {
                memberState.SetIntent(new MemberIntent(intentType, targetPosition, false, tacticalNodeId));
                return;
            }

            if (memberState.CurrentIntent.IntentType != intentType)
            {
                memberState.SetIntent(new MemberIntent(intentType, targetPosition, false, tacticalNodeId));
                return;
            }

            Vec2 delta = memberState.CurrentIntent.TargetPosition - targetPosition;
            if (delta.LengthSquared > 0.0001f || memberState.CurrentIntent.TacticalNodeId != tacticalNodeId)
            {
                memberState.CurrentIntent.UpdateTarget(targetPosition, tacticalNodeId);
            }
        }
    }
}
