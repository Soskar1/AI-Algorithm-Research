using System.Collections.Generic;

namespace AiAlgorithmsResearch.Core.Combat.Api
{
    public interface IBattle
    {
        IReadOnlyList<IBattleParticipant> TurnOrder { get; }

        IBattleParticipant Current { get; }

        void NextTurn();
    }
}
