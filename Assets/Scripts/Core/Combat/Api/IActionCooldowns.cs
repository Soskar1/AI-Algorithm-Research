using AiAlgorithmsResearch.Core.Entities.Api;

namespace AiAlgorithmsResearch.Core.Combat.Api
{
    public interface IActionCooldowns
    {
        bool IsOnCooldown(IEntityView entity, CombatActionId actionId);
        int GetRemainingCooldown(IEntityView entity, CombatActionId actionId);
    }
}