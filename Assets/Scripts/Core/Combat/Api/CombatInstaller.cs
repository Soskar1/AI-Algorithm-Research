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
            builder.RegisterFactory<IBattleInitializer>(builder => new BattleInitializer(builder.Resolve<IWorldEditor>()), Lifetime.Singleton, Resolution.Lazy);

            builder.RegisterType(typeof(StunStatus), Lifetime.Singleton, Resolution.Lazy);
            builder.RegisterFactory<IStunStatus>(builder => builder.Resolve<StunStatus>(), Lifetime.Singleton, Resolution.Lazy);
            builder.RegisterFactory<IStunStatusEditor>(builder => builder.Resolve<StunStatus>(), Lifetime.Singleton, Resolution.Lazy);

            builder.RegisterType(typeof(ActionCooldowns), Lifetime.Singleton, Resolution.Lazy);
            builder.RegisterFactory<IActionCooldowns>(builder => builder.Resolve<ActionCooldowns>(), Lifetime.Singleton, Resolution.Lazy);
            builder.RegisterFactory<IActionCooldownEditor>(builder => builder.Resolve<ActionCooldowns>(), Lifetime.Singleton, Resolution.Lazy);

            builder.RegisterFactory<IRuntimeCombatStateFactory>(builder =>
                new RuntimeCombatStateFactory(
                    builder.Resolve<IWorldView>(),
                    builder.Resolve<IWorldEditor>(),
                    builder.Resolve<IEntityHealthEditor>(),
                    builder.Resolve<IEntityEnergyEditor>(),
                    builder.Resolve<IActionCooldowns>(),
                    builder.Resolve<IActionCooldownEditor>(),
                    builder.Resolve<IStunStatusEditor>(),
                    builder.Resolve<IStunStatus>()),
                Lifetime.Singleton, Resolution.Lazy);

            builder.RegisterFactory<ICombatLogger>(builder => new CombatLogger(), Lifetime.Singleton, Resolution.Lazy);
            builder.RegisterFactory<ICombatActionExecutor>(builder =>
            {
                var combatLogger = builder.Resolve<ICombatLogger>();

                return new CombatActionExecutor(
                    new Dictionary<CombatActionId, ICombatActionHandler>()
                    {
                        [CombatActionIds.Wait] = new WaitActionHandler(combatLogger),
                        [CombatActionIds.Move] = new MoveActionHandler(combatLogger),
                        [CombatActionIds.Attack] = new AttackActionHandler(combatLogger),
                        [CombatActionIds.Teleport] = new TeleportActionHandler(combatLogger),
                        [CombatActionIds.Heal] = new HealActionHandler(combatLogger),
                        [CombatActionIds.Stun] = new StunActionHandler(combatLogger)
                    });
            }, Lifetime.Singleton, Resolution.Lazy);
            return builder;
        }
    }
}
