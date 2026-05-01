using AiAlgorithmsResearch.Core.Combat.Api;
using AiAlgorithmsResearch.Core.Worlds.Api;
using UnityEngine;

namespace AiAlgorithmsResearch.Core.Combat.Application
{
    internal sealed class MoveActionHandler : ICombatActionHandler
    {
        private readonly IWorldView _worldView;
        private readonly IWorldEditor _worldEditor;

        public CombatActionId ActionId => CombatActionIds.Move;

        public MoveActionHandler(IWorldView worldView, IWorldEditor worldEditor)
        {
            _worldView = worldView;
            _worldEditor = worldEditor;
        }

        public bool CanExecute(ICombatAction action)
        {
            var move = (MoveAction)action;
            return _worldView.TryGetEntityPosition(move.Actor, out _);
        }

        public int GetCost(ICombatAction action)
        {
            var move = (MoveAction)action;

            if (!_worldView.TryGetEntityPosition(move.Actor, out var currentPosition))
                return int.MaxValue;

            var distance = GridDistance.Manhattan(currentPosition, move.TargetPosition);

            return Mathf.CeilToInt(distance / 2f);
        }

        public bool Apply(ICombatAction action)
        {
            var move = (MoveAction)action;
            return _worldEditor.TryMoveEntity(move.Actor, move.TargetPosition);
        }

        public int GetCooldown(ICombatAction action)
        {
            return 0;
        }
    }
}
