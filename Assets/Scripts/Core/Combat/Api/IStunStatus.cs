using AiAlgorithmsResearch.Core.Entities.Api;

namespace AiAlgorithmsResearch.Core.Combat.Api
{
    public interface IStunStatus
    {
        bool IsStunned(EntityId entity);
    }
}