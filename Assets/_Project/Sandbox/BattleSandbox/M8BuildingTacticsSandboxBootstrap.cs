using UnityEngine;
using Warzone.Application.Services;
using Warzone.Combat;
using Warzone.Core.Math;

namespace Warzone.Sandbox.BattleSandbox
{
    public sealed class M8BuildingTacticsSandboxBootstrap : MonoBehaviour
    {
        private BattleSandboxRuntimeContext _context;
        private BattleSandboxViewPresenter _viewPresenter;
        private BattleSandboxInputController _inputController;
        private M8BuildingTacticsDebugPanel _debugPanel;
        private Camera _mainCamera;

        private void Awake()
        {
            EnsureGround();
            EnsureCamera();
            EnsureLight();
            EnsureComponents();
            RebuildScenario();
        }

        private void Update()
        {
            if (_context == null)
            {
                return;
            }

            if (!_context.IsPaused && _context.BattleService != null && !_context.BattleService.IsBattleComplete())
            {
                _context.BattleService.Tick(Time.deltaTime);
            }

            PublishSnapshot();
        }

        public void RebuildScenario()
        {
            if (_viewPresenter != null)
            {
                _viewPresenter.ClearViews();
            }

            M8BuildingTacticsScenario scenario = M8BuildingTacticsScenarioFactory.CreateScenario();
            BattleService battleService = new BattleService(scenario.ContentCatalog);
            TacticalCommandService tacticalCommandService = new TacticalCommandService(battleService);
            battleService.Start(scenario.BattleState);

            if (_context == null)
            {
                _context = new BattleSandboxRuntimeContext();
            }

            _context.Bind(
                BattleSandboxMode.M8BuildingTactics,
                BattleSandboxScenarioRegistry.GetDisplayName(BattleSandboxMode.M8BuildingTactics),
                battleService,
                tacticalCommandService,
                BattleSandboxScenarioRegistry.GetDefaultSelectedSquadId(BattleSandboxMode.M8BuildingTactics));

            _viewPresenter.RebuildFromSnapshot(_context.GetSnapshot());
            _inputController.Initialize(
                _context,
                _mainCamera,
                RebuildScenario,
                null,
                null,
                null,
                ToggleNight,
                TogglePlayerNightVision,
                AddSmokeZone,
                AddFireZone,
                AddLightZone);
            PublishSnapshot();
        }

        private void PublishSnapshot()
        {
            if (_context == null)
            {
                return;
            }

            _viewPresenter.Refresh(_context.GetSnapshot(), _context.SelectedSquadId, _context.ShowFireLines);
            _debugPanel.Bind(_context.GetSnapshot(), _context);
        }

        private void ToggleNight()
        {
            BattleState battleState = _context != null && _context.BattleService != null ? _context.BattleService.ActiveBattleState : null;
            if (battleState == null)
            {
                return;
            }

            bool nextNight = !battleState.EnvironmentState.IsNight;
            battleState.EnvironmentState.SetNight(nextNight);
            battleState.EnvironmentState.SetGlobalVisibilityMultiplier(nextNight ? 0.55f : 1f);
            battleState.EnvironmentState.SetAmbientLightLevel(nextNight ? 0.35f : 1f);
        }

        private void TogglePlayerNightVision()
        {
            BattleState battleState = _context != null && _context.BattleService != null ? _context.BattleService.ActiveBattleState : null;
            if (battleState == null)
            {
                return;
            }

            BattleSquadState squadState;
            if (!battleState.TryGetSquad(_context.SelectedSquadId, out squadState))
            {
                return;
            }

            int targetLevel = 1;
            if (squadState.MemberIds.Count > 0 &&
                battleState.TryGetMember(squadState.MemberIds[0], out BattleMemberState firstMember) &&
                firstMember.NightVisionLevel > 0)
            {
                targetLevel = 0;
            }

            for (int i = 0; i < squadState.MemberIds.Count; i++)
            {
                if (battleState.TryGetMember(squadState.MemberIds[i], out BattleMemberState memberState))
                {
                    memberState.SetNightVisionLevel(targetLevel);
                }
            }
        }

        private void AddSmokeZone(Vec2 position)
        {
            AddZone(EnvironmentalZoneType.Smoke, position, 2.5f, 0.85f, 18f, 0.55f, 0f, 0f);
        }

        private void AddFireZone(Vec2 position)
        {
            AddZone(EnvironmentalZoneType.Fire, position, 1.6f, 1f, 20f, 0f, 8f, 5f);
        }

        private void AddLightZone(Vec2 position)
        {
            AddZone(EnvironmentalZoneType.Light, position, 2.4f, 1f, 24f, 0f, 0f, 0f);
        }

        private void AddZone(EnvironmentalZoneType zoneType, Vec2 position, float radius, float intensity, float duration, float visionPenalty, float damagePerSecond, float pressurePerSecond)
        {
            BattleState battleState = _context != null && _context.BattleService != null ? _context.BattleService.ActiveBattleState : null;
            if (battleState == null)
            {
                return;
            }

            BattleStateFactory factory = new BattleStateFactory();
            int zoneId = battleState.EnvironmentState.GetNextZoneId();
            battleState.EnvironmentState.AddZone(factory.CreateEnvironmentalZone(zoneId, zoneType, position, radius, intensity, duration, true, visionPenalty, damagePerSecond, pressurePerSecond));
        }

        private void EnsureComponents()
        {
            if (_viewPresenter == null)
            {
                _viewPresenter = GetComponent<BattleSandboxViewPresenter>() ?? gameObject.AddComponent<BattleSandboxViewPresenter>();
            }

            if (_inputController == null)
            {
                _inputController = GetComponent<BattleSandboxInputController>() ?? gameObject.AddComponent<BattleSandboxInputController>();
            }

            if (_debugPanel == null)
            {
                _debugPanel = GetComponent<M8BuildingTacticsDebugPanel>() ?? gameObject.AddComponent<M8BuildingTacticsDebugPanel>();
            }
        }

        private void EnsureGround()
        {
            if (GameObject.Find("M8SandboxGround") != null)
            {
                return;
            }

            GameObject ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
            ground.name = "M8SandboxGround";
            ground.transform.position = Vector3.zero;
            ground.transform.localScale = new Vector3(8f, 1f, 8f);
            Renderer renderer = ground.GetComponent<Renderer>();
            renderer.material.color = new Color(0.16f, 0.2f, 0.22f);
        }

        private void EnsureCamera()
        {
            _mainCamera = Camera.main;
            if (_mainCamera != null)
            {
                return;
            }

            GameObject cameraObject = new GameObject("Main Camera");
            cameraObject.tag = "MainCamera";
            cameraObject.transform.position = new Vector3(1f, 24f, -16f);
            cameraObject.transform.rotation = Quaternion.Euler(58f, 0f, 0f);
            _mainCamera = cameraObject.AddComponent<Camera>();
            cameraObject.AddComponent<AudioListener>();
        }

        private static void EnsureLight()
        {
            if (FindObjectOfType<Light>() != null)
            {
                return;
            }

            GameObject lightObject = new GameObject("Directional Light");
            Light light = lightObject.AddComponent<Light>();
            light.type = LightType.Directional;
            light.intensity = 0.7f;
            lightObject.transform.rotation = Quaternion.Euler(50f, -30f, 0f);
        }
    }
}
