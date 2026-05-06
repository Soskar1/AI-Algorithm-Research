using AiAlgorithmsResearch.Core.Combat.Api;
using UnityEngine;

namespace AiAlgorithmsResearch.Core.Combat.Infrastructure
{
    [CreateAssetMenu(menuName = "Research/Combat/Heal Action")]
    internal class HealCombatActionDefinitionAsset : CombatActionDefinitionAsset
    {
        [SerializeField] private int _amount;
        [SerializeField] private int _baseCost;
        [SerializeField] private int _cooldown;

        public override ICombatActionDefinition ToCombatActionDefinition()
        {
            return new HealActionDefinition(_amount, _baseCost, _cooldown);
        }
    }
}
