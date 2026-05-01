namespace AiAlgorithmsResearch.Core.Entities.Api
{
    public interface IEntityHealthEditor
    {
        void DealDamage(IEntityView entity, int amount);
        void Heal(IEntityView entity, int amount);
    }
}
