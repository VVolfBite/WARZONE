using Warzone.Combat;
using Warzone.Content;
using Warzone.Content.Definitions;
using Warzone.Core.Math;

namespace Warzone.Sandbox.BattleSandbox
{
    public static class M7EnvironmentCombatScenarioFactory
    {
        public static M7EnvironmentCombatScenario CreateScenario()
        {
            ContentCatalog contentCatalog = SandboxCombatContentCatalogFactory.CreateEnvironmentCombatCatalog();
            M6PressureRetreatScenario baseScenario = M6PressureRetreatScenarioFactory.CreateScenario();
            BattleState battleState = baseScenario.BattleState;
            BattleStateFactory battleStateFactory = new BattleStateFactory();

            battleState.EnvironmentState.SetNight(true);
            battleState.EnvironmentState.SetGlobalVisibilityMultiplier(0.55f);
            battleState.EnvironmentState.SetAmbientLightLevel(0.35f);

            foreach (BattleMemberState memberState in battleState.MembersById.Values)
            {
                memberState.SetNightVisionLevel(1);
                memberState.SetSmokeVisionLevel(0);
                memberState.SetHasLightSource(false);
                memberState.SetEffectiveDetectionRange(memberState.DetectionRange);
            }

            bool assignedEnemyNightVision = false;
            foreach (BattleEnemyState enemyState in battleState.EnemiesById.Values)
            {
                enemyState.SetNightVisionLevel(assignedEnemyNightVision ? 0 : 1);
                enemyState.SetHasLightSource(false);
                enemyState.SetEffectiveDetectionRange(enemyState.DetectionRange);
                assignedEnemyNightVision = true;
            }

            battleState.EnvironmentState.AddZone(battleStateFactory.CreateEnvironmentalZone(1000, EnvironmentalZoneType.Smoke, new Vec2(-1.5f, -2f), 2.8f, 0.85f, 18f, true, 0.55f, 0f, 0f));
            battleState.EnvironmentState.AddZone(battleStateFactory.CreateEnvironmentalZone(1001, EnvironmentalZoneType.Smoke, new Vec2(6.2f, 1.8f), 2.2f, 0.75f, 18f, true, 0.45f, 0f, 0f));
            battleState.EnvironmentState.AddZone(battleStateFactory.CreateEnvironmentalZone(1002, EnvironmentalZoneType.Fire, new Vec2(3.4f, -0.8f), 1.5f, 1f, 24f, true, 0f, 8f, 5f));
            battleState.EnvironmentState.AddZone(battleStateFactory.CreateEnvironmentalZone(1003, EnvironmentalZoneType.Toxic, new Vec2(10.2f, 1.2f), 2f, 1f, 30f, true, 0.15f, 4f, 7f));
            battleState.EnvironmentState.AddZone(battleStateFactory.CreateEnvironmentalZone(1004, EnvironmentalZoneType.Light, new Vec2(11.1f, 1.2f), 2.4f, 1f, 999f, true, 0f, 0f, 0f));
            battleState.EnvironmentState.AddZone(battleStateFactory.CreateEnvironmentalZone(1005, EnvironmentalZoneType.Darkness, new Vec2(13.6f, 4.9f), 3f, 1f, 999f, true, 0.35f, 0f, 0f));

            BattleEnemyState nightEnemy;
            if (battleState.TryGetEnemy(new BattleEntityId(9405), out nightEnemy))
            {
                nightEnemy.SetNightVisionLevel(1);
            }

            return new M7EnvironmentCombatScenario(contentCatalog, battleState);
        }
    }
}
