using System.Collections.Generic;
using Warzone.Content.Definitions;

namespace Warzone.Combat
{
    public sealed class BattleMissionRuntimeState
    {
        private readonly Dictionary<string, BattleObjectiveRuntimeState> _objectiveStates = new Dictionary<string, BattleObjectiveRuntimeState>();
        private readonly HashSet<int> _completedSearchPointIds = new HashSet<int>();
        private readonly HashSet<int> _enteredBuildingIds = new HashSet<int>();
        private readonly HashSet<BattleEntityId> _extractedMemberIds = new HashSet<BattleEntityId>();
        private readonly HashSet<BattleEntityId> _killedEnemyIds = new HashSet<BattleEntityId>();

        public BattleMissionRuntimeState()
        {
            LootRuntimeState = new BattleLootRuntimeState();
            CompletionType = BattleCompletionType.Partial;
        }

        public IReadOnlyDictionary<string, BattleObjectiveRuntimeState> ObjectiveStates
        {
            get { return _objectiveStates; }
        }

        public IReadOnlyCollection<int> CompletedSearchPointIds
        {
            get { return _completedSearchPointIds; }
        }

        public IReadOnlyCollection<int> EnteredBuildingIds
        {
            get { return _enteredBuildingIds; }
        }

        public IReadOnlyCollection<BattleEntityId> ExtractedMemberIds
        {
            get { return _extractedMemberIds; }
        }

        public IReadOnlyCollection<BattleEntityId> KilledEnemyIds
        {
            get { return _killedEnemyIds; }
        }

        public BattleLootRuntimeState LootRuntimeState { get; private set; }
        public bool IsBattleComplete { get; private set; }
        public BattleCompletionType CompletionType { get; private set; }
        public float CompletionTimeSeconds { get; private set; }

        public int LootDiscoveredCount
        {
            get { return LootRuntimeState.LootDiscoveredCount; }
        }

        public bool MarkSearchPointCompleted(int nodeId)
        {
            return _completedSearchPointIds.Add(nodeId);
        }

        public bool HasCompletedSearchPoint(int nodeId)
        {
            return _completedSearchPointIds.Contains(nodeId);
        }

        public bool MarkBuildingEntered(int buildingId)
        {
            return _enteredBuildingIds.Add(buildingId);
        }

        public bool HasEnteredBuilding(int buildingId)
        {
            return _enteredBuildingIds.Contains(buildingId);
        }

        public bool MarkMemberExtracted(BattleEntityId memberId)
        {
            return _extractedMemberIds.Add(memberId);
        }

        public bool MarkEnemyKilled(BattleEntityId enemyId)
        {
            return _killedEnemyIds.Add(enemyId);
        }

        public bool HasKilledEnemy(BattleEntityId enemyId)
        {
            return _killedEnemyIds.Contains(enemyId);
        }

        public BattleObjectiveRuntimeState GetOrCreateObjectiveState(string objectiveId, MissionObjectiveType objectiveType, int requiredCount)
        {
            string key = string.IsNullOrEmpty(objectiveId) ? objectiveType.ToString() : objectiveId;
            BattleObjectiveRuntimeState objectiveState;
            if (!_objectiveStates.TryGetValue(key, out objectiveState))
            {
                objectiveState = new BattleObjectiveRuntimeState(key, objectiveType, requiredCount);
                _objectiveStates.Add(key, objectiveState);
            }

            return objectiveState;
        }

        public void SetCompletion(bool isBattleComplete, BattleCompletionType completionType, float completionTimeSeconds)
        {
            IsBattleComplete = isBattleComplete;
            CompletionType = completionType;
            if (isBattleComplete)
            {
                CompletionTimeSeconds = completionTimeSeconds;
            }
        }
    }
}
