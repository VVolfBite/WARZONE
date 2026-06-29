using Warzone.Application;

namespace Warzone.Controls
{
    public sealed class MainMenuScreenController
    {
        private readonly IMainMenuFlow _mainMenuFlow;

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
                SecondaryActionLabel = "Exit",
            };
        }

        public void StartDemo()
        {
            _mainMenuFlow.StartDemo();
        }
    }
}
