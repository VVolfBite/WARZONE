using System.Numerics;

namespace Warzone.Combat
{
    public sealed class BattleMemberSnapshot
    {
        public BattleMemberSnapshot(
            BattleEntityId memberId,
            int squadId,
            Vector2 position,
            Vector2 facing,
            int health,
            int maxHealth,
            bool isAlive,
            string currentIntent,
            Vector2? moveTarget,
            bool hasReachedTarget)
        {
            MemberId = memberId;
            SquadId = squadId;
            Position = position;
            Facing = facing;
            Health = health;
            MaxHealth = maxHealth;
            IsAlive = isAlive;
            CurrentIntent = currentIntent;
            MoveTarget = moveTarget;
            HasReachedTarget = hasReachedTarget;
        }

        public BattleEntityId MemberId { get; private set; }
        public int SquadId { get; private set; }
        public Vector2 Position { get; private set; }
        public Vector2 Facing { get; private set; }
        public int Health { get; private set; }
        public int MaxHealth { get; private set; }
        public bool IsAlive { get; private set; }
        public string CurrentIntent { get; private set; }
        public Vector2? MoveTarget { get; private set; }
        public bool HasReachedTarget { get; private set; }
    }
}
