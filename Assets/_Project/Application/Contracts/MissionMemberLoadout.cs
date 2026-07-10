namespace Warzone.Application
{
    public sealed class MissionMemberLoadout
    {
        public MissionMemberLoadout(
            string campaignMemberId,
            int battleMemberId,
            string displayName,
            int squadId,
            string weaponDefinitionId,
            int maxHealth,
            float baseMovementSpeed,
            float detectionRange,
            int nightVisionLevel = 0,
            int smokeVisionLevel = 0,
            float accuracyModifier = 1f,
            bool hasLightSource = false)
        {
            CampaignMemberId = campaignMemberId;
            BattleMemberId = battleMemberId;
            DisplayName = displayName;
            SquadId = squadId;
            WeaponDefinitionId = weaponDefinitionId;
            MaxHealth = maxHealth;
            BaseMovementSpeed = baseMovementSpeed;
            DetectionRange = detectionRange;
            NightVisionLevel = nightVisionLevel;
            SmokeVisionLevel = smokeVisionLevel;
            AccuracyModifier = accuracyModifier;
            HasLightSource = hasLightSource;
        }

        public string CampaignMemberId { get; private set; }
        public int BattleMemberId { get; private set; }
        public string DisplayName { get; private set; }
        public int SquadId { get; private set; }
        public string WeaponDefinitionId { get; private set; }
        public int MaxHealth { get; private set; }
        public float BaseMovementSpeed { get; private set; }
        public float DetectionRange { get; private set; }
        public int NightVisionLevel { get; private set; }
        public int SmokeVisionLevel { get; private set; }
        public float AccuracyModifier { get; private set; }
        public bool HasLightSource { get; private set; }
    }
}
