using AiAlgorithmsResearch.Core.Entities.Api;
using AiAlgorithmsResearch.Core.Entities.Domain;
using System;

namespace AiAlgorithmsResearch.Core.Entities.Application
{
    internal sealed class EntityFactory : IEntityFactory
    {
        public IEntityView CreateEntity(EntityDefinition definition)
        {
            var health = new Health(definition.MaxHealth);
            var energy = new Energy(definition.MaxEnergy, definition.EnergyRegenerationPerTurn);
            var id = new EntityId(Guid.NewGuid());

            return new Entity(id, health, energy, definition.Speed, definition.Strength);
        }
    }
}
