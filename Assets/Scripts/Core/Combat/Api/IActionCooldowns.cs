using AiAlgorithmsResearch.Core.Entities.Api;
using System.Collections.Generic;

namespace AiAlgorithmsResearch.Core.Combat.Api
{
    public interface IActionCooldowns
    {
        bool IsOnCooldown(IEntityView entity, CombatActionId actionId);
        int GetRemainingCooldown(IEntityView entity, CombatActionId actionId);
        IDictionary<CombatActionId, int> CopyEntityCooldowns(IEntityView entity);
    }
}