using Warzone.Application.Save;
using Warzone.Campaign;

namespace Warzone.Application.Services
{
    public sealed class SaveGameService
    {
        private readonly CampaignLifecycleService _lifecycleService;
        private readonly ISaveGameSerializer _serializer;
        private readonly ISaveGameRepository _repository;

        public SaveGameService()
            : this(new CampaignLifecycleService(), new JsonSaveGameSerializer(), new InMemorySaveGameRepository())
        {
        }

        public SaveGameService(CampaignLifecycleService lifecycleService, ISaveGameSerializer serializer, ISaveGameRepository repository)
        {
            _lifecycleService = lifecycleService;
            _serializer = serializer;
            _repository = repository;
        }

        public bool SaveCampaign(string slotId, CampaignState campaignState, out string reason)
        {
            reason = null;
            if (_lifecycleService == null || _serializer == null || _repository == null)
            {
                reason = "Save service is not configured.";
                return false;
            }

            SaveGameSnapshot snapshot = _lifecycleService.CreateSnapshot(campaignState, slotId);
            if (snapshot == null)
            {
                reason = "Could not create save snapshot.";
                return false;
            }

            string data = _serializer.Serialize(snapshot);
            if (string.IsNullOrWhiteSpace(data))
            {
                reason = "Could not serialize save snapshot.";
                return false;
            }

            return _repository.Save(slotId, data, out reason);
        }

        public bool TryLoadCampaign(string slotId, out CampaignState campaignState, out string reason)
        {
            campaignState = null;
            reason = null;

            if (_lifecycleService == null || _serializer == null || _repository == null)
            {
                reason = "Save service is not configured.";
                return false;
            }

            string data;
            if (!_repository.TryLoad(slotId, out data, out reason))
            {
                return false;
            }

            SaveGameSnapshot snapshot;
            if (!_serializer.TryDeserialize(data, out snapshot, out reason))
            {
                return false;
            }

            campaignState = _lifecycleService.LoadFromSnapshot(snapshot);
            if (campaignState == null)
            {
                reason = "Could not load campaign snapshot.";
                return false;
            }

            return true;
        }
    }
}
