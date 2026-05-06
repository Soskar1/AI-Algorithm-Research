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

            builder.RegisterFactory<ICombatLogger>(builder => new CombatLogger(builder.Resolve<IWorldView>()), Lifetime.Singleton, Resolution.Lazy);

            builder.RegisterFactory(builder =>
                new RuntimeCombatState(
                    builder.Resolve<IWorldView>(),
                    builder.Resolve<IWorldEditor>(),
                    builder.Resolve<IEntityHealthEditor>(),
                    builder.Resolve<IEntityEnergyEditor>(),
                    builder.Resolve<IActionCooldowns>(),
                    builder.Resolve<IActionCooldownEditor>(),
                    builder.Resolve<IStunStatusEditor>()
                ), Lifetime.Singleton, Resolution.Lazy);

            builder.RegisterFactory<ICombatStateView>(builder => builder.Resolve<RuntimeCombatState>(), Lifetime.Singleton, Resolution.Lazy);
            builder.RegisterFactory<ICombatStateEditor>(builder => builder.Resolve<RuntimeCombatState>(), Lifetime.Singleton, Resolution.Lazy);

            builder.RegisterFactory<ICombatActionExecutor>(builder =>
            {
                var combatStateView = builder.Resolve<ICombatStateView>();
                var combatStateEditor = builder.Resolve<ICombatStateEditor>();
                var combatLogger = builder.Resolve<ICombatLogger>();

                return new CombatActionExecutor(
                    new Dictionary<CombatActionId, ICombatActionHandler>()
                    {
                        [CombatActionIds.Wait] = new WaitActionHandler(combatLogger),
                        [CombatActionIds.Move] = new MoveActionHandler(combatStateView, combatStateEditor, combatLogger),
                        [CombatActionIds.Attack] = new AttackActionHandler(combatStateView, combatStateEditor, combatLogger),
                        [CombatActionIds.Teleport] = new TeleportActionHandler(combatStateView, combatStateEditor, combatLogger),
                        [CombatActionIds.Heal] = new HealActionHandler(combatStateView, combatStateEditor, combatLogger),
                        [CombatActionIds.Stun] = new StunActionHandler(combatStateView, combatStateEditor, combatLogger)
                    }, combatStateView, combatStateEditor);
            }, Lifetime.Singleton, Resolution.Lazy);

            return builder;
        }
    }
}
