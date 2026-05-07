using AiAlgorithmsResearch.Core.Combat.Api;

internal interface ICombatActionHandler
{
    CombatActionId ActionId { get; }

    bool CanExecute(ICombatAction action, ICombatStateView stateView);
    bool Apply(ICombatAction action, ICombatStateView stateView, ICombatStateEditor stateEditor);
}