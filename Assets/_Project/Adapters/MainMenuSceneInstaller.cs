using UnityEngine;
using UnityEngine.SceneManagement;
using Warzone.Application;
using Warzone.Controls;

namespace Warzone.Adapters
{
    public static class MainMenuSceneInstaller
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Install()
        {
            Scene activeScene = SceneManager.GetActiveScene();
            if (!activeScene.IsValid() || activeScene.name != "MainMenu")
            {
                return;
            }

            Camera mainCamera = EnsureCamera();
            mainCamera.backgroundColor = new Color(0.06f, 0.08f, 0.1f);
            mainCamera.clearFlags = CameraClearFlags.SolidColor;

            SceneFlow sceneFlow = new SceneFlow("MainMenu", "SampleScene");
            MainMenuScreenController controller = new MainMenuScreenController(sceneFlow);
            MainMenuScreen menuScreen = EnsureMainMenuScreen();
            menuScreen.Configure(controller);
        }

        private static Camera EnsureCamera()
        {
            Camera mainCamera = Camera.main;
            if (mainCamera != null)
            {
                return mainCamera;
            }

            GameObject cameraObject = new GameObject("Main Camera");
            cameraObject.tag = "MainCamera";
            cameraObject.transform.position = new Vector3(0f, 8f, -10f);
            cameraObject.transform.rotation = Quaternion.Euler(20f, 0f, 0f);
            mainCamera = cameraObject.AddComponent<Camera>();
            cameraObject.AddComponent<AudioListener>();
            return mainCamera;
        }

        private static MainMenuScreen EnsureMainMenuScreen()
        {
            MainMenuScreen existing = Object.FindFirstObjectByType<MainMenuScreen>();
            if (existing != null)
            {
                return existing;
            }

            return new GameObject("MainMenuScreen").AddComponent<MainMenuScreen>();
        }
    }
}
