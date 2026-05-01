namespace AiAlgorithmsResearch.Core.Entities.Api
{
    public readonly struct EntityDefinition
    {
        public int MaxHealth { get; }
        public int MaxEnergy { get; }
        public int EnergyRegenerationPerTurn { get; }
        public int Speed { get; }
        public int Strength { get; }

        public EntityDefinition(int maxHealth, int maxEnergy, int energyRegenerationPerTurn, int speed, int strength)
        {
            MaxHealth = maxHealth;
            MaxEnergy = maxEnergy;
            EnergyRegenerationPerTurn = energyRegenerationPerTurn;
            Speed = speed;
            Strength = strength;
        }
    }
}