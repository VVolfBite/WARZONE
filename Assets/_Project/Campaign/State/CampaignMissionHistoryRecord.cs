using System.Collections.Generic;

namespace Warzone.Campaign
{
    public sealed class CampaignMissionHistoryRecord
    {
        public CampaignMissionHistoryRecord(
            string missionId,
            string siteId,
            bool succeeded,
            int casualties,
            int loot,
            float timestamp,
            string completionType = null,
            IReadOnlyDictionary<string, int> resourceRewards = null)
        {
            MissionId = missionId;
            SiteId = siteId;
            Succeeded = succeeded;
            Casualties = casualties;
            Loot = loot;
            Timestamp = timestamp;
            CompletionType = completionType;
            ResourceRewards = resourceRewards ?? new Dictionary<string, int>();
        }

        public string MissionId { get; private set; }
        public string SiteId { get; private set; }
        public bool Succeeded { get; private set; }
        public int Casualties { get; private set; }
        public int Loot { get; private set; }
        public float Timestamp { get; private set; }
        public string CompletionType { get; private set; }
        public IReadOnlyDictionary<string, int> ResourceRewards { get; private set; }
    }
}
