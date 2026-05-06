using AiAlgorithmsResearch.Core.Combat.Api;
using UnityEngine;

namespace AiAlgorithmsResearch.Core.Combat.Infrastructure
{
    [CreateAssetMenu(menuName = "Research/Combat/Teleport Action")]
    internal class TeleportCombatActionDefinitionAsset : CombatActionDefinitionAsset
    {
        [SerializeField] private int _baseCost;
        [SerializeField] private int _cooldown;

        public override ICombatActionDefinition ToCombatActionDefinition()
        {
            return new TeleportActionDefinition(_baseCost, _cooldown);
        }
    }
}
