using AiAlgorithmsResearch.Core.Benchmarks.Infrastructure;
using AiAlgorithmsResearch.Core.Combat.Api;
using TMPro;
using UnityEngine;

namespace AiAlgorithmsResearch.Core.Benchmarks.Presentation
{
    internal class ActionExecutionStatistics : MonoBehaviour
    {
        [SerializeField] private AlgorithmActionExecutionStatistics _firstAlgorithm;
        [SerializeField] private AlgorithmActionExecutionStatistics _secondAlgorithm;
        [SerializeField] private TextMeshProUGUI _overallExecutedActionsText;
        private int _overallExecutedActions = 0;

        private CombatAgentType _firstAgent;
        private CombatAgentType _secondAgent;

        public void Initialize(CombatAgentType firstAgent, CombatAgentType secondAgent)
        {
            _firstAgent = firstAgent;
            _secondAgent = secondAgent;

            _firstAlgorithm.Initialize(_firstAgent.ToString());
            _secondAlgorithm.Initialize(_secondAgent.ToString());
        }

        public void DisplayExecutedAction(CombatAgentType agent, CombatActionId combatAction)
        {
            ++_overallExecutedActions;
            _overallExecutedActionsText.text = $"Executed {_overallExecutedActions} actions";

            _firstAlgorithm.UpdateTotal(_overallExecutedActions);
            _secondAlgorithm.UpdateTotal(_overallExecutedActions);

            if (agent == _firstAgent)
            {
                _firstAlgorithm.DisplayExecutedAction(combatAction);
            }
            else if (agent == _secondAgent)
            {
                _secondAlgorithm.DisplayExecutedAction(combatAction);
            }
            else
            {
                Debug.LogError("Team not found in the statistics");
            }
        }
    }
}