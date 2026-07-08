namespace Warzone.Combat
{
    public struct DamageEvent
    {
        private readonly BattleEntityId _sourceEntityId;
        private readonly BattleEntityId _targetEntityId;
        private readonly int _damageAmount;
        private readonly string _weaponId;
        private readonly float _weaponRange;
        private readonly float _projectileSpeed;
        private readonly bool _didKillTarget;

        public DamageEvent(BattleEntityId sourceEntityId, BattleEntityId targetEntityId, int damageAmount, string weaponId, float weaponRange, float projectileSpeed, bool didKillTarget)
        {
            _sourceEntityId = sourceEntityId;
            _targetEntityId = targetEntityId;
            _damageAmount = damageAmount;
            _weaponId = weaponId;
            _weaponRange = weaponRange;
            _projectileSpeed = projectileSpeed;
            _didKillTarget = didKillTarget;
        }

        public BattleEntityId SourceEntityId { get { return _sourceEntityId; } }
        public BattleEntityId TargetEntityId { get { return _targetEntityId; } }
        public int DamageAmount { get { return _damageAmount; } }
        public string WeaponId { get { return _weaponId; } }
        public float WeaponRange { get { return _weaponRange; } }
        public float ProjectileSpeed { get { return _projectileSpeed; } }
        public bool DidKillTarget { get { return _didKillTarget; } }
    }
}




