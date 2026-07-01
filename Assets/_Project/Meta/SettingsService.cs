using System.IO;
using UnityEngine;

namespace Warzone.Meta
{
    public sealed class SettingsService : ISettingsService
    {
        private readonly ISaveService _saveService;
        private SettingsData _settings;

        public SettingsService()
            : this(new JsonSaveService())
        {
        }

        public SettingsService(ISaveService saveService)
        {
            _saveService = saveService;
            _settings = _saveService.LoadSettings() ?? new SettingsData(0.8f, 0.7f, 0.8f, 2);
        }

        public SettingsData Current => _settings;

        public void Save(SettingsData settings)
        {
            _settings = settings;
            _saveService.SaveSettings(settings);
        }
    }
}
