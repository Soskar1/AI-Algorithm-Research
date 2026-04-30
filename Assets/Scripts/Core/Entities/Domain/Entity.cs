using AiAlgorithmsResearch.Core.Entities.Api;

namespace AiAlgorithmsResearch.Core.Entities.Domain
{
    internal sealed class Entity : IEntityView
    {
        public int Health { get; private set; }
        public int Energy { get; private set; }

        public Entity(int health, int energy)
        {
            Health = health;
            Energy = energy;
        }

        public void ChangeHealth(int amount)
        {
            Health += amount;
        }

        public void ChangeEnergy(int amount)
        {
            Energy += amount;
        }
    }
}