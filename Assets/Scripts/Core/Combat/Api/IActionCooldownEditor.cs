using AiAlgorithmsResearch.Core.Entities.Api;

namespace AiAlgorithmsResearch.Core.Combat.Api
{
    public interface IActionCooldownEditor
    {
        void PutOnCooldown(IEntityView entity, CombatActionId actionId, int turns);
        void TickCooldowns(IEntityView entity);
    }
}   