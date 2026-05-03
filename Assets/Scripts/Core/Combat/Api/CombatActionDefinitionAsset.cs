using UnityEngine;

namespace AiAlgorithmsResearch.Core.Combat.Api
{
    public abstract class CombatActionDefinitionAsset : ScriptableObject
    {
        public abstract ICombatActionDefinition ToCombatActionDefinition();
    }
}
