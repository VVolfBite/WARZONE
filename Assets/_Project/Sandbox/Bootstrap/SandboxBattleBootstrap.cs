using System.Collections.Generic;
using UnityEngine;
using Warzone.Application;
using Warzone.Combat;
using Warzone.Content;

namespace Warzone.Sandbox.Bootstrap
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
        private SandboxCommandDispatcher _commandDispatcher;
        private readonly SandboxCameraFocusController _cameraFocusController = new SandboxCameraFocusController();
        private static readonly float[] TimeScales = { 0.5f, 0.75f, 1f, 1.5f, 2f, 3f };
        private bool _isPaused;
        private bool _hasPublishedBattleResult;
        private ObstacleVolume[] _obstacleVolumes = new ObstacleVolume[0];
        private Camera _mainCamera;
        private int _timeScaleIndex = 2;
        private int _formationIndex;

        public void Configure(BattleRuntimeHost battleRuntimeHost, Camera mainCamera)
        {
            _battleRuntimeHost = battleRuntimeHost;
            _mainCamera = mainCamera;
            _battleRuntimeHost.BattleStarted += HandleBattleStarted;

            SelectionBoxOverlay selectionBoxOverlay = gameObject.AddComponent<SelectionBoxOverlay>();
            _sandboxHudOverlay = gameObject.AddComponent<SandboxHudOverlay>();
            SandboxSelectionInfoOverlay selectionInfoOverlay = gameObject.AddComponent<SandboxSelectionInfoOverlay>();

            _selectionService = new SandboxSelectionService();
            _commandDispatcher = new SandboxCommandDispatcher();
            AudioService audioService = FindFirstObjectByType<AudioService>();
            _presentationSync = new SandboxPresentationSync(mainCamera, selectionInfoOverlay, audioService);
            _inputInterpreter = new SandboxInputInterpreter(mainCamera, selectionBoxOverlay, _selectionService, _commandDispatcher, _presentationSync);
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
                Time.timeScale = _isPaused ? 0f : TimeScales[_timeScaleIndex];
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

            if (_inputInterpreter != null && _inputInterpreter.TryConsumeFormation(out int formationIndex))
            {
                _formationIndex = formationIndex;
                _inputInterpreter.SetFormationIndex(_formationIndex);
                _notifications.Enqueue("Formation set: " + GetFormationLabel(_formationIndex));
            }

            if (_inputInterpreter != null && _inputInterpreter.ConsumeCameraFocus())
            {
                _cameraFocusController.FocusOnSelection(_mainCamera, _selectionService, _battleSession);
            }

            if (_inputInterpreter != null && _inputInterpreter.TryConsumeDoubleClickSelectionFocus(out int focusedSquadId))
            {
                FocusCameraOnSquad(focusedSquadId);
            }

            if (_inputInterpreter != null && _inputInterpreter.ConsumePrimaryAbility())
            {
                TriggerPrimaryAbility();
            }

            if (_inputInterpreter != null && _inputInterpreter.TryConsumeSpeedChange(out int speedDirection))
            {
                AdjustTimeScale(speedDirection);
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
            _hudPresenter.BindBattle(_isPaused, _battleSession, _waveController, _selectionService, Time.timeScale);
            if (_sandboxHudOverlay.TryConsumeMinimapJump(out Vector2 minimapJump))
            {
                JumpCameraFromMinimap(minimapJump);
            }

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
            _timeScaleIndex = 2;
            _formationIndex = 0;
            Time.timeScale = TimeScales[_timeScaleIndex];
            _inputInterpreter.SetFormationIndex(_formationIndex);
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
            Time.timeScale = TimeScales[_timeScaleIndex];
        }

        private void TriggerPrimaryAbility()
        {
            if (_battleSession == null || _selectionService == null || _selectionService.SelectedSquadIds.Count == 0)
            {
                return;
            }

            bool triggered = false;
            foreach (int squadId in _selectionService.SelectedSquadIds)
            {
                BattleSquadState squad = _battleSession.FindSquadById(squadId);
                if (squad == null)
                {
                    continue;
                }

                string abilityId = _battleSession.GetPrimaryDefinition(squad)?.ActiveAbilityId;
                if (string.IsNullOrEmpty(abilityId))
                {
                    continue;
                }

                _commandDispatcher.IssueUseAbility(_battleSession, new[] { squadId }, abilityId);
                triggered = true;
            }

            if (triggered)
            {
                _notifications.Enqueue("Ability used");
            }
        }

        private void RestartBattle()
        {
            Time.timeScale = 1f;
            UnityEngine.SceneManagement.SceneManager.LoadScene("SampleScene");
        }

        private void ReturnToMainMenu()
        {
            Time.timeScale = 1f;
            UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
        }

        private void OnEnable()
        {
            if (_sandboxHudOverlay != null)
            {
                _sandboxHudOverlay.SetPauseActions(ResumeBattle, RestartBattle, ReturnToMainMenu);
            }
        }

        private void AdjustTimeScale(int direction)
        {
            _timeScaleIndex = Mathf.Clamp(_timeScaleIndex + direction, 0, TimeScales.Length - 1);
            Time.timeScale = _isPaused ? 0f : TimeScales[_timeScaleIndex];
            _notifications.Enqueue("Speed set to x" + TimeScales[_timeScaleIndex].ToString("F1"));
        }

        private void FocusCameraOnSquad(int squadId)
        {
            if (_mainCamera == null || _battleSession == null)
            {
                return;
            }

            BattleSquadState squad = _battleSession.FindSquadById(squadId);
            if (squad == null)
            {
                return;
            }

            _mainCamera.transform.position = new Vector3(squad.Position.X, _mainCamera.transform.position.y, squad.Position.Y - 4f);
        }

        private static string GetFormationLabel(int formationIndex)
        {
            switch (formationIndex)
            {
                case 1: return "Line";
                case 2: return "Column";
                case 3: return "Circle";
                default: return "Grid";
            }
        }

        private void JumpCameraFromMinimap(Vector2 normalizedPosition)
        {
            if (_mainCamera == null)
            {
                return;
            }

            float x = Mathf.Lerp(-40f, 40f, normalizedPosition.x);
            float z = Mathf.Lerp(-40f, 40f, normalizedPosition.y);
            _mainCamera.transform.position = new Vector3(x, _mainCamera.transform.position.y, z - 4f);
            _notifications.Enqueue("Camera jumped");
        }
    }
}



