using System.Numerics;
using Warzone.Content.Definitions;

namespace Warzone.Combat
{
    public sealed class BattleEnemyState
    {
        public BattleEnemyState(BattleEntityId enemyId, FactionId factionId, Vector2 position, int health)
        {
            EnemyId = enemyId;
            FactionId = factionId;
            Position = position;
            Health = health;
        }

        public BattleEntityId EnemyId { get; private set; }
        public FactionId FactionId { get; private set; }
        public Vector2 Position { get; private set; }
        public int Health { get; private set; }

        public bool IsAlive
        {
            get { return Health > 0; }
        }
    }
}
