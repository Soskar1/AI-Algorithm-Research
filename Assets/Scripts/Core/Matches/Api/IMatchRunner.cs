using System;

namespace AiAlgorithmsResearch.Core.Matches.Api
{
    public interface IMatchRunner
    {
        IMatchView StartMatch(MatchInitializationRequest request, Random random);
        void Tick();
    }
}