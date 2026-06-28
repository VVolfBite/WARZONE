namespace Warzone.Meta
{
    public sealed class CampaignState
    {
        public CampaignState(string currentMissionId)
        {
            CurrentMissionId = currentMissionId;
        }

        public string CurrentMissionId { get; }
    }
}
