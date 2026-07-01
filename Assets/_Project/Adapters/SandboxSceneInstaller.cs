using UnityEngine;
using UnityEngine.SceneManagement;
using Warzone.Application;
using Warzone.Controls;
using Warzone.Meta;

namespace Warzone.Adapters
{
    public static class SandboxSceneInstaller
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Install()
        {
            Scene activeScene = SceneManager.GetActiveScene();
            if (!activeScene.IsValid() || activeScene.name != "SampleScene")
            {
                return;
            }

            EnsureGroundPlane();
            Camera mainCamera = EnsureCamera();
            BattleRuntimeHost battleRuntimeHost = new GameObject("BattleRuntimeHost").AddComponent<BattleRuntimeHost>();
            AudioService audioService = new GameObject("AudioService").AddComponent<AudioService>();
            GameFlow gameFlow = new GameFlow();
            ProgressionService progressionService = new ProgressionService();
            ISettingsService settingsService = RuntimeServiceRegistry.SettingsService;
            MissionFlow missionFlow = new MissionFlow(gameFlow, battleRuntimeHost, progressionService);
            MissionStartRequest missionStartRequest = SandboxMissionRequestFactory.CreateDemoMissionRequest();

            SandboxBattleBootstrap battleBootstrap = new GameObject("SandboxBattleBootstrap").AddComponent<SandboxBattleBootstrap>();
            battleBootstrap.Configure(battleRuntimeHost, mainCamera);

            SandboxMissionStarter missionStarter = new GameObject("SandboxMissionStarter").AddComponent<SandboxMissionStarter>();
            missionStarter.Configure(missionFlow, missionStartRequest);

            DebriefScreen debriefScreen = EnsureDebriefScreen();
            SandboxMissionCompletionPresenter missionCompletionPresenter = new GameObject("SandboxMissionCompletionPresenter").AddComponent<SandboxMissionCompletionPresenter>();
            missionCompletionPresenter.Configure(battleRuntimeHost, missionFlow, debriefScreen);
            SandboxAudioListener audioListener = new GameObject("SandboxAudioListener").AddComponent<SandboxAudioListener>();
            audioListener.Configure(battleRuntimeHost, audioService);
            ApplySettings(settingsService.Current);

            missionStarter.StartDemoMission();
        }

        private static void EnsureGroundPlane()
        {
            if (GameObject.Find("GroundPlane") != null)
            {
                return;
            }

            GameObject ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
            ground.name = "GroundPlane";
            ground.transform.position = Vector3.zero;
            ground.transform.localScale = new Vector3(5f, 1f, 5f);
            Renderer renderer = ground.GetComponent<Renderer>();
            renderer.material.color = new Color(0.36f, 0.46f, 0.29f);
            RenderSettings.ambientLight = new Color(0.72f, 0.74f, 0.76f);

            CreateEnvironmentBlock("Warehouse_A", new Vector3(-14f, 1.5f, -11f), new Vector3(4.5f, 3.2f, 6f), new Color(0.62f, 0.62f, 0.66f));
            CreateEnvironmentBlock("Warehouse_B", new Vector3(15f, 1.5f, 10f), new Vector3(4.5f, 3.2f, 6f), new Color(0.58f, 0.58f, 0.62f));
            CreateEnvironmentBlock("ConcreteBarrier_A", new Vector3(-6f, 0.45f, 7f), new Vector3(3.8f, 0.9f, 1.2f), new Color(0.52f, 0.52f, 0.54f));
            CreateEnvironmentBlock("ConcreteBarrier_B", new Vector3(7f, 0.45f, -8f), new Vector3(3.2f, 0.9f, 1.2f), new Color(0.5f, 0.5f, 0.52f));
            CreateEnvironmentBlock("Container_A", new Vector3(3f, 0.85f, 9f), new Vector3(2.2f, 1.7f, 5.4f), new Color(0.32f, 0.46f, 0.68f));
            CreateEnvironmentBlock("Container_B", new Vector3(-2f, 0.85f, -6f), new Vector3(2.2f, 1.7f, 5.4f), new Color(0.58f, 0.34f, 0.22f));
            CreateEnvironmentBlock("WatchTower_A", new Vector3(-20f, 2.4f, 14f), new Vector3(2f, 4.8f, 2f), new Color(0.38f, 0.34f, 0.28f));
            CreateEnvironmentBlock("WatchTower_B", new Vector3(20f, 2.4f, -14f), new Vector3(2f, 4.8f, 2f), new Color(0.38f, 0.34f, 0.28f));
            CreateTerrainZone("Terrain_Forest", new Vector3(-6f, 0.02f, 7f), new Vector3(6.6f, 0.02f, 6.6f), new Color(0.18f, 0.4f, 0.2f));
            CreateTerrainZone("Terrain_Rough", new Vector3(7f, 0.02f, -8f), new Vector3(5.8f, 0.02f, 5.8f), new Color(0.52f, 0.42f, 0.28f));
            CreateTerrainZone("Terrain_Road", new Vector3(3f, 0.02f, 9f), new Vector3(5f, 0.02f, 5f), new Color(0.42f, 0.42f, 0.42f));
            CreateTerrainZone("Terrain_Water", new Vector3(-2f, 0.02f, -6f), new Vector3(4.4f, 0.02f, 4.4f), new Color(0.2f, 0.38f, 0.62f));
        }

        private static void CreateEnvironmentBlock(string name, Vector3 position, Vector3 scale, Color color)
        {
            if (GameObject.Find(name) != null)
            {
                return;
            }

            GameObject block = GameObject.CreatePrimitive(PrimitiveType.Cube);
            block.name = name;
            block.transform.position = position;
            block.transform.localScale = scale;
            Renderer renderer = block.GetComponent<Renderer>();
            renderer.material.color = color;
            block.AddComponent<ObstacleVolume>();
        }

        private static void CreateTerrainZone(string name, Vector3 position, Vector3 scale, Color color)
        {
            if (GameObject.Find(name) != null)
            {
                return;
            }

            GameObject zone = GameObject.CreatePrimitive(PrimitiveType.Cube);
            zone.name = name;
            zone.transform.position = position;
            zone.GetComponent<Collider>().enabled = false;
            TerrainZoneView terrainZoneView = zone.AddComponent<TerrainZoneView>();
            terrainZoneView.Initialize(color, scale);
        }

        private static Camera EnsureCamera()
        {
            Camera mainCamera = Camera.main;
            if (mainCamera != null)
            {
                if (mainCamera.GetComponent<RtsCameraController>() == null)
                {
                    mainCamera.gameObject.AddComponent<RtsCameraController>();
                }

                return mainCamera;
            }

            GameObject cameraObject = new GameObject("Main Camera");
            cameraObject.tag = "MainCamera";
            cameraObject.transform.position = new Vector3(0f, 18f, -12f);
            cameraObject.transform.rotation = Quaternion.Euler(45f, 0f, 0f);
            mainCamera = cameraObject.AddComponent<Camera>();
            cameraObject.AddComponent<AudioListener>();
            cameraObject.AddComponent<RtsCameraController>();
            return mainCamera;
        }

        private static DebriefScreen EnsureDebriefScreen()
        {
            DebriefScreen existing = Object.FindFirstObjectByType<DebriefScreen>();
            if (existing != null)
            {
                return existing;
            }

            return new GameObject("DebriefScreen").AddComponent<DebriefScreen>();
        }

        private static void ApplySettings(SettingsData settings)
        {
            AudioListener.volume = settings.MasterVolume;
            QualitySettings.SetQualityLevel(Mathf.Clamp(settings.GraphicsQuality, 0, QualitySettings.names.Length - 1), true);
        }

    }
}
