using AiAlgorithmsResearch.Core.Entities.Api;
using UnityEngine;

namespace AiAlgorithmsResearch.Core.Entities.Infrastructure
{
    [CreateAssetMenu(menuName = "Research/Entities/Entity Definition")]
    internal class EntityDefinitionAsset : ScriptableObject
    {
        [SerializeField] private int _maxHealth = 100;
        [SerializeField] private int _maxEnergy = 10;
        [SerializeField] private int _energyRegenPerTurn = 2;
        [SerializeField] private int _speed = 1;
        [SerializeField] private int _strength = 1;

        public EntityDefinition ToDefinition()
        {
            return new EntityDefinition(_maxHealth, _maxEnergy, _energyRegenPerTurn, _speed, _strength);
        }
    }
}
