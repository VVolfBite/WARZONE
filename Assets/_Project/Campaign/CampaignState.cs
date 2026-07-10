using System.Collections.Generic;

namespace Warzone.Campaign
{
    public sealed class CampaignState
    {
        private readonly Dictionary<string, CampaignSiteState> _sitesById = new Dictionary<string, CampaignSiteState>();
        private readonly List<CampaignMissionHistoryRecord> _missionHistory = new List<CampaignMissionHistoryRecord>();

        public CampaignState()
            : this(null)
        {
        }

        public CampaignState(string currentMissionId)
        {
            CurrentMissionId = currentMissionId;
            Roster = new CampaignRosterState();
            Inventory = new CampaignInventoryState();
            ResourceLedger = new CampaignResourceLedgerState();
            CampaignTime = 0f;
        }

        public string CurrentMissionId { get; private set; }
        public CampaignRosterState Roster { get; private set; }
        public CampaignInventoryState Inventory { get; private set; }
        public CampaignResourceLedgerState ResourceLedger { get; private set; }
        public float CampaignTime { get; private set; }
        public CampaignMissionState CurrentMission { get; private set; }

        public IReadOnlyDictionary<string, CampaignSiteState> SitesById
        {
            get { return _sitesById; }
        }

        public IReadOnlyList<CampaignMissionHistoryRecord> MissionHistory
        {
            get { return _missionHistory; }
        }

        public void SetCurrentMissionId(string missionId)
        {
            CurrentMissionId = missionId;
        }

        public void SetCurrentMission(CampaignMissionState mission)
        {
            CurrentMission = mission;
            CurrentMissionId = mission != null ? mission.MissionId : null;
        }

        public void SetCampaignTime(float campaignTime)
        {
            CampaignTime = campaignTime < 0f ? 0f : campaignTime;
        }

        public void AdvanceCampaignTime(float deltaTime)
        {
            if (deltaTime <= 0f)
            {
                return;
            }

            CampaignTime += deltaTime;
        }

        public void AddSite(CampaignSiteState site)
        {
            if (site == null || string.IsNullOrEmpty(site.SiteId))
            {
                return;
            }

            _sitesById[site.SiteId] = site;
        }

        public bool TryGetSite(string siteId, out CampaignSiteState site)
        {
            return _sitesById.TryGetValue(siteId, out site);
        }

        public void AddMissionHistory(CampaignMissionHistoryRecord record)
        {
            if (record != null)
            {
                _missionHistory.Add(record);
            }
        }

        public void AddMember(CampaignMemberState member)
        {
            Roster.AddMember(member);
        }

        public void AddSquad(CampaignSquadState squad)
        {
            Roster.AddSquad(squad);
        }
    }
}
