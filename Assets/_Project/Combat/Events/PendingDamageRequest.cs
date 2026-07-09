namespace Warzone.Combat
{
    public sealed class PendingDamageRequest
    {
        public PendingDamageRequest(
            BattleEntityId sourceId,
            BattleEntityId targetId,
            int damageAmount,
            string weaponId,
            bool targetIsMember,
            float damageMultiplier = 1f,
            int? coverObstacleId = null)
        {
            SourceId = sourceId;
            TargetId = targetId;
            DamageAmount = damageAmount;
            WeaponId = weaponId;
            TargetIsMember = targetIsMember;
            DamageMultiplier = damageMultiplier;
            CoverObstacleId = coverObstacleId;
        }

        public BattleEntityId SourceId { get; private set; }
        public BattleEntityId TargetId { get; private set; }
        public int DamageAmount { get; private set; }
        public string WeaponId { get; private set; }
        public bool TargetIsMember { get; private set; }
        public float DamageMultiplier { get; private set; }
        public int? CoverObstacleId { get; private set; }
    }
}

