using AiAlgorithmsResearch.Core.Entities.Api;

namespace AiAlgorithmsResearch.Core.Entities.Domain
{
    internal sealed class Entity : IEntityView
    {
        private readonly Health _health;
        private readonly Energy _energy;

        public IHealthView Health => _health;
        public IEnergyView Energy => _energy;

        public Entity(Health health, Energy energy)
        {
            _health = health;
            _energy = energy;
        }

        public void TakeDamage(int amount) => _health.TakeDamage(amount);
        public void Heal(int amount) => _health.Heal(amount);

        public bool TrySpendEnergy(int amount) => _energy.TrySpend(amount);
        public void RegenerateEnergy() => _energy.Regenerate();
    }
}