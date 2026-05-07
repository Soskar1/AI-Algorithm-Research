using AiAlgorithmsResearch.Core.Entities.Api;

namespace AiAlgorithmsResearch.Core.Combat.Api
{
    public interface IActionCooldownEditor
    {
        void PutOnCooldown(EntityId entity, CombatActionId actionId, int turns);
        void TickCooldowns(EntityId entity);
        void Clear();
    }
}   