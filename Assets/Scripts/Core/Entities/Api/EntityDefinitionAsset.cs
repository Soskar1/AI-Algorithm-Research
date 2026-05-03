using UnityEngine;

namespace AiAlgorithmsResearch.Core.Entities.Api
{
    [CreateAssetMenu(menuName = "Research/Entities/Entity Definition")]
    public class EntityDefinitionAsset : ScriptableObject
    {
        [SerializeField] private string _id;
        [SerializeField] private string _displayName;
        [SerializeField] private int _maxHealth = 100;
        [SerializeField] private int _maxEnergy = 10;
        [SerializeField] private int _energyRegenPerTurn = 2;
        [SerializeField] private int _speed = 1;
        [SerializeField] private int _strength = 1;
        public EntityDefinitionId Id => new EntityDefinitionId(_id);

        public EntityDefinition ToDefinition()
        {
            return new EntityDefinition(_maxHealth, _maxEnergy, _energyRegenPerTurn, _speed, _strength, _displayName);
        }
    }
}
