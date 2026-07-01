using Warzone.Application;
using Warzone.Meta;

namespace Warzone.Controls
{
    public sealed class MainMenuScreenController
    {
        private readonly IMainMenuFlow _mainMenuFlow;
        private readonly ISettingsService _settingsService;

        public MainMenuScreenController(IMainMenuFlow mainMenuFlow, ISettingsService settingsService)
        {
            _mainMenuFlow = mainMenuFlow;
            _settingsService = settingsService;
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
    }
}
