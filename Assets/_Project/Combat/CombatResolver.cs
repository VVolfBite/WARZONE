using System.Collections.Generic;
using System.Numerics;
using Warzone.Content;
using Warzone.Content.Definitions;

namespace Warzone.Combat
{
    public sealed class CombatResolver
    {
        private readonly ContentCatalog _contentCatalog;

        public CombatResolver(ContentCatalog contentCatalog)
        {
            _contentCatalog = contentCatalog;
        }

        public IReadOnlyList<DamageEvent> ResolveAttack(BattleSquadState attacker, BattleSquadState defender)
        {
            if (!attacker.HasLivingUnits || !defender.HasLivingUnits)
            {
                return new List<DamageEvent>();
            }

            List<DamageEvent> damageEvents = new List<DamageEvent>();
            BattleUnitState targetUnit = GetFirstAliveUnit(defender);
            if (targetUnit == null)
            {
                return damageEvents;
            }

            foreach (BattleUnitState attackingUnit in attacker.Units)
            {
                if (!attackingUnit.IsAlive || !targetUnit.IsAlive)
                {
                    continue;
                }

                UnitDefinition definition = _contentCatalog.Units[attackingUnit.DefinitionId];
                WeaponDefinition weapon = definition.Weapon;
                int damage = weapon.DamagePerHit;
                targetUnit.ApplyDamage(damage);
                damageEvents.Add(new DamageEvent(
                    attackingUnit.EntityId,
                    targetUnit.EntityId,
                    damage,
                    weapon.Id,
                    weapon.Range,
                    weapon.ProjectileSpeed,
                    !targetUnit.IsAlive));

                if (!targetUnit.IsAlive)
                {
                    targetUnit = GetFirstAliveUnit(defender);
                    if (targetUnit == null)
                    {
                        break;
                    }
                }
            }

            return damageEvents;
        }

        public float GetAttackRange(BattleSquadState squad)
        {
            BattleUnitState firstAliveUnit = GetFirstAliveUnit(squad);
            if (firstAliveUnit == null)
            {
                return 0f;
            }

            return _contentCatalog.Units[firstAliveUnit.DefinitionId].Weapon.Range;
        }

        public float GetAttackInterval(BattleSquadState squad)
        {
            BattleUnitState firstAliveUnit = GetFirstAliveUnit(squad);
            if (firstAliveUnit == null)
            {
                return 1f;
            }

            return _contentCatalog.Units[firstAliveUnit.DefinitionId].Weapon.AttackIntervalSeconds;
        }

        public float GetMoveSpeed(BattleSquadState squad)
        {
            BattleUnitState firstAliveUnit = GetFirstAliveUnit(squad);
            if (firstAliveUnit == null)
            {
                return 0f;
            }

            return _contentCatalog.Units[firstAliveUnit.DefinitionId].MoveSpeed;
        }

        public float GetAggroRange(BattleSquadState squad)
        {
            BattleUnitState firstAliveUnit = GetFirstAliveUnit(squad);
            if (firstAliveUnit == null)
            {
                return 0f;
            }

            return _contentCatalog.Units[firstAliveUnit.DefinitionId].AggroRange;
        }

        public float GetCollisionRadius(BattleSquadState squad)
        {
            BattleUnitState firstAliveUnit = GetFirstAliveUnit(squad);
            if (firstAliveUnit == null)
            {
                return squad.Units.Count > 0 ? _contentCatalog.Units[squad.Units[0].DefinitionId].CollisionRadius : 0.5f;
            }

            return _contentCatalog.Units[firstAliveUnit.DefinitionId].CollisionRadius;
        }

        public UnitDefinition GetPrimaryDefinition(BattleSquadState squad)
        {
            BattleUnitState firstAliveUnit = GetFirstAliveUnit(squad);
            if (firstAliveUnit == null)
            {
                return squad.Units.Count > 0 ? _contentCatalog.Units[squad.Units[0].DefinitionId] : null;
            }

            return _contentCatalog.Units[firstAliveUnit.DefinitionId];
        }

        public static float GetDistance(BattleSquadState squadA, BattleSquadState squadB)
        {
            return Vector2.Distance(squadA.Position, squadB.Position);
        }

        private static BattleUnitState GetFirstAliveUnit(BattleSquadState squad)
        {
            for (int i = 0; i < squad.Units.Count; i++)
            {
                BattleUnitState unit = squad.Units[i];
                if (unit.IsAlive)
                {
                    return unit;
                }
            }

            return null;
        }
    }
}
