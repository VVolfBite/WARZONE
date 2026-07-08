using Warzone.Core.Math;

namespace Warzone.Combat
{
    public sealed class AiBlackboard
    {
        public int SelfSquadId { get; set; }
        public int? TargetSquadId { get; set; }
        public string SelfDefinitionId { get; set; }
        public Vec2 SelfPosition { get; set; }
        public Vec2 TargetPosition { get; set; }
        public float DistanceToTarget { get; set; }
        public float CurrentLeashDistance { get; set; }
        public float HealthRatio { get; set; }

        public bool HasTarget
        {
            get { return TargetSquadId.HasValue; }
        }
    }
}


