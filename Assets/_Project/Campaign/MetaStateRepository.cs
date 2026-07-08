namespace Warzone.Campaign
{
    public sealed class MetaStateRepository
    {
        public MetaStateRepository()
        {
            CampaignState = new CampaignState("sandbox_mission");
            RosterState = new RosterState(new OwnedUnitState[0]);
            Settings = new SettingsData(0.8f, 0.7f, 0.8f, 2);
        }

        public CampaignState CampaignState { get; private set; }
        public RosterState RosterState { get; private set; }
        public SettingsData Settings { get; private set; }

        public void SaveCampaignState(CampaignState state)
        {
            CampaignState = state;
        }

        public void SaveRosterState(RosterState state)
        {
            RosterState = state;
        }

        public void SaveSettings(SettingsData settings)
        {
            Settings = settings;
        }
    }
}

