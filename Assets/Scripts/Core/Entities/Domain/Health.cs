using AiAlgorithmsResearch.Core.Entities.Api;
using System;

namespace AiAlgorithmsResearch.Core.Entities.Domain
{
    internal sealed class Health : IHealthView
    {
        public int Current { get; private set; }
        public int Max { get; }

        public Health(int max)
        {
            if (max <= 0)
                throw new ArgumentOutOfRangeException(nameof(max));

            Max = max;
            Current = max;
        }

        public void TakeDamage(int amount)
        {
            if (amount < 0)
                throw new ArgumentOutOfRangeException(nameof(amount));

            Current = Math.Max(0, Current - amount);
        }

        public void Heal(int amount)
        {
            if (amount < 0)
                throw new ArgumentOutOfRangeException(nameof(amount));

            Current = Math.Min(Max, Current + amount);
        }

        public bool IsDead => Current == 0;
    }
}
