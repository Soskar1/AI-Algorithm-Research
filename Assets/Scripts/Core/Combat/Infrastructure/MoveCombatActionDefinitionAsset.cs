using AiAlgorithmsResearch.Core.Combat.Api;
using AiAlgorithmsResearch.Core.Combat.Domain;
using UnityEngine;

namespace AiAlgorithmsResearch.Core.Combat.Infrastructure
{
    [CreateAssetMenu(menuName = "Research/Combat/Move Action")]
    internal class MoveCombatActionDefinitionAsset : CombatActionDefinitionAsset
    {
        public override ICombatActionDefinition ToCombatActionDefinition()
        {
            return new MoveActionDefinition();
        }
    }
}
