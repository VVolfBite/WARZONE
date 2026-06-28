namespace Warzone.Application
{
    public sealed class GameFlow : IGameFlowStateReader
    {
        public bool IsMissionRunning { get; private set; }

        public void EnterMission()
        {
            IsMissionRunning = true;
        }

        public void ExitMission()
        {
            IsMissionRunning = false;
        }
    }
}
