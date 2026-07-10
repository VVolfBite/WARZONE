using Warzone.Application.Save;
using Warzone.Campaign;

namespace Warzone.Application.Services
{
    public sealed class CampaignLifecycleService
    {
        private readonly StartingCampaignFactory _startingCampaignFactory;
        private readonly CampaignSaveMapper _saveMapper;

        public CampaignLifecycleService()
            : this(new StartingCampaignFactory(), new CampaignSaveMapper())
        {
        }

        public CampaignLifecycleService(StartingCampaignFactory startingCampaignFactory, CampaignSaveMapper saveMapper)
        {
            _startingCampaignFactory = startingCampaignFactory;
            _saveMapper = saveMapper;
        }

        public CampaignState NewGame()
        {
            return _startingCampaignFactory.CreateStartingCampaign();
        }

        public SaveGameSnapshot CreateSnapshot(CampaignState campaignState, string slotId = null)
        {
            return _saveMapper.ToSnapshot(campaignState, slotId);
        }

        public CampaignState LoadFromSnapshot(SaveGameSnapshot snapshot)
        {
            return _saveMapper.FromSnapshot(snapshot);
        }
    }
}
