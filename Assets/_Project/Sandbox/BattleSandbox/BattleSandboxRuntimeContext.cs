using Warzone.Application.Services;
using Warzone.Combat;
using Warzone.Core.Math;
using System.Collections.Generic;

namespace Warzone.Sandbox.BattleSandbox
{
    public sealed class BattleSandboxRuntimeContext
    {
        private readonly List<SquadCommandDebugRecord> _recentCommandRecords = new List<SquadCommandDebugRecord>();

        public BattleSandboxMode Mode { get; private set; }
        public string ModeLabel { get; private set; }
        public BattleService BattleService { get; private set; }
        public TacticalCommandService TacticalCommandService { get; private set; }
        public int SelectedSquadId { get; private set; }
        public bool IsTogglePaused { get; private set; }
        public bool IsHoldPaused { get; private set; }
        public bool IsPaused
        {
            get { return IsTogglePaused || IsHoldPaused; }
        }

        public bool ShowFireLines { get; private set; }
        public bool ShowCommandPlan { get; private set; }
        public string LastInputAction { get; private set; }
        public string LastCommandIssued { get; private set; }
        public Vec2? LastCommandWorldPosition { get; private set; }
        public int LastCommandFrame { get; private set; }
        public float LastCommandTime { get; private set; }
        public string LastRaycastHitName { get; private set; }
        public bool LastRaycastUsedGroundFallback { get; private set; }

        public IReadOnlyList<SquadCommandDebugRecord> RecentCommandRecords
        {
            get { return _recentCommandRecords; }
        }

        public bool AllowCommandsWhilePaused
        {
            get { return true; }
        }

        public string ValidationHint
        {
            get { return "Pause stops Tick but still allows command queueing. Commands execute after resume."; }
        }

        public void Bind(
            BattleSandboxMode mode,
            string modeLabel,
            BattleService battleService,
            TacticalCommandService tacticalCommandService,
            int defaultSelectedSquadId)
        {
            Mode = mode;
            ModeLabel = modeLabel;
            BattleService = battleService;
            TacticalCommandService = tacticalCommandService;
            ResetFlags(defaultSelectedSquadId);
        }

        public void ResetFlags(int defaultSelectedSquadId)
        {
            SelectedSquadId = defaultSelectedSquadId;
            IsTogglePaused = false;
            IsHoldPaused = false;
            ShowFireLines = true;
            ShowCommandPlan = false;
            LastInputAction = "None";
            LastCommandIssued = "None";
            LastCommandWorldPosition = null;
            LastCommandFrame = 0;
            LastCommandTime = 0f;
            LastRaycastHitName = "-";
            LastRaycastUsedGroundFallback = false;
            _recentCommandRecords.Clear();
        }

        public BattleSnapshot GetSnapshot()
        {
            return BattleService != null ? BattleService.GetSnapshot() : null;
        }

        public void SetSelectedSquad(int squadId)
        {
            SelectedSquadId = squadId;
        }

        public void TogglePause()
        {
            IsTogglePaused = !IsTogglePaused;
        }

        public void SetHoldPause(bool isHeld)
        {
            IsHoldPaused = isHeld;
        }

        public void ClearPause()
        {
            IsTogglePaused = false;
            IsHoldPaused = false;
        }

        public void SetCommandPlanVisible(bool visible)
        {
            ShowCommandPlan = visible;
        }

        public void ToggleFireLines()
        {
            ShowFireLines = !ShowFireLines;
        }

        public void RecordInput(string inputAction, string commandIssued, Vec2? worldPosition, int frame, float time, string raycastHitName, bool usedGroundFallback)
        {
            LastInputAction = inputAction ?? "None";
            LastCommandIssued = commandIssued ?? "None";
            LastCommandWorldPosition = worldPosition;
            LastCommandFrame = frame;
            LastCommandTime = time;
            LastRaycastHitName = string.IsNullOrEmpty(raycastHitName) ? "-" : raycastHitName;
            LastRaycastUsedGroundFallback = usedGroundFallback;
        }

        public void RecordSquadCommand(string commandName, Vec2? desiredPosition, int frame, float time)
        {
            SquadCommandDebugRecord record = new SquadCommandDebugRecord(SelectedSquadId, commandName, desiredPosition, frame, time);
            _recentCommandRecords.Add(record);
            while (_recentCommandRecords.Count > 5)
            {
                _recentCommandRecords.RemoveAt(0);
            }
        }
    }

    public sealed class SquadCommandDebugRecord
    {
        public SquadCommandDebugRecord(int squadId, string commandName, Vec2? desiredPosition, int frame, float time)
        {
            SquadId = squadId;
            CommandName = commandName;
            DesiredPosition = desiredPosition;
            Frame = frame;
            Time = time;
        }

        public int SquadId { get; private set; }
        public string CommandName { get; private set; }
        public Vec2? DesiredPosition { get; private set; }
        public int Frame { get; private set; }
        public float Time { get; private set; }
    }
}
