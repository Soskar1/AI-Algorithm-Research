using AiAlgorithmsResearch.Core.Combat.Api;
using System.Collections.Generic;

namespace AiAlgorithmsResearch.Core.Combat.Domain
{
    internal sealed class Battle : IBattle
    {
        private readonly List<BattleParticipant> _turnOrder;
        private int _currentIndex;

        public IReadOnlyList<IBattleParticipant> TurnOrder => _turnOrder;
        public IBattleParticipant Current => _turnOrder[_currentIndex];

        public Battle(List<BattleParticipant> turnOrder)
        {
            _turnOrder = turnOrder;
            _currentIndex = 0;
        }

        public void NextTurn()
        {
            _currentIndex = (_currentIndex + 1) % _turnOrder.Count;
        }
    }
}
