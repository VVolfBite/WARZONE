using UnityEngine;

namespace Warzone.Content.Authoring
{
    [CreateAssetMenu(menuName = "Warzone/Content/Weapon Definition", fileName = "WeaponDefinition")]
    public sealed class WeaponDefinitionAsset : ScriptableObject
    {
        [SerializeField] private string id = "weapon.default";
        [SerializeField] private float range = 1.5f;
        [SerializeField] private float attackIntervalSeconds = 1f;
        [SerializeField] private int damagePerHit = 1;
        [SerializeField] private float projectileSpeed = 18f;

        public string Id => id;
        public float Range => range;
        public float AttackIntervalSeconds => attackIntervalSeconds;
        public int DamagePerHit => damagePerHit;
        public float ProjectileSpeed => projectileSpeed;
    }
}
