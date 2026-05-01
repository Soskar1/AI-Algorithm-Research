using AiAlgorithmsResearch.Core.Entities.Api;
using AiAlgorithmsResearch.Core.Entities.Domain;

namespace AiAlgorithmsResearch.Core.Entities.Application
{
    internal sealed class EntityHealthEditor : IEntityHealthEditor
    {
        public void DealDamage(IEntityView entityView, int amount)
        {
            if (entityView is Entity entity)
            {
                entity.TakeDamage(amount);
            }
        }

        public void Heal(IEntityView entityView, int amount)
        {
            if (entityView is Entity entity)
            {
                entity.Heal(amount);
            }
        }
    }
}
