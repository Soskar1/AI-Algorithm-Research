using AiAlgorithmsResearch.Core.Combat.Api;

namespace AiAlgorithmsResearch.Core.Combat.Application
{
    internal sealed class HealActionHandler : ICombatActionHandler
    {
        private readonly ICombatStateView _stateView;
        private readonly ICombatStateEditor _stateEditor;
        private readonly ICombatLogger _combatLogger;

        public CombatActionId ActionId => CombatActionIds.Heal;

        public HealActionHandler(ICombatStateView stateView, ICombatStateEditor stateEditor, ICombatLogger combatLogger)
        {
            _stateView = stateView;
            _stateEditor = stateEditor;
            _combatLogger = combatLogger;
        }

        public bool CanExecute(ICombatAction action)
        {
            var heal = (HealAction)action;
            var currentHealth = _stateView.GetHealth(heal.Actor.Id);
            var maxHealth = _stateView.GetMaxHealth(heal.Actor.Id);
            
            return currentHealth < maxHealth;
        }

        public bool Apply(ICombatAction action)
        {
            var heal = (HealAction)action;
            _stateEditor.Heal(heal.Actor.Id, heal.Amount);

            _combatLogger.Log(action.Actor, $"is healing {heal.Amount}. Health: {action.Actor.Health.Current}");

            return true;
        }
    }
}