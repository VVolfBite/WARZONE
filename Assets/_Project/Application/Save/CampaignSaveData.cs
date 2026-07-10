using System.Collections.Generic;

namespace Warzone.Application.Save
{
    public sealed class CampaignSaveData
    {
        public CampaignSaveData()
        {
            CampaignId = null;
            CampaignTime = 0f;
            CurrentMission = null;
            Roster = new RosterSaveData();
            Inventory = new InventorySaveData();
            ResourceLedger = new ResourceLedgerSaveData();
            Sites = new List<SiteSaveData>();
            Base = null;
            Outposts = new List<OutpostSaveData>();
            MissionHistory = new List<MissionHistorySaveData>();
        }

        public string CampaignId { get; set; }
        public float CampaignTime { get; set; }
        public CurrentMissionSaveData CurrentMission { get; set; }
        public RosterSaveData Roster { get; set; }
        public InventorySaveData Inventory { get; set; }
        public ResourceLedgerSaveData ResourceLedger { get; set; }
        public List<SiteSaveData> Sites { get; set; }
        public BaseSaveData Base { get; set; }
        public List<OutpostSaveData> Outposts { get; set; }
        public List<MissionHistorySaveData> MissionHistory { get; set; }
    }
}
