using Warzone.Meta;

namespace Warzone.Adapters
{
    public static class RuntimeServiceRegistry
    {
        private static ISettingsService _settingsService;

        public static ISettingsService SettingsService
        {
            get
            {
                if (_settingsService == null)
                {
                    _settingsService = new SettingsService();
                }

                return _settingsService;
            }
        }
    }
}
