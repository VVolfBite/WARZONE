using UnityEngine;
using Warzone.Application.Services;
using Warzone.Combat;

namespace Warzone.Sandbox.BattleSandbox
{
    public sealed class M6PressureRetreatSandboxBootstrap : MonoBehaviour
    {
        private readonly PressureSystem _pressureSystem = new PressureSystem();
        private readonly RetreatSystem _retreatSystem = new RetreatSystem();

        private BattleSandboxRuntimeContext _context;
        private BattleSandboxViewPresenter _viewPresenter;
        private BattleSandboxInputController _inputController;
        private M6PressureRetreatDebugPanel _debugPanel;
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

            M6PressureRetreatScenario scenario = M6PressureRetreatScenarioFactory.CreateScenario();
            BattleService battleService = new BattleService(scenario.ContentCatalog);
            TacticalCommandService tacticalCommandService = new TacticalCommandService(battleService);
            battleService.Start(scenario.BattleState);

            if (_context == null)
            {
                _context = new BattleSandboxRuntimeContext();
            }

            _context.Bind(
                BattleSandboxMode.M6PressureRetreat,
                BattleSandboxScenarioRegistry.GetDisplayName(BattleSandboxMode.M6PressureRetreat),
                battleService,
                tacticalCommandService,
                BattleSandboxScenarioRegistry.GetDefaultSelectedSquadId(BattleSandboxMode.M6PressureRetreat));

            _viewPresenter.RebuildFromSnapshot(_context.GetSnapshot());
            _inputController.Initialize(_context, _mainCamera, RebuildScenario, ApplyDebugPressure, ClearDebugPressure, EmitDebugIncomingFire);
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

        private void ApplyDebugPressure()
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

            for (int i = 0; i < squadState.MemberIds.Count; i++)
            {
                BattleMemberState memberState;
                if (!battleState.TryGetMember(squadState.MemberIds[i], out memberState) || !memberState.CanAct)
                {
                    continue;
                }

                memberState.AddPressure(35f);
                memberState.SetRecentIncomingFire(PressureGainRule.RecentIncomingFireDurationSeconds);
                battleState.AddEvent(new BattleEventRecord(BattleEventTypes.PressureChanged, squadState.SquadId, memberState.MemberId, "debug", null, (int)(memberState.Pressure * 10f)));
                PressureSystem.UpdateStatusFlags(battleState, memberState);
            }

            _retreatSystem.Execute(battleState);
        }

        private void ClearDebugPressure()
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

            for (int i = 0; i < squadState.MemberIds.Count; i++)
            {
                BattleMemberState memberState;
                if (!battleState.TryGetMember(squadState.MemberIds[i], out memberState))
                {
                    continue;
                }

                memberState.SetPressure(0f);
                memberState.SetSuppression(0f);
                memberState.SetSuppressed(false);
                memberState.ClearRetreat();
                memberState.SetRecentIncomingFire(0f);
                battleState.AddEvent(new BattleEventRecord(BattleEventTypes.PressureChanged, squadState.SquadId, memberState.MemberId, "clear", null, 0));
            }
        }

        private void EmitDebugIncomingFire()
        {
            BattleState battleState = _context != null && _context.BattleService != null ? _context.BattleService.ActiveBattleState : null;
            if (battleState == null)
            {
                return;
            }

            BattleEnemyState sourceEnemy = null;
            foreach (BattleEnemyState enemyState in battleState.EnemiesById.Values)
            {
                if (enemyState != null && enemyState.IsAlive)
                {
                    sourceEnemy = enemyState;
                    break;
                }
            }

            if (sourceEnemy == null)
            {
                return;
            }

            BattleSquadState squadState;
            if (!battleState.TryGetSquad(_context.SelectedSquadId, out squadState))
            {
                return;
            }

            for (int i = 0; i < squadState.MemberIds.Count; i++)
            {
                BattleMemberState memberState;
                if (!battleState.TryGetMember(squadState.MemberIds[i], out memberState) || !memberState.CanAct)
                {
                    continue;
                }

                battleState.AddEvent(new BattleEventRecord(BattleEventTypes.WeaponFired, squadState.SquadId, sourceEnemy.EnemyId, "debug.incoming", memberState.MemberId, 0));
            }

            _pressureSystem.Execute(battleState, 0f);
            _retreatSystem.Execute(battleState);
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
                _debugPanel = GetComponent<M6PressureRetreatDebugPanel>();
                if (_debugPanel == null)
                {
                    _debugPanel = gameObject.AddComponent<M6PressureRetreatDebugPanel>();
                }
            }
        }

        private void EnsureGround()
        {
            if (GameObject.Find("M6SandboxGround") != null)
            {
                return;
            }

            GameObject ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
            ground.name = "M6SandboxGround";
            ground.transform.position = Vector3.zero;
            ground.transform.localScale = new Vector3(8f, 1f, 8f);
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
