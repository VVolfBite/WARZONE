using System.Numerics;

namespace Warzone.Combat
{
    public sealed class AiBlackboard
    {
        public int SelfSquadId { get; set; }
        public int? TargetSquadId { get; set; }
        public string SelfDefinitionId { get; set; }
        public Vector2 SelfPosition { get; set; }
        public Vector2 TargetPosition { get; set; }
        public float DistanceToTarget { get; set; }
        public float CurrentLeashDistance { get; set; }
        public float HealthRatio { get; set; }

        public bool HasTarget
        {
            get { return TargetSquadId.HasValue; }
        }
    }
}

