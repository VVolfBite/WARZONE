namespace Warzone.Application.Save
{
    public sealed class SaveGameMetadata
    {
        public SaveGameMetadata()
        {
            SaveVersion = SaveGameVersion.Current;
            CreatedAtUtcTicks = 0L;
            SlotId = null;
            CampaignId = null;
        }

        public int SaveVersion { get; set; }
        public long CreatedAtUtcTicks { get; set; }
        public string SlotId { get; set; }
        public string CampaignId { get; set; }
    }
}
