using AiAlgorithmsResearch.Core.Entities.Api;
using System.Collections.Generic;

namespace AiAlgorithmsResearch.Core.Combat.Api
{
    public interface IBattle
    {
        IReadOnlyList<IBattleParticipant> TurnOrder { get; }

        IBattleParticipant Current { get; }

        IReadOnlyDictionary<IEntityView, TeamId> EntityTeams { get; }

        void NextTurn();
    }
}
