using System.Collections.Generic;

namespace AiAlgorithmsResearch.Core.Combat.Api
{
    public interface ICombatActionProvider
    {
        IReadOnlyCollection<ICombatAction> GetAvailableActions(IBattle battle, IBattleParticipant participant);
    }
}
