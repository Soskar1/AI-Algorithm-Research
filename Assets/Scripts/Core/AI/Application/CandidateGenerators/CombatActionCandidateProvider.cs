using AiAlgorithmsResearch.Core.Combat.Api;
using AiAlgorithmsResearch.Core.Entities.Api;
using System.Collections.Generic;
using System.Linq;

namespace AiAlgorithmsResearch.Core.Ai.Application
{
    internal sealed class CombatActionCandidateProvider
    {
        private readonly Dictionary<CombatActionId, ICombatActionCandidateGenerator> _generators;

        public CombatActionCandidateProvider(IEnumerable<ICombatActionCandidateGenerator> generators)
        {
            _generators = generators.ToDictionary(generator => generator.ActionId, generator => generator);
        }

        public IList<ICombatAction> GetCandidates(ICombatStateView combatState, EntityId entityId)
        {
            var candidates = new List<ICombatAction>() { new WaitAction(entityId) };
            var availableActions = combatState.GetCombatActionDefinitions(entityId);

            foreach (var definition in availableActions)
            {
                if (!_generators.TryGetValue(definition.Id, out var generator))
                    continue;

                if (combatState.IsOnCooldown(entityId, definition.Id))
                    continue;

                var generated = generator.GetCandidates(definition, combatState, entityId);

                if (generated == null)
                    continue;

                candidates.AddRange(generated);
            }

            return candidates;
        }
    }
}
