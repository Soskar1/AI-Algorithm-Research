using AiAlgorithmsResearch.Core.Ai.Api;
using AiAlgorithmsResearch.Core.Ai.Application;
using AiAlgorithmsResearch.Core.Combat.Api;
using AiAlgorithmsResearch.Core.Combat.Application;
using AiAlgorithmsResearch.Core.Combat.Domain;
using AiAlgorithmsResearch.Core.Entities.Api;
using AiAlgorithmsResearch.Core.Entities.Application;
using AiAlgorithmsResearch.Core.Maps.Api;
using AiAlgorithmsResearch.Core.Maps.Application;
using AiAlgorithmsResearch.Core.Maps.Domain;
using AiAlgorithmsResearch.Core.Worlds.Api;
using AiAlgorithmsResearch.Core.Worlds.Application;
using AiAlgorithmsResearch.Core.Worlds.Domain;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AiAlgorithmsResearch.Core.Ai.Tests
{
    public sealed class AiEngineTests
    {
        private IMapEditor _mapEditor;
        private IWorldView _worldView;
        private IWorldEditor _worldEditor;
        private IEntityFactory _entityFactory;
        private IBattleInitializer _battleInitializer;
        private IAiEngine _aiEngine;
        private IActionCooldowns _actionCooldowns;

        [SetUp]
        public void SetUp()
        {
            var map = new Map();
            _mapEditor = new MapEditor(map);

            var world = new World(_mapEditor, map);

            _worldView = world;
            _worldEditor = new WorldEditor(world);

            _entityFactory = new EntityFactory();
            _battleInitializer = new BattleInitializer(_worldEditor, new FakeInitiativeRoller());

            _actionCooldowns = new ActionCooldowns();

            var candidateProvider = new CombatActionCandidateProvider(
                new ICombatActionCandidateGenerator[]
                {
                    new MoveActionCandidateGenerator(),
                    new AttackActionCandidateGenerator(),
                    new HealActionCandidateGenerator(),
                    new TeleportActionCandidateGenerator(),
                    new StunActionCandidateGenerator()
                }, _actionCooldowns);

            _aiEngine = new AiEngine(candidateProvider);
        }

        [Test]
        public void ProduceMove_WhenOnlyAttackIsAvailableAndEnemyIsInRange_ReturnsAttackAction()
        {
            var actor = CreateEntity();
            var enemy = CreateEntity();

            var actorActions = new ICombatActionDefinition[]
            {
                new AttackActionDefinition(baseDamage: 5, range: 1)
            };

            var battle = StartBattle(
                new BattleParticipantSetup(
                    actor,
                    new Vector2Int(1, 1),
                    new TeamId(1),
                    actorActions),
                new BattleParticipantSetup(
                    enemy,
                    new Vector2Int(2, 1),
                    new TeamId(2),
                    EmptyActions()));

            var actorParticipant = FindParticipant(battle, actor);
            var context = new CombatAgentContext(actorParticipant, _worldView, battle);

            var action = _aiEngine.ProduceMove(new FirstActionAgent(), context);

            Assert.IsInstanceOf<AttackAction>(action);

            var attack = (AttackAction)action;
            Assert.AreSame(actor, attack.Actor);
            Assert.AreSame(enemy, attack.Target);
            Assert.AreEqual(5, attack.BaseDamage);
            Assert.AreEqual(1, attack.Range);
        }

        [Test]
        public void ProduceMove_WhenNoCandidateActionsExist_ReturnsWaitAction()
        {
            var actor = CreateEntity();

            var actorActions = new ICombatActionDefinition[]
            {
                new AttackActionDefinition(baseDamage: 5, range: 1)
            };

            var battle = StartBattle(
                new BattleParticipantSetup(
                    actor,
                    new Vector2Int(1, 1),
                    new TeamId(1),
                    actorActions));

            var actorParticipant = FindParticipant(battle, actor);
            var context = new CombatAgentContext(actorParticipant, _worldView, battle);

            var action = _aiEngine.ProduceMove(new FirstActionAgent(), context);

            Assert.IsInstanceOf<WaitAction>(action);
            Assert.AreSame(actor, action.Actor);
        }

        [Test]
        public void ProduceMove_WhenMultipleCandidatesExist_PassesThemToAgent()
        {
            var actor = CreateEntity();
            var enemy = CreateEntity();

            var actorActions = new ICombatActionDefinition[]
            {
                new AttackActionDefinition(baseDamage: 5, range: 1),
                new HealActionDefinition(amount: 5)
            };

            var battle = StartBattle(
                new BattleParticipantSetup(
                    actor,
                    new Vector2Int(1, 1),
                    new TeamId(1),
                    actorActions),
                new BattleParticipantSetup(
                    enemy,
                    new Vector2Int(2, 1),
                    new TeamId(2),
                    EmptyActions()));

            var actorParticipant = FindParticipant(battle, actor);
            var context = new CombatAgentContext(actorParticipant, _worldView, battle);

            var agent = new CapturingAgent();

            _aiEngine.ProduceMove(agent, context);

            Assert.AreEqual(2, agent.ReceivedActions.Count);
            Assert.IsTrue(ContainsAction<AttackAction>(agent.ReceivedActions));
            Assert.IsTrue(ContainsAction<HealAction>(agent.ReceivedActions));
        }

        private IBattle StartBattle(params BattleParticipantSetup[] participants)
        {
            foreach (var participant in participants)
            {
                _mapEditor.AddTile(participant.SpawnPosition, MapNodeType.Free);
            }

            return _battleInitializer.StartBattle(
                new BattleInitializationRequest(participants));
        }

        private static IBattleParticipant FindParticipant(
            IBattle battle,
            IEntityView entity)
        {
            return battle.TurnOrder.First(participant =>
                ReferenceEquals(participant.Entity, entity));
        }

        private IEntityView CreateEntity()
        {
            return _entityFactory.CreateEntity(
                new EntityDefinition(
                    maxHealth: 100,
                    maxEnergy: 10,
                    energyRegenerationPerTurn: 3,
                    speed: 1,
                    strength: 2));
        }

        private static IReadOnlyCollection<ICombatActionDefinition> EmptyActions()
        {
            return new ICombatActionDefinition[0];
        }

        private static bool ContainsAction<TAction>(IList<ICombatAction> actions)
            where TAction : ICombatAction
        {
            foreach (var action in actions)
            {
                if (action is TAction)
                    return true;
            }

            return false;
        }

        private sealed class FakeInitiativeRoller : IInitiativeRoller
        {
            public int Roll(IEntityView entity)
            {
                return 10;
            }
        }

        private sealed class FirstActionAgent : ICombatAgent
        {
            public ICombatAction ChooseAction(IList<ICombatAction> actions)
            {
                return actions[0];
            }
        }

        private sealed class CapturingAgent : ICombatAgent
        {
            public IList<ICombatAction> ReceivedActions { get; private set; }

            public ICombatAction ChooseAction(IList<ICombatAction> actions)
            {
                ReceivedActions = actions;
                return actions[0];
            }
        }
    }
}