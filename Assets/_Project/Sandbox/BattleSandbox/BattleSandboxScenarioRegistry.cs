namespace Warzone.Sandbox.BattleSandbox
{
    public static class BattleSandboxScenarioRegistry
    {
        public static string GetDisplayName(BattleSandboxMode mode)
        {
            switch (mode)
            {
                case BattleSandboxMode.M1MemberMovement:
                    return "M1 Member Movement";
                case BattleSandboxMode.M2CombatSlice:
                    return "M2 Combat Slice";
                case BattleSandboxMode.M3TacticalMission:
                    return "M3 Tactical Mission";
                case BattleSandboxMode.M4SpatialCombat:
                    return "M4 Spatial Combat";
                case BattleSandboxMode.M5IntegratedSandbox:
                    return "M5 Integrated Sandbox";
                case BattleSandboxMode.M6PressureRetreat:
                    return "M6 Pressure Retreat";
                default:
                    return mode.ToString();
            }
        }

        public static bool IsCompatibilityEntry(BattleSandboxMode mode)
        {
            return mode != BattleSandboxMode.M5IntegratedSandbox &&
                   mode != BattleSandboxMode.M6PressureRetreat;
        }

        public static int GetDefaultSelectedSquadId(BattleSandboxMode mode)
        {
            return 1;
        }
    }
}
