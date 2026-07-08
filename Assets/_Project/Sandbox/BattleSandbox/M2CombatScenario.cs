using Warzone.Combat;
using Warzone.Content;

namespace Warzone.Sandbox.BattleSandbox
{
    public sealed class M2CombatScenario
    {
        public M2CombatScenario(ContentCatalog contentCatalog, BattleState battleState)
        {
            ContentCatalog = contentCatalog;
            BattleState = battleState;
        }

        public ContentCatalog ContentCatalog { get; private set; }
        public BattleState BattleState { get; private set; }
    }
}
