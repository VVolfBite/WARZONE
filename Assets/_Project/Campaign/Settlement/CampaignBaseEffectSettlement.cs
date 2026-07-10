namespace Warzone.Campaign
{
    public sealed class CampaignBaseEffectSettlement
    {
        public CampaignBaseEffectSettlement(string baseId, bool isOperational, string warning = null)
        {
            BaseId = baseId;
            IsOperational = isOperational;
            Warning = warning;
        }

        public string BaseId { get; private set; }
        public bool IsOperational { get; private set; }
        public string Warning { get; private set; }
    }
}
