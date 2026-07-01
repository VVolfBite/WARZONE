using Warzone.Combat;

namespace Warzone.Meta
{
    public interface ISaveService
    {
        void SaveSettings(SettingsData settings);
        SettingsData LoadSettings();
        void SaveBestScore(string missionId, BattleStatistics statistics);
        BattleStatistics LoadBestScore(string missionId);
    }
}
