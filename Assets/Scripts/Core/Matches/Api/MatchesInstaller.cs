using AiAlgorithmsResearch.Core.Combat.Api;
using AiAlgorithmsResearch.Core.Entities.Api;
using AiAlgorithmsResearch.Core.Matches.Application;
using AiAlgorithmsResearch.Core.Worlds.Api;
using Reflex.Core;
using Reflex.Enums;

namespace AiAlgorithmsResearch.Core.Matches.Api
{
    public static class MatchesInstaller
    {
        public static ContainerBuilder InstallMatches(this ContainerBuilder builder)
        {
            builder.RegisterFactory<IMatchRunner>(builder =>
                new MatchRunner(
                    builder.Resolve<IBattleInitializer>(),
                    builder.Resolve<IActionCooldownEditor>(),
                    builder.Resolve<IEntityEnergyEditor>(),
                    builder.Resolve<IStunStatusEditor>(),
                    builder.Resolve<ICombatActionExecutor>(),
                    builder.Resolve<IWorldEditor>(),
                    builder.Resolve<ICombatLogger>(),
                    builder.Resolve<IRuntimeCombatStateFactory>()
                    ), Lifetime.Singleton, Resolution.Lazy);

            return builder;
        }
    }
}
