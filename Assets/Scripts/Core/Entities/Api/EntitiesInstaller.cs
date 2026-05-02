using AiAlgorithmsResearch.Core.Entities.Application;
using Reflex.Core;
using Reflex.Enums;
using Resolution = Reflex.Enums.Resolution;

namespace AiAlgorithmsResearch.Core.Entities.Api
{
    public static class EntitiesInstaller
    {
        public static ContainerBuilder InstallEntities(this ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterFactory<IEntityFactory>(container => new EntityFactory(), Lifetime.Singleton, Resolution.Lazy);
            containerBuilder.RegisterFactory<IEntityEnergyEditor>(container => new EntityEnergyEditor(), Lifetime.Singleton, Resolution.Lazy);
            containerBuilder.RegisterFactory<IEntityHealthEditor>(container => new EntityHealthEditor(), Lifetime.Singleton, Resolution.Lazy);

            return containerBuilder;
        }
    }
}