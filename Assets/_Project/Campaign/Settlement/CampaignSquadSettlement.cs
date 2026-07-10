namespace Warzone.Campaign
{
    public sealed class CampaignSquadSettlement
    {
        public CampaignSquadSettlement(int squadId, bool isAvailable, int experienceGained)
        {
            SquadId = squadId;
            IsAvailable = isAvailable;
            ExperienceGained = experienceGained;
        }

        public int SquadId { get; private set; }
        public bool IsAvailable { get; private set; }
        public int ExperienceGained { get; private set; }
    }
}
