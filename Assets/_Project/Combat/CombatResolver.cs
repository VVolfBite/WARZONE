using System;
using System.Collections.Generic;
using System.Numerics;
using Warzone.Content;
using Warzone.Content.Definitions;

namespace Warzone.Combat
{
    public sealed class CombatResolver
    {
        private readonly ContentCatalog _contentCatalog;
        private readonly TerrainMap _terrainMap;

        public CombatResolver(ContentCatalog contentCatalog, TerrainMap terrainMap = null)
        {
            _contentCatalog = contentCatalog;
            _terrainMap = terrainMap;
        }

        public ContentCatalog ContentCatalog => _contentCatalog;

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
                UnitDefinition defenderDefinition = _contentCatalog.Units[defender.Units[0].DefinitionId];
                int damage = ApplyDamageTable(weapon.DamagePerHit, weapon.DamageType, defenderDefinition.ArmorType);
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

            float range = _contentCatalog.Units[firstAliveUnit.DefinitionId].Weapon.Range;
            return range * firstAliveUnit.GetRangeMultiplier() * GetRangeMultiplier(squad.Position);
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

            float speed = _contentCatalog.Units[firstAliveUnit.DefinitionId].MoveSpeed;
            return speed * firstAliveUnit.GetMoveSpeedMultiplier() * GetMoveSpeedMultiplier(squad.Position);
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

        public float GetRangeMultiplier(Vector2 position)
        {
            return _terrainMap != null ? _terrainMap.GetRangeMultiplier(position) : 1f;
        }

        public float GetMoveSpeedMultiplier(Vector2 position)
        {
            return _terrainMap != null ? _terrainMap.GetMoveSpeedMultiplier(position) : 1f;
        }

        public bool BlocksLineOfSight(Vector2 from, Vector2 to)
        {
            return _terrainMap != null && _terrainMap.BlocksLineOfSight(from, to);
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

        private static int ApplyDamageTable(int baseDamage, DamageType damageType, ArmorType armorType)
        {
            float multiplier = 1f;
            if (damageType == DamageType.Piercing && armorType == ArmorType.Heavy)
            {
                multiplier = 1.25f;
            }
            else if (damageType == DamageType.Explosive && armorType == ArmorType.Light)
            {
                multiplier = 1.1f;
            }
            else if (damageType == DamageType.Fire && armorType == ArmorType.Fortified)
            {
                multiplier = 0.85f;
            }

            return Math.Max(1, (int)Math.Round(baseDamage * multiplier, MidpointRounding.AwayFromZero));
        }
    }
}
