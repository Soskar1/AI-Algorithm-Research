using AiAlgorithmsResearch.Core.Entities.Api;
using System.Collections.Generic;

namespace AiAlgorithmsResearch.Core.Combat.Api
{
    public interface IActionCooldowns
    {
        bool IsOnCooldown(EntityId entity, CombatActionId actionId);
        int GetRemainingCooldown(EntityId entity, CombatActionId actionId);
        IDictionary<CombatActionId, int> CopyEntityCooldowns(EntityId entity);
    }
}