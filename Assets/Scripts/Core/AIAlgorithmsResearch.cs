using UnityEngine.SceneManagement;

namespace AIAlgorithmsResearch.Core
{
    public class AIAlgorithmsResearch
    {
        private readonly int _mainMenuBuildIndex;

        public AIAlgorithmsResearch(int mainMenuBuildIndex)
        {
            _mainMenuBuildIndex = mainMenuBuildIndex;
        }

        public void EnterMainMenu() => SceneManager.LoadScene(_mainMenuBuildIndex);
    }
}

