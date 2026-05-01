
namespace AiAlgorithmsResearch.Core.Combat.Api
{
    public readonly struct CombatActionId
    {
        public string Value { get; }

        public CombatActionId(string value)
        {
            Value = value;
        }
    }
}