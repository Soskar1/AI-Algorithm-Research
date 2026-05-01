using AiAlgorithmsResearch.Core.Entities.Api;
using AiAlgorithmsResearch.Core.Entities.Domain;

namespace AiAlgorithmsResearch.Core.Entities.Application
{
    internal sealed class EntityEnergyEditor : IEntityEnergyEditor
    {
        public bool TrySpendEnergy(IEntityView entityView, int amount)
        {
            if (entityView is not Entity entity)
            {
                return false;
            }

            return entity.TrySpendEnergy(amount);
        }

        public void RegenerateEnergy(IEntityView entityView)
        {
            if (entityView is Entity entity)
            {
                entity.RegenerateEnergy();
            }
        }
    }
}
