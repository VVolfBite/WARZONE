using System.Collections.Generic;
using UnityEngine;
using Warzone.Application;
using Warzone.Combat;
using Warzone.Content;

namespace Warzone.Adapters
{
    public sealed class SandboxBattleBootstrap : MonoBehaviour
    {
        private readonly Queue<string> _notifications = new Queue<string>();

        private BattleRuntimeHost _battleRuntimeHost;
        private SandboxHudOverlay _sandboxHudOverlay;
        private BattleSession _battleSession;
        private ContentCatalog _contentCatalog;
        private SandboxInputInterpreter _inputInterpreter;
        private SandboxSelectionService _selectionService;
        private SandboxPresentationSync _presentationSync;
        private SandboxWaveController _waveController;
        private SandboxHudPresenter _hudPresenter;
        private readonly SandboxCameraFocusController _cameraFocusController = new SandboxCameraFocusController();
        private bool _isPaused;
        private bool _hasPublishedBattleResult;
        private ObstacleVolume[] _obstacleVolumes = new ObstacleVolume[0];
        private Camera _mainCamera;

        public void Configure(BattleRuntimeHost battleRuntimeHost, Camera mainCamera)
        {
            _battleRuntimeHost = battleRuntimeHost;
            _mainCamera = mainCamera;
            _battleRuntimeHost.BattleStarted += HandleBattleStarted;

            SelectionBoxOverlay selectionBoxOverlay = gameObject.AddComponent<SelectionBoxOverlay>();
            _sandboxHudOverlay = gameObject.AddComponent<SandboxHudOverlay>();
            SandboxSelectionInfoOverlay selectionInfoOverlay = gameObject.AddComponent<SandboxSelectionInfoOverlay>();

            _selectionService = new SandboxSelectionService();
            SandboxCommandDispatcher commandDispatcher = new SandboxCommandDispatcher();
            _presentationSync = new SandboxPresentationSync(mainCamera, selectionInfoOverlay);
            _inputInterpreter = new SandboxInputInterpreter(mainCamera, selectionBoxOverlay, _selectionService, commandDispatcher, _presentationSync);
            _waveController = new SandboxWaveController(_notifications);
            _hudPresenter = new SandboxHudPresenter(_notifications, _sandboxHudOverlay);
            _sandboxHudOverlay.SetPauseActions(ResumeBattle, RestartBattle, ReturnToMainMenu);
        }

        private void OnDestroy()
        {
            if (_battleRuntimeHost != null)
            {
                _battleRuntimeHost.BattleStarted -= HandleBattleStarted;
            }
        }

        private void Update()
        {
            if (_inputInterpreter != null && _inputInterpreter.ConsumePauseToggle())
            {
                _isPaused = !_isPaused;
            }

            if (_inputInterpreter != null && _inputInterpreter.TryConsumeTeamCommand(out int slotIndex, out bool bindTeam))
            {
                if (bindTeam)
                {
                    _selectionService.BindTeam(slotIndex);
                }
                else if (!_selectionService.TrySelectTeam(slotIndex))
                {
                    _selectionService.Clear();
                }
            }

            if (_inputInterpreter != null && _inputInterpreter.ConsumeCameraFocus())
            {
                _cameraFocusController.FocusOnSelection(_mainCamera, _selectionService, _battleSession);
            }

            if (_battleSession != null)
            {
                _inputInterpreter.Tick(_battleSession);
            }

            if (_battleSession == null)
            {
                _hudPresenter.BindWaiting(_isPaused);
                return;
            }

            if (!_isPaused && _battleSession.CurrentOutcome == MissionOutcome.InProgress)
            {
                _battleSession.Tick(Time.deltaTime);
                IReadOnlyList<BattleSquadState> spawned = _waveController.Tick(_battleSession, Time.deltaTime);
                if (spawned != null)
                {
                    _presentationSync.AddViewsForSquads(spawned, _contentCatalog);
                }
            }

            _presentationSync.RenderDamageEvents(_battleSession);
            _presentationSync.Sync(_battleSession, _contentCatalog, _selectionService, _obstacleVolumes);
            _hudPresenter.BindBattle(_isPaused, _battleSession, _waveController, _selectionService);

            if (!_hasPublishedBattleResult && _battleSession.TryBuildResultOnce(out BattleResult result))
            {
                _hasPublishedBattleResult = true;
                _battleRuntimeHost.FinishBattle(result);
            }
        }

        private void HandleBattleStarted(MissionStartRequest request)
        {
            _contentCatalog = SandboxBattleFactory.BuildSandboxContent();
            _battleSession = SandboxBattleFactory.CreateSandboxBattle(_contentCatalog, request.Seed);
            _waveController.Reset(SandboxBattleFactory.BuildWaves(), _battleSession);

            _hasPublishedBattleResult = false;
            _isPaused = false;
            _selectionService.Clear();
            _notifications.Clear();
            _notifications.Enqueue("Demo started");
            _obstacleVolumes = FindObjectsByType<ObstacleVolume>(FindObjectsSortMode.None);
            _presentationSync.RebuildViews(_battleSession, _contentCatalog);

            IReadOnlyList<BattleSquadState> initialWave = _waveController.SpawnInitialWave(_battleSession);
            if (initialWave != null)
            {
                _presentationSync.AddViewsForSquads(initialWave, _contentCatalog);
            }
        }

        private void ResumeBattle()
        {
            _isPaused = false;
        }

        private void RestartBattle()
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("SampleScene");
        }

        private void ReturnToMainMenu()
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
        }

        private void OnEnable()
        {
            if (_sandboxHudOverlay != null)
            {
                _sandboxHudOverlay.SetPauseActions(ResumeBattle, RestartBattle, ReturnToMainMenu);
            }
        }
    }
}
