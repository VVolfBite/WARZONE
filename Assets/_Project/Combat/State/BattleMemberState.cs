using System.Numerics;
using Warzone.Content.Definitions;

namespace Warzone.Combat
{
    public sealed class BattleMemberState
    {
        public BattleMemberState(
            BattleEntityId memberId,
            int squadId,
            FactionId factionId,
            Vector2 position,
            int health,
            int maxHealth,
            float movementSpeed,
            string weaponId = null,
            string definitionId = null)
        {
            MemberId = memberId;
            SquadId = squadId;
            FactionId = factionId;
            Position = position;
            Facing = new Vector2(0f, 1f);
            Health = health;
            MaxHealth = maxHealth;
            MovementSpeed = movementSpeed;
            WeaponId = weaponId;
            DefinitionId = definitionId;
        }

        public BattleEntityId MemberId { get; private set; }
        public int SquadId { get; private set; }
        public FactionId FactionId { get; private set; }
        public Vector2 Position { get; private set; }
        public Vector2 Facing { get; private set; }
        public int Health { get; private set; }
        public int MaxHealth { get; private set; }
        public float MovementSpeed { get; private set; }
        public float Pressure { get; private set; }

        public bool IsAlive
        {
            get { return Health > 0; }
        }

        public string WeaponId { get; private set; }
        public string DefinitionId { get; private set; }
        public MemberIntent CurrentIntent { get; private set; }

        public void SetIntent(MemberIntent intent)
        {
            CurrentIntent = intent;
        }

        public void ClearIntent()
        {
            CurrentIntent = null;
        }

        public void UpdatePosition(Vector2 position)
        {
            Position = position;
        }

        public void UpdateFacing(Vector2 facing)
        {
            if (facing.LengthSquared() <= 0.0001f)
            {
                return;
            }

            Facing = Vector2.Normalize(facing);
        }

        public void ApplyDamage(int damage)
        {
            if (damage <= 0 || !IsAlive)
            {
                return;
            }

            Health -= damage;
            if (Health < 0)
            {
                Health = 0;
            }
        }

        public void SetPressure(float pressure)
        {
            Pressure = pressure < 0f ? 0f : pressure;
        }
    }
}
