using System.Numerics;

namespace Warzone.Combat
{
    public sealed class MemberIntent
    {
        public MemberIntent(MemberIntentType intentType, Vector2 targetPosition, bool isCompleted = false)
        {
            IntentType = intentType;
            TargetPosition = targetPosition;
            IsCompleted = isCompleted;
        }

        public MemberIntentType IntentType { get; private set; }
        public Vector2 TargetPosition { get; private set; }
        public bool IsCompleted { get; private set; }

        public void UpdateTarget(Vector2 targetPosition)
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
