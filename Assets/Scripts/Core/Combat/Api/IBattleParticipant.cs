using AiAlgorithmsResearch.Core.Entities.Api;

namespace AiAlgorithmsResearch.Core.Combat.Api
{
    public interface IBattleParticipant
    {
        IEntityView Entity { get; }
        TeamId TeamId { get; }
        int Initiative { get; }
    }
}
