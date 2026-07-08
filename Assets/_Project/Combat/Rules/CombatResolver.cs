using System;
using System.Collections.Generic;
using System.Numerics;
using Warzone.Content;
using Warzone.Content.Definitions;

namespace Warzone.Combat
{
    public sealed class CombatResolver
    {
        // TODO(legacy): This resolver preserves the current prototype combat loop.
        // Future member-level combat rules should land in dedicated systems/rules instead of expanding this class.
        private readonly ContentCatalog _contentCatalog;
        private readonly TerrainMap _terrainMap;

        public CombatResolver(ContentCatalog contentCatalog, TerrainMap terrainMap = null)
        {
            _contentCatalog = contentCatalog;
            _terrainMap = terrainMap;
        }

        public ContentCatalog ContentCatalog
        {
            get { return _contentCatalog; }
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

                UnitDefinition attackerDefinition = GetDefinition(attackingUnit.DefinitionId);
                if (attackerDefinition == null || attackerDefinition.Weapon == null)
                {
                    continue;
                }

                float distance = GetDistance(attacker, defender);
                float attackRange = attackerDefinition.Weapon.Range * attackingUnit.GetRangeMultiplier();
                if (distance > attackRange)
                {
                    continue;
                }

                float terrainMitigation = GetTerrainDefenseModifier(defender.Position);
                int damage = Math.Max(1, (int)Math.Round(attackerDefinition.Weapon.DamagePerHit * terrainMitigation));
                targetUnit.ApplyDamage(damage);
                damageEvents.Add(new DamageEvent(
                    attackingUnit.EntityId,
                    targetUnit.EntityId,
                    damage,
                    attackerDefinition.Weapon.Id,
                    attackerDefinition.Weapon.Range,
                    attackerDefinition.Weapon.ProjectileSpeed,
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

            UnitDefinition definition = GetDefinition(firstAliveUnit.DefinitionId);
            return definition != null && definition.Weapon != null
                ? definition.Weapon.Range * firstAliveUnit.GetRangeMultiplier()
                : 0f;
        }

        public float GetAttackInterval(BattleSquadState squad)
        {
            BattleUnitState firstAliveUnit = GetFirstAliveUnit(squad);
            if (firstAliveUnit == null)
            {
                return 1f;
            }

            UnitDefinition definition = GetDefinition(firstAliveUnit.DefinitionId);
            return definition != null && definition.Weapon != null
                ? definition.Weapon.AttackIntervalSeconds
                : 1f;
        }

        public float GetMoveSpeed(BattleSquadState squad)
        {
            BattleUnitState firstAliveUnit = GetFirstAliveUnit(squad);
            if (firstAliveUnit == null)
            {
                return 0f;
            }

            UnitDefinition definition = GetDefinition(firstAliveUnit.DefinitionId);
            return definition != null
                ? definition.MoveSpeed * firstAliveUnit.GetMoveSpeedMultiplier() * GetTerrainMoveModifier(squad.Position)
                : 0f;
        }

        public float GetAggroRange(BattleSquadState squad)
        {
            BattleUnitState firstAliveUnit = GetFirstAliveUnit(squad);
            if (firstAliveUnit == null)
            {
                return 0f;
            }

            UnitDefinition definition = GetDefinition(firstAliveUnit.DefinitionId);
            return definition != null ? definition.AggroRange : 0f;
        }

        public float GetCollisionRadius(BattleSquadState squad)
        {
            BattleUnitState firstAliveUnit = GetFirstAliveUnit(squad);
            if (firstAliveUnit == null)
            {
                return 0.5f;
            }

            UnitDefinition definition = GetDefinition(firstAliveUnit.DefinitionId);
            return definition != null ? definition.CollisionRadius : 0.5f;
        }

        public UnitDefinition GetPrimaryDefinition(BattleSquadState squad)
        {
            BattleUnitState firstAliveUnit = GetFirstAliveUnit(squad);
            return firstAliveUnit != null ? GetDefinition(firstAliveUnit.DefinitionId) : null;
        }

        public static float GetDistance(BattleSquadState squadA, BattleSquadState squadB)
        {
            return Vector2.Distance(squadA.Position, squadB.Position);
        }

        private UnitDefinition GetDefinition(string definitionId)
        {
            UnitDefinition definition;
            return _contentCatalog.Units.TryGetValue(definitionId, out definition) ? definition : null;
        }

        private float GetTerrainMoveModifier(Vector2 position)
        {
            if (_terrainMap == null)
            {
                return 1f;
            }

            return _terrainMap.GetMoveSpeedMultiplier(position);
        }

        private float GetTerrainDefenseModifier(Vector2 position)
        {
            if (_terrainMap == null)
            {
                return 1f;
            }

            switch (_terrainMap.GetTerrainAt(position))
            {
                case TerrainType.Forest:
                    return 0.85f;
                case TerrainType.Rough:
                    return 0.92f;
                default:
                    return 1f;
            }
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

