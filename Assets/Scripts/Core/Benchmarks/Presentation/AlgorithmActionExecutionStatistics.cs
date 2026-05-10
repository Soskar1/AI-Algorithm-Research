using AiAlgorithmsResearch.Core.Combat.Api;
using TMPro;
using UnityEngine;

namespace AiAlgorithmsResearch.Core.Benchmarks.Presentation
{
    public class AlgorithmActionExecutionStatistics : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _algorithmName;
        [SerializeField] private TextMeshProUGUI _algorithmTotalExecutedActions;

        [SerializeField] private TextMeshProUGUI _executedMoveActionsText;
        [SerializeField] private TextMeshProUGUI _executedAttackActionsText;
        [SerializeField] private TextMeshProUGUI _executedStunActionsText;
        [SerializeField] private TextMeshProUGUI _executedTeleportActionsText;
        [SerializeField] private TextMeshProUGUI _executedHealActionsText;
        [SerializeField] private TextMeshProUGUI _executedDoNothingActionsText;

        private int _moveActions = 0;
        private int _attackActions = 0;
        private int _stunActions = 0;
        private int _teleportActions = 0;
        private int _healActions = 0;
        private int _doNothingActions = 0;
        private int _overallAlgorithmActions = 0;

        public void Initialize(string algorithmName)
        {
            _algorithmName.text = algorithmName;
            _algorithmTotalExecutedActions.text = "0";
            _executedMoveActionsText.text = "0";
            _executedAttackActionsText.text = "0";
            _executedStunActionsText.text = "0";
            _executedTeleportActionsText.text = "0";
            _executedHealActionsText.text = "0";
            _executedDoNothingActionsText.text = "0";
        }

        public void DisplayExecutedAction(CombatActionId combatActionId, int currentTotalAmountOfActions)
        {
            ++_overallAlgorithmActions;
            _algorithmTotalExecutedActions.text = $"{_overallAlgorithmActions} ({(_overallAlgorithmActions / (float)currentTotalAmountOfActions) * 100:F2}%)";

            if (combatActionId.Value == CombatActionIds.Move.Value)
            {
                ++_moveActions;
                UpdateStat(_executedMoveActionsText, _moveActions);
            }
            else if (combatActionId.Value == CombatActionIds.Attack.Value)
            {
                ++_attackActions;
                UpdateStat(_executedAttackActionsText, _attackActions);
            }
            else if (combatActionId.Value == CombatActionIds.Stun.Value)
            {
                ++_stunActions;
                UpdateStat(_executedStunActionsText, _stunActions);
            }
            else if (combatActionId.Value == CombatActionIds.Teleport.Value)
            {
                ++_teleportActions;
                UpdateStat(_executedTeleportActionsText, _teleportActions);
            }
            else if (combatActionId.Value == CombatActionIds.Heal.Value)
            {
                ++_healActions;
                UpdateStat(_executedHealActionsText, _healActions);
            }
            else if (combatActionId.Value == CombatActionIds.Wait.Value)
            {
                ++_doNothingActions;
                UpdateStat(_executedDoNothingActionsText, _doNothingActions);
            }
        }

        private void UpdateStat(TextMeshProUGUI text, int actionCount)
        {
            text.text = $"{actionCount} ({(actionCount / (float)_overallAlgorithmActions) * 100:F2}%)";
        }
    }
}