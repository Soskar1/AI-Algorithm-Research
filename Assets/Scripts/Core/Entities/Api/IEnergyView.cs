namespace AiAlgorithmsResearch.Core.Entities.Api
{
    public interface IEnergyView
    {
        int Current { get; }
        int Max { get; }
        int RegenerationPerTurn { get; }
    }
}
