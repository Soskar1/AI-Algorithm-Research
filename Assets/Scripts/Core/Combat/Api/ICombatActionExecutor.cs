namespace AiAlgorithmsResearch.Core.Combat.Api
{
    public interface ICombatActionExecutor
    {
        bool TryExecute(ICombatAction action);
    }
}