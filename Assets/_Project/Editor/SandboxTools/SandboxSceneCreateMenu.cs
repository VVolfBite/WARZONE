using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using Warzone.Sandbox.BattleSandbox;

namespace Warzone.Editor.SandboxTools
{
    public static class SandboxSceneCreateMenu
    {
        private const string SceneDirectory = "Assets/Scenes/Sandbox";
        private const string ScenePath = "Assets/Scenes/Sandbox/Sandbox_M5_Integrated.unity";

        [MenuItem("Warzone/Sandbox/Create M5 Integrated Sandbox Scene")]
        public static void CreateM5IntegratedSandboxScene()
        {
            if (!Directory.Exists(SceneDirectory))
            {
                Directory.CreateDirectory(SceneDirectory);
            }

            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);

            GameObject launcherObject = new GameObject("M5 Sandbox Launcher");
            BattleSandboxLauncher launcher = launcherObject.AddComponent<BattleSandboxLauncher>();
            launcher.Mode = BattleSandboxMode.M5IntegratedSandbox;

            EditorSceneManager.SaveScene(scene, ScenePath);
            Selection.activeGameObject = launcherObject;
            EditorUtility.DisplayDialog("Warzone", "Created " + ScenePath, "OK");
        }
    }
}
