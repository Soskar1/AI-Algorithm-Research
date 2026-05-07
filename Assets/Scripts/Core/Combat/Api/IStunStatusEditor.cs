using AiAlgorithmsResearch.Core.Entities.Api;

namespace AiAlgorithmsResearch.Core.Combat.Api
{
    public interface IStunStatusEditor
    {
        void StunForNextTurn(EntityId entity);
        bool ConsumeStun(EntityId entity);
    }
}