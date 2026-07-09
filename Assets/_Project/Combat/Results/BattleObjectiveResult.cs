using Warzone.Content.Definitions;

namespace Warzone.Combat
{
    public sealed class BattleObjectiveResult
    {
        public BattleObjectiveResult(MissionObjectiveType objectiveType, string targetId, int currentCount, int requiredCount, bool isCompleted)
        {
            ObjectiveType = objectiveType;
            TargetId = targetId;
            CurrentCount = currentCount;
            RequiredCount = requiredCount;
            IsCompleted = isCompleted;
        }

        public MissionObjectiveType ObjectiveType { get; private set; }
        public string TargetId { get; private set; }
        public int CurrentCount { get; private set; }
        public int RequiredCount { get; private set; }
        public bool IsCompleted { get; private set; }
    }
}
