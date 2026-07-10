using System.Collections.Generic;
using Warzone.Combat;
using Warzone.Content.Definitions;
using Warzone.Core.Math;

namespace Warzone.Application
{
    public sealed class BattleStateFromMissionFactory
    {
        private readonly BattleStateFactory _battleStateFactory = new BattleStateFactory();

        public BattleState Create(MissionLaunchPlan launchPlan)
        {
            if (launchPlan == null)
            {
                return null;
            }

            BattleState battleState = new BattleState(launchPlan.MissionId);
            battleState.SetMissionDefinitionId(launchPlan.MissionId);
            battleState.MissionRuntimeState.SetCompletion(false, BattleCompletionType.Partial, 0f);

            for (int i = 0; i < launchPlan.ObjectiveDefinitions.Count; i++)
            {
                MissionObjectiveDefinition objectiveDefinition = launchPlan.ObjectiveDefinitions[i];
                battleState.MissionRuntimeState.GetOrCreateObjectiveState(
                    string.IsNullOrEmpty(objectiveDefinition.TargetId) ? objectiveDefinition.ObjectiveType.ToString() : objectiveDefinition.TargetId,
                    objectiveDefinition.ObjectiveType,
                    objectiveDefinition.RequiredCount);
            }

            List<BattleEntityId> memberIds = new List<BattleEntityId>();
            for (int i = 0; i < launchPlan.SquadLoadouts.Count; i++)
            {
                MissionSquadLoadout squadLoadout = launchPlan.SquadLoadouts[i];
                List<BattleEntityId> squadMemberIds = new List<BattleEntityId>();
                Vec2 squadPosition = launchPlan.SiteContext.EntryPosition + new Vec2(i * 2f, 0f);
                for (int j = 0; j < squadLoadout.Members.Count; j++)
                {
                    MissionMemberLoadout memberLoadout = squadLoadout.Members[j];
                    BattleEntityId battleMemberId = new BattleEntityId(memberLoadout.BattleMemberId);
                    squadMemberIds.Add(battleMemberId);
                    memberIds.Add(battleMemberId);

                    BattleMemberState memberState = new BattleMemberState(
                        battleMemberId,
                        squadLoadout.SquadId,
                        FactionId.Player,
                        squadPosition + new Vec2((j % 2) * squadLoadout.FormationSpacing, -(j / 2) * squadLoadout.FormationSpacing),
                        memberLoadout.MaxHealth,
                        memberLoadout.MaxHealth,
                        memberLoadout.BaseMovementSpeed,
                        memberLoadout.WeaponDefinitionId,
                        memberLoadout.CampaignMemberId,
                        memberLoadout.DetectionRange,
                        10f,
                        memberLoadout.AccuracyModifier,
                        100f,
                        memberLoadout.NightVisionLevel,
                        memberLoadout.SmokeVisionLevel,
                        memberLoadout.HasLightSource);
                    battleState.AddMember(memberState);
                }

                BattleSquadState squadState = new BattleSquadState(
                    squadLoadout.SquadId,
                    FactionId.Player,
                    squadPosition,
                    squadMemberIds,
                    squadLoadout.FormationSpacing);
                squadState.SetDesiredPosition(launchPlan.SiteContext.EntryPosition);
                squadState.SetCurrentOrder(new MoveSquadCommand(squadLoadout.SquadId, launchPlan.SiteContext.EntryPosition));
                battleState.AddSquad(squadState);
            }

            battleState.AddTacticalNode(_battleStateFactory.CreateTacticalNode(1, TacticalNodeType.RallyPoint, launchPlan.SiteContext.EntryPosition, 1.5f));
            battleState.AddTacticalNode(_battleStateFactory.CreateTacticalNode(2, TacticalNodeType.ExtractionPoint, launchPlan.SiteContext.ExtractionPosition, 1.8f, true, 0f, 1));
            int buildingId = 100;
            battleState.AddTacticalNode(_battleStateFactory.CreateTacticalNode(3, TacticalNodeType.SearchPoint, launchPlan.SiteContext.SearchPosition, 1.2f, true, 3f, null, buildingId, true, false, false, false, true));

            if (launchPlan.SiteContext.IsEnterable)
            {
                battleState.AddTacticalNode(_battleStateFactory.CreateTacticalNode(101, TacticalNodeType.BuildingEntrance, launchPlan.SiteContext.SearchPosition + new Vec2(1.5f, 0f), 0.8f, true, 0f, null, buildingId, false, true, true, true));
                battleState.AddTacticalNode(_battleStateFactory.CreateTacticalNode(102, TacticalNodeType.InteriorPosition, launchPlan.SiteContext.SearchPosition, 0.8f, true, 0f, null, buildingId, true));
                battleState.AddTacticalNode(_battleStateFactory.CreateTacticalNode(103, TacticalNodeType.Window, launchPlan.SiteContext.SearchPosition + new Vec2(0f, 1.5f), 0.75f, true, 0f, null, buildingId, false, true, true));
                battleState.AddBuilding(_battleStateFactory.CreateBuilding(buildingId, launchPlan.SiteContext.SearchPosition, 3f, true, new List<int> { 101, 102, 103, 3 }, new List<int> { 101 }, new List<int> { 103 }, new List<int> { 102 }, new List<int> { 3 }));
                battleState.MissionRuntimeState.GetOrCreateObjectiveState("building." + buildingId, MissionObjectiveType.EnterBuilding, 1);
            }

            return battleState;
        }
    }
}
