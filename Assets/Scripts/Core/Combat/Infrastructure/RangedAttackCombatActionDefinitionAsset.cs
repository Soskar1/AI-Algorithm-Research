using AiAlgorithmsResearch.Core.Combat.Api;
using UnityEngine;

namespace AiAlgorithmsResearch.Core.Combat.Infrastructure
{
    [CreateAssetMenu(menuName = "Research/Combat/Ranged Attack Action")]
    internal class RangedAttackCombatActionDefinitionAsset : CombatActionDefinitionAsset
    {
        [SerializeField] private int _baseDamage;
        [SerializeField] private int _range;
        [SerializeField] private int _baseCost;
        [SerializeField] private int _cooldown;

        public override ICombatActionDefinition ToCombatActionDefinition()
        {
            return new RangedAttackActionDefinition(_baseDamage, _range, _baseCost, _cooldown);
        }
    }
}
