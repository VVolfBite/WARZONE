namespace Warzone.Combat
{
    public sealed class PendingDamageRequest
    {
        public PendingDamageRequest(BattleEntityId sourceId, BattleEntityId targetId, int damageAmount, string weaponId, bool targetIsMember)
        {
            SourceId = sourceId;
            TargetId = targetId;
            DamageAmount = damageAmount;
            WeaponId = weaponId;
            TargetIsMember = targetIsMember;
        }

        public BattleEntityId SourceId { get; private set; }
        public BattleEntityId TargetId { get; private set; }
        public int DamageAmount { get; private set; }
        public string WeaponId { get; private set; }
        public bool TargetIsMember { get; private set; }
    }
}

