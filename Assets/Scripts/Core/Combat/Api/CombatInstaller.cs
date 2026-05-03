using AiAlgorithmsResearch.Core.Combat.Application;
using AiAlgorithmsResearch.Core.Combat.Domain;
using AiAlgorithmsResearch.Core.Entities.Api;
using AiAlgorithmsResearch.Core.Worlds.Api;
using Reflex.Core;
using Reflex.Enums;
using System.Collections.Generic;

namespace AiAlgorithmsResearch.Core.Combat.Api
{
    public static class CombatInstaller
    {
        public static ContainerBuilder InstallCombat(this ContainerBuilder builder)
        {
            builder.RegisterFactory<IInitiativeRoller>(builder => new UnityInitiativeRoller(), Lifetime.Singleton, Resolution.Lazy);
            builder.RegisterFactory<IBattleInitializer>(builder =>
                new BattleInitializer(
                    builder.Resolve<IWorldEditor>(),
                    builder.Resolve<IInitiativeRoller>()),
                Lifetime.Singleton, Resolution.Lazy);

            builder.RegisterType(typeof(StunStatus), Lifetime.Singleton, Resolution.Lazy);
            builder.RegisterFactory<IStunStatus>(builder => builder.Resolve<StunStatus>(), Lifetime.Singleton, Resolution.Lazy);
            builder.RegisterFactory<IStunStatusEditor>(builder => builder.Resolve<StunStatus>(), Lifetime.Singleton, Resolution.Lazy);

            builder.RegisterType(typeof(ActionCooldowns), Lifetime.Singleton, Resolution.Lazy);
            builder.RegisterFactory<IActionCooldowns>(builder => builder.Resolve<ActionCooldowns>(), Lifetime.Singleton, Resolution.Lazy);
            builder.RegisterFactory<IActionCooldownEditor>(builder => builder.Resolve<ActionCooldowns>(), Lifetime.Singleton, Resolution.Lazy);

            builder.RegisterFactory<ICombatActionExecutor>(builder =>
            {
                var worldView = builder.Resolve<IWorldView>();
                var worldEditor = builder.Resolve<IWorldEditor>();
                var healthEditor = builder.Resolve<IEntityHealthEditor>();
                var stunStatusEditor = builder.Resolve<IStunStatusEditor>();
                var energyEditor = builder.Resolve<IEntityEnergyEditor>();
                var actionCooldowns = builder.Resolve<IActionCooldowns>();
                var actionCooldownsEditor = builder.Resolve<IActionCooldownEditor>();

                return new CombatActionExecutor(
                    new Dictionary<CombatActionId, ICombatActionHandler>()
                    {
                        [CombatActionIds.Wait] = new WaitActionHandler(),
                        [CombatActionIds.Move] = new MoveActionHandler(worldView, worldEditor),
                        [CombatActionIds.Attack] = new AttackActionHandler(worldView, healthEditor),
                        [CombatActionIds.Teleport] = new TeleportActionHandler(worldView, worldEditor),
                        [CombatActionIds.Heal] = new HealActionHandler(healthEditor),
                        [CombatActionIds.Stun] = new StunActionHandler(worldView, stunStatusEditor)
                    }, energyEditor, actionCooldowns, actionCooldownsEditor);
            }, Lifetime.Singleton, Resolution.Lazy);

            return builder;
        }
    }
}
