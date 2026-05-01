using AiAlgorithmsResearch.Core.Combat.Api;

namespace AiAlgorithmsResearch.Core.Matches.Api
{
    public interface IMatchRunner
    {
        IMatchView StartMatch(BattleInitializationRequest battleRequest);
        void Tick();
    }
}