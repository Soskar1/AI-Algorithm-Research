namespace AiAlgorithmsResearch.Core.Entities.Api
{
    public interface IEntityFactory
    {
        IEntityView CreateEntity(EntityDefinition definition);
    }
}
