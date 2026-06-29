using Warzone.Content.Definitions;

namespace Warzone.Combat
{
    public sealed class BattleUnitState
    {
        public BattleUnitState(
            BattleEntityId entityId,
            string definitionId,
            FactionId factionId,
            int currentHealth)
        {
            EntityId = entityId;
            DefinitionId = definitionId;
            FactionId = factionId;
            CurrentHealth = currentHealth;
        }

        public BattleEntityId EntityId { get; }
        public string DefinitionId { get; }
        public FactionId FactionId { get; }
        public int CurrentHealth { get; private set; }
        public bool IsAlive => CurrentHealth > 0;

        public void ApplyDamage(int damage)
        {
            if (damage <= 0 || !IsAlive)
            {
                return;
            }

            CurrentHealth -= damage;
            if (CurrentHealth < 0)
            {
                CurrentHealth = 0;
            }
        }
    }
}
