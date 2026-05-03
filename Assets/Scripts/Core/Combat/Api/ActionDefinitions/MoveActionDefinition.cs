using AiAlgorithmsResearch.Core.Maps.Api;
using AiAlgorithmsResearch.Core.Worlds.Api;
using System.Collections.Generic;
using UnityEngine;

namespace AiAlgorithmsResearch.Core.Combat.Api
{
    public class MoveActionDefinition : ICombatActionDefinition
    {
        public CombatActionId Id => CombatActionIds.Move;
    }
}
