using UnityEngine;
using Warzone.Combat;

namespace Warzone.Adapters
{
    public sealed class SandboxCameraFocusController
    {
        public void FocusOnSelection(Camera mainCamera, SandboxSelectionService selectionService, BattleSession battleSession)
        {
            if (mainCamera == null || battleSession == null || selectionService == null || selectionService.SelectedSquadIds.Count == 0)
            {
                return;
            }

            Vector3 sum = Vector3.zero;
            int count = 0;
            foreach (int squadId in selectionService.SelectedSquadIds)
            {
                BattleSquadState squad = FindSquadById(battleSession, squadId);
                if (squad == null)
                {
                    continue;
                }

                sum += new Vector3(squad.Position.X, 0f, squad.Position.Y);
                count++;
            }

            if (count == 0)
            {
                return;
            }

            Vector3 focus = sum / count;
            mainCamera.transform.position = new Vector3(focus.x, mainCamera.transform.position.y, focus.z - 4f);
        }

        private static BattleSquadState FindSquadById(BattleSession battleSession, int squadId)
        {
            for (int i = 0; i < battleSession.Squads.Count; i++)
            {
                if (battleSession.Squads[i].SquadId == squadId)
                {
                    return battleSession.Squads[i];
                }
            }

            return null;
        }
    }
}
