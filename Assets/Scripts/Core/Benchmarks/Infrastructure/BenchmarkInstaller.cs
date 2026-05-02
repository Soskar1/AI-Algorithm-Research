using AiAlgorithmsResearch.Core.Entities.Api;
using AiAlgorithmsResearch.Core.Maps.Api;
using AiAlgorithmsResearch.Core.Worlds.Api;
using Reflex.Core;
using UnityEngine;

namespace AiAlgorithmsResearch.Core.Benchmarks.Infrastructure
{
    internal class BenchmarkInstaller : MonoBehaviour, IInstaller
    {
        public void InstallBindings(ContainerBuilder containerBuilder)
        {
            containerBuilder
                .InstallMaps()
                .InstallEntities()
                .InstallWorlds();
        }
    }
}