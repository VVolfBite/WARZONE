using System.Collections.Generic;
using Warzone.Combat;

namespace Warzone.Adapters
{
    public sealed class SandboxHudPresenter
    {
        private readonly Queue<string> _notifications;
        private readonly SandboxHudOverlay _hudOverlay;

        public SandboxHudPresenter(Queue<string> notifications, SandboxHudOverlay hudOverlay)
        {
            _notifications = notifications;
            _hudOverlay = hudOverlay;
        }

        public void BindWaiting(bool isPaused)
        {
            _hudOverlay.Bind(isPaused, 0, 0, "Waiting for mission...", string.Empty, speedText: "Speed x1");
        }

        public void BindBattle(
            bool isPaused,
            BattleSession battleSession,
            SandboxWaveController waveController,
            SandboxSelectionService selectionService,
            float timeScale)
        {
            _hudOverlay.Bind(
                isPaused,
                waveController.ActiveWaveIndex,
                waveController.TotalWaveCount,
                GetObjectiveText(battleSession, waveController),
                BuildNotificationText(),
                BuildDebugText(battleSession, selectionService),
                BuildTeamText(selectionService),
                BuildSpeedText(timeScale));
        }

        private string GetObjectiveText(BattleSession battleSession, SandboxWaveController waveController)
        {
            if (battleSession == null)
            {
                return "Initialize";
            }

            if (battleSession.CurrentOutcome == MissionOutcome.Victory)
            {
                return "Objective complete";
            }

            if (battleSession.CurrentOutcome == MissionOutcome.Defeat)
            {
                return "Mission failed";
            }

            return "Hold the line. Waves cleared: " + waveController.ActiveWaveIndex + "/" + waveController.TotalWaveCount;
        }

        private string BuildNotificationText()
        {
            while (_notifications.Count > 4)
            {
                _notifications.Dequeue();
            }

            if (_notifications.Count == 0)
            {
                return "No alerts";
            }

            return string.Join("\n", _notifications.ToArray());
        }

        private static string BuildDebugText(BattleSession battleSession, SandboxSelectionService selectionService)
        {
            if (battleSession == null)
            {
                return string.Empty;
            }

            int selectedCount = selectionService != null ? selectionService.SelectedSquadIds.Count : 0;
            return "Selected squads: " + selectedCount + "\nTerrain: active";
        }

        private static string BuildTeamText(SandboxSelectionService selectionService)
        {
            if (selectionService == null || selectionService.SelectedSquadIds.Count == 0)
            {
                return "No team selected";
            }

            return "Selected: " + string.Join(",", selectionService.BuildOrderedSelection());
        }

        private static string BuildSpeedText(float timeScale)
        {
            return "Speed x" + timeScale.ToString("F1");
        }
    }
}
