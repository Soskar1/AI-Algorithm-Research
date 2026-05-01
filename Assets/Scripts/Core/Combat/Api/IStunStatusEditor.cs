using AiAlgorithmsResearch.Core.Entities.Api;

namespace AiAlgorithmsResearch.Core.Combat.Api
{
    public interface IStunStatusEditor
    {
        void StunForNextTurn(IEntityView entity);
        bool ConsumeStun(IEntityView entity);
    }
}