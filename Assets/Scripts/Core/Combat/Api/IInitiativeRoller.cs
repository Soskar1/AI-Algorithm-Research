using AiAlgorithmsResearch.Core.Entities.Api;

namespace AiAlgorithmsResearch.Core.Combat.Api
{
    public interface IInitiativeRoller
    {
        int Roll(IEntityView entity);
    }
}
