using System.IO;
using UnityEngine;

namespace Warzone.Meta
{
    public sealed class SettingsService : ISettingsService
    {
        private const string SettingsFileName = "warzone.settings.json";
        private SettingsData _settings;

        public SettingsService()
        {
            _settings = Load() ?? new SettingsData(0.8f, 0.7f, 0.8f, 2);
        }

        public SettingsData Current => _settings;

        public void Save(SettingsData settings)
        {
            _settings = settings;
            string path = GetSettingsPath();
            string json = JsonUtility.ToJson(new SettingsDataRecord(settings), true);
            File.WriteAllText(path, json);
        }

        private static SettingsData Load()
        {
            string path = GetSettingsPath();
            if (!File.Exists(path))
            {
                return null;
            }

            string json = File.ReadAllText(path);
            if (string.IsNullOrEmpty(json))
            {
                return null;
            }

            SettingsDataRecord record = JsonUtility.FromJson<SettingsDataRecord>(json);
            return record != null ? record.ToSettingsData() : null;
        }

        private static string GetSettingsPath()
        {
            return Path.Combine(Application.persistentDataPath, SettingsFileName);
        }

        [System.Serializable]
        private sealed class SettingsDataRecord
        {
            public float masterVolume;
            public float musicVolume;
            public float sfxVolume;
            public int graphicsQuality;

            public SettingsDataRecord(SettingsData settings)
            {
                masterVolume = settings.MasterVolume;
                musicVolume = settings.MusicVolume;
                sfxVolume = settings.SfxVolume;
                graphicsQuality = settings.GraphicsQuality;
            }

            public SettingsData ToSettingsData()
            {
                return new SettingsData(masterVolume, musicVolume, sfxVolume, graphicsQuality);
            }
        }
    }
}
