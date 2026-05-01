using AiAlgorithmsResearch.Core.Combat.Api;
using AiAlgorithmsResearch.Core.Combat.Domain;
using AiAlgorithmsResearch.Core.Worlds.Api;
using System.Collections.Generic;
using System.Linq;

namespace AiAlgorithmsResearch.Core.Combat.Application
{
    internal sealed class BattleInitializer : IBattleInitializer
    {
        private readonly IWorldEditor _worldEditor;
        private readonly IInitiativeRoller _initiativeRoller;

        public BattleInitializer(IWorldEditor worldEditor, IInitiativeRoller initiativeRoller)
        {
            _worldEditor = worldEditor;
            _initiativeRoller = initiativeRoller;
        }

        public IBattle StartBattle(BattleInitializationRequest request)
        {
            var spawnedEntities = new List<BattleParticipantSetup>();

            foreach (var participant in request.Participants)
            {
                var added = _worldEditor.TryAddEntity(participant.Entity, participant.SpawnPosition);

                if (!added)
                {
                    return null;
                }

                spawnedEntities.Add(participant);
            }

            var participants = spawnedEntities
                .Select(participant => new BattleParticipant(participant.Entity, _initiativeRoller.Roll(participant.Entity)))
                .OrderByDescending(participant => participant.Initiative)
                .ToList();

            return new Battle(participants);
        }
    }
}