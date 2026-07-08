using System.Collections.Generic;
using UnityEngine;
using Warzone.Combat;

namespace Warzone.Sandbox.UI
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
            SandboxHudSnapshot snapshot = new SandboxHudSnapshot
            {
                IsPaused = isPaused,
                ObjectiveText = "Waiting for mission...",
                NotificationText = string.Empty,
                SpeedText = "Speed x1"
            };
            _hudOverlay.Bind(snapshot);
        }

        public void BindBattle(
            bool isPaused,
            BattleSession battleSession,
            SandboxWaveController waveController,
            SandboxSelectionService selectionService,
            float timeScale)
        {
            SandboxHudSnapshot snapshot = new SandboxHudSnapshot
            {
                IsPaused = isPaused,
                ActiveWaveIndex = waveController.ActiveWaveIndex,
                TotalWaveCount = waveController.TotalWaveCount,
                ObjectiveText = GetObjectiveText(battleSession, waveController),
                NotificationText = BuildNotificationText(),
                DebugText = BuildDebugText(battleSession, selectionService),
                TeamText = BuildTeamText(selectionService),
                SpeedText = BuildSpeedText(timeScale)
            };

            BuildMinimapDots(snapshot, battleSession, selectionService);
            BuildTeamSlots(snapshot, selectionService);
            _hudOverlay.Bind(snapshot);
        }

        private string GetObjectiveText(BattleSession battleSession, SandboxWaveController waveController)
        {
            if (battleSession == null)
            {
                return "Initialize";
            }

            if (battleSession.CurrentOutcome == MissionOutcome.Victory)
            {
                return "Operation: Industrial Cleanup\nObjective complete";
            }

            if (battleSession.CurrentOutcome == MissionOutcome.Defeat)
            {
                return "Operation: Industrial Cleanup\nMission failed";
            }

            return "Operation: Industrial Cleanup\nHold the line. Waves cleared: " + waveController.ActiveWaveIndex + "/" + waveController.TotalWaveCount;
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

        private static void BuildMinimapDots(SandboxHudSnapshot snapshot, BattleSession battleSession, SandboxSelectionService selectionService)
        {
            const float mapExtent = 40f;

            for (int i = 0; i < battleSession.Squads.Count; i++)
            {
                BattleSquadState squad = battleSession.Squads[i];
                if (!squad.HasLivingUnits)
                {
                    continue;
                }

                float normalizedX = Mathf.InverseLerp(-mapExtent, mapExtent, squad.Position.X);
                float normalizedY = Mathf.InverseLerp(-mapExtent, mapExtent, squad.Position.Y);
                snapshot.MinimapDots.Add(new SandboxMinimapDot
                {
                    NormalizedPosition = new Vector2(normalizedX, normalizedY),
                    Color = squad.FactionId == Content.Definitions.FactionId.Player ? new Color(0.2f, 0.85f, 1f) : new Color(1f, 0.3f, 0.25f),
                    IsSelected = selectionService != null && selectionService.Contains(squad.SquadId)
                });
            }
        }

        private static void BuildTeamSlots(SandboxHudSnapshot snapshot, SandboxSelectionService selectionService)
        {
            if (selectionService == null)
            {
                return;
            }

            for (int i = 0; i < 10; i++)
            {
                snapshot.TeamSlots.Add(new SandboxTeamSlotSnapshot
                {
                    SlotIndex = i,
                    BoundCount = selectionService.GetBoundCount(i),
                    IsActive = selectionService.IsTeamActive(i)
                });
            }
        }
    }
}



