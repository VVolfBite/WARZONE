using System.IO;
using UnityEngine;
using Warzone.Campaign;
using Warzone.Combat;

namespace Warzone.Runtime.Persistence
{
    public sealed class JsonSaveService : ISaveService
    {
        private const string SettingsFileName = "warzone.settings.json";
        private const string BestScorePrefix = "warzone.bestscore.";

        public void SaveSettings(SettingsData settings)
        {
            File.WriteAllText(GetSettingsPath(), JsonUtility.ToJson(new SettingsRecord(settings), true));
        }

        public SettingsData LoadSettings()
        {
            string path = GetSettingsPath();
            if (!File.Exists(path))
            {
                return null;
            }

            SettingsRecord record = JsonUtility.FromJson<SettingsRecord>(File.ReadAllText(path));
            return record != null ? record.ToSettingsData() : null;
        }

        public void SaveBestScore(string missionId, BattleStatistics statistics)
        {
            if (string.IsNullOrEmpty(missionId))
            {
                return;
            }

            File.WriteAllText(GetBestScorePath(missionId), JsonUtility.ToJson(new BestScoreRecord(statistics), true));
        }

        public BattleStatistics LoadBestScore(string missionId)
        {
            string path = GetBestScorePath(missionId);
            if (!File.Exists(path))
            {
                return null;
            }

            BestScoreRecord record = JsonUtility.FromJson<BestScoreRecord>(File.ReadAllText(path));
            return record != null ? record.ToBattleStatistics() : null;
        }

        private static string GetSettingsPath()
        {
            return Path.Combine(Application.persistentDataPath, SettingsFileName);
        }

        private static string GetBestScorePath(string missionId)
        {
            return Path.Combine(Application.persistentDataPath, BestScorePrefix + missionId + ".json");
        }

        [System.Serializable]
        private sealed class SettingsRecord
        {
            public float masterVolume;
            public float musicVolume;
            public float sfxVolume;
            public int graphicsQuality;

            public SettingsRecord(SettingsData settings)
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

        [System.Serializable]
        private sealed class BestScoreRecord
        {
            public int playerUnitsRemaining;
            public int enemyUnitsRemaining;

            public BestScoreRecord(BattleStatistics statistics)
            {
                playerUnitsRemaining = statistics.PlayerUnitsRemaining;
                enemyUnitsRemaining = statistics.EnemyUnitsRemaining;
            }

            public BattleStatistics ToBattleStatistics()
            {
                return new BattleStatistics(playerUnitsRemaining, enemyUnitsRemaining);
            }
        }
    }
}


