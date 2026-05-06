namespace AiAlgorithmsResearch.Core.Combat.Api
{
    public class TeleportActionDefinition : ICombatActionDefinition
    {
        public CombatActionId Id => CombatActionIds.Teleport;
        public int BaseCost { get; }
        public int Cooldown { get; }

        public TeleportActionDefinition(int baseCost, int cooldown)
        {
            BaseCost = baseCost;
            Cooldown = cooldown;
        }
    }
}
