namespace Warzone.Meta
{
    public interface ISettingsService
    {
        SettingsData Current { get; }
        void Save(SettingsData settings);
    }
}
