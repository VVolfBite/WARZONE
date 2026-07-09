using Warzone.Application.Services;
using Warzone.Combat;

namespace Warzone.Sandbox.BattleSandbox
{
    public sealed class BattleSandboxRuntimeContext
    {
        public BattleSandboxMode Mode { get; private set; }
        public string ModeLabel { get; private set; }
        public BattleService BattleService { get; private set; }
        public TacticalCommandService TacticalCommandService { get; private set; }
        public int SelectedSquadId { get; private set; }
        public bool IsPaused { get; private set; }
        public bool ShowFireLines { get; private set; }

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
            IsPaused = false;
            ShowFireLines = true;
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
            IsPaused = !IsPaused;
        }

        public void ToggleFireLines()
        {
            ShowFireLines = !ShowFireLines;
        }
    }
}
