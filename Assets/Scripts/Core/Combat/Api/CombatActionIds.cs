namespace AiAlgorithmsResearch.Core.Combat.Api
{
    public static class CombatActionIds
    {
        public static readonly CombatActionId Move = new("move");
        public static readonly CombatActionId Wait = new("wait");
        public static readonly CombatActionId Attack = new("attack");
        public static readonly CombatActionId Teleport = new("teleport");
    }
}
