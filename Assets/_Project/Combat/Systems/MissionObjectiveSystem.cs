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
                return new BattleMissionStatusSnapshot(0, 0, 0, 0, 0, 0, false, false, false, false, false, BattleCompletionType.Partial, 0);
            }

            MissionDefinition missionDefinition = null;
            bool hasMissionDefinition = !string.IsNullOrEmpty(battleState.MissionDefinitionId) &&
                                        _contentCatalog.TryGetMission(battleState.MissionDefinitionId, out missionDefinition);

            IReadOnlyList<BattleObjectiveResult> objectiveResults = BuildObjectiveResults(battleState, hasMissionDefinition ? missionDefinition.Objectives : null);
            bool searchComplete = IsObjectiveComplete(objectiveResults, MissionObjectiveType.SearchPoint);
            bool eliminateComplete = IsObjectiveComplete(objectiveResults, MissionObjectiveType.EliminateEnemies);
            bool extractComplete = IsObjectiveComplete(objectiveResults, MissionObjectiveType.ExtractSquad);
            int aliveEnemyCount = CountAliveEnemies(battleState);
            int searchedPointCount = CountSearchedPoints(battleState);
            int totalSearchPointCount = CountSearchPoints(battleState);
            int extractedMemberCount = CountExtractedMembers(battleState);
            int totalAliveMemberCount = CountAliveMembers(battleState);
            bool allMembersDead = totalAliveMemberCount == 0;
            bool objectivesComplete = searchComplete && eliminateComplete && extractComplete;
            bool isBattleComplete = objectivesComplete || allMembersDead;
            BattleCompletionType resultType = objectivesComplete
                ? BattleCompletionType.Success
                : allMembersDead
                    ? (AnyObjectiveComplete(objectiveResults) ? BattleCompletionType.Partial : BattleCompletionType.Failure)
                    : BattleCompletionType.Partial;

            return new BattleMissionStatusSnapshot(
                aliveEnemyCount,
                battleState.EnemiesById.Count,
                searchedPointCount,
                totalSearchPointCount,
                extractedMemberCount,
                totalAliveMemberCount,
                objectivesComplete,
                searchComplete,
                eliminateComplete,
                extractComplete,
                isBattleComplete,
                resultType,
                CountLootEvents(battleState));
        }

        public IReadOnlyList<BattleObjectiveResult> BuildObjectiveResults(BattleState battleState, IReadOnlyList<MissionObjectiveDefinition> objectives)
        {
            List<BattleObjectiveResult> results = new List<BattleObjectiveResult>();

            if (objectives == null || objectives.Count == 0)
            {
                results.Add(new BattleObjectiveResult(MissionObjectiveType.SearchPoint, "search.auto", CountSearchedPoints(battleState), System.Math.Max(1, CountSearchPoints(battleState)), CountSearchPoints(battleState) == 0 || CountSearchedPoints(battleState) >= CountSearchPoints(battleState)));
                results.Add(new BattleObjectiveResult(MissionObjectiveType.EliminateEnemies, "enemy.auto", battleState.EnemiesById.Count - CountAliveEnemies(battleState), System.Math.Max(1, battleState.EnemiesById.Count), CountAliveEnemies(battleState) == 0));
                results.Add(new BattleObjectiveResult(MissionObjectiveType.ExtractSquad, "extract.auto", CountExtractedMembers(battleState), System.Math.Max(1, CountAliveMembers(battleState)), CountAliveMembers(battleState) == 0 || CountExtractedMembers(battleState) >= CountAliveMembers(battleState)));
                return results;
            }

            for (int i = 0; i < objectives.Count; i++)
            {
                MissionObjectiveDefinition objective = objectives[i];
                switch (objective.ObjectiveType)
                {
                    case MissionObjectiveType.SearchPoint:
                        results.Add(new BattleObjectiveResult(
                            objective.ObjectiveType,
                            objective.TargetId,
                            CountSearchedPoints(battleState),
                            objective.RequiredCount,
                            CountSearchedPoints(battleState) >= objective.RequiredCount));
                        break;

                    case MissionObjectiveType.EliminateEnemies:
                        int deadEnemies = battleState.EnemiesById.Count - CountAliveEnemies(battleState);
                        results.Add(new BattleObjectiveResult(
                            objective.ObjectiveType,
                            objective.TargetId,
                            deadEnemies,
                            objective.RequiredCount > 0 ? objective.RequiredCount : battleState.EnemiesById.Count,
                            CountAliveEnemies(battleState) == 0));
                        break;

                    case MissionObjectiveType.ExtractSquad:
                        int aliveMembers = CountAliveMembers(battleState);
                        results.Add(new BattleObjectiveResult(
                            objective.ObjectiveType,
                            objective.TargetId,
                            CountExtractedMembers(battleState),
                            objective.RequiredCount > 0 ? objective.RequiredCount : aliveMembers,
                            aliveMembers == 0 || CountExtractedMembers(battleState) >= aliveMembers));
                        break;
                }
            }

            return results;
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

        private static int CountSearchedPoints(BattleState battleState)
        {
            int count = 0;
            foreach (TacticalNodeState nodeState in battleState.TacticalNodesById.Values)
            {
                if (nodeState.NodeType == TacticalNodeType.SearchPoint && nodeState.IsSearched)
                {
                    count++;
                }
            }

            return count;
        }

        private static int CountExtractedMembers(BattleState battleState)
        {
            int count = 0;
            foreach (BattleMemberState memberState in battleState.MembersById.Values)
            {
                if (memberState.IsExtracted)
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

        private static int CountLootEvents(BattleState battleState)
        {
            int count = 0;
            for (int i = 0; i < battleState.RecentEvents.Count; i++)
            {
                if (battleState.RecentEvents[i].EventType == BattleEventTypes.LootDiscovered)
                {
                    count += battleState.RecentEvents[i].Amount > 0 ? battleState.RecentEvents[i].Amount : 1;
                }
            }

            return count;
        }
    }
}
