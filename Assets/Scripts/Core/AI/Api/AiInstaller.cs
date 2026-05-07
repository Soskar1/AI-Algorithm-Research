using AiAlgorithmsResearch.Core.Ai.Application;
using Reflex.Core;
using Reflex.Enums;
using System.Collections.Generic;

namespace AiAlgorithmsResearch.Core.Ai.Api
{
    public static class AiInstaller
    {
        public static ContainerBuilder InstallAi(this ContainerBuilder builder)
        {
            builder.RegisterFactory(builder =>
                new CombatActionCandidateProvider(
                    new List<ICombatActionCandidateGenerator>()
                    {
                        new AttackActionCandidateGenerator(),
                        new MoveActionCandidateGenerator(),
                        new HealActionCandidateGenerator(),
                        new StunActionCandidateGenerator(),
                        new TeleportActionCandidateGenerator()
                    }
            ), Lifetime.Singleton, Resolution.Lazy);

            builder.RegisterFactory<ICombatAgentFactory>(builder => new CombatAgentFactory(), Lifetime.Singleton, Resolution.Lazy);

            return builder;
        }
    }
}
