using AiAlgorithmsResearch.Core.Combat.Api;
using UnityEngine;

namespace AiAlgorithmsResearch.Core.Combat.Infrastructure
{
    internal abstract class CombatActionDefinitionAsset : ScriptableObject
    {
        public abstract ICombatActionDefinition ToCombatActionDefinition();
    }
}
