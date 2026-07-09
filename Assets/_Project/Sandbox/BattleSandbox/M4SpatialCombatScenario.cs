using Warzone.Combat;
using Warzone.Content;

namespace Warzone.Sandbox.BattleSandbox
{
    public sealed class M4SpatialCombatScenario
    {
        public M4SpatialCombatScenario(ContentCatalog contentCatalog, BattleState battleState)
        {
            ContentCatalog = contentCatalog;
            BattleState = battleState;
        }

        public ContentCatalog ContentCatalog { get; private set; }
        public BattleState BattleState { get; private set; }
    }
}
