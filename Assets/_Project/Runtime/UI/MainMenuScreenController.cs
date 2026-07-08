using Warzone.Application;
using Warzone.Campaign;
using Warzone.Runtime.Persistence;

namespace Warzone.Runtime.UI
{
    public sealed class MainMenuScreenController
    {
        private readonly IMainMenuFlow _mainMenuFlow;
        private readonly ISettingsService _settingsService;
        private readonly ISaveService _saveService;

        public MainMenuScreenController(IMainMenuFlow mainMenuFlow, ISettingsService settingsService)
        {
            _mainMenuFlow = mainMenuFlow;
            _settingsService = settingsService;
            _saveService = new JsonSaveService();
        }

        public MainMenuViewModel BuildViewModel()
        {
            return new MainMenuViewModel
            {
                Title = "WARZONE",
                Subtitle = "PMC Strike Team vs Red Hand Militia",
                PrimaryActionLabel = "New Operation",
                TertiaryActionLabel = "Credits",
                SecondaryActionLabel = "Exit",
                BestScoreSummary = BuildBestScoreSummary(),
                MasterVolume = _settingsService.Current.MasterVolume,
                MusicVolume = _settingsService.Current.MusicVolume,
                GraphicsQuality = _settingsService.Current.GraphicsQuality,
            };
        }

        public void StartDemo()
        {
            _mainMenuFlow.StartDemo();
        }

        public void SaveSettings(float masterVolume, float musicVolume, int graphicsQuality)
        {
            float sfxVolume = _settingsService.Current.SfxVolume;
            _settingsService.Save(new SettingsData(masterVolume, musicVolume, sfxVolume, graphicsQuality));
        }

        private string BuildBestScoreSummary()
        {
            Warzone.Combat.BattleStatistics statistics = _saveService.LoadBestScore("mission.sandbox");
            if (statistics == null)
            {
                return "Best Result: None";
            }

            return "Best Result: " + statistics.PlayerUnitsRemaining + " survivors, " + statistics.EnemyUnitsRemaining + " enemies left";
        }
    }
}




