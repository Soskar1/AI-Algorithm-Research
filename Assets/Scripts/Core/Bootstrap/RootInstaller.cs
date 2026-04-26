using Reflex.Core;
using Reflex.Enums;
using UnityEngine;
using Resolution = Reflex.Enums.Resolution;

namespace AIAlgorithmsResearch.Core
{
    public class RootInstaller : MonoBehaviour, IInstaller
    {
        [SerializeField] private int _mainMenuBuildIndex;

        public void InstallBindings(ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterFactory(container => new AIAlgorithmsResearch(_mainMenuBuildIndex), Lifetime.Singleton, Resolution.Lazy);       
        }
    }
}