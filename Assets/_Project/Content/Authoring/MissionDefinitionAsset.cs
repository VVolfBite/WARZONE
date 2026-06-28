using UnityEngine;

namespace Warzone.Content.Authoring
{
    [CreateAssetMenu(menuName = "Warzone/Content/Mission Definition", fileName = "MissionDefinition")]
    public sealed class MissionDefinitionAsset : ScriptableObject
    {
        [SerializeField] private string id = "mission.sandbox";
        [SerializeField] private string displayName = "Sandbox";
        [SerializeField] private int playerSquadCount = 1;
        [SerializeField] private int enemySquadCount = 1;

        public string Id => id;
        public string DisplayName => displayName;
        public int PlayerSquadCount => playerSquadCount;
        public int EnemySquadCount => enemySquadCount;
    }
}
