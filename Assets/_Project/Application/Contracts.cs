using Warzone.BattleDomain;
using Warzone.Content.Definitions;
using Warzone.Meta;

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

        public MissionDefinition MissionDefinition { get; }
        public RosterSnapshot RosterSnapshot { get; }
        public int Seed { get; }
    }

    public interface IBattleRuntimeHost
    {
        void StartBattle(MissionStartRequest request);
        void FinishBattle(BattleResult result);
    }
}
