using AiAlgorithmsResearch.Core.Combat.Api;

namespace AiAlgorithmsResearch.Core.Combat.Application
{
    internal sealed class HealActionHandler : ICombatActionHandler
    {
        private readonly ICombatLogger _combatLogger;

        public HealActionHandler(ICombatLogger combatLogger)
        {
            _combatLogger = combatLogger;
        }

        public bool CanExecute(ICombatAction action, ICombatStateView stateView)
        {
            var heal = (HealAction)action;
            var currentHealth = stateView.GetHealth(heal.ExecutorId);
            var maxHealth = stateView.GetMaxHealth(heal.ExecutorId);
            
            return currentHealth < maxHealth;
        }

        public bool Apply(ICombatAction action, ICombatStateView stateView, ICombatStateEditor stateEditor)
        {
            var heal = (HealAction)action;
            stateEditor.Heal(heal.ExecutorId, heal.Amount);

            var health = stateView.GetHealth(action.ExecutorId);
            _combatLogger.Log(action.ExecutorId, $"is healing {heal.Amount}. Health: {health}", stateView);

            return true;
        }
    }
}