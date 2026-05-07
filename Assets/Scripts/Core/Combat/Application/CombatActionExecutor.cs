using AiAlgorithmsResearch.Core.Combat.Api;
using System.Collections.Generic;

namespace AiAlgorithmsResearch.Core.Combat.Application
{
    internal sealed class CombatActionExecutor : ICombatActionExecutor
    {
        private readonly Dictionary<CombatActionId, ICombatActionHandler> _handlers;

        public CombatActionExecutor(Dictionary<CombatActionId, ICombatActionHandler> handlers)
        {
            _handlers = handlers;
        }

        public bool TryExecute(ICombatAction action, ICombatStateView stateView, ICombatStateEditor stateEditor)
        {
            if (!_handlers.TryGetValue(action.Id, out var handler))
                return false;

            if (stateView.IsOnCooldown(action.ExecutorId, action.Id))
                return false;

            if (!handler.CanExecute(action, stateView))
                return false;

            if (!stateEditor.TrySpendEnergy(action.ExecutorId, action.Cost))
                return false;

            if (!handler.Apply(action, stateView, stateEditor))
                return false;

            var cooldown = action.Cooldown;

            if (cooldown > 0)
                stateEditor.PutOnCooldown(action.ExecutorId, action.Id, cooldown);

            return true;
        }
    }
}