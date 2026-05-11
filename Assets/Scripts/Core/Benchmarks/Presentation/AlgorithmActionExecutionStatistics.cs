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
        [SerializeField] private TextMeshProUGUI _executedRangedActionsText;

        private int _moveActions = 0;
        private int _attackActions = 0;
        private int _stunActions = 0;
        private int _teleportActions = 0;
        private int _healActions = 0;
        private int _doNothingActions = 0;
        private int _rangedAction = 0;
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
            _executedRangedActionsText.text = "0";
        }

        public void UpdateTotal(int currentTotalAmountOfActions)
        {
            _algorithmTotalExecutedActions.text = $"{_overallAlgorithmActions} ({(_overallAlgorithmActions / (float)currentTotalAmountOfActions) * 100:F2}%)";
        }

        public void DisplayExecutedAction(CombatActionId combatActionId)
        {
            ++_overallAlgorithmActions;

            if (combatActionId.Value == CombatActionIds.Move.Value)
            {
                ++_moveActions;
            }
            else if (combatActionId.Value == CombatActionIds.Attack.Value)
            {
                ++_attackActions;
            }
            else if (combatActionId.Value == CombatActionIds.Stun.Value)
            {
                ++_stunActions;
            }
            else if (combatActionId.Value == CombatActionIds.Teleport.Value)
            {
                ++_teleportActions;
            }
            else if (combatActionId.Value == CombatActionIds.Heal.Value)
            {
                ++_healActions;
            }
            else if (combatActionId.Value == CombatActionIds.Wait.Value)
            {
                ++_doNothingActions;
            }
            else if (combatActionId.Value == CombatActionIds.RangedAttack.Value)
            {
                ++_rangedAction;
            }

            UpdateStat(_executedMoveActionsText, _moveActions);
            UpdateStat(_executedAttackActionsText, _attackActions);
            UpdateStat(_executedStunActionsText, _stunActions);
            UpdateStat(_executedTeleportActionsText, _teleportActions);
            UpdateStat(_executedHealActionsText, _healActions);
            UpdateStat(_executedDoNothingActionsText, _doNothingActions);
            UpdateStat(_executedRangedActionsText, _rangedAction);
        }

        private void UpdateStat(TextMeshProUGUI text, int actionCount)
        {
            text.text = $"{actionCount} ({(actionCount / (float)_overallAlgorithmActions) * 100:F2}%)";
        }
    }
}