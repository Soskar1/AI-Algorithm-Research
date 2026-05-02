namespace AiAlgorithmsResearch.Core.Matches.Api
{
    public interface IMatchRunner
    {
        IMatchView StartMatch(MatchInitializationRequest request);
        void Tick();
    }
}