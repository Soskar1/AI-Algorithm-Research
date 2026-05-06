using AiAlgorithmsResearch.Core.Combat.Api;
using UnityEngine;

namespace AiAlgorithmsResearch.Core.Combat.Infrastructure
{
    [CreateAssetMenu(menuName = "Research/Combat/Attack Action")]
    internal class AttackCombatActionDefinitionAsset : CombatActionDefinitionAsset
    {
        [SerializeField] private int _baseDamage;
        [SerializeField] private int _range;
        [SerializeField] private int _baseCost;

        public override ICombatActionDefinition ToCombatActionDefinition()
        {
            return new AttackActionDefinition(_baseDamage, _range, _baseCost);
        }
    }
}
