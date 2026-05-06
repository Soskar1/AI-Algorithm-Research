using AiAlgorithmsResearch.Core.Combat.Api;
using System.Collections.Generic;

namespace AiAlgorithmsResearch.Core.Combat.Application
{
    internal sealed class CombatActionExecutor : ICombatActionExecutor
    {
        private readonly Dictionary<CombatActionId, ICombatActionHandler> _handlers;

        private readonly ICombatStateEditor _stateEditor;
        private readonly ICombatStateView _stateView;

        public CombatActionExecutor(
            Dictionary<CombatActionId, ICombatActionHandler> handlers,
            ICombatStateView stateView,
            ICombatStateEditor stateEditor)
        {
            _handlers = handlers;
            _stateEditor = stateEditor;
            _stateView = stateView;
        }

        public bool TryExecute(ICombatAction action)
        {
            if (!_handlers.TryGetValue(action.Id, out var handler))
                return false;

            if (_stateView.IsOnCooldown(action.Actor.Id, action.Id))
                return false;

            if (!handler.CanExecute(action))
                return false;

            if (!_stateEditor.TrySpendEnergy(action.Actor.Id, action.Cost))
                return false;

            if (!handler.Apply(action))
                return false;

            var cooldown = action.Cooldown;

            if (cooldown > 0)
                _stateEditor.PutOnCooldown(action.Actor.Id, action.Id, cooldown);

            return true;
        }
    }
}