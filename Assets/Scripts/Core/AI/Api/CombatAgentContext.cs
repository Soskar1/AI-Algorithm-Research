using AiAlgorithmsResearch.Core.Combat.Api;
using AiAlgorithmsResearch.Core.Entities.Api;
using AiAlgorithmsResearch.Core.Worlds.Api;
using System.Collections.Generic;

namespace AiAlgorithmsResearch.Core.Ai.Api
{
    public sealed class CombatAgentContext
    {
        public IBattleParticipant BattleParticipant { get; }
        public IEntityView Actor => BattleParticipant.Entity;
        public IReadOnlyCollection<ICombatActionDefinition> AvailableActions => BattleParticipant.ActionDefinitions;
        public TeamId TeamId => BattleParticipant.TeamId;
        public IWorldView World { get; }
        public IBattle Battle { get; }

        public CombatAgentContext(IBattleParticipant battleParticipant, IWorldView worldView, IBattle battle)
        {
            BattleParticipant = battleParticipant;
            World = worldView;
            Battle = battle;
        }
    }
}