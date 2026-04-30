using AiAlgorithmsResearch.Core.Maps.Application;
using AiAlgorithmsResearch.Core.Maps.Domain;
using Reflex.Core;
using Reflex.Enums;
using Resolution = Reflex.Enums.Resolution;

namespace AiAlgorithmsResearch.Core.Maps.Api
{
    public static class MapInstaller
    {
        public static ContainerBuilder Install(this ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterType(typeof(Map), Lifetime.Singleton, Resolution.Lazy);
            containerBuilder.RegisterFactory<IReadOnlyTileMap>(container => container.Resolve<Map>(), Lifetime.Singleton, Resolution.Lazy);
            containerBuilder.RegisterFactory<IMapEditor>(container => new MapEditor(container.Resolve<Map>()), Lifetime.Singleton, Resolution.Lazy);
            return containerBuilder;
        }
    }
}