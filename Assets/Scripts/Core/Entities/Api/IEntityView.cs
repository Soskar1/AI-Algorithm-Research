namespace AiAlgorithmsResearch.Core.Entities.Api
{
    public interface IEntityView
    {
        EntityId Id { get; }
        IHealthView Health { get; }
        IEnergyView Energy { get; }
        public int Speed { get; }
        public int Strength { get; }
    }
}