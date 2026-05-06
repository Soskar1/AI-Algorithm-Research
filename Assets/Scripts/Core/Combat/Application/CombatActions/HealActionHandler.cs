using AiAlgorithmsResearch.Core.Combat.Api;
using AiAlgorithmsResearch.Core.Entities.Api;

namespace AiAlgorithmsResearch.Core.Combat.Application
{
    internal sealed class HealActionHandler : ICombatActionHandler
    {
        private readonly IEntityHealthEditor _healthEditor;
        private readonly ICombatLogger _combatLogger;

        public CombatActionId ActionId => CombatActionIds.Heal;

        public HealActionHandler(IEntityHealthEditor healthEditor, ICombatLogger combatLogger)
        {
            _healthEditor = healthEditor;
            _combatLogger = combatLogger;
        }

        public bool CanExecute(ICombatAction action)
        {
            var heal = (HealAction)action;
            return heal.Actor.Health.Current < heal.Actor.Health.Max;
        }

        public bool Apply(ICombatAction action)
        {
            var heal = (HealAction)action;
            _healthEditor.Heal(heal.Actor, heal.Amount);

            _combatLogger.Log(action.Actor, $"is healing {heal.Amount}. Health: {action.Actor.Health.Current}");

            return true;
        }
    }
}