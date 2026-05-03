using AiAlgorithmsResearch.Core.Ai.Api;
using AiAlgorithmsResearch.Core.Combat.Api;
using System.Collections.Generic;
using System.Linq;

namespace AiAlgorithmsResearch.Core.Ai.Application
{
    internal sealed class CombatActionCandidateProvider
    {
        private readonly Dictionary<CombatActionId, ICombatActionCandidateGenerator> _generators;
        private readonly IActionCooldowns _actionCooldowns;

        public CombatActionCandidateProvider(IEnumerable<ICombatActionCandidateGenerator> generators, IActionCooldowns actionCooldowns)
        {
            _generators = generators.ToDictionary(generator => generator.ActionId, generator => generator);
            _actionCooldowns = actionCooldowns;
        }

        public IList<ICombatAction> GetCandidates(CombatAgentContext context)
        {
            var candidates = new List<ICombatAction>();

            foreach (var definition in context.AvailableActions)
            {
                if (!_generators.TryGetValue(definition.Id, out var generator))
                    continue;

                if (_actionCooldowns.IsOnCooldown(context.Actor, definition.Id))
                    continue;

                var generated = generator.GetCandidates(definition, context);

                if (generated == null)
                    continue;

                candidates.AddRange(generated);
            }

            if (candidates.Count == 0)
                candidates.Add(new WaitAction(context.Actor));

            return candidates;
        }
    }
}
