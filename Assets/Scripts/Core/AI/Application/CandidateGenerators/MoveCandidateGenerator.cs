using AiAlgorithmsResearch.Core.Ai.Api;
using AiAlgorithmsResearch.Core.Combat.Api;
using AiAlgorithmsResearch.Core.Worlds.Api;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AiAlgorithmsResearch.Core.Ai.Application
{
    internal sealed class MoveActionCandidateGenerator : ICombatActionCandidateGenerator
    {
        public CombatActionId ActionId => CombatActionIds.Move;

        public IEnumerable<ICombatAction> GetCandidates(ICombatActionDefinition definition, CombatAgentContext context)
        {
            if (!context.World.TryGetEntityPosition(context.Actor, out var actorPosition))
                return Enumerable.Empty<ICombatAction>();

            if (!Targeting.TryGetClosestEnemyPosition(context, actorPosition, out var enemyPosition))
            {
                return Enumerable.Empty<ICombatAction>();
            }

            if (!Targeting.TryGetClosestValidAdjacentTileToTarget(context, actorPosition, enemyPosition, out var moveTarget))
            {
                return Enumerable.Empty<ICombatAction>();
            }

            var distance = GridDistance.Manhattan(actorPosition, moveTarget);
            var cost = Mathf.CeilToInt(distance / 2f);

            return new List<ICombatAction>()
            {
                new MoveAction(context.Actor, moveTarget, cost)
            };
        }
    }
}
