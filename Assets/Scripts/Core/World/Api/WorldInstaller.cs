using AiAlgorithmsResearch.Core.Worlds.Application;
using AiAlgorithmsResearch.Core.Worlds.Domain;
using Reflex.Core;
using Reflex.Enums;
using Resolution = Reflex.Enums.Resolution;

namespace AiAlgorithmsResearch.Core.Worlds.Api
{
    public static class WorldInstaller
    {
        public static ContainerBuilder InstallWorlds(this ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterType(typeof(World), Lifetime.Singleton, Resolution.Lazy);
            containerBuilder.RegisterFactory<IWorldView>(container => container.Resolve<World>(), Lifetime.Singleton, Resolution.Lazy);
            containerBuilder.RegisterFactory<IWorldEditor>(container=> new WorldEditor(container.Resolve<World>()), Lifetime.Singleton, Resolution.Lazy);
            containerBuilder.RegisterFactory<IWorldGenerator>(container => new WorldGenerator(container.Resolve<World>()), Lifetime.Singleton, Resolution.Lazy);

            return containerBuilder;
        }
    }
}