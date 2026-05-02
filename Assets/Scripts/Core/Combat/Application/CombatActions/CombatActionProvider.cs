using AiAlgorithmsResearch.Core.Combat.Api;
using AiAlgorithmsResearch.Core.Combat.Domain;
using AiAlgorithmsResearch.Core.Worlds.Api;
using System.Collections.Generic;

namespace AiAlgorithmsResearch.Core.Combat.Application
{
    internal sealed class CombatActionProvider : ICombatActionProvider
    {
        private readonly IWorldView _worldView;

        public CombatActionProvider(IWorldView worldView)
        {
            _worldView = worldView;
        }

        public IReadOnlyCollection<ICombatAction> GetAvailableActions(IBattle battle, IBattleParticipant participant)
        {
            var actor = participant.Entity;
            var actions = new List<ICombatAction>
            {
                new WaitAction(actor)
            };

            var possibleActions = GetActions(battle, participant);
            actions.AddRange(possibleActions);

            return actions;
        }

        private IEnumerable<ICombatAction> GetActions(IBattle battle, IBattleParticipant battleParticipant)
        {
            List<ICombatAction> combatActions = new();
            foreach (var action in battleParticipant.ActionDefinitions)
            {
                var actions = GetActions(battle, battleParticipant, action);
                combatActions.AddRange(actions);
            }

            return combatActions;
        }

        private IEnumerable<ICombatAction> GetActions(IBattle battle, IBattleParticipant battleParticipant, ICombatActionDefinition actionDefinition)
        {
            // Add other actions later:
            // Attack
            // Heal
            // Teleport
            // Stun

            switch (actionDefinition)
            {
                case MoveActionDefinition moveAction:
                    if (!_worldView.TryGetEntityPosition(battleParticipant.Entity, out var position))
                    {
                        return null;
                    }

                    return moveAction.GetMoveAction(_worldView, battle, battleParticipant, position);

                case AttackActionDefinition attackAction:
                    return null;

                default:
                    return null;
            }
        }
    }
}
