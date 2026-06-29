using UnityEngine;
using Warzone.Application;
using Warzone.Content.Definitions;
using Warzone.Meta;

namespace Warzone.Adapters
{
    public sealed class SandboxMissionStarter : MonoBehaviour
    {
        private BattleRuntimeHost _battleRuntimeHost;

        private MissionFlow _missionFlow;

        public void Configure(BattleRuntimeHost battleRuntimeHost)
        {
            _battleRuntimeHost = battleRuntimeHost;
        }

        public void StartDemoMission()
        {
            GameFlow gameFlow = new GameFlow();
            ProgressionService progressionService = new ProgressionService();
            _missionFlow = new MissionFlow(gameFlow, _battleRuntimeHost, progressionService);

            MissionDefinition missionDefinition = new MissionDefinition("mission.sandbox", "Sandbox", 1, 1);
            RosterSnapshot rosterSnapshot = new RosterSnapshot(
                new OwnedUnitState[]
                {
                    new OwnedUnitState("owned.player.1", "unit.player.infantry", 0),
                    new OwnedUnitState("owned.player.2", "unit.player.infantry", 0),
                    new OwnedUnitState("owned.player.3", "unit.player.infantry", 0),
                    new OwnedUnitState("owned.player.4", "unit.player.infantry", 0)
                });

            _missionFlow.StartMission(new MissionStartRequest(missionDefinition, rosterSnapshot, seed: 1));
        }
    }
}
