using UnityEngine;
using UnityEngine.SceneManagement;

namespace Warzone.Application.Composition
{
    public sealed class GameBootstrap : MonoBehaviour
    {
        [SerializeField] private string initialSceneName = "MainMenu";

        private void Start()
        {
            if (SceneManager.GetActiveScene().name == initialSceneName)
            {
                return;
            }

            SceneManager.LoadScene(initialSceneName);
        }
    }
}



