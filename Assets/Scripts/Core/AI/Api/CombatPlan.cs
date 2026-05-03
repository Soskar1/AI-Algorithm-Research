using AiAlgorithmsResearch.Core.Combat.Api;
using System.Collections.Generic;

namespace AiAlgorithmsResearch.Core.Ai.Api
{
    public sealed class CombatPlan
    {
        public IReadOnlyList<ICombatAction> Actions { get; }

        public CombatPlan(IReadOnlyList<ICombatAction> actions)
        {
            Actions = actions;
        }

        public static CombatPlan Single(ICombatAction action)
        {
            return new CombatPlan(new[] { action });
        }
    }
}
