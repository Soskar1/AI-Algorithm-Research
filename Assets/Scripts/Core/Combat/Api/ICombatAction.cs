using AiAlgorithmsResearch.Core.Entities.Api;

namespace AiAlgorithmsResearch.Core.Combat.Api
{
    public interface ICombatAction
    {
        IEntityView Actor { get; }
    }
}