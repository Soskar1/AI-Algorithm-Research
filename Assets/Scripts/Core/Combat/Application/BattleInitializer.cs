using AiAlgorithmsResearch.Core.Combat.Api;
using AiAlgorithmsResearch.Core.Combat.Domain;
using AiAlgorithmsResearch.Core.Worlds.Api;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AiAlgorithmsResearch.Core.Combat.Application
{
    internal sealed class BattleInitializer : IBattleInitializer
    {
        private readonly IWorldEditor _worldEditor;

        public BattleInitializer(IWorldEditor worldEditor)
        {
            _worldEditor = worldEditor;
        }

        public IBattle StartBattle(BattleInitializationRequest request, Random random)
        {
            var spawnedEntities = new List<BattleParticipantSetup>();
            var initiativeRoller = new InitiativeRoller(random);

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
                .Select(participant => new BattleParticipant(
                    participant.Entity,
                    initiativeRoller.Roll(),
                    participant.TeamId,
                    participant.ActionDefinitions))
                .OrderByDescending(participant => participant.Initiative)
                .ToList();

            return new Battle(participants);
        }
    }
}