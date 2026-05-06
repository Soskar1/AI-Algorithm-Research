namespace AiAlgorithmsResearch.Core.Combat.Api
{
    public class MoveActionDefinition : ICombatActionDefinition
    {
        public CombatActionId Id => CombatActionIds.Move;
        public int BaseCost => 0;
        public int Cooldown => 0;
    }
}
