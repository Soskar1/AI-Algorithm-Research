using AiAlgorithmsResearch.Core.Combat.Api;
using TMPro;
using UnityEngine;

namespace AiAlgorithmsResearch.Core.Benchmarks.Presentation
{
    public class ActionExecutionStatistics : MonoBehaviour
    {
        [SerializeField] private AlgorithmActionExecutionStatistics _firstAlgorithm;
        [SerializeField] private AlgorithmActionExecutionStatistics _secondAlgorithm;
        [SerializeField] private TextMeshProUGUI _overallExecutedActionsText;
        private int _overallExecutedActions = 0;

        private TeamId _firstTeam;
        private TeamId _secondTeam;

        private bool _isInitialized = false;

        public void Initialize(TeamId firstTeam, TeamId secondTeam)
        {
            if (_isInitialized)
                return;

            _firstTeam = firstTeam;
            _secondTeam = secondTeam;

            _firstAlgorithm.Initialize(_firstTeam.DisplayName);
            _secondAlgorithm.Initialize(_secondTeam.DisplayName);

            _isInitialized = true;
        }

        public void DisplayExecutedAction(TeamId teamId, CombatActionId combatAction)
        {
            ++_overallExecutedActions;
            _overallExecutedActionsText.text = $"Executed {_overallExecutedActions} actions";

            if (teamId.Value == _firstTeam.Value)
            {
                _firstAlgorithm.DisplayExecutedAction(combatAction, _overallExecutedActions);
            }
            else if (teamId.Value == _secondTeam.Value)
            {
                _secondAlgorithm.DisplayExecutedAction(combatAction, _overallExecutedActions);
            }
            else
            {
                Debug.LogError("Team not found in the statistics");
            }
        }
    }
}