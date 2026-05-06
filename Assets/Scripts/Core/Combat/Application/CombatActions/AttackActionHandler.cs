using AiAlgorithmsResearch.Core.Combat.Api;
using AiAlgorithmsResearch.Core.Worlds.Api;

namespace AiAlgorithmsResearch.Core.Combat.Application
{
    internal sealed class AttackActionHandler : ICombatActionHandler
    {
        private readonly ICombatStateView _stateView;
        private readonly ICombatStateEditor _stateEditor;
        private readonly ICombatLogger _combatLogger;

        public CombatActionId ActionId => CombatActionIds.Attack;

        public AttackActionHandler(ICombatStateView stateView, ICombatStateEditor stateEditor, ICombatLogger logger)
        {
            _stateView = stateView;
            _stateEditor = stateEditor;
            _combatLogger = logger;
        }

        public bool CanExecute(ICombatAction action)
        {
            var attack = (AttackAction)action;

            if (!_stateView.TryGetPosition(attack.Actor.Id, out var actorPosition))
                return false;

            if (!_stateView.TryGetPosition(attack.Target.Id, out var targetPosition))
                return false;

            return GridDistance.Manhattan(actorPosition, targetPosition) <= attack.Range;
        }

        public bool Apply(ICombatAction action)
        {
            var attack = (AttackAction)action;
            var damage = attack.BaseDamage + attack.Actor.Strength;

            _stateEditor.DealDamage(attack.Target.Id, damage);

            var targetWithPositionLog = _combatLogger.GetEntityRepresentation(attack.Target);
            var targetId = _combatLogger.GetEntityIdString(attack.Target);
            _combatLogger.Log(action.Actor, $"is dealing {damage} damage to {targetWithPositionLog}. {targetId} Health: {attack.Target.Health.Current}");

            return true;
        }
    }
}