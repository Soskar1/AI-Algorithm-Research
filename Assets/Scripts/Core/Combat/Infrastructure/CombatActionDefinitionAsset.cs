using AiAlgorithmsResearch.Core.Combat.Api;
using AiAlgorithmsResearch.Core.Combat.Domain;
using UnityEngine;

namespace AiAlgorithmsResearch.Core.Combat.Infrastructure
{
    internal abstract class CombatActionDefinitionAsset : ScriptableObject
    {
        public abstract ICombatActionDefinition ToCombatActionDefinition();
    }
}
