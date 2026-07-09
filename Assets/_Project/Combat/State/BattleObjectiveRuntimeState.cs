using Warzone.Content.Definitions;

namespace Warzone.Combat
{
    public sealed class BattleObjectiveRuntimeState
    {
        public BattleObjectiveRuntimeState(string objectiveId, MissionObjectiveType objectiveType, int requiredCount)
        {
            ObjectiveId = string.IsNullOrEmpty(objectiveId) ? objectiveType.ToString() : objectiveId;
            ObjectiveType = objectiveType;
            RequiredCount = requiredCount > 0 ? requiredCount : 1;
        }

        public string ObjectiveId { get; private set; }
        public MissionObjectiveType ObjectiveType { get; private set; }
        public bool IsCompleted { get; private set; }
        public int CurrentCount { get; private set; }
        public int RequiredCount { get; private set; }

        public float Progress
        {
            get
            {
                if (RequiredCount <= 0)
                {
                    return IsCompleted ? 1f : 0f;
                }

                float progress = (float)CurrentCount / RequiredCount;
                if (progress < 0f)
                {
                    return 0f;
                }

                return progress > 1f ? 1f : progress;
            }
        }

        public void Update(int currentCount, int requiredCount, bool isCompleted)
        {
            CurrentCount = currentCount < 0 ? 0 : currentCount;
            RequiredCount = requiredCount > 0 ? requiredCount : 1;
            IsCompleted = isCompleted;
        }
    }
}
