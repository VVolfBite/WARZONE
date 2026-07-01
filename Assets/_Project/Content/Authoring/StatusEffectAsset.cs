using UnityEngine;

namespace Warzone.Content.Authoring
{
    [CreateAssetMenu(menuName = "Warzone/Content/Status Effect", fileName = "StatusEffect")]
    public sealed class StatusEffectAsset : ScriptableObject
    {
        [SerializeField] private string id = "effect.default";
        [SerializeField] private string displayName = "Effect";
        [SerializeField] private float durationSeconds = 3f;
        [SerializeField] private float tickIntervalSeconds = 1f;
        [SerializeField] private int flatDamagePerTick = 0;
        [SerializeField] private int flatHealingPerTick = 0;
        [SerializeField] private float moveSpeedMultiplier = 1f;
        [SerializeField] private float rangeMultiplier = 1f;

        public string Id => id;
        public string DisplayName => displayName;
        public float DurationSeconds => durationSeconds;
        public float TickIntervalSeconds => tickIntervalSeconds;
        public int FlatDamagePerTick => flatDamagePerTick;
        public int FlatHealingPerTick => flatHealingPerTick;
        public float MoveSpeedMultiplier => moveSpeedMultiplier;
        public float RangeMultiplier => rangeMultiplier;
    }
}
