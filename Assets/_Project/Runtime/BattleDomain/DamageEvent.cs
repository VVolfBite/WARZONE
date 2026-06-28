namespace Warzone.BattleDomain
{
    public struct DamageEvent
    {
        public DamageEvent(BattleEntityId sourceEntityId, BattleEntityId targetEntityId, int damageAmount, string weaponId, float weaponRange, float projectileSpeed, bool didKillTarget)
        {
            SourceEntityId = sourceEntityId;
            TargetEntityId = targetEntityId;
            DamageAmount = damageAmount;
            WeaponId = weaponId;
            WeaponRange = weaponRange;
            ProjectileSpeed = projectileSpeed;
            DidKillTarget = didKillTarget;
        }

        public BattleEntityId SourceEntityId { get; }
        public BattleEntityId TargetEntityId { get; }
        public int DamageAmount { get; }
        public string WeaponId { get; }
        public float WeaponRange { get; }
        public float ProjectileSpeed { get; }
        public bool DidKillTarget { get; }
    }
}
