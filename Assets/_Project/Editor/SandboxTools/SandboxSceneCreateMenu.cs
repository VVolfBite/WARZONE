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
        private const string M5ScenePath = "Assets/Scenes/Sandbox/Sandbox_M5_Integrated.unity";
        private const string M6ScenePath = "Assets/Scenes/Sandbox/Sandbox_M6_PressureRetreat.unity";

        [MenuItem("Warzone/Sandbox/Create M5 Integrated Sandbox Scene")]
        public static void CreateM5IntegratedSandboxScene()
        {
            CreateSandboxScene("M5 Sandbox Launcher", BattleSandboxMode.M5IntegratedSandbox, M5ScenePath);
        }

        [MenuItem("Warzone/Sandbox/Create M6 Pressure Retreat Sandbox Scene")]
        public static void CreateM6PressureRetreatSandboxScene()
        {
            CreateSandboxScene("M6 Sandbox Launcher", BattleSandboxMode.M6PressureRetreat, M6ScenePath);
        }

        private static void CreateSandboxScene(string launcherName, BattleSandboxMode mode, string scenePath)
        {
            if (!Directory.Exists(SceneDirectory))
            {
                Directory.CreateDirectory(SceneDirectory);
            }

            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);

            GameObject launcherObject = new GameObject(launcherName);
            BattleSandboxLauncher launcher = launcherObject.AddComponent<BattleSandboxLauncher>();
            launcher.Mode = mode;

            EditorSceneManager.SaveScene(scene, scenePath);
            Selection.activeGameObject = launcherObject;
            EditorUtility.DisplayDialog("Warzone", "Created " + scenePath, "OK");
        }
    }
}
