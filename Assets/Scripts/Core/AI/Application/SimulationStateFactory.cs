using AiAlgorithmsResearch.Core.Ai.Domain;
using AiAlgorithmsResearch.Core.Combat.Api;
using AiAlgorithmsResearch.Core.Entities.Api;
using AiAlgorithmsResearch.Core.Maps.Api;
using System.Collections.Generic;

namespace AiAlgorithmsResearch.Core.Ai.Application
{
    internal class SimulationStateFactory
    {
        private readonly IReadOnlyTileMap _map;

        public SimulationStateFactory(IReadOnlyTileMap map)
        {
            _map = map;
        }

        public SimulationCombatState Create(ICombatStateView currentStateView)
        {
            var simulationEntities = new Dictionary<EntityId, SimulationEntityState>();
            var actions = new Dictionary<EntityId, IReadOnlyCollection<ICombatActionDefinition>>();

            foreach (var entityId in currentStateView.EntityIds)
            {
                currentStateView.TryGetPosition(entityId, out var entityPosition);
                var teamId = currentStateView.GetTeamId(entityId);
                var isStunned = currentStateView.IsStunned(entityId);
                var cooldowns = currentStateView.GetCooldowns(entityId);
                var health = currentStateView.GetHealth(entityId);
                var maxHealth = currentStateView.GetMaxHealth(entityId);
                var energy = currentStateView.GetEnergy(entityId);
                var maxEnergy = currentStateView.GetMaxEnergy(entityId);
                var energyRegenerationPerTurn = currentStateView.GetEnergyRegenerationPerTurn(entityId);
                var strength = currentStateView.GetStrength(entityId);
                var speed = currentStateView.GetSpeed(entityId);

                var simulationEntity = new SimulationEntityState(entityId, health, maxHealth, energy, maxEnergy, strength, speed, energyRegenerationPerTurn, teamId, entityPosition, cooldowns, isStunned);
                simulationEntities.Add(entityId, simulationEntity);

                var availableActions = currentStateView.GetCombatActionDefinitions(entityId);
                actions.Add(entityId, availableActions);
            }

            var turnOrder = new List<EntityId>(currentStateView.TurnOrder);
            return new SimulationCombatState(simulationEntities, actions, _map, turnOrder, currentStateView.CurrentEntityTurn);
        }
    }
}
