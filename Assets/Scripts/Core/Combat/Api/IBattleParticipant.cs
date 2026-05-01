using AiAlgorithmsResearch.Core.Entities.Api;

namespace AiAlgorithmsResearch.Core.Combat.Api
{
    public interface IBattleParticipant
    {
        IEntityView Entity { get; }
        int Initiative { get; }
    }
}
