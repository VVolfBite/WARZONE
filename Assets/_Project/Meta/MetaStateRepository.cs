namespace Warzone.Meta
{
    public sealed class MetaStateRepository
    {
        public CampaignState CampaignState { get; private set; } = new CampaignState("sandbox_mission");
        public RosterState RosterState { get; private set; } = new RosterState(new OwnedUnitState[0]);

        public void SaveCampaignState(CampaignState state)
        {
            CampaignState = state;
        }

        public void SaveRosterState(RosterState state)
        {
            RosterState = state;
        }
    }
}
