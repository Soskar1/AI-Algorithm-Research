using Reflex.Attributes;
using UnityEngine;

namespace AIAlgorithmsResearch.Core
{
    public class RootBootstrap : MonoBehaviour
    {
        private AIAlgorithmsResearch _aiAlgorithmsResearch;

        [Inject]
        public void Inject(AIAlgorithmsResearch aiAlgorithmsResearch)
        {
            _aiAlgorithmsResearch = aiAlgorithmsResearch;
        }

        public void Start()
        {
            _aiAlgorithmsResearch.EnterMainMenu();
        }
    }
}