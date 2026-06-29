namespace Warzone.Meta
{
    public sealed class SettingsService
    {
        private SettingsData _settings = new SettingsData(0.8f, 0.8f, 0.7f, 2);

        public SettingsData Current => _settings;

        public void Save(SettingsData settings)
        {
            _settings = settings;
        }
    }
}
