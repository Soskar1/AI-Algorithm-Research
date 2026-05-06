using AiAlgorithmsResearch.Core.Ai.Api;
using AiAlgorithmsResearch.Core.Ai.Domain;
using AiAlgorithmsResearch.Core.Combat.Api;
using AiAlgorithmsResearch.Core.Entities.Api;
using System.Collections.Generic;

namespace AiAlgorithmsResearch.Core.Ai.Application
{
    internal class CombatStateFactory
    {
        private readonly IActionCooldowns _actionCooldowns;
        private readonly IStunStatus _stunStatus;

        public CombatStateFactory(IActionCooldowns actionCooldowns, IStunStatus stunStatus)
        {
            _actionCooldowns = actionCooldowns;
            _stunStatus = stunStatus;
        }

        public CombatState Create(CombatAgentContext context)
        {
            var simulationEntities = new Dictionary<EntityId, SimulationEntityState>();

            foreach (var worldEntity in context.World.Entities)
            {
                var entityView = worldEntity.Entity;
                var cooldowns = _actionCooldowns.CopyEntityCooldowns(entityView);
                var isStunned = _stunStatus.IsStunned(entityView);
                var teamId = context.Battle.EntityTeams[entityView];

                var simulationEntity = new SimulationEntityState(entityView, teamId, worldEntity.Position, cooldowns, isStunned);
                simulationEntities.Add(entityView.Id, simulationEntity);
            }

            return new CombatState(simulationEntities);
        }
    }
}
