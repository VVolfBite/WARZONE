using UnityEngine;

namespace Warzone.Content.Authoring
{
    [CreateAssetMenu(menuName = "Warzone/Content/Content Catalog", fileName = "ContentCatalog")]
    public sealed class ContentCatalogAsset : ScriptableObject
    {
        [SerializeField] private UnitDefinitionAsset[] unitDefinitions;
        [SerializeField] private MissionDefinitionAsset[] missionDefinitions;
        [SerializeField] private StatusEffectAsset[] statusEffects;

        public UnitDefinitionAsset[] UnitDefinitions => unitDefinitions;
        public MissionDefinitionAsset[] MissionDefinitions => missionDefinitions;
        public StatusEffectAsset[] StatusEffects => statusEffects;
    }
}


