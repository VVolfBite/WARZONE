namespace Warzone.Meta
{
    public sealed class MetaStateRepository
    {
        public CampaignState CampaignState { get; private set; } = new CampaignState("sandbox_mission");
        public RosterState RosterState { get; private set; } = new RosterState(new OwnedUnitState[0]);
        public SettingsData Settings { get; private set; } = new SettingsData(0.8f, 0.8f, 0.7f, 2);

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
