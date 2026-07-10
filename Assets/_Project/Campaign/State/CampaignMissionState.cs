namespace Warzone.Campaign
{
    public sealed class CampaignMissionState
    {
        public CampaignMissionState(
            string missionId,
            string siteId,
            string displayName = null,
            bool isAvailable = true,
            int threatLevel = 0,
            string missionType = null)
        {
            MissionId = missionId;
            SiteId = siteId;
            DisplayName = displayName ?? missionId;
            IsAvailable = isAvailable;
            ThreatLevel = threatLevel;
            MissionType = missionType;
        }

        public string MissionId { get; private set; }
        public string SiteId { get; private set; }
        public string DisplayName { get; private set; }
        public bool IsAvailable { get; private set; }
        public int ThreatLevel { get; private set; }
        public string MissionType { get; private set; }
    }
}
