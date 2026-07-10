namespace Warzone.Campaign
{
    public sealed class CampaignResourceRewardSettlement
    {
        public CampaignResourceRewardSettlement(string resourceId, int count, string sourceId = null)
        {
            ResourceId = resourceId;
            Count = count;
            SourceId = sourceId;
        }

        public string ResourceId { get; private set; }
        public int Count { get; private set; }
        public string SourceId { get; private set; }
    }
}
