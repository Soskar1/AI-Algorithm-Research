using AiAlgorithmsResearch.Core.Combat.Api;
using AiAlgorithmsResearch.Core.Entities.Api;
using System.Collections.Generic;
using System.Linq;

namespace AiAlgorithmsResearch.Core.Ai.Application
{
    internal sealed class TeleportActionCandidateGenerator : ICombatActionCandidateGenerator
    {
        public CombatActionId ActionId => CombatActionIds.Teleport;

        public IEnumerable<ICombatAction> GetCandidates(ICombatActionDefinition definition, ICombatStateView combatState, EntityId executorId)
        {
            if (!Targeting.TryGetAllEnemyPositions(combatState, executorId, out var enemyPositions))
            {
                return Enumerable.Empty<ICombatAction>();
            }

            var teleportActions = new List<ICombatAction>();

            foreach (var position in enemyPositions)
            {
                var adjacentTiles = Targeting.GetAdjacentTiles(position);

                foreach (var tile in adjacentTiles)
                {
                    if (Targeting.IsValidDestination(combatState, tile))
                    {
                        var teleportAction = new TeleportAction(executorId, tile, definition.BaseCost, definition.Cooldown);
                        teleportActions.Add(teleportAction);
                        break;
                    }
                }
            }

            return teleportActions;
        }
    }
}
