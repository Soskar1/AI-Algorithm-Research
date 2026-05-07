using AiAlgorithmsResearch.Core.Entities.Api;

namespace AiAlgorithmsResearch.Core.Combat.Api
{
    public interface ICombatAction
    {
        EntityId ExecutorId { get; }
        CombatActionId Id { get; }
        int Cost { get; }
        int Cooldown { get; }
    }
}