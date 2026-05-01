namespace AiAlgorithmsResearch.Core.Entities.Api
{
    public interface IEntityView
    {
        IHealthView Health { get; }
        IEnergyView Energy { get; }
        public int Speed { get; }
        public int Strength { get; }
    }
}