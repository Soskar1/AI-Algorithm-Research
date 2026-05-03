using AiAlgorithmsResearch.Core.Combat.Api;
using UnityEngine;

namespace AiAlgorithmsResearch.Core.Combat.Infrastructure
{
    [CreateAssetMenu(menuName = "Research/Combat/Heal Action")]
    internal class HealCombatActionDefinitionAsset : CombatActionDefinitionAsset
    {
        [SerializeField] private int _amount;

        public override ICombatActionDefinition ToCombatActionDefinition()
        {
            return new HealActionDefinition(_amount);
        }
    }
}
