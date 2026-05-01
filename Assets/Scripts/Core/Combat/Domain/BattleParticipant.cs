using AiAlgorithmsResearch.Core.Combat.Api;
using AiAlgorithmsResearch.Core.Entities.Api;

namespace AiAlgorithmsResearch.Core.Combat.Domain
{
    internal sealed class BattleParticipant : IBattleParticipant
    {
        public IEntityView Entity { get; }
        public int Initiative { get; }
        public TeamId TeamId { get; }

        public BattleParticipant(IEntityView entity, int initiative, TeamId teamId)
        {
            Entity = entity;
            Initiative = initiative;
            TeamId = teamId;
        }
    }
}
