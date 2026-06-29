using UnityEngine.SceneManagement;

namespace Warzone.Application
{
    public sealed class SceneFlow : IMainMenuFlow
    {
        private readonly string _menuSceneName;
        private readonly string _battleSceneName;

        public SceneFlow(string menuSceneName, string battleSceneName)
        {
            _menuSceneName = menuSceneName;
            _battleSceneName = battleSceneName;
        }

        public void StartDemo()
        {
            if (SceneManager.GetActiveScene().name == _battleSceneName)
            {
                return;
            }

            SceneManager.LoadScene(_battleSceneName);
        }

        public void ReturnToMenu()
        {
            if (SceneManager.GetActiveScene().name == _menuSceneName)
            {
                return;
            }

            SceneManager.LoadScene(_menuSceneName);
        }
    }
}
