namespace Warzone.Content.Definitions
{
    public sealed class WeaponRewardDefinition
    {
        public WeaponRewardDefinition(string weaponDefinitionId, int count = 1)
        {
            WeaponDefinitionId = weaponDefinitionId;
            Count = count;
        }

        public string WeaponDefinitionId { get; private set; }
        public int Count { get; private set; }
    }
}
