using AiAlgorithmsResearch.Core.Combat.Api;
using AiAlgorithmsResearch.Core.Entities.Api;

namespace AiAlgorithmsResearch.Core.Combat.Application
{
    internal sealed class HealActionHandler : ICombatActionHandler
    {
        private readonly IEntityHealthEditor _healthEditor;

        public CombatActionId ActionId => CombatActionIds.Heal;

        public HealActionHandler(IEntityHealthEditor healthEditor)
        {
            _healthEditor = healthEditor;
        }

        public bool CanExecute(ICombatAction action)
        {
            var heal = (HealAction)action;
            return heal.Actor.Health.Current < heal.Actor.Health.Max;
        }

        public int GetCost(ICombatAction action)
        {
            return 2;
        }

        public bool Apply(ICombatAction action)
        {
            var heal = (HealAction)action;
            _healthEditor.Heal(heal.Actor, heal.Amount);
            return true;
        }

        public int GetCooldown(ICombatAction action)
        {
            return 3;
        }
    }
}