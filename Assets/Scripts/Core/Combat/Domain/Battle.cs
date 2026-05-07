using AiAlgorithmsResearch.Core.Combat.Api;
using AiAlgorithmsResearch.Core.Entities.Api;
using System.Collections.Generic;

namespace AiAlgorithmsResearch.Core.Combat.Domain
{
    internal sealed class Battle : IBattle
    {
        private readonly List<BattleParticipant> _turnOrder;
        private readonly Dictionary<EntityId, TeamId> _entityTeams;
        private int _currentIndex;

        public IReadOnlyList<IBattleParticipant> TurnOrder => _turnOrder;
        public IBattleParticipant Current => _turnOrder[_currentIndex];
        public IReadOnlyDictionary<EntityId, TeamId> EntityTeams => _entityTeams;

        public Battle(List<BattleParticipant> turnOrder)
        {
            _turnOrder = turnOrder;
            _currentIndex = 0;

            _entityTeams = new Dictionary<EntityId, TeamId>();
            foreach (var participant in _turnOrder)
            {
                _entityTeams.Add(participant.Entity.Id, participant.TeamId);
            }
        }

        public void NextTurn()
        {
            _currentIndex = (_currentIndex + 1) % _turnOrder.Count;
        }
    }
}
