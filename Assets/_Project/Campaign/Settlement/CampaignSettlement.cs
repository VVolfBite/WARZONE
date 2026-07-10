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
                resourceRewards: new List<CampaignResourceRewardSettlement>(),
                itemRewards: new List<CampaignItemRewardSettlement>(),
                weaponRewards: new List<CampaignWeaponRewardSettlement>(),
                baseEffects: new List<CampaignBaseEffectSettlement>(),
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
            IReadOnlyList<CampaignResourceRewardSettlement> resourceRewards = null,
            IReadOnlyList<CampaignItemRewardSettlement> itemRewards = null,
            IReadOnlyList<CampaignWeaponRewardSettlement> weaponRewards = null,
            IReadOnlyList<CampaignBaseEffectSettlement> baseEffects = null,
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
            ResourceRewards = resourceRewards ?? new List<CampaignResourceRewardSettlement>();
            ItemRewards = itemRewards ?? new List<CampaignItemRewardSettlement>();
            WeaponRewards = weaponRewards ?? new List<CampaignWeaponRewardSettlement>();
            BaseEffects = baseEffects ?? new List<CampaignBaseEffectSettlement>();
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
        public IReadOnlyList<CampaignResourceRewardSettlement> ResourceRewards { get; private set; }
        public IReadOnlyList<CampaignItemRewardSettlement> ItemRewards { get; private set; }
        public IReadOnlyList<CampaignWeaponRewardSettlement> WeaponRewards { get; private set; }
        public IReadOnlyList<CampaignBaseEffectSettlement> BaseEffects { get; private set; }
        public int UnitsLost { get; private set; }
        public int UnitsKept { get; private set; }
    }
}
