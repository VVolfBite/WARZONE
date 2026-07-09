using Warzone.Combat;
using Warzone.Core.Math;

namespace Warzone.Application.Services
{
    public sealed class TacticalCommandService
    {
        private readonly BattleService _battleService;

        public TacticalCommandService(BattleService battleService)
        {
            _battleService = battleService;
        }

        public void MoveSquad(int squadId, Vec2 destination)
        {
            _battleService.Enqueue(new MoveSquadCommand(squadId, destination));
        }

        public void DefendArea(int squadId, Vec2 center, float radius)
        {
            _battleService.Enqueue(new DefendAreaCommand(squadId, center, radius));
        }

        public void SearchPoint(int squadId, int nodeId)
        {
            _battleService.Enqueue(new SearchPointCommand(squadId, nodeId));
        }

        public void ExtractSquad(int squadId, int nodeId)
        {
            _battleService.Enqueue(new ExtractSquadCommand(squadId, nodeId));
        }

        public void ClearOrder(int squadId)
        {
            _battleService.Enqueue(new ClearSquadOrderCommand(squadId));
        }
    }
}
