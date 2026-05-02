using AiAlgorithmsResearch.Core.Ai.Api;
using AiAlgorithmsResearch.Core.Combat.Api;
using System.Collections.Generic;

namespace AiAlgorithmsResearch.Core.Matches.Api
{
    public sealed class MatchInitializationRequest
    {
        public BattleInitializationRequest BattleRequest { get; }
        public IReadOnlyDictionary<TeamId, ICombatAgent> AgentsByTeam { get; }

        public MatchInitializationRequest(BattleInitializationRequest battleRequest, IReadOnlyDictionary<TeamId, ICombatAgent> agentsByTeam)
        {
            BattleRequest = battleRequest;
            AgentsByTeam = agentsByTeam;
        }
    }
}
