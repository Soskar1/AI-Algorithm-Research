using AiAlgorithmsResearch.Core.Ai.Application;
using AiAlgorithmsResearch.Core.Combat.Api;
using AiAlgorithmsResearch.Core.Maps.Api;
using Reflex.Core;
using Reflex.Enums;
using System.Collections.Generic;

namespace AiAlgorithmsResearch.Core.Ai.Api
{
    public static class AiInstaller
    {
        public static ContainerBuilder InstallAi(this ContainerBuilder builder)
        {
            builder.RegisterFactory<ICombatAgentFactory>(builder =>
                {
                    var provider = new CombatActionCandidateProvider(new List<ICombatActionCandidateGenerator>()
                        {
                            new AttackActionCandidateGenerator(),
                            new MoveActionCandidateGenerator(),
                            new HealActionCandidateGenerator(),
                            new StunActionCandidateGenerator(),
                            new TeleportActionCandidateGenerator(),
                            new RangedAttackActionCandidateGenerator()
                        });

                    var map = builder.Resolve<IReadOnlyTileMap>();
                    var actionExecutor = builder.Resolve<ICombatActionExecutor>();
                    
                    return new CombatAgentFactory(provider, map, actionExecutor);
                }, Lifetime.Singleton, Resolution.Lazy);

            return builder;
        }
    }
}
