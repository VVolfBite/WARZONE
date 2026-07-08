using Warzone.Core.Math;

namespace Warzone.Combat
{
    public sealed class MemberIntent
    {
        public MemberIntent(MemberIntentType intentType, Vec2 targetPosition, bool isCompleted = false)
        {
            IntentType = intentType;
            TargetPosition = targetPosition;
            IsCompleted = isCompleted;
        }

        public MemberIntentType IntentType { get; private set; }
        public Vec2 TargetPosition { get; private set; }
        public bool IsCompleted { get; private set; }

        public void UpdateTarget(Vec2 targetPosition)
        {
            TargetPosition = targetPosition;
            IsCompleted = false;
        }

        public void MarkCompleted()
        {
            IsCompleted = true;
        }
    }
}

