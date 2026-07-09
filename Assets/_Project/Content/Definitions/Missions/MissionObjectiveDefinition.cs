namespace Warzone.Content.Definitions
{
    public sealed class MissionObjectiveDefinition
    {
        public MissionObjectiveDefinition(MissionObjectiveType objectiveType, string targetId, int requiredCount = 1)
        {
            ObjectiveType = objectiveType;
            TargetId = targetId;
            RequiredCount = requiredCount;
        }

        public MissionObjectiveType ObjectiveType { get; private set; }
        public string TargetId { get; private set; }
        public int RequiredCount { get; private set; }
    }
}
