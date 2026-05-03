using AiAlgorithmsResearch.Core.Combat.Api;
using UnityEngine;

namespace AiAlgorithmsResearch.Core.Combat.Infrastructure
{
    [CreateAssetMenu(menuName = "Research/Combat/Stun Action")]
    internal class StunCombatActionDefinitionAsset : CombatActionDefinitionAsset
    {
        [SerializeField] private int _range;

        public override ICombatActionDefinition ToCombatActionDefinition()
        {
            return new StunActionDefinition(_range);
        }
    }
}
