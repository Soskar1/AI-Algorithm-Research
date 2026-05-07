namespace AiAlgorithmsResearch.Core.Combat.Api
{
    public interface IRuntimeCombatStateFactory
    {
        public (ICombatStateView, ICombatStateEditor) Create(IBattle battle);
    }
}
