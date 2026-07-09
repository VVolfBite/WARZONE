namespace Warzone.Sandbox.BattleSandbox
{
    public static class M5IntegratedSandboxScenarioFactory
    {
        public static M5IntegratedSandboxScenario CreateScenario()
        {
            // M5 keeps the M4 combat slice and moves the Unity entry path to a unified launcher/input/view stack.
            M4SpatialCombatScenario scenario = M4SpatialCombatScenarioFactory.CreateScenario();
            return new M5IntegratedSandboxScenario(scenario.ContentCatalog, scenario.BattleState);
        }
    }
}
