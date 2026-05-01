using AiAlgorithmsResearch.Core.Combat.Api;
using AiAlgorithmsResearch.Core.Entities.Api;
using System.Collections.Generic;

namespace AiAlgorithmsResearch.Core.Combat.Application
{
    internal sealed class CombatActionExecutor : ICombatActionExecutor
    {
        private readonly Dictionary<CombatActionId, ICombatActionHandler> _handlers;
        private readonly IEntityEnergyEditor _energyEditor;
        private readonly IActionCooldowns _cooldowns;
        private readonly IActionCooldownEditor _cooldownEditor;

        public CombatActionExecutor(
            Dictionary<CombatActionId, ICombatActionHandler> handlers,
            IEntityEnergyEditor energyEditor,
            IActionCooldowns cooldowns,
            IActionCooldownEditor cooldownEditor)
        {
            _handlers = handlers;
            _energyEditor = energyEditor;
            _cooldowns = cooldowns;
            _cooldownEditor = cooldownEditor;
        }

        public bool TryExecute(ICombatAction action)
        {
            if (!_handlers.TryGetValue(action.Id, out var handler))
                return false;

            if (_cooldowns.IsOnCooldown(action.Actor, action.Id))
                return false;

            if (!handler.CanExecute(action))
                return false;

            var cost = handler.GetCost(action);

            if (!_energyEditor.TrySpendEnergy(action.Actor, cost))
                return false;

            if (!handler.Apply(action))
                return false;

            var cooldown = handler.GetCooldown(action);

            if (cooldown > 0)
                _cooldownEditor.PutOnCooldown(action.Actor, action.Id, cooldown);

            return true;
        }
    }
}