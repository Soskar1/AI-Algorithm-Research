using AiAlgorithmsResearch.Core.Entities.Api;

namespace AiAlgorithmsResearch.Core.Entities.Domain
{
    internal sealed class Entity : IEntityView
    {
        public EntityId Id { get; }
        public IHealthView Health => _health;
        public IEnergyView Energy => _energy;
        public int Speed { get; }
        public int Strength { get; }

        private readonly Health _health;
        private readonly Energy _energy;

        public Entity(EntityId id, Health health, Energy energy, int speed, int strength)
        {
            Id = id;
            _health = health;
            _energy = energy;
            Speed = speed;
            Strength = strength;
        }

        public void TakeDamage(int amount) => _health.TakeDamage(amount);
        public void Heal(int amount) => _health.Heal(amount);

        public bool TrySpendEnergy(int amount) => _energy.TrySpend(amount);
        public void RegenerateEnergy() => _energy.Regenerate();
    }
}