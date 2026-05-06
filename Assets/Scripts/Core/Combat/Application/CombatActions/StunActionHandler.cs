using AiAlgorithmsResearch.Core.Combat.Api;
using AiAlgorithmsResearch.Core.Worlds.Api;

namespace AiAlgorithmsResearch.Core.Combat.Application
{
    internal sealed class StunActionHandler : ICombatActionHandler
    {
        private readonly ICombatStateView _stateView;
        private readonly ICombatStateEditor _stateEditor;
        private readonly ICombatLogger _combatLogger;

        public CombatActionId ActionId => CombatActionIds.Stun;

        public StunActionHandler(ICombatStateView stateView, ICombatStateEditor stateEditor, ICombatLogger combatLogger)
        {
            _stateView = stateView;
            _stateEditor = stateEditor;
            _combatLogger = combatLogger;
        }

        public bool CanExecute(ICombatAction action)
        {
            var stun = (StunAction)action;

            if (!_stateView.TryGetPosition(stun.Actor.Id, out var actorPosition))
                return false;

            if (!_stateView.TryGetPosition(stun.Target.Id, out var targetPosition))
                return false;

            return GridDistance.Manhattan(actorPosition, targetPosition) == 1;
        }

        public bool Apply(ICombatAction action)
        {
            var stun = (StunAction)action;
            _stateEditor.StunForNextTurn(stun.Target.Id);

            var targetLog = _combatLogger.GetEntityRepresentation(stun.Target);
            _combatLogger.Log(action.Actor, $"stunned {targetLog}.");

            return true;
        }
    }
}
