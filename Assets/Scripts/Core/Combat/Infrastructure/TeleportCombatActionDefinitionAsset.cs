using AiAlgorithmsResearch.Core.Combat.Api;
using UnityEngine;

namespace AiAlgorithmsResearch.Core.Combat.Infrastructure
{
    [CreateAssetMenu(menuName = "Research/Combat/Teleport Action")]
    internal class TeleportCombatActionDefinitionAsset : CombatActionDefinitionAsset
    {
        public override ICombatActionDefinition ToCombatActionDefinition()
        {
            return new TeleportActionDefinition();
        }
    }
}
