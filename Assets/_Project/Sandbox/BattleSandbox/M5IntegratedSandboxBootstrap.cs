using UnityEngine;
using Warzone.Application.Services;

namespace Warzone.Sandbox.BattleSandbox
{
    public sealed class M5IntegratedSandboxBootstrap : MonoBehaviour
    {
        private BattleSandboxRuntimeContext _context;
        private BattleSandboxViewPresenter _viewPresenter;
        private BattleSandboxInputController _inputController;
        private M5IntegratedSandboxDebugPanel _debugPanel;
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

            M5IntegratedSandboxScenario scenario = M5IntegratedSandboxScenarioFactory.CreateScenario();
            BattleService battleService = new BattleService(scenario.ContentCatalog);
            TacticalCommandService tacticalCommandService = new TacticalCommandService(battleService);
            battleService.Start(scenario.BattleState);

            if (_context == null)
            {
                _context = new BattleSandboxRuntimeContext();
            }

            _context.Bind(
                BattleSandboxMode.M5IntegratedSandbox,
                BattleSandboxScenarioRegistry.GetDisplayName(BattleSandboxMode.M5IntegratedSandbox),
                battleService,
                tacticalCommandService,
                BattleSandboxScenarioRegistry.GetDefaultSelectedSquadId(BattleSandboxMode.M5IntegratedSandbox));

            _viewPresenter.RebuildFromSnapshot(_context.GetSnapshot());
            _inputController.Initialize(_context, _mainCamera, RebuildScenario);
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

        private void EnsureComponents()
        {
            if (_viewPresenter == null)
            {
                _viewPresenter = GetComponent<BattleSandboxViewPresenter>();
                if (_viewPresenter == null)
                {
                    _viewPresenter = gameObject.AddComponent<BattleSandboxViewPresenter>();
                }
            }

            if (_inputController == null)
            {
                _inputController = GetComponent<BattleSandboxInputController>();
                if (_inputController == null)
                {
                    _inputController = gameObject.AddComponent<BattleSandboxInputController>();
                }
            }

            if (_debugPanel == null)
            {
                _debugPanel = GetComponent<M5IntegratedSandboxDebugPanel>();
                if (_debugPanel == null)
                {
                    _debugPanel = gameObject.AddComponent<M5IntegratedSandboxDebugPanel>();
                }
            }
        }

        private void EnsureGround()
        {
            if (GameObject.Find("M5SandboxGround") != null)
            {
                return;
            }

            GameObject ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
            ground.name = "M5SandboxGround";
            ground.transform.position = Vector3.zero;
            ground.transform.localScale = new Vector3(7f, 1f, 7f);
            Renderer renderer = ground.GetComponent<Renderer>();
            renderer.material.color = new Color(0.28f, 0.36f, 0.24f);
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
            light.intensity = 1f;
            lightObject.transform.rotation = Quaternion.Euler(50f, -30f, 0f);
        }
    }
}
