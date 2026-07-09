using Warzone.Core.Math;

namespace Warzone.Combat
{
    public sealed class MemberIntent
    {
        public MemberIntent(MemberIntentType intentType, Vec2 targetPosition, bool isCompleted = false, int? tacticalNodeId = null)
        {
            IntentType = intentType;
            TargetPosition = targetPosition;
            IsCompleted = isCompleted;
            TacticalNodeId = tacticalNodeId;
        }

        public MemberIntentType IntentType { get; private set; }
        public Vec2 TargetPosition { get; private set; }
        public bool IsCompleted { get; private set; }
        public int? TacticalNodeId { get; private set; }

        public void UpdateTarget(Vec2 targetPosition, int? tacticalNodeId = null)
        {
            TargetPosition = targetPosition;
            IsCompleted = false;
            TacticalNodeId = tacticalNodeId;
        }

        public void MarkCompleted()
        {
            IsCompleted = true;
        }
    }
}

