using System.Collections.Generic;

namespace Warzone.Campaign
{
    public sealed class CampaignSettlement
    {
        public CampaignSettlement(
            bool missionCompleted,
            int unitsLost,
            int unitsKept)
            : this(
                missionId: null,
                siteId: null,
                missionCompleted: missionCompleted,
                casualties: new List<CampaignCasualtySettlement>(),
                loot: new List<CampaignLootSettlement>(),
                siteSettlements: new List<CampaignSiteSettlement>(),
                squadSettlements: new List<CampaignSquadSettlement>(),
                historyRecord: null,
                unitsLost: unitsLost,
                unitsKept: unitsKept)
        {
        }

        public CampaignSettlement(
            string missionId,
            string siteId,
            bool missionCompleted,
            IReadOnlyList<CampaignCasualtySettlement> casualties,
            IReadOnlyList<CampaignLootSettlement> loot,
            IReadOnlyList<CampaignSiteSettlement> siteSettlements,
            IReadOnlyList<CampaignSquadSettlement> squadSettlements,
            CampaignMissionHistoryRecord historyRecord,
            int unitsLost = 0,
            int unitsKept = 0)
        {
            MissionId = missionId;
            SiteId = siteId;
            MissionCompleted = missionCompleted;
            Casualties = casualties ?? new List<CampaignCasualtySettlement>();
            Loot = loot ?? new List<CampaignLootSettlement>();
            SiteSettlements = siteSettlements ?? new List<CampaignSiteSettlement>();
            SquadSettlements = squadSettlements ?? new List<CampaignSquadSettlement>();
            HistoryRecord = historyRecord;
            UnitsLost = unitsLost;
            UnitsKept = unitsKept;
        }

        public string MissionId { get; private set; }
        public string SiteId { get; private set; }
        public bool MissionCompleted { get; private set; }
        public IReadOnlyList<CampaignCasualtySettlement> Casualties { get; private set; }
        public IReadOnlyList<CampaignLootSettlement> Loot { get; private set; }
        public IReadOnlyList<CampaignSiteSettlement> SiteSettlements { get; private set; }
        public IReadOnlyList<CampaignSquadSettlement> SquadSettlements { get; private set; }
        public CampaignMissionHistoryRecord HistoryRecord { get; private set; }
        public int UnitsLost { get; private set; }
        public int UnitsKept { get; private set; }
    }
}
