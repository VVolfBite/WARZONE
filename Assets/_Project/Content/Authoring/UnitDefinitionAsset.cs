using UnityEngine;
using Warzone.Content.Definitions;

namespace Warzone.Content.Authoring
{
    [CreateAssetMenu(menuName = "Warzone/Content/Unit Definition", fileName = "UnitDefinition")]
    public sealed class UnitDefinitionAsset : ScriptableObject
    {
        [SerializeField] private string id = "unit.default";
        [SerializeField] private string displayName = "Unit";
        [SerializeField] private FactionId factionId = FactionId.Player;
        [SerializeField] private int maxHealth = 10;
        [SerializeField] private float moveSpeed = 4f;
        [SerializeField] private float aggroRange = 10f;
        [SerializeField] private float collisionRadius = 0.65f;
        [SerializeField] private ArmorType armorType = ArmorType.Light;
        [SerializeField] private StatusEffectAsset defaultStatusEffect;
        [SerializeField] private string activeAbilityId;
        [SerializeField] private WeaponDefinitionAsset weapon;

        public string Id => id;
        public string DisplayName => displayName;
        public FactionId FactionId => factionId;
        public int MaxHealth => maxHealth;
        public float MoveSpeed => moveSpeed;
        public float AggroRange => aggroRange;
        public float CollisionRadius => collisionRadius;
        public ArmorType ArmorType => armorType;
        public StatusEffectAsset DefaultStatusEffect => defaultStatusEffect;
        public string ActiveAbilityId => activeAbilityId;
        public WeaponDefinitionAsset Weapon => weapon;
    }
}
