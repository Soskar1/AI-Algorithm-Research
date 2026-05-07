using AiAlgorithmsResearch.Core.Combat.Api;
using AiAlgorithmsResearch.Core.Entities.Api;
using AiAlgorithmsResearch.Core.Worlds.Api;

namespace AiAlgorithmsResearch.Core.Combat.Application
{
    internal class RuntimeCombatStateFactory : IRuntimeCombatStateFactory
    {
        private readonly IWorldView _worldView;
        private readonly IWorldEditor _worldEditor;
        private readonly IEntityHealthEditor _healthEditor;
        private readonly IEntityEnergyEditor _energyEditor;
        private readonly IActionCooldowns _cooldowns;
        private readonly IActionCooldownEditor _cooldownEditor;
        private readonly IStunStatusEditor _stunEditor;
        private readonly IStunStatus _stunStatus;

        public RuntimeCombatStateFactory(IWorldView worldView, IWorldEditor worldEditor, IEntityHealthEditor healthEditor, IEntityEnergyEditor energyEditor, IActionCooldowns cooldowns, IActionCooldownEditor cooldownEditor, IStunStatusEditor stunEditor, IStunStatus stunStatus)
        {
            _worldView = worldView;
            _worldEditor = worldEditor;
            _healthEditor = healthEditor;
            _energyEditor = energyEditor;
            _cooldowns = cooldowns;
            _cooldownEditor = cooldownEditor;
            _stunEditor = stunEditor;
            _stunStatus = stunStatus;
        }

        public (ICombatStateView, ICombatStateEditor) Create(IBattle battle)
        {
            var combatState = new RuntimeCombatState(_worldView, _worldEditor, _healthEditor, _energyEditor, _cooldowns, _cooldownEditor, _stunEditor, _stunStatus, battle);
            return (combatState, combatState);
        }
    }
}
