using AiAlgorithmsResearch.Core.Entities.Api;
using System.Collections.Generic;

namespace AiAlgorithmsResearch.Core.Ai.Domain
{
    internal class CombatState
    {
        public IDictionary<EntityId, SimulationEntityState> Entities { get; }

        public CombatState(IDictionary<EntityId, SimulationEntityState> entities)
        {
            Entities = entities;
        }
    }
}
