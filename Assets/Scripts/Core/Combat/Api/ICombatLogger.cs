using AiAlgorithmsResearch.Core.Entities.Api;

namespace AiAlgorithmsResearch.Core.Combat.Api
{
    public interface ICombatLogger
    {
        void Log(IEntityView entity, string text);
        string GetEntityRepresentation(IEntityView entity);
        string GetEntityIdString(IEntityView entity);
    }
}
