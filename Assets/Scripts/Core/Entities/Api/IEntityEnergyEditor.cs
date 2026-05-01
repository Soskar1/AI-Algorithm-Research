namespace AiAlgorithmsResearch.Core.Entities.Api
{
    public interface IEntityEnergyEditor
    {
        bool TrySpendEnergy(IEntityView entity, int amount);
        void RegenerateEnergy(IEntityView entity);
    }
}
