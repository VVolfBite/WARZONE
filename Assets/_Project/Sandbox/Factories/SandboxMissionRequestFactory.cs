using Warzone.Application;
using Warzone.Content.Definitions;
using Warzone.Campaign;

namespace Warzone.Sandbox.Factories
{
    public static class SandboxMissionRequestFactory
    {
        public static MissionStartRequest CreateDemoMissionRequest()
        {
            MissionDefinition missionDefinition = new MissionDefinition("mission.sandbox", "Sandbox", 1, 1);
            RosterSnapshot rosterSnapshot = new RosterSnapshot(
                new[]
                {
                    new OwnedUnitState("owned.player.1", "unit.player.infantry", 0),
                    new OwnedUnitState("owned.player.2", "unit.player.infantry", 0),
                    new OwnedUnitState("owned.player.3", "unit.player.infantry", 0),
                    new OwnedUnitState("owned.player.4", "unit.player.infantry", 0)
                });

            return new MissionStartRequest(missionDefinition, rosterSnapshot, seed: 1);
        }
    }
}



