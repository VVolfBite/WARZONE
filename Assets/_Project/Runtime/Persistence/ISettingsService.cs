using Warzone.Campaign;

namespace Warzone.Runtime.Persistence
{
    public interface ISettingsService
    {
        SettingsData Current { get; }
        void Save(SettingsData settings);
    }
}




