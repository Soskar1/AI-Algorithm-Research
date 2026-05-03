using AiAlgorithmsResearch.Core.Ai.Api;
using AiAlgorithmsResearch.Core.AI.Application;
using AiAlgorithmsResearch.Core.Combat.Api;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Scripts.Core.AI.Application
{
    internal sealed class TeleportActionCandidateGenerator : ICombatActionCandidateGenerator
    {
        public CombatActionId ActionId => CombatActionIds.Teleport;

        public IEnumerable<ICombatAction> GetCandidates(ICombatActionDefinition definition, CombatAgentContext context)
        {
            if (!context.World.TryGetEntityPosition(context.Actor, out var actorPosition))
                return Enumerable.Empty<ICombatAction>();

            if (!Targeting.TryGetClosestEnemyPosition(context, actorPosition, out var enemyPosition))
            {
                return Enumerable.Empty<ICombatAction>();
            }

            if (!Targeting.TryGetClosestValidAdjacentTileToTarget(context, enemyPosition, enemyPosition, out var teleportTarget))
            {
                return Enumerable.Empty<ICombatAction>();
            }

            return new List<ICombatAction>()
            {
                new TeleportAction(context.Actor, teleportTarget)
            };
        }
    }
}
