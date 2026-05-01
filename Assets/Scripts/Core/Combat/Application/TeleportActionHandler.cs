using AiAlgorithmsResearch.Core.Combat.Api;
using AiAlgorithmsResearch.Core.Worlds.Api;

namespace AiAlgorithmsResearch.Core.Combat.Application
{
    internal sealed class TeleportActionHandler : ICombatActionHandler
    {
        private readonly IWorldView _worldView;
        private readonly IWorldEditor _worldEditor;

        public CombatActionId ActionId => CombatActionIds.Teleport;

        public TeleportActionHandler(IWorldView worldView, IWorldEditor worldEditor)
        {
            _worldView = worldView;
            _worldEditor = worldEditor;
        }

        public bool CanExecute(ICombatAction action)
        {
            var teleport = (TeleportAction)action;
            return _worldView.TryGetEntityPosition(teleport.Actor, out _);
        }

        public int GetCost(ICombatAction action) => 2;

        public bool Apply(ICombatAction action)
        {
            var teleport = (TeleportAction)action;
            return _worldEditor.TryMoveEntity(teleport.Actor, teleport.TargetPosition);
        }

        public int GetCooldown(ICombatAction action) => 4;
    }
}
