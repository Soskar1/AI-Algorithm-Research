using System.Collections.Generic;

namespace AiAlgorithmsResearch.Core.Combat.Api
{
    public sealed class BattleInitializationRequest
    {
        public IReadOnlyCollection<BattleParticipantSetup> Participants { get; }

        public BattleInitializationRequest(IReadOnlyCollection<BattleParticipantSetup> participants)
        {
            Participants = participants;
        }
    }
}