using AiAlgorithmsResearch.Core.Entities.Api;
using System.Collections.Generic;
using UnityEngine;

namespace AiAlgorithmsResearch.Core.Combat.Api
{
    [CreateAssetMenu(menuName = "Research/Combat/Combat Entity")]
    public class BattleParticipantAsset : ScriptableObject
    {
        [SerializeField] private EntityDefinitionAsset _entityDefinition;
        [SerializeField] private List<CombatActionDefinitionAsset> _combatActions;

        public EntityDefinitionAsset EntityDefinition => _entityDefinition;
        public List<CombatActionDefinitionAsset> CombatActions => _combatActions;
    }
}
