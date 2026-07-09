using System.Collections.Generic;
using Warzone.Content;
using Warzone.Content.Definitions;

namespace Warzone.Combat
{
    public sealed class MissionObjectiveSystem
    {
        private readonly ContentCatalog _contentCatalog;

        public MissionObjectiveSystem(ContentCatalog contentCatalog)
        {
            _contentCatalog = contentCatalog;
        }

        public BattleMissionStatusSnapshot Evaluate(BattleState battleState)
        {
            if (battleState == null)
            {
                return new BattleMissionStatusSnapshot(0, 0, 0, 0, 0, 0, 0, 0, false, false, false, false, false, false, BattleCompletionType.Partial, 0);
            }

            MissionDefinition missionDefinition = null;
            bool hasMissionDefinition = !string.IsNullOrEmpty(battleState.MissionDefinitionId) &&
                                        _contentCatalog.TryGetMission(battleState.MissionDefinitionId, out missionDefinition);

            IReadOnlyList<BattleObjectiveResult> objectiveResults = BuildObjectiveResults(battleState, hasMissionDefinition ? missionDefinition.Objectives : null);
            bool enterBuildingComplete = IsObjectiveComplete(objectiveResults, MissionObjectiveType.EnterBuilding);
            bool searchComplete = IsObjectiveComplete(objectiveResults, MissionObjectiveType.SearchPoint);
            bool eliminateComplete = IsObjectiveComplete(objectiveResults, MissionObjectiveType.EliminateEnemies);
            bool extractComplete = IsObjectiveComplete(objectiveResults, MissionObjectiveType.ExtractSquad);
            int aliveEnemyCount = CountAliveEnemies(battleState);
            int searchedPointCount = battleState.MissionRuntimeState.CompletedSearchPointIds.Count;
            int totalSearchPointCount = CountSearchPoints(battleState);
            int extractedMemberCount = battleState.MissionRuntimeState.ExtractedMemberIds.Count;
            int totalAliveMemberCount = CountAliveMembers(battleState);
            int enteredBuildingCount = battleState.MissionRuntimeState.EnteredBuildingIds.Count;
            int totalBuildingObjectiveCount = CountEnterableBuildings(battleState);
            bool allMembersDead = totalAliveMemberCount == 0;
            bool objectivesComplete = AllRelevantObjectivesComplete(objectiveResults);
            bool isBattleComplete = objectivesComplete || allMembersDead;
            BattleCompletionType resultType = objectivesComplete
                ? BattleCompletionType.Success
                : allMembersDead
                    ? (AnyObjectiveComplete(objectiveResults) ? BattleCompletionType.Partial : BattleCompletionType.Failure)
                    : BattleCompletionType.Partial;

            battleState.MissionRuntimeState.SetCompletion(isBattleComplete, resultType, battleState.ElapsedTimeSeconds);

            return new BattleMissionStatusSnapshot(
                enteredBuildingCount,
                totalBuildingObjectiveCount,
                aliveEnemyCount,
                battleState.EnemiesById.Count,
                searchedPointCount,
                totalSearchPointCount,
                extractedMemberCount,
                totalAliveMemberCount,
                objectivesComplete,
                enterBuildingComplete,
                searchComplete,
                eliminateComplete,
                extractComplete,
                isBattleComplete,
                resultType,
                battleState.MissionRuntimeState.LootDiscoveredCount);
        }

        public IReadOnlyList<BattleObjectiveResult> BuildObjectiveResults(BattleState battleState, IReadOnlyList<MissionObjectiveDefinition> objectives)
        {
            List<BattleObjectiveResult> results = new List<BattleObjectiveResult>();
            BattleMissionRuntimeState runtimeState = battleState.MissionRuntimeState;

            if (objectives == null || objectives.Count == 0)
            {
                results.Add(BuildObjectiveResult(runtimeState, MissionObjectiveType.SearchPoint, "search.auto", runtimeState.CompletedSearchPointIds.Count, System.Math.Max(1, CountSearchPoints(battleState))));
                results.Add(BuildObjectiveResult(runtimeState, MissionObjectiveType.EliminateEnemies, "enemy.auto", runtimeState.KilledEnemyIds.Count, System.Math.Max(1, battleState.EnemiesById.Count)));
                results.Add(BuildObjectiveResult(runtimeState, MissionObjectiveType.ExtractSquad, "extract.auto", runtimeState.ExtractedMemberIds.Count, System.Math.Max(1, CountAliveMembers(battleState))));
                return results;
            }

            for (int i = 0; i < objectives.Count; i++)
            {
                MissionObjectiveDefinition objective = objectives[i];
                switch (objective.ObjectiveType)
                {
                    case MissionObjectiveType.EnterBuilding:
                        results.Add(BuildObjectiveResult(
                            runtimeState,
                            objective.ObjectiveType,
                            objective.TargetId,
                            CountCompletedBuildingsForObjective(battleState, objective.TargetId),
                            objective.RequiredCount > 0 ? objective.RequiredCount : 1));
                        break;

                    case MissionObjectiveType.SearchPoint:
                        results.Add(BuildObjectiveResult(
                            runtimeState,
                            objective.ObjectiveType,
                            objective.TargetId,
                            CountCompletedSearchPointsForObjective(battleState, objective.TargetId),
                            objective.RequiredCount > 0 ? objective.RequiredCount : 1));
                        break;

                    case MissionObjectiveType.EliminateEnemies:
                        int requiredEnemyCount = objective.RequiredCount > 0 ? objective.RequiredCount : battleState.EnemiesById.Count;
                        results.Add(BuildObjectiveResult(
                            runtimeState,
                            objective.ObjectiveType,
                            objective.TargetId,
                            CountKilledEnemiesForObjective(battleState, objective.TargetId),
                            requiredEnemyCount));
                        break;

                    case MissionObjectiveType.ExtractSquad:
                        int aliveMembers = CountAliveMembers(battleState);
                        results.Add(BuildObjectiveResult(
                            runtimeState,
                            objective.ObjectiveType,
                            objective.TargetId,
                            CountExtractedMembersForObjective(battleState, objective.TargetId),
                            objective.RequiredCount > 0 ? objective.RequiredCount : System.Math.Max(1, aliveMembers)));
                        break;
                }
            }

            return results;
        }

        private static BattleObjectiveResult BuildObjectiveResult(
            BattleMissionRuntimeState runtimeState,
            MissionObjectiveType objectiveType,
            string targetId,
            int currentCount,
            int requiredCount)
        {
            bool isCompleted = currentCount >= requiredCount;
            BattleObjectiveRuntimeState objectiveState = runtimeState.GetOrCreateObjectiveState(targetId, objectiveType, requiredCount);
            objectiveState.Update(currentCount, requiredCount, isCompleted);
            return new BattleObjectiveResult(objectiveType, targetId, currentCount, requiredCount, isCompleted);
        }

        private static bool AllRelevantObjectivesComplete(IReadOnlyList<BattleObjectiveResult> results)
        {
            if (results == null || results.Count == 0)
            {
                return false;
            }

            for (int i = 0; i < results.Count; i++)
            {
                if (!results[i].IsCompleted)
                {
                    return false;
                }
            }

            return true;
        }

        private static bool IsObjectiveComplete(IReadOnlyList<BattleObjectiveResult> results, MissionObjectiveType objectiveType)
        {
            bool found = false;
            for (int i = 0; i < results.Count; i++)
            {
                if (results[i].ObjectiveType != objectiveType)
                {
                    continue;
                }

                found = true;
                if (!results[i].IsCompleted)
                {
                    return false;
                }
            }

            return found;
        }

        private static bool AnyObjectiveComplete(IReadOnlyList<BattleObjectiveResult> results)
        {
            for (int i = 0; i < results.Count; i++)
            {
                if (results[i].IsCompleted)
                {
                    return true;
                }
            }

            return false;
        }

        private static int CountAliveEnemies(BattleState battleState)
        {
            int count = 0;
            foreach (BattleEnemyState enemyState in battleState.EnemiesById.Values)
            {
                if (enemyState.IsAlive)
                {
                    count++;
                }
            }

            return count;
        }

        private static int CountSearchPoints(BattleState battleState)
        {
            int count = 0;
            foreach (TacticalNodeState nodeState in battleState.TacticalNodesById.Values)
            {
                if (nodeState.NodeType == TacticalNodeType.SearchPoint)
                {
                    count++;
                }
            }

            return count;
        }

        private static int CountEnterableBuildings(BattleState battleState)
        {
            int count = 0;
            foreach (BuildingState buildingState in battleState.BuildingsById.Values)
            {
                if (buildingState.IsEnterable)
                {
                    count++;
                }
            }

            return count;
        }

        private static int CountAliveMembers(BattleState battleState)
        {
            int count = 0;
            foreach (BattleMemberState memberState in battleState.MembersById.Values)
            {
                if (memberState.IsAlive)
                {
                    count++;
                }
            }

            return count;
        }

        private static int CountCompletedSearchPointsForObjective(BattleState battleState, string targetId)
        {
            if (string.IsNullOrEmpty(targetId) || targetId == "search.auto" || targetId == "search.main")
            {
                return battleState.MissionRuntimeState.CompletedSearchPointIds.Count;
            }

            TacticalNodeState nodeState;
            if (TryGetNodeForTargetId(battleState, targetId, out nodeState))
            {
                return battleState.MissionRuntimeState.HasCompletedSearchPoint(nodeState.NodeId) ? 1 : 0;
            }

            return battleState.MissionRuntimeState.CompletedSearchPointIds.Count;
        }

        private static int CountCompletedBuildingsForObjective(BattleState battleState, string targetId)
        {
            if (string.IsNullOrEmpty(targetId) || targetId == "building.auto")
            {
                return battleState.MissionRuntimeState.EnteredBuildingIds.Count;
            }

            int buildingId;
            if (TryGetBuildingIdForTargetId(battleState, targetId, out buildingId))
            {
                return battleState.MissionRuntimeState.HasEnteredBuilding(buildingId) ? 1 : 0;
            }

            return battleState.MissionRuntimeState.EnteredBuildingIds.Count;
        }

        private static int CountKilledEnemiesForObjective(BattleState battleState, string targetId)
        {
            if (string.IsNullOrEmpty(targetId) || targetId == "enemy.all" || targetId == "enemy.auto")
            {
                return battleState.MissionRuntimeState.KilledEnemyIds.Count;
            }

            int count = 0;
            foreach (BattleEnemyState enemyState in battleState.EnemiesById.Values)
            {
                if (enemyState.DefinitionId == targetId && battleState.MissionRuntimeState.HasKilledEnemy(enemyState.EnemyId))
                {
                    count++;
                }
            }

            return count;
        }

        private static int CountExtractedMembersForObjective(BattleState battleState, string targetId)
        {
            return battleState.MissionRuntimeState.ExtractedMemberIds.Count;
        }

        private static bool TryGetNodeForTargetId(BattleState battleState, string targetId, out TacticalNodeState nodeState)
        {
            foreach (TacticalNodeState currentNodeState in battleState.TacticalNodesById.Values)
            {
                if (currentNodeState.NodeId.ToString() == targetId)
                {
                    nodeState = currentNodeState;
                    return true;
                }
            }

            nodeState = null;
            return false;
        }

        private static bool TryGetBuildingIdForTargetId(BattleState battleState, string targetId, out int buildingId)
        {
            foreach (BuildingState buildingState in battleState.BuildingsById.Values)
            {
                if (buildingState.BuildingId.ToString() == targetId)
                {
                    buildingId = buildingState.BuildingId;
                    return true;
                }
            }

            buildingId = 0;
            return false;
        }
    }
}
