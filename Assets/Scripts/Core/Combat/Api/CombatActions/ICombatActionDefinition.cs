namespace AiAlgorithmsResearch.Core.Combat.Api
{
    public interface ICombatActionDefinition
    {
        CombatActionId Id { get; }
        int BaseCost { get; }
        int Cooldown { get; }
    }
}
