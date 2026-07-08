using Warzone.Combat;
using Warzone.Campaign;

namespace Warzone.Application
{
    public sealed class MissionFlow : IMissionFlow
    {
        private readonly GameFlow _gameFlow;
        private readonly IBattleRuntimeHost _battleRuntimeHost;
        private readonly ProgressionService _progressionService;

        public MissionFlow(
            GameFlow gameFlow,
            IBattleRuntimeHost battleRuntimeHost,
            ProgressionService progressionService)
        {
            _gameFlow = gameFlow;
            _battleRuntimeHost = battleRuntimeHost;
            _progressionService = progressionService;
        }

        public void StartMission(MissionStartRequest request)
        {
            _gameFlow.EnterMission();
            _battleRuntimeHost.StartBattle(request);
        }

        public MetaSettlementResult CompleteMission(BattleResult battleResult)
        {
            _gameFlow.ExitMission();
            return _progressionService.ApplySettlement(BuildSettlement(battleResult));
        }

        private static CampaignSettlement BuildSettlement(BattleResult battleResult)
        {
            int keptUnits = 0;
            for (int i = 0; i < battleResult.UnitOutcomes.Count; i++)
            {
                if (battleResult.UnitOutcomes[i].Survived)
                {
                    keptUnits++;
                }
            }

            return new CampaignSettlement(
                missionCompleted: battleResult.MissionOutcome == MissionOutcome.Victory,
                unitsLost: battleResult.Casualties.Count,
                unitsKept: keptUnits);
        }
    }
}


