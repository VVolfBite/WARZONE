using Warzone.Application;
using Warzone.Meta;

namespace Warzone.Controls
{
    public sealed class MainMenuScreenController
    {
        private readonly IMainMenuFlow _mainMenuFlow;
        private readonly SettingsService _settingsService = new SettingsService();

        public MainMenuScreenController(IMainMenuFlow mainMenuFlow)
        {
            _mainMenuFlow = mainMenuFlow;
        }

        public MainMenuViewModel BuildViewModel()
        {
            return new MainMenuViewModel
            {
                Title = "WARZONE",
                Subtitle = "RTS Prototype",
                PrimaryActionLabel = "Demo",
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
            _settingsService.Save(new SettingsData(masterVolume, masterVolume, musicVolume, graphicsQuality));
        }
    }
}
