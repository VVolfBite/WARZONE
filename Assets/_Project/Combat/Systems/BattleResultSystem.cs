using System.Collections.Generic;
using Warzone.Content;
using Warzone.Content.Definitions;

namespace Warzone.Combat
{
    public sealed class BattleResultSystem
    {
        private readonly ContentCatalog _contentCatalog;
        private readonly MissionObjectiveSystem _missionObjectiveSystem;

        public BattleResultSystem(ContentCatalog contentCatalog)
        {
            _contentCatalog = contentCatalog;
            _missionObjectiveSystem = new MissionObjectiveSystem(contentCatalog);
        }

        public BattleMissionStatusSnapshot UpdateMissionStatus(BattleState battleState)
        {
            BattleMissionStatusSnapshot missionStatus = _missionObjectiveSystem.Evaluate(battleState);
            battleState.SetMissionStatus(missionStatus);
            return missionStatus;
        }

        public BattleResult UpdateBattleResult(BattleState battleState)
        {
            BattleMissionStatusSnapshot missionStatus = battleState.CurrentMissionStatus ?? UpdateMissionStatus(battleState);
            if (!missionStatus.IsBattleComplete)
            {
                battleState.SetBattleResult(null);
                return null;
            }

            BattleResult existingResult = battleState.CurrentBattleResult;
            if (existingResult != null)
            {
                return existingResult;
            }

            MissionDefinition missionDefinition;
            IReadOnlyList<MissionObjectiveDefinition> objectives = null;
            if (!string.IsNullOrEmpty(battleState.MissionDefinitionId) &&
                _contentCatalog.TryGetMission(battleState.MissionDefinitionId, out missionDefinition))
            {
                objectives = missionDefinition.Objectives;
            }

            List<UnitOutcome> unitOutcomes = new List<UnitOutcome>();
            List<BattleEntityId> casualties = new List<BattleEntityId>();
            List<BattleEntityId> deadEnemies = new List<BattleEntityId>();
            List<BattleEntityId> extractedMembers = new List<BattleEntityId>();
            List<BattleEntityId> woundedMembers = new List<BattleEntityId>();
            int aliveMembers = 0;
            int aliveEnemies = 0;

            foreach (BattleMemberState memberState in battleState.MembersById.Values)
            {
                bool survived = memberState.IsAlive;
                unitOutcomes.Add(new UnitOutcome(memberState.MemberId, memberState.DefinitionId, survived));
                if (survived)
                {
                    aliveMembers++;
                    if (memberState.Health < memberState.MaxHealth)
                    {
                        woundedMembers.Add(memberState.MemberId);
                    }
                }
                else
                {
                    casualties.Add(memberState.MemberId);
                }

                if (memberState.IsExtracted)
                {
                    extractedMembers.Add(memberState.MemberId);
                }
            }

            foreach (BattleEnemyState enemyState in battleState.EnemiesById.Values)
            {
                if (enemyState.IsAlive)
                {
                    aliveEnemies++;
                }
                else
                {
                    deadEnemies.Add(enemyState.EnemyId);
                }
            }

            MissionOutcome missionOutcome = missionStatus.ResultType == BattleCompletionType.Success
                ? MissionOutcome.Victory
                : MissionOutcome.Defeat;

            BattleResult battleResult = new BattleResult(
                missionOutcome,
                unitOutcomes,
                casualties,
                new BattleStatistics(aliveMembers, aliveEnemies),
                battleState.MissionRuntimeState.CompletionTimeSeconds > 0f ? battleState.MissionRuntimeState.CompletionTimeSeconds : battleState.ElapsedTimeSeconds,
                0,
                missionStatus.ResultType,
                _missionObjectiveSystem.BuildObjectiveResults(battleState, objectives),
                new BattleCasualtyResult(casualties, deadEnemies),
                new BattleLootResult(battleState.MissionRuntimeState.LootDiscoveredCount),
                new BattleExtractionResult(extractedMembers, aliveMembers),
                new BattleWoundResult(woundedMembers));

            battleState.SetBattleResult(battleResult);
            battleState.AddEvent(new BattleEventRecord(BattleEventTypes.BattleCompleted, message: battleResult.CompletionType.ToString()));
            return battleResult;
        }
    }
}
