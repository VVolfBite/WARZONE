namespace Warzone.Content.Definitions
{
    public sealed class VisionEquipmentDefinition
    {
        public VisionEquipmentDefinition(string id, string displayName, int nightVisionLevel, int smokeVisionLevel)
        {
            Id = id;
            DisplayName = displayName;
            NightVisionLevel = nightVisionLevel;
            SmokeVisionLevel = smokeVisionLevel;
        }

        public string Id { get; private set; }
        public string DisplayName { get; private set; }
        public int NightVisionLevel { get; private set; }
        public int SmokeVisionLevel { get; private set; }
    }
}
