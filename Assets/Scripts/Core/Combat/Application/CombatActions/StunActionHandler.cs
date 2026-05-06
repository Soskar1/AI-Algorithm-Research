using AiAlgorithmsResearch.Core.Combat.Api;
using AiAlgorithmsResearch.Core.Worlds.Api;

namespace AiAlgorithmsResearch.Core.Combat.Application
{
    internal sealed class StunActionHandler : ICombatActionHandler
    {
        private readonly IWorldView _worldView;
        private readonly IStunStatusEditor _stunStatusEditor;
        private readonly ICombatLogger _combatLogger;

        public CombatActionId ActionId => CombatActionIds.Stun;

        public StunActionHandler(IWorldView worldView, IStunStatusEditor stunStatusEditor, ICombatLogger combatLogger)
        {
            _worldView = worldView;
            _stunStatusEditor = stunStatusEditor;
            _combatLogger = combatLogger;
        }

        public bool CanExecute(ICombatAction action)
        {
            var stun = (StunAction)action;

            if (!_worldView.TryGetEntityPosition(stun.Actor, out var actorPosition))
                return false;

            if (!_worldView.TryGetEntityPosition(stun.Target, out var targetPosition))
                return false;

            return GridDistance.Manhattan(actorPosition, targetPosition) == 1;
        }

        public bool Apply(ICombatAction action)
        {
            var stun = (StunAction)action;
            _stunStatusEditor.StunForNextTurn(stun.Target);

            var targetLog = _combatLogger.GetEntityRepresentation(stun.Target);
            _combatLogger.Log(action.Actor, $"stunned {targetLog}.");

            return true;
        }
    }
}
