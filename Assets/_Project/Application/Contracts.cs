using Warzone.Combat;
using Warzone.Content.Definitions;
using Warzone.Campaign;

namespace Warzone.Application
{
    public interface IMissionFlow
    {
        void StartMission(MissionStartRequest request);
    }

    public interface IGameFlowStateReader
    {
        bool IsMissionRunning { get; }
    }

    public sealed class MissionStartRequest
    {
        public MissionStartRequest(
            MissionDefinition missionDefinition,
            RosterSnapshot rosterSnapshot,
            int seed)
        {
            MissionDefinition = missionDefinition;
            RosterSnapshot = rosterSnapshot;
            Seed = seed;
        }

        public MissionDefinition MissionDefinition { get; private set; }
        public RosterSnapshot RosterSnapshot { get; private set; }
        public int Seed { get; private set; }
    }

    public interface IBattleRuntimeHost
    {
        void StartBattle(MissionStartRequest request);
        void FinishBattle(BattleResult result);
    }
}


