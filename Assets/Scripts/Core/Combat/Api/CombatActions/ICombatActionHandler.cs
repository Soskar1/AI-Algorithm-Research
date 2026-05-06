using AiAlgorithmsResearch.Core.Combat.Api;

internal interface ICombatActionHandler
{
    CombatActionId ActionId { get; }

    bool CanExecute(ICombatAction action);
    bool Apply(ICombatAction action);
}