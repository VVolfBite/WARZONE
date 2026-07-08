using Warzone.Campaign;
using Warzone.Combat;

namespace Warzone.Runtime.Persistence
{
    public interface ISaveService
    {
        void SaveSettings(SettingsData settings);
        SettingsData LoadSettings();
        void SaveBestScore(string missionId, BattleStatistics statistics);
        BattleStatistics LoadBestScore(string missionId);
    }
}


