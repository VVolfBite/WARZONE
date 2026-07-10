namespace Warzone.Application.Save
{
    public sealed class SaveGameSnapshot
    {
        public SaveGameSnapshot()
        {
            Metadata = new SaveGameMetadata();
            Campaign = new CampaignSaveData();
            SaveVersion = SaveGameVersion.Current;
            CreatedAtUtcTicks = 0L;
            SlotId = null;
            CampaignId = null;
        }

        public int SaveVersion { get; set; }
        public long CreatedAtUtcTicks { get; set; }
        public string SlotId { get; set; }
        public string CampaignId { get; set; }
        public SaveGameMetadata Metadata { get; set; }
        public CampaignSaveData Campaign { get; set; }
    }
}
