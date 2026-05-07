using AiAlgorithmsResearch.Core.Entities.Api;

namespace AiAlgorithmsResearch.Core.Combat.Api
{
    public interface ICombatLogger
    {
        void Log(EntityId entityId, string text, ICombatStateView combatStateView);
        string GetEntityRepresentation(EntityId entityId, ICombatStateView combatStateView);
        string GetEntityDisplayName(EntityId entityId);
    }
}
